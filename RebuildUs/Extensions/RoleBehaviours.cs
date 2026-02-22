namespace RebuildUs.Extensions;

internal static class RoleBehaviours
{
    extension(PlayerControl player)
    {
        internal bool CanUseVents()
        {
            bool roleCouldUse = false;
            if (player.IsRole(RoleType.Engineer)
                || (Jackal.CanUseVents && player.IsRole(RoleType.Jackal))
                || (Sidekick.CanUseVents && player.IsRole(RoleType.Sidekick))
                || (Spy.CanEnterVents && player.IsRole(RoleType.Spy))
                || (Madmate.CanEnterVents && player.HasModifier(ModifierType.Madmate))
                || (MadmateRole.CanEnterVents && player.IsRole(RoleType.Madmate))
                || (Suicider.CanEnterVents && player.IsRole(RoleType.Suicider))
                || (CreatedMadmate.CanEnterVents && player.HasModifier(ModifierType.CreatedMadmate))
                || (Vulture.CanUseVents && player.IsRole(RoleType.Vulture)))
            {
                roleCouldUse = true;
            }
            else if (player.Data?.Role != null && player.Data.Role.CanVent)
            {
                if ((!Mafia.Janitor.CanVent && player.IsRole(RoleType.Janitor))
                    || (!Mafia.Mafioso.CanVent && player.IsRole(RoleType.Mafioso)))
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

        internal bool CanMoveInVents()
        {
            return !player.IsRole(RoleType.Spy)
                   && !player.HasModifier(ModifierType.Madmate)
                   && !player.IsRole(RoleType.Madmate)
                   && !player.IsRole(RoleType.Suicider)
                   && !player.HasModifier(ModifierType.CreatedMadmate);
        }

        internal bool CanSabotage()
        {
            bool roleCouldUse = false;
            if ((Madmate.CanSabotage && player.HasModifier(ModifierType.Madmate))
                || (MadmateRole.CanSabotage && player.IsRole(RoleType.Madmate))
                || (CreatedMadmate.CanSabotage && player.HasModifier(ModifierType.CreatedMadmate))
                || (Jester.CanSabotage && player.IsRole(RoleType.Jester))
                || (Mafia.Mafioso.CanSabotage && player.IsRole(RoleType.Mafioso))
                || (Mafia.Janitor.CanSabotage && player.IsRole(RoleType.Janitor))
                || (player.Data?.Role != null && player.Data.Role.IsImpostor))
            {
                roleCouldUse = true;
            }

            return roleCouldUse;
        }

        internal bool HasFakeTasks()
        {
            return (player.IsNeutral() && !player.NeutralHasTasks())
                   || (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks)
                   || (player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks)
                   || (player.IsRole(RoleType.Madmate) && !MadmateRole.CanKnowImpostorAfterFinishTasks)
                   || (player.IsRole(RoleType.Suicider) && !Suicider.CanKnowImpostorAfterFinishTasks)
                   || (player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount);
        }

        internal bool IsNeutral()
        {
            return player != null
                   && (player.IsRole(RoleType.Jackal)
                       || player.IsRole(RoleType.Sidekick)
                       || Jackal.FormerJackals.Contains(player)
                       || player.IsRole(RoleType.Arsonist)
                       || player.IsRole(RoleType.Jester)
                       ||
                       // player.IsRole(RoleType.Opportunist) ||
                       player.IsRole(RoleType.Vulture)
                       || (player.IsRole(RoleType.Shifter) && Shifter.IsNeutral));
        }

        internal bool NeutralHasTasks()
        {
            return player.IsNeutral() && player.IsRole(RoleType.Shifter);
        }
    }
}