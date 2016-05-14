@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


for %%v in (%SrcDir%*%SrcExt%) do (
   points2grid --mean --output_format arc --resolution 0.5 -o %%v -i %%v
   echo.
)

echo.
echo %~n0 finished.
timeout 30