namespace RebuildUs.Utilities;

internal static class MapUtilities
{
    private static readonly Dictionary<SystemTypes, UnityObject> _systems = [];
    private static ShipStatus _cachedShipStatus;
    internal static ShipStatus CachedShipStatus
    {
        get => _cachedShipStatus ?? ShipStatus.Instance;
        private set => _cachedShipStatus = value;
    }

    internal static Dictionary<SystemTypes, UnityObject> Systems
    {
        get
        {
            if (_systems.Count == 0) GetSystems();
            return _systems;
        }
    }

    private static void MapDestroyed()
    {
        CachedShipStatus = ShipStatus.Instance;
        _systems.Clear();
    }

    private static void GetSystems()
    {
        Logger.LogInfo("GetSystems");

        if (!CachedShipStatus)
        {
            Logger.LogInfo("NO CachedShipStatus");
            CachedShipStatus = ShipStatus.Instance;
        }

        var systems = CachedShipStatus.Systems;
        if (systems.Count <= 0)
        {
            Logger.LogInfo("NO CachedShipStatus.Systems");
            return;
        }

        foreach (var systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) continue;
            _systems[systemTypes] = systems[systemTypes].CastFast<UnityObject>();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
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