namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameSettingMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    internal static bool ChangeTabPrefix(GameSettingMenu __instance, int tabNum, bool previewOnly)
    {
        return CustomOption.ChangeTabPrefix(__instance, (OptionPage)tabNum, previewOnly);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    internal static bool OnEnablePrefix(GameSettingMenu __instance)
    {
        CustomOption.OnEnablePrefix(__instance);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    internal static void StartPostfix(GameSettingMenu __instance)
    {
        CustomOption.SettingMenuStart(__instance);
    }
}