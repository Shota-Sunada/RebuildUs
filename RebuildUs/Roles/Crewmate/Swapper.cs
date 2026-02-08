namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Swapper : RoleBase<Swapper>
{
    public static int RemainSwaps = 2;

    public static byte PlayerId1 = byte.MaxValue;
    public static byte PlayerId2 = byte.MaxValue;

    public Swapper()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.NiceSwapper;
        RemainSwaps = NumSwaps;
    }

    public static Color NameColor
    {
        get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? Palette.ImpostorRed : new Color32(134, 55, 86, byte.MaxValue);
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static int NumSwaps
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SwapperNumSwaps.GetFloat());
    }

    public static bool CanCallEmergency
    {
        get => CustomOptionHolder.SwapperCanCallEmergency.GetBool();
    }

    public static bool CanOnlySwapOthers
    {
        get => CustomOptionHolder.SwapperCanOnlySwapOthers.GetBool();
    }

    public override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer) Update.SetPlayerNameColor(Player, NameColor);
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
        Players.Clear();
        PlayerId1 = byte.MaxValue;
        PlayerId2 = byte.MaxValue;
        RemainSwaps = 2;
    }
}
