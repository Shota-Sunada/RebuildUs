namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class HashRandomPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HashRandom), nameof(HashRandom.FastNext))]
    internal static bool FastNextPrefix(ref int __result, [HarmonyArgument(0)] int maxInt)
    {
        __result = RebuildUs.Rnd.Next(maxInt);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HashRandom), nameof(HashRandom.Next), typeof(int))]
    internal static bool NextPrefix(ref int __result, [HarmonyArgument(0)] int maxInt)
    {
        __result = RebuildUs.Rnd.Next(maxInt);
        return false;
    }
}