@echo off

REM windows batch script for run msbuild of the gcmodeller distribution packages

SET ROOT=%~d0
SET msbuild_logger=%CD%\logs
REM gcmodeller source dir
SET gcmodeller_src=%base%\GCModeller\src
SET jump=null

mkdir %msbuild_logger%

REM if the argument is exists in the commandline
REM then just run build of the R# packages
REM skip build of the .NET 5 assembly files.
if "%1"=="--Rpackage" (
	goto :jump_to_build_Rpackages
)

goto :jump_to_build_clr_asm

REM ----===== msbuild function =====----
:exec_msbuild
SETLOCAL

REM the function accept two required parameters
REM 
REM 1. the relative path of the package source folder
REM 2. the filename of the target VisualStudio solution file to run msbuild
SET _src=%1
SET _sln=%2
SET logfile="%msbuild_logger%\%_sln%.txt"

echo "build %_sln% package"
echo "  --> %_src%"
echo "  --> vs_sln: %_src%\%_sln%"

REM clean works and rebuild libraries
cd %_src%

echo "VisualStudio work folder: %CD%"

dotnet msbuild %_src%\%_sln% -target:Clean
dotnet msbuild %_src%\%_sln% -t:Rebuild /p:Configuration="Rsharp_app_release" /p:Platform="x64" -detailedSummary:True -verbosity:minimal > %logfile% & type %logfile%

@echo:
echo "build package %_sln% job done!"
@echo:
@echo:
@echo:
echo --------------------------------------------------------
@echo:
@echo:

ENDLOCAL & SET _result=0
goto :%jump%

REM ----===== end of function =====----


:jump_to_build_clr_asm

echo "start to run msbuild for the R-sharp environment!"

SET jump=renv
CALL :exec_msbuild %gcmodeller_src%\R-sharp R_system.NET5.sln
:renv

SET jump=gcmodeller
CALL :exec_msbuild %gcmodeller_src%\workbench\R# packages.NET5.sln
:gcmodeller

SET jump=ggplot
CALL :exec_msbuild %gcmodeller_src%\runtime\ggplot ggplot.NET5.sln
:ggplot

echo "do msbuild of the .net 6.0 clr assembly success!"

:jump_to_build_Rpackages

echo "start to run build of the R-sharp packages via Rscript tool!"

SET R_HOME=%gcmodeller_src%\R-sharp\App\net6.0
SET Rscript=%R_HOME%\Rscript.exe
SET REnv=%R_HOME%\R#.exe
SET pkg_release=%~d0\etc\packages_release

mkdir %pkg_release%
goto :r_build_and_install_packages

REM ----===== Rscript build function =====----
:exec_rscript_build
SETLOCAL
SET _src=%1
SET _pkg=%2

echo "build '%_pkg%' package..."
echo "  --> source:  %_src%"
echo "  --> package_release: %pkg_release%\%_pkg%"

%Rscript% --build /src "%_src%" /save "%pkg_release%\%_pkg%" --skip-src-build
%REnv% --install.packages "%pkg_release%\%_pkg%"

@echo:
@echo:
echo "build package %_pkg% job done!"
@echo:
@echo:
@echo:

ENDLOCAL & SET _result=0
goto :%jump%

REM ----===== end of function =====----

:r_build_and_install_packages

SET jump=pkg_renv
CALL :exec_rscript_build "%gcmodeller_src%\R-sharp\REnv\REnv.Rproj" REnv.zip
:pkg_renv

SET jump=pkg_gcmodeller
CALL :exec_rscript_build "%gcmodeller_src%\workbench\pkg\GCModeller.Rproj" GCModeller.zip
:pkg_gcmodeller

SET jump=pkg_ggplot
CALL :exec_rscript_build "%gcmodeller_src%\runtime\ggplot\ggplot.Rproj" ggplot.zip
:pkg_ggplot

echo "build packages job done!"

pause
exit 0