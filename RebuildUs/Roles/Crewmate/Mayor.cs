namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Mayor : RoleBase<Mayor>
{
    public static Color NameColor = new Color32(32, 77, 66, byte.MaxValue);
    public override Color RoleColor => NameColor;

    public static Minigame Emergency = null;
    public static CustomButton MayorMeetingButton;

    // write configs here
    public int RemoteMeetingsLeft = 1;
    public static int NumVotes { get { return CustomOptionHolder.MayorNumVotes.GetSelection(); } }
    public static bool MayorCanSeeVoteColors { get { return CustomOptionHolder.MayorCanSeeVoteColors.GetBool(); } }
    public static int MayorTasksNeededToSeeVoteColors { get { return (int)CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetFloat(); } }
    public static bool MayorHasMeetingButton { get { return CustomOptionHolder.MayorMeetingButton.GetBool(); } }
    public static int MayorMaxRemoteMeetings { get { return (int)CustomOptionHolder.MayorMaxRemoteMeetings.GetFloat(); } }

    public Mayor()
    {
        StaticRoleType = CurrentRoleType = RoleType.Mayor;
        Local.RemoteMeetingsLeft = MayorMaxRemoteMeetings;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        MayorMeetingButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local.RemoteMeetingsLeft--;
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedCmdReportDeadBody);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                sender.Write(byte.MaxValue);
                RPCProcedure.UncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                MayorMeetingButton.Timer = 1f;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mayor) && !PlayerControl.LocalPlayer.Data.IsDead && MayorHasMeetingButton; },
            () =>
            {
                MayorMeetingButton.ActionButton.OverrideText("Emergency (" + Local.RemoteMeetingsLeft + ")");
                bool sabotageActive = false;
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles
                    || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                    {
                        sabotageActive = true;
                    }
                }

                return !sabotageActive && PlayerControl.LocalPlayer.CanMove && Local.RemoteMeetingsLeft > 0;
            },
            () => { MayorMeetingButton.Timer = MayorMeetingButton.MaxTimer; },
            AssetLoader.EmergencyButton,
            CustomButton.ButtonPositions.LowerRowRight,
            hm,
            hm.AbilityButton,
            KeyCode.F,
            true,
            0f,
            () => { },
            false,
            "Meeting"
        );
    }
    public override void SetButtonCooldowns()
    {
        MayorMeetingButton.MaxTimer = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}