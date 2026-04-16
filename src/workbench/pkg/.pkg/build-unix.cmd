@echo off

SET R_HOME=../../../R-sharp\App\net10.0
SET Rscript="%R_HOME%/Rscript.exe"
SET R="%R_HOME%/R#.exe"
SET pkg_name=GCModeller.zip
SET js_url="https://gcmodeller.org/lib/R_syntax.js"

%Rscript% --build /src ../ /save %pkg_name%  --github-page %js_url%

REM install local
%R% --install.packages %pkg_name%

pause