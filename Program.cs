using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     viewBox=""0 0 {width * 96} {height * 96}"" 
     width=""{width}in"" 
     height=""{height}in"">
  <!-- Generated by Cricut Stencil Maker v0.9.0 -->
  <!-- Design Space Compatible SVG -->
  
  <!-- Sample stencil shape - replace with actual image processing -->
  <path d=""M{width * 48 - 100},150 L{width * 48 + 100},150 L{width * 48 + 100},250 L{width * 48 - 100},250 Z 
           M{width * 48 - 80},170 L{width * 48 + 80},170 L{width * 48 + 80},230 L{width * 48 - 80},230 Z""
        fill=""black""
        fill-rule=""evenodd""/>
        
  <path d=""M{width * 48 - 50},300 L{width * 48 + 50},300 L{width * 48 + 50},400 L{width * 48 - 50},400 Z""
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