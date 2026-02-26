namespace RebuildUs.Modules.Cosmetics.Patches;

[HarmonyPatch(typeof(HatManager))]
internal static class HatManagerPatches
{
    private static bool _isRunning;
    private static bool _isLoaded;
    private static List<HatData> _allHats;

    [HarmonyPatch(nameof(HatManager.GetHatById))]
    [HarmonyPrefix]
    private static void GetHatByIdPrefix(HatManager __instance)
    {
        if (_isRunning || _isLoaded) return;
        _isRunning = true;
        // Maybe we can use lock keyword to ensure simultaneous list manipulations ?
        _allHats = [.. __instance.allHats];
        List<CustomHat> cache = [.. CustomHatManager.UnregisteredHats];
        foreach (CustomHat hat in cache)
        {
            try
            {
                _allHats.Add(CustomHatManager.CreateHatBehaviour(hat));
                CustomHatManager.UnregisteredHats.Remove(hat);
            }
            catch
            {
                // This means the file has not been downloaded yet, do nothing...
            }
        }

        if (CustomHatManager.UnregisteredHats.Count == 0)
            _isLoaded = true;
        cache.Clear();

        __instance.allHats = new([.. _allHats]);
    }

    [HarmonyPatch(nameof(HatManager.GetHatById))]
    [HarmonyPostfix]
    private static void GetHatByIdPostfix()
    {
        _isRunning = false;
    }
}