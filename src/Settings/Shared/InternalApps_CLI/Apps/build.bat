@echo off

SET config="../../../../../GCModeller/bin/Settings"
SET output="./"

REM run GCModeller config tools for generates 
REM the CLI source invoke automatically.
%config% /dev /out %output%