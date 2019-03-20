from osgeo import ogr
from osgeo import gdal
import os

raster_filepath = r"D:\Work.Dev\GitHub\gdal-dragndrop\N-33-47-D-b-4.tif"

shp_filepath = r"D:\GIS\tiff\topo_10_92_2015-cutline.shp"
shp_ds = ogr.Open(shp_filepath, 0) # 0 means read-only. 1 means writeable.

# Check to see if shapefile is found.
if shp_ds is None:
    print 'Could not open %s' % (shp_filepath)
else:
    print 'Opened %s' % (shp_filepath)
    layer = shp_ds.GetLayer()
    featureCount = layer.GetFeatureCount()
    print "Number of features in %s: %d" % (os.path.basename(shp_filepath),featureCount)


# https://gis.stackexchange.com/questions/46893/getting-pixel-value-of-gdal-raster-under-ogr-point-without-numpy
src_filename = '/tmp/test.tif'
shp_filename = '/tmp/test.shp'

src_ds=gdal.Open(raster_filepath) 
gt=src_ds.GetGeoTransform()
rb=src_ds.GetRasterBand(1)

ds=ogr.Open(shp_filepath)
lyr=ds.GetLayer()
for feat in lyr:
    geom = feat.GetGeometryRef()
    mx,my=geom.GetX(), geom.GetY()  #coord in map units

    #Convert from map to pixel coordinates.
    #Only works for geotransforms with no rotation.
    px = int((mx - gt[0]) / gt[1]) #x pixel
    py = int((my - gt[3]) / gt[5]) #y pixel

    intval=rb.ReadAsArray(px,py,1,1)
    print intval[0] #intval is a numpy array, length=1 as we only asked for 1 pixel value