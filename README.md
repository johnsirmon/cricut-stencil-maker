# Cricut Stencil Maker

A modern Windows application for creating Design Space-compatible SVG stencils from images.

## Features

- **Drag & Drop Interface**: Simply drag any image to start processing
- **AI Background Removal**: Automatic subject detection with manual refinement tools
- **Material Presets**: Optimized settings for Vinyl/HTV and Mylar materials
- **Bridge Generation**: Automatic bridge insertion for stencil stability
- **Design Space Compatible**: Exports clean SVG files with proper path limits
- **Batch Processing**: Process multiple images at once

## System Requirements

- Windows 10 version 1809 (build 17763) or later
- Windows 11 (recommended)
- No additional runtime dependencies required

## Building the Application

This application is built using:
- WinUI 3 for modern Windows UI
- .NET 6 for cross-platform compatibility
- C# for application logic
- Win2D for graphics processing

### Prerequisites

- Visual Studio 2022 with:
  - .NET 6.0 SDK
  - Windows App SDK
  - Windows 10/11 SDK

### Build Steps

1. Open `CricutStencilMaker.csproj` in Visual Studio 2022
2. Restore NuGet packages
3. Build for your target platform (x64 or ARM64)
4. Deploy using MSIX packaging

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

## License

This project follows the specifications in `prd.md` for a commercial Cricut stencil creation tool.