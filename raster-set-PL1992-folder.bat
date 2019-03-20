@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

for %%v in (*%SrcExt%) do (
   echo %%v
   python.exe %~dp0\osgeo\bin\gdal_edit.py -a_srs EPSG:2180 %%v
   echo.
) 

timeout 30