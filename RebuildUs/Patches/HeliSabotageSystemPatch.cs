namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class HeliSabotageSystemPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.Deteriorate))]
    internal static void DeterioratePrefix(HeliSabotageSystem __instance)
    {
        if (!__instance.IsActive)
        {
            return;
        }

        if (MapUtilities.CachedShipStatus == null)
        {
            return;
        }
        var reactorDuration = CustomOptionHolder.AirshipReactorDuration.GetFloat();
        if (__instance.Countdown >= reactorDuration)
        {
            __instance.Countdown = reactorDuration;
        }
    }
}