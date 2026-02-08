namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Spy : RoleBase<Spy>
{
    public static Color NameColor = Palette.ImpostorRed;

    public Spy()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Spy;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static bool ImpostorsCanKillAnyone
    {
        get => CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool();
    }

    public static bool CanEnterVents
    {
        get => CustomOptionHolder.SpyCanEnterVents.GetBool();
    }

    public static bool HasImpostorVision
    {
        get => CustomOptionHolder.SpyHasImpostorVision.GetBool();
    }

    public override void OnUpdateNameColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && localPlayer.IsTeamImpostor()) Update.SetPlayerNameColor(Player, NameColor);
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
    }
}
