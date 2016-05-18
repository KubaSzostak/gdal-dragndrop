@echo off
echo *** %~n0 ...
echo *** %1
echo.
call %~dp0\_init.bat %1 %2 %3

SET OutDir=xyz-sort


if not exist %OutDir% mkdir %OutDir%

for %%v in (*%SrcExt%) do (
   echo %%v
   %~dp0\dragndrop\xyz_sort.exe %%v %OutDir%\%%v
   echo.
) 


echo.
echo %~n0 finished.
timeout 30
