namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class KillButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal static bool DoClickPrefix(KillButton __instance)
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (!__instance.isActiveAndEnabled || !__instance.currentTarget || __instance.isCoolingDown || lp.Data.IsDead || !lp.CanMove)
        {
            return false;
        }
        const bool showAnimation = true;

        // Use an unchecked kill command, to allow shorter kill cooldowns etc. without getting kicked
        MurderAttemptResult res = Helpers.CheckMurderAttemptAndKill(lp, __instance.currentTarget, showAnimation: showAnimation);
        // Handle blank kill
        if (res == MurderAttemptResult.BlankKill)
        {
            float cooldown = Helpers.GetOption(FloatOptionNames.KillCooldown);
            lp.SetKillTimer(cooldown);
            if (lp.IsRole(RoleType.Cleaner))
            {
                lp.killTimer = Cleaner.CleanerCleanButton.Timer = Cleaner.CleanerCleanButton.MaxTimer;
            }
            else if (lp.IsRole(RoleType.Warlock))
            {
                lp.killTimer = Warlock.WarlockCurseButton.Timer = Warlock.WarlockCurseButton.MaxTimer;
            }
            else if (lp.HasModifier(ModifierType.Mini) && lp.Data.Role.IsImpostor)
            {
                lp.SetKillTimer(cooldown * (Mini.IsGrownUp(lp) ? 0.66f : 2f));
            }
            else if (lp.IsRole(RoleType.Witch))
            {
                lp.killTimer = Witch.WitchSpellButton.Timer = Witch.WitchSpellButton.MaxTimer;
            }
        }

        __instance.SetTarget(null);

        return false;
    }
}