namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SabotageButtonPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    public static void RefreshPostfix()
    {
        // Mafia disable sabotage button for Janitor and sometimes for Mafioso
        bool blockSabotageJanitor = CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Janitor);
        bool blockSabotageMafioso = CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mafioso) && !Mafia.IsGodfatherDead;
        if (blockSabotageJanitor || blockSabotageMafioso)
        {
            FastDestroyableSingleton<HudManager>.Instance.SabotageButton.SetDisabled();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public static bool DoClickPrefix(SabotageButton __instance)
    {
        // The sabotage button behaves just fine if it's a regular impostor
        if (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.TeamType == RoleTeamTypes.Impostor) return true;

        // FastDestroyableSingleton<HudManager>.Instance.ToggleMapVisible((Il2CppSystem.Action<MapBehaviour>)((m) => { m.ShowSabotageMap(); }));
        FastDestroyableSingleton<MapBehaviour>.Instance.ShowSabotageMap();
        return false;
    }
}