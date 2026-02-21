namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MapBehaviourPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    internal static bool FixedUpdatePrefix(MapBehaviour __instance)
    {
        if (!MeetingHud.Instance) return true; // Only run in meetings, and then set the Position of the HerePoint to the Position before the Meeting!
        Map.UpdatePrefix(__instance);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    internal static void FixedUpdatePostfix(MapBehaviour __instance)
    {
        Map.UpdatePostfix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    internal static bool ShowNormalMapPrefix(MapBehaviour __instance)
    {
        if (!MeetingHud.Instance || __instance.IsOpen) return true; // Only run in meetings and when the map is closed

        return Map.ShowNormalMap(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
    internal static void GenericShowPrefix(MapBehaviour __instance)
    {
        Map.GenericShowPrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
    internal static void GenericShowPostfix(MapBehaviour __instance)
    {
        Map.GenericShowPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
    internal static void ClosePostfix(MapBehaviour __instance)
    {
        Map.Close(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.IsOpenStopped), MethodType.Getter)]
    internal static bool IsOpenStoppedPrefix(ref bool __result, MapBehaviour __instance)
    {
        return Map.IsOpenStopped(ref __result, __instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal static bool ShowSabotageMapPrefix(MapBehaviour __instance)
    {
        return Map.ShowSabotageMapPrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal static void ShowSabotageMapPostfix(MapBehaviour __instance)
    {
        Map.ShowSabotageMapPostfix(__instance);
    }
}