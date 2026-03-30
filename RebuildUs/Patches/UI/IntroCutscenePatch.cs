using BepInEx.Unity.IL2CPP.Utils.Collections;
using IEnumerator = Il2CppSystem.Collections.IEnumerator;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class IntroCutscenePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    internal static void OnDestroyPrefix(IntroCutscene __instance)
    {
        Intro.GenerateMiniCrewIcons(__instance);
        GameModeManager.OnIntroDestroyed();
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
    internal static bool BeginCrewmatePrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        if (GameModeManager.CurrentGameMode != CustomGamemode.Normal && GameModeManager.CurrentGameModeInstance.OnBeginIntro(__instance, ref teamToDisplay))
        {
            return false;
        }

        Intro.SetupIntroTeamIcons(__instance, ref teamToDisplay);
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    internal static void BeginCrewmatePostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        if (GameModeManager.CurrentGameMode != CustomGamemode.Normal) return;

        Intro.SetupIntroTeam(__instance, ref teamToDisplay);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    internal static bool BeginImpostorPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (GameModeManager.CurrentGameMode != CustomGamemode.Normal && GameModeManager.CurrentGameModeInstance.OnBeginIntro(__instance, ref yourTeam))
        {
            return false;
        }

        Intro.SetupIntroTeamIcons(__instance, ref yourTeam);
        Intro.BeginImpostor(__instance, ref yourTeam);
        Intro.SetupIntroTeam(__instance, ref yourTeam);

        return false;
    }
}