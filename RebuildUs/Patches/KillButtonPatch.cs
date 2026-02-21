namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class KillButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal static bool DoClickPrefix(KillButton __instance)
    {
        return Usables.KillButtonDoClick(__instance);
    }
}