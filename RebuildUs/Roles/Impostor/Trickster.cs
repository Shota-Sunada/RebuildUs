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
    public override void OnMeetingEnd()
    {
        if (Exists && JackInTheBox.HasJackInTheBoxLimitReached())
        {
            JackInTheBox.ConvertToVents();
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        
        lightsOutTimer -= Time.deltaTime;
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceJackInTheBox, Hazel.SendOption.Reliable);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeJackInTheBox(buff);
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached(); },
            () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
            Trickster.getPlaceBoxButtonSprite(),
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
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LightsOut, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
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
            Trickster.getLightsOutButtonSprite(),
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
    public static void SetButtonCooldowns()
    {
        placeJackInTheBoxButton.MaxTimer = placeBoxCooldown;
        lightsOutButton.MaxTimer = lightsOutCooldown;
        lightsOutButton.EffectDuration = lightsOutDuration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        lightsOutTimer = 0f;
        Players.Clear();
    }
}