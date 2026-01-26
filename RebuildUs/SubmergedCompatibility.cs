using System.Reflection;
using Submerged.Enums;
using Submerged.Extensions;
using Submerged.Floors;
using Submerged.Map;
using Submerged.Systems.Oxygen;
using Submerged.Vents;

namespace RebuildUs;

public static class SubmergedCompatibility
{
    public static class Classes
    {
        public const string ElevatorMover = "ElevatorMover";
    }

    public const string SUBMERGED_GUID = "Submerged";
    public const string SUBMERGED_VERSION = "2026.01.26";
    public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version Version { get; private set; }
    public static bool Loaded { get; private set; }
    public static bool LoadedExternally { get; private set; }
    public static BasePlugin Plugin { get; private set; }
    public static Assembly Assembly { get; private set; }

    public static SubmarineStatus SubmarineStatus { get; private set; }

    public static bool IsSubmerged { get; private set; }

    public static void SetupMap(ShipStatus map)
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

    public static TaskTypes RetrieveOxygenMask;

    public static void Initialize()
    {
        Loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out PluginInfo plugin);
        if (!Loaded) return;

        LoadedExternally = true;
        Plugin = plugin!.Instance as BasePlugin;
        Version = plugin.Metadata.Version.BaseVersion();
        Assembly = Plugin.GetType().Assembly;

        RetrieveOxygenMask = CustomTaskTypes.RetrieveOxygenMask.taskType;
    }

    public static MonoBehaviour AddSubmergedComponent(this GameObject obj, string typeName)
    {
        if (!Loaded) return obj.AddComponent<MissingSubmergedBehaviour>();
        bool validType = ComponentExtensions.RegisteredTypes.TryGetValue(typeName, out Type type);
        return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingSubmergedBehaviour>();
    }

    public static float GetSubmergedNeutralLightRadius(bool isImpostor)
    {
        return !Loaded || SubmarineStatus == null ? 0 : SubmarineStatus.CalculateLightRadius(null, true, isImpostor);
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!Loaded) return;
        FloorHandler.GetFloorHandler(PlayerControl.LocalPlayer).RpcRequestChangeFloor(toUpper);
    }

    public static bool GetInTransition()
    {
        return Loaded && VentPatchData.InTransition;
    }

    public static void RepairOxygen()
    {
        if (!Loaded) return;
        try
        {
            ShipStatus.Instance.RpcUpdateSystem((SystemTypes)130, 64);
            SubmarineOxygenSystem.Instance.RepairDamage(PlayerControl.LocalPlayer, 64);
        }
        catch (NullReferenceException)
        {
            Logger.LogMessage("null reference in engineer oxygen fix");
        }
    }
}

public class MissingSubmergedBehaviour : MonoBehaviour
{
    static MissingSubmergedBehaviour() => ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
    public MissingSubmergedBehaviour(IntPtr ptr) : base(ptr) { }
}