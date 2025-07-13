# Contributing to Cricut Stencil Maker

Thank you for your interest in contributing to Cricut Stencil Maker! This document provides guidelines for contributing to the project.

## 🚀 Getting Started

### Prerequisites
- Windows 10/11 development environment
- Visual Studio 2022 with .NET 6.0 SDK
- Git for version control
- WiX Toolset (for installer development)

### Setting up Development Environment

1. **Fork and Clone**
   ```bash
   git clone https://github.com/YOUR_USERNAME/cricut-stencil-maker.git
   cd cricut-stencil-maker
   ```

2. **Open in Visual Studio**
   - Open `CricutStencilMaker.csproj`
   - Restore NuGet packages
   - Build and run to ensure everything works

3. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

## 📋 How to Contribute

### Types of Contributions Welcome

- 🐛 **Bug Fixes** - Fix issues reported in GitHub Issues
- ✨ **New Features** - Implement features from the roadmap or propose new ones
- 📖 **Documentation** - Improve README, code comments, or create tutorials
- 🎨 **UI/UX Improvements** - Enhance the user interface and experience
- ⚡ **Performance Optimizations** - Improve processing speed and memory usage
- 🧪 **Testing** - Add unit tests, integration tests, or manual testing
- 🌐 **Localization** - Add support for new languages

### Priority Areas for Development

1. **C++ Core Integration** - High-performance image processing engine
2. **Advanced Vectorization** - Better path generation algorithms
3. **Material Presets** - Additional material-specific optimizations
4. **Batch Processing** - Enhanced multi-file processing capabilities
5. **Export Formats** - Support for additional file formats

## 🔄 Development Workflow

### 1. Issue First
- Check existing issues before starting work
- Create a new issue for bugs or feature requests
- Discuss approach in the issue before implementing

### 2. Code Standards
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Maintain MVVM pattern for UI components

### 3. Testing
- Test your changes thoroughly
- Verify compatibility with different image formats
- Test installer creation (if applicable)
- Check Design Space compatibility for SVG exports

### 4. Pull Request Process
- Create pull request with clear description
- Reference related issues
- Include screenshots for UI changes
- Ensure all checks pass

## 🏗️ Code Architecture

### Project Structure
```
CricutStencilMaker/
├── Models/              # Data models and business logic
├── ViewModels/          # MVVM view models
├── Views/               # XAML UI files
├── Services/            # Image processing and file operations
├── Installer/           # WiX installer project
└── Build/              # Build scripts and tools
```

### Key Components
- **MainWindow** - Primary application interface
- **ImageProcessor** - Core image processing logic
- **MaterialPresets** - Vinyl/Mylar optimization settings
- **SVGExporter** - Design Space compatible export

## 🎯 Coding Guidelines

### C# Style
```csharp
// Use PascalCase for public members
public class ImageProcessor
{
    // Use camelCase for private fields
    private readonly StorageFile currentFile;
    
    // XML documentation for public APIs
    /// <summary>
    /// Processes an image for stencil creation
    /// </summary>
    public async Task<ProcessingResult> ProcessImageAsync(ProcessingSettings settings)
    {
        // Implementation
    }
}
```

### XAML Style
```xml
<!-- Use clear, descriptive names -->
<Button x:Name="ExportButton"
        Content="Export SVG for Cricut Design Space"
        Style="{StaticResource PrimaryButtonStyle}"
        Click="ExportSVG_Click" />
```

## 🧪 Testing

### Manual Testing Checklist
- [ ] Drag and drop various image formats
- [ ] Background removal with different image types
- [ ] Material preset switching
- [ ] SVG export and Design Space import
- [ ] Installer creation and deployment

### Automated Testing (Future)
- Unit tests for image processing algorithms
- Integration tests for file operations
- UI automation tests for key workflows

## 📦 Building and Deployment

### Local Development Build
```bash
dotnet build -c Debug
```

### Release Build with Installer
```bash
Build\build-installer.ps1
```

### Testing Installer
- Test on clean Windows installation
- Verify file associations work
- Check Start Menu and Desktop shortcuts

## 🐛 Bug Reports

### Good Bug Report Includes:
- Clear description of the issue
- Steps to reproduce
- Expected vs actual behavior
- System information (Windows version, etc.)
- Sample image files (if applicable)
- Screenshots or error messages

### Bug Report Template:
```markdown
**Bug Description:**
Brief description of the issue

**Steps to Reproduce:**
1. Step one
2. Step two
3. Step three

**Expected Behavior:**
What should happen

**Actual Behavior:**
What actually happens

**System Info:**
- Windows Version: 
- App Version: 
- Image Format: 

**Additional Context:**
Any other relevant information
```

## 💡 Feature Requests

### Feature Request Template:
```markdown
**Feature Description:**
Clear description of the proposed feature

**Use Case:**
Why this feature would be useful

**Proposed Implementation:**
Ideas for how this could be implemented

**Alternatives Considered:**
Other approaches that were considered
```

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the same license as the project.

## 🤔 Questions?

- 💬 Start a [Discussion](https://github.com/johnsirmon/cricut-stencil-maker/discussions)
- 📧 Contact the maintainers
- 📖 Check the [Wiki](https://github.com/johnsirmon/cricut-stencil-maker/wiki)

Thank you for contributing to Cricut Stencil Maker! 🎨