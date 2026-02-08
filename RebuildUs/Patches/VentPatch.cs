namespace RebuildUs.Patches;

[HarmonyPatch]
public static class VentPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static bool CanUsePrefix(Vent __instance, ref float __result, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
    {
        return Usables.VentCanUse(__instance, ref __result, pc, out canUse, out couldUse);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    public static bool DoClickPrefix(VentButton __instance)
    {
        // Manually modifying the VentButton to use Vent.Use again in order to trigger the Vent.Use prefix patch
        __instance.currentTarget?.Use();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
    public static bool UsePrefix(Vent __instance)
    {
        return Usables.VentUse(__instance);
    }

    // disable vent animation
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    public static bool EnterVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    public static bool ExitVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }
}
