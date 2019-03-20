@echo off
echo *** %~n0 ...
echo *** %1
call %~dp0\_init.bat %1 %2 %3

SET Compression=LZW
if NOT "%2"=="" SET Compression=%2

::  PredictorSetup:Horizontal differencing "Predictor" not supported with 64-bit samples

gdaladdo -clean %1
gdaladdo -ro -r average --config GDAL_TIFF_OVR_BLOCKSIZE 512 --config COMPRESS_OVERVIEW %Compression% --config BIGTIFF_OVERVIEW YES %1


echo %~n0 finished.
echo %1.ovr
::timeout 3
echo.
