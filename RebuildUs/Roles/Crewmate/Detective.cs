namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Detective : RoleBase<Detective>
{
    public static Color NameColor = new Color32(45, 106, 165, byte.MaxValue);
    public float Timer = 6.2f;

    public Detective()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Detective;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static bool AnonymousFootprints
    {
        get => CustomOptionHolder.DetectiveAnonymousFootprints.GetBool();
    }

    public static float FootprintInterval
    {
        get => CustomOptionHolder.DetectiveFootprintInterval.GetFloat();
    }

    public static float FootprintDuration
    {
        get => CustomOptionHolder.DetectiveFootprintDuration.GetFloat();
    }

    public static float ReportNameDuration
    {
        get => CustomOptionHolder.DetectiveReportNameDuration.GetFloat();
    }

    public static float ReportColorDuration
    {
        get => CustomOptionHolder.DetectiveReportColorDuration.GetFloat();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        if (!Exists || !PlayerControl.LocalPlayer.IsRole(RoleType.Detective)) return;

        Timer -= Time.fixedDeltaTime;
        if (Timer <= 0f)
        {
            Timer = FootprintInterval;
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !player.IsGm())
                    FootprintHolder.Instance.MakeFootprint(player);
            }
        }
    }

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
