@echo off
REM Cricut Stencil Maker - Build Installer Script
REM This script builds both the application and installer

echo ================================
echo Cricut Stencil Maker Build Script
echo ================================
echo.

REM Set build configuration
set Configuration=Release
set Platform=x64

echo [1/4] Cleaning previous builds...
if exist "bin\%Configuration%" (
    rmdir /s /q "bin\%Configuration%"
)
if exist "Installer\bin" (
    rmdir /s /q "Installer\bin"
)

echo [2/4] Building main application...
dotnet build CricutStencilMaker.csproj -c %Configuration% -p:Platform=%Platform%
if %ERRORLEVEL% neq 0 (
    echo ERROR: Application build failed!
    pause
    exit /b 1
)

echo [3/4] Publishing self-contained application...
dotnet publish CricutStencilMaker.csproj -c %Configuration% -p:Platform=%Platform% --self-contained true -r win10-x64
if %ERRORLEVEL% neq 0 (
    echo ERROR: Application publish failed!
    pause
    exit /b 1
)

echo [4/4] Building installer...
REM Check if WiX is installed
where candle.exe >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo ERROR: WiX Toolset not found! Please install WiX Toolset v3.11 or newer.
    echo Download from: https://github.com/wixtoolset/wix3/releases
    pause
    exit /b 1
)

cd Installer

REM Compile WiX source files
candle.exe Product.wxs -ext WixUIExtension
if %ERRORLEVEL% neq 0 (
    echo ERROR: WiX compilation failed!
    pause
    exit /b 1
)

REM Create MSI installer
light.exe Product.wixobj -ext WixUIExtension -out "CricutStencilMaker-v0.9.0-Setup.msi"
if %ERRORLEVEL% neq 0 (
    echo ERROR: MSI creation failed!
    pause
    exit /b 1
)

cd ..

echo.
echo ================================
echo BUILD COMPLETED SUCCESSFULLY!
echo ================================
echo.
echo Installer created: Installer\CricutStencilMaker-v0.9.0-Setup.msi
echo.
echo To sign the installer (optional):
echo signtool sign /a /t http://timestamp.digicert.com "Installer\CricutStencilMaker-v0.9.0-Setup.msi"
echo.
pause