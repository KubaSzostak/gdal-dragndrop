@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdal_edit -a_nodata 0 %1

timeout 30