using HarmonyLib;
using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ShipPatcher
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
        => Ship.CalculateLightRadius(ref __result, __instance, player);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void IsGameOverDueToDeathPostfix(ref bool __result)
        => Ship.IsGameOverDueToDeath(ref __result);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static bool BeginPrefix(ShipStatus __instance)
        => Ship.BeginPrefix(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static void BeginPostfix(ShipStatus __instance)
        => Ship.BeginPostfix(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static void StartPostfix()
        => Ship.StartPostfix();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    public static void SpawnPlayerPostfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
        => Ship.SpawnPlayer(__instance, player, numPlayers, initialSpawn);
}
