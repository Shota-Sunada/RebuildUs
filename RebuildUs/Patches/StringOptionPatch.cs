namespace RebuildUs.Patches;

[HarmonyPatch]
public static class StringOptionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public static void InitializePrefix(StringOption __instance)
    {
        CustomOption.StringOptionInitializePrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public static void InitializePostfix(StringOption __instance)
    {
        CustomOption.StringOptionInitializePostfix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public static bool InitializePrefix2(StringOption __instance)
    {
        return CustomOption.StringOptionInitialize(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public static bool IncreasePrefix(StringOption __instance)
    {
        return CustomOption.StringOptionIncrease(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public static bool DecreasePrefix(StringOption __instance)
    {
        return CustomOption.StringOptionDecrease(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
    public static void FixedUpdatePostfix(StringOption __instance)
    {
        CustomOption.StringOptionFixedUpdate(__instance);
    }
}