using System.Drawing;
using System.Windows.Forms;

namespace ApkAnalyzer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox picIcon;
        private TextBox txtFileName;
        private TextBox txtAppName;
        private TextBox txtPackageName;
        private TextBox txtVersionName;
        private TextBox txtVersionCode;
        private TextBox txtFileSize;
        private TextBox txtMD5;
        private Button btnCopyFileName;
        private Button btnCopyAppName;
        private Button btnCopyPackageName;
        private Button btnCopyVersionName;
        private Button btnCopyVersionCode;
        private Button btnCopyFileSize;
        private Button btnCopyMD5;
        private Button btnSaveIcon;
        private Button btnMinimize;
        private Button btnClose;
        private Label lblToast;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // 窗体设置
            this.Text = "APK 文件分析工具";
            this.Size = new Size(600, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft YaHei UI", 9F);

            // 顶部拖动区域
            Panel dragPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(520, 50),
                BackColor = Color.Transparent
            };
            dragPanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(this.Handle, 0xA1, 0x2, 0);
                }
            };
            this.Controls.Add(dragPanel);

            // 最小化按钮
            btnMinimize = new Button
            {
                Location = new Point(520, 15),
                Size = new Size(32, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Text = "—",
                Font = new Font("Arial", 12F, FontStyle.Bold),
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnMinimize.Click += btnMinimize_Click;
            this.Controls.Add(btnMinimize);

            // 关闭按钮
            btnClose = new Button
            {
                Location = new Point(555, 15),
                Size = new Size(32, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Text = "✕",
                Font = new Font("Arial", 12F, FontStyle.Bold),
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.Gray;
            btnClose.Click += btnClose_Click;
            this.Controls.Add(btnClose);

            // Toast 提示
            lblToast = new Label
            {
                Location = new Point(200, 20),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(220, 0, 0, 0),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false,
                Font = new Font("Microsoft YaHei UI", 9F)
            };
            this.Controls.Add(lblToast);

            // 图标
            picIcon = new PictureBox
            {
                Location = new Point(240, 70),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(picIcon);

            int yPos = 210;
            int labelWidth = 80;
            int textBoxWidth = 340;
            int buttonWidth = 60;
            int spacing = 45;

            // 文件名
            AddInfoRow("文件名", ref txtFileName, ref btnCopyFileName, yPos);
            yPos += spacing;

            // 应用名称
            AddInfoRow("应用名称", ref txtAppName, ref btnCopyAppName, yPos);
            yPos += spacing;

            // 包名
            AddInfoRow("包名", ref txtPackageName, ref btnCopyPackageName, yPos);
            yPos += spacing;

            // 版本名
            AddInfoRow("版本名", ref txtVersionName, ref btnCopyVersionName, yPos);
            yPos += spacing;

            // 版本号
            AddInfoRow("版本号", ref txtVersionCode, ref btnCopyVersionCode, yPos);
            yPos += spacing;

            // 文件大小
            AddInfoRow("文件大小", ref txtFileSize, ref btnCopyFileSize, yPos);
            yPos += spacing;

            // MD5
            AddInfoRow("MD5 值", ref txtMD5, ref btnCopyMD5, yPos);
            yPos += spacing;

            // 保存图标按钮
            btnSaveIcon = new Button
            {
                Location = new Point(30, yPos + 10),
                Size = new Size(540, 40),
                Text = "保存图标",
                BackColor = Color.FromArgb(61, 220, 132),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveIcon.FlatAppearance.BorderSize = 0;
            btnSaveIcon.Click += btnSaveIcon_Click;
            this.Controls.Add(btnSaveIcon);

            this.ResumeLayout(false);
        }

        private void AddInfoRow(string labelText, ref TextBox textBox, ref Button button, int yPos)
        {
            Label label = new Label
            {
                Location = new Point(30, yPos + 5),
                Size = new Size(80, 20),
                Text = labelText,
                ForeColor = Color.FromArgb(102, 102, 102),
                Font = new Font("Microsoft YaHei UI", 9F)
            };
            this.Controls.Add(label);

            textBox = new TextBox
            {
                Location = new Point(120, yPos),
                Size = new Size(340, 30),
                ReadOnly = true,
                BackColor = Color.FromArgb(249, 249, 249),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(textBox);

            button = new Button
            {
                Location = new Point(470, yPos),
                Size = new Size(100, 30),
                Text = "复制",
                BackColor = Color.FromArgb(61, 220, 132),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei UI", 9F),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            this.Controls.Add(button);
        }
    }

    // 用于窗口拖动的 Win32 API
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}
