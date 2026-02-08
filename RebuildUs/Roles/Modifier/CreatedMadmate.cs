namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class CreatedMadmate : ModifierBase<CreatedMadmate>
{
    public enum CreatedMadmateAbility
    {
        None = 0, Fanatic = 1,
    }

    public enum CreatedMadmateType
    {
        Simple = 0, WithRole = 1, Random = 2,
    }

    public CreatedMadmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.CreatedMadmate;
    }

    public override Color ModifierColor
    {
        get => Madmate.NameColor;
    }

    // write configs here

    public static bool CanEnterVents
    {
        get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool();
    }

    public static bool HasImpostorVision
    {
        get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool();
    }

    public static bool CanSabotage
    {
        get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool();
    }

    public static bool CanFixComm
    {
        get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool();
    }

    public static CreatedMadmateType MadmateType
    {
        get => CreatedMadmateType.Simple;
    }

    public static CreatedMadmateAbility MadmateAbility
    {
        get => (CreatedMadmateAbility)CustomOptionHolder.CreatedMadmateAbility.GetSelection();
    }

    public static int NumTasks
    {
        get => (int)CustomOptionHolder.CreatedMadmateNumTasks.GetFloat();
    }

    public static bool HasTasks
    {
        get => MadmateAbility == CreatedMadmateAbility.Fanatic;
    }

    public static bool ExileCrewmate
    {
        get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool();
    }

    public override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            Update.SetPlayerNameColor(Player, ModifierColor);

            if (Madmate.KnowsImpostors(Player))
            {
                var allPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator();
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                        Update.SetPlayerNameColor(p, Palette.ImpostorRed);
                }
            }
        }
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }

    public override void OnDeath(PlayerControl killer = null)
    {
        Player.ClearAllTasks();
    }

    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
