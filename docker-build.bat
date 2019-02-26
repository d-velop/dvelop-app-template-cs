@echo off

setlocal enableextensions enabledelayedexpansion

set APPNAME=acme-apptemplatecs
set BUILDCONTAINER=registry.invalid/%APPNAME%_build

set "denv="
for /f "usebackq delims=" %%a in ("environment") do (
    set "denv=!denv!-e %%a=!%%a! "
)

echo Building new docker image ...
docker build -t %BUILDCONTAINER% -f ./buildcontainer/Dockerfile . > ./buildcontainer/build.log && (
    echo done
) || (
    echo error building image
    exit /b 1
)

setlocal disabledelayedexpansion

if "%1"=="it" (
    docker run -it --rm %denv% -v %cd%/terraform:/build/terraform --entrypoint /bin/bash %BUILDCONTAINER%
) else (
    docker run --rm %denv% -v %cd%/terraform:/build/terraform %BUILDCONTAINER% %*
)