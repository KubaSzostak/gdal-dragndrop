import sys
import time
import raster_utils as rutils

stime = time.time()
raster = rutils.open_raster(sys.argv[1], True)
rutils.set_nodata_auto(raster)
print("--- %s seconds ---" % round(time.time() - stime, 1))

raster = None