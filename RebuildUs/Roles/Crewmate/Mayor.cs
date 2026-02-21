namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Mayor : RoleBase<Mayor>
{
    internal static Color NameColor = new Color32(32, 77, 66, byte.MaxValue);

    private static CustomButton _mayorMeetingButton;

    // write configs here
    private int _remoteMeetingsLeft;

    public Mayor()
    {
        StaticRoleType = CurrentRoleType = RoleType.Mayor;
        _remoteMeetingsLeft = MayorMaxRemoteMeetings;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static int NumVotes { get => (int)CustomOptionHolder.MayorNumVotes.GetFloat(); }
    internal static bool MayorCanSeeVoteColors { get => CustomOptionHolder.MayorCanSeeVoteColors.GetBool(); }
    internal static int MayorTasksNeededToSeeVoteColors { get => (int)CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetFloat(); }
    private static bool MayorHasMeetingButton { get => CustomOptionHolder.MayorMeetingButton.GetBool(); }
    private static int MayorMaxRemoteMeetings { get => (int)CustomOptionHolder.MayorMaxRemoteMeetings.GetFloat(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _mayorMeetingButton = new(() =>
        {
            PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
            Local._remoteMeetingsLeft--;
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedCmdReportDeadBody);
            sender.Write(PlayerControl.LocalPlayer.PlayerId);
            sender.Write(byte.MaxValue);
            RPCProcedure.UncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
            _mayorMeetingButton.Timer = 1f;
        }, () => PlayerControl.LocalPlayer.IsRole(RoleType.Mayor) && PlayerControl.LocalPlayer?.Data?.IsDead == false && MayorHasMeetingButton, () =>
        {
            _mayorMeetingButton.ActionButton.OverrideText(string.Format(Tr.Get(TrKey.Emergency), Local._remoteMeetingsLeft));
            bool sabotageActive = false;
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
            {
                if (task.TaskType is TaskTypes.FixLights
                                     or TaskTypes.RestoreOxy
                                     or TaskTypes.ResetReactor
                                     or TaskTypes.ResetSeismic
                                     or TaskTypes.FixComms
                                     or TaskTypes.StopCharles
                    || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                {
                    sabotageActive = true;
                }
            }

            return !sabotageActive && PlayerControl.LocalPlayer.CanMove && Local._remoteMeetingsLeft > 0;
        }, () => { _mayorMeetingButton.Timer = _mayorMeetingButton.MaxTimer; }, AssetLoader.EmergencyButton, ButtonPosition.Layout, hm, hm.AbilityButton, AbilitySlot.CrewmateAbilityPrimary, true, 0f, () => { }, false, Tr.Get(TrKey.Meeting));
    }

    internal static void SetButtonCooldowns()
    {
        _mayorMeetingButton.MaxTimer = Helpers.GetOption(Int32OptionNames.EmergencyCooldown);
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}