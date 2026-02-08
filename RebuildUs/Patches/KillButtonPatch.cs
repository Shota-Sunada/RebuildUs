namespace RebuildUs.Patches;

[HarmonyPatch]
public static class KillButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static bool DoClickPrefix(KillButton __instance)
    {
        return Usables.KillButtonDoClick(__instance);
    }
}
