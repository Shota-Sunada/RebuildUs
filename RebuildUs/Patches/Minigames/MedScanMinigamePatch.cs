namespace RebuildUs.Patches;

internal static class MedScanMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    internal static void Prefix(MedScanMinigame __instance)
    {
        if (!MapSettings.AllowParallelMedBayScans)
        {
            return;
        }
        __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
        __instance.medscan.UsersList.Clear();
    }
}