import sys
import time
import raster_utils as rutils
from osgeo import gdal



stime = time.time()
raster = rutils.Raster(sys.argv[1], True, True)
raster.assert_datatype(gdal.GDT_Byte, "Unable to reclassify near NoData value")    



black = 0
near_black = None
if black in raster.nodata:
    print "NoData value is already set to", black, "(black values will be ignored)"
else:
    near_black = black + 1
    while near_black in raster.nodata: # nodata == 1
        near_black = near_black + 1
    print "Reclassify", black, "to", near_black

white = 255
near_white = None
if white in raster.nodata:
    print "NoData value is already set to", white, "(white values will be ignored)"
else:
    near_white = white - 1
    while near_white in raster.nodata: # nodata == 254
        near_white = near_white - 1
    print "Reclassify", white, "to", near_white

print ""

for block in raster.blocks():
    if near_black:
        block.reclassify_val(black, near_black)
    if near_white:
        block.reclassify_val(white, near_white)


if not raster.nodata_exists:
    print "Changing NoData value to", black, "..."
    raster.set_nodata(black)
    print "NoData value changed."


raster.flush_cache()
print("--- %s seconds ---" % round(time.time() - stime, 1))
raster = None