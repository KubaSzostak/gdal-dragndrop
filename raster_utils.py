import sys
import ntpath
import time
import numpy as np
from osgeo import gdal


# DOCS
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



class RasterValSet(set):

    def addarr(self, arr):
        uvals = np.unique(arr)
        self.update(uvals)

    def addband(self, band, verbose = True):
        for blockarr in band_blocks(band, verbose):
            self.addarr(blockarr)
            del blockarr
                    
    def addraster(self, raster):
        for band in raster_bands(raster):
            self.addband(band)

    def estimate_nodata(self):
        for val in range(128):
            if not val in self:
                return val
            # Check from both sided (0...255 => black...white)
            rval = 255 - val 
            if not val in self:
                return rval
        return None   



GDT_IntTypes = {gdal.GDT_Byte, gdal.GDT_CInt16, gdal.GDT_CInt32, gdal.GDT_Int16, gdal.GDT_Int32, gdal.GDT_UInt16, gdal.GDT_UInt32, gdal.GDT_TypeCount}

def open_raster(filepath, update = False):
    mode = gdal.GA_ReadOnly
    if update:
        mode = gdal.GA_Update
    raster = gdal.Open(filepath, mode)
    fname = ntpath.basename(filepath)
    
    if raster is None:
        print "Unable to open", filepath
        sys.exit(1) 

    print fname
    print "-" * len(fname)
    print "  Driver:", raster.GetDriver().LongName # raster.GetDriver().ShortName, 
    print "  Size:  ", raster.RasterXSize, "x", raster.RasterYSize
    print "  Bands: ", raster.RasterCount
    print "  NoData:", raster_nodata(raster)
    print ""

    return raster
                                    


def raster_bands(raster, verbose = False):
    for band_no in range( raster.RasterCount ):
        band_no += 1
        band = raster.GetRasterBand(band_no)
        if band is None:            
            print "Unable to open raster band:", band_no
            sys.exit(1)        

        if verbose:
            print "Processing band:", band_no, "(" + gdal.GetDataTypeName(band.DataType) + ") ..."
        yield band


def band_blocks(band, verbose = False):
    # https://gis.stackexchange.com/q/172666
    block_xsize = band.GetBlockSize()[0] * 32
    block_ysize = band.GetBlockSize()[1] * 32

    progress = ProgressBar(band.YSize)
    for y in xrange(0, band.YSize, block_ysize):
        rows = block_ysize
        if y + block_ysize > band.YSize:
            rows = band.YSize - y

        if verbose:
            progress.next(y)
        for x in xrange(0, band.XSize, block_xsize):
            cols = block_xsize
            if x + block_xsize > band.XSize:
                cols = band.XSize - x
            yield band.ReadAsArray(x, y, cols, rows)
    
    if verbose:
        progress.finish()



def set_nodata_auto(raster):
    existing_nodata = raster_nodata(raster)
    if existing_nodata and False:
        print "NoData value already exists: NoData =", existing_nodata
        overridde = raw_input("Do you want to estimate new NoData value and overridde existing NoData value? [y/n]: ")
        if overridde != "y":
            print "NoData value was not changed."
            sys.exit(0)

    print "Estimating NoData value...\n"
    rvalset = RasterValSet()
    for band in raster_bands(raster, True):
        if band.DataType != 1:
            print "ERROR: Unable to update NoData value"
            print "Expected '" + gdal.GetDataTypeName(1) + "' raster type but '" + gdal.GetDataTypeName(band.DataType) + "' raster was used."
            sys.exit(1)
        rvalset.addband(band)
    
    print ""
    estimated_nodata = rvalset.estimate_nodata()
    if not estimated_nodata:
        print "ERROR: Cannot estimate NoData value - all valueas are already used."
        sys.exit(1)

    if existing_nodata == estimated_nodata:
        print "NoData value is already set to unique raster value:", existing_nodata
        print "NoData value was unchanged."
    else:
        print "Changing NoData value to", estimated_nodata, "..."
        for band in raster_bands(raster):
            band.SetNoDataValue(estimated_nodata)
            band.FlushCache()
        print "NoData value changed."



def set_near_nodata():
    # Reclasify raster faster:
    # https://gis.stackexchange.com/a/177675/26684
    pass


def raster_nodata(raster):
    nodata = None
    for band in raster_bands(raster):
        nodata = band.GetNoDataValue()
        if nodata:
            if band.DataType in GDT_IntTypes:
                nodata = int(nodata)
            return nodata






