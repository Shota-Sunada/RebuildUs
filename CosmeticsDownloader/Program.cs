using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmeticsDownloader
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(args));
        }
    }

    public class MainForm : Form
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private TextBox logBox;
        private string[] args;
        private static readonly HttpClient client = new();

        [DllImport("ntdll.dll")]
        private static extern int NtSuspendProcess(IntPtr processHandle);

        [DllImport("ntdll.dll")]
        private static extern int NtResumeProcess(IntPtr processHandle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;

        public MainForm(string[] args)
        {
            this.args = args;
            this.Text = "Rebuild-Us Cosmetics Downloader";
            this.Size = new System.Drawing.Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.TopMost = true;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(10) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            statusLabel = new Label { Text = "Initializing...", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, AutoEllipsis = true };
            progressBar = new ProgressBar { Dock = DockStyle.Fill, Maximum = 100 };
            logBox = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };

            layout.Controls.Add(statusLabel, 0, 0);
            layout.Controls.Add(progressBar, 0, 1);
            layout.Controls.Add(logBox, 0, 2);

            this.Controls.Add(layout);

            this.Shown += (s, e) =>
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                this.Activate();
                Task.Run(async () => await StartUpdate());
            };
        }

        private void AppendLog(string msg)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                try { this.Invoke(new Action<string>(AppendLog), msg); } catch { }
                return;
            }
            var line = $"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}";
            logBox.AppendText(line);
            try { File.AppendAllText("CosmeticsDownloader.log", line); } catch { }
        }

        private async Task StartUpdate()
        {
            try
            {
                string repoURLs = null;
                string targetDir = null;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-urls" && i + 1 < args.Length) repoURLs = args[++i];
                    if (args[i] == "-dir" && i + 1 < args.Length) targetDir = args[++i];
                }

                if (string.IsNullOrEmpty(repoURLs) || string.IsNullOrEmpty(targetDir))
                {
                    AppendLog("Error: Missing required arguments (-urls, -dir).");
                    this.Invoke(new Action(() => statusLabel.Text = "Error: Missing arguments"));
                    return;
                }

                AppendLog("Starting update process...");

                var pids = Process.GetProcessesByName("Among Us").Select(p => p.Id).ToList();
                foreach (var pid in pids)
                {
                    AppendLog($"Suspending Among Us (PID: {pid})...");
                    var process = Process.GetProcessById(pid);
                    NtSuspendProcess(process.Handle);
                }

                try
                {
                    if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                    var urls = repoURLs.Split(',');
                    var allDownloadItems = new List<DownloadItem>();

                    for (int i = 0; i < urls.Length; i++)
                    {
                        var url = urls[i].Trim();
                        if (string.IsNullOrEmpty(url)) continue;

                        var repoFolderName = GetRepoFolderName(url);
                        var repoDir = Path.Combine(targetDir, repoFolderName);
                        if (!Directory.Exists(repoDir)) Directory.CreateDirectory(repoDir);

                        this.Invoke(new Action(() =>
                        {
                            statusLabel.Text = $"Downloading manifest from: {url}";
                        }));
                        var manifestURL = $"{url}/CustomHats.json";
                        var manifestPath = Path.Combine(repoDir, "CustomHats.json");

                        AppendLog($"Downloading manifest: {manifestURL}");
                        var manifestData = await client.GetByteArrayAsync(manifestURL);
                        await File.WriteAllBytesAsync(manifestPath, manifestData);

                        var config = JsonSerializer.Deserialize<SkinsConfigFile>(manifestData, new JsonSerializerOptions { AllowTrailingCommas = true });
                        if (config == null || config.Hats == null) continue;

                        foreach (var hat in config.Hats)
                        {
                            var files = new[] { hat.Resource, hat.BackResource, hat.ClimbResource, hat.FlipResource, hat.BackFlipResource };
                            var hashes = new[] { hat.ResHashA, hat.ResHashB, hat.ResHashC, hat.ResHashF, hat.ResHashBf };

                            for (int fIdx = 0; fIdx < files.Length; fIdx++)
                            {
                                var fileName = files[fIdx];
                                var expectedHash = hashes[fIdx];
                                if (string.IsNullOrEmpty(fileName)) continue;

                                var safeName = SanitizeFileName(fileName);
                                if (string.IsNullOrEmpty(safeName)) continue;

                                var localPath = Path.Combine(repoDir, safeName);
                                var fileURL = $"{url}/hats/{safeName.Replace(" ", "%20")}";

                                allDownloadItems.Add(new DownloadItem
                                {
                                    URL = fileURL,
                                    LocalPath = localPath,
                                    ExpectedHash = expectedHash,
                                    FileName = safeName,
                                    RepoURL = url
                                });
                            }
                        }
                    }

                    // 重複を排除 (同じローカルパスへのダウンロード)
                    allDownloadItems = allDownloadItems.GroupBy(x => x.LocalPath).Select(g => g.First()).ToList();

                    int totalItems = allDownloadItems.Count;
                    int completedItems = 0;
                    var semaphore = new SemaphoreSlim(8); // 同時ダウンロード数を8に制限

                    if (totalItems > 0)
                    {
                        var tasks = allDownloadItems.Select(async item =>
                        {
                            await semaphore.WaitAsync();
                            try
                            {
                                if (NeedsDownload(item.LocalPath, item.ExpectedHash))
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        statusLabel.Text = $"Downloading from: {item.RepoURL}";
                                    }));
                                    AppendLog($"Downloading {item.FileName}...");
                                    var fileData = await client.GetByteArrayAsync(item.URL);
                                    await File.WriteAllBytesAsync(item.LocalPath, fileData);
                                }
                            }
                            catch (Exception ex)
                            {
                                AppendLog($"Failed to download {item.FileName}: {ex.Message}");
                            }
                            finally
                            {
                                Interlocked.Increment(ref completedItems);
                                this.Invoke(new Action(() =>
                                {
                                    int percent = Math.Min(100, (int)((double)completedItems / totalItems * 100));
                                    progressBar.Value = percent;
                                    this.Text = $"[{completedItems}/{totalItems}] ({percent}%) Rebuild-Us Cosmetics Downloader";
                                }));
                                semaphore.Release();
                            }
                        });

                        await Task.WhenAll(tasks);
                    }

                    AppendLog("Update finished successfully.");
                    this.Invoke(new Action(() => statusLabel.Text = "Finished. Closing in 3 seconds..."));
                }
                catch (Exception ex)
                {
                    AppendLog($"Error: {ex.Message}");
                    this.Invoke(new Action(() => statusLabel.Text = "Error occurred."));
                }
                finally
                {
                    foreach (var pid in pids)
                    {
                        try
                        {
                            AppendLog($"Resuming Among Us (PID: {pid})...");
                            var process = Process.GetProcessById(pid);
                            NtResumeProcess(process.Handle);
                        }
                        catch { }
                    }
                    await Task.Delay(3000);
                    this.Invoke(new Action(() => this.Close()));
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Critical Error: {ex.Message}");
            }
        }

        private string SanitizeFileName(string path)
        {
            if (!path.ToLower().EndsWith(".png")) return "";
            return path.Replace("\\", "").Replace("/", "").Replace("*", "").Replace("..", "");
        }
        private string GetRepoFolderName(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2)
                {
                    return $"{segments[0]}-{segments[1]}";
                }
            }
            catch { }
            return "unknown-repo-" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }
        private bool NeedsDownload(string path, string expectedHash)
        {
            if (string.IsNullOrEmpty(expectedHash) || !File.Exists(path)) return true;
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            return !hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class SkinsConfigFile
    {
        [JsonPropertyName("hats")] public List<CustomHat> Hats { get; set; }
    }

    public class DownloadItem
    {
        public string URL { get; set; }
        public string LocalPath { get; set; }
        public string ExpectedHash { get; set; }
        public string FileName { get; set; }
        public string RepoURL { get; set; }
    }

    public class CustomHat
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("resource")] public string Resource { get; set; }
        [JsonPropertyName("backresource")] public string BackResource { get; set; }
        [JsonPropertyName("climbresource")] public string ClimbResource { get; set; }
        [JsonPropertyName("flipresource")] public string FlipResource { get; set; }
        [JsonPropertyName("backflipresource")] public string BackFlipResource { get; set; }
        [JsonPropertyName("reshasha")] public string ResHashA { get; set; }
        [JsonPropertyName("reshashb")] public string ResHashB { get; set; }
        [JsonPropertyName("reshashc")] public string ResHashC { get; set; }
        [JsonPropertyName("reshashf")] public string ResHashF { get; set; }
        [JsonPropertyName("reshashbf")] public string ResHashBf { get; set; }
    }
}
