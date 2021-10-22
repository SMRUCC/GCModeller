@echo off

SET root=%CD%
SET base=%CD%/../

SET REnv=%base%/src/R-sharp
SET Darwinism=%base%/src/runtime/Darwinism
SET ggplot=%base%/src/runtime/ggplot
SET jump=run

goto :%jump%

REM ----===== git sync function =====----
:sync_git
SETLOCAL
SET _repo=%1

cd "%_repo%"

echo "git repository directory:"
echo " --> %CD%"

cd "%_repo%/.github"
CALL sync_multiplegit.cmd
cd %base%

:echo
:echo
echo "sync of git repository %_repo% job done!"
echo "---------------------------------------------------------"
:echo
:echo

ENDLOCAL & SET _result=0
goto :%jump%

REM ----===== end of function =====----

:run

SET jump=renv
CALL :sync_git %REnv%
:renv

SET jump=darwinism
CALL :sync_git %Darwinism%
:darwinism

SET jump=ggplot
CALL :sync_git %ggplot%
:ggplot

cd %root%
CALL sync_multiplegit.cmd

pause
exit 0