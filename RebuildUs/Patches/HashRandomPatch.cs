namespace RebuildUs.Patches;

[HarmonyPatch]
public static class HashRandomPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HashRandom), nameof(HashRandom.FastNext))]
    public static bool FastNextPrefix(ref int __result, [HarmonyArgument(0)] int maxInt)
    {
        __result = RebuildUs.Instance.Rnd.Next(maxInt);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HashRandom), nameof(HashRandom.Next), [typeof(int)])]
    public static bool NextPrefix(ref int __result, [HarmonyArgument(0)] int maxInt)
    {
        __result = RebuildUs.Instance.Rnd.Next(maxInt);
        return false;
    }
}