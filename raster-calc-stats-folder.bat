@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

del _%~n0-errors.log

2>_%~n0-errors.log (

  for %%v in (*%SrcExt%) do (
     %~dp0\raster-calc-stats.bat %%v
  )

)


 
echo.
echo %~n0 finished.
timeout 30
