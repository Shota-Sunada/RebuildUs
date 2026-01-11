namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Mayor : RoleBase<Mayor>
{
    public static Color RoleColor = new Color32(32, 77, 66, byte.MaxValue);

    public static Minigame emergency = null;
    public static Sprite emergencySprite = null;
    public static CustomButton mayorMeetingButton;

    // write configs here
    public int remoteMeetingsLeft = 1;
    public static int numVotes { get { return CustomOptionHolder.mayorNumVotes.GetSelection(); } }
    public static bool mayorCanSeeVoteColors { get { return CustomOptionHolder.mayorCanSeeVoteColors.GetBool(); } }
    public static int mayorTasksNeededToSeeVoteColors { get { return (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.GetFloat(); } }
    public static bool mayorHasMeetingButton { get { return CustomOptionHolder.mayorMeetingButton.GetBool(); } }
    public static int mayorMaxRemoteMeetings { get { return (int)CustomOptionHolder.mayorMaxRemoteMeetings.GetFloat(); } }

    public Mayor()
    {
        StaticRoleType = CurrentRoleType = ERoleType.Mayor;
        Local.remoteMeetingsLeft = mayorMaxRemoteMeetings;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        mayorMeetingButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local.remoteMeetingsLeft--;
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedCmdReportDeadBody);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                sender.Write(byte.MaxValue);
                RPCProcedure.uncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                mayorMeetingButton.Timer = 1f;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Mayor) && !PlayerControl.LocalPlayer.Data.IsDead && mayorHasMeetingButton; },
            () =>
            {
                mayorMeetingButton.actionButton.OverrideText("Emergency (" + Local.remoteMeetingsLeft + ")");
                bool sabotageActive = false;
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles
                    || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                    {
                        sabotageActive = true;
                    }
                }

                return !sabotageActive && PlayerControl.LocalPlayer.CanMove && Local.remoteMeetingsLeft > 0;
            },
            () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
            getMeetingSprite(),
            CustomButton.ButtonPositions.lowerRowRight,
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
    public static void SetButtonCooldowns()
    {
        mayorMeetingButton.MaxTimer = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
    }

    // write functions here
    public static Sprite getMeetingSprite()
    {
        if (emergencySprite) return emergencySprite;
        emergencySprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.EmergencyButton.png", 550f);
        return emergencySprite;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}