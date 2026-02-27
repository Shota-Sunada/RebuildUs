namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Lighter : MultiRoleBase<Lighter>
{
    internal static Color NameColor = new Color32(238, 229, 190, byte.MaxValue);

    private static CustomButton _lighterButton;
    private bool _lightActive;

    public Lighter()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Lighter;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float ModeLightsOnVision
    {
        get => CustomOptionHolder.LighterModeLightsOnVision.GetFloat();
    }

    internal static float ModeLightsOffVision
    {
        get => CustomOptionHolder.LighterModeLightsOffVision.GetFloat();
    }

    private static float Cooldown
    {
        get => CustomOptionHolder.LighterCooldown.GetFloat();
    }

    private static float Duration
    {
        get => CustomOptionHolder.LighterDuration.GetFloat();
    }

    internal static bool IsLightActive(PlayerControl player)
    {
        if (!IsRole(player) || !player.IsAlive())
        {
            return false;
        }
        Lighter r = GetRole(player);
        return r._lightActive;
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        // Lighter light
        _lighterButton = new(() =>
            {
                Lighter local = Local;
                local?._lightActive = true;
            },
            () => Local != null && PlayerControl.LocalPlayer?.Data?.IsDead == false,
            () => PlayerControl.LocalPlayer.CanMove,
            () =>
            {
                Lighter local = Local;
                local?._lightActive = false;
                _lighterButton.Timer = _lighterButton.MaxTimer;
                _lighterButton.IsEffectActive = false;
                _lighterButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LighterButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            true,
            Duration,
            () =>
            {
                Lighter local = Local;
                local?._lightActive = false;
                _lighterButton.Timer = _lighterButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.LighterText));
    }

    internal static void SetButtonCooldowns()
    {
        _lighterButton.MaxTimer = Cooldown;
        _lighterButton.EffectDuration = Duration;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}