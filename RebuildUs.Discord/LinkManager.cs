using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BepInEx;

namespace RebuildUs.Discord;

public static class LinkManager
{
    private static readonly string FilePath = Path.Combine(Paths.GameRootPath, "DiscordLinks.json");

    // In-Game Name -> Discord User ID
    public static Dictionary<string, ulong> Links { get; private set; } = new Dictionary<string, ulong>();

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                Links = JsonSerializer.Deserialize<Dictionary<string, ulong>>(json) ?? new Dictionary<string, ulong>();
                Plugin.Instance.Log.LogInfo($"Loaded {Links.Count} Discord links from {FilePath}");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Log.LogError($"Failed to load DiscordLinks.json: {ex.Message}");
            }
        }
    }

    public static void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(Links, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Plugin.Instance.Log.LogError($"Failed to save DiscordLinks.json: {ex.Message}");
        }
    }

    public static void UpdateLink(string playerName, ulong discordId)
    {
        Links[playerName] = discordId;
        Save();
    }

    public static bool TryGetDiscordId(string playerName, out ulong discordId)
    {
        return Links.TryGetValue(playerName, out discordId);
    }
}
