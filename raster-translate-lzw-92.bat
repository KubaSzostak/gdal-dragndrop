@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdal_translate %TiffOpts% -a_srs EPSG:2180  %1 %~dpn1-92.tif

echo.
echo %~n0 finished.
timeout 30