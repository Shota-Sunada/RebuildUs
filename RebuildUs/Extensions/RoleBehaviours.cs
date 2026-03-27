namespace RebuildUs.Extensions;

internal static class RoleBehaviours
{
    extension(PlayerControl player)
    {
        internal bool CanUseVents()
        {
            if (GameModeManager.CurrentGameMode != CustomGamemode.Normal) return false;

            var roleCouldUse = false;
            if (player.IsRole(RoleType.Engineer)
                || Jackal.CanUseVents && player.IsRole(RoleType.Jackal)
                || Sidekick.CanUseVents && player.IsRole(RoleType.Sidekick)
                || Spy.CanEnterVents && player.IsRole(RoleType.Spy)
                || Madmate.CanEnterVents && player.HasModifier(ModifierType.Madmate)
                || CreatedMadmate.CanEnterVents && player.HasModifier(ModifierType.CreatedMadmate)
                || Vulture.CanUseVents && player.IsRole(RoleType.Vulture)
                || player.IsTeamImpostor() && !player.IsRole(RoleType.Mafioso) && !player.IsRole(RoleType.Janitor)
                || player.IsRole(RoleType.Mafioso) && (Mafia.Mafioso.CanVent || Mafia.IsGodfatherDead)
                || player.IsRole(RoleType.Janitor) && Mafia.Janitor.CanVent)
            {
                roleCouldUse = true;
            }

            return roleCouldUse;
        }

        internal bool CanMoveInVents()
        {
            return !player.IsRole(RoleType.Spy)
                && !player.HasModifier(ModifierType.Madmate)
                && !player.HasModifier(ModifierType.CreatedMadmate);
        }

        internal bool CanSabotage()
        {
            if (GameModeManager.CurrentGameMode != CustomGamemode.Normal) return false;

            var roleCouldUse = false;
            if (Madmate.CanSabotage && player.HasModifier(ModifierType.Madmate)
            || CreatedMadmate.CanSabotage && player.HasModifier(ModifierType.CreatedMadmate)
            || Jester.CanSabotage && player.IsRole(RoleType.Jester)
            || (Mafia.Mafioso.CanSabotage || Mafia.IsGodfatherDead) && player.IsRole(RoleType.Mafioso)
            || Mafia.Janitor.CanSabotage && player.IsRole(RoleType.Janitor)
            || player.IsTeamImpostor() && !player.IsRole(RoleType.Mafioso) && !player.IsRole(RoleType.Janitor))
            {
                roleCouldUse = true;
            }

            return roleCouldUse;
        }

        internal bool HasFakeTasks()
        {
            return player.IsNeutral() && !player.NeutralHasTasks()
                || player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks
                || player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks
                || player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount;
        }

        internal bool IsNeutral()
        {
            return player != null && (
                player.IsRole(RoleType.Jackal) ||
                player.IsRole(RoleType.Sidekick) ||
                Jackal.FormerJackals.Contains(player) ||
                player.IsRole(RoleType.Arsonist) ||
                player.IsRole(RoleType.Jester) ||
                // player.IsRole(RoleType.Opportunist) ||
                player.IsRole(RoleType.Vulture) ||
                player.IsRole(RoleType.Shifter) && Shifter.IsNeutral);
        }

        internal bool NeutralHasTasks()
        {
            return player.IsNeutral() && player.IsRole(RoleType.Shifter);
        }
    }
}