using Object = UnityEngine.Object;

namespace RebuildUs.Utilities;

public static class MapUtilities
{
    private static readonly Dictionary<SystemTypes, Object> SYSTEMS = [];
    internal static ShipStatus CachedShipStatus { get; private set; } = ShipStatus.Instance;

    internal static Dictionary<SystemTypes, Object> Systems
    {
        get
        {
            if (SYSTEMS.Count == 0) GetSystems();
            return SYSTEMS;
        }
    }

    private static void MapDestroyed()
    {
        CachedShipStatus = ShipStatus.Instance;
        SYSTEMS.Clear();
    }

    private static void GetSystems()
    {
        if (!CachedShipStatus) return;

        var systems = CachedShipStatus.Systems;
        if (systems.Count <= 0) return;

        foreach (var systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) continue;
            SYSTEMS[systemTypes] = systems[systemTypes].TryCast<Object>();
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void AwakePostfix(ShipStatus __instance)
    {
        CachedShipStatus = __instance;
        SubmergedCompatibility.SetupMap(__instance);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void OnDestroyPostfix()
    {
        CachedShipStatus = null;
        MapDestroyed();
        SubmergedCompatibility.SetupMap(null);
    }
}
