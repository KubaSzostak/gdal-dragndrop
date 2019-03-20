import sys
import time
import raster_utils as rutils
from osgeo import gdal

stime = time.time()
raster = rutils.Raster(sys.argv[1], True, True)
raster.assert_datatype(gdal.GDT_Byte, "Unable to update NoData value")    

if raster.nodata_exists and False:
    print "NoData value already exists: NoData = ", raster.nodata_text
    overridde = raw_input("Do you want to estimate new NoData value and overridde existing NoData value? [y/n]: ")
    if overridde != "y":
        print "NoData value was unchanged."
        sys.exit(0)

print "Estimating NoData value...\n"
rvalset = rutils.RasterValSet()
for band in raster.bands(True):
    rvalset.addband(band)
    band = None

print ""
estimated_nodata = rvalset.estimate_nodata()
if estimated_nodata == None:
    print "ERROR: Cannot estimate NoData value - all valueas are already used."
    sys.exit(1)

print "Changing NoData value to", estimated_nodata, "..."
raster.set_nodata(estimated_nodata)
print "NoData value changed."


print("--- %s seconds ---" % round(time.time() - stime, 1))
raster = None