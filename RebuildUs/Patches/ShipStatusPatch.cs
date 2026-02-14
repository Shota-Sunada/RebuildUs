namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ShipStatusPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    public static void AwakePostfix(ShipStatus __instance)
    {
        Airship.Awake();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
    public static void OnEnablePostfix()
    {
        DiscordModManager.OnGameStart();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        return Ship.CalculateLightRadius(ref __result, __instance, player);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static bool BeginPrefix(ShipStatus __instance)
    {
        return Ship.BeginPrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static void BeginPostfix(ShipStatus __instance)
    {
        Ship.BeginPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static void StartPostfix()
    {
        Ship.StartPostfix();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    public static void SpawnPlayerPostfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        Ship.SpawnPlayer(__instance, player, numPlayers, initialSpawn);
    }
}