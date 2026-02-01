namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Morphing : RoleBase<Morphing>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton MorphingButton;
    public static PlayerControl CurrentTarget;
    public static PlayerControl SampledTarget;
    public static PlayerControl MorphTarget;
    public static float MorphTimer = 0f;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.MorphingCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.MorphingDuration.GetFloat(); } }

    public Morphing()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
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
        MorphingButton = new CustomButton(
            () =>
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
                    MorphingButton.EffectDuration = Duration;
                }
                else if (CurrentTarget != null)
                {
                    SampledTarget = CurrentTarget;
                    MorphingButton.Sprite = AssetLoader.MorphButton;
                    MorphingButton.ButtonText = Tr.Get("MorphText");
                    MorphingButton.EffectDuration = 1f;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Morphing) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return (CurrentTarget || SampledTarget) && PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                MorphingButton.Timer = MorphingButton.MaxTimer;
                MorphingButton.Sprite = AssetLoader.SampleButton;
                MorphingButton.ButtonText = Tr.Get("SampleText");
                MorphingButton.IsEffectActive = false;
                MorphingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                SampledTarget = null;
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
                if (SampledTarget == null)
                {
                    MorphingButton.Timer = MorphingButton.MaxTimer;
                    MorphingButton.Sprite = AssetLoader.SampleButton;
                    MorphingButton.ButtonText = Tr.Get("SampleText");
                }
            },
            false,
            Tr.Get("SampleText")
        );
    }
    public static void SetButtonCooldowns()
    {
        MorphingButton.MaxTimer = Cooldown;
        MorphingButton.EffectDuration = Duration;
    }

    // write functions here
    public void HandleMorphing()
    {
        // first, if camouflager is active, don't do anything
        if (Camouflager.Exists && Camouflager.CamouflageTimer > 0f) return;

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
        for (var i = 0; i < Players.Count; i++)
        {
            Players[i].HandleMorphing();
        }
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
