namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Witch : RoleBase<Witch>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static CustomButton WitchSpellButton;

    internal static List<PlayerControl> FutureSpelled = [];
    internal static PlayerControl CurrentTarget;
    internal static PlayerControl SpellCastingTarget;

    public Witch()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Witch;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.WitchCooldown.GetFloat(); }
    internal static float AdditionalCooldown { get => CustomOptionHolder.WitchAdditionalCooldown.GetFloat(); }
    internal static bool CanSpellAnyone { get => CustomOptionHolder.WitchCanSpellAnyone.GetBool(); }
    internal static float SpellCastingDuration { get => CustomOptionHolder.WitchSpellCastingDuration.GetFloat(); }
    internal static bool TriggerBothCooldowns { get => CustomOptionHolder.WitchTriggerBothCooldowns.GetBool(); }
    internal static bool VoteSavesTargets { get => CustomOptionHolder.WitchVoteSavesTargets.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Witch local = Local;
        if (local == null) return;
        List<PlayerControl> untargetables = [];
        if (SpellCastingTarget != null)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (p.PlayerId != SpellCastingTarget.PlayerId)
                    untargetables.Add(p);
        }
        else
        {
            // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            if (Spy.Exists && !CanSpellAnyone)
            {
                List<PlayerControl> spyPlayers = Spy.AllPlayers;
                for (int i = 0; i < spyPlayers.Count; i++) untargetables.Add(spyPlayers[i]);
            }

            List<Sidekick> sidekickPlayers = Sidekick.Players;
            for (int i = 0; i < sidekickPlayers.Count; i++)
            {
                Sidekick sidekick = sidekickPlayers[i];
                if (sidekick.WasTeamRed && !CanSpellAnyone) untargetables.Add(sidekick.Player);
            }

            List<Jackal> jackalPlayers = Jackal.Players;
            for (int i = 0; i < jackalPlayers.Count; i++)
            {
                Jackal jackal = jackalPlayers[i];
                if (jackal.WasTeamRed && !CanSpellAnyone) untargetables.Add(jackal.Player);
            }
        }

        CurrentTarget = Helpers.SetTarget(!CanSpellAnyone, untargetablePlayers: untargetables);
        Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
    }

    internal override void OnKill(PlayerControl target)
    {
        if (TriggerBothCooldowns && PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && WitchSpellButton != null) WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        WitchSpellButton = new(() =>
        {
            if (CurrentTarget != null) SpellCastingTarget = CurrentTarget;
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            if (WitchSpellButton.IsEffectActive && SpellCastingTarget != CurrentTarget)
            {
                SpellCastingTarget = null;
                WitchSpellButton.Timer = 0f;
                WitchSpellButton.IsEffectActive = false;
            }

            return PlayerControl.LocalPlayer.CanMove && CurrentTarget != null;
        }, () =>
        {
            WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
            WitchSpellButton.IsEffectActive = false;
            SpellCastingTarget = null;
        }, AssetLoader.SpellButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, true, SpellCastingDuration, () =>
        {
            if (SpellCastingTarget == null) return;

            KillAnimationPatch.AvoidNextKillMovement = true;

            MurderAttemptResult attempt = Helpers.CheckMurderAttempt(Local.Player, SpellCastingTarget);
            if (attempt == MurderAttemptResult.PerformKill)
            {
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureSpelled);
                    sender.Write(CurrentTarget.PlayerId);
                }
                RPCProcedure.SetFutureSpelled(CurrentTarget.PlayerId);
            }

            if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
            {
                WitchSpellButton.MaxTimer += AdditionalCooldown;
                WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                if (TriggerBothCooldowns) Local.Player.killTimer = Helpers.GetOption(FloatOptionNames.KillCooldown);
            }
            else
                WitchSpellButton.Timer = 0f;

            SpellCastingTarget = null;
        }, false, Tr.Get(TrKey.WitchText));
    }

    internal static void SetButtonCooldowns()
    {
        WitchSpellButton.MaxTimer = Cooldown;
        WitchSpellButton.EffectDuration = SpellCastingDuration;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        FutureSpelled = [];
        CurrentTarget = null;
        SpellCastingTarget = null;
    }
}