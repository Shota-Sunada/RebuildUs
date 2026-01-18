using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Witch : RoleBase<Witch>
{
    public static Color RoleColor = Palette.ImpostorRed;
    public static CustomButton witchSpellButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.witchCooldown.GetFloat(); } }
    public static float additionalCooldown { get { return CustomOptionHolder.witchAdditionalCooldown.GetFloat(); } }
    public static bool canSpellAnyone { get { return CustomOptionHolder.witchCanSpellAnyone.GetBool(); } }
    public static float spellCastingDuration { get { return CustomOptionHolder.witchSpellCastingDuration.GetFloat(); } }
    public static bool triggerBothCooldowns { get { return CustomOptionHolder.witchTriggerBothCooldowns.GetBool(); } }
    public static bool voteSavesTargets { get { return CustomOptionHolder.witchVoteSavesTargets.GetBool(); } }

    public static List<PlayerControl> futureSpelled = [];
    public static PlayerControl currentTarget;
    public static PlayerControl spellCastingTarget;

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
        if (spellCastingTarget != null)
        {
            untargetables = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.PlayerId != spellCastingTarget.PlayerId)]; // Don't switch the target from the the one you're currently casting a spell on
        }
        else
        {
            // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            untargetables = [];
            if (Spy.Exists && !canSpellAnyone)
            {
                untargetables.AddRange(Spy.AllPlayers);
            }
            foreach (var sidekick in Sidekick.Players)
            {
                if (sidekick.WasTeamRed && !canSpellAnyone)
                {
                    untargetables.Add(sidekick.Player);
                }
            }

            foreach (var jackal in Jackal.Players)
            {
                if (jackal.WasTeamRed && !canSpellAnyone)
                {
                    untargetables.Add(jackal.Player);
                }
            }
        }
        currentTarget = Helpers.SetTarget(onlyCrewmates: !canSpellAnyone, untargetablePlayers: untargetables);
        Helpers.SetPlayerOutline(currentTarget, RoleColor);

    }
    public override void OnKill(PlayerControl target)
    {
        if (triggerBothCooldowns && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch) && witchSpellButton != null)
        {
            witchSpellButton.Timer = witchSpellButton.MaxTimer;
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        witchSpellButton = new CustomButton(
                () =>
                {
                    if (currentTarget != null)
                    {
                        spellCastingTarget = currentTarget;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (witchSpellButton.IsEffectActive && spellCastingTarget != currentTarget)
                    {
                        spellCastingTarget = null;
                        witchSpellButton.Timer = 0f;
                        witchSpellButton.IsEffectActive = false;
                    }
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && currentTarget != null;
                },
                () =>
                {
                    witchSpellButton.Timer = witchSpellButton.MaxTimer;
                    witchSpellButton.IsEffectActive = false;
                    spellCastingTarget = null;
                },
                AssetLoader.SpellButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                spellCastingDuration,
                () =>
                {
                    if (spellCastingTarget == null) return;
                    MurderAttemptResult attempt = Helpers.CheckMurderAttempt(Player, spellCastingTarget);
                    if (attempt == MurderAttemptResult.PerformKill)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureSpelled, SendOption.Reliable, -1);
                        writer.Write(currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setFutureSpelled(currentTarget.PlayerId);
                    }
                    if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                    {
                        witchSpellButton.MaxTimer += additionalCooldown;
                        witchSpellButton.Timer = witchSpellButton.MaxTimer;
                        if (triggerBothCooldowns)
                        {
                            Player.killTimer = Helpers.GetOption(FloatOptionNames.KillCooldown);
                        }
                    }
                    else
                    {
                        witchSpellButton.Timer = 0f;
                    }
                    spellCastingTarget = null;
                }
            )
        {
            ButtonText = Tr.Get("WitchText")
        };
    }
    public override void SetButtonCooldowns()
    {
        witchSpellButton.MaxTimer = cooldown;
        witchSpellButton.EffectDuration = spellCastingDuration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        futureSpelled = [];
        currentTarget = null;
        spellCastingTarget = null;
    }
}