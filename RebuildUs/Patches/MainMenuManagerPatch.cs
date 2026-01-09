using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MainMenuManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void StartPostfix()
    {
        ModStamp.Show();
        Credits.EditMainMenu();
    }
}