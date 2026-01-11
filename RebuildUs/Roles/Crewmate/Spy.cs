namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Spy : RoleBase<Spy>
{
    public static Color RoleColor = Palette.ImpostorRed;

    // write configs here
    public static bool impostorsCanKillAnyone { get { return CustomOptionHolder.spyImpostorsCanKillAnyone.GetBool(); } }
    public static bool canEnterVents { get { return CustomOptionHolder.spyCanEnterVents.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.spyHasImpostorVision.GetBool(); } }

    public Spy()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Spy;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm) { }
    public static void SetButtonCooldowns() { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}