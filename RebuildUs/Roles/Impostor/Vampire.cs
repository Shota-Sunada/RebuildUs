namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Vampire, RoleTeam.Impostor, typeof(SingleRoleBase<Vampire>), nameof(CustomOptionHolder.VampireSpawnRate))]
internal class Vampire : SingleRoleBase<Vampire>
{
    public static Color Color = Palette.ImpostorRed;

    private static CustomButton VampireKillButton;
    internal static bool PlayerPlacedGarlic;

    // States
    internal static PlayerControl Bitten;

    internal PlayerControl CurrentTarget;
    internal bool TargetNearGarlic;

    public Vampire()
    {
        StaticRoleType = CurrentRoleType = RoleType.Vampire;
    }

    internal static float Delay { get => CustomOptionHolder.VampireKillDelay.GetFloat(); }
    internal static float Cooldown { get => CustomOptionHolder.VampireCooldown.GetFloat(); }
    internal static bool CanKillNearGarlics { get => CustomOptionHolder.VampireCanKillNearGarlics.GetBool(); }
    internal static bool GarlicsActive { get => CustomOptionHolder.VampireSpawnRate.GetSelection() > 0; }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (Local == null || Player.IsDead())
        {
            return;
        }

        // Update target selection
        Local.CurrentTarget = Helpers.SetTarget();

        Local.TargetNearGarlic = false;
        if (Local.CurrentTarget != null)
        {
            Helpers.SetPlayerOutline(Local.CurrentTarget, base.RoleColor);

            var targetPos = Local.CurrentTarget.GetTruePosition();
            var garlics = Garlic.Garlics;
            for (var i = 0; i < garlics.Count; i++)
            {
                var garlic = garlics[i];
                if (garlic?.GarlicObject != null && Vector2.Distance(targetPos, garlic.GarlicObject.transform.position) <= 2.5f)
                {
                    Local.TargetNearGarlic = true;
                    break;
                }
            }
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        VampireKillButton = new(
            nameof(VampireKillButton),
            () =>
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
                        {
                            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
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
                            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                            sender.Write(Bitten.PlayerId);
                            sender.Write((byte)0);
                        }
                        RPCProcedure.VampireSetBitten(Bitten.PlayerId, 0);

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Delay,
                            new Action<float>(p =>
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

                        VampireKillButton.HasEffect = true; // Trigger effect on this click
                    }
                }
                else if (murder == MurderAttemptResult.BlankKill)
                {
                    VampireKillButton.Timer = VampireKillButton.MaxTimer;
                    VampireKillButton.HasEffect = false;
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Vampire) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                if (Local == null)
                {
                    return false;
                }

                if (Local.TargetNearGarlic && CanKillNearGarlics)
                {
                    VampireKillButton?.Sprite = hm.KillButton.graphic.sprite;
                    VampireKillButton?.ButtonText = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel);
                }
                else
                {
                    VampireKillButton?.Sprite = AssetLoader.VampireButton;
                    VampireKillButton?.ButtonText = Tr.Get(TrKey.VampireText);
                }

                return Local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && (!Local.TargetNearGarlic || CanKillNearGarlics);
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
            Tr.Get(TrKey.VampireText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        VampireKillButton.MaxTimer = Cooldown;
        VampireKillButton.EffectDuration = Delay;
    }

    internal static void Clear()
    {
        Bitten = null;

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}