namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class LobbyViewSettingsPanePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
    internal static bool LobbyViewSettingsPaneSetTabPrefix(LobbyViewSettingsPane __instance)
    {
        CustomOption.SetTab(__instance, (PanePage)__instance.currentTab);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
    internal static bool LobbyViewSettingsPaneChangeTabPrefix(LobbyViewSettingsPane __instance, StringNames category)
    {
        CustomOption.SettingsPaneChangeTab(__instance, (PanePage)category);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
    internal static void UpdatePostfix(LobbyViewSettingsPane __instance)
    {
        // CustomOption.SettingsPaneUpdate(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    internal static void SettingsPaneAwake(LobbyViewSettingsPane __instance)
    {
        CustomOption.SettingsPaneAwake(__instance);
    }
}