namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SabotageButtonPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    public static void RefreshPostfix()
    {
        Usables.SabotageButtonRefresh();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public static bool DoClickPrefix(SabotageButton __instance)
    {
        // The sabotage button behaves just fine if it's a regular impostor
        if (PlayerControl.LocalPlayer.Data.Role.TeamType == RoleTeamTypes.Impostor) return true;

        // FastDestroyableSingleton<HudManager>.Instance.ToggleMapVisible((Il2CppSystem.Action<MapBehaviour>)((m) => { m.ShowSabotageMap(); }));
        FastDestroyableSingleton<MapBehaviour>.Instance.ShowSabotageMap();
        return false;
    }
}