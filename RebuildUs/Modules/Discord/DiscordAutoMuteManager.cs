namespace RebuildUs.Modules.Discord;

public static class DiscordAutoMuteManager
{
    public static async void SetMute(ulong discordId, bool mute, bool deaf)
    {
        if (!ModMapOptions.EnableDiscordAutoMute) return;
        var guildId = RebuildUs.DiscordGuildId.Value;
        if (string.IsNullOrEmpty(guildId)) return;

        var targetVcId = RebuildUs.DiscordVCId.Value;
        if (!string.IsNullOrEmpty(targetVcId))
        {
            if (!DiscordModManager.PlayerVoiceStates.TryGetValue(discordId, out var currentVcId) || currentVcId != targetVcId)
            {
                return;
            }
        }

        await DiscordModManager.SendRequest("PATCH", $"https://discord.com/api/v10/guilds/{guildId}/members/{discordId}", new { mute, deaf });
    }

    public static void UnmuteEveryone()
    {
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p != null && !string.IsNullOrEmpty(p.FriendCode) && DiscordModManager.TryGetDiscordId(p.FriendCode, out var did))
                SetMute(did, false, false);
        }
    }

    public static void MuteEveryone()
    {
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p != null && !string.IsNullOrEmpty(p.FriendCode) && DiscordModManager.TryGetDiscordId(p.FriendCode, out var did))
                SetMute(did, true, true);
        }
    }

    public static void OnPlayerLeft(string friendCode)
    {
        if (!string.IsNullOrEmpty(friendCode) && DiscordModManager.TryGetDiscordId(friendCode, out var did))
            SetMute(did, false, false);
    }

    public static void UpdatePlayerMute(PlayerControl p)
    {
        if (!AmongUsClient.Instance.AmHost || p == null || p.Data == null || string.IsNullOrEmpty(p.FriendCode)) return;
        if (!DiscordModManager.TryGetDiscordId(p.FriendCode, out var did)) return;

        bool mute = false;
        bool deaf = false;

        switch (DiscordModManager.CurrentGameState)
        {
            case "行動中":
            case "追放中":
                if (!p.Data.IsDead) { mute = true; deaf = true; }
                else { mute = false; deaf = false; }
                break;
            case "会議中":
                if (!p.Data.IsDead) { mute = false; deaf = false; }
                else { mute = true; deaf = false; }
                break;
            case "ドロップシップ":
                mute = false;
                deaf = false;
                break;
        }

        SetMute(did, mute, deaf);
    }
}
