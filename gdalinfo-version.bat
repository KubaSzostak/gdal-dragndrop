@echo off

CALL "%~dp0\OSGeo4W\bin\o4w_env.bat"
gdalinfo --version

python --version

pause