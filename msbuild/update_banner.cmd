@echo off

SET R_proj=../src/R-sharp
SET R_HOME=%R_proj%/App/net6.0
SET REnv="%R_HOME%/R#.exe"
SET updater=%R_proj%/studio/code_banner.R

%REnv% %updater% --banner-xml ../gpl3.xml --proj-folder ../src/GCModeller/

git add -A
git commit -m "update source file banner headers![GCModeller Internal Modules]"

%REnv% %updater% --banner-xml ../gpl3.xml --proj-folder ../src/interops/

git add -A
git commit -m "update source file banner headers![GCModeller External Tools Interop Code]"

%REnv% %updater% --banner-xml ../gpl3.xml --proj-folder ../src/repository/

git add -A
git commit -m "update source file banner headers![GCModeller Internal Data Repository]"

%REnv% %updater% --banner-xml ../gpl3.xml --proj-folder ../src/Settings/
%REnv% %updater% --banner-xml ../gpl3.xml --proj-folder ../src/workbench/

git add -A
git commit -m "update source file banner headers![GCModeller Workbench Module Code]"

pause