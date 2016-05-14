@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET Compression=LZW
if NOT "%2"=="" SET Compression=%2

gdaladdo -ro -r average --config COMPRESS_OVERVIEW %Compression% --config PREDICTOR_OVERVIEW 2 --config BIGTIFF_OVERVIEW YES %1 2 4 8 16 32 64 128 256 512 1024 2048


echo %~n0 finished.
echo %1.ovr
timeout 30
echo.
