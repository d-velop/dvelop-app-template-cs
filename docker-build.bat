@echo off

setlocal enableextensions enabledelayedexpansion

set BUILDCONTAINER=vacationprocess_build_deploy_cs
set BUILDCONTAINER_VERSION=latest


set "denv="
for /f "usebackq delims=" %%a in ("environment") do (
    set "denv=!denv!-e %%a=!%%a! "
)

setlocal disabledelayedexpansion

if "%1"=="it" (
    docker run -it --rm %denv% -v %cd%:/build --entrypoint /bin/bash %BUILDCONTAINER%:%BUILDCONTAINER_VERSION%
) else (
    docker run --rm %denv% -v %cd%:/build %BUILDCONTAINER%:%BUILDCONTAINER_VERSION% %*
)
