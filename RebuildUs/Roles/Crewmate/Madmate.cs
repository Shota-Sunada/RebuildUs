namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class MadmateRole : MultiRoleBase<MadmateRole>
{
    internal static Color NameColor = Palette.ImpostorRed;

    public MadmateRole()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Madmate;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static bool CanEnterVents
    {
        get => CustomOptionHolder.MadmateRoleCanEnterVents.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.MadmateRoleHasImpostorVision.GetBool();
    }

    internal static bool CanSabotage
    {
        get => CustomOptionHolder.MadmateRoleCanSabotage.GetBool();
    }

    internal static bool CanFixComm
    {
        get => CustomOptionHolder.MadmateRoleCanFixComm.GetBool();
    }

    internal static bool CanKnowImpostorAfterFinishTasks
    {
        get => CustomOptionHolder.MadmateRoleCanKnowImpostorAfterFinishTasks.GetBool();
    }

    private static int NumCommonTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.CommonTasksNum;
    }

    private static int NumLongTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.LongTasksNum;
    }

    private static int NumShortTasks
    {
        get => CustomOptionHolder.MadmateRoleTasks.ShortTasksNum;
    }

    internal override void OnUpdateNameColors()
    {
        if (Player != PlayerControl.LocalPlayer)
        {
            return;
        }
        HudManagerPatch.SetPlayerNameColor(Player, NameColor);

        if (!KnowsImpostors(Player))
        {
            return;
        }
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamImpostor()
                || p.IsRole(RoleType.Spy)
                || p.IsRole(RoleType.Jackal) && Jackal.Instance.WasTeamRed
                || p.IsRole(RoleType.Sidekick) && Sidekick.Instance.WasTeamRed)
            {
                HudManagerPatch.SetPlayerNameColor(p, Palette.ImpostorRed);
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

    private static bool KnowsImpostors(PlayerControl player)
    {
        return CanKnowImpostorAfterFinishTasks && IsRole(player) && TasksComplete(player);
    }

    private static bool TasksComplete(PlayerControl player)
    {
        if (!CanKnowImpostorAfterFinishTasks)
        {
            return false;
        }

        int counter = 0;
        int totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
        if (totalTasks == 0)
        {
            return true;
        }
        foreach (NetworkedPlayerInfo.TaskInfo task in player.Data.Tasks)
        {
            if (task.Complete)
            {
                counter++;
            }
        }

        return counter == totalTasks;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}