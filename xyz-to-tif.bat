@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

gdal_translate -ot Float32 -stats %TiffOpts%  %1 %SrcDirName%.tif


echo.
echo %~n0 finished.
echo.