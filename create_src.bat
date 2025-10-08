@echo off
REM Create the src subtree from WarpWorld/BepinEx-Example-Plugin

REM Ensure working tree is clean
for /f "delims=" %%i in ('git status --porcelain') do (
    echo.
    echo Your working tree is not clean.
    echo Please commit, stash, or clean your changes before creating subtree.
    pause
    exit /b 1
)

REM Fail if src folder already exists
if exist src (
    echo.
    echo The folder "src" already exists.
    echo Please remove or rename it before running this script.
    pause
    exit /b 1
)

REM Add remote if missing
git remote get-url bepinex-example >nul 2>&1
if errorlevel 1 (
    git remote add bepinex-example https://github.com/WarpWorld/BepinEx-Example-Plugin.git
)

REM Add the subtree
echo.
echo Adding src subtree from upstream...
git subtree add --prefix=src bepinex-example main --squash
if errorlevel 1 (
    echo.
    echo Failed to create subtree.
    pause
    exit /b 1
)

REM Handle ConnectorLib.JSON submodule safely
if exist src\ConnectorLib.JSON (
    echo.
    echo Preparing ConnectorLib.JSON submodule...
    REM Remove index entry if it exists
    git rm --cached -r src/ConnectorLib.JSON >nul 2>&1
    REM Remove folder if empty
    rd /s /q src\ConnectorLib.JSON 2>nul
)

echo.
echo Adding ConnectorLib.JSON submodule...
git submodule add https://github.com/WarpWorld/ConnectorLib.JSON.git src/ConnectorLib.JSON
git submodule update --init --recursive

echo.
echo Subtree and ConnectorLib.JSON submodule created successfully in src\
pause
exit /b 1