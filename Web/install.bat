@echo off
REM Cricut Stencil Maker - Simple Batch Installer
REM This is a backup installer method for users who can't run PowerShell

title Cricut Stencil Maker Installer
color 0A

echo.
echo  ░▒▓██████▓▒░░▒▓███████▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░
echo ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
echo ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
echo ░▒▓█▓▒░      ░▒▓███████▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
echo ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
echo ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
echo  ░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓██████▓▒░ ░▒▓██████▓▒░    ░▒▓█▓▒░   
echo.
echo                     STENCIL MAKER v0.9.0
echo         Turn any image into a perfect Cricut stencil instantly!
echo.
echo ════════════════════════════════════════════════════════════════════════════
echo.
echo 🎨 Welcome to the Cricut Stencil Maker installer!
echo    This will download and install everything automatically.
echo.

REM Check Windows version
ver | find "10.0" > nul
if %errorlevel% neq 0 (
    echo ❌ ERROR: Windows 10 or 11 is required!
    echo    Please upgrade your Windows version.
    pause
    exit /b 1
)

echo ✅ Windows version check passed
echo.

REM Check if we have PowerShell (preferred method)
powershell -Command "Get-Host" > nul 2>&1
if %errorlevel% equ 0 (
    echo 🚀 PowerShell detected - using advanced installer...
    echo.
    powershell -ExecutionPolicy Bypass -File "%~dp0install.ps1"
    goto :end
)

REM Fallback: Manual download method
echo ⚠️  PowerShell not available - using manual download method
echo.
echo 📥 Please follow these steps:
echo.
echo 1. Go to: https://github.com/johnsirmon/cricut-stencil-maker/releases
echo 2. Download: CricutStencilMaker-v0.9.0-Setup.msi
echo 3. Double-click the downloaded file to install
echo 4. Follow the installation wizard
echo.
echo Opening the download page now...
start https://github.com/johnsirmon/cricut-stencil-maker/releases

:end
echo.
echo Installation process complete!
echo.
echo 🚀 To launch Cricut Stencil Maker:
echo    - Check your Desktop for a shortcut
echo    - Or find it in the Start Menu
echo.
echo 📖 Need help? Visit: https://github.com/johnsirmon/cricut-stencil-maker
echo.
pause