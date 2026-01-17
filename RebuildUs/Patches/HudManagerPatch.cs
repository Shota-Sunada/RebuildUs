namespace RebuildUs.Patches;

[HarmonyPatch]
public static class HudManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void UpdatePrefix(HudManager __instance)
    {
        CustomOption.HudSettingsManager.UpdateScrollerPosition(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void UpdatePostfix(HudManager __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

        CustomButton.HudUpdate();
        Update.resetNameTagsAndColors();
        Update.setNameColors();
        Update.setNameTags();

        Update.updateImpostorKillButton(__instance);
        Update.updateSabotageButton(__instance);
        Update.updateUseButton(__instance);
        Update.updateReportButton(__instance);
        Update.updateVentButton(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.CoShowIntro))]
    public static void CoShowIntroPrefix()
    {
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive))]
    public static void SetHudActivePostfix(HudManager __instance)
    {
        __instance.transform.FindChild("TaskDisplay").FindChild("TaskPanel").gameObject.SetActive(true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
    public static void Prefix(HudManager __instance)
    {
        Meeting.startMeeting();
    }
}