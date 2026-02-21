namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Suicider : RoleBase<Suicider>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _suicideButton;

    public Suicider()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Suicider;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static bool CanEnterVents { get => CustomOptionHolder.SuiciderCanEnterVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SuiciderHasImpostorVision.GetBool(); }
    internal static bool CanFixComm { get => CustomOptionHolder.SuiciderCanFixComm.GetBool(); }
    internal static bool CanKnowImpostorAfterFinishTasks { get => CustomOptionHolder.SuiciderCanKnowImpostorAfterFinishTasks.GetBool(); }
    internal static int NumCommonTasks { get => CustomOptionHolder.SuiciderTasks.CommonTasksNum; }
    internal static int NumLongTasks { get => CustomOptionHolder.SuiciderTasks.LongTasksNum; }
    internal static int NumShortTasks { get => CustomOptionHolder.SuiciderTasks.ShortTasksNum; }

    internal override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            Update.SetPlayerNameColor(Player, NameColor);

            if (KnowsImpostors(Player))
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                        Update.SetPlayerNameColor(p, Palette.ImpostorRed);
            }
        }
    }

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
        _suicideButton = new(() =>
        {
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                sender.Write(PlayerControl.LocalPlayer.PlayerId); // source
                sender.Write(PlayerControl.LocalPlayer.PlayerId); // target
                sender.Write((byte)1); // showAnimation
            }
            RPCProcedure.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, 1);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Suicider) && PlayerControl.LocalPlayer?.Data?.IsDead == false; }, () => { return PlayerControl.LocalPlayer.CanMove; }, () => { _suicideButton.Timer = _suicideButton.MaxTimer; }, hm.KillButton.graphic.sprite, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, 0f, null, false, Tr.Get(TrKey.Suicide));
    }

    internal static void SetButtonCooldowns()
    {
        _suicideButton?.MaxTimer = 0f;
    }

    internal static bool KnowsImpostors(PlayerControl player)
    {
        return CanKnowImpostorAfterFinishTasks && IsRole(player) && TasksComplete(player);
    }

    internal static bool TasksComplete(PlayerControl player)
    {
        if (!CanKnowImpostorAfterFinishTasks) return false;

        int counter = 0;
        int totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
        if (totalTasks == 0) return true;
        foreach (NetworkedPlayerInfo.TaskInfo task in player.Data.Tasks)
            if (task.Complete)
                counter++;

        return counter == totalTasks;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}