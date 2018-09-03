@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3
::-tr 0.6 0.6  -srcnodata 255 -vrtnodata 0 -r average
gdalbuildvrt %2 _merge.vrt -overwrite *%SrcExt%

echo.
echo %~n0 finished.
timeout 30