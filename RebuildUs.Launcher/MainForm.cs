using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace RebuildUs.Launcher;

public partial class MainForm : Form
{
    private const string ModFolderName = "Among Us - RU";
    private readonly List<GitHubRelease> Releases = [];
    private readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher_settings.txt");
    private string? DetectedOriginalPath;
    private string? InstalledModPath;
    private string? LastInstalledVersion;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();

        // アイコンの設定（簡易的に実行ファイルのアイコンを使用）
        try
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            NotifyIcon.Icon = Icon;
        }
        catch { }

        Shown += async (s, e) => await InitializeLauncher();
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
            LblStatus.Text = "リリース情報を取得中...";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "RebuildUs-Launcher");
            string response = await client.GetStringAsync("https://api.github.com/repos/Shota-Sunada/RebuildUs/releases");
            List<GitHubRelease>? result = JsonSerializer.Deserialize<List<GitHubRelease>>(response);

            if (result != null)
            {
                Releases.Clear();
                // プレリリースでなく、かつ期待するZIPファイル（RebuildUs-v...-Steam-Itch-Submerged.zip）が含まれるリリースのみを対象とする
                foreach (GitHubRelease release in result)
                {
                    if (release.Prerelease)
                    {
                        continue;
                    }

                    string expectedName = $"RebuildUs-v{release.TagName.TrimStart('v')}-Steam-Itch-Submerged.zip";
                    if (release.Assets.Any(a => a.Name == expectedName))
                    {
                        Releases.Add(release);
                    }
                }

                CmbVersions.Items.Clear();
                foreach (GitHubRelease release in Releases)
                {
                    CmbVersions.Items.Add(release.TagName);
                }

                if (CmbVersions.Items.Count > 0)
                {
                    int index = !string.IsNullOrEmpty(LastInstalledVersion) ? CmbVersions.Items.IndexOf(LastInstalledVersion) : -1;
                    if (index >= 0)
                    {
                        CmbVersions.SelectedIndex = index;
                    }
                    else
                    {
                        CmbVersions.SelectedIndex = 0;
                    }
                }
                else
                {
                    LblStatus.Text = "インストール可能なリリースが見つかりませんでした。";
                    BtnAction.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            LblStatus.Text = "リリース情報の取得に失敗しました。";
            BtnAction.Enabled = false;
            MessageBox.Show($"リリースの取得に失敗しました: {ex.Message}");
        }
    }

    private void LoadSettings()
    {
        if (File.Exists(SettingsPath))
        {
            try
            {
                string[] lines = File.ReadAllLines(SettingsPath);
                if (lines.Length > 0)
                {
                    InstalledModPath = lines[0].Trim();
                }
                // Previously lines[1] was txtUrl.Text, now we ignore it or use as lastInstalledVersion if it looks like one
                if (lines.Length > 2)
                {
                    LastInstalledVersion = lines[2].Trim();
                }
            }
            catch { }
        }
    }

    private void SaveSettings()
    {
        try
        {
            File.WriteAllLines(SettingsPath, [InstalledModPath ?? "", CmbVersions.Text, LastInstalledVersion ?? ""]);
        }
        catch { }
    }

    private void RefreshStatus()
    {
        bool isInstalled = !string.IsNullOrEmpty(InstalledModPath) && File.Exists(InstalledModPath);

        // 1. 保存されたパスを優先チェック
        if (isInstalled)
        {
            LblStatus.Text = $"RebuildUs is installed at:\n{Path.GetDirectoryName(InstalledModPath)}";
            BtnUninstall.Visible = true;

            // バージョン情報の取得
            string currentVersion = GetInstalledModVersion();
            LblVersion.Text = "Version: " + currentVersion;

            // コンボボックスで選択されているバージョンと、インストールされているバージョンを比較
            bool needsUpdate = false;
            if (!string.IsNullOrEmpty(CmbVersions.Text))
            {
                if (!string.IsNullOrEmpty(LastInstalledVersion))
                {
                    needsUpdate = CmbVersions.Text != LastInstalledVersion;
                }
                else if (currentVersion != "Unknown")
                {
                    // lastInstalledVersion がない場合は、ファイルバージョンとタグ名を比較（'v'プレフィックスを無視）
                    needsUpdate = CmbVersions.Text.TrimStart('v') != currentVersion.TrimStart('v');
                }
                else
                {
                    // インストールはされているがバージョンが特定できない場合、選択されているものがあればアップデート可能とする
                    needsUpdate = true;
                }
            }

            BtnAction.Text = needsUpdate ? "Update" : "Launch";
        }
        else
        {
            // 2. 自動検出を試みる
            DetectedOriginalPath = DetectAmongUs();
            if (DetectedOriginalPath != null)
            {
                string parentDir = Path.GetDirectoryName(Path.GetDirectoryName(DetectedOriginalPath))!;
                string modFolderName = ModFolderName;
                string modExePath = Path.Combine(parentDir, modFolderName, "Among Us.exe");

                if (File.Exists(modExePath))
                {
                    InstalledModPath = modExePath;
                    // 自動検出時、可能であればバージョンも特定しておく
                    string currentVersion = GetInstalledModVersion();
                    if (currentVersion != "Unknown")
                    {
                        GitHubRelease? matched = Releases.FirstOrDefault(r => r.TagName.TrimStart('v') == currentVersion.TrimStart('v'));
                        if (matched != null)
                        {
                            LastInstalledVersion = matched.TagName;
                        }
                    }
                    SaveSettings();
                    RefreshStatus();
                    return;
                }
            }

            LblStatus.Text = "RebuildUs is not installed.";
            LblVersion.Text = "Version: -";
            BtnAction.Text = "Install";
            BtnUninstall.Visible = false;
            InstalledModPath = null;
        }

        ValidateSelectedVersion();
    }

    private void ValidateSelectedVersion()
    {
        if (CmbVersions.Items.Count == 0)
        {
            BtnAction.Enabled = false;
            return;
        }

        BtnAction.Enabled = true;
    }

    private void CheckForUpdates()
    {
        if (Releases.Count > 0 && !string.IsNullOrEmpty(LastInstalledVersion))
        {
            string latest = Releases[0].TagName;
            if (latest != LastInstalledVersion)
            {
                NotifyIcon.Visible = true;
                NotifyIcon.ShowBalloonTip(5000, "Update Available", $"新しいバージョン {latest} が利用可能です。アップデートを推奨します。", ToolTipIcon.Info);
            }
        }
    }

    private string GetInstalledModVersion()
    {
        if (string.IsNullOrEmpty(InstalledModPath))
        {
            return "Unknown";
        }

        try
        {
            // BepInExのプラグインフォルダを探す
            string modRootDir = Path.GetDirectoryName(InstalledModPath)!;
            string pluginPath = Path.Combine(modRootDir, "BepInEx", "plugins", "RebuildUs.dll");

            if (File.Exists(pluginPath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(pluginPath);
                return versionInfo.ProductVersion ?? versionInfo.FileVersion ?? "Unknown";
            }
        }
        catch { }

        return "Unknown";
    }

    private async void BtnAction_Click(object sender, EventArgs e)
    {
        if (InstalledModPath == null)
        {
            await InstallMod();
        }
        else if (BtnAction.Text == "Update")
        {
            await UpdateMod();
        }
        else
        {
            await LaunchGame();
        }
    }

    private void CmbVersions_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshStatus();
    }

    private async Task InstallMod()
    {
        string version = CmbVersions.Text;
        if (string.IsNullOrWhiteSpace(version))
        {
            MessageBox.Show("バージョンを選択してください。");
            return;
        }

        string downloadUrl = $"https://github.com/Shota-Sunada/RebuildUs/releases/download/{version}/RebuildUs-v{version.TrimStart('v')}-Steam-Itch-Submerged.zip";

        string? originalExePath = DetectedOriginalPath ?? DetectAmongUs();

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

        if (originalExePath == null || !File.Exists(originalExePath))
        {
            return;
        }

        string originalDir = Path.GetDirectoryName(originalExePath)!;
        string parentDir = Path.GetDirectoryName(originalDir)!;
        string targetDir = Path.Combine(parentDir, ModFolderName);
        string newModExePath = Path.Combine(targetDir, "Among Us.exe");

        if (Directory.Exists(targetDir))
        {
            DialogResult result = MessageBox.Show($"{ModFolderName} が既に存在します。上書きしますか？", "確認", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
        }

        try
        {
            BtnAction.Enabled = false;
            CmbVersions.Enabled = false;

            // 1. ZIPのダウンロード
            LblStatus.Text = "Modをダウンロード中...";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "rebuildus_download.zip");
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"指定されたバージョンのファイルが見つかりません。 (HTTP {response.StatusCode})\nURL: {downloadUrl}");
                    return;
                }
                using (FileStream fs = new(tempZipPath, FileMode.Create)) await response.Content.CopyToAsync(fs);
            }

            // 2. ファイルコピー
            LblStatus.Text = "インストール中... ファイルをコピーしています。";
            await Task.Run(() =>
            {
                if (Directory.Exists(targetDir))
                {
                    Directory.Delete(targetDir, true);
                }
                CopyDirectory(originalDir, targetDir);
            });

            // 3. Modの展開
            LblStatus.Text = "インストール中... Modを適用しています。";
            await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, targetDir, true));

            // 一時ファイルの削除
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            InstalledModPath = newModExePath;
            LastInstalledVersion = version;
            SaveSettings();

            MessageBox.Show("インストールが完了しました。");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"インストールの失敗: {ex.Message}");
            LblStatus.Text = "インストールに失敗しました。";
        }
        finally
        {
            BtnAction.Enabled = true;
            CmbVersions.Enabled = true;
        }
    }

    private async Task UpdateMod()
    {
        string version = CmbVersions.Text;
        if (InstalledModPath == null || string.IsNullOrWhiteSpace(version))
        {
            return;
        }

        string downloadUrl = $"https://github.com/Shota-Sunada/RebuildUs/releases/download/{version}/RebuildUs-v{version.TrimStart('v')}-Steam-Itch-Submerged.zip";

        try
        {
            BtnAction.Enabled = false;
            CmbVersions.Enabled = false;

            string targetDir = Path.GetDirectoryName(InstalledModPath)!;

            // 1. ZIPのダウンロード
            LblStatus.Text = "最新のModをダウンロード中...";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "rebuildus_update.zip");
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(downloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"指定されたバージョンのファイルが見つかりません。 (HTTP {response.StatusCode})\nURL: {downloadUrl}");
                    return;
                }
                using (FileStream fs = new(tempZipPath, FileMode.Create)) await response.Content.CopyToAsync(fs);
            }

            // 2. Modの展開 (上書き)
            LblStatus.Text = "アップデート中... ファイルを更新しています。";
            await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, targetDir, true));

            // 一時ファイルの削除
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            LastInstalledVersion = version;
            SaveSettings();

            MessageBox.Show("アップデートが完了しました。");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"アップデートの失敗: {ex.Message}");
            LblStatus.Text = "アップデートに失敗しました。";
        }
        finally
        {
            BtnAction.Enabled = true;
            CmbVersions.Enabled = true;
        }
    }

    private async void BtnUninstall_Click(object sender, EventArgs e)
    {
        await UninstallMod();
    }

    private async Task UninstallMod()
    {
        if (InstalledModPath == null || !File.Exists(InstalledModPath))
        {
            MessageBox.Show("Modがインストールされていません。");
            return;
        }

        DialogResult result = MessageBox.Show($"{ModFolderName} をアンインストールしますか？\n(Modフォルダ全体が削除されます)", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.No)
        {
            return;
        }

        try
        {
            string targetDir = Path.GetDirectoryName(InstalledModPath)!;

            // プロセスが動いているかチェックして終了させるか警告
            Process[] processes = Process.GetProcessesByName("Among Us");
            foreach (Process p in processes)
            {
                try
                {
                    if (p.MainModule?.FileName == InstalledModPath)
                    {
                        DialogResult stopResult = MessageBox.Show("Among Us が実行中です。終了して続行しますか？", "警告", MessageBoxButtons.YesNo);
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

            InstalledModPath = null;
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
        if (InstalledModPath == null || !File.Exists(InstalledModPath))
        {
            RefreshStatus();
            return;
        }

        ProcessStartInfo si = new(InstalledModPath)
        {
            WorkingDirectory = Path.GetDirectoryName(InstalledModPath),
        };

        try
        {
            Process? process = Process.Start(si);
            if (process != null)
            {
                Hide();
                NotifyIcon.Visible = true;
                NotifyIcon.Text = "RebuildUs Running...";
                NotifyIcon.ShowBalloonTip(3000, "RebuildUs", "Game is running. Launcher is hidden in tray.", ToolTipIcon.Info);

                await process.WaitForExitAsync();

                Show();
                WindowState = FormWindowState.Normal;
                Activate();
                NotifyIcon.Visible = false;
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
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 945360");
            if (key != null)
            {
                string? installLocation = key.GetValue("InstallLocation") as string;
                if (!string.IsNullOrEmpty(installLocation))
                {
                    string exePath = Path.Combine(installLocation, "Among Us.exe");
                    if (File.Exists(exePath))
                    {
                        return exePath;
                    }
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
            @"E:\SteamLibrary\steamapps\common\Among Us\Among Us.exe",
        ];

        foreach (string path in commonPaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
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

    private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        ShowLauncher();
    }

    private void MenuShow_Click(object sender, EventArgs e)
    {
        ShowLauncher();
    }

    private void MenuExit_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void ShowLauncher()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
        NotifyIcon.Visible = false;
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