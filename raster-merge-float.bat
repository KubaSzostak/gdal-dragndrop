@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdalbuildvrt _merge.vrt *%SrcExt%

%~dp0\raster-translate-lzw.bat _merge.vrt 
%~dp0\raster-pyramid.bat _merge.vrt_lzw.tif
%~dp0\raster-calc-stats.bat _merge.vrt_lzw.tif

echo.
echo %~n0 finished.
timeout 30