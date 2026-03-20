namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Mayor, RoleTeam.Crewmate, typeof(MultiRoleBase<Mayor>), nameof(CustomOptionHolder.MayorSpawnRate))]
internal class Mayor : MultiRoleBase<Mayor>
{
    internal static Color Color = new Color32(32, 77, 66, byte.MaxValue);

    private static CustomButton _mayorMeetingButton;

    // write configs here
    private int _remoteMeetingsLeft;

    public Mayor()
    {
        StaticRoleType = CurrentRoleType = RoleType.Mayor;
        _remoteMeetingsLeft = MayorMaxRemoteMeetings;
    }

    internal static int NumVotes
    {
        get => (int)CustomOptionHolder.MayorNumVotes.GetFloat();
    }

    internal static bool MayorCanSeeVoteColors
    {
        get => CustomOptionHolder.MayorCanSeeVoteColors.GetBool();
    }

    internal static int MayorTasksNeededToSeeVoteColors
    {
        get => (int)CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetFloat();
    }

    private static bool MayorHasMeetingButton
    {
        get => CustomOptionHolder.MayorMeetingButton.GetBool();
    }

    private static int MayorMaxRemoteMeetings
    {
        get => (int)CustomOptionHolder.MayorMaxRemoteMeetings.GetFloat();
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _mayorMeetingButton = new(() =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local._remoteMeetingsLeft--;
                RPCProcedure.UncheckedCmdReportDeadBody(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                _mayorMeetingButton.Timer = 1f;
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Mayor) && PlayerControl.LocalPlayer?.Data?.IsDead == false && MayorHasMeetingButton,
            () =>
            {
                _mayorMeetingButton.ActionButton.OverrideText(string.Format(Tr.Get(TrKey.Emergency), Local._remoteMeetingsLeft));
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType is TaskTypes.FixLights
                                         or TaskTypes.RestoreOxy
                                         or TaskTypes.ResetReactor
                                         or TaskTypes.ResetSeismic
                                         or TaskTypes.FixComms
                                         or TaskTypes.StopCharles
                        || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        sabotageActive = true;
                    }
                }

                return !sabotageActive && PlayerControl.LocalPlayer.CanMove && Local._remoteMeetingsLeft > 0;
            },
            () =>
            {
                _mayorMeetingButton.Timer = _mayorMeetingButton.MaxTimer;
            },
            AssetLoader.EmergencyButton,
            ButtonPosition.Layout,
            hm,
            hm.AbilityButton,
            AbilitySlot.CrewmateAbilityPrimary,
            true,
            0f,
            () => { },
            false,
            Tr.Get(TrKey.Meeting));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _mayorMeetingButton.MaxTimer = Int32OptionNames.EmergencyCooldown.Get();
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}