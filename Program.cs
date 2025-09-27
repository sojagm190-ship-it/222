using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace SystemWarningApp
{
    public class MainForm : Form
    {
        private static int windowCount = 0;
        private const int maxWindows = 6;
        private Random random = new Random();
        private System.Windows.Forms.Timer spawnTimer;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        public MainForm()
        {
            InitializeForm();
            CreateControls();
            StartSpawning();
        }

        private void InitializeForm()
        {
            this.Text = "Windows Security";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = GetRandomLocation();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            this.BackColor = Color.White;

            // 移除关闭按钮
            int style = GetWindowLong(this.Handle, GWL_STYLE);
            SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_SYSMENU);

            // 阻止关闭
            this.FormClosing += (s, e) => {
                e.Cancel = true;
                SpawnNewWindow();
                MoveToRandomLocation();
            };
        }

        private void CreateControls()
        {
            // 标题面板
            var titlePanel = new Panel();
            titlePanel.BackColor = Color.FromArgb(0, 120, 215);
            titlePanel.Size = new Size(500, 40);
            titlePanel.Location = new Point(0, 0);
            
            var titleLabel = new Label();
            titleLabel.Text = "Windows Security";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            titleLabel.Size = new Size(200, 30);
            titleLabel.Location = new Point(15, 5);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // 警告图标和文本
            var warningIcon = new Label();
            warningIcon.Text = "⚠️";
            warningIcon.Font = new Font("Segoe UI", 28);
            warningIcon.ForeColor = Color.Red;
            warningIcon.Size = new Size(60, 60);
            warningIcon.Location = new Point(20, 60);

            var mainWarning = new Label();
            mainWarning.Text = "CRITICAL SYSTEM WARNING";
            mainWarning.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            mainWarning.ForeColor = Color.Red;
            mainWarning.Size = new Size(300, 25);
            mainWarning.Location = new Point(90, 65);

            var detailText = new Label();
            detailText.Text = "Threat detected: Trojan:Win32/MaliciousBehavior\n\n" +
                            "Your system security has been compromised.\n" +
                            "Immediate action is required to protect your data.";
            detailText.Font = new Font("Segoe UI", 9);
            detailText.Size = new Size(450, 80);
            detailText.Location = new Point(20, 100);

            // 按钮
            var actionButton = new Button();
            actionButton.Text = "Remove Threat";
            actionButton.Size = new Size(120, 35);
            actionButton.Location = new Point(150, 220);
            actionButton.BackColor = Color.FromArgb(0, 120, 215);
            actionButton.ForeColor = Color.White;
            actionButton.FlatStyle = FlatStyle.Flat;
            actionButton.Click += (s, e) => {
                ShowFakeScan();
                SpawnNewWindow();
            };

            var cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(120, 35);
            cancelButton.Location = new Point(280, 220);
            cancelButton.Click += (s, e) => {
                SpawnNewWindow();
                SpawnNewWindow();
            };

            // 窗口计数显示
            var countLabel = new Label();
            countLabel.Text = $"Windows: {windowCount + 1}/{maxWindows}";
            countLabel.ForeColor = Color.Gray;
            countLabel.Font = new Font("Segoe UI", 8);
            countLabel.Size = new Size(100, 20);
            countLabel.Location = new Point(380, 300);

            // 添加所有控件
            titlePanel.Controls.Add(titleLabel);
            this.Controls.AddRange(new Control[] { 
                titlePanel, warningIcon, mainWarning, detailText, 
                actionButton, cancelButton, countLabel 
            });
        }

        private void StartSpawning()
        {
            spawnTimer = new System.Windows.Forms.Timer();
            spawnTimer.Interval = 3000;
            spawnTimer.Tick += (s, e) => {
                if (windowCount < maxWindows)
                {
                    SpawnNewWindow();
                }
            };
            spawnTimer.Start();
        }

        private void SpawnNewWindow()
        {
            if (windowCount >= maxWindows) return;
            
            windowCount++;
            
            Thread thread = new Thread(() => {
                Application.Run(new MainForm());
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        private void MoveToRandomLocation()
        {
            this.Location = GetRandomLocation();
        }

        private Point GetRandomLocation()
        {
            var screen = Screen.PrimaryScreen.Bounds;
            int x = random.Next(50, Math.Max(51, screen.Width - this.Width - 50));
            int y = random.Next(50, Math.Max(51, screen.Height - this.Height - 50));
            return new Point(x, y);
        }

        private void ShowFakeScan()
        {
            var scanForm = new Form();
            scanForm.Text = "System Scan";
            scanForm.Size = new Size(400, 200);
            scanForm.StartPosition = FormStartPosition.CenterScreen;
            scanForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            scanForm.MaximizeBox = false;
            scanForm.MinimizeBox = false;
            
            var progressBar = new ProgressBar();
            progressBar.Size = new Size(350, 20);
            progressBar.Location = new Point(20, 80);
            progressBar.Style = ProgressBarStyle.Marquee;
            
            var label = new Label();
            label.Text = "Scanning your system for threats...\nThis may take a few minutes.";
            label.Size = new Size(350, 50);
            label.Location = new Point(20, 30);
            label.TextAlign = ContentAlignment.MiddleCenter;
            
            scanForm.Controls.AddRange(new Control[] { label, progressBar });
            scanForm.ShowDialog();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
