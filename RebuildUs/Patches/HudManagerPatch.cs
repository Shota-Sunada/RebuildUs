using InnerNet;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class HudManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void UpdatePostfix(HudManager __instance)
    {
        if (AmongUsClient.Instance?.GameState != InnerNetClient.GameStates.Started) return;

        try
        {
            CustomButton.HudUpdate();
            Update.UpdatePlayerNamesAndColors();
            Update.CamouflageAndMorphActions();
            Update.UpdateImpostorKillButton(__instance);
            Update.UpdateSabotageButton(__instance);
            Update.UpdateUseButton(__instance);
            Update.UpdateReportButton(__instance);
            Update.UpdateVentButton(__instance);
            Update.TimerUpdate();

            CaptureTheFlag.CaptureTheFlagUpdate();
            PoliceAndThief.PoliceandthiefUpdate();
            HotPotato.HotPotatoUpdate();
            BattleRoyale.BattleRoyaleUpdate();

            Hacker.HackerTimer -= Time.deltaTime;
            Trickster.LightsOutTimer -= Time.deltaTime;
            Tracker.CorpsesTrackingTimer -= Time.deltaTime;
        }
        catch (Exception ex)
        {
            Logger.LogError($"[HudManagerPatch] UpdatePostfix error: {ex}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour),
                  typeof(bool))]
    public static void SetHudActivePostfix(HudManager __instance)
    {
        __instance.TaskPanel.gameObject.SetActive(true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
    public static void Prefix(HudManager __instance)
    {
        Meeting.StartMeetingClear();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static void StartPostfix(HudManager __instance)
    {
        RebuildUs.MakeButtons(__instance);
        RebuildUs.SetButtonCooldowns();
    }
}
