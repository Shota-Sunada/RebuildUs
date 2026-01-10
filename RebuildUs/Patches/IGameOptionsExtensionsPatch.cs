namespace RebuildUs.Patches;

[HarmonyPatch]
public static class IGameOptionsExtensionsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
    public static void ToHudStringPostfix(ref string __result)
    {
        CustomOption.ToHudString(ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.AppendItem), [typeof(Il2CppSystem.Text.StringBuilder), typeof(StringNames), typeof(string)])]
    public static void AppendItemPrefix(ref StringNames stringName, ref string value)
    {
        CustomOption.AppendItem(ref stringName, ref value);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    public static void GetAdjustedNumImpostorsPostfix(ref int __result)
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == AmongUs.GameOptions.GameModes.Normal)
        {
            // Ignore Vanilla impostor limits in TOR Games.
            __result = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.NumImpostors, 1, 3);
        }
    }
}