namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Swapper : RoleBase<Swapper>
{
    public static Color RoleColor = new Color32(134, 55, 86, byte.MaxValue);

    // write configs here
    public static int numSwaps { get { return Mathf.RoundToInt(CustomOptionHolder.swapperNumSwaps.GetFloat()); } }
    public static bool canCallEmergency { get { return CustomOptionHolder.swapperCanCallEmergency.GetBool(); } }
    public static bool canOnlySwapOthers { get { return CustomOptionHolder.swapperCanOnlySwapOthers.GetBool(); } }
    public static int remainSwaps = 2;

    public static byte playerId1 = byte.MaxValue;
    public static byte playerId2 = byte.MaxValue;

    public Swapper()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Swapper;
        remainSwaps = numSwaps;
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
        playerId1 = byte.MaxValue;
        playerId2 = byte.MaxValue;
        remainSwaps = 2;
    }
}