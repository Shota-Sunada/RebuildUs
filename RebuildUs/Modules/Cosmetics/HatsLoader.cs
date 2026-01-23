using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Security.Cryptography;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using static RebuildUs.Modules.Cosmetics.CustomHatManager;

namespace RebuildUs.Modules.Cosmetics;

public class HatsLoader : MonoBehaviour
{
    public bool IsRunning { get; private set; }
    private static readonly HttpClient Client = new();

    public void FetchHats()
    {
        if (IsRunning) return;
        this.StartCoroutine(CoFetchHats());
    }

    [HideFromIl2Cpp]
    private IEnumerator CoFetchHats()
    {
        while (Camera.main == null) yield return null;

        IsRunning = true;
        LoadScreen.Create();

        Logger.LogMessage("Downloading cosmetics...");
        var downloadTask = DownloadAllHats(Repositories, HatsDirectory);
        while (!downloadTask.IsCompleted)
        {
            LoadScreen.Update();
            yield return null;
        }
        Logger.LogMessage("Downloading finished.");

        UnregisteredHats.Clear();
        LoadScreen.StatusText = "Loading hats...";
        LoadScreen.Progress = 1.0f;
        LoadScreen.ProgressDetailText = "";
        LoadScreen.Update();
        if (Directory.Exists(HatsDirectory))
        {
            foreach (var dir in Directory.GetDirectories(HatsDirectory))
            {
                var manifestPath = Path.Combine(dir, "CustomHats.json");
                if (File.Exists(manifestPath))
                {
                    try
                    {
                        var text = File.ReadAllText(manifestPath);
                        var response = JsonSerializer.Deserialize<SkinsConfigFile>(text, new JsonSerializerOptions
                        {
                            AllowTrailingCommas = true
                        });

                        if (response?.Hats != null)
                        {
                            var dirName = Path.GetFileName(dir);
                            foreach (var hat in response.Hats)
                            {
                                hat.Resource = SanitizeAndPrefix(hat.Resource, dirName);
                                hat.BackResource = SanitizeAndPrefix(hat.BackResource, dirName);
                                hat.ClimbResource = SanitizeAndPrefix(hat.ClimbResource, dirName);
                                hat.FlipResource = SanitizeAndPrefix(hat.FlipResource, dirName);
                                hat.BackFlipResource = SanitizeAndPrefix(hat.BackFlipResource, dirName);
                            }
                            UnregisteredHats.AddRange(response.Hats);
                            Logger.LogMessage($"Loaded {response.Hats.Count} hats from {manifestPath}.");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to load hats from {manifestPath}: {e}");
                    }
                }
            }
        }

        Logger.LogMessage($"Total {UnregisteredHats.Count} hats registered.");

        LoadScreen.Destroy();
        IsRunning = false;
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { AllowTrailingCommas = true };

    private string SanitizeAndPrefix(string path, string prefix)
    {
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".png")) return null;
        return Path.Combine(prefix, CustomHatManager.SanitizeFileName(path));
    }

    [HideFromIl2Cpp]
    private async Task DownloadAllHats(string[] repositories, string targetDir)
    {
        try
        {
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            LoadScreen.Progress = 0;
            LoadScreen.ProgressDetailText = "";
            var toDownload = new ConcurrentBag<(string url, string localPath, string expectedHash, string fileName)>();

            LoadScreen.StatusText = "Checking repositories...";

            var checkTasks = repositories.Select(async repoUrl =>
            {
                repoUrl = repoUrl.Trim();
                if (string.IsNullOrEmpty(repoUrl)) return;

                var repoFolderName = GetRepoFolderName(repoUrl);
                var repoDir = Path.Combine(targetDir, repoFolderName);
                if (!Directory.Exists(repoDir)) Directory.CreateDirectory(repoDir);

                var manifestURL = $"{repoUrl}/CustomHats.json";
                var manifestPath = Path.Combine(repoDir, "CustomHats.json");

                try
                {
                    Logger.LogMessage($"Downloading manifest: {manifestURL}");
                    var manifestData = await Client.GetByteArrayAsync(manifestURL);
                    if (manifestData == null) return;
                    await File.WriteAllBytesAsync(manifestPath, manifestData);

                    var config = JsonSerializer.Deserialize<SkinsConfigFile>(manifestData, JsonOptions);
                    if (config == null || config.Hats == null) return;

                    foreach (var hat in config.Hats)
                    {
                        ProcessHatFile(repoUrl, repoDir, hat.Resource, hat.ResHashA, toDownload);
                        ProcessHatFile(repoUrl, repoDir, hat.BackResource, hat.ResHashB, toDownload);
                        ProcessHatFile(repoUrl, repoDir, hat.ClimbResource, hat.ResHashC, toDownload);
                        ProcessHatFile(repoUrl, repoDir, hat.FlipResource, hat.ResHashF, toDownload);
                        ProcessHatFile(repoUrl, repoDir, hat.BackFlipResource, hat.ResHashBf, toDownload);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarn($"Failed to check repository {repoUrl}: {ex.Message}");
                }
            });

            await Task.WhenAll(checkTasks);

            if (toDownload.Count > 0)
            {
                int completed = 0;
                int total = toDownload.Count;
                var semaphore = new SemaphoreSlim(10); // Limit concurrent downloads

                var tasks = toDownload.Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        LoadScreen.StatusText = $"Downloading {item.fileName}";
                        var fileData = await Client.GetByteArrayAsync(item.url);
                        if (fileData != null)
                        {
                            await File.WriteAllBytesAsync(item.localPath, fileData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarn($"Failed to download {item.url}: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                        var done = Interlocked.Increment(ref completed);
                        LoadScreen.StatusText = $"Downloading {item.fileName} ({done} / {total})";
                        LoadScreen.Progress = 0.2f + ((float)done / total * 0.8f);
                    }
                });

                await Task.WhenAll(tasks);
            }
            LoadScreen.Progress = 1.0f;
            LoadScreen.ProgressDetailText = "";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Download error: {ex.Message}");
            LoadScreen.StatusText = $"Error: {ex.Message}";
            await Task.Delay(2000);
        }
    }

    private void ProcessHatFile(string repoUrl, string repoDir, string fileName, string expectedHash, ConcurrentBag<(string url, string localPath, string expectedHash, string fileName)> toDownload)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var safeName = CustomHatManager.SanitizeFileName(fileName);
        if (string.IsNullOrEmpty(safeName)) return;

        var localPath = Path.Combine(repoDir, safeName);
        if (NeedsDownload(localPath, expectedHash))
        {
            var fileURL = $"{repoUrl}/hats/{safeName.Replace(" ", "%20")}";
            toDownload.Add((fileURL, localPath, expectedHash, safeName));
        }
    }

    private string SanitizeFileName(string path)
    {
        if (string.IsNullOrEmpty(path) || !path.ToLower().EndsWith(".png")) return "";
        return CustomHatManager.SanitizeFileName(path);
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