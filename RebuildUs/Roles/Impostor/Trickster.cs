namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Trickster : RoleBase<Trickster>
{
    public static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _placeJackInTheBoxButton;
    private static CustomButton _lightsOutButton;
    public static float LightsOutTimer;

    public Trickster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Trickster;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float PlaceBoxCooldown
    {
        get => CustomOptionHolder.TricksterPlaceBoxCooldown.GetFloat();
    }

    public static float LightsOutCooldown
    {
        get => CustomOptionHolder.TricksterLightsOutCooldown.GetFloat();
    }

    public static float LightsOutDuration
    {
        get => CustomOptionHolder.TricksterLightsOutDuration.GetFloat();
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
        _placeJackInTheBoxButton = new(() =>
        {
            _placeJackInTheBoxButton.Timer = _placeJackInTheBoxButton.MaxTimer;

            var pos = PlayerControl.LocalPlayer.transform.position;
            var buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceJackInTheBox);
            sender.WriteBytesAndSize(buff);
            RPCProcedure.PlaceJackInTheBox(buff);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster) && PlayerControl.LocalPlayer.IsAlive() && !JackInTheBox.HasJackInTheBoxLimitReached(); }, () => { return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached(); }, () => { _placeJackInTheBoxButton.Timer = _placeJackInTheBoxButton.MaxTimer; }, AssetLoader.PlaceJackInTheBoxButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.PlaceJackInTheBoxText));

        _lightsOutButton = new(() =>
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.LightsOut);
            RPCProcedure.LightsOut();
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster) && PlayerControl.LocalPlayer.IsAlive() && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; }, () => { return PlayerControl.LocalPlayer.CanMove && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; }, () =>
        {
            _lightsOutButton.Timer = _lightsOutButton.MaxTimer;
            _lightsOutButton.IsEffectActive = false;
            _lightsOutButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.LightsOutButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, true, LightsOutDuration, () => { _lightsOutButton.Timer = _lightsOutButton.MaxTimer; }, false, Tr.Get(TrKey.LightsOutText));
    }

    public static void SetButtonCooldowns()
    {
        _placeJackInTheBoxButton.MaxTimer = PlaceBoxCooldown;
        _lightsOutButton.MaxTimer = LightsOutCooldown;
        _lightsOutButton.EffectDuration = LightsOutDuration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        LightsOutTimer = 0f;
        Players.Clear();
    }
}
