namespace RebuildUs.Impostor.Models;

public sealed class DiscordConfig
{
    public bool DisableDiscord { get; set; }
    public bool AutoMute { get; set; } = true;
    public string Token { get; set; } = string.Empty;
    public List<string> WorkerTokens { get; set; } = [];
    public ulong GuildId { get; set; }
    public ulong VoiceChannelId { get; set; }
    public ulong TextChannelId { get; set; }
}
