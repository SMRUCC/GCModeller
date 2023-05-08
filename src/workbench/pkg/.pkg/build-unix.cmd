@echo off

SET R_HOME=../../../R-sharp\App\net6.0
SET Rscript="%R_HOME%/Rscript.exe"
SET R="%R_HOME%/R#.exe"
SET pkg_name=GCModeller.zip

%Rscript% --build /src ../ /save %pkg_name%

REM install local
%R% --install.packages %pkg_name%

pause