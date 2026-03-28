namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Morphing, RoleTeam.Impostor, typeof(MultiRoleBase<Morphing>), nameof(CustomOptionHolder.MorphingSpawnRate))]
internal class Morphing : MultiRoleBase<Morphing>
{
    public static Color Color = Palette.ImpostorRed;

    private static CustomButton MorphingButton;
    private static PlayerControl _currentTarget;
    private static PlayerControl _sampledTarget;
    internal static PlayerControl MorphTarget;
    internal static float MorphTimer;
    private static readonly Vector3 IconOffset = new(0f, 0.2f, -10f);

    public Morphing()
    {
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
    }

    private static float Cooldown { get => CustomOptionHolder.MorphingCooldown.GetFloat(); }
    private static float Duration { get => CustomOptionHolder.MorphingDuration.GetFloat(); }

    [CustomEvent(CustomEventType.OnIntroEnd)]
    internal void OnIntroEnd()
    {
        ResetMorph();
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (Local == null)
        {
            return;
        }

        if (Player.IsDead() && _sampledTarget != null)
        {
            if (MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var deadIcon) && deadIcon != null && deadIcon.gameObject != null)
            {
                deadIcon.gameObject.SetActive(false);
            }
            _sampledTarget = null;
        }

        _currentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(_currentTarget, RoleColor);

        // Update Sampled Player Icon
        if (FastDestroyableSingleton<HudManager>.Instance != null && MorphingButton != null && MorphingButton.ActionButton != null)
        {
            if (_sampledTarget != null && !MeetingHud.Instance)
            {
                if (MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var icon) && icon != null && icon.gameObject != null)
                {
                    icon.transform.position = MorphingButton.ActionButton.transform.position + IconOffset; // Slight offset
                    icon.transform.localScale = Vector3.one * 0.35f;
                    icon.gameObject.SetActive(true);
                }
            }
            else
            {
                if (_sampledTarget != null && MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var icon) && icon != null && icon.gameObject != null)
                {
                    icon.gameObject.SetActive(false);
                }
            }
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        MorphingButton = new(
            nameof(MorphingButton),
            () =>
            {
                if (_sampledTarget != null)
                {
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.MorphingMorph);
                        sender.Write(_sampledTarget.PlayerId);
                        sender.Write(Local.Player.PlayerId);
                    }
                    RPCProcedure.MorphingMorph(_sampledTarget.PlayerId, Local.Player.PlayerId);
                    if (MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var icon) && icon != null && icon.gameObject != null)
                    {
                        icon.gameObject.SetActive(false);
                    }
                    _sampledTarget = null;
                    MorphingButton.EffectDuration = Duration;
                }
                else if (_currentTarget != null)
                {
                    _sampledTarget = _currentTarget;
                    MorphingButton.Sprite = AssetLoader.MorphButton;
                    MorphingButton.ButtonText = Tr.Get(TrKey.MorphText);
                    MorphingButton.EffectDuration = 1f;
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Morphing) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return (_currentTarget || _sampledTarget) && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (_sampledTarget != null && MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var icon) && icon != null && icon.gameObject != null)
                {
                    icon.gameObject.SetActive(false);
                }
                MorphingButton.Timer = MorphingButton.MaxTimer;
                MorphingButton.Sprite = AssetLoader.SampleButton;
                MorphingButton.ButtonText = Tr.Get(TrKey.SampleText);
                MorphingButton.IsEffectActive = false;
                MorphingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                _sampledTarget = null;
            },
            AssetLoader.SampleButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            true,
            Duration,
            () =>
            {
                if (_sampledTarget == null)
                {
                    MorphingButton.Timer = MorphingButton.MaxTimer;
                    MorphingButton.Sprite = AssetLoader.SampleButton;
                    MorphingButton.ButtonText = Tr.Get(TrKey.SampleText);
                }
            },
            false,
            Tr.Get(TrKey.SampleText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        MorphingButton.MaxTimer = Cooldown;
        MorphingButton.EffectDuration = Duration;
    }

    internal void HandleMorphing()
    {
        // first, if camouflager is active, don't do anything
        if (Camouflager.Exists && Camouflager.CamouflageTimer > 0f)
        {
            return;
        }

        // next, if we're currently morphed, set our skin to the target
        if (MorphTimer > 0f && MorphTarget != null)
        {
            Player.MorphToPlayer(MorphTarget);
        }
        else
        {
            Player.ResetMorph();
        }
    }

    internal void StartMorph(PlayerControl target)
    {
        MorphTarget = target;
        MorphTimer = Duration;
        HandleMorphing();
    }

    internal static void ResetMorph()
    {
        MorphTarget = null;
        MorphTimer = 0f;
        foreach (var t in Players)
        {
            t.HandleMorphing();
        }
    }

    internal static void Clear()
    {
        Players.Clear();
        _currentTarget = null;
        if (_sampledTarget != null && MapSettings.PlayerIcons.TryGetValue(_sampledTarget.PlayerId, out var icon) && icon != null && icon.gameObject != null)
        {
            icon.gameObject.SetActive(false);
        }
        _sampledTarget = null;
        MorphTarget = null;
        MorphTimer = 0;
    }
}