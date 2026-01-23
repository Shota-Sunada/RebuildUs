using InnerNet;

namespace RebuildUs.Extensions;

public static class PlayerControlHelpers
{
    public static void GenerateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
    {
        if (player == null) return;

        var taskTypeIds = Helpers.GenerateTasks(numCommon, numShort, numLong);
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedSetTasks);
            sender.Write(player.PlayerId);
            sender.WriteBytesAndSize(taskTypeIds.ToArray());
        }
        RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
    }

    public static bool IsDead(this PlayerControl player)
    {
        if (player == null) return true;
        var data = player.Data;
        if (data == null || data.IsDead || data.Disconnected) return true;

        if (GameHistory.FinalStatuses != null && GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out var status))
        {
            return status != EFinalStatus.Alive;
        }

        return false;
    }

    public static bool IsAlive(this PlayerControl player)
    {
        return !IsDead(player);
    }

    public static bool IsNeutral(this PlayerControl player)
    {
        return player != null
            && (player.IsRole(RoleType.Jackal) ||
                player.IsRole(RoleType.Sidekick) ||
                Jackal.FormerJackals.Contains(player) ||
                player.IsRole(RoleType.Arsonist) ||
                player.IsRole(RoleType.Jester) ||
                // player.IsRole(RoleType.Opportunist) ||
                player.IsRole(RoleType.Vulture) ||
                (player.IsRole(RoleType.Shifter) && Shifter.IsNeutral));
    }

    public static bool IsTeamCrewmate(this PlayerControl player)
    {
        return player != null && !player.IsTeamImpostor() && !player.IsNeutral() && !player.IsGM();
    }

    public static bool IsTeamImpostor(this PlayerControl player)
    {
        return player != null && player.Data.Role.IsImpostor;
    }

    public static bool HasFakeTasks(this PlayerControl player)
    {
        return (player.IsNeutral() && !player.NeutralHasTasks())
            || (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks)
            || (player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks)
            || (player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount);
    }

    public static bool NeutralHasTasks(this PlayerControl player)
    {
        return player.IsNeutral() && player.IsRole(RoleType.Shifter);
    }

    public static bool IsGM(this PlayerControl player)
    {
        return false;
        // return GM.gm != null && player == GM.gm;
    }

    public static bool IsLovers(this PlayerControl player)
    {
        return player != null && Lovers.IsLovers(player);
    }

    public static PlayerControl GetPartner(this PlayerControl player)
    {
        return Lovers.GetPartner(player);
    }

    public static bool CanBeErased(this PlayerControl player)
    {
        return !player.IsRole(RoleType.Jackal) && !player.IsRole(RoleType.Sidekick) && !Jackal.FormerJackals.Contains(player);
    }

    public static void ClearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks)
        {
            playerTask.OnRemove();
            UnityEngine.Object.Destroy(playerTask.gameObject);
        }
        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
        {
            player.Data.Tasks.Clear();
        }
    }

    public static bool CanUseVents(this PlayerControl player)
    {
        bool roleCouldUse = false;
        if (player.IsRole(RoleType.Engineer))
        {
            roleCouldUse = true;
        }
        else if (Jackal.CanUseVents && player.IsRole(RoleType.Jackal))
        {
            roleCouldUse = true;
        }
        else if (Sidekick.CanUseVents && player.IsRole(RoleType.Sidekick))
        {
            roleCouldUse = true;
        }
        else if (Spy.CanEnterVents && player.IsRole(RoleType.Spy))
        {
            roleCouldUse = true;
        }
        else if (Madmate.CanEnterVents && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.CanEnterVents && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Vulture.CanUseVents && player.IsRole(RoleType.Vulture))
        {
            roleCouldUse = true;
        }
        else if (player.Data?.Role != null && player.Data.Role.CanVent)
        {
            if (!Mafia.Janitor.CanVent && player.IsRole(RoleType.Janitor))
            {
                roleCouldUse = false;
            }
            else if (!Mafia.Mafioso.CanVent && player.IsRole(RoleType.Mafioso))
            {
                roleCouldUse = false;
            }
            else
            {
                roleCouldUse = true;
            }
        }
        return roleCouldUse;
    }

    public static bool CanSabotage(this PlayerControl player)
    {
        bool roleCouldUse = false;
        if (Madmate.CanSabotage && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.CanSabotage && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Jester.CanSabotage && player.IsRole(RoleType.Jester))
        {
            roleCouldUse = true;
        }
        else if (!Mafia.Mafioso.CanSabotage && player.IsRole(RoleType.Mafioso))
        {
            roleCouldUse = false;
        }
        else if (!Mafia.Janitor.CanSabotage && player.IsRole(RoleType.Janitor))
        {
            roleCouldUse = false;
        }
        else if (player.Data?.Role != null && player.Data.Role.IsImpostor)
        {
            roleCouldUse = true;
        }

        return roleCouldUse;
    }

    public static ClientData GetClient(this PlayerControl player)
    {
        if (player == null) return null;
        var allClients = AmongUsClient.Instance.allClients;
        for (int i = 0; i < allClients.Count; i++)
        {
            var cd = allClients[i];
            if (cd?.Character != null && cd.Character.PlayerId == player.PlayerId) return cd;
        }
        return null;
    }

    public static string GetPlatform(this PlayerControl player)
    {
        var client = player.GetClient();
        return client != null ? client.PlatformData.Platform.ToString() : "Unknown";
    }

    private static readonly StringBuilder RoleStringBuilder = new();
    public static string GetRoleName(this PlayerControl player) => RoleInfo.GetRolesString(player, false, joinSeparator: " + ");

    public static string GetNameWithRole(this PlayerControl player)
    {
        if (player == null || player.Data == null) return "";
        RoleStringBuilder.Clear();
        RoleStringBuilder.Append(player.Data.PlayerName)
            .Append(" (")
            .Append(player.GetRoleName())
            .Append(')');
        return RoleStringBuilder.ToString();
    }

    public static void MurderPlayer(this PlayerControl player, PlayerControl target)
    {
        player.MurderPlayer(target, MurderResultFlags.Succeeded);
    }
}