namespace RebuildUs.Impostor.Models;

public class DiscordConfig
{
    public bool DisableDiscord { get; set; }
    public string Token { get; set; } = string.Empty;
    public List<string> WorkerTokens { get; set; } = [];
    public ulong GuildId { get; set; }
    public ulong VoiceChannelId { get; set; }
    public ulong TextChannelId { get; set; }
}
