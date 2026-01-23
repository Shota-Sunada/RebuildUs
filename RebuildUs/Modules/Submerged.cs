using System.Reflection;

namespace RebuildUs.Modules;

[HarmonyPatch]
public class Submerged
{
    public static Type SubmarineElevatorType;
    public static Type FloorHandlerType;
    public static Type SubmarineSpawnInSystemType;
    public static Type SubmarinePlayerFloorSystemType;
    public static Type SpawnInStateType;
    public static MethodInfo GetFloorHandlerMethod;
    public static MethodInfo RpcRequestChangeFloorMethod;

    private static FieldInfo _currentStateField;
    private static FieldInfo _timerField;
    private static FieldInfo _playersField;
    private static PropertyInfo _isDirtyProp;
    private static PropertyInfo _playerFloorSystemInstanceProp;
    private static MethodInfo _changePlayerFloorStateMethod;
    private static MethodInfo _getTotalPlayerAmountMethod;
    private static MethodInfo _getReadyPlayerAmountMethod;

    public static void Patch()
    {
        var loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SubmergedCompatibility.SUBMERGED_GUID, out PluginInfo pluginInfo);
        if (!loaded) return;
        var plugin = pluginInfo!.Instance as BasePlugin;
        var version = pluginInfo.Metadata.Version;
        var assembly = plugin!.GetType().Assembly;
        var types = AccessTools.GetTypesFromAssembly(assembly);

        foreach (var t in types)
        {
            if (t.Name == "SubmarineElevator") SubmarineElevatorType = t;
            else if (t.Name == "SubmarinePlayerFloorSystem") SubmarinePlayerFloorSystemType = t;
            else if (t.Name == "SpawnInState") SpawnInStateType = t;
            else if (t.Name == "FloorHandler") FloorHandlerType = t;
            else if (t.Name == "SubmarineSpawnInSystem") SubmarineSpawnInSystemType = t;
        }

        GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", [typeof(PlayerControl)]);
        RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

        _currentStateField = SubmarineSpawnInSystemType.GetField("CurrentState", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        _timerField = SubmarineSpawnInSystemType.GetField("Timer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        _playersField = SubmarineSpawnInSystemType.GetField("Players", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        _isDirtyProp = SubmarineSpawnInSystemType.GetProperty("IsDirty", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        _playerFloorSystemInstanceProp = SubmarinePlayerFloorSystemType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
        _changePlayerFloorStateMethod = SubmarinePlayerFloorSystemType.GetMethod("ChangePlayerFloorState");
        _getTotalPlayerAmountMethod = SubmarineSpawnInSystemType.GetMethod("GetTotalPlayerAmount");
        _getReadyPlayerAmountMethod = SubmarineSpawnInSystemType.GetMethod("GetReadyPlayerAmount");

        // OnDestroyパッチ
        Type submarineSelectSpawnType = null;
        foreach (var t in types) if (t.Name == "SubmarineSelectSpawn") { submarineSelectSpawnType = t; break; }
        var submarineSelectSpawnOnDestroyOriginal = AccessTools.Method(submarineSelectSpawnType, "OnDestroy");
        var submarineSelectSpawnOnDestroyPostfix = SymbolExtensions.GetMethodInfo(() => SubmarineSelectSpawnOnDestroyPatch.Postfix());
        var submarineSelectSpawnOnDestroyPrefix = SymbolExtensions.GetMethodInfo(() => SubmarineSelectSpawnOnDestroyPatch.Prefix());

        // GetTotalPlayerAmountパッチ
        var aInt = 0;
        var getTotalPlayerAmountOriginal = AccessTools.Method(SubmarineSpawnInSystemType, "GetTotalPlayerAmount");
        var getTotalPlayerAmountPostfix = SymbolExtensions.GetMethodInfo(() => SubmarineSpawnInSystemGetTotalPlayerAmountPatch.Postfix());
        var getTotalPlayerAmountPrefix = SymbolExtensions.GetMethodInfo(() => SubmarineSpawnInSystemGetTotalPlayerAmountPatch.Prefix(ref aInt));
        // Detoriorateパッチ
        object aObject = null;
        float aFloat = 0f;
        var detoriorateOriginal = AccessTools.Method(SubmarineSpawnInSystemType, "Detoriorate");
        var detorioratePostfix = SymbolExtensions.GetMethodInfo(() => SubmarineSpawnInSystemDetorioratePatch.Postfix());
        var detorioratePrefix = SymbolExtensions.GetMethodInfo(() => SubmarineSpawnInSystemDetorioratePatch.Prefix(aObject, aFloat));

        // パッチ適応
        var harmony = new Harmony("Submerged");
        harmony.Patch(submarineSelectSpawnOnDestroyOriginal, new HarmonyMethod(submarineSelectSpawnOnDestroyPrefix), new HarmonyMethod(submarineSelectSpawnOnDestroyPostfix));
        harmony.Patch(getTotalPlayerAmountOriginal, new HarmonyMethod(getTotalPlayerAmountPrefix), new HarmonyMethod(getTotalPlayerAmountPostfix));
        harmony.Patch(detoriorateOriginal, new HarmonyMethod(detorioratePrefix), new HarmonyMethod(detorioratePostfix));
    }

    public static void ChangePlayerFloorState(byte playerId, bool toUpper)
    {
        if (_playerFloorSystemInstanceProp == null || _changePlayerFloorStateMethod == null) return;
        var instance = _playerFloorSystemInstanceProp.GetValue(null);
        _changePlayerFloorStateMethod.Invoke(instance, [playerId, toUpper]);
    }

    public class SubmarineSelectSpawnOnDestroyPatch
    {
        public static void Prefix() { }
        public static void Postfix()
        {
            PlayerControl.LocalPlayer.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown));
            MapUtilities.CachedShipStatus.EmergencyCooldown = Helpers.GetOption(Int32OptionNames.EmergencyCooldown);
            Exile.ReEnableGameplay();
        }
    }
    public class SubmarineSpawnInSystemGetTotalPlayerAmountPatch
    {
        public static bool Prefix(ref int __result)
        {
            int count = 0;
            var players = GameData.Instance.AllPlayers;
            for (int i = 0; i < players.Count; i++)
            {
                var p = players[i];
                if (p != null && !p.IsDead && !p.Disconnected)
                {
                    PlayerControl @object = p.Object;
                    if (@object != null && !@object.isDummy)
                    {
                        count++;
                    }
                }
            }
            __result = count;
            return false;
        }
        public static void Postfix() { }
    }
    public class SubmarineSpawnInSystemDetorioratePatch
    {
        public static void Postfix() { }
        public static bool Prefix(object __instance, float deltaTime)
        {
            if (_getTotalPlayerAmountMethod == null || _getReadyPlayerAmountMethod == null || _currentStateField == null || _timerField == null) return true;

            var totalPlayerAmount = (int)_getTotalPlayerAmountMethod.Invoke(__instance, null);
            var readyPlayerAmount = (int)_getReadyPlayerAmountMethod.Invoke(__instance, null);

            object currentStateValue = _currentStateField.GetValue(__instance);
            Type enumUnderlyingType = Enum.GetUnderlyingType(SpawnInStateType);
            byte state = (byte)Convert.ChangeType(currentStateValue, enumUnderlyingType);

            if (state == 1)
            {
                var timerVal = (float)_timerField.GetValue(__instance);
                _timerField.SetValue(__instance, MathF.Max(0f, timerVal - deltaTime));
            }

            if (totalPlayerAmount == readyPlayerAmount)
            {
                if (_playersField != null && _isDirtyProp != null)
                {
                    _currentStateField.SetValue(__instance, state + 1);
                    _playersField.SetValue(__instance, new HashSet<byte>());
                    _timerField.SetValue(__instance, 10f);
                    _isDirtyProp.SetValue(__instance, true);
                }
            }

            return false;
        }
    }
}