namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Vampire : RoleBase<Vampire>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton VampireKillButton;
    private static CustomButton GarlicButton;

    // write configs here
    public static float Delay => CustomOptionHolder.VampireKillDelay.GetFloat();
    public static float Cooldown => CustomOptionHolder.VampireCooldown.GetFloat();
    public static bool CanKillNearGarlics => CustomOptionHolder.VampireCanKillNearGarlics.GetBool();
    public static bool GarlicsActive => CustomOptionHolder.VampireSpawnRate.GetSelection() > 0;

    public PlayerControl CurrentTarget;
    public bool TargetNearGarlic = false;
    public static bool PlayerPlacedGarlic = false;

    // States
    public static PlayerControl Bitten;

    public Vampire()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vampire;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var local = Local;
        if (local == null || Player.IsDead()) return;

        // Update target selection
        local.CurrentTarget = Helpers.SetTarget();

        local.TargetNearGarlic = false;
        if (local.CurrentTarget != null)
        {
            Helpers.SetPlayerOutline(local.CurrentTarget, RoleColor);

            var targetPos = local.CurrentTarget.GetTruePosition();
            var garlics = Garlic.Garlics;
            for (var i = 0; i < garlics.Count; i++)
            {
                var garlic = garlics[i];
                if (garlic?.GarlicObject != null && Vector2.Distance(targetPos, (Vector2)garlic.GarlicObject.transform.position) <= 2.5f)
                {
                    local.TargetNearGarlic = true;
                    break;
                }
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        VampireKillButton = new CustomButton(
            () =>
            {
                if (Local == null) return;

                MurderAttemptResult murder = Helpers.CheckMurderAttempt(Local.Player, Local.CurrentTarget);
                if (murder == MurderAttemptResult.PerformKill)
                {
                    if (Local.TargetNearGarlic)
                    {
                        {
                            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                            sender.Write(Local.Player.PlayerId);
                            sender.Write(Local.CurrentTarget.PlayerId);
                            sender.Write(byte.MaxValue);
                        }
                        RPCProcedure.UncheckedMurderPlayer(Local.Player.PlayerId, Local.CurrentTarget.PlayerId, byte.MaxValue);

                        VampireKillButton.HasEffect = false; // Block effect on this click
                        VampireKillButton.Timer = VampireKillButton.MaxTimer;
                    }
                    else
                    {
                        Bitten = Local.CurrentTarget;
                        // Notify players about bitten
                        {
                            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                            sender.Write(Bitten.PlayerId);
                            sender.Write((byte)0);
                        }
                        RPCProcedure.VampireSetBitten(Bitten.PlayerId, 0);

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Delay, new Action<float>((p) =>
                        {
                            // Delayed action
                            if (p == 1f)
                            {
                                // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                Helpers.CheckMurderAttemptAndKill(Local.Player, Bitten, showAnimation: false);
                                {
                                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
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
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Vampire) && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                var local = Local;
                if (local == null) return false;

                if (local.TargetNearGarlic && CanKillNearGarlics)
                {
                    VampireKillButton.Sprite = hm.KillButton.graphic.sprite;
                    VampireKillButton.ButtonText = TranslationController.Instance.GetString(StringNames.KillLabel);
                }
                else
                {
                    VampireKillButton.Sprite = AssetLoader.VampireButton;
                    VampireKillButton.ButtonText = Tr.Get("VampireText");
                }
                return local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && (!local.TargetNearGarlic || CanKillNearGarlics);
            },
            () =>
            {
                VampireKillButton.Timer = VampireKillButton.MaxTimer;
                VampireKillButton.IsEffectActive = false;
                VampireKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.VampireButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            0f,
            () =>
            {
                VampireKillButton.Timer = VampireKillButton.MaxTimer;
            },
            false,
            Tr.Get("VampireText")
        );

        GarlicButton = new CustomButton(
            () =>
            {
                PlayerPlacedGarlic = true;
                var pos = PlayerControl.LocalPlayer.transform.position;

                using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceGarlic))
                {
                    sender.Write(pos.x);
                    sender.Write(pos.y);
                }
                RPCProcedure.PlaceGarlic(pos.x, pos.y);
            },
            () =>
            {
                return !PlayerPlacedGarlic && PlayerControl.LocalPlayer.IsAlive() && GarlicsActive && !PlayerControl.LocalPlayer.IsGM();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !PlayerPlacedGarlic;
            },
            () => { },
            AssetLoader.GarlicButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            Tr.Get("GarlicText")
        );
    }
    public static void SetButtonCooldowns()
    {
        if (VampireKillButton != null)
        {
            VampireKillButton.MaxTimer = Cooldown;
            VampireKillButton.EffectDuration = Delay;
        }
        GarlicButton?.MaxTimer = 0f;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Bitten = null;
        Players.Clear();
    }
}
