using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SystemDrawing = System.Drawing;

namespace CricutStencilMaker
{
    public partial class MainForm : Form
    {
        private string currentImagePath = "";
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Cricut Stencil Maker v0.9.0";
            this.Size = new SystemDrawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AllowDrop = true;
            this.BackColor = SystemDrawing.Color.FromArgb(248, 249, 250);

            // Main layout
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            // Title
            var titleLabel = new Label
            {
                Text = "🎨 Cricut Stencil Maker",
                Font = new SystemDrawing.Font("Segoe UI", 24, SystemDrawing.FontStyle.Bold),
                ForeColor = SystemDrawing.Color.FromArgb(0, 180, 166),
                Location = new SystemDrawing.Point(20, 20),
                Size = new SystemDrawing.Size(600, 40),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            // Subtitle
            var subtitleLabel = new Label
            {
                Text = "Drag & drop an image to create a Design Space compatible stencil",
                Font = new SystemDrawing.Font("Segoe UI", 12),
                ForeColor = SystemDrawing.Color.Gray,
                Location = new SystemDrawing.Point(20, 70),
                Size = new SystemDrawing.Size(600, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            // Drop zone
            var dropZone = new Panel
            {
                Location = new SystemDrawing.Point(20, 120),
                Size = new SystemDrawing.Size(740, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemDrawing.Color.White,
                AllowDrop = true
            };

            var dropLabel = new Label
            {
                Text = "Drop your image here or click to browse\n\nSupports: PNG, JPG, BMP, GIF",
                Font = new SystemDrawing.Font("Segoe UI", 14),
                ForeColor = SystemDrawing.Color.Gray,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Name = "DropLabel"
            };

            dropZone.Controls.Add(dropLabel);
            dropZone.DragEnter += DropZone_DragEnter;
            dropZone.DragDrop += DropZone_DragDrop;
            dropZone.Click += DropZone_Click;

            // Settings
            var settingsPanel = new GroupBox
            {
                Text = "Settings",
                Location = new SystemDrawing.Point(20, 340),
                Size = new SystemDrawing.Size(740, 100),
                Font = new SystemDrawing.Font("Segoe UI", 10, SystemDrawing.FontStyle.Bold)
            };

            var removeBackgroundCheck = new CheckBox
            {
                Text = "Remove background automatically",
                Location = new SystemDrawing.Point(20, 30),
                Size = new SystemDrawing.Size(250, 25),
                Checked = true,
                Name = "RemoveBackgroundCheck"
            };

            var materialLabel = new Label
            {
                Text = "Material:",
                Location = new SystemDrawing.Point(280, 30),
                Size = new SystemDrawing.Size(60, 25),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            var materialCombo = new ComboBox
            {
                Location = new SystemDrawing.Point(340, 27),
                Size = new SystemDrawing.Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "MaterialCombo"
            };
            materialCombo.Items.AddRange(new[] { "Vinyl/HTV", "Mylar/Stencil" });
            materialCombo.SelectedIndex = 0;

            var matSizeLabel = new Label
            {
                Text = "Mat Size:",
                Location = new SystemDrawing.Point(20, 60),
                Size = new SystemDrawing.Size(80, 25),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            var matSizeCombo = new ComboBox
            {
                Location = new SystemDrawing.Point(100, 57),
                Size = new SystemDrawing.Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "MatSizeCombo"
            };
            matSizeCombo.Items.AddRange(new[] { "11.5 x 11.5 inch", "23.5 x 11.5 inch" });
            matSizeCombo.SelectedIndex = 0;

            var processButton = new Button
            {
                Text = "Create Stencil SVG",
                Location = new SystemDrawing.Point(400, 40),
                Size = new SystemDrawing.Size(200, 40),
                BackColor = SystemDrawing.Color.FromArgb(0, 180, 166),
                ForeColor = SystemDrawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new SystemDrawing.Font("Segoe UI", 12, SystemDrawing.FontStyle.Bold),
                Enabled = false,
                Name = "ProcessButton"
            };
            processButton.Click += ProcessButton_Click;

            settingsPanel.Controls.AddRange(new Control[] { removeBackgroundCheck, materialLabel, materialCombo, matSizeLabel, matSizeCombo, processButton });

            // Status bar
            var statusLabel = new Label
            {
                Text = "Ready - Drop an image to begin",
                Location = new SystemDrawing.Point(20, 460),
                Size = new SystemDrawing.Size(600, 25),
                Name = "StatusLabel"
            };

            var progressBar = new ProgressBar
            {
                Location = new SystemDrawing.Point(20, 490),
                Size = new SystemDrawing.Size(740, 20),
                Visible = false,
                Name = "ProgressBar"
            };

            // Add all controls
            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, subtitleLabel, dropZone, settingsPanel, statusLabel, progressBar
            });

            this.Controls.Add(mainPanel);
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void DropZone_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                LoadImage(files[0]);
            }
        }

        private void DropZone_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*",
                Title = "Select Image"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(dialog.FileName);
            }
        }

        private void LoadImage(string filePath)
        {
            try
            {
                currentImagePath = filePath;
                var fileName = Path.GetFileName(filePath);
                
                var statusLabel = this.Controls.Find("StatusLabel", true)[0] as Label;
                statusLabel.Text = $"Loaded: {fileName}";

                var processButton = this.Controls.Find("ProcessButton", true)[0] as Button;
                processButton.Enabled = true;

                var dropLabel = this.Controls.Find("DropLabel", true)[0] as Label;
                dropLabel.Text = $"✅ {fileName} loaded\nReady to create stencil!";
                dropLabel.ForeColor = SystemDrawing.Color.FromArgb(0, 180, 166);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error");
            }
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath)) return;

            try
            {
                var processButton = sender as Button;
                var statusLabel = this.Controls.Find("StatusLabel", true)[0] as Label;
                var progressBar = this.Controls.Find("ProgressBar", true)[0] as ProgressBar;
                var removeBackgroundCheck = this.Controls.Find("RemoveBackgroundCheck", true)[0] as CheckBox;
                var matSizeCombo = this.Controls.Find("MatSizeCombo", true)[0] as ComboBox;

                processButton.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                statusLabel.Text = "Processing image...";
                await Task.Delay(500);

                using var saveDialog = new SaveFileDialog
                {
                    Filter = "SVG Files|*.svg",
                    Title = "Save Stencil",
                    FileName = Path.GetFileNameWithoutExtension(currentImagePath) + "_stencil.svg"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var matSize = matSizeCombo.SelectedIndex == 0 ? "11.5" : "23.5";
                    
                    // Use a timeout to prevent hanging on processing
                    var processingTask = Task.Run(() => GenerateSVG(matSize));
                    statusLabel.Text = "Creating Design Space compatible SVG...";
                    
                    // Add a timeout of 30 seconds to prevent hanging
                    if (await Task.WhenAny(processingTask, Task.Delay(30000)) != processingTask)
                    {
                        throw new TimeoutException("Processing the image took too long. Try a simpler image or smaller size.");
                    }
                    
                    var svgContent = await processingTask;
                    
                    await File.WriteAllTextAsync(saveDialog.FileName, svgContent);
                    
                    statusLabel.Text = "✅ Stencil created successfully!";
                    
                    var result = MessageBox.Show(
                        $"Stencil saved!\n\n{saveDialog.FileName}\n\nOpen folder?",
                        "Success!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                    }
                }
            }
            catch (TimeoutException tex)
            {
                MessageBox.Show($"Processing timeout: {tex.Message}", "Processing Timeout");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
            finally
            {
                var processButton = sender as Button;
                var progressBar = this.Controls.Find("ProgressBar", true)[0] as ProgressBar;
                processButton.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private string GenerateSVG(string matSize)
        {
            var width = matSize == "11.5" ? 11.5 : 23.5;
            var height = 11.5;

            try
            {
                // Process the actual image file
                var paths = ProcessImageToSVGPaths(currentImagePath, width, height);
                
                // Check if we have any content
                if (string.IsNullOrWhiteSpace(paths))
                {
                    throw new Exception("No contours were detected in the image.");
                }
                
                return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     viewBox=""0 0 {width * 96} {height * 96}"" 
     width=""{width}in"" 
     height=""{height}in"">
  <!-- Generated by Cricut Stencil Maker v0.9.0 -->
  <!-- Design Space Compatible SVG -->
  
{paths}
</svg>";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing image: {ex.Message}", "Processing Error");
                return GenerateFallbackSVG(width, height);
            }
        }

        // Store contours for bridge generation
        private List<List<SystemDrawing.Point>> _contours;
        
        private string ProcessImageToSVGPaths(string imagePath, double matWidth, double matHeight)
        {
            // Load the original image
            using var originalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath);
            
            var matWidthPx = (int)(matWidth * 96);
            var matHeightPx = (int)(matHeight * 96);
            
            // Resize image to fit mat while maintaining aspect ratio
            var scale = Math.Min((double)matWidthPx / originalImage.Width, (double)matHeightPx / originalImage.Height);
            var newWidth = (int)(originalImage.Width * scale);
            var newHeight = (int)(originalImage.Height * scale);
            
            // Create a resized copy for processing
            using var processedImage = originalImage.Clone(x => x.Resize(newWidth, newHeight));
            
            // Get material settings
            var materialCombo = this.Controls.Find("MaterialCombo", true)[0] as ComboBox;
            var removeBackgroundCheck = this.Controls.Find("RemoveBackgroundCheck", true)[0] as CheckBox;
            bool isMylarMode = materialCombo?.SelectedIndex == 1;
            bool removeBackground = removeBackgroundCheck?.Checked ?? true;
            
            // Apply background removal if enabled
            if (removeBackground)
            {
                processedImage.Mutate(x => x.AutoOrient());
                RemoveBackground(processedImage);
            }
            
            // Center the image on the mat
            var offsetX = (matWidthPx - newWidth) / 2;
            var offsetY = (matHeightPx - newHeight) / 2;
            
            // Generate multi-layer stencil for complete image representation
            return GenerateMultiLayerStencil(processedImage, offsetX, offsetY, isMylarMode);
        }
        
        private string GenerateMultiLayerStencil(SixLabors.ImageSharp.Image<Rgba32> image, int offsetX, int offsetY, bool isMylarMode)
        {
            var paths = new StringBuilder();
            int totalPathCount = 0;
            
            // Define multi-layer thresholds for comprehensive stencil representation
            // Each layer captures different detail levels for complete image coverage
            var layers = new[]
            {
                new { Name = "Bold Outlines", Threshold = 0.2f, Description = "Main shapes and bold features" },
                new { Name = "Main Shapes", Threshold = 0.4f, Description = "Primary structural elements" },
                new { Name = "Medium Details", Threshold = 0.6f, Description = "Secondary features and mid-tones" },
                new { Name = "Fine Details", Threshold = 0.8f, Description = "Delicate features and highlights" }
            };
            
            paths.AppendLine($"  <!-- Multi-Layer Stencil Generation -->");
            paths.AppendLine($"  <!-- Total Layers: {layers.Length} for comprehensive coverage -->");
            
            var allLayerContours = new List<List<SystemDrawing.Point>>();
            
            // Process each layer with different threshold for multi-layer representation
            foreach (var layer in layers)
            {
                if (totalPathCount >= 1000) break; // Reasonable limit for each layer
                
                paths.AppendLine($"  <!-- Layer: {layer.Name} (Threshold: {layer.Threshold}) - {layer.Description} -->");
                
                // Create layer-specific processed image
                using var layerImage = image.Clone();
                layerImage.Mutate(x => x
                    .Grayscale()
                    .GaussianBlur(layer.Threshold * 1.5f) // Adaptive blur based on detail level
                    .BinaryThreshold(layer.Threshold)
                );
                
                // Create binary mask for this layer
                bool[,] mask = new bool[layerImage.Width, layerImage.Height];
                for (int y = 0; y < layerImage.Height; y++)
                {
                    for (int x = 0; x < layerImage.Width; x++)
                    {
                        mask[x, y] = layerImage[x, y].R < 128;
                    }
                }
                
                // Detect contours for this layer
                var layerContours = CreateSingleLayerContours(mask, offsetX, offsetY, layer.Threshold);
                
                // Filter out contours that are too similar to previous layers (prevent duplication)
                var uniqueContours = FilterDuplicateContours(layerContours, allLayerContours, layer.Threshold);
                allLayerContours.AddRange(uniqueContours);
                
                // Remove micro-islands based on material type and layer
                double minAreaThreshold = isMylarMode ? (50 * layer.Threshold) : (10 * layer.Threshold);
                uniqueContours = RemoveMicroIslands(uniqueContours, minAreaThreshold);
                
                // Generate SVG paths for this layer
                foreach (var contour in uniqueContours)
                {
                    if (totalPathCount >= 5000) break; // PRD limit
                    if (contour.Count < 5) continue;
                    
                    // Simplify contour based on layer detail level
                    var simplificationTolerance = isMylarMode ? (0.5 * layer.Threshold) : (1.0 * layer.Threshold);
                    var simplified = SimplifyContourEnhanced(contour, simplificationTolerance);
                    
                    if (simplified.Count < 3) continue;
                    
                    // Create SVG path
                    var pathBuilder = new StringBuilder();
                    pathBuilder.Append($"M{simplified[0].X},{simplified[0].Y}");
                    
                    for (int i = 1; i < simplified.Count; i++)
                    {
                        pathBuilder.Append($" L {simplified[i].X},{simplified[i].Y}");
                    }
                    pathBuilder.Append(" z");
                    
                    // Add path with layer information in comment
                    paths.AppendLine($"  <path d=\"{pathBuilder}\" fill=\"black\" fill-rule=\"evenodd\"/><!-- {layer.Name} -->");
                    totalPathCount++;
                }
                
                paths.AppendLine($"  <!-- End {layer.Name}: {uniqueContours.Count} paths added -->");
            }
            
            // Add bridges for Mylar mode if needed
            if (isMylarMode && allLayerContours.Count > 0)
            {
                paths.AppendLine($"  <!-- Bridge Generation for Mylar Stencil Integrity -->");
                var islands = IdentifyIslands(allLayerContours);
                var bridgePaths = GenerateBridges(islands);
                
                foreach (var bridge in bridgePaths)
                {
                    if (totalPathCount >= 5000) break;
                    paths.AppendLine($"  <path d=\"{bridge}\" fill=\"black\" stroke=\"black\" stroke-width=\"1\"/><!-- Bridge -->");
                    totalPathCount++;
                }
            }
            
            paths.AppendLine($"  <!-- Multi-Layer Generation Complete: {totalPathCount} total paths -->");
            return paths.ToString();
        }
        
        private List<List<SystemDrawing.Point>> CreateSingleLayerContours(bool[,] mask, int offsetX, int offsetY, float threshold)
        {
            var contours = new List<List<SystemDrawing.Point>>();
            var width = mask.GetLength(0);
            var height = mask.GetLength(1);
            var visited = new bool[width, height];
            
            // Limit contours per layer based on threshold (more detailed layers get more contours)
            int maxContoursPerLayer = (int)(50 / threshold); // Fine details get more paths
            
            for (int y = 0; y < height && contours.Count < maxContoursPerLayer; y++)
            {
                for (int x = 0; x < width && contours.Count < maxContoursPerLayer; x++)
                {
                    if (visited[x, y] || !mask[x, y]) continue;
                    
                    // Check if this is a boundary pixel
                    if (IsBoundaryPixel(mask, x, y))
                    {
                        var contour = new List<SystemDrawing.Point>();
                        TraceBoundarySimple(mask, visited, x, y, contour, offsetX, offsetY);
                        
                        if (contour.Count > 4) // Minimum viable contour size
                        {
                            contours.Add(contour);
                        }
                    }
                }
            }
            
            return contours;
        }
        
        private bool IsBoundaryPixel(bool[,] mask, int x, int y)
        {
            var width = mask.GetLength(0);
            var height = mask.GetLength(1);
            
            // Check 8-connected neighbors
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            
            for (int k = 0; k < 8; k++)
            {
                int nx = x + dx[k];
                int ny = y + dy[k];
                
                if (nx >= 0 && ny >= 0 && nx < width && ny < height && !mask[nx, ny])
                {
                    return true; // Found a background neighbor
                }
            }
            return false;
        }
        
        private void TraceBoundarySimple(bool[,] mask, bool[,] visited, int startX, int startY, 
                                       List<SystemDrawing.Point> contour, int offsetX, int offsetY)
        {
            var width = mask.GetLength(0);
            var height = mask.GetLength(1);
            
            // Simple boundary following to prevent infinite loops and duplication
            var current = new SystemDrawing.Point(startX, startY);
            var start = current;
            
            int maxPoints = 500; // Reasonable limit per contour
            int pointCount = 0;
            
            do
            {
                if (pointCount++ > maxPoints) break;
                
                visited[current.X, current.Y] = true;
                contour.Add(new SystemDrawing.Point(current.X + offsetX, current.Y + offsetY));
                
                // Find next boundary point using simple 8-connectivity
                var next = FindNextBoundaryPoint(mask, current, width, height);
                if (next.X == -1) break; // No next point found
                
                current = next;
                
            } while (!(current.X == start.X && current.Y == start.Y) && pointCount < maxPoints);
        }
        
        private SystemDrawing.Point FindNextBoundaryPoint(bool[,] mask, SystemDrawing.Point current, int width, int height)
        {
            // 8-connected neighbors in clockwise order
            int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
            
            for (int i = 0; i < 8; i++)
            {
                int nx = current.X + dx[i];
                int ny = current.Y + dy[i];
                
                if (nx >= 0 && ny >= 0 && nx < width && ny < height && 
                    mask[nx, ny] && IsBoundaryPixel(mask, nx, ny))
                {
                    return new SystemDrawing.Point(nx, ny);
                }
            }
            
            return new SystemDrawing.Point(-1, -1); // No next point found
        }
        
        private List<List<SystemDrawing.Point>> FilterDuplicateContours(
            List<List<SystemDrawing.Point>> newContours, 
            List<List<SystemDrawing.Point>> existingContours, 
            float threshold)
        {
            var uniqueContours = new List<List<SystemDrawing.Point>>();
            
            foreach (var contour in newContours)
            {
                bool isDuplicate = false;
                
                // Check against existing contours with threshold-based tolerance
                var tolerance = (int)(10 / threshold); // Finer thresholds allow closer contours
                
                foreach (var existing in existingContours)
                {
                    if (AreContoursNearDuplicate(contour, existing, tolerance))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                
                if (!isDuplicate)
                {
                    uniqueContours.Add(contour);
                }
            }
            
            return uniqueContours;
        }
        
        private bool AreContoursNearDuplicate(List<SystemDrawing.Point> contour1, List<SystemDrawing.Point> contour2, int tolerance)
        {
            if (Math.Abs(contour1.Count - contour2.Count) > Math.Max(contour1.Count, contour2.Count) * 0.3) 
                return false;
            
            var bounds1 = GetBoundingBox(contour1);
            var bounds2 = GetBoundingBox(contour2);
            
            return Math.Abs(bounds1.X - bounds2.X) < tolerance &&
                   Math.Abs(bounds1.Y - bounds2.Y) < tolerance &&
                   Math.Abs(bounds1.Width - bounds2.Width) < tolerance &&
                   Math.Abs(bounds1.Height - bounds2.Height) < tolerance;
        }
        
        private void RemoveBackground(SixLabors.ImageSharp.Image<Rgba32> image)
        {
            // Simple background removal using edge-based segmentation
            // This is a basic implementation - a full AI segmentation would be more complex
            
            // Apply edge detection to find the main subject
            var edgeImage = image.Clone();
            
            // Enhance contrast to make subject more distinct
            edgeImage.Mutate(x => x
                .Contrast(1.2f)
                .Brightness(1.1f)
                .Saturate(1.1f)
            );
            
            // Convert to grayscale for processing
            using var grayImage = edgeImage.Clone(x => x.Grayscale());
            
            // Apply Gaussian blur followed by threshold to create a mask
            grayImage.Mutate(x => x
                .GaussianBlur(2.0f)
                .BinaryThreshold(0.4f)
            );
            
            // Use the mask to remove background from original image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var maskPixel = grayImage[x, y];
                    
                    // If the mask pixel is black (background), make it transparent
                    if (maskPixel.R < 128)
                    {
                        image[x, y] = new Rgba32(255, 255, 255, 255); // White background
                    }
                }
            }
        }
        
        private List<List<SystemDrawing.Point>> DeduplicatePaths(List<List<SystemDrawing.Point>> contours)
        {
            var result = new List<List<SystemDrawing.Point>>();
            
            foreach (var contour in contours)
            {
                bool isDuplicate = false;
                
                // Check if this contour is very similar to an existing one
                foreach (var existing in result)
                {
                    if (AreContoursEquivalent(contour, existing))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                
                if (!isDuplicate)
                {
                    result.Add(contour);
                }
            }
            
            return result;
        }
        
        private bool AreContoursEquivalent(List<SystemDrawing.Point> contour1, List<SystemDrawing.Point> contour2)
        {
            // Simple check: if contours have very similar bounding boxes and similar number of points
            if (Math.Abs(contour1.Count - contour2.Count) > contour1.Count * 0.2) return false;
            
            var bounds1 = GetBoundingBox(contour1);
            var bounds2 = GetBoundingBox(contour2);
            
            // Check if bounding boxes are very similar (within 5 pixels)
            return Math.Abs(bounds1.X - bounds2.X) < 5 &&
                   Math.Abs(bounds1.Y - bounds2.Y) < 5 &&
                   Math.Abs(bounds1.Width - bounds2.Width) < 5 &&
                   Math.Abs(bounds1.Height - bounds2.Height) < 5;
        }
        
        private SystemDrawing.Rectangle GetBoundingBox(List<SystemDrawing.Point> contour)
        {
            if (contour.Count == 0) return new SystemDrawing.Rectangle();
            
            int minX = contour.Min(p => p.X);
            int maxX = contour.Max(p => p.X);
            int minY = contour.Min(p => p.Y);
            int maxY = contour.Max(p => p.Y);
            
            return new SystemDrawing.Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
        
        private List<List<SystemDrawing.Point>> RemoveMicroIslands(List<List<SystemDrawing.Point>> contours, double minAreaThreshold)
        {
            var result = new List<List<SystemDrawing.Point>>();
            
            foreach (var contour in contours)
            {
                // Calculate the approximate area of the contour
                double area = CalculatePolygonArea(contour);
                
                // Keep contours larger than the threshold
                if (area > minAreaThreshold)
                {
                    result.Add(contour);
                }
            }
            
            return result;
        }
        
        private double CalculatePolygonArea(List<SystemDrawing.Point> polygon)
        {
            // Calculate area using the Shoelace formula
            double area = 0;
            int j = polygon.Count - 1;
            
            for (int i = 0; i < polygon.Count; i++)
            {
                area += (polygon[j].X + polygon[i].X) * (polygon[j].Y - polygon[i].Y);
                j = i;
            }
            
            return Math.Abs(area / 2.0);
        }
        
        private Dictionary<int, int> IdentifyIslands(List<List<SystemDrawing.Point>> contours)
        {
            var islands = new Dictionary<int, int>(); // Maps island index to its container index
            
            // Safety check - if no contours or too many contours, return empty dictionary to prevent hanging
            if (contours == null || contours.Count == 0 || contours.Count > 1000)
            {
                return islands;
            }
            
            // For each potential island (limit to first 100 contours to prevent hanging)
            int maxContoursToCheck = Math.Min(contours.Count, 100);
            
            for (int i = 0; i < maxContoursToCheck; i++)
            {
                var innerContour = contours[i];
                
                // Skip empty contours
                if (innerContour == null || innerContour.Count < 3) continue;
                
                try
                {
                    // Find a point that's definitely inside this contour
                    var testPoint = new SystemDrawing.Point(
                        (innerContour.Min(p => p.X) + innerContour.Max(p => p.X)) / 2,
                        (innerContour.Min(p => p.Y) + innerContour.Max(p => p.Y)) / 2
                    );
                    
                    // Test against all other contours (limit to prevent hanging)
                    for (int j = 0; j < maxContoursToCheck; j++)
                    {
                        // Skip self-comparison or already identified islands
                        if (i == j || islands.ContainsKey(i)) continue;
                        
                        var outerContour = contours[j];
                        
                        // Skip invalid contours
                        if (outerContour == null || outerContour.Count < 3) continue;
                        
                        // Convert to array for PointInPolygon
                        var polygon = outerContour.ToArray();
                        
                        // Check if inner contour's center is inside outer contour
                        if (PointInPolygon(testPoint, polygon))
                        {
                            // This is an island inside the container j
                            islands[i] = j;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // Skip this contour if any errors occur
                    continue;
                }
            }
            
            return islands;
        }
        
        private bool PointInPolygon(SystemDrawing.Point point, SystemDrawing.Point[] polygon)
        {
            // Implementation of the ray casting algorithm
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y) &&
                    point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / 
                    (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }
            return inside;
        }
        
        private List<string> GenerateBridges(Dictionary<int, int> islands)
        {
            var bridges = new List<string>();
            
            // Safety check to prevent hanging
            if (islands == null || islands.Count == 0 || _contours == null || _contours.Count == 0)
            {
                return bridges;
            }
            
            // Limit the number of bridges to prevent performance issues
            int maxBridges = Math.Min(islands.Count, 50);
            int bridgeCount = 0;
            
            // For each identified island, create a bridge to its container
            foreach (var island in islands)
            {
                try
                {
                    // Safety check for invalid indices
                    int islandIndex = island.Key;
                    int containerIndex = island.Value;
                    
                    if (islandIndex < 0 || islandIndex >= _contours.Count ||
                        containerIndex < 0 || containerIndex >= _contours.Count)
                    {
                        continue;
                    }
                    
                    // Find the closest points between the island and its container
                    var closestPoints = FindClosestPointsBetweenContours(islandIndex, containerIndex);
                    
                    if (closestPoints.Item1.X != 0 || closestPoints.Item1.Y != 0)
                    {
                        // Create a bridge path with proper width for mylar cutting
                        var bridgePath = CreateBridgePath(closestPoints.Item1, closestPoints.Item2);
                        bridges.Add(bridgePath);
                        
                        bridgeCount++;
                        if (bridgeCount >= maxBridges) break;
                    }
                }
                catch (Exception)
                {
                    // Skip this bridge if any error occurs
                    continue;
                }
            }
            
            return bridges;
        }
        
        private (SystemDrawing.Point, SystemDrawing.Point) FindClosestPointsBetweenContours(int contourIndex1, int contourIndex2)
        {
            var contour1 = _contours[contourIndex1];
            var contour2 = _contours[contourIndex2];
            
            SystemDrawing.Point closestPoint1 = contour1[0];
            SystemDrawing.Point closestPoint2 = contour2[0];
            double minDistance = double.MaxValue;
            
            // For each point in contour1, find the closest point in contour2
            foreach (var p1 in contour1)
            {
                foreach (var p2 in contour2)
                {
                    double distance = Distance(p1, p2);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint1 = p1;
                        closestPoint2 = p2;
                    }
                }
            }
            
            return (closestPoint1, closestPoint2);
        }
        
        private string CreateBridgePath(SystemDrawing.Point point1, SystemDrawing.Point point2)
        {
            // Create a bridge with width for better cutting on mylar
            double bridgeWidth = 2.0; // 2 pixels wide
            
            // Calculate perpendicular direction for bridge width
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            
            if (length == 0) return $"M{point1.X},{point1.Y} L{point2.X},{point2.Y}";
            
            // Normalize and get perpendicular
            double perpX = -dy / length * bridgeWidth / 2;
            double perpY = dx / length * bridgeWidth / 2;
            
            // Create rectangle bridge
            var p1 = new SystemDrawing.Point((int)(point1.X + perpX), (int)(point1.Y + perpY));
            var p2 = new SystemDrawing.Point((int)(point1.X - perpX), (int)(point1.Y - perpY));
            var p3 = new SystemDrawing.Point((int)(point2.X - perpX), (int)(point2.Y - perpY));
            var p4 = new SystemDrawing.Point((int)(point2.X + perpX), (int)(point2.Y + perpY));
            
            return $"M{p1.X},{p1.Y} L{p2.X},{p2.Y} L{p3.X},{p3.Y} L{p4.X},{p4.Y} z";
        }
        
        private SystemDrawing.Point FindClosestPointToContour(int contourIndex1, int contourIndex2)
        {
            // Find the closest point from contour1 to contour2
            var contour1 = _contours[contourIndex1];
            var contour2 = _contours[contourIndex2];
            
            SystemDrawing.Point closestPoint = contour1[0];
            double minDistance = double.MaxValue;
            
            // For each point in contour1
            foreach (var p1 in contour1)
            {
                // Find the minimum distance to any point in contour2
                foreach (var p2 in contour2)
                {
                    double distance = Distance(p1, p2);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = p1;
                    }
                }
            }
            
            return closestPoint;
        }
        
        private SystemDrawing.Point FindClosestPointToPoint(int contourIndex, SystemDrawing.Point targetPoint)
        {
            // Find the closest point on a contour to a given point
            var contour = _contours[contourIndex];
            
            SystemDrawing.Point closestPoint = contour[0];
            double minDistance = Distance(contour[0], targetPoint);
            
            foreach (var point in contour)
            {
                double distance = Distance(point, targetPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }
            
            return closestPoint;
        }
        
        private double Distance(SystemDrawing.Point p1, SystemDrawing.Point p2)
        {
            // Calculate Euclidean distance between two points
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private List<List<SystemDrawing.Point>> FindContours(SixLabors.ImageSharp.Image<Rgba32> image, bool[,] visited, int offsetX, int offsetY)
        {
            var contours = new List<List<SystemDrawing.Point>>();
            
            // Scan the binary image for contours
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Skip if already visited or if pixel is background (white)
                    if (visited[x, y] || image[x, y].R > 128) continue;

                    // Found a black pixel, trace its contour
                    var contour = new List<SystemDrawing.Point>();
                    
                    // Use flood fill to trace the shape
                    FloodFillTraceContour(image, x, y, visited, contour, offsetX, offsetY);
                    
                    // Only add contours that have enough points
                    if (contour.Count > 4)
                    {
                        contours.Add(contour);
                    }
                }
            }
            
            return contours;
        }
        
        private List<List<SystemDrawing.Point>> EnhancedContourDetection(bool[,] mask, int offsetX, int offsetY)
        {
            var contours = new List<List<SystemDrawing.Point>>();
            
            try
            {
                // Safety check
                if (mask == null) return contours;
                
                var width = mask.GetLength(0);
                var height = mask.GetLength(1);
                
                // Set a limit on the number of contours to detect
                int maxContours = 100; // Limit to prevent performance issues
                
                // Create a visited mask for tracking
                bool[,] visited = new bool[width, height];
                
                // Boundary tracing algorithm (Moore-Neighbor Tracing)
                for (int y = 0; y < height; y++)
                {
                    // Break if we've found enough contours
                    if (contours.Count >= maxContours) break;
                    
                    for (int x = 0; x < width; x++)
                    {
                        // Break if we've found enough contours
                        if (contours.Count >= maxContours) break;
                        
                        // Skip if already visited or if pixel is background
                        if (visited[x, y] || !mask[x, y]) continue;
                        
                        // Check if this is a boundary pixel by examining neighbors
                        bool isBoundary = false;
                        
                        // Define the 8-connected neighbors
                        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
                        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
                        
                        // Check if any neighbor is background
                        for (int k = 0; k < 8; k++)
                        {
                            int nx = x + dx[k];
                            int ny = y + dy[k];
                            
                            if (nx >= 0 && ny >= 0 && nx < width && ny < height && !mask[nx, ny])
                            {
                                isBoundary = true;
                                break;
                            }
                        }
                        
                        // If this is a boundary pixel, trace the complete contour
                        if (isBoundary)
                        {
                            var contour = new List<SystemDrawing.Point>();
                            TraceContourBoundary(mask, visited, x, y, contour, offsetX, offsetY);
                            
                            // Only add contours with enough points
                            if (contour.Count > 4)
                            {
                                contours.Add(contour);
                            }
                        }
                    }
                }
                
                // Only detect inner contours if we have space left
                if (contours.Count < maxContours)
                {
                    // Also detect inner contours (holes)
                    bool[,] innerMask = new bool[width, height];
                    // Invert the mask to find inner contours
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            innerMask[x, y] = !mask[x, y];
                        }
                    }
                    
                    // Reset visited mask
                    Array.Clear(visited, 0, visited.Length);
                    
                    // Find inner contours using the same algorithm
                    for (int y = 1; y < height - 1 && contours.Count < maxContours; y++)
                    {
                        for (int x = 1; x < width - 1 && contours.Count < maxContours; x++)
                        {
                            // Skip if already visited or if pixel is background in the inverted mask
                            if (visited[x, y] || !innerMask[x, y]) continue;
                            
                            // Check if this is a boundary pixel
                            bool isBoundary = false;
                            
                            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
                            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
                            
                            for (int k = 0; k < 8; k++)
                            {
                                int nx = x + dx[k];
                                int ny = y + dy[k];
                                
                                if (nx >= 0 && ny >= 0 && nx < width && ny < height && !innerMask[nx, ny])
                                {
                                    isBoundary = true;
                                    break;
                                }
                            }
                            
                            // If this is a boundary pixel of a hole, trace it
                            if (isBoundary)
                            {
                                var contour = new List<SystemDrawing.Point>();
                                TraceContourBoundary(innerMask, visited, x, y, contour, offsetX, offsetY);
                                
                                // Only add inner contours with enough points
                                if (contour.Count > 4)
                                {
                                    contours.Add(contour);
                                }
                            }
                        }
                    }
                }
                
                // If we didn't find any contours, create a simple fallback contour
                if (contours.Count == 0)
                {
                    var fallbackContour = new List<SystemDrawing.Point>
                    {
                        new SystemDrawing.Point(offsetX + 10, offsetY + 10),
                        new SystemDrawing.Point(offsetX + width - 10, offsetY + 10),
                        new SystemDrawing.Point(offsetX + width - 10, offsetY + height - 10),
                        new SystemDrawing.Point(offsetX + 10, offsetY + height - 10),
                        new SystemDrawing.Point(offsetX + 10, offsetY + 10)
                    };
                    contours.Add(fallbackContour);
                }
            }
            catch (Exception)
            {
                // If anything goes wrong, return a simple rectangle contour as fallback
                int width = 100, height = 100;
                var fallbackContour = new List<SystemDrawing.Point>
                {
                    new SystemDrawing.Point(offsetX + 10, offsetY + 10),
                    new SystemDrawing.Point(offsetX + width - 10, offsetY + 10),
                    new SystemDrawing.Point(offsetX + width - 10, offsetY + height - 10),
                    new SystemDrawing.Point(offsetX + 10, offsetY + height - 10),
                    new SystemDrawing.Point(offsetX + 10, offsetY + 10)
                };
                contours.Add(fallbackContour);
            }
            
            return contours;
        }
        
        private void TraceContourBoundary(bool[,] mask, bool[,] visited, int startX, int startY, 
                                        List<SystemDrawing.Point> contour, int offsetX, int offsetY)
        {
            var width = mask.GetLength(0);
            var height = mask.GetLength(1);
            
            // Safety check for boundaries
            if (startX < 0 || startY < 0 || startX >= width || startY >= height)
            {
                return; // Invalid starting point
            }
            
            // Starting point
            int x = startX;
            int y = startY;
            
            // Direction: 0=E, 1=SE, 2=S, 3=SW, 4=W, 5=NW, 6=N, 7=NE
            int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
            
            // Jacob's stopping criterion
            int initialX = x;
            int initialY = y;
            int initialDir = 7; // Start by looking NE
            
            bool firstPoint = true;
            int maxPoints = 5000; // Limit to prevent infinite loops
            int pointCount = 0;
            
            try
            {
                do {
                    // Safety check to prevent infinite loops
                    if (pointCount++ > maxPoints)
                    {
                        break;
                    }
                    
                    // Mark current pixel as visited
                    visited[x, y] = true;
                    
                    // Add to contour
                    contour.Add(new SystemDrawing.Point(x + offsetX, y + offsetY));
                    
                    // Find next boundary pixel (Moore Neighborhood)
                    int dir = (initialDir + 6) % 8; // Start looking at the left neighbor
                    bool found = false;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        int checkDir = (dir + i) % 8;
                        int nx = x + dx[checkDir];
                        int ny = y + dy[checkDir];
                        
                        // Check if valid and is a foreground pixel
                        if (nx >= 0 && ny >= 0 && nx < width && ny < height && mask[nx, ny])
                        {
                            x = nx;
                            y = ny;
                            initialDir = checkDir;
                            found = true;
                            break;
                        }
                    }
                    
                    if (!found) break; // No more boundary pixels
                    
                    // Exit condition: we've returned to the starting point
                    if (!firstPoint && x == initialX && y == initialY)
                    {
                        break;
                    }
                    
                    firstPoint = false;
                    
                } while (true);
                
                // Ensure the contour is closed
                if (contour.Count > 0 && (contour[0].X != contour[contour.Count - 1].X || contour[0].Y != contour[contour.Count - 1].Y))
                {
                    contour.Add(new SystemDrawing.Point(contour[0].X, contour[0].Y));
                }
                
                // Limit contour size to prevent performance issues
                if (contour.Count > maxPoints)
                {
                    var simplifiedContour = new List<SystemDrawing.Point>();
                    double step = (double)contour.Count / maxPoints;
                    
                    for (double i = 0; i < contour.Count; i += step)
                    {
                        simplifiedContour.Add(contour[(int)i]);
                    }
                    
                    // Always include the last point
                    if (!simplifiedContour.Contains(contour[contour.Count - 1]))
                    {
                        simplifiedContour.Add(contour[contour.Count - 1]);
                    }
                    
                    contour.Clear();
                    contour.AddRange(simplifiedContour);
                }
            }
            catch (Exception)
            {
                // If any error occurs, ensure we have a minimal valid contour
                if (contour.Count < 3)
                {
                    contour.Clear(); // Clear invalid contour
                }
            }
        }
        
        private void FloodFillTraceContour(SixLabors.ImageSharp.Image<Rgba32> image, int startX, int startY, 
                                         bool[,] visited, List<SystemDrawing.Point> contour,
                                         int offsetX, int offsetY)
        {
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));
            
            // Define movement directions (8-connected neighbors)
            var dx = new[] { -1, -1, -1,  0,  0,  1, 1, 1 };
            var dy = new[] { -1,  0,  1, -1,  1, -1, 0, 1 };
            
            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                
                if (x < 0 || y < 0 || x >= image.Width || y >= image.Height || visited[x, y])
                    continue;
                
                // Skip white pixels (background)
                if (image[x, y].R > 128)
                    continue;
                
                // Mark as visited and add to contour
                visited[x, y] = true;
                
                // Check if this is a boundary pixel
                bool isBoundary = false;
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    
                    if (nx < 0 || ny < 0 || nx >= image.Width || ny >= image.Height || image[nx, ny].R > 128)
                    {
                        isBoundary = true;
                        break;
                    }
                }
                
                // Only add boundary pixels to the contour
                if (isBoundary)
                {
                    contour.Add(new SystemDrawing.Point(x + offsetX, y + offsetY));
                }
                
                // Add unvisited neighbors to queue
                for (int i = 0; i < dx.Length; i++)
                {
                    queue.Enqueue((x + dx[i], y + dy[i]));
                }
            }
        }

        private List<SystemDrawing.Point> SimplifyContour(List<SystemDrawing.Point> contour, double epsilon)
        {
            if (contour.Count <= 3) return contour;
            
            // Douglas-Peucker algorithm implementation for path simplification
            
            // Find the point with the maximum distance from the line between start and end
            double maxDistance = 0;
            int index = 0;
            
            int start = 0;
            int end = contour.Count - 1;
            
            for (int i = start + 1; i < end; i++)
            {
                double distance = PointToLineDistance(contour[i], contour[start], contour[end]);
                
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    index = i;
                }
            }
            
            // If max distance is greater than epsilon, recursively simplify
            if (maxDistance > epsilon)
            {
                // Recursive call
                var firstPart = SimplifyContour(contour.GetRange(0, index + 1), epsilon);
                var secondPart = SimplifyContour(contour.GetRange(index, contour.Count - index), epsilon);
                
                // Build the result
                var result = new List<SystemDrawing.Point>();
                result.AddRange(firstPart.GetRange(0, firstPart.Count - 1));
                result.AddRange(secondPart);
                
                return result;
            }
            else
            {
                // Return start and end points
                return new List<SystemDrawing.Point> { contour[start], contour[end] };
            }
        }
        
        private List<SystemDrawing.Point> SimplifyContourEnhanced(List<SystemDrawing.Point> contour, double epsilon)
        {
            if (contour.Count <= 3) return contour;
            
            // Enhanced Douglas-Peucker algorithm implementation
            // Uses a more adaptive epsilon value based on contour length
            
            // Adjust epsilon based on contour length to preserve important details
            double adaptiveEpsilon = epsilon * (1 + Math.Log10(Math.Max(1, contour.Count / 100.0)));
            
            // Enhanced version with angle-based simplification
            List<SystemDrawing.Point> result = new List<SystemDrawing.Point>();
            result.Add(contour[0]); // Always include the first point
            
            // Apply Douglas-Peucker and preserve critical corners
            for (int i = 1; i < contour.Count - 1; i++)
            {
                var prev = contour[i - 1];
                var curr = contour[i];
                var next = contour[i + 1];
                
                // Calculate angle to determine if this is a corner
                double angle = CalculateAngle(prev, curr, next);
                
                // Calculate distance from line
                double distance = PointToLineDistance(curr, prev, next);
                
                // Keep points that form a significant angle or are far from the line
                if (angle < 150 || distance > adaptiveEpsilon)
                {
                    result.Add(curr);
                }
            }
            
            // Always include the last point
            if (contour.Count > 1)
            {
                result.Add(contour[contour.Count - 1]);
            }
            
            // Post-processing: ensure we don't have too many nodes (as per PRD: ≤4000 nodes)
            if (result.Count > 4000)
            {
                // Further simplify by sampling evenly
                var finalResult = new List<SystemDrawing.Point>();
                double step = (double)result.Count / 4000;
                
                for (double i = 0; i < result.Count; i += step)
                {
                    finalResult.Add(result[(int)i]);
                }
                
                // Always include the last point
                if (!finalResult.Contains(result[result.Count - 1]))
                {
                    finalResult.Add(result[result.Count - 1]);
                }
                
                return finalResult;
            }
            
            return result;
        }
        
        private double CalculateAngle(SystemDrawing.Point p1, SystemDrawing.Point p2, SystemDrawing.Point p3)
        {
            // Calculate the angle between three points (in degrees)
            
            // Vectors
            double v1x = p1.X - p2.X;
            double v1y = p1.Y - p2.Y;
            double v2x = p3.X - p2.X;
            double v2y = p3.Y - p2.Y;
            
            // Dot product
            double dotProduct = v1x * v2x + v1y * v2y;
            
            // Magnitudes
            double mag1 = Math.Sqrt(v1x * v1x + v1y * v1y);
            double mag2 = Math.Sqrt(v2x * v2x + v2y * v2y);
            
            // Angle in degrees
            double angle = Math.Acos(Math.Max(-1, Math.Min(1, dotProduct / (mag1 * mag2)))) * 180 / Math.PI;
            
            return angle;
        }
        
        private double PointToLineDistance(SystemDrawing.Point point, SystemDrawing.Point lineStart, SystemDrawing.Point lineEnd)
        {
            // Calculate the perpendicular distance from point to line segment
            double A = point.X - lineStart.X;
            double B = point.Y - lineStart.Y;
            double C = lineEnd.X - lineStart.X;
            double D = lineEnd.Y - lineStart.Y;

            double dot = A * C + B * D;
            double lenSq = C * C + D * D;

            if (lenSq == 0) return Math.Sqrt(A * A + B * B);

            double param = dot / lenSq;

            double xx, yy;
            if (param < 0)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else
            {
                xx = lineStart.X + param * C;
                yy = lineStart.Y + param * D;
            }

            double dx = point.X - xx;
            double dy = point.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private string GenerateFallbackSVG(double width, double height)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     viewBox=""0 0 {width * 96} {height * 96}"" 
     width=""{width}in"" 
     height=""{height}in"">
  <!-- Generated by Cricut Stencil Maker v0.9.0 -->
  <!-- Design Space Compatible SVG -->
  
  <rect width=""100%"" height=""100%"" fill=""white""/>
  <text x=""50%"" y=""50%"" 
        font-family=""Segoe UI"" 
        font-size=""24"" 
        fill=""black"" 
        text-anchor=""middle"" 
        alignment-baseline=""middle""
        dominant-baseline=""middle"">
    Error processing image
  </text>
  
</svg>";
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}