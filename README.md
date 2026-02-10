# APK 文件分析工具 - WinForms 版本

## 特点

- ✅ 启动速度快（< 1秒）
- ✅ 体积小（约 10-15MB）
- ✅ 基于 .NET Framework 4.8（Windows 自带，无需额外安装）
- ✅ 界面美观，功能完整
- ✅ 支持拖拽 APK 文件
- ✅ 一键复制所有信息
- ✅ 保存应用图标

## 本地编译

### 前置要求

- Visual Studio 2019/2022（Community 版本免费）
- .NET Framework 4.8 SDK

### 编译步骤

1. 打开 Visual Studio
2. 打开 `ApkAnalyzer.csproj` 项目文件
3. 确保 `aapt2.exe` 和 `res/android.png` 在项目目录中
4. 点击"生成" -> "生成解决方案"
5. 编译完成后，可执行文件在 `bin/Release/` 目录中

### 命令行编译

```cmd
# 使用 MSBuild 编译
msbuild ApkAnalyzer.csproj /p:Configuration=Release /p:Platform="Any CPU"
```

## GitHub Actions 自动编译

项目已配置 GitHub Actions，推送代码后会自动编译。

### 触发编译

1. 推送代码到 `main` 或 `master` 分支
2. 修改 `WinForms/` 目录下的文件
3. 手动触发工作流

### 下载编译产物

1. 进入 GitHub 仓库的 "Actions" 标签
2. 选择最新的工作流运行
3. 下载 "APK分析工具-WinForms" 构建产物

### 创建 Release

推送标签即可自动创建 Release：

```bash
git tag v1.0.0
git push origin v1.0.0
```

## 使用说明

1. 运行 `APK分析工具.exe`
2. 拖拽 APK 文件到窗口
3. 查看分析结果
4. 点击"复制"按钮复制信息
5. 点击"保存图标"保存应用图标

## 文件结构

```
WinForms/
├── ApkAnalyzer.csproj      # 项目文件
├── Program.cs              # 程序入口
├── ApkAnalyzer.cs          # APK 分析逻辑
├── MainForm.cs             # 主窗体代码
├── MainForm.Designer.cs    # 窗体设计器代码
├── MainForm.resx           # 窗体资源文件
├── aapt2.exe              # Android 资源分析工具
└── res/
    └── android.png        # 默认图标
```

## 与 Electron 版本对比

| 特性 | WinForms | Electron |
|------|----------|----------|
| 启动速度 | < 1秒 | 2-3秒 |
| 文件大小 | 10-15MB | 70MB+ |
| 内存占用 | 30-50MB | 100-150MB |
| 跨平台 | ❌ 仅 Windows | ✅ 全平台 |
| 界面美观度 | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 开发难度 | 简单 | 中等 |

## 技术栈

- C# / .NET Framework 4.8
- Windows Forms
- System.IO.Compression（ZIP 解析）
- aapt2.exe（APK 信息提取）

## 许可证

MIT License
