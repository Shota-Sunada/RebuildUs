namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Witch, RoleTeam.Impostor, typeof(MultiRoleBase<Witch>), nameof(Witch.NameColor), nameof(CustomOptionHolder.WitchSpawnRate))]
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

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local == null)
        {
            return;
        }
        List<PlayerControl> untargetables = [];
        if (_spellCastingTarget != null)
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
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

    [CustomEvent(CustomEventType.OnKill)]
    internal void OnKill(PlayerControl target)
    {
        if (TriggerBothCooldowns && PlayerControl.LocalPlayer.IsRole(RoleType.Witch) && WitchSpellButton != null)
        {
            WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
        }
    }



    [RegisterCustomButton]
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

                var attempt = Helpers.CheckMurderAttempt(Local.Player, _spellCastingTarget);
                if (attempt == MurderAttemptResult.PerformKill)
                {
                    SetFutureSpelled(PlayerControl.LocalPlayer, _currentTarget.PlayerId);
                }

                if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                {
                    WitchSpellButton.MaxTimer += AdditionalCooldown;
                    WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                    if (TriggerBothCooldowns)
                    {
                        Local.Player.killTimer = FloatOptionNames.KillCooldown.Get();
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

    [RegisterCustomButton]
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

    [MethodRpc((uint)CustomRPC.SetFutureSpelled)]
    internal static void SetFutureSpelled(PlayerControl sender, byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        FutureSpelled ??= [];
        if (player != null)
        {
            FutureSpelled.Add(player);
        }
    }

    [MethodRpc((uint)CustomRPC.WitchSpellCast)]
    internal static void WitchSpellCast(PlayerControl sender, byte playerId)
    {
        RPCProcedure.ExilePlayerLocal(playerId);
        GameHistory.FinalStatuses[playerId] = FinalStatus.Spelled;
    }
}