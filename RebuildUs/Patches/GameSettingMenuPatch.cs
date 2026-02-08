namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameSettingMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static bool ChangeTabPrefix(GameSettingMenu __instance, int tabNum, bool previewOnly)
    {
        return CustomOption.ChangeTabPrefix(__instance, (OptionPage)tabNum, previewOnly);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    public static bool OnEnablePrefix(GameSettingMenu __instance)
    {
        CustomOption.OnEnablePrefix(__instance);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static void StartPostfix(GameSettingMenu __instance)
    {
        CustomOption.SettingMenuStart(__instance);
    }
}
