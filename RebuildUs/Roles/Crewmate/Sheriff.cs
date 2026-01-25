namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Sheriff : RoleBase<Sheriff>
{
    public static Color NameColor = new Color32(248, 205, 70, byte.MaxValue);
    public override Color RoleColor => NameColor;

    // write configs here
    private static CustomButton SheriffKillButton;
    public static TMP_Text SheriffNumShotsText;

    public static float Cooldown { get { return CustomOptionHolder.SheriffCooldown.GetFloat(); } }
    public static int MaxShots { get { return Mathf.RoundToInt(CustomOptionHolder.SheriffNumShots.GetFloat()); } }
    public static bool CanKillNeutrals { get { return CustomOptionHolder.SheriffCanKillNeutrals.GetBool(); } }
    public static bool MisfireKillsTarget { get { return CustomOptionHolder.SheriffMisfireKillsTarget.GetBool(); } }
    public static bool SpyCanDieToSheriff { get { return CustomOptionHolder.SpyCanDieToSheriff.GetBool(); } }
    public static bool MadmateCanDieToSheriff { get { return CustomOptionHolder.MadmateCanDieToSheriff.GetBool(); } }
    public static bool CreatedMadmateCanDieToSheriff { get { return CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool(); } }
    public static bool SheriffCanKillNoDeadBody { get { return CustomOptionHolder.SheriffCanKillNoDeadBody.GetBool(); } }

    public int NumShots = 2;
    public bool CanKill = SheriffCanKillNoDeadBody;
    public PlayerControl CurrentTarget;

    public Sheriff()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Sheriff;
        NumShots = MaxShots;
        CanKill = SheriffCanKillNoDeadBody;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        bool anyoneDead = false;
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.Data.IsDead)
            {
                anyoneDead = true;
                break;
            }
        }
        CanKill = SheriffCanKillNoDeadBody || anyoneDead;
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (Player == PlayerControl.LocalPlayer && NumShots > 0)
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, NameColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        // Sheriff Kill
        SheriffKillButton = new CustomButton(
            () =>
            {
                if (Local.NumShots <= 0)
                {
                    return;
                }

                MurderAttemptResult murderAttemptResult = Helpers.CheckMurderAttempt(PlayerControl.LocalPlayer, Local.CurrentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;

                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    bool misfire = false;
                    byte targetId = Local.CurrentTarget.PlayerId; ;
                    if ((Local.CurrentTarget.Data.Role.IsImpostor && (!Local.CurrentTarget.HasModifier(ModifierType.Mini) || Mini.IsGrownUp(Local.CurrentTarget))) ||
                        (SpyCanDieToSheriff && Local.CurrentTarget.IsRole(RoleType.Spy)) ||
                        (MadmateCanDieToSheriff && Local.CurrentTarget.HasModifier(ModifierType.Madmate)) ||
                        (CreatedMadmateCanDieToSheriff && Local.CurrentTarget.HasModifier(ModifierType.CreatedMadmate)) ||
                        (CanKillNeutrals && Local.CurrentTarget.IsNeutral()) ||
                        Local.CurrentTarget.IsRole(RoleType.Jackal) || Local.CurrentTarget.IsRole(RoleType.Sidekick))
                    {
                        //targetId = Sheriff.currentTarget.PlayerId;
                        misfire = false;
                    }
                    else
                    {
                        //targetId = PlayerControl.LocalPlayer.PlayerId;
                        misfire = true;
                    }

                    // Mad sheriff always misfires.
                    if (Local.Player.HasModifier(ModifierType.Madmate))
                    {
                        misfire = true;
                    }
                    {
                        using var killSender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SheriffKill);
                        killSender.Write(PlayerControl.LocalPlayer.Data.PlayerId);
                        killSender.Write(targetId);
                        killSender.Write(misfire);
                    }
                    RPCProcedure.SheriffKill(PlayerControl.LocalPlayer.Data.PlayerId, targetId, misfire);
                }

                SheriffKillButton.Timer = SheriffKillButton.MaxTimer;
                Local.CurrentTarget = null;
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Sheriff) && Local.NumShots > 0 && PlayerControl.LocalPlayer?.Data?.IsDead == false && Local.CanKill; },
            () =>
            {
                SheriffNumShotsText?.text = Local.NumShots > 0 ? string.Format(Tr.Get("Hud.SheriffShots"), Local.NumShots) : "";
                return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { SheriffKillButton.Timer = SheriffKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            KeyCode.Q
        );

        SheriffNumShotsText = GameObject.Instantiate(SheriffKillButton.ActionButton.cooldownTimerText, SheriffKillButton.ActionButton.cooldownTimerText.transform.parent);
        SheriffNumShotsText.text = "";
        SheriffNumShotsText.enableWordWrapping = false;
        SheriffNumShotsText.transform.localScale = Vector3.one * 0.5f;
        SheriffNumShotsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }
    public static void SetButtonCooldowns()
    {
        SheriffKillButton.MaxTimer = Cooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}