namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Morphing, RoleTeam.Impostor, typeof(MultiRoleBase<Morphing>), nameof(CustomOptionHolder.MorphingSpawnRate))]
internal class Morphing : MultiRoleBase<Morphing>
{
    public static Color Color = Palette.ImpostorRed;

    private static CustomButton _morphingButton;
    private static PlayerControl _currentTarget;
    private static PlayerControl _sampledTarget;
    internal static PlayerControl MorphTarget;
    internal static float MorphTimer;

    public Morphing()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.MorphingCooldown.GetFloat();
    }

    private static float Duration
    {
        get => CustomOptionHolder.MorphingDuration.GetFloat();
    }

    [CustomEvent(CustomEventType.OnIntroEnd)]
    internal void OnIntroEnd()
    {
        ResetMorph();
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local == null)
        {
            return;
        }
        _currentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(_currentTarget, RoleColor);
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _morphingButton = new(() =>
            {
                if (_sampledTarget != null)
                {
                    MorphingMorph(PlayerControl.LocalPlayer, _sampledTarget.PlayerId, Local.Player.PlayerId);
                    _sampledTarget = null;
                    _morphingButton.EffectDuration = Duration;
                }
                else if (_currentTarget != null)
                {
                    _sampledTarget = _currentTarget;
                    _morphingButton.Sprite = AssetLoader.MorphButton;
                    _morphingButton.ButtonText = Tr.Get(TrKey.MorphText);
                    _morphingButton.EffectDuration = 1f;
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
                _morphingButton.Timer = _morphingButton.MaxTimer;
                _morphingButton.Sprite = AssetLoader.SampleButton;
                _morphingButton.ButtonText = Tr.Get(TrKey.SampleText);
                _morphingButton.IsEffectActive = false;
                _morphingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
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
                    _morphingButton.Timer = _morphingButton.MaxTimer;
                    _morphingButton.Sprite = AssetLoader.SampleButton;
                    _morphingButton.ButtonText = Tr.Get(TrKey.SampleText);
                }
            },
            false,
            Tr.Get(TrKey.SampleText));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _morphingButton.MaxTimer = Cooldown;
        _morphingButton.EffectDuration = Duration;
    }

    // write functions here
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
        // reset configs here
        Players.Clear();
        _currentTarget = null;
        _sampledTarget = null;
        MorphTarget = null;
        MorphTimer = 0;
    }

    [MethodRpc((uint)CustomRPC.MorphingMorph)]
    internal static void MorphingMorph(PlayerControl sender, byte playerId, byte morphId)
    {
        var morphPlayer = Helpers.PlayerById(morphId);
        var target = Helpers.PlayerById(playerId);
        if (morphPlayer == null || target == null)
        {
            return;
        }
        GetRole(morphPlayer).StartMorph(target);
    }
}