using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using RebuildUs.Discord.Services;

namespace RebuildUs.Discord.Patches;

[HarmonyPatch]
public static class GameStatePatcher
{
    private static bool IsHost => AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost;

    private static async Task TriggerMuteUpdate(bool isMeeting, bool isMeetingEnd = false, bool isGameEnd = false)
    {
        if (!IsHost) return;

        var requests = new List<DiscordMultiBotService.MuteRequest>();

        foreach (var player in GameData.Instance.AllPlayers)
        {
            if (player == null || player.Disconnected) continue;

            if (LinkManager.TryGetDiscordId(player.PlayerName, out var discordId))
            {
                var isDead = player.IsDead;

                bool mute;
                bool deafen;
                if (isGameEnd)
                {
                    // ゲーム終了時: 全員ミュート解除
                    mute = false;
                    deafen = false;
                }
                else if (isMeeting)
                {
                    // 会議開始時
                    if (isDead)
                    {
                        mute = true;  // 死亡者はマイクのみミュート
                        deafen = false;
                    }
                    else
                    {
                        mute = false; // 生存者は完全解除
                        deafen = false;
                    }
                }
                else if (isMeetingEnd)
                {
                    // 会議終了時 (タスク中)
                    if (isDead)
                    {
                        mute = false; // 死亡者は完全解除
                        deafen = false;
                    }
                    else
                    {
                        mute = true;  // 生存者は完全ミュート
                        deafen = true;
                    }
                }
                else
                {
                    // ゲーム開始時
                    mute = true;
                    deafen = true;
                }

                requests.Add(new DiscordMultiBotService.MuteRequest
                {
                    DiscordId = discordId,
                    Mute = mute,
                    Deafen = deafen
                });
            }
        }

        await DiscordMultiBotService.Instance.ApplyMuteStatesAsync(requests);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    [HarmonyPostfix]
    public static void ShipStatus_Start()
    {
        _ = Task.Run(() => TriggerMuteUpdate(isMeeting: false));
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    [HarmonyPostfix]
    public static void MeetingHud_Start()
    {
        _ = Task.Run(() => TriggerMuteUpdate(isMeeting: true));
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    [HarmonyPostfix]
    public static void MeetingHud_Close()
    {
        _ = Task.Run(() => TriggerMuteUpdate(isMeeting: false, isMeetingEnd: true));
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    [HarmonyPostfix]
    public static void AmongUsClient_OnGameEnd()
    {
        _ = Task.Run(() => TriggerMuteUpdate(isMeeting: false, isMeetingEnd: false, isGameEnd: true));
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    [HarmonyPostfix]
    public static void AmongUsClient_OnGameJoined()
    {
        if (IsHost)
        {
            _ = Task.Run(() => DiscordMultiBotService.Instance.UpdateLobbyEmbedAsync());
        }
    }

    // Embedの更新 (プレイヤーの出入り)
    [HarmonyPatch(typeof(GameData), nameof(GameData.AddPlayer))]
    [HarmonyPostfix]
    public static void GameData_AddPlayer()
    {
        if (IsHost)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(100); // すぐにリストに反映されない可能性があるため少し待機
                await DiscordMultiBotService.Instance.UpdateLobbyEmbedAsync();
            });
        }
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.RemovePlayer))]
    [HarmonyPostfix]
    public static void GameData_RemovePlayer()
    {
        if (IsHost)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                await DiscordMultiBotService.Instance.UpdateLobbyEmbedAsync();
            });
        }
    }
}
