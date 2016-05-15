@echo off
echo ### %~n0 ...
echo %1
echo.
call %~dp0\_init.bat %1 %2 %3



for %%v in (*%SrcExt%) do (
   call %~dp0\xyz-to-tif.bat %%v
)



echo.
echo %~n0 finished.
timeout 30
