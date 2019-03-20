import sys
import collections
import ntpath
import time
import numpy as np
from osgeo import gdal


# DOCS
# https://gdal.org/python/
# https://www.gdal.org/classGDALRasterBand.html
# https://pcjericks.github.io/py-gdalogr-cookbook/raster_layers.html
# https://gist.github.com/CMCDragonkai/ac6289fa84bcc8888035744d7e00e2e6
# https://gdal.org/python/osgeo.gdalconst-module.html


class ProgressBar(object):

    def __init__(self, maxval=100):
        self.currval = 0
        self.maxval = int(maxval)
        self.last_progress = 0
        sys.stdout.write("0")
        sys.stdout.flush()

    def next(self, val = None):
        if val:
            self.currval = val
        else:
            self.currval = self.currval + 1
        if self.currval > self.maxval - 1:
            self.currval = self.maxval - 1
        progress = round(20 * self.currval / self.maxval)

        while self.last_progress < progress:
            self.last_progress = self.last_progress + 1
            self._print_progress(self.last_progress)

    def finish(self):
        while self.last_progress < 20:
            self.last_progress = self.last_progress + 1
            self._print_progress(self.last_progress)
        sys.stdout.write("\n")
        sys.stdout.flush()

    def _print_progress(self, progress):
        if progress % 5 == 0:
            percent = int(progress * 5)
            sys.stdout.write(str(percent))
        else:
            sys.stdout.write(".")
        sys.stdout.flush()



class Progressable(object):

    def __init__(self, maxval = None):
        if maxval:
            self.progress = ProgressBar(maxval)

    def progress_next(self, val = None):
        if self.progress:
            self.progress.next(val)

    def progress_finish(self):
        if self.progress:
            self.progress.finish()



class RasterValSet(set):

    def addarr(self, arr):
        uvals = np.unique(arr)
        self.update(uvals)

    def addband(self, band, verbose = True):
        for block in BandBlocks(band, verbose):
            self.addarr(block.arr)

    def estimate_nodata(self):
        for val in range(128):
            if not val in self:
                return val
            # Check from both sided (0...255 => black...white)
            rval = 255 - val 
            if not val in self:
                return rval
        return None   



# ----------------------------------------------------------------------------------------------------

GDT_IntTypes = {gdal.GDT_Byte, gdal.GDT_CInt16, gdal.GDT_CInt32, gdal.GDT_Int16, gdal.GDT_Int32, gdal.GDT_UInt16, gdal.GDT_UInt32, gdal.GDT_TypeCount}

gdal.UseExceptions()

class Raster(object):

    def __init__(self, filepath, verbose = True, update = False):
        if not filepath:
            print "Unable to open raster: filepath was not set"
            sys.exit(1) 

        mode = gdal.GA_ReadOnly
        if update:
            mode = gdal.GA_Update
        self.ds = gdal.Open(filepath, mode)
        self.filepath = filepath
        self.filename = ntpath.basename(filepath)
        
        if self.ds is None:
            print "Unable to open raster:"
            print self.filepath
            sys.exit(1) 

        self.nodata = []
        self.nodata_text = ""
        self.nodata_exists = False
        self.colortable_info = None
        self.overview_count = 0
        datatype = None
        for band in self.bands():
            datatype = band.DataType
            nodata = band.GetNoDataValue()
            self.nodata_exists = self.nodata_exists or nodata != None
            if nodata != None and band.DataType in GDT_IntTypes:
                nodata = int(nodata)
            self.nodata.append(nodata)
            self.nodata_text = self.nodata_text + str(nodata) + "   "

            ctable = band.GetColorTable()
            if ctable:
                palette = ctable.GetPaletteInterpretation()
                self.colortable_info = gdal.GetColorInterpretationName(palette) + " (" + str(ctable.GetCount()) + " entries)"
            if band.GetOverviewCount() > self.overview_count:
                self.overview_count = band.GetOverviewCount()

        (self.xoff, self.xsize, self.xrotation, self.yoff, self.yrotation, self.ysize) = self.ds.GetGeoTransform()

        print self.filename
        print "-" * len(self.filename)
        print "  Driver:     ", self.ds.GetDriver().LongName # raster.GetDriver().ShortName, 
        print "  Size:       ", self.ds.RasterXSize, "x", self.ds.RasterYSize, "x", self.ds.RasterCount, "Bands", "(" + gdal.GetDataTypeName(datatype) + ")"
        print "  NoData:     ", self.nodata_text
        print "  Color table:", self.colortable_info
        print "  Pyramids:   ", self.overview_count
        print ""


    def geo_coords(self, rx, ry):
        """Returns global coordinates from local raster rx, ry indexes """
        gx = self.xoff  +  rx * self.xsize  +  ry * self.xrotation  +  self.xsize * 0.5 # add half the cell size
        gy = self.yoff  +  ry * self.ysize  +  rx * self.yrotation  +  self.ysize * 0.5 # to centre the point
        return (gx, gy)

    def raster_coords(self, gx, gy):
        """Returns local raster coordinates from global gx, gy coords"""  

        # Convert from map to pixel coordinates.
        # Only works for geotransforms with no rotation.
        rx = int((gx - self.xoff) / self.xsize) #x pixel
        ry = int((gy - self.yoff) / self.ysize) #y pixel
        return rx, ry

        # structval=rb.ReadRaster(px,py,1,1,buf_type=gdal.GDT_UInt16) #Assumes 16 bit int aka 'short'
        # intval = struct.unpack('h' , structval) #use the 'short' format code (2 bytes) not int (4 bytes)
        # print intval[0] #intval is a tuple, length=1 as we only asked for 1 pixel value

    def bands(self, verbose = False):
        for band_no in range(self.ds.RasterCount):
            band_no += 1
            band = self.ds.GetRasterBand(band_no)
            if band is None:            
                print "Unable to open raster band:", band_no
                print self.filename
                sys.exit(1)       
            if verbose:
                print "Processing band", band_no, ":"
            yield band


    def blocks(self, verbose = True):        
        for band in self.bands(verbose):
            for block in BandBlocks(band, verbose):
                yield block
            band = None

    def set_nodata(self, value):        
        for band in self.bands():
            if band.GetNoDataValue() != value:
                band.SetNoDataValue(value)
                band.FlushCache()
                band = None

    def flush_cache(self, verbose = True): 
        if verbose:   
            sys.stdout.write("\nSaving raster...  ")
        self.ds.FlushCache()
        for band in self.bands():
            band.FlushCache()
        if verbose:
            sys.stdout.write("Saved.  \n\n")

        
    def assert_datatype(self, gdt = gdal.GDT_Byte, errmsg = "Invalid raster type"):
        for band in self.bands():
            if band.DataType != gdt:
                print "ERROR: " + errmsg
                print "Expected '" + gdal.GetDataTypeName(gdt) + "' raster type but '" + gdal.GetDataTypeName(band.DataType) + "' raster was used."
                print self.filename
                sys.exit(1)

    def assert_int(self, errmsg = "Invalid raster type"):
        for band in self.bands():
            if band.DataType in GDT_IntTypes:
                print "ERROR: " + errmsg
                print "Expected integer raster type but '" + gdal.GetDataTypeName(band.DataType) + "' raster was used."
                print self.filename
                sys.exit(1)



class BandBlock(object):

    def __init__(self, band, x, y, cols, rows):
        if x < 0 or y < 0 or x > band.XSize - 1 or y > band.YSize - 1:
            print "Invalid BandBlock parameters:"
            print "  x, y:", x, y
            print "  Band size:", band.XSize, band.YSize
            sys.exit(1)

        if cols < 1 or rows < 1  or cols > band.XSize or rows > band.YSize: 
            print "Invalid BandBlock parameters:"
            print "  cols, rows:", cols, rows
            print "  Band size:", band.XSize, band.YSize
            sys.exit(1)

        self.band = band
        self.x = x
        self.y = y
        self.cols = cols
        self.rows = rows
        self.arr = band.ReadAsArray(x, y, cols, rows)

    def write(self):
        self.band.WriteArray(self.arr, self.x, self.y)

    def reclassify_val(self, oldval, newval):        
        if oldval in self.arr:
            self.arr[np.where(self.arr == oldval)] = newval
            self.write()
            #self.band.FlushCache()

    def reclassify_arr(self, oldvals, newvals):
        changed = False
        for i in range(len(oldvals)):
            oldval = oldvals[i]
            if oldval in self.arr:
                self.arr[np.where(self.arr == oldval)] = newvals[i]
                changed = True
        if changed:
            self.write()




class BandBlocks(object):

    def __init__(self, band, verbose = False):
        self.band = band
        self.x = 0
        self.y = 0
        self.block_xsize = band.GetBlockSize()[0] * 32
        self.block_ysize = band.GetBlockSize()[1] * 32
        if verbose:
            self.progress = ProgressBar(band.YSize)

    def __iter__(self):
        return self

    def next(self): # Python 3: def __next__(self)
        # https://gis.stackexchange.com/q/172666        

        if self.x + self.block_xsize >= self.band.XSize and self.y + self.block_ysize >= self.band.YSize:
            if self.progress:
                self.progress.finish()
            raise StopIteration

        y = self.y
        rows = self.block_ysize
        if self.y + rows > self.band.YSize:
            rows = self.band.YSize - self.y
            
        x = self.x
        cols = self.block_xsize
        if self.x + self.block_xsize > self.band.XSize:
            cols = self.band.XSize - self.x

        if self.x + self.block_xsize < self.band.XSize:
            self.x = self.x + self.block_xsize
        else:
            self.x = 0
            self.y = self.y + self.block_ysize
            if self.progress:
                self.progress.next(self.y)
        
        #print 
        #print "self.x,y:", self.x, self.y
        #print "     x,y:", x, y
        #print "self.block size:", self.block_xsize, self.block_ysize
        #print "     cols, rows:", cols, rows
        return BandBlock(self.band, x, y, cols, rows)
        










