# Cricut Stencil Maker - One-Click Installer
# This script downloads and installs everything automatically

param(
    [string]$Version = "0.9.0"
)

# Set console colors and title
$Host.UI.RawUI.WindowTitle = "Cricut Stencil Maker Installer"
$Host.UI.RawUI.BackgroundColor = "DarkBlue"
$Host.UI.RawUI.ForegroundColor = "White"
Clear-Host

# ASCII Art Header
Write-Host @"
 ░▒▓██████▓▒░░▒▓███████▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░
░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
░▒▓█▓▒░      ░▒▓███████▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░   ░▒▓█▓▒░   
 ░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓██████▓▒░ ░▒▓██████▓▒░    ░▒▓█▓▒░   

                    STENCIL MAKER v$Version
        Turn any image into a perfect Cricut stencil instantly!
"@ -ForegroundColor Cyan

Write-Host ""
Write-Host "🎨 Welcome to the easiest Cricut stencil maker installation!" -ForegroundColor Green
Write-Host "   This will download and install everything you need automatically." -ForegroundColor White
Write-Host ""

# Check if running as admin (not required, but good to know)
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if ($isAdmin) {
    Write-Host "✅ Running with administrator privileges" -ForegroundColor Green
} else {
    Write-Host "ℹ️  Running as regular user (this is fine!)" -ForegroundColor Yellow
}

# System Requirements Check
Write-Host ""
Write-Host "🔍 Checking your system..." -ForegroundColor Cyan

# Check Windows version
$osVersion = [System.Environment]::OSVersion.Version
$windowsBuild = (Get-ItemProperty "HKLM:SOFTWARE\Microsoft\Windows NT\CurrentVersion").ReleaseId
Write-Host "   Windows Version: $windowsBuild (Build $($osVersion.Build))" -ForegroundColor White

if ($osVersion.Build -lt 17763) {
    Write-Host "❌ ERROR: Windows 10 build 17763 or later is required!" -ForegroundColor Red
    Write-Host "   Please update Windows and try again." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "✅ Windows version is compatible" -ForegroundColor Green

# Check available disk space
$drive = Get-WmiObject -Class Win32_LogicalDisk -Filter "DeviceID='C:'"
$freeSpaceGB = [math]::Round($drive.FreeSpace / 1GB, 2)
Write-Host "   Available disk space: $freeSpaceGB GB" -ForegroundColor White

if ($freeSpaceGB -lt 1) {
    Write-Host "⚠️  Warning: Low disk space. Installation may fail." -ForegroundColor Yellow
} else {
    Write-Host "✅ Sufficient disk space available" -ForegroundColor Green
}

# Check internet connection
Write-Host "   Testing internet connection..." -ForegroundColor White
try {
    $null = Invoke-WebRequest -Uri "https://github.com" -UseBasicParsing -TimeoutSec 10
    Write-Host "✅ Internet connection is working" -ForegroundColor Green
} catch {
    Write-Host "❌ ERROR: No internet connection detected!" -ForegroundColor Red
    Write-Host "   Please check your internet connection and try again." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "🚀 System check complete! Ready to install." -ForegroundColor Green
Write-Host ""

# Confirm installation
do {
    $response = Read-Host "   Do you want to continue? (Y/N)"
    $response = $response.ToUpper()
} while ($response -ne "Y" -and $response -ne "N" -and $response -ne "YES" -and $response -ne "NO")

if ($response -eq "N" -or $response -eq "NO") {
    Write-Host "Installation cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "📥 Starting download and installation..." -ForegroundColor Cyan
Write-Host ""

# Create temp directory
$tempDir = Join-Path $env:TEMP "CricutStencilMaker"
if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}
New-Item -Path $tempDir -ItemType Directory -Force | Out-Null

try {
    # Download URLs (these would be updated when you create releases)
    $installerUrl = "https://github.com/johnsirmon/cricut-stencil-maker/releases/download/v$Version/CricutStencilMaker-v$Version-Setup.msi"
    $installerPath = Join-Path $tempDir "CricutStencilMaker-Setup.msi"

    # Download with progress bar
    Write-Host "[1/3] 📦 Downloading Cricut Stencil Maker installer..." -ForegroundColor Yellow
    
    # PowerShell 5+ has better progress support
    if ($PSVersionTable.PSVersion.Major -ge 5) {
        $ProgressPreference = 'Continue'
        Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath -UseBasicParsing
    } else {
        # Fallback for older PowerShell versions
        $webClient = New-Object System.Net.WebClient
        $webClient.DownloadFile($installerUrl, $installerPath)
    }
    
    if (-not (Test-Path $installerPath)) {
        throw "Download failed - installer file not found"
    }
    
    $fileSize = [math]::Round((Get-Item $installerPath).Length / 1MB, 2)
    Write-Host "✅ Download complete! ($fileSize MB)" -ForegroundColor Green

    # Check if .NET 6 is installed
    Write-Host ""
    Write-Host "[2/3] 🔧 Checking .NET 6 Runtime..." -ForegroundColor Yellow
    
    $dotnetInstalled = $false
    try {
        $dotnetVersion = & dotnet --version 2>$null
        if ($dotnetVersion -and $dotnetVersion.StartsWith("6.")) {
            Write-Host "✅ .NET 6 is already installed ($dotnetVersion)" -ForegroundColor Green
            $dotnetInstalled = $true
        }
    } catch {
        # dotnet command not found
    }
    
    if (-not $dotnetInstalled) {
        Write-Host "📥 Downloading .NET 6 Runtime..." -ForegroundColor Yellow
        $dotnetUrl = "https://download.microsoft.com/download/1/7/2/172989dc-7c15-4c88-bd7f-be2d67b76adf/windowsdesktop-runtime-6.0.25-win-x64.exe"
        $dotnetPath = Join-Path $tempDir "dotnet-runtime.exe"
        
        Invoke-WebRequest -Uri $dotnetUrl -OutFile $dotnetPath -UseBasicParsing
        
        Write-Host "🔧 Installing .NET 6 Runtime..." -ForegroundColor Yellow
        $dotnetProcess = Start-Process -FilePath $dotnetPath -ArgumentList "/quiet" -Wait -PassThru
        
        if ($dotnetProcess.ExitCode -eq 0) {
            Write-Host "✅ .NET 6 Runtime installed successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  .NET installation may have issues, but continuing..." -ForegroundColor Yellow
        }
    }

    # Install the main application
    Write-Host ""
    Write-Host "[3/3] 🎨 Installing Cricut Stencil Maker..." -ForegroundColor Yellow
    Write-Host "       (The installer window will open - just click through it!)" -ForegroundColor Cyan
    
    # Run MSI installer
    $msiProcess = Start-Process -FilePath "msiexec.exe" -ArgumentList "/i `"$installerPath`" /qb" -Wait -PassThru
    
    if ($msiProcess.ExitCode -eq 0) {
        Write-Host ""
        Write-Host "🎉 SUCCESS! Cricut Stencil Maker is now installed!" -ForegroundColor Green
        Write-Host ""
        Write-Host "🚀 How to get started:" -ForegroundColor Cyan
        Write-Host "   1. Find 'Cricut Stencil Maker' in your Start Menu" -ForegroundColor White
        Write-Host "   2. Or double-click the Desktop shortcut" -ForegroundColor White
        Write-Host "   3. Drag any image into the app window" -ForegroundColor White
        Write-Host "   4. Click 'Remove Background' and 'Export SVG'" -ForegroundColor White
        Write-Host "   5. Import the SVG into Cricut Design Space!" -ForegroundColor White
        Write-Host ""
        Write-Host "📖 Need help? Visit: https://github.com/johnsirmon/cricut-stencil-maker" -ForegroundColor Cyan
        
        # Ask if they want to launch the app now
        Write-Host ""
        $launch = Read-Host "Would you like to launch Cricut Stencil Maker now? (Y/N)"
        if ($launch.ToUpper() -eq "Y" -or $launch.ToUpper() -eq "YES") {
            Start-Process "shell:AppsFolder\CricutStencilMaker_App"
        }
        
    } else {
        Write-Host "❌ Installation failed with error code: $($msiProcess.ExitCode)" -ForegroundColor Red
        Write-Host "   Try running as administrator or contact support." -ForegroundColor Red
    }

} catch {
    Write-Host ""
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "🔧 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "   - Check your internet connection" -ForegroundColor White
    Write-Host "   - Try running as administrator" -ForegroundColor White
    Write-Host "   - Temporarily disable antivirus" -ForegroundColor White
    Write-Host "   - Visit our GitHub page for help" -ForegroundColor White
} finally {
    # Cleanup
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host ""
Write-Host "Press Enter to close this window..."
Read-Host