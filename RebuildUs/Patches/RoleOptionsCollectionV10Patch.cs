namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class RoleOptionsCollectionV10Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoleOptionsCollectionV10), nameof(RoleOptionsCollectionV10.GetNumPerGame))]
    internal static void GetNumPerGamePostfix(ref int __result)
    {
        if (Helpers.IsNormal)
        {
            __result = 0; // Deactivate Vanilla Roles if the mod roles are active
        }
    }
}