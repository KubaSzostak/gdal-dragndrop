@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

REM SET PixelSize=-ps 0.25 .25
REM SET PixelSize=-tr 0.6 0.6

echo This tool is obolete: it raises MemoryError for Float32 rasters larger than 2GB
echo Use raster-merge.bat instead.
echo. 

SET TifOutFile=_merge.tif
SET TifSrcList=_merge-list.txt


IF EXIST %TifOutFile% (
  echo %TifOutFile% already exists. It will be used to generate mosaic.
  pause
)

dir /b *%SrcExt% > %TifSrcList%

 
gdal_merge.bat  %PixelSize% -v %TiffOpts% -o %TifOutFile% --optfile %TifSrcList%


%~dp0\raster-pyramid.bat %TifOutFile%
%~dp0\raster-calc-stats.bat %TifOutFile%

echo.
echo %~n0 finished.
timeout 30
