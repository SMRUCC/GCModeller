@echo off

set dotnet_ver=net10.0
SET js_url="https://gcmodeller.org/lib/R_syntax.js"
SET Rscript="\GCModeller\src\R-sharp\App/%dotnet_ver%/Rscript.exe"

%Rscript% --build /src ../  --skip-src-build --github-page %js_url%