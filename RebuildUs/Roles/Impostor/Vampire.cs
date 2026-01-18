namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Vampire : RoleBase<Vampire>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton VampireKillButton;
    private static CustomButton GarlicButton;

    // write configs here
    public static float Delay { get { return CustomOptionHolder.VampireKillDelay.GetFloat(); } }
    public static float Cooldown { get { return CustomOptionHolder.VampireCooldown.GetFloat(); } }
    public static bool CanKillNearGarlics { get { return CustomOptionHolder.VampireCanKillNearGarlics.GetBool(); } }

    public static PlayerControl CurrentTarget;
    public static PlayerControl Bitten;
    public static bool TargetNearGarlic = false;
    public static bool LocalPlacedGarlic = false;
    public static bool GarlicsActive = true;

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
        VampireKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murder = Helpers.CheckMurderAttempt(Player, Vampire.CurrentTarget);
                    if (murder == MurderAttemptResult.PerformKill)
                    {
                        if (Vampire.TargetNearGarlic)
                        {
                            {
                                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UncheckedMurderPlayer);
                                sender.Write(Player.PlayerId);
                                sender.Write(Vampire.CurrentTarget.PlayerId);
                                sender.Write(Byte.MaxValue);
                            }
                            RPCProcedure.UncheckedMurderPlayer(Player.PlayerId, Vampire.CurrentTarget.PlayerId, Byte.MaxValue);

                            VampireKillButton.HasEffect = false; // Block effect on this click
                            VampireKillButton.Timer = VampireKillButton.MaxTimer;
                        }
                        else
                        {
                            Vampire.Bitten = Vampire.CurrentTarget;
                            // Notify players about bitten
                            {
                                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.VampireSetBitten);
                                sender.Write(Vampire.Bitten.PlayerId);
                                sender.Write((byte)0);
                            }
                            RPCProcedure.VampireSetBitten(Vampire.Bitten.PlayerId, 0);

                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.Delay, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.CheckMurderAttemptAndKill(Player, Vampire.Bitten, showAnimation: false);
                                    {
                                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.VampireSetBitten);
                                        sender.Write(byte.MaxValue);
                                        sender.Write(byte.MaxValue);
                                    }
                                    RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
                                }
                            })));

                            VampireKillButton.HasEffect = true; // Trigger effect on this click
                        }
                    }
                    else if (murder == MurderAttemptResult.BlankKill)
                    {
                        VampireKillButton.Timer = VampireKillButton.MaxTimer;
                        VampireKillButton.HasEffect = false;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vampire) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (Vampire.TargetNearGarlic && Vampire.CanKillNearGarlics)
                    {
                        VampireKillButton.Sprite = hm.KillButton.graphic.sprite;
                        VampireKillButton.ButtonText = TranslationController.Instance.GetString(StringNames.KillLabel);
                    }
                    else
                    {
                        VampireKillButton.Sprite = AssetLoader.VampireButton;
                        VampireKillButton.ButtonText = Tr.Get("Hud.VampireText");
                    }
                    return Vampire.CurrentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove && (!Vampire.TargetNearGarlic || Vampire.CanKillNearGarlics);
                },
                () =>
                {
                    VampireKillButton.Timer = VampireKillButton.MaxTimer;
                    VampireKillButton.IsEffectActive = false;
                    VampireKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
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
                    VampireKillButton.Timer = VampireKillButton.MaxTimer;
                }
            )
        {
            ButtonText = Tr.Get("Hud.VampireText")
        };

        GarlicButton = new CustomButton(
            () =>
            {
                Vampire.LocalPlacedGarlic = true;
                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                {
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PlaceGarlic);
                    sender.WriteBytesAndSize(buff);
                }
                RPCProcedure.PlaceGarlic(buff);
            },
            () => { return !Vampire.LocalPlacedGarlic && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && Vampire.GarlicsActive && !CachedPlayer.LocalPlayer.PlayerControl.IsGM(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Vampire.LocalPlacedGarlic; },
            () => { },
            AssetLoader.GarlicButton,
            new Vector3(0, -0.06f, 0),
            hm,
            hm.UseButton,
            null,
            true
        )
        {
            ButtonText = Tr.Get("Hud.GarlicText")
        };
    }
    public override void SetButtonCooldowns()
    {
        VampireKillButton.MaxTimer = Cooldown;
        VampireKillButton.EffectDuration = Delay;
        GarlicButton.MaxTimer = 0f;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        Bitten = null;
        TargetNearGarlic = false;
        LocalPlacedGarlic = false;
        CurrentTarget = null;
    }
}