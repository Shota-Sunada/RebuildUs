namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Lighter : RoleBase<Lighter>
{
    public static Color NameColor = new Color32(238, 229, 190, byte.MaxValue);

    private static CustomButton _lighterButton;
    private bool _lightActive;

    public Lighter()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Lighter;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float ModeLightsOnVision
    {
        get => CustomOptionHolder.LighterModeLightsOnVision.GetFloat();
    }

    public static float ModeLightsOffVision
    {
        get => CustomOptionHolder.LighterModeLightsOffVision.GetFloat();
    }

    public static float Cooldown
    {
        get => CustomOptionHolder.LighterCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.LighterDuration.GetFloat();
    }

    public static bool IsLightActive(PlayerControl player)
    {
        if (IsRole(player) && player.IsAlive())
        {
            var r = GetRole(player);
            return r._lightActive;
        }

        return false;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        // Lighter light
        _lighterButton = new(() =>
        {
            var local = Local;
            local?._lightActive = true;
        }, () => { return Local != null && PlayerControl.LocalPlayer?.Data?.IsDead == false; }, () => { return PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            var local = Local;
            local?._lightActive = false;
            _lighterButton.Timer = _lighterButton.MaxTimer;
            _lighterButton.IsEffectActive = false;
            _lighterButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.LighterButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, Duration, () =>
        {
            var local = Local;
            local?._lightActive = false;
            _lighterButton.Timer = _lighterButton.MaxTimer;
        }, false, Tr.Get(TrKey.LighterText));
    }

    public static void SetButtonCooldowns()
    {
        _lighterButton.MaxTimer = Cooldown;
        _lighterButton.EffectDuration = Duration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
