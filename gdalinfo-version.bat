@echo off
echo %~n0 
echo %1
echo.
set GDAL_CACHEMAX=1024 :: RAM in MB for read/write caching
call "%~dp0\bin\o4w_env.bat"


gdalinfo --version
pause