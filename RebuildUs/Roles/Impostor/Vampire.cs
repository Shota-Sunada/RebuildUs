namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Vampire : RoleBase<Vampire>
{
    public static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _vampireKillButton;
    private static CustomButton _garlicButton;
    public static bool PlayerPlacedGarlic;

    // States
    public static PlayerControl Bitten;

    public PlayerControl CurrentTarget;
    public bool TargetNearGarlic;

    public Vampire()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vampire;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Delay
    {
        get => CustomOptionHolder.VampireKillDelay.GetFloat();
    }

    public static float Cooldown
    {
        get => CustomOptionHolder.VampireCooldown.GetFloat();
    }

    public static bool CanKillNearGarlics
    {
        get => CustomOptionHolder.VampireCanKillNearGarlics.GetBool();
    }

    public static bool GarlicsActive
    {
        get => CustomOptionHolder.VampireSpawnRate.GetSelection() > 0;
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
                if (garlic?.GarlicObject != null && Vector2.Distance(targetPos, garlic.GarlicObject.transform.position) <= 2.5f)
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
        _vampireKillButton = new(() =>
        {
            if (Local == null) return;

            var murder = Helpers.CheckMurderAttempt(Local.Player, Local.CurrentTarget);
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

                    _vampireKillButton.HasEffect = false; // Block effect on this click
                    _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
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

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Delay, new Action<float>(p =>
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

                    _vampireKillButton.HasEffect = true; // Trigger effect on this click
                }
            }
            else if (murder == MurderAttemptResult.BlankKill)
            {
                _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
                _vampireKillButton.HasEffect = false;
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Vampire) && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            var local = Local;
            if (local == null) return false;

            if (local.TargetNearGarlic && CanKillNearGarlics)
            {
                _vampireKillButton.Sprite = hm.KillButton.graphic.sprite;
                _vampireKillButton.ButtonText = TranslationController.Instance.GetString(StringNames.KillLabel);
            }
            else
            {
                _vampireKillButton.Sprite = AssetLoader.VampireButton;
                _vampireKillButton.ButtonText = Tr.Get(TrKey.VampireText);
            }

            return local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && (!local.TargetNearGarlic || CanKillNearGarlics);
        }, () =>
        {
            _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
            _vampireKillButton.IsEffectActive = false;
            _vampireKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.VampireButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, 0f, () => { _vampireKillButton.Timer = _vampireKillButton.MaxTimer; }, false, Tr.Get(TrKey.VampireText));

        _garlicButton = new(() =>
        {
            PlayerPlacedGarlic = true;
            var pos = PlayerControl.LocalPlayer.transform.position;

            using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceGarlic))
            {
                sender.Write(pos.x);
                sender.Write(pos.y);
            }

            RPCProcedure.PlaceGarlic(pos.x, pos.y);
        }, () => { return !PlayerPlacedGarlic && PlayerControl.LocalPlayer.IsAlive() && GarlicsActive && !PlayerControl.LocalPlayer.IsGm(); }, () => { return PlayerControl.LocalPlayer.CanMove && !PlayerPlacedGarlic; }, () => { }, AssetLoader.GarlicButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CommonAbilityPrimary, true, Tr.Get(TrKey.GarlicText));
    }

    public static void SetButtonCooldowns()
    {
        if (_vampireKillButton != null)
        {
            _vampireKillButton.MaxTimer = Cooldown;
            _vampireKillButton.EffectDuration = Delay;
        }

        _garlicButton?.MaxTimer = 0f;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Bitten = null;
        Players.Clear();
    }
}
