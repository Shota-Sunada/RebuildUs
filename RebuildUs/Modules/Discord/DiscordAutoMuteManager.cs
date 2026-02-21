namespace RebuildUs.Modules.Discord;

internal static class DiscordAutoMuteManager
{
    internal static async void SetMute(ulong discordId, bool mute, bool deaf)
    {
        if (!MapSettings.EnableDiscordAutoMute) return;
        string guildId = RebuildUs.DiscordGuildId.Value;
        if (string.IsNullOrEmpty(guildId)) return;

        string targetVcId = RebuildUs.DiscordVcId.Value;
        if (!string.IsNullOrEmpty(targetVcId))
            if (!DiscordModManager.PlayerVoiceStates.TryGetValue(discordId, out string currentVcId) || currentVcId != targetVcId)
                return;

        await DiscordModManager.SendRequest("PATCH", $"https://discord.com/api/v10/guilds/{guildId}/members/{discordId}", new { mute, deaf });
    }

    internal static void UnmuteEveryone()
    {
        foreach (ulong did in DiscordModManager.PlayerVoiceStates.Keys.ToArray()) SetMute(did, false, false);
    }

    internal static void MuteEveryone()
    {
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            string id = DiscordModManager.GetIdentifier(p);
            if (id != null && DiscordModManager.TryGetDiscordId(id, out ulong did))
                SetMute(did, true, true);
        }
    }

    internal static void OnPlayerLeft(string friendCode)
    {
        if (!string.IsNullOrEmpty(friendCode) && DiscordModManager.TryGetDiscordId(friendCode, out ulong did))
            SetMute(did, false, false);
    }

    internal static void UpdatePlayerMute(PlayerControl p)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        string id = DiscordModManager.GetIdentifier(p);
        if (id == null || !DiscordModManager.TryGetDiscordId(id, out ulong did)) return;

        bool mute = false;
        bool deaf = false;

        switch (DiscordModManager.CurrentGameState)
        {
            case "行動中":
            case "追放中":
                if (!p.Data.IsDead && p.PlayerId != DiscordModManager.ExiledPlayerId)
                {
                    mute = true;
                    deaf = true;
                }
                else
                {
                    mute = false;
                    deaf = false;
                }

                break;
            case "会議中":
                mute = p.Data.IsDead;

                deaf = false;

                break;
            case "ドロップシップ":
                mute = false;
                deaf = false;
                break;
        }

        SetMute(did, mute, deaf);
    }
}