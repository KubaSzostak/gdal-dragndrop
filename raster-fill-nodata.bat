@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


echo.
echo This tool replaces file in place
echo %1
echo.
echo Are You sure?

timeout 30

SET MaxDistPixels=7
gdal_fillnodata -si 2 -md %MaxDistPixels% %1 




echo.
echo %~n0 finished.
timeout 30