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
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Morphing)) return;
        CurrentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        MorphingButton = new CustomButton(
                () =>
                {
                    if (SampledTarget != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MorphingMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(SampledTarget.PlayerId);
                        writer.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.MorphingMorph(SampledTarget.PlayerId, Player.PlayerId);
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
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Morphing) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return (CurrentTarget || SampledTarget) && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
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
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
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
                }
            )
        {
            ButtonText = Tr.Get("SampleText")
        };
    }
    public override void SetButtonCooldowns()
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
        foreach (var morph in Players)
        {
            morph.HandleMorphing();
        }
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        CurrentTarget = null;
        SampledTarget = null;
        MorphTarget = null;
        MorphTimer = 0;
    }
}