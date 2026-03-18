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

            if (Madmate.KnowsImpostors(Player))
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
    }
}