namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class CreateGameOptionsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
    internal static void StartPostfix(CreateGameOptions __instance)
    {
        // Disable HideNSeek
        __instance.SelectMode(0);
        __instance.modeButtons[1].gameObject.SetActive(false);
    }
}