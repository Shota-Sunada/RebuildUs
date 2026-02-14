using System.Text.Json;
using InnerNet;

namespace RebuildUs.Modules.Discord;

public static class DiscordEmbedManager
{
    private static ulong? _statusMessageId = null;

    public static async void UpdateStatus()
    {
        if (!MapSettings.EnableDiscordEmbed || !AmongUsClient.Instance.AmHost || string.IsNullOrEmpty(RebuildUs.StatusChannelId.Value)) return;
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
            Title = "Among Us ルーム情報",
            Color = 3447003,
            Fields = new[] {
                new { name = "マップ名", value = mapName, inline = true },
                new { name = "ルームID", value = roomId, inline = true },
                new { name = "状態", value = DiscordModManager.CurrentGameState, inline = true },
                new { name = "状態の説明", value = desc, inline = false },
                new { name = "部屋人数", value = $"{count}/{Helpers.GetOption(Int32OptionNames.MaxPlayers)}", inline = true }
            },
            Description = GetPlayerListString()
        };

        var components = new[] {
            new { Type = 1, Components = new[] {
                new { type = 2, Style = 1, Label = "アカウント連携", Custom_id = "start_link" }
            } }
        };

        var body = new { Embeds = new[] { embed }, components };
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
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data == null) continue;
            var name = p.Data.PlayerName;
            var id = DiscordModManager.GetIdentifier(p);
            if (id != null && DiscordModManager.PlayerMappings.ContainsKey(id)) linked.Add($":white_check_mark: {name}");
            else unlinked.Add($":x: {name}");
        }
        var sb = new StringBuilder("**プレイヤー名一覧:**\n");
        foreach (var s in linked) sb.AppendLine(s);
        foreach (var s in unlinked) sb.AppendLine(s);
        return sb.ToString();
    }

    public static async void SendGameResult()
    {
        if (!MapSettings.EnableDiscordEmbed || !AmongUsClient.Instance.AmHost || string.IsNullOrEmpty(RebuildUs.ResultChannelId.Value)) return;

        var sb = new StringBuilder();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null || p.Data == null) continue;
            var role = RoleInfo.GetRolesString(p, false, true, null, true, " + ");
            sb.Append(p.Data.PlayerName).Append(": ").AppendLine(role);
        }

        var logs = new StringBuilder();
        foreach (var d in GameHistory.DeadPlayers)
        {
            var time = d.TimeOfDeath.AddHours(9).ToString("HH:mm:ss");
            var victimName = d.Player.Data.PlayerName;
            var victimRole = RoleInfo.GetRolesString(d.Player, false, true, null, true, " + ");

            var killerName = "不明";
            var killerRole = "";

            if (d.KillerIfExisting != null && d.KillerIfExisting.Data != null)
            {
                killerName = d.KillerIfExisting.Data.PlayerName;
                killerRole = RoleInfo.GetRolesString(d.KillerIfExisting, false, true, null, true, " + ");
            }
            else if (d.DeathReason == DeathReason.Exile)
            {
                killerName = "投票";
            }

            var reason = d.DeathReason switch
            {
                DeathReason.Exile => "追放",
                DeathReason.Kill => "キル",
                _ => d.DeathReason.ToString()
            };

            logs.Append(time).Append(' ');
            if (d.DeathReason == DeathReason.Exile)
            {
                logs.Append("投票 => ").Append(victimName).Append(" (").Append(victimRole).Append(") [").Append(reason).Append("]");
            }
            else
            {
                logs.Append(killerName);
                if (!string.IsNullOrEmpty(killerRole)) logs.Append(" (").Append(killerRole).Append(")");
                logs.Append(" => ").Append(victimName).Append(" (").Append(victimRole).Append(") [").Append(reason).Append("]");
            }
            logs.AppendLine();
        }

        var embed = new
        {
            title = "試合結果",
            color = 15105570,
            fields = new[] {
                new { Name = "役職一覧", value = sb.Length > 0 ? sb.ToString() : "なし", inline = false },
                new { Name = "キル記録", value = logs.Length > 0 ? logs.ToString() : "なし", inline = false }
            }
        };

        var body = new { embeds = new[] { embed } };
        await DiscordModManager.SendRequest("POST", $"https://discord.com/api/v10/channels/{RebuildUs.ResultChannelId.Value}/messages", body);
    }
}