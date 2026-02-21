namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Spy : SingleRoleBase<Spy>
{
    internal static Color NameColor = Palette.ImpostorRed;

    public Spy()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Spy;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static bool ImpostorsCanKillAnyone { get => CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool(); }
    internal static bool CanEnterVents { get => CustomOptionHolder.SpyCanEnterVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SpyHasImpostorVision.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && localPlayer.IsTeamImpostor()) HudManagerPatch.SetPlayerNameColor(Player, NameColor);
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
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}