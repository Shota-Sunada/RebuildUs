using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using static RebuildUs.Modules.Cosmetics.CustomHatManager;

namespace RebuildUs.Modules.Cosmetics;

internal sealed class HatsLoader : MonoBehaviour
{
    private static readonly HttpClient Client = new();

    private static readonly JsonSerializerOptions JsonOptions = new() { AllowTrailingCommas = true };

    static HatsLoader()
    {
        Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "RebuildUs-Mod");
    }

    internal bool IsRunning { get; private set; }

    internal void FetchHats()
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
        Task downloadTask = DownloadAllHats(Repositories, HatsDirectory);
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
            foreach (string dir in Directory.GetDirectories(HatsDirectory))
            {
                string manifestPath = Path.Combine(dir, "CustomHats.json");
                if (!File.Exists(manifestPath)) continue;
                try
                {
                    string text = File.ReadAllText(manifestPath);
                    SkinsConfigFile response = JsonSerializer.Deserialize<SkinsConfigFile>(text, new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                    });

                    if (response?.Hats != null)
                    {
                        string dirName = Path.GetFileName(dir);
                        foreach (CustomHat hat in response.Hats)
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

        Logger.LogMessage($"Total {UnregisteredHats.Count} hats registered.");

        LoadScreen.Destroy();
        IsRunning = false;
    }

    private static string SanitizeAndPrefix(string path, string prefix)
    {
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".png")) return null;
        return Path.Combine(prefix, SanitizeFileName(path));
    }

    [HideFromIl2Cpp]
    private async Task DownloadAllHats(IEnumerable<string> repositories, string targetDir)
    {
        try
        {
            if (!Directory.Exists(targetDir))
            {
                if (targetDir != null)
                    Directory.CreateDirectory(targetDir);
            }

            LoadScreen.Progress = 0;
            LoadScreen.ProgressDetailText = "";
            ConcurrentBag<(string url, string localPath, string expectedHash, string fileName)> toDownload = new();

            LoadScreen.StatusText = "Checking repositories...";

            IEnumerable<Task> checkTasks = repositories.Select(async repoUrl =>
            {
                repoUrl = repoUrl.Trim();
                if (string.IsNullOrEmpty(repoUrl)) return;

                string repoFolderName = GetRepoFolderName(repoUrl);
                if (targetDir != null)
                {
                    string repoDir = Path.Combine(targetDir, repoFolderName);
                    if (!Directory.Exists(repoDir)) Directory.CreateDirectory(repoDir);

                    string manifestURL = $"{repoUrl}/CustomHats.json";
                    string manifestPath = Path.Combine(repoDir, "CustomHats.json");

                    try
                    {
                        Logger.LogMessage($"Downloading manifest: {manifestURL}");
                        HttpResponseMessage response = await Client.GetAsync(manifestURL);
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogWarn($"Failed to download manifest from {manifestURL}: {response.StatusCode}");
                            return;
                        }

                        byte[] manifestData = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(manifestPath, manifestData);

                        SkinsConfigFile config = JsonSerializer.Deserialize<SkinsConfigFile>(manifestData, JsonOptions);
                        if (config?.Hats == null) return;

                        foreach (CustomHat hat in config.Hats)
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
                }
            });

            await Task.WhenAll(checkTasks);

            if (!toDownload.IsEmpty)
            {
                int completed = 0;
                int total = toDownload.Count;
                SemaphoreSlim semaphore = new(10); // Limit concurrent downloads

                IEnumerable<Task> tasks = toDownload.Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        LoadScreen.StatusText = $"Downloading {item.fileName}";
                        HttpResponseMessage response = await Client.GetAsync(item.url);
                        if (response.IsSuccessStatusCode)
                        {
                            byte[] fileData = await response.Content.ReadAsByteArrayAsync();
                            await File.WriteAllBytesAsync(item.localPath, fileData);
                        }
                        else
                            Logger.LogWarn($"Failed to download {item.url}: {response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarn($"Failed to download {item.url}: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                        int done = Interlocked.Increment(ref completed);
                        LoadScreen.StatusText = $"Downloading {item.fileName} ({done} / {total})";
                        LoadScreen.Progress = 0.2f + (((float)done / total) * 0.8f);
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

        string safeName = SanitizeFileName(fileName);
        if (string.IsNullOrEmpty(safeName)) return;

        string localPath = Path.Combine(repoDir, safeName);
        if (!NeedsDownload(localPath, expectedHash)) return;
        string fileURL = $"{repoUrl}/hats/{fileName.Replace(" ", "%20")}";
        toDownload.Add((fileURL, localPath, expectedHash, safeName));
    }

    private static string GetRepoFolderName(string url)
    {
        try
        {
            Uri uri = new(url);
            string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2) return $"{segments[0]}-{segments[1]}";
        }
        catch
        {
            // ignored
        }

        return string.Concat("unknown-repo-", Guid.NewGuid().ToString("N").AsSpan(0, 8));
    }

    private static bool NeedsDownload(string path, string expectedHash)
    {
        if (string.IsNullOrEmpty(expectedHash) || !File.Exists(path)) return true;
        using MD5 md5 = MD5.Create();
        using FileStream stream = File.OpenRead(path);
        string hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        return !hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}