@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdalbuildvrt %MergeParams% _merge.vrt *%SrcExt%
call %~dp0\raster-translate-lzw.bat _merge.vrt 

call %~dp0\raster-pyramid.bat _merge.tif

echo.
echo %~n0 finished.
timeout 30