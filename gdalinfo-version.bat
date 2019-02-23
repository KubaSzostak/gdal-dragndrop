@echo off

CALL "%~dp0\osgeo\bin\o4w_env.bat"
gdalinfo --version

python --version

pause