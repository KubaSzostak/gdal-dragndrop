@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


for %%v in (*%SrcExt%) do (
   %~dp0\raster-pyramid.bat %%v
)

 
echo.
echo %~n0 finished.
timeout 30
