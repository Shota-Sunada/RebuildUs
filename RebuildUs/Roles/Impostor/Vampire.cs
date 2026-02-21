namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Vampire : RoleBase<Vampire>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _vampireKillButton;
    private static CustomButton _garlicButton;
    internal static bool PlayerPlacedGarlic;

    // States
    internal static PlayerControl Bitten;

    internal PlayerControl CurrentTarget;
    internal bool TargetNearGarlic;

    public Vampire()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vampire;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Delay
    {
        get => CustomOptionHolder.VampireKillDelay.GetFloat();
    }

    internal static float Cooldown
    {
        get => CustomOptionHolder.VampireCooldown.GetFloat();
    }

    internal static bool CanKillNearGarlics
    {
        get => CustomOptionHolder.VampireCanKillNearGarlics.GetBool();
    }

    internal static bool GarlicsActive
    {
        get => CustomOptionHolder.VampireSpawnRate.GetSelection() > 0;
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Vampire local = Local;
        if (local == null || Player.IsDead()) return;

        // Update target selection
        local.CurrentTarget = Helpers.SetTarget();

        local.TargetNearGarlic = false;
        if (local.CurrentTarget != null)
        {
            Helpers.SetPlayerOutline(local.CurrentTarget, RoleColor);

            Vector2 targetPos = local.CurrentTarget.GetTruePosition();
            List<Garlic> garlics = Garlic.Garlics;
            for (int i = 0; i < garlics.Count; i++)
            {
                Garlic garlic = garlics[i];
                if (garlic?.GarlicObject != null && Vector2.Distance(targetPos, garlic.GarlicObject.transform.position) <= 2.5f)
                {
                    local.TargetNearGarlic = true;
                    break;
                }
            }
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _vampireKillButton = new(() =>
        {
            if (Local == null) return;

            MurderAttemptResult murder = Helpers.CheckMurderAttempt(Local.Player, Local.CurrentTarget);
            if (murder == MurderAttemptResult.PerformKill)
            {
                if (Local.TargetNearGarlic)
                {
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
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
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
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
                                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
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
            Vampire local = Local;
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
        }, AssetLoader.VampireButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, 0f, () =>
        {
            _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
        }, false, Tr.Get(TrKey.VampireText));

        _garlicButton = new(() =>
        {
            PlayerPlacedGarlic = true;
            Vector3 pos = PlayerControl.LocalPlayer.transform.position;

            using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceGarlic))
            {
                sender.Write(pos.x);
                sender.Write(pos.y);
            }

            RPCProcedure.PlaceGarlic(pos.x, pos.y);
        }, () =>
        {
            return !PlayerPlacedGarlic && PlayerControl.LocalPlayer.IsAlive() && GarlicsActive && !PlayerControl.LocalPlayer.IsGm();
        }, () =>
        {
            return PlayerControl.LocalPlayer.CanMove && !PlayerPlacedGarlic;
        }, () => { }, AssetLoader.GarlicButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CommonAbilityPrimary, true, Tr.Get(TrKey.GarlicText));
    }

    internal static void SetButtonCooldowns()
    {
        if (_vampireKillButton != null)
        {
            _vampireKillButton.MaxTimer = Cooldown;
            _vampireKillButton.EffectDuration = Delay;
        }

        _garlicButton?.MaxTimer = 0f;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Bitten = null;
        Players.Clear();
    }
}