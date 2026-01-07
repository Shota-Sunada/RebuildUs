namespace RebuildUs.Patches;

[HarmonyPatch]
public static class LobbyViewSettingsPanePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
    public static bool LobbyViewSettingsPaneSetTabPrefix(LobbyViewSettingsPane __instance)
    {
        return CustomOption.SettingsPaneSetTab(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
    public static void LobbyViewSettingsPaneChangeTabPostfix(LobbyViewSettingsPane __instance, StringNames category)
    {
        CustomOption.SettingsPaneChangeTab(__instance, category);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
    public static void UpdatePostfix(LobbyViewSettingsPane __instance)
    {
        CustomOption.SettingsPaneUpdate(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    public static void SettingsPaneAwake(LobbyViewSettingsPane __instance)
    {
        CustomOption.SettingsPaneAwake(__instance);
    }
}