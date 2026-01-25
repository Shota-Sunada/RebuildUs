using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class IntroCutscenePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    public static void OnDestroyPrefix(IntroCutscene __instance)
    {
        Intro.GenerateMiniCrewIcons(__instance);
        LastImpostor.OnIntroDestroy(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    public static bool CoBeginPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = Intro.CoBegin(__instance).WrapToIl2Cpp();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    public static void BeginCrewmatePrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeamIcons(__instance, ref teamToDisplay);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    public static void BeginCrewmatePostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeam(__instance, ref teamToDisplay);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    public static void BeginImpostorPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        Intro.SetupIntroTeamIcons(__instance, ref yourTeam);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    public static void BeginImpostorPostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        Intro.SetupIntroTeam(__instance, ref yourTeam);
    }
}