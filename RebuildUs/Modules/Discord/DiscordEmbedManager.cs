using System.Text.Json;
using InnerNet;

namespace RebuildUs.Modules.Discord;

public static class DiscordEmbedManager
{
    private static ulong? _statusMessageId = null;

    public static async void UpdateStatus()
    {
        if (!ModMapOptions.EnableDiscordEmbed || !AmongUsClient.Instance.AmHost || string.IsNullOrEmpty(RebuildUs.StatusChannelId.Value)) return;
        if (DiscordModManager.CurrentGameState == "ドロップシップ") return;

        var mapName = Helpers.GetOption(ByteOptionNames.MapId) switch
        {
            0 or 3 => TranslationController.Instance.GetString(StringNames.MapNameSkeld),
            1 => TranslationController.Instance.GetString(StringNames.MapNameMira),
            2 => TranslationController.Instance.GetString(StringNames.MapNamePolus),
            4 => TranslationController.Instance.GetString(StringNames.MapNameAirship),
            5 => TranslationController.Instance.GetString(StringNames.MapNameFungle),
            6 => SubmergedCompatibility.SUBMERGED_GUID,
            _ => "Unknown"
        };
        var roomId = GameCode.IntToGameName(AmongUsClient.Instance.GameId) ?? "---";
        var count = PlayerControl.AllPlayerControls.Count;

        string desc = DiscordModManager.CurrentGameState switch
        {
            "ドロップシップ" => "ゲーム開始を待機中",
            "行動中" => "各自タスクやキルを行う",
            "追放中" => string.IsNullOrEmpty(DiscordModManager.ExiledPlayerName) ? "誰かが追放された" : $"{DiscordModManager.ExiledPlayerName}が追放された",
            "会議中" => "話し合いをしている",
            _ => ""
        };

        var embed = new
        {
            title = "Among Us ルーム情報",
            color = 3447003,
            fields = new[] {
                new { name = "マップ名", value = mapName, inline = true },
                new { name = "ルームID", value = roomId, inline = true },
                new { name = "状態", value = DiscordModManager.CurrentGameState, inline = true },
                new { name = "状態の説明", value = desc, inline = false },
                new { name = "部屋人数", value = $"{count}/{Helpers.GetOption(Int32OptionNames.MaxPlayers)}", inline = true }
            },
            description = GetPlayerListString()
        };

        var components = new[] {
            new { type = 1, components = new[] {
                new { type = 2, style = 1, label = "アカウント連携", custom_id = "start_link" }
            } }
        };

        var body = new { embeds = new[] { embed }, components };
        var url = $"https://discord.com/api/v10/channels/{RebuildUs.StatusChannelId.Value}/messages" + (_statusMessageId == null ? "" : $"/{_statusMessageId}");
        var resp = await DiscordModManager.SendRequest(_statusMessageId == null ? "POST" : "PATCH", url, body, RebuildUs.DiscordBotToken.Value);
        if (_statusMessageId == null && resp != null && resp.IsSuccessStatusCode)
        {
            var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("id", out var id)) _statusMessageId = ulong.Parse(id.GetString()!);
        }
    }

    private static string GetPlayerListString()
    {
        var linked = new List<string>();
        var unlinked = new List<string>();
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.Data == null) continue;
            var name = p.Data.PlayerName;
            if (!string.IsNullOrEmpty(p.FriendCode) && DiscordModManager.PlayerMappings.ContainsKey(p.FriendCode)) linked.Add($":white_check_mark: {name}");
            else unlinked.Add($":x: {name}");
        }
        var sb = new StringBuilder("**プレイヤー名一覧:**\n");
        foreach (var s in linked) sb.AppendLine(s);
        foreach (var s in unlinked) sb.AppendLine(s);
        return sb.ToString();
    }
}
