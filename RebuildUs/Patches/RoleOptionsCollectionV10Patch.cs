using AmongUs.GameOptions;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class RoleOptionsCollectionV10Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoleOptionsCollectionV10), nameof(RoleOptionsCollectionV10.GetNumPerGame))]
    public static void GetNumPerGamePostfix(ref int __result)
    {
        if (GameOptions.IsNormalMode) __result = 0; // Deactivate Vanilla Roles if the mod roles are active
    }
}