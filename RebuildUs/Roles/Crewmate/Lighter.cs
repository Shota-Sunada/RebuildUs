namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Lighter : RoleBase<Lighter>
{
    public static Color RoleColor = new Color32(238, 229, 190, byte.MaxValue);
    private static CustomButton lighterButton;
    private bool lightActive = false;

    // write configs here
    public static float modeLightsOnVision { get { return CustomOptionHolder.lighterModeLightsOnVision.GetFloat(); } }
    public static float modeLightsOffVision { get { return CustomOptionHolder.lighterModeLightsOffVision.GetFloat(); } }
    public static float cooldown { get { return CustomOptionHolder.lighterCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.lighterDuration.GetFloat(); } }

    public static bool isLightActive(PlayerControl player)
    {
        if (IsRole(player) && player.IsAlive())
        {
            var r = GetRole(player);
            return r.lightActive;
        }
        return false;
    }

    public Lighter()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Lighter;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        // Lighter light
        lighterButton = new CustomButton(
            () =>
            {
                Local.lightActive = true;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Lighter) && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                Local?.lightActive = false;
                lighterButton.Timer = lighterButton.MaxTimer;
                lighterButton.IsEffectActive = false;
                lighterButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LighterButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            duration,
            () =>
            {
                Local.lightActive = false;
                lighterButton.Timer = lighterButton.MaxTimer;
            }
        )
        {
            ButtonText = Tr.Get("LighterText")
        };
    }
    public override void SetButtonCooldowns()
    {
        lighterButton.MaxTimer = cooldown;
        lighterButton.EffectDuration = duration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}