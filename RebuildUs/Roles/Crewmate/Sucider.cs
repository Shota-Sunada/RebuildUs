namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Suicider : RoleBase<Suicider>
{
    public static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _suicideButton;

    public Suicider()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Suicider;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    public static bool CanEnterVents
    {
        get => CustomOptionHolder.SuiciderCanEnterVents.GetBool();
    }

    public static bool HasImpostorVision
    {
        get => CustomOptionHolder.SuiciderHasImpostorVision.GetBool();
    }

    public static bool CanFixComm
    {
        get => CustomOptionHolder.SuiciderCanFixComm.GetBool();
    }

    public static bool CanKnowImpostorAfterFinishTasks
    {
        get => CustomOptionHolder.SuiciderCanKnowImpostorAfterFinishTasks.GetBool();
    }

    public static int NumCommonTasks
    {
        get => CustomOptionHolder.SuiciderTasks.CommonTasksNum;
    }

    public static int NumLongTasks
    {
        get => CustomOptionHolder.SuiciderTasks.LongTasksNum;
    }

    public static int NumShortTasks
    {
        get => CustomOptionHolder.SuiciderTasks.ShortTasksNum;
    }

    public override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            Update.SetPlayerNameColor(Player, NameColor);

            if (KnowsImpostors(Player))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                        Update.SetPlayerNameColor(p, Palette.ImpostorRed);
                }
            }
        }
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
        _suicideButton = new(() =>
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                sender.Write(PlayerControl.LocalPlayer.PlayerId); // source
                sender.Write(PlayerControl.LocalPlayer.PlayerId); // target
                sender.Write((byte)1); // showAnimation
            }
            RPCProcedure.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, 1);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Suicider) && PlayerControl.LocalPlayer?.Data?.IsDead == false; }, () => { return PlayerControl.LocalPlayer.CanMove; }, () => { _suicideButton.Timer = _suicideButton.MaxTimer; }, hm.KillButton.graphic.sprite, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, 0f, null, false, Tr.Get(TrKey.Suicide));
    }

    public static void SetButtonCooldowns()
    {
        _suicideButton?.MaxTimer = 0f;
    }

    public static bool KnowsImpostors(PlayerControl player)
    {
        return CanKnowImpostorAfterFinishTasks && IsRole(player) && TasksComplete(player);
    }

    public static bool TasksComplete(PlayerControl player)
    {
        if (!CanKnowImpostorAfterFinishTasks) return false;

        var counter = 0;
        var totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
        if (totalTasks == 0) return true;
        foreach (var task in player.Data.Tasks)
        {
            if (task.Complete)
                counter++;
        }

        return counter == totalTasks;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
