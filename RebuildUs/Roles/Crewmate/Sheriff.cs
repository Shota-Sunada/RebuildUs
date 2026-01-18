namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Sheriff : RoleBase<Sheriff>
{
    public static Color NameColor = new Color32(248, 205, 70, byte.MaxValue);
    public override Color RoleColor => NameColor;

    // write configs here
    private static CustomButton sheriffKillButton;
    public static TMP_Text sheriffNumShotsText;

    public static float cooldown { get { return CustomOptionHolder.sheriffCooldown.GetFloat(); } }
    public static int maxShots { get { return Mathf.RoundToInt(CustomOptionHolder.sheriffNumShots.GetFloat()); } }
    public static bool canKillNeutrals { get { return CustomOptionHolder.sheriffCanKillNeutrals.GetBool(); } }
    public static bool misfireKillsTarget { get { return CustomOptionHolder.sheriffMisfireKillsTarget.GetBool(); } }
    public static bool spyCanDieToSheriff { get { return CustomOptionHolder.SpyCanDieToSheriff.GetBool(); } }
    public static bool madmateCanDieToSheriff { get { return CustomOptionHolder.MadmateCanDieToSheriff.GetBool(); } }
    public static bool createdMadmateCanDieToSheriff { get { return CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool(); } }
    public static bool sheriffCanKillNoDeadBody { get { return CustomOptionHolder.sheriffCanKillNoDeadBody.GetBool(); } }

    public int numShots = 2;
    public bool canKill = sheriffCanKillNoDeadBody;
    public PlayerControl currentTarget;

    public Sheriff()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Sheriff;
        numShots = maxShots;
        canKill = sheriffCanKillNoDeadBody;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        canKill = sheriffCanKillNoDeadBody || PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Any(p => p.Data.IsDead);
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (Player == CachedPlayer.LocalPlayer.PlayerControl && numShots > 0)
        {
            currentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(currentTarget, NameColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        // Sheriff Kill
        sheriffKillButton = new CustomButton(
            () =>
            {
                if (numShots <= 0)
                {
                    return;
                }

                MurderAttemptResult murderAttemptResult = Helpers.CheckMurderAttempt(CachedPlayer.LocalPlayer.PlayerControl, currentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;

                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    bool misfire = false;
                    byte targetId = currentTarget.PlayerId; ;
                    if ((currentTarget.Data.Role.IsImpostor && (!currentTarget.HasModifier(ModifierType.Mini) || Mini.IsGrownUp(currentTarget))) ||
                        (spyCanDieToSheriff && currentTarget.IsRole(RoleType.Spy)) ||
                        (madmateCanDieToSheriff && currentTarget.HasModifier(ModifierType.Madmate)) ||
                        (createdMadmateCanDieToSheriff && currentTarget.HasModifier(ModifierType.CreatedMadmate)) ||
                        (canKillNeutrals && currentTarget.IsNeutral()) ||
                        currentTarget.IsRole(RoleType.Jackal) || currentTarget.IsRole(RoleType.Sidekick))
                    {
                        //targetId = Sheriff.currentTarget.PlayerId;
                        misfire = false;
                    }
                    else
                    {
                        //targetId = CachedPlayer.LocalPlayer.PlayerControl.PlayerId;
                        misfire = true;
                    }

                    // Mad sheriff always misfires.
                    if (Player.HasModifier(ModifierType.Madmate))
                    {
                        misfire = true;
                    }
                    {
                        using var killSender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SheriffKill);
                        killSender.Write(CachedPlayer.LocalPlayer.PlayerControl.Data.PlayerId);
                        killSender.Write(targetId);
                        killSender.Write(misfire);
                    }
                    RPCProcedure.sheriffKill(CachedPlayer.LocalPlayer.PlayerControl.Data.PlayerId, targetId, misfire);
                }

                sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                currentTarget = null;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Sheriff) && numShots > 0 && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && canKill; },
            () =>
            {
                sheriffNumShotsText?.text = numShots > 0 ? string.Format(Tr.Get("Hud.SheriffShots"), numShots) : "";
                return currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            new Vector3(0f, 1f, 0),
            hm,
            hm.KillButton,
            KeyCode.Q
        );

        sheriffNumShotsText = GameObject.Instantiate(sheriffKillButton.ActionButton.cooldownTimerText, sheriffKillButton.ActionButton.cooldownTimerText.transform.parent);
        sheriffNumShotsText.text = "";
        sheriffNumShotsText.enableWordWrapping = false;
        sheriffNumShotsText.transform.localScale = Vector3.one * 0.5f;
        sheriffNumShotsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }
    public override void SetButtonCooldowns()
    {
        sheriffKillButton.MaxTimer = cooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}