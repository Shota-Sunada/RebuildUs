namespace RebuildUs.Utilities;

internal static class MapUtilities
{
    private static readonly Dictionary<SystemTypes, UnityObject> PrivateSystems = [];
    internal static ShipStatus CachedShipStatus { get; private set; } = ShipStatus.Instance;

    internal static Dictionary<SystemTypes, UnityObject> Systems
    {
        get
        {
            if (PrivateSystems.Count == 0) GetSystems();
            return PrivateSystems;
        }
    }

    private static void MapDestroyed()
    {
        CachedShipStatus = ShipStatus.Instance;
        PrivateSystems.Clear();
    }

    private static void GetSystems()
    {
        if (!CachedShipStatus) return;

        Il2CppSystem.Collections.Generic.Dictionary<SystemTypes, ISystemType> systems = CachedShipStatus.Systems;
        if (systems.Count <= 0) return;

        foreach (SystemTypes systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) continue;
            PrivateSystems[systemTypes] = systems[systemTypes].TryCast<UnityObject>();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPriority(Priority.Last)]
    internal static void AwakePostfix(ShipStatus __instance)
    {
        CachedShipStatus = __instance;
        SubmergedCompatibility.SetupMap(__instance);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    internal static void OnDestroyPostfix()
    {
        CachedShipStatus = null;
        MapDestroyed();
        SubmergedCompatibility.SetupMap(null);
    }
}