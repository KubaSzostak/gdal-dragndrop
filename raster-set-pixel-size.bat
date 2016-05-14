@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


SET PixelSize=0.6

gdalwarp %TiffOpts% -r average -tr %PixelSize% %PixelSize% %1 %~dpn1-%PixelSize%m.tif


echo.
echo %~n0 finished.
timeout 30