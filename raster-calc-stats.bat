@echo off
echo *** %~n0 ...
echo *** %1
call %~dp0\_init.bat %1 %2 %3

call gdal_edit -stats %1

echo %~n0 finished.
echo.
echo.
:: timeout 30