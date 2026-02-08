using AmongUs.Data.Player;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerBanDataPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static void IsBannedPostfix(out bool __result)
    {
        __result = false;
    }
}
