@echo off

SET R_HOME=../../../R-sharp\App\net6.0
SET Rscript="%R_HOME%/Rscript.exe"
SET R="%R_HOME%/R#.exe"

%Rscript% --build /src ../ 

REM install local
%R% --install.packages ../../../workbench/GCModeller_1.1.0-beta.zip

pause