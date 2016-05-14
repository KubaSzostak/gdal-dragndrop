echo.

REM Set RAM in MB for read/write caching
REM the same is used also to set RAM in MB for wrap operations
SET GDAL_CACHEMAX=512
SET GDAL_NUM_THREADS=ALL_CPU

:: %1 = C:\UMGDY\Lidar2015\N-33-48-C-b-2-3.las

SET SrcDir=%~dp1
:: C:\UMGDY\Lidar2015\
 
SET SrcName=%~n1
:: N-33-48-C-b-2-3
 
SET SrcExt=%~x1
:: .las

CD /D %SrcDir%


:: PREDICTOR for 56.9MB DTM file
:: PREDICTOR=1: 52.3MB
:: PREDICTOR=2: 31.8MB
:: PREDICTOR=3: 60.2MB

SET TiffOpts=-of GTiff -co COMPRESS=LZW -co BIGTIFF=YES -co TILED=YES  -co PREDICTOR=2
SET JpegOpts=-co compress=lzw -co photometric=ycbcr -co JPEG_QUALITY=85

SET StartTime=%TIME%
set /A WorkingTimeMin= (%TIME%-%STARTTIME%)*0.000166667

call "%~dp0\OSGeo4W\bin\o4w_env.bat"


