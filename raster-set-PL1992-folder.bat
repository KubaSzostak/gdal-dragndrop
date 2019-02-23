@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

for %%v in (*%SrcExt%) do (
   echo %%v
   %~dp0\raster-set-PL1992.bat %%v
   echo.
) 

timeout 30