using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CricutStencilMaker.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace CricutStencilMaker.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string statusText = "Ready";

        [ObservableProperty]
        private bool isProcessing = false;

        [ObservableProperty]
        private bool hasImageLoaded = false;

        [ObservableProperty]
        private StorageFile currentImageFile;

        [ObservableProperty]
        private double detailLevel = 75;

        [ObservableProperty]
        private double pathSmoothness = 50;

        [ObservableProperty]
        private double minIslandSize = 5;

        [ObservableProperty]
        private double bridgeWidth = 2;

        [ObservableProperty]
        private double bridgeSpacing = 20;

        [ObservableProperty]
        private bool isVinylMode = true;

        [ObservableProperty]
        private bool addBridges = false;

        [ObservableProperty]
        private bool addWeedingBoxes = false;

        [ObservableProperty]
        private double matWidth = 11.5;

        [ObservableProperty]
        private double matHeight = 11.5;

        public MainViewModel()
        {
            // Initialize with safe defaults
        }

        [RelayCommand]
        public async Task LoadImageAsync(StorageFile file)
        {
            try
            {
                IsProcessing = true;
                StatusText = "Loading image...";

                CurrentImageFile = file;
                HasImageLoaded = true;

                StatusText = $"Loaded: {file.Name}";
            }
            catch (Exception ex)
            {
                StatusText = $"Error loading image: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        public async Task RemoveBackgroundAsync()
        {
            if (CurrentImageFile == null) return;

            try
            {
                IsProcessing = true;
                StatusText = "Removing background with AI...";

                await ImageProcessor.RemoveBackgroundAsync(CurrentImageFile);

                StatusText = "Background removed successfully";
            }
            catch (Exception ex)
            {
                StatusText = $"Error removing background: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        public async Task ExportSvgAsync()
        {
            if (CurrentImageFile == null) return;

            try
            {
                IsProcessing = true;
                StatusText = "Processing and generating SVG...";

                var settings = new ImageProcessor.ProcessingSettings
                {
                    RemoveBackground = true,
                    DetailLevel = DetailLevel,
                    PathSmoothness = PathSmoothness,
                    MinIslandSize = MinIslandSize,
                    AddBridges = AddBridges,
                    BridgeWidth = BridgeWidth,
                    BridgeSpacing = BridgeSpacing,
                    AddWeedingBoxes = AddWeedingBoxes,
                    MatWidth = MatWidth,
                    MatHeight = MatHeight
                };

                var svgContent = await ImageProcessor.VectorizeImageAsync(CurrentImageFile, settings);

                // Validate Design Space compatibility
                if (ImageProcessor.ValidateDesignSpaceCompatibility(svgContent))
                {
                    StatusText = "SVG generated successfully - Design Space compatible";
                }
                else
                {
                    StatusText = "Warning: SVG may have compatibility issues with Design Space";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Export error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        partial void OnIsVinylModeChanged(bool value)
        {
            if (value)
            {
                // Vinyl/HTV mode - optimize for weeding
                AddBridges = false;
                AddWeedingBoxes = true;
                StatusText = "Vinyl/HTV mode - optimized for easy weeding";
            }
            else
            {
                // Mylar mode - add bridges for stencil stability
                AddBridges = true;
                AddWeedingBoxes = false;
                StatusText = "Mylar mode - bridges enabled for stencil stability";
            }
        }
    }
}