@echo off
SET MergeParams=-vrtnodata -9999.0 

%~dp0\raster-merge.bat %1 
%~dp0\raster-calc-stats.bat _merge.tif