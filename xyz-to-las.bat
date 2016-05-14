@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3


for %%v in (%SrcDir%*%SrcExt%) do (
   echo %%v
   txt2las -parse xyz -olas -i %%v
   echo.
)



echo.
echo %~n0 finished.
timeout 30