namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Jackal : RoleBase<Jackal>
{
    public static Color NameColor = new Color32(0, 180, 235, byte.MaxValue);
    public override Color RoleColor => NameColor;
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
        StaticRoleType = CurrentRoleType = RoleType.Jackal;
        CanSidekick = CanCreateSidekick;
    }

    public override void OnUpdateNameColors()
    {
        var lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
            if (Sidekick.Exists)
            {
                var skPlayers = Sidekick.Players;
                if (skPlayers.Count > 0)
                {
                    var sk = skPlayers[0];
                    if (sk != null) Update.SetPlayerNameColor(sk.Player, RoleColor);
                }
            }
            if (FakeSidekick != null)
            {
                Update.SetPlayerNameColor(FakeSidekick, RoleColor);
            }
        }
        else if (lp.IsTeamImpostor() && WasTeamRed)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
        }
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            var untargetablePlayers = new List<PlayerControl>();
            if (CanCreateSidekickFromImpostor)
            {
                // Only exclude sidekick from being targeted if the jackal can create sidekicks from impostors
                if (Sidekick.Exists)
                {
                    var skPlayers = Sidekick.AllPlayers;
                    for (var i = 0; i < skPlayers.Count; i++)
                    {
                        untargetablePlayers.Add(skPlayers[i]);
                    }
                }
            }
            var miniPlayers = Mini.Players;
            for (var i = 0; i < miniPlayers.Count; i++)
            {
                var mini = miniPlayers[i];
                if (!Mini.IsGrownUp(mini.Player))
                {
                    untargetablePlayers.Add(mini.Player);
                }
            }
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
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SidekickPromotes);
            sender.Write(MySidekick.PlayerId);
            RPCProcedure.SidekickPromotes(MySidekick.PlayerId);
        }
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        JackalKillButton = new CustomButton(
            () =>
            {
                var local = Local;
                if (local == null) return;
                if (Helpers.CheckMurderAttemptAndKill(local.Player, local.CurrentTarget) == MurderAttemptResult.SuppressKill) return;

                JackalKillButton?.Timer = JackalKillButton.MaxTimer;
                local.CurrentTarget = null;
            },
            () => { return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                var local = Local;
                return local != null && local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { JackalKillButton?.Timer = JackalKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary
        );

        // Jackal Sidekick Button
        JackalSidekickButton = new CustomButton(
            () =>
            {
                var local = Local;
                if (local == null || local.CurrentTarget == null) return;

                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.JackalCreatesSidekick);
                sender.Write(local.CurrentTarget.PlayerId);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.JackalCreatesSidekick(local.CurrentTarget.PlayerId, PlayerControl.LocalPlayer.PlayerId);
            },
            () =>
            {
                var local = Local;
                return local != null && local.CanSidekick && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var local = Local;
                return local != null && local.CanSidekick && local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { JackalSidekickButton?.Timer = JackalSidekickButton.MaxTimer; },
            AssetLoader.SidekickButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilitySecondary,
            false,
            Tr.Get("Hud.SidekickText")
        );

        JackalSabotageLightsButton = new CustomButton(
            () =>
            {
                ShipStatus.Instance?.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () =>
            {
                return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                if (JackalSabotageLightsButton == null) return false;
                if (Helpers.SabotageTimer() > JackalSabotageLightsButton.Timer || Helpers.SabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    JackalSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                }
                return Helpers.CanUseSabotage();
            },
            () =>
            {
                JackalSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
            },
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.AbilityButton,
            AbilitySlot.CommonAbilitySecondary
        );
    }
    public static void SetButtonCooldowns()
    {
        JackalKillButton?.MaxTimer = KillCooldown;
        JackalSidekickButton?.MaxTimer = CreateSidekickCooldown;
    }

    // write functions here
    public static void RemoveCurrentJackal()
    {
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            var p = Players[i].Player;
            bool alreadyFormer = false;
            for (int j = 0; j < FormerJackals.Count; j++)
            {
                if (FormerJackals[j].PlayerId == p.PlayerId)
                {
                    alreadyFormer = true;
                    break;
                }
            }
            if (!alreadyFormer) FormerJackals.Add(p);
            p.EraseRole(RoleType.Jackal);
        }
    }

    public static void Clear()
    {
        // reset configs here
        FormerJackals = [];
        Players.Clear();
    }
}