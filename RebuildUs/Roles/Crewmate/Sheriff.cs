namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Sheriff, RoleTeam.Crewmate, typeof(MultiRoleBase<Sheriff>), nameof(CustomOptionHolder.SheriffSpawnRate))]
internal class Sheriff : MultiRoleBase<Sheriff>
{
    internal static Color Color = new Color32(248, 205, 70, byte.MaxValue);

    // write configs here
    private static CustomButton _sheriffKillButton;
    internal static TMP_Text SheriffNumShotsText;
    internal bool CanKill = SheriffCanKillNoDeadBody;
    internal PlayerControl CurrentTarget;

    internal int NumShots = 2;

    public Sheriff()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Sheriff;
        NumShots = MaxShots;
        CanKill = SheriffCanKillNoDeadBody;
        CurrentTarget = null;
    }

    internal static float Cooldown
    {
        get => CustomOptionHolder.SheriffCooldown.GetFloat();
    }

    internal static int MaxShots
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SheriffNumShots.GetFloat());
    }

    internal static bool CanKillNeutrals
    {
        get => CustomOptionHolder.SheriffCanKillNeutrals.GetBool();
    }

    internal static bool MisfireKillsTarget
    {
        get => CustomOptionHolder.SheriffMisfireKillsTarget.GetBool();
    }

    internal static bool SpyCanDieToSheriff
    {
        get => CustomOptionHolder.SpyCanDieToSheriff.GetBool();
    }

    internal static bool MadmateCanDieToSheriff
    {
        get => CustomOptionHolder.MadmateCanDieToSheriff.GetBool();
    }

    internal static bool MadmateRoleCanDieToSheriff
    {
        get => CustomOptionHolder.MadmateRoleCanDieToSheriff.GetBool();
    }

    internal static bool SuiciderCanDieToSheriff
    {
        get => CustomOptionHolder.SuiciderCanDieToSheriff.GetBool();
    }

    internal static bool CreatedMadmateCanDieToSheriff
    {
        get => CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool();
    }

    internal static bool SheriffCanKillNoDeadBody
    {
        get => CustomOptionHolder.SheriffCanKillNoDeadBody.GetBool();
    }

    [CustomEvent(CustomEventType.OnMeetingEnd)]
    internal void OnMeetingEnd()
    {
        var anyoneDead = false;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data.IsDead)
            {
                anyoneDead = true;
                break;
            }
        }

        CanKill = SheriffCanKillNoDeadBody || anyoneDead;
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (Player == PlayerControl.LocalPlayer && NumShots > 0)
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        // Sheriff Kill
        _sheriffKillButton = new(() =>
            {
                if (Local.NumShots <= 0)
                {
                    return;
                }

                var murderAttemptResult = Helpers.CheckMurderAttempt(PlayerControl.LocalPlayer, Local.CurrentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill)
                {
                    return;
                }

                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    var targetId = Local.CurrentTarget.PlayerId;

                    if (AmongUsClient.Instance.AmHost)
                    {
                        var misfire = CheckKill(Local.CurrentTarget);
                        SheriffKill(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data.PlayerId, targetId, misfire);
                    }
                    else
                    {
                        SheriffKillRequest(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data.PlayerId, targetId);
                    }
                }

                _sheriffKillButton.Timer = _sheriffKillButton.MaxTimer;
                Local.CurrentTarget = null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Sheriff)
                       && Local.NumShots > 0
                       && PlayerControl.LocalPlayer?.Data?.IsDead == false
                       && Local.CanKill;
            },
            () =>
            {
                SheriffNumShotsText?.text = Local.NumShots > 0 ? string.Format(Tr.Get(TrKey.SheriffShots), Local.NumShots) : "";
                return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _sheriffKillButton.Timer = _sheriffKillButton.MaxTimer;
            },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        SheriffNumShotsText = UnityObject.Instantiate(_sheriffKillButton.ActionButton.cooldownTimerText,
            _sheriffKillButton.ActionButton.cooldownTimerText.transform.parent);
        SheriffNumShotsText.text = "";
        SheriffNumShotsText.enableWordWrapping = false;
        SheriffNumShotsText.transform.localScale = Vector3.one * 0.5f;
        SheriffNumShotsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _sheriffKillButton.MaxTimer = Cooldown;
    }

    internal static bool CheckKill(PlayerControl target)
    {
        if (target == null)
        {
            return true;
        }

        if (target.Data.Role.IsImpostor && (!target.HasModifier(ModifierType.Mini) || Mini.IsGrownUp(target))
            || SpyCanDieToSheriff && target.IsRole(RoleType.Spy)
            || MadmateCanDieToSheriff && target.HasModifier(ModifierType.Madmate)
            || MadmateRoleCanDieToSheriff && target.IsRole(RoleType.Madmate)
            || SuiciderCanDieToSheriff && target.IsRole(RoleType.Suicider)
            || CreatedMadmateCanDieToSheriff && target.HasModifier(ModifierType.CreatedMadmate)
            || CanKillNeutrals && target.IsNeutral()
            || target.IsRole(RoleType.Jackal)
            || target.IsRole(RoleType.Sidekick))
        {
            return false;
        }

        return true;
    }

    [MethodRpc((uint)CustomRPC.SheriffKillRequest)]
    internal static void SheriffKillRequest(PlayerControl sender, byte sheriffId, byte targetId)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }
        var sheriff = Helpers.PlayerById(sheriffId);
        var target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        var misfire = CheckKill(target);

        SheriffKill(sheriff, sheriffId, targetId, misfire);
    }

    [MethodRpc((uint)CustomRPC.SheriffKill)]
    internal static void SheriffKill(PlayerControl sender, byte sheriffId, byte targetId, bool misfire)
    {
        var sheriff = Helpers.PlayerById(sheriffId);
        var target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        var role = GetRole(sheriff);
        if (role != null)
        {
            role.NumShots--;
        }

        if (misfire)
        {
            sheriff.MurderPlayer(sheriff);
            GameHistory.FinalStatuses[sheriffId] = FinalStatus.Misfire;

            if (!MisfireKillsTarget)
            {
                return;
            }
            GameHistory.FinalStatuses[targetId] = FinalStatus.Misfire;
        }

        sheriff.MurderPlayer(target);
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}