using AmongUs.Data.Legacy;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class LegacySaveManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
    public static void LoadPlayerPrefsPrefix([HarmonyArgument(0)] bool overrideLoad)
    {
        CustomColors.LoadPlayerPrefsPrefix(overrideLoad);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
    public static void LoadPlayerPrefsPostfix()
    {
        CustomColors.LoadPlayerPrefsPostfix();
    }
}