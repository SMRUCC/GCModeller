@echo off

SET js_url="https://gcmodeller.org/lib/R_syntax.js"
SET Rscript="\GCModeller\src\R-sharp\App\net6.0\Rscript.exe"

%Rscript% --build /src ../  --skip-src-build --github-page %js_url%