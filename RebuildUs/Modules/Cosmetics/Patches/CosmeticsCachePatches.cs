namespace RebuildUs.Modules.Cosmetics.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    [HarmonyPrefix]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        return !CustomHatManager.VIEW_DATA_CACHE_BY_NAME.TryGetValue(id, out __result);
    }
}
