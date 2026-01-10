using BepInEx.Unity.IL2CPP.Utils.Collections;
using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class IntroCutscenePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    public static void OnDestroyPrefix(IntroCutscene __instance)
    {
        Intro.GenerateMiniCrewIcons(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    public static bool CoBeginPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (RoleAssignmentPatch.isAssigned)
        {
            return true;
        }
        __result = Intro.CoBegin(__instance).WrapToIl2Cpp();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
    public static bool ShowRolePrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        return Intro.ShowRole(__instance, ref __result);
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
    public static void BeginImpostorPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeamIcons(__instance, ref teamToDisplay);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    public static void BeginImpostorPostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        Intro.SetupIntroTeam(__instance, ref teamToDisplay);
    }
}