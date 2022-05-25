@echo off

SET R_HOME=D:\GCModeller\src\R-sharp\App\net6.0
SET Rscript="%R_HOME%\Rscript.exe"
SET REnv="%R_HOME%\R#.exe"

%Rscript% --build /src ../ /save ./kegg_graphquery.zip
%REnv% --install.packages "./kegg_graphquery.zip"

pause