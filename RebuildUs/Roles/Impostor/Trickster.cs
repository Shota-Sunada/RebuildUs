namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Trickster : SingleRoleBase<Trickster>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _placeJackInTheBoxButton;
    private static CustomButton _lightsOutButton;
    internal static float LightsOutTimer;

    public Trickster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Trickster;
    }

    internal override Color RoleColor
    {
        get => NameColor;
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
        _placeJackInTheBoxButton = new(() =>
            {
                _placeJackInTheBoxButton.Timer = _placeJackInTheBoxButton.MaxTimer;

                Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceJackInTheBox);
                sender.WriteBytesAndSize(buff);
                RPCProcedure.PlaceJackInTheBox(buff);
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
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.LightsOut);
                RPCProcedure.LightsOut();
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
}