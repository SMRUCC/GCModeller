@echo off
rem First input is solution path.
rem Use the "~" on the input parameters to strip the existing quotes.
set source="%~1C DLL\bin\%2\SegmentSignal.dll"
set destination="%~1C Sharp Unit Tests\bin\%2\"

echo.
echo Copying C DLL to C Sharp Unit Testing directory.
echo From: %source%
echo To: %destination%
echo.
xcopy %source% %destination% /y
echo.