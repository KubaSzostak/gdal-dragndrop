@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET OutDir=out-translate



CD /D %SrcDir%
if not exist %OutDir% mkdir %OutDir%

for %%v in (*%SrcExt%) do (
   echo %%v
   gdal_translate %TiffOpts% %%v %OutDir%\%%v
   echo.
) 


echo.
echo %~n0 finished.
timeout 30
