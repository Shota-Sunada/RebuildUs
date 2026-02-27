namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Detective : MultiRoleBase<Detective>
{
    internal static Color NameColor = new Color32(45, 106, 165, byte.MaxValue);
    private float _timer = 6.2f;

    public Detective()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Detective;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static bool AnonymousFootprints
    {
        get => CustomOptionHolder.DetectiveAnonymousFootprints.GetBool();
    }

    private static float FootprintInterval
    {
        get => CustomOptionHolder.DetectiveFootprintInterval.GetFloat();
    }

    internal static float FootprintDuration
    {
        get => CustomOptionHolder.DetectiveFootprintDuration.GetFloat();
    }

    internal static float ReportNameDuration
    {
        get => CustomOptionHolder.DetectiveReportNameDuration.GetFloat();
    }

    internal static float ReportColorDuration
    {
        get => CustomOptionHolder.DetectiveReportColorDuration.GetFloat();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (!Exists || !PlayerControl.LocalPlayer.IsRole(RoleType.Detective))
        {
            return;
        }

        _timer -= Time.fixedDeltaTime;
        if (!(_timer <= 0f))
        {
            return;
        }
        _timer = FootprintInterval;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !player.IsGm())
            {
                FootprintHolder.Instance.MakeFootprint(player);
            }
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}