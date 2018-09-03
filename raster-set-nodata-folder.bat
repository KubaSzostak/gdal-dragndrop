@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

:: Black: 0, White: 255, Float: -9999.0

for %%v in (*%SrcExt%) do (
   echo %%v
   :: gdal_edit -a_nodata 255 %%v
   %~dp0\raster-set-nodata-white.bat %%v
   echo.

) 

timeout 30