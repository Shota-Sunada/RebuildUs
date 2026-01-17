namespace RebuildUs.Patches;

[HarmonyPatch]
public static class HeliSabotageSystemPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.Deteriorate))]
    public static void DeterioratePrefix(HeliSabotageSystem __instance)
    {
        if (!__instance.IsActive || !CustomOptionHolder.AirshipOptions.GetBool())
        {
            return;
        }

        if (ShipStatus.Instance != null)
        {
            if (__instance.Countdown >= CustomOptionHolder.AirshipReactorDuration.GetFloat())
            {
                __instance.Countdown = CustomOptionHolder.AirshipReactorDuration.GetFloat();
            }
        }
    }
}