using AmongUs.Data.Player;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlayerBanDataPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    internal static void IsBannedPostfix(out bool __result)
    {
        __result = false;
    }
}