namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Morphing : RoleBase<Morphing>
{
    public static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _morphingButton;
    public static PlayerControl CurrentTarget;
    public static PlayerControl SampledTarget;
    public static PlayerControl MorphTarget;
    public static float MorphTimer;

    public Morphing()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.MorphingCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.MorphingDuration.GetFloat();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }

    public override void OnIntroEnd()
    {
        ResetMorph();
    }

    public override void FixedUpdate()
    {
        var local = Local;
        if (local == null) return;
        CurrentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
    }

    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        _morphingButton = new(() =>
        {
            if (SampledTarget != null)
            {
                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.MorphingMorph);
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

    public static void SetButtonCooldowns()
    {
        _morphingButton.MaxTimer = Cooldown;
        _morphingButton.EffectDuration = Duration;
    }

    // write functions here
    public void HandleMorphing()
    {
        // first, if camouflager is active, don't do anything
        if (Camouflager.Exists && Camouflager.CamouflageTimer > 0f) return;

        // next, if we're currently morphed, set our skin to the target
        if (MorphTimer > 0f && MorphTarget != null)
            Player.MorphToPlayer(MorphTarget);
        else
            Player.ResetMorph();
    }

    public void StartMorph(PlayerControl target)
    {
        MorphTarget = target;
        MorphTimer = Duration;
        HandleMorphing();
    }

    public static void ResetMorph()
    {
        MorphTarget = null;
        MorphTimer = 0f;
        for (var i = 0; i < Players.Count; i++) Players[i].HandleMorphing();
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        CurrentTarget = null;
        SampledTarget = null;
        MorphTarget = null;
        MorphTimer = 0;
    }
}
