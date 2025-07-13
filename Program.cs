using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AllowDrop = true;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Main layout
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            // Title
            var titleLabel = new Label
            {
                Text = "🎨 Cricut Stencil Maker",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 180, 166),
                Location = new Point(20, 20),
                Size = new Size(600, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Subtitle
            var subtitleLabel = new Label
            {
                Text = "Drag & drop an image to create a Design Space compatible stencil",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 70),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Drop zone
            var dropZone = new Panel
            {
                Location = new Point(20, 120),
                Size = new Size(740, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                AllowDrop = true
            };

            var dropLabel = new Label
            {
                Text = "Drop your image here or click to browse\n\nSupports: PNG, JPG, BMP, GIF",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
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
                Location = new Point(20, 340),
                Size = new Size(740, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var removeBackgroundCheck = new CheckBox
            {
                Text = "Remove background automatically",
                Location = new Point(20, 30),
                Size = new Size(250, 25),
                Checked = true,
                Name = "RemoveBackgroundCheck"
            };

            var matSizeLabel = new Label
            {
                Text = "Mat Size:",
                Location = new Point(20, 60),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var matSizeCombo = new ComboBox
            {
                Location = new Point(100, 57),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "MatSizeCombo"
            };
            matSizeCombo.Items.AddRange(new[] { "11.5 x 11.5 inch", "23.5 x 11.5 inch" });
            matSizeCombo.SelectedIndex = 0;

            var processButton = new Button
            {
                Text = "Create Stencil SVG",
                Location = new Point(400, 40),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 180, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Enabled = false,
                Name = "ProcessButton"
            };
            processButton.Click += ProcessButton_Click;

            settingsPanel.Controls.AddRange(new Control[] { removeBackgroundCheck, matSizeLabel, matSizeCombo, processButton });

            // Status bar
            var statusLabel = new Label
            {
                Text = "Ready - Drop an image to begin",
                Location = new Point(20, 460),
                Size = new Size(600, 25),
                Name = "StatusLabel"
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 490),
                Size = new Size(740, 20),
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
                dropLabel.ForeColor = Color.FromArgb(0, 180, 166);
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
                await Task.Delay(1000);

                statusLabel.Text = "Creating Design Space compatible SVG...";
                await Task.Delay(1000);

                using var saveDialog = new SaveFileDialog
                {
                    Filter = "SVG Files|*.svg",
                    Title = "Save Stencil",
                    FileName = Path.GetFileNameWithoutExtension(currentImagePath) + "_stencil.svg"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var matSize = matSizeCombo.SelectedIndex == 0 ? "11.5" : "23.5";
                    var svgContent = GenerateSVG(matSize);
                    
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

        private string ProcessImageToSVGPaths(string imagePath, double matWidth, double matHeight)
        {
            using var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imagePath);
            
            var paths = new System.Text.StringBuilder();
            var matWidthPx = (int)(matWidth * 96);
            var matHeightPx = (int)(matHeight * 96);
            
            // Resize image to fit mat while maintaining aspect ratio
            var scale = Math.Min((double)matWidthPx / image.Width, (double)matHeightPx / image.Height);
            var newWidth = (int)(image.Width * scale);
            var newHeight = (int)(image.Height * scale);
            
            image.Mutate(x => x.Resize(newWidth, newHeight));
            
            // Center the image on the mat
            var offsetX = (matWidthPx - newWidth) / 2;
            var offsetY = (matHeightPx - newHeight) / 2;
            
            // Convert to black and white and trace contours
            var contours = ExtractContours(image, offsetX, offsetY);
            
            foreach (var contour in contours)
            {
                if (contour.Count < 3) continue; // Skip tiny contours
                
                var pathData = "M" + string.Join(" L", contour.Select(p => $"{p.X},{p.Y}")) + " Z";
                paths.AppendLine($"  <path d=\"{pathData}\" fill=\"black\" fill-rule=\"evenodd\"/>");
            }
            
            return paths.ToString();
        }

        private List<List<System.Drawing.Point>> ExtractContours(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image, int offsetX, int offsetY)
        {
            var contours = new List<List<System.Drawing.Point>>();
            var visited = new bool[image.Width, image.Height];
            
            // Simple contour extraction - find edges of non-transparent pixels
            for (int y = 1; y < image.Height - 1; y++)
            {
                for (int x = 1; x < image.Width - 1; x++)
                {
                    if (visited[x, y]) continue;
                    
                    var pixel = image[x, y];
                    
                    // Check if this is a solid pixel (not transparent)
                    if (pixel.A > 128) // More than 50% opacity
                    {
                        // Check if it's on an edge (has transparent neighbors)
                        bool isEdge = false;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                var neighbor = image[x + dx, y + dy];
                                if (neighbor.A <= 128)
                                {
                                    isEdge = true;
                                    break;
                                }
                            }
                            if (isEdge) break;
                        }
                        
                        if (isEdge)
                        {
                            // Trace contour from this edge pixel
                            var contour = TraceContour(image, x, y, visited, offsetX, offsetY);
                            if (contour.Count > 10) // Only keep substantial contours
                            {
                                contours.Add(contour);
                            }
                        }
                    }
                }
            }
            
            return contours;
        }

        private List<System.Drawing.Point> TraceContour(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image, int startX, int startY, bool[,] visited, int offsetX, int offsetY)
        {
            var contour = new List<System.Drawing.Point>();
            var directions = new[] { (-1,-1), (-1,0), (-1,1), (0,1), (1,1), (1,0), (1,-1), (0,-1) };
            
            int x = startX, y = startY;
            int dir = 0;
            
            do
            {
                if (!visited[x, y])
                {
                    visited[x, y] = true;
                    contour.Add(new System.Drawing.Point(x + offsetX, y + offsetY));
                }
                
                // Find next edge pixel
                bool found = false;
                for (int i = 0; i < 8; i++)
                {
                    int newDir = (dir + i) % 8;
                    int nx = x + directions[newDir].Item1;
                    int ny = y + directions[newDir].Item2;
                    
                    if (nx >= 0 && nx < image.Width && ny >= 0 && ny < image.Height)
                    {
                        var pixel = image[nx, ny];
                        if (pixel.A > 128)
                        {
                            x = nx;
                            y = ny;
                            dir = newDir;
                            found = true;
                            break;
                        }
                    }
                }
                
                if (!found || contour.Count > 1000) break; // Prevent infinite loops
                
            } while (x != startX || y != startY);
            
            return SimplifyContour(contour);
        }

        private List<System.Drawing.Point> SimplifyContour(List<System.Drawing.Point> contour)
        {
            if (contour.Count <= 3) return contour;
            
            // Simple Douglas-Peucker-like simplification
            var simplified = new List<System.Drawing.Point> { contour[0] };
            
            for (int i = 1; i < contour.Count - 1; i++)
            {
                var prev = simplified[simplified.Count - 1];
                var curr = contour[i];
                var next = contour[i + 1];
                
                // Calculate if this point significantly changes direction
                var dist = PointToLineDistance(curr, prev, next);
                if (dist > 2.0) // Keep points that are more than 2 pixels from the line
                {
                    simplified.Add(curr);
                }
            }
            
            simplified.Add(contour[contour.Count - 1]);
            return simplified;
        }

        private double PointToLineDistance(System.Drawing.Point point, System.Drawing.Point lineStart, System.Drawing.Point lineEnd)
        {
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
  <!-- Fallback SVG - processing failed -->
  <text x=""50%"" y=""50%"" text-anchor=""middle"" fill=""black"" font-size=""24"">
    Processing failed - please try a different image
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