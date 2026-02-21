namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ConsolePatch
{
    private static Minigame _alignTelescopeMinigame;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.Use))]
    internal static bool UsePrefix(Console __instance)
    {
        if (Helpers.IsBlocked(__instance, PlayerControl.LocalPlayer)) return false;

        if (!CustomOptionHolder.AirshipReplaceSafeTask.GetBool()) return true;
        PlayerTask playerTask = __instance.FindTask(PlayerControl.LocalPlayer);
        if (playerTask == null || playerTask.MinigamePrefab.name != "SafeGame") return true;
        if (_alignTelescopeMinigame == null)
        {
            foreach (var task in MapData.PolusShip.ShortTasks)
            {
                if (task.name != "AlignTelescope") continue;
                _alignTelescopeMinigame = task.MinigamePrefab;
                break;
            }
        }

        playerTask.MinigamePrefab = _alignTelescopeMinigame;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    internal static bool CanUsePrefix(ref float __result, Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        canUse = couldUse = false;
        __result = float.MaxValue;

        if (Helpers.IsBlocked(__instance, pc.Object)) return false;
        if (__instance.AllowImpostor) return true;
        return !pc.Object.HasFakeTasks();
    }
}