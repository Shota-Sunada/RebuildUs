namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
internal class CreatedMadmate : ModifierBase<CreatedMadmate>
{
    public CreatedMadmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.CreatedMadmate;
    }

    internal override Color ModifierColor
    {
        get => Madmate.NameColor;
    }

    // write configs here

    internal static bool CanEnterVents { get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool(); }
    internal static bool CanSabotage { get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool(); }
    internal static bool CanFixComm { get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool(); }

    internal static CreatedMadmateType MadmateType { get => CreatedMadmateType.Simple; }
    internal static CreatedMadmateAbility MadmateAbility { get => (CreatedMadmateAbility)CustomOptionHolder.CreatedMadmateAbility.GetSelection(); }

    internal static int NumTasks { get => (int)CustomOptionHolder.CreatedMadmateNumTasks.GetFloat(); }

    internal static bool HasTasks { get => MadmateAbility == CreatedMadmateAbility.Fanatic; }
    internal static bool ExileCrewmate { get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            HudManagerPatch.SetPlayerNameColor(Player, ModifierColor);

            if (Madmate.KnowsImpostors(Player))
            {
                IEnumerable<PlayerControl> allPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                        HudManagerPatch.SetPlayerNameColor(p, Palette.ImpostorRed);
                }
            }
        }
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }

    internal override void OnDeath(PlayerControl killer = null)
    {
        Player.ClearAllTasks();
    }

    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

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