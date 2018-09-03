@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


SET MergeName=_merge-60cm
SET VrtParams=-vrtnodata 255 
SET PixelSize=-tr 0.6 0.6 -r average

gdalbuildvrt %VrtParams% %MergeName%.vrt *%SrcExt%
gdal_translate %TiffOpts% %PixelSize% %MergeName%.vrt %MergeName%.tif

call %~dp0\raster-pyramid.bat %MergeName%.tif
:: %~dp0\raster-calc-stats.bat %MergeName%.tif


echo.
echo %~n0 finished.
timeout 30