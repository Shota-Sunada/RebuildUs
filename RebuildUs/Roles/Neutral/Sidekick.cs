namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Sidekick : RoleBase<Sidekick>
{
    // write configs here
    public static CustomButton SidekickKillButton;
    public static CustomButton SidekickSabotageLightsButton;
    public PlayerControl CurrentTarget;
    public bool WasTeamRed = false;
    public bool WasImpostor = false;
    public bool WasSpy = false;

    public static bool CanKill { get { return CustomOptionHolder.SidekickCanKill.GetBool(); } }
    public static bool CanUseVents { get { return CustomOptionHolder.SidekickCanUseVents.GetBool(); } }
    public static bool CanSabotageLights { get { return CustomOptionHolder.SidekickCanSabotageLights.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.SidekickHasImpostorVision.GetBool(); } }
    public static bool PromotesToJackal { get { return CustomOptionHolder.SidekickPromotesToJackal.GetBool(); } }

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
            CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
            if (CanKill) Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        SidekickKillButton = new CustomButton(
                () =>
                {
                    if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local.CurrentTarget) == MurderAttemptResult.SuppressKill) return;
                    SidekickKillButton.Timer = SidekickKillButton.MaxTimer;
                    Local.CurrentTarget = null;
                },
                () => { return CanKill && CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Sidekick) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return Local.CurrentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { SidekickKillButton.Timer = SidekickKillButton.MaxTimer; },
                hm.KillButton.graphic.sprite,
                new Vector3(0, 1f, 0),
                hm,
                hm.KillButton,
                KeyCode.Q
            );

        SidekickSabotageLightsButton = new CustomButton(
            () =>
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(ERoleType.Sidekick) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                if (Helpers.SabotageTimer() > SidekickSabotageLightsButton.Timer || Helpers.SabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    SidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                }
                return Helpers.CanUseSabotage();
            },
            () =>
            {
                SidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
            },
            // Trickster.getLightsOutButtonSprite(),
            null,
            CustomButton.ButtonPositions.UpperRowCenter,
            hm,
            hm.AbilityButton,
            KeyCode.G
        );
    }
    public static void SetButtonCooldowns()
    {
        SidekickKillButton.MaxTimer = Jackal.KillCooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}