namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jester : RoleBase<Jester>
{
    public static Color NameColor = new Color32(236, 98, 165, byte.MaxValue);

    public static bool TriggerJesterWin;

    public Jester()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Jester;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    public static bool CanCallEmergency
    {
        get => CustomOptionHolder.JesterCanCallEmergency.GetBool();
    }

    public static bool CanSabotage
    {
        get => CustomOptionHolder.JesterCanSabotage.GetBool();
    }

    public static bool HasImpostorVision
    {
        get => CustomOptionHolder.JesterHasImpostorVision.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        TriggerJesterWin = false;
        Players.Clear();
    }
}
