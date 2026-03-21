namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
[RegisterModifier(ModifierType.CreatedMadmate, typeof(CreatedMadmate), nameof(CustomOptionHolder.EvilHackerSpawnRate))]
internal class CreatedMadmate : ModifierBase<CreatedMadmate>
{
    public CreatedMadmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.CreatedMadmate;
    }

    // write configs here
    private static CustomButton _suicideButton;

    internal static bool CanEnterVents
    {
        get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool();
    }

    internal static bool CanSabotage
    {
        get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool();
    }

    internal static bool CanFixComm
    {
        get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool();
    }

    internal static CreatedMadmateType MadmateType
    {
        get => CreatedMadmateType.Simple;
    }

    internal static CreatedMadmateAbility MadmateAbility
    {
        get => (CreatedMadmateAbility)CustomOptionHolder.CreatedMadmateAbility.GetSelection();
    }

    internal static int NumTasks
    {
        get => (int)CustomOptionHolder.CreatedMadmateNumTasks.GetFloat();
    }

    internal static bool HasTasks
    {
        get => MadmateAbility == CreatedMadmateAbility.Fanatic;
    }

    internal static bool ExileCrewmate
    {
        get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool();
    }

    internal override void OnUpdateRoleColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            HudManagerPatch.SetPlayerNameColor(Player, ModifierColor);

            if (KnowsImpostors(Player))
            {
                var allPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator();
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
        }
    }

    [CustomEvent(CustomEventType.OnDeath)]
    internal void OnDeath(PlayerControl killer)
    {
        Player.ClearAllTasks();
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _suicideButton = new(() =>
            {
                RPCProcedure.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, 1);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.HasModifier(ModifierType.CreatedMadmate) && MadmateAbility == CreatedMadmateAbility.Suicider && PlayerControl.LocalPlayer?.Data?.IsDead == false;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _suicideButton.Timer = _suicideButton.MaxTimer;
            },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            0f,
            null,
            false,
            Tr.Get(TrKey.Suicide));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        if (_suicideButton != null) _suicideButton.MaxTimer = 0f;
    }

    internal static bool KnowsImpostors(PlayerControl player)
    {
        return HasTasks && HasModifier(player) && TasksComplete(player);
    }

    internal static bool TasksComplete(PlayerControl player)
    {
        if (!HasTasks)
        {
            return false;
        }

        var counter = 0;
        var totalTasks = NumTasks;
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

        return counter >= totalTasks;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }

    internal enum CreatedMadmateType
    {
        Simple = 0,
        WithRole = 1,
        Random = 2,
    }

    internal enum CreatedMadmateAbility
    {
        None = 0,
        Fanatic = 1,
        Suicider = 2,
    }
}