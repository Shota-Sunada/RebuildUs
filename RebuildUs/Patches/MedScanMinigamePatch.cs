namespace RebuildUs.Patches;

public static class MedScanMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (ModMapOptions.AllowParallelMedBayScans)
        {
            __instance.medscan.CurrentUser = CachedPlayer.LocalPlayer.PlayerControl.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}