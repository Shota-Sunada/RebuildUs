namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Jester : SingleRoleBase<Jester>
{
    internal static Color NameColor = new Color32(236, 98, 165, byte.MaxValue);

    internal static bool TriggerJesterWin;

    public Jester()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Jester;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static bool CanCallEmergency
    {
        get => CustomOptionHolder.JesterCanCallEmergency.GetBool();
    }

    internal static bool CanSabotage
    {
        get => CustomOptionHolder.JesterCanSabotage.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.JesterHasImpostorVision.GetBool();
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
        TriggerJesterWin = false;

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}