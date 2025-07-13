# Cricut Stencil Maker - PowerShell Build Script
# This script builds both the application and installer

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Cricut Stencil Maker Build Script" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Set build configuration
$Configuration = "Release"
$Platform = "x64"
$Version = "0.9.0"

try {
    Write-Host "[1/4] Cleaning previous builds..." -ForegroundColor Yellow
    if (Test-Path "bin\$Configuration") {
        Remove-Item "bin\$Configuration" -Recurse -Force
    }
    if (Test-Path "Installer\bin") {
        Remove-Item "Installer\bin" -Recurse -Force
    }

    Write-Host "[2/4] Building main application..." -ForegroundColor Yellow
    $buildResult = & dotnet build CricutStencilMaker.csproj -c $Configuration -p:Platform=$Platform
    if ($LASTEXITCODE -ne 0) {
        throw "Application build failed!"
    }

    Write-Host "[3/4] Publishing self-contained application..." -ForegroundColor Yellow
    $publishResult = & dotnet publish CricutStencilMaker.csproj -c $Configuration -p:Platform=$Platform --self-contained true -r win10-x64
    if ($LASTEXITCODE -ne 0) {
        throw "Application publish failed!"
    }

    Write-Host "[4/4] Building installer..." -ForegroundColor Yellow
    
    # Check if WiX is installed
    $wixPath = Get-Command candle.exe -ErrorAction SilentlyContinue
    if (-not $wixPath) {
        throw "WiX Toolset not found! Please install WiX Toolset v3.11 or newer from: https://github.com/wixtoolset/wix3/releases"
    }

    Set-Location Installer

    # Compile WiX source files
    & candle.exe Product.wxs -ext WixUIExtension
    if ($LASTEXITCODE -ne 0) {
        throw "WiX compilation failed!"
    }

    # Create MSI installer
    & light.exe Product.wixobj -ext WixUIExtension -out "CricutStencilMaker-v$Version-Setup.msi"
    if ($LASTEXITCODE -ne 0) {
        throw "MSI creation failed!"
    }

    Set-Location ..

    Write-Host ""
    Write-Host "================================" -ForegroundColor Green
    Write-Host "BUILD COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Installer created: Installer\CricutStencilMaker-v$Version-Setup.msi" -ForegroundColor Green
    Write-Host ""
    Write-Host "To sign the installer (optional):" -ForegroundColor Cyan
    Write-Host "signtool sign /a /t http://timestamp.digicert.com `"Installer\CricutStencilMaker-v$Version-Setup.msi`"" -ForegroundColor Gray
}
catch {
    Write-Host ""
    Write-Host "BUILD FAILED: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")