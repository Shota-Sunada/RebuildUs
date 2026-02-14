namespace RebuildUs.Modules.Cosmetics.Patches;

[HarmonyPatch(typeof(HatManager))]
internal static class HatManagerPatches
{
    private static bool IsRunning;
    private static bool IsLoaded;
    private static List<HatData> AllHats;

    [HarmonyPatch(nameof(HatManager.GetHatById))]
    [HarmonyPrefix]
    private static void GetHatByIdPrefix(HatManager __instance)
    {
        if (IsRunning || IsLoaded) return;
        IsRunning = true;
        // Maybe we can use lock keyword to ensure simultaneous list manipulations ?
        AllHats = [.. __instance.allHats];
        var cache = new List<CustomHat>(CustomHatManager.UnregisteredHats);
        foreach (var hat in cache)
        {
            try
            {
                AllHats.Add(CustomHatManager.CreateHatBehaviour(hat));
                CustomHatManager.UnregisteredHats.Remove(hat);
            }
            catch
            {
                // This means the file has not been downloaded yet, do nothing...
            }
        }
        if (CustomHatManager.UnregisteredHats.Count == 0)
            IsLoaded = true;
        cache.Clear();

        __instance.allHats = new Il2CppReferenceArray<HatData>([.. AllHats]);
    }

    [HarmonyPatch(nameof(HatManager.GetHatById))]
    [HarmonyPostfix]
    private static void GetHatByIdPostfix()
    {
        IsRunning = false;
    }
}