@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdal_translate -a_nodata none -of XYZ %1 %SrcDirName%.xyz

timeout 30