using InnerNet;
using RebuildUs.Modules.RPC;
using RebuildUs.Players;
using RebuildUs.Roles;
using RebuildUs.Roles.Crewmate;
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
        return false;
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
        return false;
        // return (player.IsNeutral() && !player.NeutralHasTasks()) ||
        //        (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.hasTasks) ||
        //        (player.HasModifier(ModifierType.Madmate) && !Madmate.hasTasks) ||
        //        (player.IsLovers() && Lovers.separateTeam && !Lovers.tasksCount);
    }

    public static bool NeutralHasTasks(this PlayerControl player)
    {
        return false;
        // if (player.IsRole(RoleType.SchrodingersCat) && SchrodingersCat.hideRole) return true;
        // if (player.IsRole(RoleType.JekyllAndHyde)) return true;
        // return player.IsNeutral() && (player.IsRole(ERoleType.Lawyer) || player.isRole(RoleType.Pursuer) || player.isRole(RoleType.Shifter) || player.isRole(RoleType.Fox));
    }

    public static bool IsGM(this PlayerControl player)
    {
        return false;
        // return GM.gm != null && player == GM.gm;
    }

    public static bool IsLovers(this PlayerControl player)
    {
        return false;
        // return player != null && Lovers.isLovers(player);
    }

    public static bool CanBeErased(this PlayerControl player)
    {
        return false;
        // return player != Jackal.jackal && player != Sidekick.sidekick && !Jackal.formerJackals.Contains(player);
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
        if (player.IsRole(ERoleType.Engineer))
            roleCouldUse = true;
        else if (Jackal.CanUseVents && player.IsRole(ERoleType.Jackal))
            roleCouldUse = true;
        else if (Sidekick.CanUseVents && player.IsRole(ERoleType.Sidekick))
            roleCouldUse = true;
        else if (Spy.CanEnterVents && player.IsRole(ERoleType.Spy))
            roleCouldUse = true;
        // else if (Madmate.canEnterVents && player.hasModifier(ModifierType.Madmate))
        //     roleCouldUse = true;
        // else if (CreatedMadmate.canEnterVents && player.hasModifier(ModifierType.CreatedMadmate))
        //     roleCouldUse = true;
        else if (Vulture.CanUseVents && player.IsRole(ERoleType.Vulture))
            roleCouldUse = true;
        // else if (player.isRole(RoleType.JekyllAndHyde) && !JekyllAndHyde.isJekyll())
        //     roleCouldUse = true;
        // else if (player.isRole(RoleType.Moriarty))
        //     roleCouldUse = true;
        // else if (player.Data?.Role != null && player.Data.Role.CanVent)
        // {
        //     if (!Janitor.canVent && player.isRole(RoleType.Janitor))
        //         roleCouldUse = false;
        //     else if (!Mafioso.canVent && player.isRole(RoleType.Mafioso))
        //         roleCouldUse = false;
        //     else if (!Ninja.canUseVents && player.isRole(RoleType.Ninja))
        //         roleCouldUse = false;
        //     else
        //         roleCouldUse = true;
        // }
        return roleCouldUse;
    }

    public static bool CanSabotage(this PlayerControl player)
    {
        bool roleCouldUse = false;
        // if (Madmate.canSabotage && player.hasModifier(ModifierType.Madmate))
        //     roleCouldUse = true;
        // else if (CreatedMadmate.canSabotage && player.hasModifier(ModifierType.CreatedMadmate))
        //     roleCouldUse = true;
        if (Jester.CanSabotage && player.IsRole(ERoleType.Jester))
            roleCouldUse = true;
        // else if (!Mafioso.canSabotage && player.isRole(RoleType.Mafioso))
        //     roleCouldUse = false;
        // else if (!Janitor.canSabotage && player.isRole(RoleType.Janitor))
        //     roleCouldUse = false;
        // else if (player.Data?.Role != null && player.Data.Role.IsImpostor)
        //     roleCouldUse = true;

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