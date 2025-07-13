using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CricutStencilMaker.WinForms
{
    public partial class MainForm : Form
    {
        private Label statusLabel;
        private ProgressBar progressBar;
        private Button processButton;
        private CheckBox removeBackgroundCheck;
        private ComboBox matSizeCombo;
        private Panel dropPanel;
        private string? currentImagePath;

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

            // Create main layout
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20)
            };

            // Header
            var headerPanel = new Panel { Height = 80, Dock = DockStyle.Top };
            var titleLabel = new Label
            {
                Text = "🎨 Cricut Stencil Maker",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 180, 166),
                Location = new Point(20, 10),
                AutoSize = true
            };
            var subtitleLabel = new Label
            {
                Text = "Turn any image into a perfect Cricut stencil instantly!",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 50),
                AutoSize = true
            };
            headerPanel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });

            // Drop zone
            dropPanel = new Panel
            {
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                AllowDrop = true
            };
            dropPanel.DragEnter += DropPanel_DragEnter;
            dropPanel.DragDrop += DropPanel_DragDrop;
            dropPanel.Click += DropPanel_Click;

            var dropLabel = new Label
            {
                Text = "📁 Drag & Drop Your Image Here\n\nSupports PNG, JPEG, BMP, TIFF, GIF\n\nOr click to browse files",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            dropPanel.Controls.Add(dropLabel);

            // Settings panel
            var settingsPanel = new Panel { Height = 120 };
            var settingsGroup = new GroupBox
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            removeBackgroundCheck = new CheckBox
            {
                Text = "Remove background automatically",
                Checked = true,
                Location = new Point(20, 30),
                AutoSize = true
            };

            var matSizeLabel = new Label
            {
                Text = "Mat Size:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            matSizeCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(100, 57),
                Width = 200
            };
            matSizeCombo.Items.AddRange(new[] {
                "11.5 x 11.5 inch Cricut Mat",
                "23.5 x 11.5 inch Cricut Mat",
                "Custom Size"
            });
            matSizeCombo.SelectedIndex = 0;

            processButton = new Button
            {
                Text = "🚀 Process & Export SVG",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 180, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 45,
                Location = new Point(350, 45),
                Width = 200,
                Enabled = false
            };
            processButton.Click += ProcessButton_Click;

            settingsGroup.Controls.AddRange(new Control[] {
                removeBackgroundCheck, matSizeLabel, matSizeCombo, processButton
            });
            settingsPanel.Controls.Add(settingsGroup);

            // Status panel
            var statusPanel = new Panel { Height = 80, Dock = DockStyle.Bottom };
            statusLabel = new Label
            {
                Text = "Ready - Drop an image to begin",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 20),
                AutoSize = true
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 45),
                Width = 600,
                Height = 20,
                Visible = false
            };

            statusPanel.Controls.AddRange(new Control[] { statusLabel, progressBar });

            // Add all panels to main form
            mainPanel.Controls.AddRange(new Control[] { headerPanel, dropPanel, settingsPanel, statusPanel });
            this.Controls.Add(mainPanel);

            // Set up drag and drop for the entire form
            this.DragEnter += DropPanel_DragEnter;
            this.DragDrop += DropPanel_DragDrop;
        }

        private void DropPanel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
                dropPanel.BackColor = Color.FromArgb(230, 255, 250);
            }
        }

        private void DropPanel_DragDrop(object? sender, DragEventArgs e)
        {
            dropPanel.BackColor = Color.White;
            
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                LoadImage(files[0]);
            }
        }

        private void DropPanel_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.gif;*.webp|All Files|*.*",
                Title = "Select Image for Stencil Creation"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(openFileDialog.FileName);
            }
        }

        private void LoadImage(string filePath)
        {
            try
            {
                currentImagePath = filePath;
                var fileName = Path.GetFileName(filePath);
                statusLabel.Text = $"Loaded: {fileName}";
                processButton.Enabled = true;

                // Update drop panel to show image loaded
                dropPanel.Controls.Clear();
                var imageLabel = new Label
                {
                    Text = $"✅ Image Loaded\n\n{fileName}\n\nReady to process!",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 180, 166),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                dropPanel.Controls.Add(imageLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ProcessButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
                return;

            try
            {
                processButton.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                statusLabel.Text = "Processing image...";
                await Task.Delay(500);

                // Get settings
                var removeBackground = removeBackgroundCheck.Checked;
                var matSize = matSizeCombo.SelectedIndex switch
                {
                    0 => "11in",
                    1 => "23in",
                    _ => "11in"
                };

                // Choose output file
                using var saveFileDialog = new SaveFileDialog
                {
                    Filter = "SVG Files|*.svg",
                    Title = "Save Stencil As",
                    FileName = Path.GetFileNameWithoutExtension(currentImagePath) + "_stencil.svg"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    statusLabel.Text = "Generating Design Space compatible SVG...";
                    await Task.Delay(1000);

                    // Generate SVG content (this would call the actual processing logic)
                    var svgContent = GenerateDesignSpaceCompatibleSVG(matSize);
                    await File.WriteAllTextAsync(saveFileDialog.FileName, svgContent);

                    statusLabel.Text = "✅ Stencil created successfully!";
                    
                    var result = MessageBox.Show(
                        $"Stencil saved successfully!\n\n{saveFileDialog.FileName}\n\nWould you like to open the folder?",
                        "Success!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        Process.Start("explorer.exe", $"/select,\"{saveFileDialog.FileName}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing image: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error occurred during processing";
            }
            finally
            {
                processButton.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private static string GenerateDesignSpaceCompatibleSVG(string size)
        {
            // Parse size
            double width = 11.5, height = 11.5;
            
            switch (size.ToLower())
            {
                case "11in":
                    width = height = 11.5;
                    break;
                case "23in":
                    width = 23.5;
                    height = 11.5;
                    break;
            }

            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     viewBox=""0 0 {width * 96} {height * 96}"" 
     width=""{width}in"" 
     height=""{height}in"">
  <!-- Generated by Cricut Stencil Maker v0.9.0 -->
  <!-- Design Space Compatible: Path-only SVG -->
  
  <!-- Sample stencil path - in real implementation this would be generated from image processing -->
  <path d=""M{width * 48 - 100},100 L{width * 48 + 100},100 L{width * 48 + 100},200 L{width * 48 - 100},200 Z 
           M{width * 48 - 80},120 L{width * 48 + 80},120 L{width * 48 + 80},180 L{width * 48 - 80},180 Z""
        fill=""black""
        fill-rule=""evenodd""/>
        
  <!-- Additional sample paths -->
  <path d=""M{width * 48 - 50},250 L{width * 48 + 50},250 L{width * 48 + 50},350 L{width * 48 - 50},350 Z""
        fill=""black""/>
  
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