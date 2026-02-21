using AmongUs.Data.Legacy;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class LegacySaveManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
    internal static void LoadPlayerPrefsPrefix([HarmonyArgument(0)] bool overrideLoad)
    {
        CustomColors.LoadPlayerPrefsPrefix(overrideLoad);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
    internal static void LoadPlayerPrefsPostfix()
    {
        CustomColors.LoadPlayerPrefsPostfix();
    }
}