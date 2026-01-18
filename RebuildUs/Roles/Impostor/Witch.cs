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
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch)) return;
        List<PlayerControl> untargetables;
        if (SpellCastingTarget != null)
        {
            untargetables = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.PlayerId != SpellCastingTarget.PlayerId)]; // Don't switch the target from the the one you're currently casting a spell on
        }
        else
        {
            // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            untargetables = [];
            if (Spy.Exists && !CanSpellAnyone)
            {
                untargetables.AddRange(Spy.AllPlayers);
            }
            foreach (var sidekick in Sidekick.Players)
            {
                if (sidekick.WasTeamRed && !CanSpellAnyone)
                {
                    untargetables.Add(sidekick.Player);
                }
            }

            foreach (var jackal in Jackal.Players)
            {
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
        if (TriggerBothCooldowns && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch) && WitchSpellButton != null)
        {
            WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        WitchSpellButton = new CustomButton(
                () =>
                {
                    if (CurrentTarget != null)
                    {
                        SpellCastingTarget = CurrentTarget;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (WitchSpellButton.IsEffectActive && SpellCastingTarget != CurrentTarget)
                    {
                        SpellCastingTarget = null;
                        WitchSpellButton.Timer = 0f;
                        WitchSpellButton.IsEffectActive = false;
                    }
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && CurrentTarget != null;
                },
                () =>
                {
                    WitchSpellButton.Timer = WitchSpellButton.MaxTimer;
                    WitchSpellButton.IsEffectActive = false;
                    SpellCastingTarget = null;
                },
                AssetLoader.SpellButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                SpellCastingDuration,
                () =>
                {
                    if (SpellCastingTarget == null) return;
                    MurderAttemptResult attempt = Helpers.CheckMurderAttempt(Player, SpellCastingTarget);
                    if (attempt == MurderAttemptResult.PerformKill)
                    {
                        {
                            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetFutureSpelled);
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
                            Player.killTimer = Helpers.GetOption(FloatOptionNames.KillCooldown);
                        }
                    }
                    else
                    {
                        WitchSpellButton.Timer = 0f;
                    }
                    SpellCastingTarget = null;
                }
            )
        {
            ButtonText = Tr.Get("Hud.WitchText")
        };
    }
    public override void SetButtonCooldowns()
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