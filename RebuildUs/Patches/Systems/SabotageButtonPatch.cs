namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class SabotageButtonPatch
{
    // [HarmonyPostfix]
    // [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    // internal static void RefreshPostfix()
    // {
    //     if (PlayerControl.LocalPlayer == null)
    //     {
    //         return;
    //     }

    //     // Mafia disable sabotage button for Janitor and sometimes for Mafioso
    //     var blockSabotageJanitor = PlayerControl.LocalPlayer.IsRole(RoleType.Janitor) && Mafia.Janitor.CanSabotage;
    //     var blockSabotageMafioso = PlayerControl.LocalPlayer.IsRole(RoleType.Mafioso) && Mafia.Mafioso.CanSabotage && !Mafia.IsGodfatherDead;
    //     if (blockSabotageJanitor || blockSabotageMafioso)
    //     {
    //         FastDestroyableSingleton<HudManager>.Instance.SabotageButton.SetDisabled();
    //     }
    // }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    internal static bool DoClickPrefix(SabotageButton __instance)
    {
        // The sabotage button behaves just fine if it's a regular impostor
        if (PlayerControl.LocalPlayer?.Data?.Role?.TeamType == RoleTeamTypes.Impostor)
        {
            return true;
        }

        FastDestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new()
        {
            Mode = MapOptions.Modes.Sabotage,
        });
        return false;
    }
}