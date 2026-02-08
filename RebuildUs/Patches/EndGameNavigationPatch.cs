namespace RebuildUs.Patches;

[HarmonyPatch]
public static class EndGameNavigationPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowProgression))]
    public static void ShowProgressionPrefix()
    {
        EndGameMain.TextRenderer.gameObject.SetActive(false);
    }
}
