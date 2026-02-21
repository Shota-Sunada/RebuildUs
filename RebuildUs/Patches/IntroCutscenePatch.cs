using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppSystem.Collections;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class IntroCutscenePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    internal static void OnDestroyPrefix(IntroCutscene __instance)
    {
        Intro.GenerateMiniCrewIcons(__instance);
        LastImpostor.OnIntroDestroy(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    internal static bool CoBeginPrefix(IntroCutscene __instance, ref IEnumerator __result)
    {
        __result = Intro.CoBegin(__instance).WrapToIl2Cpp();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    internal static void BeginCrewmatePrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeamIcons(__instance, ref teamToDisplay);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    internal static void BeginCrewmatePostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeam(__instance, ref teamToDisplay);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    internal static void BeginImpostorPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        Intro.SetupIntroTeamIcons(__instance, ref yourTeam);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    internal static void BeginImpostorPostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        Intro.SetupIntroTeam(__instance, ref yourTeam);
    }
}