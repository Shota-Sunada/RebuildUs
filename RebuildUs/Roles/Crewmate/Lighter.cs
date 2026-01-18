namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Lighter : RoleBase<Lighter>
{
    public static Color NameColor = new Color32(238, 229, 190, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton LighterButton;
    private bool LightActive = false;

    // write configs here
    public static float ModeLightsOnVision { get { return CustomOptionHolder.LighterModeLightsOnVision.GetFloat(); } }
    public static float ModeLightsOffVision { get { return CustomOptionHolder.LighterModeLightsOffVision.GetFloat(); } }
    public static float Cooldown { get { return CustomOptionHolder.LighterCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.LighterDuration.GetFloat(); } }

    public static bool IsLightActive(PlayerControl player)
    {
        if (IsRole(player) && player.IsAlive())
        {
            var r = GetRole(player);
            return r.LightActive;
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
        LighterButton = new CustomButton(
            () =>
            {
                Local.LightActive = true;
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Lighter) && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                Local?.LightActive = false;
                LighterButton.Timer = LighterButton.MaxTimer;
                LighterButton.IsEffectActive = false;
                LighterButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LighterButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            Duration,
            () =>
            {
                Local.LightActive = false;
                LighterButton.Timer = LighterButton.MaxTimer;
            }
        )
        {
            ButtonText = Tr.Get("Hud.LighterText")
        };
    }
    public override void SetButtonCooldowns()
    {
        LighterButton.MaxTimer = Cooldown;
        LighterButton.EffectDuration = Duration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}