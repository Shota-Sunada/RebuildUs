using System.Diagnostics;
using System.IO.Compression;
using Microsoft.Win32;

namespace RebuildUs.Launcher;

public partial class MainForm : Form
{
    private const string ModFolderName = "Among Us - RU";
    private const string ModZipName = "rebuildus_mod.zip";
    private string? installedModPath;
    private string? detectedOriginalPath;
    private readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher_settings.txt");

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();

        // アイコンの設定（簡易的に実行ファイルのアイコンを使用）
        try
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Icon = this.Icon;
        }
        catch { }

        RefreshStatus();
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            try { installedModPath = File.ReadAllText(settingsPath).Trim(); } catch { }
        }
    }

    private void SaveSettings(string? path)
    {
        try
        {
            if (path != null) File.WriteAllText(settingsPath, path);
            else if (File.Exists(settingsPath)) File.Delete(settingsPath);
        }
        catch { }
    }

    private void RefreshStatus()
    {
        // 1. 保存されたパスを優先チェック
        if (!string.IsNullOrEmpty(installedModPath) && File.Exists(installedModPath))
        {
            lblStatus.Text = $"RebuildUs is installed at:\n{Path.GetDirectoryName(installedModPath)}";
            btnAction.Text = "Launch";
            return;
        }

        // 2. 自動検出を試みる
        detectedOriginalPath = DetectAmongUs();
        if (detectedOriginalPath != null)
        {
            string parentDir = Path.GetDirectoryName(Path.GetDirectoryName(detectedOriginalPath))!;
            string modExePath = Path.Combine(parentDir, ModFolderName, "Among Us.exe");

            if (File.Exists(modExePath))
            {
                installedModPath = modExePath;
                SaveSettings(installedModPath);
                lblStatus.Text = $"RebuildUs is installed at:\n{Path.GetDirectoryName(modExePath)}";
                btnAction.Text = "Launch";
                return;
            }
        }

        // 既知の場所に見つからない場合
        lblStatus.Text = "RebuildUs is not installed.";
        btnAction.Text = "Install";
        installedModPath = null;
    }

    private async void btnAction_Click(object sender, EventArgs e)
    {
        if (installedModPath == null)
        {
            await InstallMod();
        }
        else
        {
            await LaunchGame();
        }
    }

    private async Task InstallMod()
    {
        string? originalExePath = detectedOriginalPath ?? DetectAmongUs();

        if (originalExePath == null || !File.Exists(originalExePath))
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Among Us.exe|Among Us.exe";
                ofd.Title = "Among Us.exe を選択してください";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originalExePath = ofd.FileName;
                }
            }
        }

        if (originalExePath == null || !File.Exists(originalExePath)) return;

        string originalDir = Path.GetDirectoryName(originalExePath)!;
        string parentDir = Path.GetDirectoryName(originalDir)!;
        string targetDir = Path.Combine(parentDir, ModFolderName);
        string newModExePath = Path.Combine(targetDir, "Among Us.exe");

        if (Directory.Exists(targetDir))
        {
            var result = MessageBox.Show($"{ModFolderName} が既に存在します。上書きしますか？", "確認", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;
        }

        try
        {
            btnAction.Enabled = false;
            lblStatus.Text = "インストール中... ファイルをコピーしています。";

            await Task.Run(() =>
            {
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                CopyDirectory(originalDir, targetDir);
            });

            lblStatus.Text = "インストール中... Modを適用しています。";

            string fullZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ModZipName);
            if (File.Exists(fullZipPath))
            {
                await Task.Run(() => ZipFile.ExtractToDirectory(fullZipPath, targetDir, true));
            }

            installedModPath = newModExePath;
            SaveSettings(installedModPath);
            MessageBox.Show("インストールが完了しました。");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"インストールの失敗: {ex.Message}");
            lblStatus.Text = "インストールに失敗しました。";
        }
        finally
        {
            btnAction.Enabled = true;
        }
    }

    private async Task LaunchGame()
    {
        if (installedModPath == null || !File.Exists(installedModPath))
        {
            RefreshStatus();
            return;
        }

        ProcessStartInfo si = new ProcessStartInfo(installedModPath)
        {
            WorkingDirectory = Path.GetDirectoryName(installedModPath)
        };

        try
        {
            Process? process = Process.Start(si);
            if (process != null)
            {
                this.Hide();
                notifyIcon.Visible = true;
                notifyIcon.Text = "RebuildUs Running...";
                notifyIcon.ShowBalloonTip(3000, "RebuildUs", "Game is running. Launcher is hidden in tray.", ToolTipIcon.Info);

                await process.WaitForExitAsync();

                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                notifyIcon.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"起動に失敗しました: {ex.Message}");
        }
    }

    private string? DetectAmongUs()
    {
        // 1. Steamのレジストリから取得を試みる
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 945360");
            if (key != null)
            {
                string? installLocation = key.GetValue("InstallLocation") as string;
                if (!string.IsNullOrEmpty(installLocation))
                {
                    string exePath = Path.Combine(installLocation, "Among Us.exe");
                    if (File.Exists(exePath)) return exePath;
                }
            }
        }
        catch { }

        // 2. よくあるパスをチェック
        string[] commonPaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\Among Us\Among Us.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Steam\steamapps\common\Among Us\Among Us.exe"),
            @"D:\SteamLibrary\steamapps\common\Among Us\Among Us.exe", // よくあるサブライブラリ
            @"E:\SteamLibrary\steamapps\common\Among Us\Among Us.exe"
        };

        foreach (var path in commonPaths)
        {
            if (File.Exists(path)) return path;
        }

        return null;
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string dest = Path.Combine(targetDir, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }

        foreach (string folder in Directory.GetDirectories(sourceDir))
        {
            string dest = Path.Combine(targetDir, Path.GetFileName(folder));
            CopyDirectory(folder, dest);
        }
    }

    private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        ShowLauncher();
    }

    private void menuShow_Click(object sender, EventArgs e)
    {
        ShowLauncher();
    }

    private void menuExit_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void ShowLauncher()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.Activate();
        notifyIcon.Visible = false;
    }
}
