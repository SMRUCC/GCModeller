@echo off

SET js_url="https://gcmodeller.org/lib/R_syntax.js"

Rscript --build /src ../  --skip-src-build --github-page %js_url%