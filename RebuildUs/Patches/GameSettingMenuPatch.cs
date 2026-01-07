namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameSettingMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static void GameSettingMenuChangeTabPostfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
    {
        CustomOption.SettingMenuChangeTab(__instance, tabNum, previewOnly);
    }

[HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static void GameSettingMenuStartPostfix(GameSettingMenu __instance)
    {
        CustomOption.SettingMenuStart(__instance);
    }
}