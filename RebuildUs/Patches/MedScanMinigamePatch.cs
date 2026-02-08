namespace RebuildUs.Patches;

public static class MedScanMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (MapSettings.AllowParallelMedBayScans)
        {
            __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}
