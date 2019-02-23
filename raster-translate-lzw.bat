@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET OutExt=.tif
IF "%SrcExt%"==".tif" SET OutExt=-lzw.tif
IF "%SrcExt%"==".TIF" SET OutExt=-lzw.tif
IF "%SrcExt%"==".tiff" SET OutExt=-lzw.tif
IF "%SrcExt%"==".TIFF" SET OutExt=-lzw.tif

gdal_translate %TiffOpts%  "%1" "%SrcDirName%%OutExt%"

echo.
echo %~n0 finished.
timeout 30