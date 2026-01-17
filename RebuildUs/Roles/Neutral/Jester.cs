using AmongUs.GameOptions;

namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jester : RoleBase<Jester>
{
    public static Color RoleColor = new Color32(236, 98, 165, byte.MaxValue);

    public static bool TriggerJesterWin = false;
    public static bool CanCallEmergency { get { return CustomOptionHolder.JesterCanCallEmergency.GetBool(); } }
    public static bool CanSabotage { get { return CustomOptionHolder.JesterCanSabotage.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.JesterHasImpostorVision.GetBool(); } }

    public Jester()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Jester;
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
        TriggerJesterWin = false;
        Players.Clear();
    }
}