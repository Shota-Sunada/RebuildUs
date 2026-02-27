namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Witch : MultiRoleBase<Witch>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static CustomButton WitchSpellButton;

    internal static List<PlayerControl> FutureSpelled = [];
    private static PlayerControl _currentTarget;
    private static PlayerControl _spellCastingTarget;

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
    private static float Cooldown
    {
        get => CustomOptionHolder.WitchCooldown.GetFloat();
    }

    private static float AdditionalCooldown
    {
        get => CustomOptionHolder.WitchAdditionalCooldown.GetFloat();
    }

    private static bool CanSpellAnyone
    {
        get => CustomOptionHolder.WitchCanSpellAnyone.GetBool();
    }

    private static float SpellCastingDuration
    {
        get => CustomOptionHolder.WitchSpellCastingDuration.GetFloat();
    }

    private static bool TriggerBothCooldowns
    {
        get => CustomOptionHolder.WitchTriggerBothCooldowns.GetBool();
    }

    internal static bool VoteSavesTargets
    {
        get => CustomOptionHolder.WitchVoteSavesTargets.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Witch local = Local;
        if (local == null)
        {
            return;
        }
        List<PlayerControl> untargetables = [];
        if (_spellCastingTarget != null)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId != _spellCastingTarget.PlayerId)
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
                untargetables.Add(Spy.PlayerControl);
            }

            if (Sidekick.Exists && Sidekick.Instance.WasTeamRed && !CanSpellAnyone)
            {
                untargetables.Add(Sidekick.PlayerControl);
            }

            if (Jackal.Exists && Jackal.Instance.WasTeamRed && !CanSpellAnyone)
            {
                untargetables.Add(Jackal.PlayerControl);
            }
        }

        _currentTarget = Helpers.SetTarget(!CanSpellAnyone, untargetablePlayers: untargetables);
        Helpers.SetPlayerOutline(_currentTarget, RoleColor);
    }

    internal override void OnKill(PlayerControl target)
    {
        if (TriggerBothCooldowns && PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && WitchSpellButton != null)
        {
            WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
        }
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        WitchSpellButton = new(() =>
            {
                if (_currentTarget != null)
                {
                    _spellCastingTarget = _currentTarget;
                }
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (!WitchSpellButton.IsEffectActive || _spellCastingTarget == _currentTarget)
                {
                    return PlayerControl.LocalPlayer.CanMove && _currentTarget != null;
                }
                _spellCastingTarget = null;
                WitchSpellButton.Timer = 0f;
                WitchSpellButton.IsEffectActive = false;

                return PlayerControl.LocalPlayer.CanMove && _currentTarget != null;
            },
            () =>
            {
                WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                WitchSpellButton.IsEffectActive = false;
                _spellCastingTarget = null;
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
                if (_spellCastingTarget == null)
                {
                    return;
                }

                KillAnimationPatch.AvoidNextKillMovement = true;

                MurderAttemptResult attempt = Helpers.CheckMurderAttempt(Local.Player, _spellCastingTarget);
                if (attempt == MurderAttemptResult.PerformKill)
                {
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureSpelled);
                        sender.Write(_currentTarget.PlayerId);
                    }
                    RPCProcedure.SetFutureSpelled(_currentTarget.PlayerId);
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

                _spellCastingTarget = null;
            },
            false,
            Tr.Get(TrKey.WitchText));
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
        _currentTarget = null;
        _spellCastingTarget = null;
    }
}