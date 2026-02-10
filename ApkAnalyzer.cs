using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ApkAnalyzer
{
    public class ApkInfo
    {
        public string FileName { get; set; }
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public string VersionName { get; set; }
        public string VersionCode { get; set; }
        public string FileSize { get; set; }
        public string MD5 { get; set; }
        public byte[] IconData { get; set; }
    }

    public class ApkAnalyzer
    {
        private string aapt2Path;

        public ApkAnalyzer()
        {
            aapt2Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aapt2.exe");
        }

        public ApkInfo AnalyzeApk(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("文件不存在");

            if (!IsApkFile(filePath))
                throw new Exception("不是有效的APK文件");

            var info = new ApkInfo
            {
                FileName = Path.GetFileName(filePath),
                MD5 = CalculateMD5(filePath),
                FileSize = GetFileSize(filePath)
            };

            // 使用 aapt2 获取 APK 信息
            GetApkInfoFromAapt2(filePath, info);

            // 提取图标
            info.IconData = ExtractIcon(filePath);

            return info;
        }

        private bool IsApkFile(string filePath)
        {
            try
            {
                using (var zip = ZipFile.OpenRead(filePath))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FullName == "AndroidManifest.xml")
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private string GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            double sizeMB = fileInfo.Length / (1024.0 * 1024.0);
            return $"{sizeMB:F2} MB";
        }

        private void GetApkInfoFromAapt2(string filePath, ApkInfo info)
        {
            if (!File.Exists(aapt2Path))
            {
                info.AppName = "未知";
                info.PackageName = "未知";
                info.VersionName = "未知";
                info.VersionCode = "未知";
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = aapt2Path,
                    Arguments = $"dump badging \"{filePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (var process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // 解析输出
                    foreach (var line in output.Split('\n'))
                    {
                        if (line.StartsWith("package:"))
                        {
                            var nameMatch = Regex.Match(line, @"name='([^']+)'");
                            var versionCodeMatch = Regex.Match(line, @"versionCode='([^']+)'");
                            var versionNameMatch = Regex.Match(line, @"versionName='([^']+)'");

                            if (nameMatch.Success) info.PackageName = nameMatch.Groups[1].Value;
                            if (versionCodeMatch.Success) info.VersionCode = versionCodeMatch.Groups[1].Value;
                            if (versionNameMatch.Success) info.VersionName = versionNameMatch.Groups[1].Value;
                        }
                        else if (line.StartsWith("application-label:"))
                        {
                            var labelMatch = Regex.Match(line, @"application-label:'([^']+)'");
                            if (labelMatch.Success) info.AppName = labelMatch.Groups[1].Value;
                        }
                    }
                }

                // 设置默认值
                if (string.IsNullOrEmpty(info.AppName)) info.AppName = "未知";
                if (string.IsNullOrEmpty(info.PackageName)) info.PackageName = "未知";
                if (string.IsNullOrEmpty(info.VersionName)) info.VersionName = "未知";
                if (string.IsNullOrEmpty(info.VersionCode)) info.VersionCode = "未知";
            }
            catch
            {
                info.AppName = "未知";
                info.PackageName = "未知";
                info.VersionName = "未知";
                info.VersionCode = "未知";
            }
        }

        private byte[] ExtractIcon(string filePath)
        {
            try
            {
                using (var zip = ZipFile.OpenRead(filePath))
                {
                    string[] priorities = { "xxxhdpi", "xxhdpi", "xhdpi", "hdpi", "mdpi" };

                    foreach (var priority in priorities)
                    {
                        foreach (var entry in zip.Entries)
                        {
                            if (entry.FullName.Contains($"mipmap-{priority}") &&
                                entry.FullName.Contains("ic_launcher") &&
                                !entry.FullName.Contains("_round") &&
                                !entry.FullName.EndsWith(".xml") &&
                                (entry.FullName.EndsWith(".png") || entry.FullName.EndsWith(".webp")))
                            {
                                using (var stream = entry.Open())
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    return ms.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
