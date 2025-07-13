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

            settingsPanel.Controls.AddRange(new Control[] { removeBackgroundCheck, matSizeLabel, matSizeCombo, processButton });

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
            using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath);

            var paths = new System.Text.StringBuilder();
            var matWidthPx = (int)(matWidth * 96);
            var matHeightPx = (int)(matHeight * 96);

            // Resize image to fit mat while maintaining aspect ratio
            var scale = Math.Min((double)matWidthPx / image.Width, (double)matHeightPx / image.Height);
            var newWidth = (int)(image.Width * scale);
            var newHeight = (int)(image.Height * scale);

            image.Mutate(x => x.Resize(newWidth, newHeight).DetectEdges());

            // Center the image on the mat
            var offsetX = (matWidthPx - newWidth) / 2;
            var offsetY = (matHeightPx - newHeight) / 2;

            // Trace contours
            var contours = ExtractContours(image, offsetX, offsetY);

            foreach (var contour in contours)
            {
                if (contour.Count < 3) continue; // Skip tiny contours

                var pathData = "M" + string.Join(" L", contour.Select(p => $"{p.X},{p.Y}")) + " Z";
                paths.AppendLine($"  <path d=\"{pathData}\" fill=\"black\" fill-rule=\"evenodd\"/>");
            }

            return paths.ToString();
        }

        private List<List<SystemDrawing.Point>> ExtractContours(SixLabors.ImageSharp.Image<Rgba32> image, int offsetX, int offsetY)
        {
            var contours = new List<List<SystemDrawing.Point>>();
            var visited = new bool[image.Width, image.Height];

            // Convert image to grayscale and apply binary thresholding
            image.Mutate(x => x.Grayscale().BinaryThreshold(0.5f));

            for (int y = 1; y < image.Height - 1; y++)
            {
                for (int x = 1; x < image.Width - 1; x++)
                {
                    if (visited[x, y]) continue;

                    var pixel = image[x, y];

                    if (pixel.A > 128) // More than 50% opacity
                    {
                        var contour = TraceContour(image, x, y, visited, offsetX, offsetY);
                        if (contour != null)
                        {
                            contours.Add(contour);
                        }
                    }
                }
            }

            return contours;
        }

        private List<SystemDrawing.Point> TraceContour(SixLabors.ImageSharp.Image<Rgba32> image, int startX, int startY, bool[,] visited, int offsetX, int offsetY)
        {
            var contour = new List<SystemDrawing.Point>();
            var stack = new Stack<SystemDrawing.Point>();
            stack.Push(new SystemDrawing.Point(startX, startY));

            while (stack.Count > 0)
            {
                var point = stack.Pop();

                if (visited[point.X, point.Y]) continue;

                visited[point.X, point.Y] = true;
                contour.Add(new SystemDrawing.Point(point.X - offsetX, point.Y - offsetY));

                // 8-connectivity
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var nx = point.X + dx;
                        var ny = point.Y + dy;

                        if (nx >= 0 && ny >= 0 && nx < image.Width && ny < image.Height && !visited[nx, ny])
                        {
                            var neighbor = image[nx, ny];
                            if (neighbor.A > 128) // More than 50% opacity
                            {
                                stack.Push(new SystemDrawing.Point(nx, ny));
                            }
                        }
                    }
                }
            }

            return contour;
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