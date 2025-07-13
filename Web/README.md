# Web Installation Files

This folder contains the one-click installation system for Cricut Stencil Maker.

## Files Overview

### `index.html` - Beautiful Install Landing Page
- **Purpose**: User-friendly web page with big install button
- **Features**: 
  - Responsive design that works on any device
  - Clear system requirements
  - Step-by-step installation guide
  - Beautiful animations and modern UI
- **Hosting**: Can be hosted on GitHub Pages, Netlify, or any web server

### `install.ps1` - Smart PowerShell Installer
- **Purpose**: Fully automated installation script
- **Features**:
  - System requirements check
  - Automatic .NET 6 runtime download/install
  - Downloads and installs main application
  - Progress indicators and user feedback
  - Professional ASCII art interface
  - Error handling and troubleshooting hints
- **Usage**: Downloads and runs automatically when user clicks install button

### `install.bat` - Backup Batch Installer
- **Purpose**: Fallback for systems where PowerShell is restricted
- **Features**:
  - Basic system checks
  - Automatic PowerShell detection
  - Fallback to manual download instructions
  - Opens GitHub releases page
- **Usage**: Alternative method for restricted environments

## How It Works

### User Experience:
1. **User visits the install page** (index.html)
2. **Clicks big "Install Now" button**
3. **Browser downloads install.ps1** automatically
4. **User runs the downloaded script**
5. **Everything installs automatically** with progress feedback

### Technical Flow:
1. `index.html` serves as the landing page
2. JavaScript downloads `install.ps1` when button is clicked
3. `install.ps1` handles the complete installation:
   - System compatibility checks
   - Downloads .NET 6 runtime if needed
   - Downloads the MSI installer from GitHub releases
   - Runs the installer with progress feedback
   - Provides success/error messaging

## Deployment Instructions

### GitHub Pages Setup:
1. **Enable GitHub Pages** in repository settings
2. **Set source to main branch / docs folder**
3. **Copy web files to docs/ folder**
4. **Access at**: `https://yourusername.github.io/cricut-stencil-maker`

### Custom Domain Setup:
1. **Upload files to your web server**
2. **Update URLs in install.ps1** to point to your releases
3. **Configure HTTPS** for security
4. **Test installation process**

## Customization

### Branding:
- Update colors in `index.html` CSS section
- Replace logo emoji with custom image
- Modify text and descriptions
- Add screenshots or demo videos

### URLs:
- Update GitHub repository URLs
- Modify release download URLs
- Change support/documentation links

### Features:
- Add analytics tracking
- Include email signup
- Add social sharing buttons
- Integrate with feedback systems

## Security Considerations

- **PowerShell execution policy**: Users may need to allow script execution
- **Windows Defender**: May flag downloaded executables as potential threats
- **HTTPS required**: Modern browsers require HTTPS for downloads
- **Code signing**: Consider signing the MSI installer for trust

## Testing

### Test the complete flow:
1. **Web page loads correctly** on different browsers
2. **Download button works** and downloads the script
3. **PowerShell script runs** and shows progress
4. **Application installs successfully** and creates shortcuts
5. **Error handling works** for various failure scenarios

### Browser Compatibility:
- ✅ Chrome/Edge (Chromium)
- ✅ Firefox
- ✅ Safari
- ✅ Internet Explorer 11+

### Windows Compatibility:
- ✅ Windows 10 (build 17763+)
- ✅ Windows 11
- ✅ Both x64 and ARM64 architectures

This system provides the ultimate user-friendly installation experience while maintaining technical robustness and error handling.