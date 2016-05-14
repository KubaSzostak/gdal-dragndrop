@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdal_translate %TiffOpts%  %1 %1_lzw.tif

echo.
echo %~n0 finished.
timeout 30