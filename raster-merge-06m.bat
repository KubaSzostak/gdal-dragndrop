@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET PixelSize=-tr 0.6 0.6

:: -srcnodata 0.0 -dstnodata -9999.0
 
gdalwarp %PixelSize%  -r average -wm %GDAL_CACHEMAX% -ovr NONE %TiffOpts% *%SrcExt% _merge-60cm.tif


%~dp0\raster-pyramid.bat _merge-60cm.tif
%~dp0\raster-calc-stats.bat _merge-60cm.tif


echo.
echo %~n0 finished.
timeout 30