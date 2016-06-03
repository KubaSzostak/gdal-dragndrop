@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

:: nmt15.tif -> nmt15-slope.tif
SET OutFile=%SrcDir%%SrcName%-%2%SrcExt%

gdaldem %2 %1 %OutFile% %TiffOpts%
call %~dp0\raster-calc-stats.bat %OutFile%
call %~dp0\raster-pyramid.bat %OutFile%

echo.
echo %~n0 finished.
timeout 30