namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jackal : RoleBase<Jackal>
{
    public static Color RoleColor = new Color32(0, 180, 235, byte.MaxValue);
    public static CustomButton JackalKillButton;
    private static CustomButton JackalSidekickButton;
    public static CustomButton JackalSabotageLightsButton;

    public PlayerControl FakeSidekick;
    public PlayerControl CurrentTarget;
    public PlayerControl MySidekick;
    public static List<PlayerControl> FormerJackals = [];
    public bool CanSidekick = false;
    public bool WasTeamRed = false;
    public bool WasImpostor = false;
    public bool WasSpy = false;

    // write configs here
    public static float KillCooldown { get { return CustomOptionHolder.JackalKillCooldown.GetFloat(); } }
    public static bool CanSabotageLights { get { return CustomOptionHolder.JackalCanSabotageLights.GetBool(); } }
    public static bool CanUseVents { get { return CustomOptionHolder.JackalCanUseVents.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.JackalHasImpostorVision.GetBool(); } }
    public static bool CanCreateSidekick { get { return CustomOptionHolder.JackalCanCreateSidekick.GetBool(); } }
    public static float CreateSidekickCooldown { get { return CustomOptionHolder.JackalCreateSidekickCooldown.GetFloat(); } }
    public static bool JackalPromotedFromSidekickCanCreateSidekick { get { return CustomOptionHolder.JackalPromotedFromSidekickCanCreateSidekick.GetBool(); } }
    public static bool CanCreateSidekickFromImpostor { get { return CustomOptionHolder.JackalCanCreateSidekickFromImpostor.GetBool(); } }

    public Jackal()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Jackal;
        CanSidekick = CanCreateSidekick;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal))
        {
            var untargetablePlayers = new List<PlayerControl>();
            if (CanCreateSidekickFromImpostor)
            {
                // Only exclude sidekick from being targeted if the jackal can create sidekicks from impostors
                if (Sidekick.Exists) untargetablePlayers.AddRange(Sidekick.AllPlayers);
            }
            // foreach (var mini in Mini.players)
            // {
            //     if (!Mini.isGrownUp(mini.player))
            //     {
            //         untargetablePlayers.Add(mini.player);
            //     }
            // }
            CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
            Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Sidekick.PromotesToJackal && MySidekick != null && MySidekick.IsAlive())
        {
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SidekickPromotes);
            sender.Write(MySidekick.PlayerId);
            RPCProcedure.SidekickPromotes(MySidekick.PlayerId);
        }
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        // Jackal Sidekick Button
        JackalSidekickButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.JackalCreatesSidekick);
                sender.Write(Local.CurrentTarget.PlayerId);
                sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                RPCProcedure.JackalCreatesSidekick(Local.CurrentTarget.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
            },
            () => { return CanCreateSidekick && CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return CanCreateSidekick && Local.CurrentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { JackalSidekickButton.Timer = JackalSidekickButton.MaxTimer; },
            AssetLoader.SidekickButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("SidekickText")
        };

        // Jackal Kill
        JackalKillButton = new CustomButton(
            () =>
            {
                if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local.CurrentTarget) == MurderAttemptResult.SuppressKill) return;

                JackalKillButton.Timer = JackalKillButton.MaxTimer;
                Local.CurrentTarget = null;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return Local.CurrentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { JackalKillButton.Timer = JackalKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            new Vector3(0, 1f, 0),
            hm,
            hm.KillButton,
            KeyCode.Q
        );

        JackalSabotageLightsButton = new CustomButton(
                () =>
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
                },
                () =>
                {
                    return PlayerControl.LocalPlayer.IsRole(ERoleType.Jackal) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
                },
                () =>
                {
                    if (Helpers.SabotageTimer() > JackalSabotageLightsButton.Timer || Helpers.SabotageActive())
                    {
                        // this will give imps time to do another sabotage.
                        JackalSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                    }
                    return Helpers.CanUseSabotage();
                },
                () =>
                {
                    JackalSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
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
        JackalKillButton.MaxTimer = KillCooldown;
        JackalSidekickButton.MaxTimer = CreateSidekickCooldown;
    }

    // write functions here
    public static void RemoveCurrentJackal()
    {
        if (!FormerJackals.Any(x => x.PlayerId == Local.Player.PlayerId)) FormerJackals.Add(Local.Player);
    }

    public static void Clear()
    {
        // reset configs here
        FormerJackals = [];
        Players.Clear();
    }
}