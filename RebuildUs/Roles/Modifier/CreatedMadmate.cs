namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class CreatedMadmate : ModifierBase<CreatedMadmate>
{
    public override Color ModifierColor => Madmate.NameColor;

    public override void OnUpdateNameColors()
    {
        if (Player == CachedPlayer.LocalPlayer.PlayerControl)
        {
            Update.setPlayerNameColor(Player, ModifierColor);

            if (Madmate.knowsImpostors(Player))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                    {
                        Update.setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }
    }

    public enum CreatedMadmateType
    {
        Simple = 0,
        WithRole = 1,
        Random = 2,
    }

    public enum CreatedMadmateAbility
    {
        None = 0,
        Fanatic = 1,
    }

    // write configs here

    public static bool canEnterVents { get { return CustomOptionHolder.createdMadmateCanEnterVents.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.createdMadmateHasImpostorVision.GetBool(); } }
    public static bool canSabotage { get { return CustomOptionHolder.createdMadmateCanSabotage.GetBool(); } }
    public static bool canFixComm { get { return CustomOptionHolder.createdMadmateCanFixComm.GetBool(); } }

    public static CreatedMadmateType madmateType { get { return CreatedMadmateType.Simple; } }
    public static CreatedMadmateAbility madmateAbility { get { return (CreatedMadmateAbility)CustomOptionHolder.createdMadmateAbility.GetSelection(); } }

    public static int numTasks { get { return (int)CustomOptionHolder.createdMadmateNumTasks.GetFloat(); } }

    public static bool hasTasks { get { return madmateAbility == CreatedMadmateAbility.Fanatic; } }
    public static bool exileCrewmate { get { return CustomOptionHolder.createdMadmateExileCrewmate.GetBool(); } }

    public CreatedMadmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.CreatedMadmate;
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
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
