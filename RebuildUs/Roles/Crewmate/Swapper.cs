namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Swapper : RoleBase<Swapper>
{
    internal static int RemainSwaps = 2;

    internal static byte PlayerId1 = byte.MaxValue;
    internal static byte PlayerId2 = byte.MaxValue;

    public Swapper()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.NiceSwapper;
        RemainSwaps = NumSwaps;
    }

    internal static Color NameColor
    {
        get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? Palette.ImpostorRed : new Color32(134, 55, 86, byte.MaxValue);
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static int NumSwaps { get => Mathf.RoundToInt(CustomOptionHolder.SwapperNumSwaps.GetFloat()); }
    internal static bool CanCallEmergency { get => CustomOptionHolder.SwapperCanCallEmergency.GetBool(); }
    internal static bool CanOnlySwapOthers { get => CustomOptionHolder.SwapperCanOnlySwapOthers.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer) Update.SetPlayerNameColor(Player, NameColor);
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        PlayerId1 = byte.MaxValue;
        PlayerId2 = byte.MaxValue;
        RemainSwaps = 2;
    }
}