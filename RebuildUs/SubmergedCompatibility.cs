using System.Reflection;
using Submerged.Enums;
using Submerged.Extensions;
using Submerged.Floors;
using Submerged.Map;
using Submerged.Systems.Oxygen;
using Submerged.Vents;
using Version = SemanticVersioning.Version;

namespace RebuildUs;

internal static class SubmergedCompatibility
{
    internal const string SUBMERGED_GUID = "Submerged";
    internal const string SUBMERGED_VERSION = "2026.1.26";
    internal const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

    internal static TaskTypes RetrieveOxygenMask;

    internal static Version Version { get; private set; }
    internal static bool Loaded { get; private set; }
    internal static bool LoadedExternally { get; private set; }
    internal static BasePlugin Plugin { get; private set; }
    internal static Assembly Assembly { get; private set; }

    internal static SubmarineStatus SubmarineStatus { get; private set; }

    internal static bool IsSubmerged { get; private set; }

    internal static void SetupMap(ShipStatus map)
    {
        if (map == null)
        {
            IsSubmerged = false;
            SubmarineStatus = null;
            return;
        }

        IsSubmerged = map.Type == SUBMERGED_MAP_TYPE;
        if (!IsSubmerged) return;

        SubmarineStatus = map.GetComponent<SubmarineStatus>();
    }

    internal static void Initialize()
    {
        Loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out PluginInfo plugin);
        if (!Loaded) return;

        LoadedExternally = true;
        Plugin = plugin!.Instance as BasePlugin;
        Version = plugin.Metadata.Version.BaseVersion();
        if (Plugin != null) Assembly = Plugin.GetType().Assembly;

        RetrieveOxygenMask = CustomTaskTypes.RetrieveOxygenMask.taskType;
    }

    internal static MonoBehaviour AddSubmergedComponent(this GameObject obj, string typeName)
    {
        if (!Loaded) return obj.AddComponent<MissingSubmergedBehaviour>();
        bool validType = ComponentExtensions.RegisteredTypes.TryGetValue(typeName, out Type type);
        return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingSubmergedBehaviour>();
    }

    internal static float GetSubmergedNeutralLightRadius(bool isImpostor)
    {
        return !Loaded || SubmarineStatus == null ? 0 : SubmarineStatus.CalculateLightRadius(null, true, isImpostor);
    }

    internal static void ChangeFloor(bool toUpper)
    {
        if (!Loaded) return;
        FloorHandler.GetFloorHandler(PlayerControl.LocalPlayer).RpcRequestChangeFloor(toUpper);
    }

    internal static bool GetInTransition()
    {
        return Loaded && VentPatchData.InTransition;
    }

    internal static void RepairOxygen()
    {
        if (!Loaded) return;
        try
        {
            MapUtilities.CachedShipStatus.RpcUpdateSystem((SystemTypes)130, 64);
            SubmarineOxygenSystem.Instance.RepairDamage(PlayerControl.LocalPlayer, 64);
        }
        catch (NullReferenceException)
        {
            Logger.LogMessage("null reference in engineer oxygen fix");
        }
    }

    internal static class Classes
    {
        internal const string ELEVATOR_MOVER = "ElevatorMover";
    }
}

internal sealed class MissingSubmergedBehaviour : MonoBehaviour
{
    static MissingSubmergedBehaviour()
    {
        ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
    }

    internal MissingSubmergedBehaviour(IntPtr ptr) : base(ptr) { }
}