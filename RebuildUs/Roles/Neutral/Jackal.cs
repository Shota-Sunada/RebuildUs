namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jackal : RoleBase<Jackal>
{
    public static Color RoleColor = new Color32(0, 180, 235, byte.MaxValue);
    public static CustomButton jackalKillButton;
    private static CustomButton jackalSidekickButton;
    public static CustomButton jackalSabotageLightsButton;

    public PlayerControl fakeSidekick;
    public PlayerControl currentTarget;
    public PlayerControl mySidekick;
    public static List<PlayerControl> formerJackals = [];
    public bool canSidekick = false;

    // write configs here
    public static float killCooldown { get { return CustomOptionHolder.jackalKillCooldown.GetFloat(); } }
    public static bool canSabotageLights { get { return CustomOptionHolder.jackalCanSabotageLights.GetBool(); } }
    public static bool canUseVents { get { return CustomOptionHolder.jackalCanUseVents.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.jackalHasImpostorVision.GetBool(); } }
    public static bool canCreateSidekick { get { return CustomOptionHolder.jackalCanCreateSidekick.GetBool(); } }
    public static float createSidekickCooldown { get { return CustomOptionHolder.jackalCreateSidekickCooldown.GetFloat(); } }
    public static bool jackalPromotedFromSidekickCanCreateSidekick { get { return CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.GetBool(); } }
    public static bool canCreateSidekickFromImpostor { get { return CustomOptionHolder.jackalCanCreateSidekickFromImpostor.GetBool(); } }

    public Jackal()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Jackal;
        canSidekick = canCreateSidekick;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal))
        {
            var untargetablePlayers = new List<PlayerControl>();
            if (canCreateSidekickFromImpostor)
            {
                // Only exclude sidekick from being targeted if the jackal can create sidekicks from impostors
                if (Sidekick.Exists) untargetablePlayers.AddRange(Sidekick.AllPlayers);
            }
            foreach (var mini in Mini.players)
            {
                if (!Mini.isGrownUp(mini.player))
                {
                    untargetablePlayers.Add(mini.player);
                }
            }
            currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            setPlayerOutline(currentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Sidekick.promotesToJackal && mySidekick != null && mySidekick.IsAlive())
        {
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SidekickPromotes);
            sender.Write(mySidekick.PlayerId);
            RPCProcedure.sidekickPromotes(mySidekick.PlayerId);
        }
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        // Jackal Sidekick Button
        jackalSidekickButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.JackalCreatesSidekick);
                sender.Write(Local.currentTarget.PlayerId);
                sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                RPCProcedure.jackalCreatesSidekick(Local.currentTarget.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
            },
            () => { return canCreateSidekick && CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return canCreateSidekick && Local.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer; },
            getSidekickButtonSprite(),
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            buttonText = Tr.Get("SidekickText")
        };

        // Jackal Kill
        jackalKillButton = new CustomButton(
            () =>
            {
                if (Helpers.checkMurderAttemptAndKill(Local.Player, Local.currentTarget) == MurderAttemptResult.SuppressKill) return;

                jackalKillButton.Timer = jackalKillButton.MaxTimer;
                Local.currentTarget = null;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return Local.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            new Vector3(0, 1f, 0),
            hm,
            hm.KillButton,
            KeyCode.Q
        );

        jackalSabotageLightsButton = new CustomButton(
                () =>
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
                },
                () =>
                {
                    return PlayerControl.LocalPlayer.IsRole(ERoleType.Jackal) && canSabotageLights && PlayerControl.LocalPlayer.IsAlive();
                },
                () =>
                {
                    if (Helpers.sabotageTimer() > jackalSabotageLightsButton.Timer || Helpers.sabotageActive())
                    {
                        // this will give imps time to do another sabotage.
                        jackalSabotageLightsButton.Timer = Helpers.sabotageTimer() + 5f;
                    }
                    return Helpers.canUseSabotage();
                },
                () =>
                {
                    jackalSabotageLightsButton.Timer = Helpers.sabotageTimer() + 5f;
                },
                Trickster.getLightsOutButtonSprite(),
                CustomButton.ButtonPositions.upperRowCenter,
                hm,
                hm.AbilityButton,
                KeyCode.G
            );
    }
    public static void SetButtonCooldowns()
    {
        jackalKillButton.MaxTimer = killCooldown;
        jackalSidekickButton.MaxTimer = createSidekickCooldown;
    }

    // write functions here
    public static void removeCurrentJackal()
    {
        if (!formerJackals.Any(x => x.PlayerId == Local.Player.PlayerId)) formerJackals.Add(Local.Player);
    }

    public static void Clear()
    {
        // reset configs here
        formerJackals = [];
        Players.Clear();
    }
}