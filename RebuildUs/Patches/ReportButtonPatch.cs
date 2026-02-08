namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ReportButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    public static bool DoClickPrefix(ReportButton __instance)
    {
        if (MapSettings.GameMode is not CustomGameMode.Roles) return false;

        return true;
    }
}