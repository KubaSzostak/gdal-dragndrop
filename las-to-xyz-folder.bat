@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


for %%v in (%SrcDir%*%SrcExt%) do (
   echo %%v
   las2txt -i %%v -o %%v.xyz 
   echo XYZ saved.
   echo.
)

echo.
echo %~n0 finished.
timeout 30