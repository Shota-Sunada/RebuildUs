namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Trickster : RoleBase<Trickster>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton PlaceJackInTheBoxButton;
    private static CustomButton LightsOutButton;

    // write configs here
    public static float PlaceBoxCooldown { get { return CustomOptionHolder.TricksterPlaceBoxCooldown.GetFloat(); } }
    public static float LightsOutCooldown { get { return CustomOptionHolder.TricksterLightsOutCooldown.GetFloat(); } }
    public static float LightsOutDuration { get { return CustomOptionHolder.TricksterLightsOutDuration.GetFloat(); } }
    public static float LightsOutTimer = 0f;

    public Trickster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Trickster;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {

        LightsOutTimer -= Time.deltaTime;
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public override void MakeButtons(HudManager hm)
    {
        PlaceJackInTheBoxButton = new CustomButton(
            () =>
            {
                PlaceJackInTheBoxButton.Timer = PlaceJackInTheBoxButton.MaxTimer;

                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PlaceJackInTheBox);
                sender.WriteBytesAndSize(buff);
                RPCProcedure.PlaceJackInTheBox(buff);
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { PlaceJackInTheBoxButton.Timer = PlaceJackInTheBoxButton.MaxTimer; },
            AssetLoader.PlaceJackInTheBoxButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("PlaceJackInTheBoxText")
        };

        LightsOutButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.LightsOut);
                RPCProcedure.LightsOut();
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; },
            () =>
            {
                LightsOutButton.Timer = LightsOutButton.MaxTimer;
                LightsOutButton.IsEffectActive = false;
                LightsOutButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LightsOutButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F,
            true,
            LightsOutDuration,
            () => { LightsOutButton.Timer = LightsOutButton.MaxTimer; }
        )
        {
            ButtonText = Tr.Get("LightsOutText")
        };
    }
    public override void SetButtonCooldowns()
    {
        PlaceJackInTheBoxButton.MaxTimer = PlaceBoxCooldown;
        LightsOutButton.MaxTimer = LightsOutCooldown;
        LightsOutButton.EffectDuration = LightsOutDuration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        LightsOutTimer = 0f;
        Players.Clear();
    }
}