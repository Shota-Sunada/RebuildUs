namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Lighter, RoleTeam.Crewmate, typeof(MultiRoleBase<Lighter>), nameof(CustomOptionHolder.LighterSpawnRate))]
internal class Lighter : MultiRoleBase<Lighter>
{
    public static Color Color = new Color32(238, 229, 190, byte.MaxValue);

    private static CustomButton LighterButton;
    private bool _lightActive;

    public Lighter()
    {
        StaticRoleType = CurrentRoleType = RoleType.Lighter;
    }

    internal static float ModeLightsOnVision { get => CustomOptionHolder.LighterModeLightsOnVision.GetFloat(); }
    internal static float ModeLightsOffVision { get => CustomOptionHolder.LighterModeLightsOffVision.GetFloat(); }
    private static float Cooldown { get => CustomOptionHolder.LighterCooldown.GetFloat(); }
    private static float Duration { get => CustomOptionHolder.LighterDuration.GetFloat(); }

    internal static bool IsLightActive(PlayerControl player)
    {
        if (!IsRole(player) || !player.IsAlive())
        {
            return false;
        }
        var r = GetRole(player);
        return r._lightActive;
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        // Lighter light
        LighterButton = new(
            nameof(LighterButton),
            () =>
            {
                Local?._lightActive = true;
            },
            () => Local != null && PlayerControl.LocalPlayer?.Data?.IsDead == false,
            () => PlayerControl.LocalPlayer.CanMove,
            () =>
            {
                Local?._lightActive = false;
                LighterButton.Timer = LighterButton.MaxTimer;
                LighterButton.IsEffectActive = false;
                LighterButton.ActionButton.graphic.color = Palette.EnabledColor;
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
                var local = Local;
                local?._lightActive = false;
                LighterButton.Timer = LighterButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.LighterText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        LighterButton.MaxTimer = Cooldown;
        LighterButton.EffectDuration = Duration;
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}