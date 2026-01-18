using InnerNet;
using RebuildUs.Modules.RPC;
using RebuildUs.Players;
using RebuildUs.Roles;
using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Modifier;
using RebuildUs.Roles.Neutral;
using RebuildUs.Utilities;

namespace RebuildUs.Extensions;

public static class PlayerControlHelpers
{
    public static void GenerateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
    {
        if (player == null) return;

        var taskTypeIds = Helpers.GenerateTasks(numCommon, numShort, numLong);
        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UncheckedSetTasks);
        sender.Write(player.PlayerId);
        sender.WriteBytesAndSize(taskTypeIds.ToArray());
        RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
    }

    public static bool IsDead(this PlayerControl player)
    {
        return player == null
            || player?.Data?.IsDead == true
            || player?.Data?.Disconnected == true
            || (GameHistory.FinalStatuses != null && GameHistory.FinalStatuses.ContainsKey(player.PlayerId) && GameHistory.FinalStatuses[player.PlayerId] != EFinalStatus.Alive);
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
                player.IsRole(RoleType.Opportunist) ||
                player.IsRole(RoleType.PlagueDoctor) ||
                player.IsRole(RoleType.Vulture) ||
                (player.IsRole(RoleType.Shifter) && Shifter.isNeutral));
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
            || (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.hasTasks)
            || (player.HasModifier(ModifierType.Madmate) && !Madmate.hasTasks)
            || (player.IsLovers() && Lovers.separateTeam && !Lovers.tasksCount);
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
        return player != null && Lovers.isLovers(player);
    }

    public static PlayerControl getPartner(this PlayerControl player)
    {
        return Lovers.getPartner(player);
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
        else if (Madmate.canEnterVents && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.canEnterVents && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Vulture.CanUseVents && player.IsRole(RoleType.Vulture))
        {
            roleCouldUse = true;
        }
        else if (player.Data?.Role != null && player.Data.Role.CanVent)
        {
            if (!Mafia.Janitor.canVent && player.IsRole(RoleType.Janitor))
            {
                roleCouldUse = false;
            }
            else if (!Mafia.Mafioso.canVent && player.IsRole(RoleType.Mafioso))
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
        if (Madmate.canSabotage && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.canSabotage && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Jester.CanSabotage && player.IsRole(RoleType.Jester))
        {
            roleCouldUse = true;
        }
        else if (!Mafia.Mafioso.canSabotage && player.IsRole(RoleType.Mafioso))
        {
            roleCouldUse = false;
        }
        else if (!Mafia.Janitor.canSabotage && player.IsRole(RoleType.Janitor))
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
        return AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
    }

    public static string GetPlatform(this PlayerControl player)
    {
        return player.GetClient().PlatformData.Platform.ToString();
    }

    public static string GetRoleName(this PlayerControl player) => RoleInfo.GetRolesString(player, false, joinSeparator: " + ");
    public static string GetNameWithRole(this PlayerControl player) => $"{player.Data.PlayerName} ({player.GetRoleName()})";

    public static void MurderPlayer(this PlayerControl player, PlayerControl target)
    {
        player.MurderPlayer(target, MurderResultFlags.Succeeded);
    }
}