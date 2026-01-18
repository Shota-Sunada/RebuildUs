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
    public static void Patch()
    {
        var loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SubmergedCompatibility.SUBMERGED_GUID, out PluginInfo pluginInfo);
        if (!loaded) return;
        var plugin = pluginInfo!.Instance as BasePlugin;
        var version = pluginInfo.Metadata.Version;
        var assembly = plugin!.GetType().Assembly;
        var types = AccessTools.GetTypesFromAssembly(assembly);
        SubmarineElevatorType = types.First(t => t.Name == "SubmarineElevator");
        SubmarinePlayerFloorSystemType = types.First(t => t.Name == "SubmarinePlayerFloorSystem");
        SpawnInStateType = types.First(t => t.Name == "SpawnInState");
        FloorHandlerType = types.First(t => t.Name == "FloorHandler");
        GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", [typeof(PlayerControl)]);
        RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

        // OnDestroyパッチ
        var submarineSelectSpawnType = types.First(t => t.Name == "SubmarineSelectSpawn");
        var submarineSelectSpawnOnDestroyOriginal = AccessTools.Method(submarineSelectSpawnType, "OnDestroy");
        var submarineSelectSpawnOnDestroyPostfix = SymbolExtensions.GetMethodInfo(() => SubmarineSelectSpawnOnDestroyPatch.Postfix());
        var submarineSelectSpawnOnDestroyPrefix = SymbolExtensions.GetMethodInfo(() => SubmarineSelectSpawnOnDestroyPatch.Prefix());

        // GetTotalPlayerAmountパッチ
        var aInt = 0;
        SubmarineSpawnInSystemType = types.First(t => t.Name == "SubmarineSpawnInSystem");
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
        var subMarinePlayerFloorSystemProperties = SubmarinePlayerFloorSystemType.GetProperties(BindingFlags.Static | BindingFlags.Public);
        var instance = subMarinePlayerFloorSystemProperties.First(f => f.Name == "Instance").GetValue(null);
        var changePlayerFloorStateMethod = SubmarinePlayerFloorSystemType.GetMethod("ChangePlayerFloorState");
        changePlayerFloorStateMethod.Invoke(instance, [playerId, toUpper]);
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
            __result = Enumerable.Count(GameData.Instance.AllPlayers.ToSystemList(), delegate (NetworkedPlayerInfo p)
            {
                if (p != null && !p.IsDead && !p.Disconnected)
                {
                    PlayerControl @object = p.Object;
                    if (@object != null)
                    {
                        return !@object.isDummy;
                    }
                }
                return false;
            });
            return false;
        }
        public static void Postfix() { }
    }
    public class SubmarineSpawnInSystemDetorioratePatch
    {
        public static void Postfix() { }
        public static bool Prefix(object __instance, float deltaTime)
        {
            var getTotalPlayerAmount = AccessTools.Method(SubmarineSpawnInSystemType, "GetTotalPlayerAmount");
            var totalPlayerAmount = (getTotalPlayerAmount.Invoke(__instance, []) as int?).Value;
            var getReadyPlayerAmount = AccessTools.Method(SubmarineSpawnInSystemType, "GetReadyPlayerAmount");
            var readyPlayerAmount = (getReadyPlayerAmount.Invoke(__instance, []) as int?).Value;
            var submarineSpawnInSystemFields = SubmarineSpawnInSystemType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var currentState = submarineSpawnInSystemFields.First(f => f.Name == "CurrentState");
            currentState = SubmarineSpawnInSystemType.GetField("CurrentState");
            object currentState2 = currentState.GetValue(__instance);
            Type enumUnderlyingType = System.Enum.GetUnderlyingType(SpawnInStateType);
            object state = System.Convert.ChangeType(currentState2, enumUnderlyingType);

            var timer = submarineSpawnInSystemFields.First(f => f.Name == "Timer");
            if ((byte)state == 1)
            {
                var timer2 = MathF.Max(0f, (timer.GetValue(__instance) as float?).Value - deltaTime);
                timer.SetValue(__instance, timer2);
            }

            if (totalPlayerAmount == readyPlayerAmount)
            {
                var players = submarineSpawnInSystemFields.First(f => f.Name == "Players");
                var submarineSpawnInSystemProperties = SubmarineSpawnInSystemType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var isDirty = submarineSpawnInSystemProperties.First(f => f.Name == "IsDirty");
                currentState.SetValueDirect(__makeref(__instance), (byte)state + 1);
                //CurrentState.SetValue(__instance, Done);
                players.SetValue(__instance, new HashSet<byte>());
                timer.SetValue(__instance, 10f);
                isDirty.SetValue(__instance, true);
            }

            return false;

        }
    }
}