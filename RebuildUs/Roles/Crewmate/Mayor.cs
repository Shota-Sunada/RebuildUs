namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Mayor, RoleTeam.Crewmate, typeof(MultiRoleBase<Mayor>), nameof(CustomOptionHolder.MayorSpawnRate))]
internal class Mayor : MultiRoleBase<Mayor>
{
    public static Color Color = new Color32(32, 77, 66, byte.MaxValue);

    private static CustomButton MayorMeetingButton;
    internal static TMP_Text MayorRemoteButtonLeftText;

    private int _remoteMeetingsLeft = 0;

    public Mayor()
    {
        StaticRoleType = CurrentRoleType = RoleType.Mayor;
        _remoteMeetingsLeft = MayorMaxRemoteMeetings;
    }

    internal static int NumVotes { get => (int)CustomOptionHolder.MayorNumVotes.GetFloat(); }
    internal static bool MayorCanSeeVoteColors { get => CustomOptionHolder.MayorCanSeeVoteColors.GetBool(); }
    internal static int MayorTasksNeededToSeeVoteColors { get => (int)CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetFloat(); }
    private static bool MayorHasMeetingButton { get => CustomOptionHolder.MayorMeetingButton.GetBool(); }
    private static int MayorMaxRemoteMeetings { get => (int)CustomOptionHolder.MayorMaxRemoteMeetings.GetFloat(); }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        MayorMeetingButton = new(
            nameof(MayorMeetingButton),
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local._remoteMeetingsLeft--;
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedCmdReportDeadBody);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                sender.Write(byte.MaxValue);
                RPCProcedure.UncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                MayorMeetingButton.Timer = 1f;
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Mayor) && PlayerControl.LocalPlayer.IsAlive() && MayorHasMeetingButton,
            () =>
            {
                MayorMeetingButton?.ActionButton.OverrideText(Tr.Get(TrKey.Emergency));
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles
                        || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        sabotageActive = true;
                    }
                }

                MayorRemoteButtonLeftText?.text = Local._remoteMeetingsLeft > 0 ? string.Format(Tr.Get(TrKey.RemainButtonNum), Local._remoteMeetingsLeft) : "";
                return !sabotageActive && PlayerControl.LocalPlayer.CanMove && Local._remoteMeetingsLeft > 0;
            },
            () =>
            {
                MayorMeetingButton.Timer = MayorMeetingButton.MaxTimer;
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

        MayorRemoteButtonLeftText = UnityObject.Instantiate(MayorMeetingButton.ActionButton.cooldownTimerText, MayorMeetingButton.ActionButton.cooldownTimerText.transform.parent);
        MayorRemoteButtonLeftText.text = "";
        MayorRemoteButtonLeftText.enableWordWrapping = false;
        MayorRemoteButtonLeftText.transform.localScale = Vector3.one * 0.5f;
        MayorRemoteButtonLeftText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        MayorMeetingButton.MaxTimer = Int32OptionNames.EmergencyCooldown.Get();
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}