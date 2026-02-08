namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class MadmateRole : RoleBase<MadmateRole>
{
    public static Color NameColor = Palette.ImpostorRed;

    public MadmateRole()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Madmate;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    public static bool CanEnterVents
    {
        get => CustomOptionHolder.MadmateRoleCanEnterVents.GetBool();
    }

    public static bool HasImpostorVision
    {
        get => CustomOptionHolder.MadmateRoleHasImpostorVision.GetBool();
    }

    public static bool CanSabotage
    {
        get => CustomOptionHolder.MadmateRoleCanSabotage.GetBool();
    }

    public static bool CanFixComm
    {
        get => CustomOptionHolder.MadmateRoleCanFixComm.GetBool();
    }

    public static bool CanKnowImpostorAfterFinishTasks
    {
        get => CustomOptionHolder.MadmateRoleCanKnowImpostorAfterFinishTasks.GetBool();
    }

    public static int NumCommonTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.CommonTasksNum;
    }

    public static int NumLongTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.LongTasksNum;
    }

    public static int NumShortTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.ShortTasksNum;
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
