@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

python.exe %~dp0\dndpy\%~n0.py %1

pause
