using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RebuildUs.Impostor.Services;

public interface IPlayerMappingService
{
    void Map(string friendCode, ulong discordId);
    ulong? GetDiscordId(string friendCode);
    void Save();
    void Load();
}

public class PlayerMappingService : IPlayerMappingService
{
    private readonly string _filePath = "player_mappings.json";
    private Dictionary<string, ulong> _mappings = [];
    private readonly ILogger<PlayerMappingService> _logger;

    public PlayerMappingService(ILogger<PlayerMappingService> logger)
    {
        _logger = logger;
        Load();
    }

    public void Map(string friendCode, ulong discordId)
    {
        _mappings[friendCode] = discordId;
        Save();
    }

    public ulong? GetDiscordId(string friendCode)
    {
        return _mappings.TryGetValue(friendCode, out var id) ? id : null;
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_mappings);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save player mappings.");
        }
    }

    public void Load()
    {
        if (!File.Exists(_filePath)) return;

        try
        {
            var json = File.ReadAllText(_filePath);
            _mappings = JsonSerializer.Deserialize<Dictionary<string, ulong>>(json) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load player mappings.");
        }
    }
}