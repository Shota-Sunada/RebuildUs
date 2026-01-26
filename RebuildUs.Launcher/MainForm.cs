using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace RebuildUs.Launcher;

public partial class MainForm : Form
{
    private const string ModFolderName = "Among Us - RU";
    private string? installedModPath;
    private string? detectedOriginalPath;
    private string? lastInstalledVersion;
    private readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher_settings.txt");
    private readonly List<GitHubRelease> releases = [];

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

        this.Shown += async (s, e) => await InitializeLauncher();
    }

    private async Task InitializeLauncher()
    {
        await FetchReleases();
        RefreshStatus();
        CheckForUpdates();
    }

    private async Task FetchReleases()
    {
        try
        {
            lblStatus.Text = "リリース情報を取得中...";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "RebuildUs-Launcher");
            var response = await client.GetStringAsync("https://api.github.com/repos/Shota-Sunada/RebuildUs/releases");
            var result = JsonSerializer.Deserialize<List<GitHubRelease>>(response);

            if (result != null)
            {
                releases.Clear();
                // プレリリースでなく、かつ期待するZIPファイル（RebuildUs-v...-Steam-Itch-Submerged.zip）が含まれるリリースのみを対象とする
                foreach (var release in result)
                {
                    if (release.Prerelease) continue;

                    string expectedName = $"RebuildUs-v{release.TagName.TrimStart('v')}-Steam-Itch-Submerged.zip";
                    if (release.Assets.Any(a => a.Name == expectedName))
                    {
                        releases.Add(release);
                    }
                }

                cmbVersions.Items.Clear();
                foreach (var release in releases)
                {
                    cmbVersions.Items.Add(release.TagName);
                }

                if (cmbVersions.Items.Count > 0)
                {
                    int index = !string.IsNullOrEmpty(lastInstalledVersion) ? cmbVersions.Items.IndexOf(lastInstalledVersion) : -1;
                    if (index >= 0)
                    {
                        cmbVersions.SelectedIndex = index;
                    }
                    else
                    {
                        cmbVersions.SelectedIndex = 0;
                    }
                }
                else
                {
                    lblStatus.Text = "インストール可能なリリースが見つかりませんでした。";
                    btnAction.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "リリース情報の取得に失敗しました。";
            btnAction.Enabled = false;
            MessageBox.Show($"リリースの取得に失敗しました: {ex.Message}");
        }
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                var lines = File.ReadAllLines(settingsPath);
                if (lines.Length > 0) installedModPath = lines[0].Trim();
                // Previously lines[1] was txtUrl.Text, now we ignore it or use as lastInstalledVersion if it looks like one
                if (lines.Length > 2) lastInstalledVersion = lines[2].Trim();
            }
            catch { }
        }
    }

    private void SaveSettings()
    {
        try
        {
            File.WriteAllLines(settingsPath, [installedModPath ?? "", cmbVersions.Text, lastInstalledVersion ?? ""]);
        }
        catch { }
    }

    private void RefreshStatus()
    {
        bool isInstalled = !string.IsNullOrEmpty(installedModPath) && File.Exists(installedModPath);

        // 1. 保存されたパスを優先チェック
        if (isInstalled)
        {
            lblStatus.Text = $"RebuildUs is installed at:\n{Path.GetDirectoryName(installedModPath)}";

            if (cmbVersions.Text != lastInstalledVersion && !string.IsNullOrEmpty(lastInstalledVersion))
            {
                btnAction.Text = "Update";
            }
            else
            {
                btnAction.Text = "Launch";
            }

            btnUninstall.Visible = true;

            // バージョン情報の取得
            lblVersion.Text = "Version: " + GetInstalledModVersion();
        }
        else
        {
            // 2. 自動検出を試みる
            detectedOriginalPath = DetectAmongUs();
            if (detectedOriginalPath != null)
            {
                string parentDir = Path.GetDirectoryName(Path.GetDirectoryName(detectedOriginalPath))!;
                string modExePath = Path.Combine(parentDir, ModFolderName, "Among Us.exe");

                if (File.Exists(modExePath))
                {
                    installedModPath = modExePath;
                    SaveSettings();
                    RefreshStatus();
                    return;
                }
            }

            lblStatus.Text = "RebuildUs is not installed.";
            lblVersion.Text = "Version: -";
            btnAction.Text = "Install";
            btnUninstall.Visible = false;
            installedModPath = null;
        }

        ValidateSelectedVersion();
    }

    private void ValidateSelectedVersion()
    {
        if (cmbVersions.Items.Count == 0)
        {
            btnAction.Enabled = false;
            return;
        }

        btnAction.Enabled = true;
    }

    private void CheckForUpdates()
    {
        if (releases.Count > 0 && !string.IsNullOrEmpty(lastInstalledVersion))
        {
            var latest = releases[0].TagName;
            if (latest != lastInstalledVersion)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(5000, "Update Available", $"新しいバージョン {latest} が利用可能です。アップデートを推奨します。", ToolTipIcon.Info);
            }
        }
    }

    private string GetInstalledModVersion()
    {
        if (string.IsNullOrEmpty(installedModPath)) return "Unknown";

        try
        {
            // BepInExのプラグインフォルダを探す
            string modRootDir = Path.GetDirectoryName(installedModPath)!;
            string pluginPath = Path.Combine(modRootDir, "BepInEx", "plugins", "RebuildUs.dll");

            if (File.Exists(pluginPath))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(pluginPath);
                return versionInfo.ProductVersion ?? versionInfo.FileVersion ?? "Unknown";
            }
        }
        catch { }

        return "Unknown";
    }

    private async void btnAction_Click(object sender, EventArgs e)
    {
        if (installedModPath == null)
        {
            await InstallMod();
        }
        else if (btnAction.Text == "Update")
        {
            await UpdateMod();
        }
        else
        {
            await LaunchGame();
        }
    }

    private void cmbVersions_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshStatus();
    }

    private async Task InstallMod()
    {
        string version = cmbVersions.Text;
        if (string.IsNullOrWhiteSpace(version))
        {
            MessageBox.Show("バージョンを選択してください。");
            return;
        }

        string downloadUrl = $"https://github.com/Shota-Sunada/RebuildUs/releases/download/{version}/RebuildUs-v{version.TrimStart('v')}-Steam-Itch-Submerged.zip";

        string? originalExePath = detectedOriginalPath ?? DetectAmongUs();

        if (originalExePath == null || !File.Exists(originalExePath))
        {
            using (OpenFileDialog ofd = new())
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
            cmbVersions.Enabled = false;

            // 1. ZIPのダウンロード
            lblStatus.Text = "Modをダウンロード中...";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "rebuildus_download.zip");
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"指定されたバージョンのファイルが見つかりません。 (HTTP {response.StatusCode})\nURL: {downloadUrl}");
                    return;
                }
                using (var fs = new FileStream(tempZipPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            // 2. ファイルコピー
            lblStatus.Text = "インストール中... ファイルをコピーしています。";
            await Task.Run(() =>
            {
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                CopyDirectory(originalDir, targetDir);
            });

            // 3. Modの展開
            lblStatus.Text = "インストール中... Modを適用しています。";
            await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, targetDir, true));

            // 一時ファイルの削除
            if (File.Exists(tempZipPath)) File.Delete(tempZipPath);

            installedModPath = newModExePath;
            lastInstalledVersion = version;
            SaveSettings();

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
            cmbVersions.Enabled = true;
        }
    }

    private async Task UpdateMod()
    {
        string version = cmbVersions.Text;
        if (installedModPath == null || string.IsNullOrWhiteSpace(version)) return;

        string downloadUrl = $"https://github.com/Shota-Sunada/RebuildUs/releases/download/{version}/RebuildUs-v{version.TrimStart('v')}-Steam-Itch-Submerged.zip";

        try
        {
            btnAction.Enabled = false;
            cmbVersions.Enabled = false;

            string targetDir = Path.GetDirectoryName(installedModPath)!;

            // 1. ZIPのダウンロード
            lblStatus.Text = "最新のModをダウンロード中...";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "rebuildus_update.zip");
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"指定されたバージョンのファイルが見つかりません。 (HTTP {response.StatusCode})\nURL: {downloadUrl}");
                    return;
                }
                using (var fs = new FileStream(tempZipPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            // 2. Modの展開 (上書き)
            lblStatus.Text = "アップデート中... ファイルを更新しています。";
            await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, targetDir, true));

            // 一時ファイルの削除
            if (File.Exists(tempZipPath)) File.Delete(tempZipPath);

            lastInstalledVersion = version;
            SaveSettings();

            MessageBox.Show("アップデートが完了しました。");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"アップデートの失敗: {ex.Message}");
            lblStatus.Text = "アップデートに失敗しました。";
        }
        finally
        {
            btnAction.Enabled = true;
            cmbVersions.Enabled = true;
        }
    }

    private async void btnUninstall_Click(object sender, EventArgs e)
    {
        await UninstallMod();
    }

    private async Task UninstallMod()
    {
        if (installedModPath == null || !File.Exists(installedModPath))
        {
            MessageBox.Show("Modがインストールされていません。");
            return;
        }

        var result = MessageBox.Show($"{ModFolderName} をアンインストールしますか？\n(Modフォルダ全体が削除されます)", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.No) return;

        try
        {
            string targetDir = Path.GetDirectoryName(installedModPath)!;

            // プロセスが動いているかチェックして終了させるか警告
            var processes = Process.GetProcessesByName("Among Us");
            foreach (var p in processes)
            {
                try
                {
                    if (p.MainModule?.FileName == installedModPath)
                    {
                        var stopResult = MessageBox.Show("Among Us が実行中です。終了して続行しますか？", "警告", MessageBoxButtons.YesNo);
                        if (stopResult == DialogResult.Yes)
                        {
                            p.Kill();
                            await p.WaitForExitAsync();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                catch { } // MainModuleへのアクセス権限がない場合などはスキップ
            }

            if (Directory.Exists(targetDir))
            {
                // 少し待機（プロセスの終了後にファイルロックが解除されるまで）
                await Task.Delay(1000);
                Directory.Delete(targetDir, true);
            }

            installedModPath = null;
            SaveSettings();
            MessageBox.Show("アンインストールが完了しました。");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"アンインストールの失敗: {ex.Message}");
        }
    }

    private async Task LaunchGame()
    {
        if (installedModPath == null || !File.Exists(installedModPath))
        {
            RefreshStatus();
            return;
        }

        ProcessStartInfo si = new(installedModPath)
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
        string[] commonPaths =
        [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\Among Us\Among Us.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Steam\steamapps\common\Among Us\Among Us.exe"),
            @"D:\SteamLibrary\steamapps\common\Among Us\Among Us.exe", // よくあるサブライブラリ
            @"E:\SteamLibrary\steamapps\common\Among Us\Among Us.exe"
        ];

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

public class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }

    [JsonPropertyName("assets")]
    public List<GitHubAsset> Assets { get; set; } = [];
}

public class GitHubAsset
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
