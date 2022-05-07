@echo off

SET R_HOME=../../../R-sharp\App\net5.0
SET Rscript="%R_HOME%/Rscript.exe"
SET R="%R_HOME%/R#.exe"

%Rscript% --build /src ../ /save ./GCModeller.NET5.zip

REM install local
%R% --install.packages ./GCModeller.NET5.zip

pause