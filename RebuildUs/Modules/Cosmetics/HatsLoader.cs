using System;
using System.Collections;
using System.IO;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using RebuildUs.Utilities;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;
using static RebuildUs.Modules.Cosmetics.CustomHatManager;

namespace RebuildUs.Modules.Cosmetics;

public class HatsLoader : MonoBehaviour
{
    private bool IsRunning;

    public void FetchHats()
    {
        if (IsRunning) return;
        this.StartCoroutine(CoFetchHats());
    }

    [HideFromIl2Cpp]
    private IEnumerator CoFetchHats()
    {
        IsRunning = true;

        var assemblyLocation = typeof(HatsLoader).Assembly.Location;
        var exePath = Path.Combine(Path.GetDirectoryName(assemblyLocation), "CosmeticsDownloader.exe");

        if (File.Exists(exePath))
        {
            Logger.LogMessage("Running external cosmetics downloader...");
            var urls = string.Join(",", Repositories);
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"-urls \"{urls}\" -dir \"{HatsDirectory}\"",
                CreateNoWindow = false,
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process != null)
            {
                while (!process.HasExited)
                {
                    yield return null;
                }
            }
            Logger.LogMessage("External downloader finished.");
        }
        else
        {
            Logger.LogWarn($"CosmeticsDownloader.exe not found at {exePath}.");
        }

        UnregisteredHats.Clear();
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

        IsRunning = false;
    }

    private string SanitizeAndPrefix(string path, string prefix)
    {
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".png")) return null;
        var sanitized = path.Replace("\\", "").Replace("/", "").Replace("*", "").Replace("..", "");
        return System.IO.Path.Combine(prefix, sanitized);
    }
}