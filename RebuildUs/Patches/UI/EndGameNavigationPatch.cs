namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class EndGameNavigationPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowProgression))]
    internal static void ShowProgressionPrefix()
    {
        EndGameMain.TextRenderer.gameObject.SetActive(false);
    }
}