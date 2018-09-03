@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET TiffOpts=%TiffOpts% -tr 0.6 0.6 -r average

gdal_translate %TiffOpts% -a_srs EPSG:2180 -a_nodata 255 %1 %SrcDirName%-60cm%SrcExt%

echo.
