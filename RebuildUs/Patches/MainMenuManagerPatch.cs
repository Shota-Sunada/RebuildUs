namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MainMenuManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    internal static void StartPostfix(MainMenuManager __instance)
    {
        ModStamp.Show();
        Credits.EditMainMenu(__instance);
        ClientOptions.Start(__instance);
    }
}