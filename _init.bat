echo.

REM Set RAM in MB for read/write caching
REM the same is used also to set RAM in MB for wrap operations
SET GDAL_CACHEMAX=512
SET GDAL_NUM_THREADS=ALL_CPUS

:: %1 = C:\UMGDY\Lidar2015\N-33-48-C-b-2-3.las

SET SrcDir=%~dp1
:: C:\UMGDY\Lidar2015\
 
SET SrcName=%~n1
:: N-33-48-C-b-2-3
 
SET SrcExt=%~x1
:: .las

SET SrcDirName=%~dpn1
:: C:\UMGDY\Lidar2015\N-33-48-C-b-2-3

CD /D %SrcDir%

:: PREDICTOR for 56.9MB DTM file
:: - PREDICTOR=1: 52.3MB
:: - PREDICTOR=2: 31.8MB
:: - PREDICTOR=3: 60.2MB
:: ERROR 1: PredictorSetup:Horizontal differencing "Predictor" not supported with 64-bit samples 
::   (This error can only happen with PREDICTOR=2 as a GTiff creation option.)
::   '-co PREDICTOR=2' option removed

SET TiffOpts=-of GTiff -co COMPRESS=LZW -co BIGTIFF=YES -co TILED=YES  
SET JpegOpts=-co compress=lzw -co photometric=ycbcr -co JPEG_QUALITY=85

CALL "%~dp0\osgeo\bin\o4w_env.bat"


