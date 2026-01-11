using AmongUs.GameOptions;

namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jester : RoleBase<Jester>
{
    public static Color RoleColor = new Color32(236, 98, 165, byte.MaxValue);

    public static bool triggerJesterWin = false;
    public static bool canCallEmergency { get { return CustomOptionHolder.jesterCanCallEmergency.GetBool(); } }
    public static bool canSabotage { get { return CustomOptionHolder.jesterCanSabotage.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.jesterHasImpostorVision.GetBool(); } }

    public Jester()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Jester;
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
        triggerJesterWin = false;
        Players.Clear();
    }
}