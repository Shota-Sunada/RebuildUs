namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Sidekick : RoleBase<Sidekick>
{
    // write configs here
    public static CustomButton sidekickKillButton;
    public static CustomButton sidekickSabotageLightsButton;
    public PlayerControl currentTarget;
    public bool wasTeamRed = false;
    public bool wasImpostor = false;
    public bool wasSpy = false;

    public static bool canKill { get { return CustomOptionHolder.sidekickCanKill.GetBool(); } }
    public static bool canUseVents { get { return CustomOptionHolder.sidekickCanUseVents.GetBool(); } }
    public static bool canSabotageLights { get { return CustomOptionHolder.sidekickCanSabotageLights.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.sidekickHasImpostorVision.GetBool(); } }
    public static bool promotesToJackal { get { return CustomOptionHolder.sidekickPromotesToJackal.GetBool(); } }

    public Sidekick()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Sidekick;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Sidekick))
        {
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.Exists) untargetablePlayers.AddRange(Jackal.AllPlayers);
            // foreach (var mini in Mini.players)
            // {
            //     if (!Mini.isGrownUp(mini.player))
            //     {
            //         untargetablePlayers.Add(mini.player);
            //     }
            // }
            currentTarget = Helpers.setTarget(untargetablePlayers: untargetablePlayers);
            if (canKill) Helpers.setPlayerOutline(currentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        sidekickKillButton = new CustomButton(
                () =>
                {
                    if (Helpers.checkMurderAttemptAndKill(Local.Player, Local.currentTarget) == MurderAttemptResult.SuppressKill) return;
                    sidekickKillButton.Timer = sidekickKillButton.MaxTimer;
                    Local.currentTarget = null;
                },
                () => { return canKill && CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Sidekick) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return Local.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { sidekickKillButton.Timer = sidekickKillButton.MaxTimer; },
                hm.KillButton.graphic.sprite,
                new Vector3(0, 1f, 0),
                hm,
                hm.KillButton,
                KeyCode.Q
            );

        sidekickSabotageLightsButton = new CustomButton(
            () =>
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(ERoleType.Sidekick) && canSabotageLights && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                if (Helpers.sabotageTimer() > sidekickSabotageLightsButton.Timer || Helpers.sabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    sidekickSabotageLightsButton.Timer = Helpers.sabotageTimer() + 5f;
                }
                return Helpers.canUseSabotage();
            },
            () =>
            {
                sidekickSabotageLightsButton.Timer = Helpers.sabotageTimer() + 5f;
            },
            // Trickster.getLightsOutButtonSprite(),
            null,
            CustomButton.ButtonPositions.upperRowCenter,
            hm,
            hm.AbilityButton,
            KeyCode.G
        );
    }
    public static void SetButtonCooldowns()
    {
        sidekickKillButton.MaxTimer = Jackal.killCooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}