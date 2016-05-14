@echo off
call "%~dp0\bin\o4w_env.bat"


gdalinfo -proj4 %1

echo.
pause