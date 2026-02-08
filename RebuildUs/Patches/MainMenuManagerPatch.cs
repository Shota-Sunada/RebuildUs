namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MainMenuManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void StartPostfix(MainMenuManager __instance)
    {
        ModStamp.Show();
        Credits.EditMainMenu(__instance);
        ClientOptions.Start(__instance);
    }
}
