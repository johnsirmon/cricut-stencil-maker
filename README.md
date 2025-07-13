# Cricut Stencil Maker

A modern Windows application for creating Design Space-compatible SVG stencils from images.

## Features

- **Drag & Drop Interface**: Simply drag any image to start processing
- **AI Background Removal**: Automatic subject detection with manual refinement tools
- **Material Presets**: Optimized settings for Vinyl/HTV and Mylar materials
- **Bridge Generation**: Automatic bridge insertion for stencil stability
- **Design Space Compatible**: Exports clean SVG files with proper path limits
- **Batch Processing**: Process multiple images at once

## 📥 Installation

### 🚀 Super Easy Installation

**Simple 3-step process:**

1. **Download** the latest `CricutStencilMaker-v0.9.0.zip` from [Releases](https://github.com/johnsirmon/cricut-stencil-maker/releases)
2. **Extract** the ZIP file to any folder
3. **Run** `CricutStencilMaker.exe` - that's it!

**No installation required!** Just extract and run.

### Alternative Installation Methods

**Microsoft Store** (Coming Soon)
- Search for "Cricut Stencil Maker" in Microsoft Store
- One-click install with automatic updates

**Portable Version** (Advanced Users)
- Download `CricutStencilMaker-Portable.zip` from releases
- Extract to any folder and run `CricutStencilMaker.exe`
- No installation required, runs from any location

## System Requirements

### Minimum Requirements
- **OS**: Windows 10 version 1809 (build 17763) or later
- **Memory**: 4 GB RAM
- **Storage**: 50 MB free space
- **Graphics**: DirectX 11 compatible

### Recommended Requirements
- **OS**: Windows 11 (latest updates)
- **Memory**: 8 GB RAM or more
- **Storage**: 1 GB free space (for image processing cache)
- **Graphics**: Dedicated GPU with DirectML support

### Dependencies
- **.NET 6 Runtime** - Automatically installed by the installer
- **Visual C++ Redistributable** - Included in installer
- **No additional software** required

## 🛠️ Building the Application

### For Developers

This application is built using:
- **WinUI 3** for modern Windows UI
- **.NET 6** for cross-platform compatibility
- **C#** for application logic
- **Win2D** for graphics processing
- **WiX Toolset** for installer creation

### Prerequisites

**Required Software:**
- **Visual Studio 2022** (Community or higher) with:
  - .NET 6.0 SDK
  - Windows App SDK 1.4+
  - Windows 10/11 SDK (latest)
  - C++ CMake tools (for future C++ core integration)

**For Installer Creation:**
- **WiX Toolset v3.11+** - [Download here](https://github.com/wixtoolset/wix3/releases)

### Building from Source

**Quick Build (Application Only):**
```bash
git clone https://github.com/johnsirmon/cricut-stencil-maker.git
cd cricut-stencil-maker
dotnet restore
dotnet build -c Release
```

**Full Build with Installer:**
```bash
# Option 1: Windows Batch Script
Build\build-installer.bat

# Option 2: PowerShell Script  
Build\build-installer.ps1

# Option 3: Manual Build
dotnet publish -c Release --self-contained true -r win10-x64
cd Installer
candle.exe Product.wxs -ext WixUIExtension
light.exe Product.wixobj -ext WixUIExtension -out "CricutStencilMaker-v0.9.0-Setup.msi"
```

**Visual Studio Build:**
1. Open `CricutStencilMaker.csproj` in Visual Studio 2022
2. Set platform to x64 or ARM64
3. Build > Rebuild Solution
4. For installer: Build the `Installer` project

## Architecture

The application follows the PRD specifications:

- **Performance**: Designed for ~200ms processing on 2048×2048px images
- **Memory Safety**: Uses modern C# with safe memory management
- **GPU Acceleration**: Ready for DirectML integration
- **Scalability**: Supports images up to mat size limits

## Export Specifications

Generated SVGs are optimized for Cricut Design Space:

- Maximum 5000 paths per file
- Maximum 4000 nodes per path
- Path-only elements (no clipPaths, text, or complex shapes)
- Positive coordinate space
- Proper viewBox sizing for Cricut mats

## Material Presets

### Vinyl/HTV Mode
- Optimized for easy weeding
- Removes micro-islands below threshold
- Optional weeding guide boxes

### Mylar/Stencil Mode
- Automatic bridge generation
- Adjustable bridge width and spacing
- Optimized for stencil stability

## 🚀 Quick Start Guide

### For First-Time Users

1. **Install using one-click installer** from our website
2. **Launch** from Start Menu or Desktop shortcut
3. **Drag an image** (PNG, JPEG, etc.) into the application window
4. **Click "Auto Remove Background"** to remove the background
5. **Choose your material preset:**
   - **Vinyl/HTV**: For easy weeding
   - **Mylar**: For stencils with bridges
6. **Adjust settings** if needed (optional)
7. **Click "Export SVG"** to save your stencil
8. **Import into Cricut Design Space** and cut!

### Troubleshooting

**Common Issues:**

❓ **"Application won't start"**
- Ensure Windows 10 build 17763 or later
- Install latest Windows updates
- Try running as administrator

❓ **"Background removal not working well"**
- Use manual brush tools to refine
- Try different detail level settings
- Ensure good contrast in source image

❓ **"SVG won't import to Design Space"**
- Check that paths are under 5000 limit
- Verify file size is reasonable
- Try re-exporting with lower detail level

❓ **"Installation failed"**
- Download fresh installer from releases
- Temporarily disable antivirus
- Try portable version instead

**Support:**
- 📖 Check the [Wiki](https://github.com/johnsirmon/cricut-stencil-maker/wiki) for detailed guides
- 🐛 Report bugs in [Issues](https://github.com/johnsirmon/cricut-stencil-maker/issues)
- 💬 Ask questions in [Discussions](https://github.com/johnsirmon/cricut-stencil-maker/discussions)

## 📄 License

This project follows the specifications in `prd.md` for a commercial Cricut stencil creation tool.

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

**Development Areas:**
- C++ core integration for faster processing
- Additional material presets
- Advanced vectorization algorithms
- UI/UX improvements
- Documentation and tutorials