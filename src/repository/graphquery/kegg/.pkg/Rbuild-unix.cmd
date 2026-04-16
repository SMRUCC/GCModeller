@echo off

SET R_HOME=../../../../R-sharp\App\net10.0
SET Rscript="%R_HOME%\Rscript.exe"
SET REnv="%R_HOME%\R#.exe"
SET pkg=kegg_api.zip

%Rscript% --build /src ../ /save ./%pkg%
%REnv% --install.packages "./%pkg%"

pause