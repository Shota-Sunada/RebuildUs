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
}