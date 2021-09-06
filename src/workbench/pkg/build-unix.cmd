@echo off

SET Rscript="../../R-sharp\App\net5.0\Rscript.exe"
SET R="../../R-sharp\App\net5.0\R#.exe"

%Rscript% --build

REM install local
%R% --install.packages ../../workbench/GCModeller_1.1.0-beta.zip

pause