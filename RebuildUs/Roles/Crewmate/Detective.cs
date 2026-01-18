namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Detective : RoleBase<Detective>
{
    public static Color NameColor = new Color32(45, 106, 165, byte.MaxValue);
    public override Color RoleColor => NameColor;

    // write configs here
    public static bool AnonymousFootprints { get { return CustomOptionHolder.DetectiveAnonymousFootprints.GetBool(); } }
    public static float FootprintInterval { get { return CustomOptionHolder.DetectiveFootprintInterval.GetFloat(); } }
    public static float FootprintDuration { get { return CustomOptionHolder.DetectiveFootprintDuration.GetFloat(); } }
    public static float ReportNameDuration { get { return CustomOptionHolder.DetectiveReportNameDuration.GetFloat(); } }
    public static float ReportColorDuration { get { return CustomOptionHolder.DetectiveReportColorDuration.GetFloat(); } }
    public float Timer = 6.2f;

    public Detective()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Detective;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!Exists || !CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Detective)) return;

        Timer -= Time.fixedDeltaTime;
        if (Timer <= 0f)
        {
            Timer = FootprintInterval;
            foreach (var player in CachedPlayer.AllPlayers)
            {
                if (player.PlayerControl != null && player.PlayerControl != CachedPlayer.LocalPlayer.PlayerControl && !player.Data.IsDead && !player.PlayerControl.inVent && !player.PlayerControl.IsGM())
                {
                    new Footprint(FootprintDuration, AnonymousFootprints, player);
                }
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}