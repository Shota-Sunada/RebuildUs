namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Vampire : RoleBase<Vampire>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton vampireKillButton;
    private static CustomButton garlicButton;

    // write configs here
    public static float delay { get { return CustomOptionHolder.vampireKillDelay.GetFloat(); } }
    public static float cooldown { get { return CustomOptionHolder.vampireCooldown.GetFloat(); } }
    public static bool canKillNearGarlics { get { return CustomOptionHolder.vampireCanKillNearGarlics.GetBool(); } }

    public static PlayerControl currentTarget;
    public static PlayerControl bitten;
    public static bool targetNearGarlic = false;
    public static bool localPlacedGarlic = false;
    public static bool garlicsActive = true;

    public Vampire()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vampire;
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
        vampireKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murder = Helpers.CheckMurderAttempt(Player, Vampire.currentTarget);
                    if (murder == MurderAttemptResult.PerformKill)
                    {
                        if (Vampire.targetNearGarlic)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer.Write(Player.PlayerId);
                            writer.Write(Vampire.currentTarget.PlayerId);
                            writer.Write(Byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.UncheckedMurderPlayer(Player.PlayerId, Vampire.currentTarget.PlayerId, Byte.MaxValue);

                            vampireKillButton.HasEffect = false; // Block effect on this click
                            vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        }
                        else
                        {
                            Vampire.bitten = Vampire.currentTarget;
                            // Notify players about bitten
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                            writer.Write(Vampire.bitten.PlayerId);
                            writer.Write((byte)0);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.vampireSetBitten(Vampire.bitten.PlayerId, 0);

                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.delay, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.CheckMurderAttemptAndKill(Player, Vampire.bitten, showAnimation: false);
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                                    writer.Write(byte.MaxValue);
                                    writer.Write(byte.MaxValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                                }
                            })));

                            vampireKillButton.HasEffect = true; // Trigger effect on this click
                        }
                    }
                    else if (murder == MurderAttemptResult.BlankKill)
                    {
                        vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        vampireKillButton.HasEffect = false;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vampire) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics)
                    {
                        vampireKillButton.Sprite = hm.KillButton.graphic.sprite;
                        vampireKillButton.ButtonText = TranslationController.Instance.GetString(StringNames.KillLabel);
                    }
                    else
                    {
                        vampireKillButton.Sprite = AssetLoader.VampireButton;
                        vampireKillButton.ButtonText = Tr.Get("VampireText");
                    }
                    return Vampire.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove && (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
                },
                () =>
                {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    vampireKillButton.IsEffectActive = false;
                    vampireKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                AssetLoader.VampireButton,
                new Vector3(0, 1f, 0),
                hm,
                hm.KillButton,
                KeyCode.Q,
                false,
                0f,
                () =>
                {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                }
            )
        {
            ButtonText = Tr.Get("VampireText")
        };

        garlicButton = new CustomButton(
            () =>
            {
                Vampire.localPlacedGarlic = true;
                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceGarlic, Hazel.SendOption.Reliable);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeGarlic(buff);
            },
            () => { return !Vampire.localPlacedGarlic && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && Vampire.garlicsActive && !CachedPlayer.LocalPlayer.PlayerControl.IsGM(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Vampire.localPlacedGarlic; },
            () => { },
            AssetLoader.GarlicButton,
            new Vector3(0, -0.06f, 0),
            hm,
            hm.UseButton,
            null,
            true
        )
        {
            ButtonText = Tr.Get("GarlicText")
        };
    }
    public override void SetButtonCooldowns()
    {
        vampireKillButton.MaxTimer = cooldown;
        vampireKillButton.EffectDuration = delay;
        garlicButton.MaxTimer = 0f;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        bitten = null;
        targetNearGarlic = false;
        localPlacedGarlic = false;
        currentTarget = null;
    }
}
