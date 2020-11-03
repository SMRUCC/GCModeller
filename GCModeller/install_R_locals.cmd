@echo off

CD "bin"

REM Install a given list of local system library modules into 
REM the repository database of R# package system.

SET lib=Library

R# install.packages('%lib%/devkit.dll');
R# install.packages('%lib%/R.base.dll');
R# install.packages('%lib%/R.graph.dll');
R# install.packages('%lib%/R.graphics.dll');
R# install.packages('%lib%/R.plot.dll');
R# install.packages('%lib%/R.web.dll');
R# install.packages('%lib%/R.math.dll');
R# install.packages('%lib%/MLkit.dll');

R# --install.packages /module scan=./
R# --install.packages /module scan=%lib%

REM finally, view of the summary information about the installed
REM libraries.
R# installed.packages();

CD ..
