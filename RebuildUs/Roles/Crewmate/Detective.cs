namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Detective : RoleBase<Detective>
{
    public static Color RoleColor = new Color32(45, 106, 165, byte.MaxValue);

    // write configs here
    public static bool anonymousFootprints { get { return CustomOptionHolder.detectiveAnonymousFootprints.GetBool(); } }
    public static float footprintInterval { get { return CustomOptionHolder.detectiveFootprintInterval.GetFloat(); } }
    public static float footprintDuration { get { return CustomOptionHolder.detectiveFootprintDuration.GetFloat(); } }
    public static float reportNameDuration { get { return CustomOptionHolder.detectiveReportNameDuration.GetFloat(); } }
    public static float reportColorDuration { get { return CustomOptionHolder.detectiveReportColorDuration.GetFloat(); } }
    public float timer = 6.2f;

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

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            timer = footprintInterval;
            foreach (var player in CachedPlayer.AllPlayers)
            {
                if (player.PlayerControl != null && player.PlayerControl != CachedPlayer.LocalPlayer.PlayerControl && !player.Data.IsDead && !player.PlayerControl.inVent && !player.PlayerControl.IsGM())
                {
                    new Footprint(footprintDuration, anonymousFootprints, player);
                }
            }
        }
    }
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
        Players.Clear();
    }
}