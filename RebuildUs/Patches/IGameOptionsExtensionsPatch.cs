using StringBuilder = Il2CppSystem.Text.StringBuilder;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameOptionsExtensionsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(IGameOptionsExtensions),
        nameof(IGameOptionsExtensions.AppendItem),
        typeof(StringBuilder),
        typeof(StringNames),
        typeof(string))]
    internal static void AppendItemPrefix(ref StringNames stringName, ref string value)
    {
        CustomOption.AppendItem(ref stringName, ref value);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    internal static void GetAdjustedNumImpostorsPostfix(ref int __result)
    {
        if (Helpers.IsNormal)
        {
            // Ignore Vanilla impostor limits in RebuildUs Games.
            __result = Mathf.Clamp(Helpers.GetOption(Int32OptionNames.NumImpostors), 1, 3);
        }
    }
}