@echo off
echo ### %~n0 ...
echo %1
echo.
call %~dp0\_init.bat %1 %2 %3



for %%v in (%SrcDir%shp\*.shp) do (
   %~dp0\raster-cut-shp.bat %1 %%v
   echo.
)



echo.
echo %~n0 finished.
timeout 30