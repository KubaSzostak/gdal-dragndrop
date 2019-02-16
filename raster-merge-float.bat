@echo off

call %~dp0\_init.bat %1 %2 %3
SET MergeParams=-vrtnodata -9999.0 

gdalbuildvrt %MergeParams% _merge.vrt *%SrcExt%

gdal_translate %TiffOpts%  _merge.vrt _merge.tif

call %~dp0\raster-pyramid.bat _merge.tif

%~dp0\raster-calc-stats.bat _merge.tif