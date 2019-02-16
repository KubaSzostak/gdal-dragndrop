@echo off
echo *** %~n0 ...
echo.
call %~dp0\_init.bat %1 %2 %3

SET ShpFile=%~dpn1-cutline.shp
if NOT "%2"=="" SET ShpFile=%2

SET TiffOut=%~dpn1-crop.tif
if NOT "%2"=="" SET TiffOut=%~dpn1-%~n2.tif


echo Cutting shapefile: %ShpFile%
echo Input TIFF:        %1
echo Output TIFF:       %TiffOut%

echo.

gdalwarp -srcnodata 255 -dstnodata 0 -r average -wm %GDAL_CACHEMAX% -multi %TiffOpts% -cutline %ShpFile% -crop_to_cutline %1 %TiffOut%


echo.
echo %~n0 finished.
timeout 30
