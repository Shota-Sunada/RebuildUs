namespace RebuildUs.Utilities;

public static class MapUtilities
{
    public static ShipStatus CachedShipStatus { get; private set; } = ShipStatus.Instance;

    public static void MapDestroyed()
    {
        CachedShipStatus = ShipStatus.Instance;
        _systems.Clear();
    }

    private static readonly Dictionary<SystemTypes, UnityEngine.Object> _systems = [];
    public static Dictionary<SystemTypes, UnityEngine.Object> Systems
    {
        get
        {
            if (_systems.Count == 0) GetSystems();
            return _systems;
        }
    }

    private static void GetSystems()
    {
        if (!CachedShipStatus) return;

        var systems = CachedShipStatus.Systems;
        if (systems.Count <= 0) return;

        foreach (var systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) continue;
            _systems[systemTypes] = systems[systemTypes].TryCast<UnityEngine.Object>();
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void AwakePostfix(ShipStatus __instance)
    {
        CachedShipStatus = __instance;
        SubmergedCompatibility.SetupMap(__instance);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void OnDestroyPostfix()
    {
        CachedShipStatus = null;
        MapDestroyed();
        SubmergedCompatibility.SetupMap(null);
    }
}