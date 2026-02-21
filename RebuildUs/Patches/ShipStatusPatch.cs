namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ShipStatusPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    internal static void AwakePostfix(ShipStatus __instance)
    {
        Airship.Awake();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
    internal static void OnEnablePostfix() { }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    internal static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        return Ship.CalculateLightRadius(ref __result, __instance, player);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    internal static bool BeginPrefix(ShipStatus __instance)
    {
        return Ship.BeginPrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    internal static void BeginPostfix(ShipStatus __instance)
    {
        Ship.BeginPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    internal static void StartPostfix()
    {
        Ship.StartPostfix();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    internal static void SpawnPlayerPostfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        Ship.SpawnPlayer(__instance, player, numPlayers, initialSpawn);
    }
}