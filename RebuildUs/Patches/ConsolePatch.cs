namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.Use))]
    public static bool UsePrefix(Console __instance)
    {
        if (Usables.IsBlocked(__instance, CachedPlayer.LocalPlayer.PlayerControl))
        {
            return false;
        }

        if (CustomOptionHolder.AirshipReplaceSafeTask.GetBool())
        {
            var playerTask = __instance.FindTask(PlayerControl.LocalPlayer);
            var alignTelescopeMinigame = MapData.PolusShip.ShortTasks.FirstOrDefault(x => x.name == "AlignTelescope").MinigamePrefab;
            if (playerTask.MinigamePrefab.name == "SafeGame")
            {
                playerTask.MinigamePrefab = alignTelescopeMinigame;
            }
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static bool CanUsePrefix(ref float __result, Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        return Usables.CanUse(ref __result, __instance, pc, out canUse, out couldUse);
    }
}