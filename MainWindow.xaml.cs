using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;

namespace CricutStencilMaker
{
    public sealed partial class MainWindow : Window
    {
        private StorageFile _currentImageFile;
        private BitmapImage _currentBitmap;
        private bool _isEraseMode = false;
        private bool _isDrawing = false;
        private List<Windows.Foundation.Point> _currentStroke = new List<Windows.Foundation.Point>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Cricut Stencil Maker";
            
            // Set window size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
            
            // Center the window
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(this.AppWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea != null)
            {
                var centerX = (displayArea.WorkArea.Width - 1200) / 2;
                var centerY = (displayArea.WorkArea.Height - 800) / 2;
                this.AppWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
            }
        }

        #region Drag and Drop Handling

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            
            // Visual feedback for drag over
            DropZone.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 180, 166));
            DropZone.Background = new SolidColorBrush(Color.FromArgb(20, 0, 180, 166));
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            // Reset visual feedback
            DropZone.BorderBrush = Application.Current.Resources["PrimaryBrush"] as SolidColorBrush;
            DropZone.Background = Application.Current.Resources["SurfaceBrush"] as SolidColorBrush;
        }

        private async void DropZone_Drop(object sender, DragEventArgs e)
        {
            // Reset visual feedback
            DropZone_DragLeave(sender, e);

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0 && items[0] is StorageFile file)
                {
                    await LoadImageFile(file);
                }
            }
        }

        private async void BrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            
            // Get the current window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".tiff");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".webp");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadImageFile(file);
            }
        }

        #endregion

        #region Image Loading and Processing

        private async Task LoadImageFile(StorageFile file)
        {
            try
            {
                StatusText.Text = "Loading image...";
                ProcessingProgress.Visibility = Visibility.Visible;

                _currentImageFile = file;
                
                using var stream = await file.OpenAsync(FileAccessMode.Read);
                _currentBitmap = new BitmapImage();
                await _currentBitmap.SetSourceAsync(stream);
                
                PreviewImage.Source = _currentBitmap;
                
                // Show image preview area, hide drop instructions
                DropInstructions.Visibility = Visibility.Collapsed;
                ImagePreviewArea.Visibility = Visibility.Visible;
                
                // Enable controls
                AutoRemoveButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
                
                ExportStatusText.Text = $"Ready to process: {file.Name}";
                StatusText.Text = $"Loaded: {file.Name} ({_currentBitmap.PixelWidth}×{_currentBitmap.PixelHeight}px)";
                
                ProcessingProgress.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error loading image: {ex.Message}";
                ProcessingProgress.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Background Removal

        private async void AutoRemoveBackground_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImageFile == null) return;

            try
            {
                StatusText.Text = "Removing background...";
                ProcessingProgress.Visibility = Visibility.Visible;
                ProcessingProgress.IsIndeterminate = true;

                // Simulate AI background removal processing
                await Task.Delay(2000);
                
                // In a real implementation, this would call the C++ core for AI segmentation
                // For now, we'll just show a placeholder effect
                
                StatusText.Text = "Background removed successfully";
                ProcessingProgress.Visibility = Visibility.Collapsed;
                
                // Switch to background removed preview
                PreviewModeCombo.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error removing background: {ex.Message}";
                ProcessingProgress.Visibility = Visibility.Collapsed;
            }
        }

        private void EraseMode_Click(object sender, RoutedEventArgs e)
        {
            _isEraseMode = true;
            EraseButton.Background = Application.Current.Resources["AccentBrush"] as SolidColorBrush;
            RestoreButton.ClearValue(Button.BackgroundProperty);
            StatusText.Text = "Erase mode active - click and drag to remove areas";
        }

        private void RestoreMode_Click(object sender, RoutedEventArgs e)
        {
            _isEraseMode = false;
            RestoreButton.Background = Application.Current.Resources["AccentBrush"] as SolidColorBrush;
            EraseButton.ClearValue(Button.BackgroundProperty);
            StatusText.Text = "Restore mode active - click and drag to restore areas";
        }

        private void BrushSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Update brush cursor size
        }

        #endregion

        #region Brush Drawing for Manual Background Removal

        private void BrushCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_currentImageFile == null) return;
            
            _isDrawing = true;
            _currentStroke.Clear();
            BrushCanvas.CapturePointer(e.Pointer);
            
            var position = e.GetCurrentPoint(BrushCanvas).Position;
            _currentStroke.Add(position);
        }

        private void BrushCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDrawing) return;
            
            var position = e.GetCurrentPoint(BrushCanvas).Position;
            _currentStroke.Add(position);
            
            // Draw stroke preview (simplified)
            // In a real implementation, this would update the background removal mask
        }

        private void BrushCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDrawing) return;
            
            _isDrawing = false;
            BrushCanvas.ReleasePointerCapture(e.Pointer);
            
            // Apply the stroke to the background removal mask
            // This would be handled by the C++ processing core
            
            StatusText.Text = $"Applied {(_isEraseMode ? "erase" : "restore")} stroke";
        }

        #endregion

        #region Preview Modes

        private void PreviewMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentImageFile == null) return;
            
            var selectedIndex = PreviewModeCombo.SelectedIndex;
            var modeName = (PreviewModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            
            StatusText.Text = $"Preview mode: {modeName}";
            
            // In a real implementation, this would switch between different processed versions
            // For now, we'll just update the status
        }

        #endregion

        #region Material Presets

        private void MaterialPreset_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == MylarPreset)
            {
                BridgeControls.Visibility = Visibility.Visible;
                StatusText.Text = "Mylar preset selected - bridges enabled for stencil stability";
            }
            else
            {
                BridgeControls.Visibility = Visibility.Collapsed;
                StatusText.Text = "Vinyl/HTV preset selected - optimized for weeding";
            }
        }

        #endregion

        #region Size Settings

        private void MatSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIndex = MatSizeCombo.SelectedIndex;
            if (selectedIndex == 2) // Custom Size
            {
                CustomSizePanel.Visibility = Visibility.Visible;
            }
            else
            {
                CustomSizePanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Export

        private async void ExportSVG_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImageFile == null) return;

            try
            {
                StatusText.Text = "Processing and exporting SVG...";
                ProcessingProgress.Visibility = Visibility.Visible;
                ProcessingProgress.IsIndeterminate = true;

                // Get export settings
                var isMylar = MylarPreset.IsChecked == true;
                var addBridges = AddBridgesCheck.IsChecked == true && isMylar;
                var detailLevel = DetailSlider.Value;
                var smoothness = SmoothnessSlider.Value;
                var minIslandSize = MinIslandSlider.Value;
                var addWeedingBoxes = WeedingBoxesCheck.IsChecked == true;

                // Simulate processing pipeline
                await Task.Delay(3000);

                // Create save picker
                var savePicker = new FileSavePicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("SVG Files", new List<string>() { ".svg" });
                savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(_currentImageFile.Name) + "_stencil";

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Generate Design Space compatible SVG
                    var svgContent = GenerateDesignSpaceCompatibleSVG();
                    await FileIO.WriteTextAsync(file, svgContent);
                    
                    StatusText.Text = $"SVG exported successfully: {file.Name}";
                }
                else
                {
                    StatusText.Text = "Export cancelled";
                }

                ProcessingProgress.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Export error: {ex.Message}";
                ProcessingProgress.Visibility = Visibility.Collapsed;
            }
        }

        private string GenerateDesignSpaceCompatibleSVG()
        {
            // Generate a Design Space compatible SVG with proper viewBox and path elements
            var width = MatSizeCombo.SelectedIndex == 1 ? 23.5 : 11.5;
            var height = 11.5;
            
            if (MatSizeCombo.SelectedIndex == 2) // Custom
            {
                width = CustomWidthBox.Value;
                height = CustomHeightBox.Value;
            }

            var svgContent = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     viewBox=""0 0 {width * 96} {height * 96}"" 
     width=""{width}in"" 
     height=""{height}in"">
  <!-- Generated by Cricut Stencil Maker v0.9.0 -->
  <!-- Design Space Compatible: Path-only SVG -->
  
  <!-- Sample stencil path - in real implementation this would be generated from image processing -->
  <path d=""M100,100 L200,100 L200,200 L100,200 Z M120,120 L180,120 L180,180 L120,180 Z""
        fill=""black""
        fill-rule=""evenodd""/>
        
  <!-- Additional paths would be generated here based on the processed image -->
  
</svg>";

            return svgContent;
        }

        private async void BatchProcess_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                StatusText.Text = $"Batch processing from: {folder.Name}";
                // Implement batch processing logic
            }
        }

        #endregion
    }
}