namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Morphing : RoleBase<Morphing>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton morphingButton;
    public static PlayerControl currentTarget;
    public static PlayerControl sampledTarget;
    public static PlayerControl morphTarget;
    public static float morphTimer = 0f;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.morphingCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.morphingDuration.GetFloat(); } }

    public Morphing()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Morphing;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd()
    {
        resetMorph();
    }
    public override void FixedUpdate()
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Morphing)) return;
        currentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(currentTarget, RoleColor);
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        morphingButton = new CustomButton(
                () =>
                {
                    if (sampledTarget != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MorphingMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(sampledTarget.PlayerId);
                        writer.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.morphingMorph(sampledTarget.PlayerId, Player.PlayerId);
                        sampledTarget = null;
                        morphingButton.EffectDuration = duration;
                    }
                    else if (currentTarget != null)
                    {
                        sampledTarget = currentTarget;
                        morphingButton.Sprite = AssetLoader.MorphButton;
                        morphingButton.ButtonText = Tr.Get("MorphText");
                        morphingButton.EffectDuration = 1f;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Morphing) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return (currentTarget || sampledTarget) && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () =>
                {
                    morphingButton.Timer = morphingButton.MaxTimer;
                    morphingButton.Sprite = AssetLoader.SampleButton;
                    morphingButton.ButtonText = Tr.Get("SampleText");
                    morphingButton.IsEffectActive = false;
                    morphingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    sampledTarget = null;
                },
                AssetLoader.SampleButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                duration,
                () =>
                {
                    if (sampledTarget == null)
                    {
                        morphingButton.Timer = morphingButton.MaxTimer;
                        morphingButton.Sprite = AssetLoader.SampleButton;
                        morphingButton.ButtonText = Tr.Get("SampleText");
                    }
                }
            )
        {
            ButtonText = Tr.Get("SampleText")
        };
    }
    public override void SetButtonCooldowns()
    {
        morphingButton.MaxTimer = cooldown;
        morphingButton.EffectDuration = duration;
    }

    // write functions here
    public void handleMorphing()
    {
        // first, if camouflager is active, don't do anything
        if (Camouflager.Exists && Camouflager.camouflageTimer > 0f) return;

        // next, if we're currently morphed, set our skin to the target
        if (morphTimer > 0f && morphTarget != null)
        {
            Player.morphToPlayer(morphTarget);
        }
        else
        {
            Player.resetMorph();
        }
    }

    public void startMorph(PlayerControl target)
    {
        morphTarget = target;
        morphTimer = duration;
        handleMorphing();
    }

    public static void resetMorph()
    {
        morphTarget = null;
        morphTimer = 0f;
        foreach (var morph in Players)
        {
            morph.handleMorphing();
        }
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        currentTarget = null;
        sampledTarget = null;
        morphTarget = null;
        morphTimer = 0;
    }
}