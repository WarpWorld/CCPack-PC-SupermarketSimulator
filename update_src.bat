@echo off
REM Update the src subtree from WarpWorld/BepinEx-Example-Plugin

REM Ensure working tree is clean
for /f "delims=" %%i in ('git status --porcelain') do (
    echo.
    echo Your working tree is not clean.
    echo Please commit, stash, or clean your changes before pulling updates.
    pause
    exit /b 1
)

REM Ensure src exists
if not exist src (
    echo.
    echo The folder "src" does not exist.
    echo Run create-subtree.bat first.
    pause
    exit /b 1
)

REM Ensure remote exists
git remote get-url bepinex-example >nul 2>&1
if errorlevel 1 (
    echo.
    echo Remote "bepinex-example" not found.
    echo Please add it with:
    echo   git remote add bepinex-example https://github.com/WarpWorld/BepinEx-Example-Plugin.git
    pause
    exit /b 1
)

REM Fetch and pull updates
echo.
echo Fetching updates from upstream...
git fetch bepinex-example
if errorlevel 1 (
    echo.
    echo Failed to fetch from upstream. Check your network or remote URL.
    pause
    exit /b 1
)

echo.
echo Pulling updates into src...
git subtree pull --prefix=src bepinex-example main --squash
if errorlevel 1 (
    echo.
    echo Failed to pull updates into src.
    echo You may need to resolve conflicts manually.
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
echo Updating ConnectorLib.JSON submodule...
git submodule add https://github.com/WarpWorld/ConnectorLib.JSON.git src/ConnectorLib.JSON 2>nul
git submodule update --init --recursive

echo.
echo Subtree update complete with ConnectorLib.JSON submodule ensured.
pause
exit /b 1