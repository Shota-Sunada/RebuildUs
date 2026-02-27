namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Sheriff : MultiRoleBase<Sheriff>
{
    internal static Color NameColor = new Color32(248, 205, 70, byte.MaxValue);

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

    internal override Color RoleColor
    {
        get => NameColor;
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

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        bool anyoneDead = false;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data.IsDead)
            {
                anyoneDead = true;
                break;
            }
        }

        CanKill = SheriffCanKillNoDeadBody || anyoneDead;
    }

    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (Player == PlayerControl.LocalPlayer && NumShots > 0)
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, NameColor);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        // Sheriff Kill
        _sheriffKillButton = new(() =>
            {
                if (Local.NumShots <= 0)
                {
                    return;
                }

                MurderAttemptResult murderAttemptResult = Helpers.CheckMurderAttempt(PlayerControl.LocalPlayer, Local.CurrentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill)
                {
                    return;
                }

                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    byte targetId = Local.CurrentTarget.PlayerId;

                    if (AmongUsClient.Instance.AmHost)
                    {
                        bool misfire = CheckKill(Local.CurrentTarget);
                        {
                            using RPCSender killSender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SheriffKill);
                            killSender.Write(PlayerControl.LocalPlayer.Data.PlayerId);
                            killSender.Write(targetId);
                            killSender.Write(misfire);
                        }
                        RPCProcedure.SheriffKill(PlayerControl.LocalPlayer.Data.PlayerId, targetId, misfire);
                    }
                    else
                    {
                        using RPCSender requestSender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SheriffKillRequest);
                        requestSender.Write(PlayerControl.LocalPlayer.Data.PlayerId);
                        requestSender.Write(targetId);
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

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}