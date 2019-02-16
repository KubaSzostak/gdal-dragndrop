@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET OutDir=pct2rgb
SET RgbaOpts= -rgba
SET TiffOpts=-of GTiff 

if not exist %OutDir% mkdir %OutDir%

for %%v in (*%SrcExt%) do (
   echo %%v
   ::echo pct2rgb.py %TiffOpts% %RgbaOpts% %%v %OutDir%\%%v
   ::python.exe %~dp0\OSGeo4W\bin\pct2rgb.py %TiffOpts% %RgbaOpts% %%v %OutDir%\%%v   
   gdal_translate %TiffOpts% %%v %OutDir%\%%v
   echo.
) 


echo.
echo %~n0 finished.
timeout 30
