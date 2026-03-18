namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Trickster, RoleTeam.Impostor, typeof(SingleRoleBase<Trickster>), nameof(CustomOptionHolder.TricksterSpawnRate))]
internal class Trickster : SingleRoleBase<Trickster>
{
    internal static new Color RoleColor = Palette.ImpostorRed;

    private static CustomButton _placeJackInTheBoxButton;
    private static CustomButton _lightsOutButton;
    internal static float LightsOutTimer;

    public Trickster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Trickster;
    }

    // write configs here
    private static float PlaceBoxCooldown
    {
        get => CustomOptionHolder.TricksterPlaceBoxCooldown.GetFloat();
    }

    private static float LightsOutCooldown
    {
        get => CustomOptionHolder.TricksterLightsOutCooldown.GetFloat();
    }

    internal static float LightsOutDuration
    {
        get => CustomOptionHolder.TricksterLightsOutDuration.GetFloat();
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _placeJackInTheBoxButton = new(() =>
            {
                _placeJackInTheBoxButton.Timer = _placeJackInTheBoxButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                PlaceJackInTheBox(PlayerControl.LocalPlayer, pos.x, pos.y);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster)
                       && PlayerControl.LocalPlayer.IsAlive()
                       && !JackInTheBox.HasJackInTheBoxLimitReached();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached();
            },
            () =>
            {
                _placeJackInTheBoxButton.Timer = _placeJackInTheBoxButton.MaxTimer;
            },
            AssetLoader.PlaceJackInTheBoxButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.PlaceJackInTheBoxText));

        _lightsOutButton = new(() =>
            {
                LightsOut(PlayerControl.LocalPlayer);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster)
                       && PlayerControl.LocalPlayer.IsAlive()
                       && JackInTheBox.HasJackInTheBoxLimitReached()
                       && JackInTheBox.BoxesConvertedToVents;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents;
            },
            () =>
            {
                _lightsOutButton.Timer = _lightsOutButton.MaxTimer;
                _lightsOutButton.IsEffectActive = false;
                _lightsOutButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            true,
            LightsOutDuration,
            () =>
            {
                _lightsOutButton.Timer = _lightsOutButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.LightsOutText));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _placeJackInTheBoxButton.MaxTimer = PlaceBoxCooldown;
        _lightsOutButton.MaxTimer = LightsOutCooldown;
        _lightsOutButton.EffectDuration = LightsOutDuration;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        LightsOutTimer = 0f;

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }

    [MethodRpc((uint)CustomRPC.PlaceJackInTheBox)]
    internal static void PlaceJackInTheBox(PlayerControl sender, float x, float y)
    {
        _ = new JackInTheBox(new Vector3(x, y));
    }

    [MethodRpc((uint)CustomRPC.LightsOut)]
    internal static void LightsOut(PlayerControl sender)
    {
        LightsOutTimer = LightsOutDuration;
        // If the local player is impostor indicate lights out
        if (PlayerControl.LocalPlayer.HasImpostorVision())
        {
            _ = new CustomMessage("TricksterLightsOutText", LightsOutDuration);
        }
    }
}