using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ApkAnalyzer
{
    public partial class MainForm : Form
    {
        private ApkAnalyzer analyzer;
        private ApkInfo currentApkInfo;

        public MainForm()
        {
            InitializeComponent();
            analyzer = new ApkAnalyzer();
            
            // 启用拖放
            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string file = files[0];
                if (file.ToLower().EndsWith(".apk"))
                {
                    AnalyzeApkFile(file);
                }
                else
                {
                    MessageBox.Show("请拖入 APK 文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AnalyzeApkFile(string filePath)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                currentApkInfo = analyzer.AnalyzeApk(filePath);
                
                // 显示信息
                txtFileName.Text = currentApkInfo.FileName;
                txtAppName.Text = currentApkInfo.AppName;
                txtPackageName.Text = currentApkInfo.PackageName;
                txtVersionName.Text = currentApkInfo.VersionName;
                txtVersionCode.Text = currentApkInfo.VersionCode;
                txtFileSize.Text = currentApkInfo.FileSize;
                txtMD5.Text = currentApkInfo.MD5;

                // 显示图标
                if (currentApkInfo.IconData != null)
                {
                    using (var ms = new MemoryStream(currentApkInfo.IconData))
                    {
                        picIcon.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    // 加载嵌入的默认图标
                    byte[] defaultIcon = analyzer.GetDefaultIcon();
                    if (defaultIcon != null)
                    {
                        using (var ms = new MemoryStream(defaultIcon))
                        {
                            picIcon.Image = Image.FromStream(ms);
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"分析文件时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCopyFileName_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtFileName.Text);
        }

        private void btnCopyAppName_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtAppName.Text);
        }

        private void btnCopyPackageName_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtPackageName.Text);
        }

        private void btnCopyVersionName_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtVersionName.Text);
        }

        private void btnCopyVersionCode_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtVersionCode.Text);
        }

        private void btnCopyFileSize_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtFileSize.Text);
        }

        private void btnCopyMD5_Click(object sender, EventArgs e)
        {
            CopyToClipboard(txtMD5.Text);
        }

        private void CopyToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
                ShowToast("已复制");
            }
        }

        private void btnSaveIcon_Click(object sender, EventArgs e)
        {
            if (currentApkInfo == null || currentApkInfo.IconData == null)
            {
                ShowToast("当前APK没有图标");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG 图片|*.png";
                sfd.FileName = "app_icon.png";
                
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, currentApkInfo.IconData);
                    ShowToast("图标已保存");
                }
            }
        }

        private void ShowToast(string message)
        {
            lblToast.Text = message;
            lblToast.Visible = true;
            
            var timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += (s, e) =>
            {
                lblToast.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
