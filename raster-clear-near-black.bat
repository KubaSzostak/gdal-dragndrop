@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

nearblack %TiffOpts% -o %1-clear-near-black.tif %1

echo.
echo %~n0 finished.
timeout 30