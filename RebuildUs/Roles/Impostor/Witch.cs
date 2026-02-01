namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Witch : RoleBase<Witch>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static CustomButton WitchSpellButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.WitchCooldown.GetFloat(); } }
    public static float AdditionalCooldown { get { return CustomOptionHolder.WitchAdditionalCooldown.GetFloat(); } }
    public static bool CanSpellAnyone { get { return CustomOptionHolder.WitchCanSpellAnyone.GetBool(); } }
    public static float SpellCastingDuration { get { return CustomOptionHolder.WitchSpellCastingDuration.GetFloat(); } }
    public static bool TriggerBothCooldowns { get { return CustomOptionHolder.WitchTriggerBothCooldowns.GetBool(); } }
    public static bool VoteSavesTargets { get { return CustomOptionHolder.WitchVoteSavesTargets.GetBool(); } }

    public static List<PlayerControl> FutureSpelled = [];
    public static PlayerControl CurrentTarget;
    public static PlayerControl SpellCastingTarget;

    public Witch()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Witch;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var local = Local;
        if (local == null) return;
        List<PlayerControl> untargetables = [];
        if (SpellCastingTarget != null)
        {
            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var p = allPlayers[i];
                if (p.PlayerId != SpellCastingTarget.PlayerId)
                {
                    untargetables.Add(p);
                }
            }
        }
        else
        {
            // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            if (Spy.Exists && !CanSpellAnyone)
            {
                var spyPlayers = Spy.AllPlayers;
                for (var i = 0; i < spyPlayers.Count; i++)
                {
                    untargetables.Add(spyPlayers[i]);
                }
            }
            var sidekickPlayers = Sidekick.Players;
            for (var i = 0; i < sidekickPlayers.Count; i++)
            {
                var sidekick = sidekickPlayers[i];
                if (sidekick.WasTeamRed && !CanSpellAnyone)
                {
                    untargetables.Add(sidekick.Player);
                }
            }
            var jackalPlayers = Jackal.Players;
            for (var i = 0; i < jackalPlayers.Count; i++)
            {
                var jackal = jackalPlayers[i];
                if (jackal.WasTeamRed && !CanSpellAnyone)
                {
                    untargetables.Add(jackal.Player);
                }
            }
        }
        CurrentTarget = Helpers.SetTarget(onlyCrewmates: !CanSpellAnyone, untargetablePlayers: untargetables);
        Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
    }
    public override void OnKill(PlayerControl target)
    {
        if (TriggerBothCooldowns && PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && WitchSpellButton != null)
        {
            WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        WitchSpellButton = new CustomButton(
            () =>
            {
                if (CurrentTarget != null)
                {
                    SpellCastingTarget = CurrentTarget;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                if (WitchSpellButton.IsEffectActive && SpellCastingTarget != CurrentTarget)
                {
                    SpellCastingTarget = null;
                    WitchSpellButton.Timer = 0f;
                    WitchSpellButton.IsEffectActive = false;
                }
                return PlayerControl.LocalPlayer.CanMove && CurrentTarget != null;
            },
            () =>
            {
                WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                WitchSpellButton.IsEffectActive = false;
                SpellCastingTarget = null;
            },
            AssetLoader.SpellButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            true,
            SpellCastingDuration,
            () =>
            {
                if (SpellCastingTarget == null) return;

                KillAnimationPatch.AvoidNextKillMovement = true;

                MurderAttemptResult attempt = Helpers.CheckMurderAttempt(Local.Player, SpellCastingTarget);
                if (attempt == MurderAttemptResult.PerformKill)
                {
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureSpelled);
                        sender.Write(CurrentTarget.PlayerId);
                    }
                    RPCProcedure.SetFutureSpelled(CurrentTarget.PlayerId);
                }
                if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                {
                    WitchSpellButton.MaxTimer += AdditionalCooldown;
                    WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                    if (TriggerBothCooldowns)
                    {
                        Local.Player.killTimer = Helpers.GetOption(FloatOptionNames.KillCooldown);
                    }
                }
                else
                {
                    WitchSpellButton.Timer = 0f;
                }
                SpellCastingTarget = null;
            },
            false,
            Tr.Get(TranslateKey.WitchText)
        );
    }
    public static void SetButtonCooldowns()
    {
        WitchSpellButton.MaxTimer = Cooldown;
        WitchSpellButton.EffectDuration = SpellCastingDuration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        FutureSpelled = [];
        CurrentTarget = null;
        SpellCastingTarget = null;
    }
}

