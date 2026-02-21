namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Morphing : RoleBase<Morphing>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _morphingButton;
    internal static PlayerControl CurrentTarget;
    internal static PlayerControl SampledTarget;
    internal static PlayerControl MorphTarget;
    internal static float MorphTimer;

    public Morphing()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.MorphingCooldown.GetFloat(); }
    internal static float Duration { get => CustomOptionHolder.MorphingDuration.GetFloat(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }

    internal override void OnIntroEnd()
    {
        ResetMorph();
    }

    internal override void FixedUpdate()
    {
        Morphing local = Local;
        if (local == null) return;
        CurrentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _morphingButton = new(() =>
        {
            if (SampledTarget != null)
            {
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.MorphingMorph);
                    sender.Write(SampledTarget.PlayerId);
                    sender.Write(Local.Player.PlayerId);
                }
                RPCProcedure.MorphingMorph(SampledTarget.PlayerId, Local.Player.PlayerId);
                SampledTarget = null;
                _morphingButton.EffectDuration = Duration;
            }
            else if (CurrentTarget != null)
            {
                SampledTarget = CurrentTarget;
                _morphingButton.Sprite = AssetLoader.MorphButton;
                _morphingButton.ButtonText = Tr.Get(TrKey.MorphText);
                _morphingButton.EffectDuration = 1f;
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Morphing) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return (CurrentTarget || SampledTarget) && PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            _morphingButton.Timer = _morphingButton.MaxTimer;
            _morphingButton.Sprite = AssetLoader.SampleButton;
            _morphingButton.ButtonText = Tr.Get(TrKey.SampleText);
            _morphingButton.IsEffectActive = false;
            _morphingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            SampledTarget = null;
        }, AssetLoader.SampleButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, true, Duration, () =>
        {
            if (SampledTarget == null)
            {
                _morphingButton.Timer = _morphingButton.MaxTimer;
                _morphingButton.Sprite = AssetLoader.SampleButton;
                _morphingButton.ButtonText = Tr.Get(TrKey.SampleText);
            }
        }, false, Tr.Get(TrKey.SampleText));
    }

    internal static void SetButtonCooldowns()
    {
        _morphingButton.MaxTimer = Cooldown;
        _morphingButton.EffectDuration = Duration;
    }

    // write functions here
    internal void HandleMorphing()
    {
        // first, if camouflager is active, don't do anything
        if (Camouflager.Exists && Camouflager.CamouflageTimer > 0f) return;

        // next, if we're currently morphed, set our skin to the target
        if (MorphTimer > 0f && MorphTarget != null)
            Player.MorphToPlayer(MorphTarget);
        else
            Player.ResetMorph();
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
        for (int i = 0; i < Players.Count; i++) Players[i].HandleMorphing();
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        CurrentTarget = null;
        SampledTarget = null;
        MorphTarget = null;
        MorphTimer = 0;
    }
}