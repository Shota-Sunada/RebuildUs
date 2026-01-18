namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Spy : RoleBase<Spy>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;

    // write configs here
    public static bool ImpostorsCanKillAnyone { get { return CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool(); } }
    public static bool CanEnterVents { get { return CustomOptionHolder.SpyCanEnterVents.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.SpyHasImpostorVision.GetBool(); } }

    public Spy()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Spy;
    }

    public override void OnUpdateNameColors()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor())
        {
            Update.setPlayerNameColor(Player, NameColor);
        }
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
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
