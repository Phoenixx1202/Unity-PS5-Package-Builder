@echo off
setlocal
pushd "%~dp0"

echo.
echo =======================================================
echo   Building PS5 PKG (LibProsperoPkg + Publishing Tools)
echo =======================================================
echo.

set "BUILDER_EXE=%~dp0Tools\BuildPs5Pkg\BuildPs5Pkg.exe"

if exist "%BUILDER_EXE%" (
    "%BUILDER_EXE%"
) else (
    echo [ERROR] The precompiled PS5 package builder was not found:
    echo %BUILDER_EXE%
    echo Keep the complete Tools folder in the Unity project root.
    echo.
    pause
    popd
    exit /b 1
)

if %ERRORLEVEL% neq 0 (
    echo.
    echo [ERROR] Failed to build the PKG package.
    echo.
    pause
    popd
    exit /b %ERRORLEVEL%
)

echo.
echo =======================================================
echo   SUCCESS: PKG created in build\Build-pkg\
echo =======================================================
echo.
pause
popd
endlocal
