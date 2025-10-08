@echo off
REM Create or update ConnectorLib.JSON submodule safely (handles: missing folder, dangling folder, already-added-but-not-initialized)

REM -- Config
set SUBPATH=src/ConnectorLib.JSON
set SUBFS=src\ConnectorLib.JSON
set SUBURL=https://github.com/WarpWorld/ConnectorLib.JSON.git

REM -- Ensure working tree is clean (no staged, unstaged, or untracked files)
for /f "delims=" %%i in ('git status --porcelain') do (
    echo.
    echo Your working tree is not clean.
    echo Please commit, stash, or clean your changes before running this script.
    pause
    exit /b 1
)

REM -- Is the submodule tracked in .gitmodules?
git config -f .gitmodules submodule.%SUBPATH%.url >nul 2>&1
if errorlevel 1 (
    REM Not tracked: add it (but warn if a non-empty folder exists)
    if exist "%SUBFS%\" (
        dir /b "%SUBFS%\" | findstr . >nul
        if not errorlevel 1 (
            echo.
            echo WARNING: %SUBFS% contains files and is not tracked as a submodule.
            echo Running this will remove/replace that folder.
            pause
        )
        echo.
        echo Removing existing path to prepare for submodule add...
        git rm --cached -r "%SUBPATH%" >nul 2>&1
        rd /s /q "%SUBFS%" 2>nul
    )

    echo.
    echo Adding ConnectorLib.JSON submodule...
    git submodule add %SUBURL% %SUBPATH%
    if errorlevel 1 (
        echo.
        echo Failed to add submodule. Check the remote URL or .gitmodules state.
        pause
        exit /b 1
    )

    echo.
    echo Initializing submodule...
    git submodule update --init --recursive %SUBPATH%
    if errorlevel 1 (
        echo.
        echo Failed to initialize submodule after add.
        pause
        exit /b 1
    )

    echo.
    echo ConnectorLib.JSON added and initialized.
    pause
    exit /b 0
)

REM -- If tracked, handle three sub-cases:
echo.
echo ConnectorLib.JSON is tracked in .gitmodules.

REM 1) folder missing -> init
if not exist "%SUBFS%\" (
    echo.
    echo Submodule folder missing; initializing...
    git submodule update --init --recursive %SUBPATH%
    if errorlevel 1 (
        echo.
        echo Failed to initialize submodule. Check .gitmodules and remote URL.
        pause
        exit /b 1
    )
    echo.
    echo ConnectorLib.JSON initialized.
    pause
    exit /b 0
)

REM 2) folder exists and contains .git -> already initialized -> update
if exist "%SUBFS%\.git" (
    echo.
    echo ConnectorLib.JSON is initialized; updating...
    git submodule update --init --recursive %SUBPATH%
    if errorlevel 1 (
        echo.
        echo Failed to update submodule. You may need to resolve issues manually.
        pause
        exit /b 1
    )
    echo.
    echo ConnectorLib.JSON updated.
    pause
    exit /b 0
)

REM 3) folder exists but has no .git
dir /b "%SUBFS%\" | findstr . >nul
if not errorlevel 1 (
    echo.
    echo WARNING: %SUBFS% exists but is not a valid git repo and contains files.
    echo Running this will remove that folder and recreate the submodule.
    pause
)

echo.
echo Preparing to recreate ConnectorLib.JSON submodule...
git rm --cached -r "%SUBPATH%" >nul 2>&1
rd /s /q "%SUBFS%" 2>nul

echo.
echo Initializing ConnectorLib.JSON submodule...
git submodule update --init --recursive %SUBPATH%
if errorlevel 1 (
    echo.
    echo Failed to initialize submodule after cleanup.
    pause
    exit /b 1
)

echo.
echo ConnectorLib.JSON is now created or synced.
pause
exit /b 0