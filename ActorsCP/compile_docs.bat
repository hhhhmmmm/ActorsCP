@echo off

rem The tool returns an error level of 0 if the search is successful, of 1 if the search is unsuccessful 
rem where doxygen.exe

if errorlevel 0 (
echo 'Doxygen...'
start /wait doxygen.exe ActorsCP_Doxygen_config.txt
) else (
echo not found

)


