namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Vampire, RoleTeam.Impostor, typeof(SingleRoleBase<Vampire>), nameof(CustomOptionHolder.VampireSpawnRate))]
internal class Vampire : SingleRoleBase<Vampire>
{
    public static Color Color = Palette.ImpostorRed;

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

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local == null || Player.IsDead())
        {
            return;
        }

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



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _vampireKillButton = new(() =>
            {
                if (Local == null)
                {
                    return;
                }

                var murder = Helpers.CheckMurderAttempt(Local.Player, Local.CurrentTarget);
                if (murder == MurderAttemptResult.PerformKill)
                {
                    if (Local.TargetNearGarlic)
                    {
                        RPCProcedure.UncheckedMurderPlayer(PlayerControl.LocalPlayer, Local.Player.PlayerId, Local.CurrentTarget.PlayerId, byte.MaxValue);

                        _vampireKillButton.HasEffect = false; // Block effect on this click
                        _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
                    }
                    else
                    {
                        Bitten = Local.CurrentTarget;
                        // Notify players about bitten
                        VampireSetBitten(PlayerControl.LocalPlayer, Bitten.PlayerId, 0);

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Delay,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.CheckMurderAttemptAndKill(Local.Player, Bitten, showAnimation: false);
                                    VampireSetBitten(PlayerControl.LocalPlayer, byte.MaxValue, byte.MaxValue);
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
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Vampire) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var local = Local;
                if (local == null)
                {
                    return false;
                }

                if (local.TargetNearGarlic && CanKillNearGarlics)
                {
                    _vampireKillButton.Sprite = hm.KillButton.graphic.sprite;
                    _vampireKillButton.ButtonText = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel);
                }
                else
                {
                    _vampireKillButton.Sprite = AssetLoader.VampireButton;
                    _vampireKillButton.ButtonText = Tr.Get(TrKey.VampireText);
                }

                return local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && (!local.TargetNearGarlic || CanKillNearGarlics);
            },
            () =>
            {
                _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
                _vampireKillButton.IsEffectActive = false;
                _vampireKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
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
                _vampireKillButton.Timer = _vampireKillButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.VampireText));

        _garlicButton = new(() =>
            {
                PlayerPlacedGarlic = true;
                var pos = PlayerControl.LocalPlayer.transform.position;

                PlaceGarlic(PlayerControl.LocalPlayer, pos.x, pos.y);
            },
            () =>
            {
                return !PlayerPlacedGarlic && PlayerControl.LocalPlayer.IsAlive() && GarlicsActive && !PlayerControl.LocalPlayer.IsGm();
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
            Tr.Get(TrKey.GarlicText));
    }

    [RegisterCustomButton]
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

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }

    [MethodRpc((uint)CustomRPC.VampireSetBitten)]
    internal static void VampireSetBitten(PlayerControl sender, byte targetId, byte performReset)
    {
        if (performReset != 0)
        {
            Bitten = null;
            return;
        }

        if (!Exists)
        {
            return;
        }
        var player = Helpers.PlayerById(targetId);
        if (player != null && !player.Data.IsDead)
        {
            Bitten = player;
        }
    }

    [MethodRpc((uint)CustomRPC.PlaceGarlic)]
    internal static void PlaceGarlic(PlayerControl sender, float x, float y)
    {
        _ = new Garlic(new Vector3(x, y));
    }
}