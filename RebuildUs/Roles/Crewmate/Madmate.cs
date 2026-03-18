namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Madmate, RoleTeam.Crewmate, typeof(MultiRoleBase<MadmateRole>), nameof(CustomOptionHolder.MadmateRoleSpawnRate))]
internal class MadmateRole : MultiRoleBase<MadmateRole>
{
    internal static new Color RoleColor = Palette.ImpostorRed;

    public MadmateRole()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Madmate;
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

    internal override void OnUpdateRoleColors()
    {
        if (Player != PlayerControl.LocalPlayer)
        {
            return;
        }
        HudManagerPatch.SetPlayerNameColor(Player, RoleColor);

        if (!KnowsImpostors(Player))
        {
            return;
        }
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
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

        var counter = 0;
        var totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
        if (totalTasks == 0)
        {
            return true;
        }
        foreach (var task in player.Data.Tasks)
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