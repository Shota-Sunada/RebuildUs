namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Detective, RoleTeam.Crewmate, typeof(MultiRoleBase<Detective>), nameof(CustomOptionHolder.DetectiveSpawnRate))]
internal class Detective : MultiRoleBase<Detective>
{
    public static Color Color = new Color32(45, 106, 165, byte.MaxValue);
    private float _timer = 6.2f;

    public Detective()
    {
        StaticRoleType = CurrentRoleType = RoleType.Detective;
    }

    internal static bool AnonymousFootprints { get => CustomOptionHolder.DetectiveAnonymousFootprints.GetBool(); }
    private static float FootprintInterval { get => CustomOptionHolder.DetectiveFootprintInterval.GetFloat(); }
    internal static float FootprintDuration { get => CustomOptionHolder.DetectiveFootprintDuration.GetFloat(); }
    internal static float ReportNameDuration { get => CustomOptionHolder.DetectiveReportNameDuration.GetFloat(); }
    internal static float ReportColorDuration { get => CustomOptionHolder.DetectiveReportColorDuration.GetFloat(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
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
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !player.IsGm())
            {
                FootprintHolder.Instance.MakeFootprint(player);
            }
        }
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}