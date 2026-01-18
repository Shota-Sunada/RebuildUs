namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Trickster : RoleBase<Trickster>
{
    public static Color RoleColor = Palette.ImpostorRed;
    private static CustomButton placeJackInTheBoxButton;
    private static CustomButton lightsOutButton;

    // write configs here
    public static float placeBoxCooldown { get { return CustomOptionHolder.tricksterPlaceBoxCooldown.GetFloat(); } }
    public static float lightsOutCooldown { get { return CustomOptionHolder.tricksterLightsOutCooldown.GetFloat(); } }
    public static float lightsOutDuration { get { return CustomOptionHolder.tricksterLightsOutDuration.GetFloat(); } }
    public static float lightsOutTimer = 0f;

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

        lightsOutTimer -= Time.deltaTime;
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public override void MakeButtons(HudManager hm)
    {
        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PlaceJackInTheBox);
                sender.WriteBytesAndSize(buff);
                RPCProcedure.placeJackInTheBox(buff);
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
            AssetLoader.PlaceJackInTheBoxButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("PlaceJackInTheBoxText")
        };

        lightsOutButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.LightsOut);
                RPCProcedure.lightsOut();
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents; },
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                lightsOutButton.IsEffectActive = false;
                lightsOutButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LightsOutButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F,
            true,
            lightsOutDuration,
            () => { lightsOutButton.Timer = lightsOutButton.MaxTimer; }
        )
        {
            ButtonText = Tr.Get("LightsOutText")
        };
    }
    public override void SetButtonCooldowns()
    {
        placeJackInTheBoxButton.MaxTimer = placeBoxCooldown;
        lightsOutButton.MaxTimer = lightsOutCooldown;
        lightsOutButton.EffectDuration = lightsOutDuration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        lightsOutTimer = 0f;
        Players.Clear();
    }
}