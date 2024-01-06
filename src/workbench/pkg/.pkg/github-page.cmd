@echo off

SET js_url="https://rsharp.net/assets/js/R_syntax.js"

Rscript --build /src ../  --skip-src-build --github-page %js_url%