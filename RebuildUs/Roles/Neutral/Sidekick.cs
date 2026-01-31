namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Sidekick : RoleBase<Sidekick>
{
    public override Color RoleColor => Jackal.NameColor;

    // write configs here
    public static CustomButton SidekickKillButton;
    public static CustomButton SidekickSabotageLightsButton;
    public PlayerControl CurrentTarget;
    public bool WasTeamRed = false;
    public bool WasImpostor = false;
    public bool WasSpy = false;

    public override void OnUpdateNameColors()
    {
        var lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
            if (Jackal.Exists)
            {
                var jkPlayers = Jackal.Players;
                if (jkPlayers.Count > 0)
                {
                    var jk = jkPlayers[0];
                    if (jk != null) Update.SetPlayerNameColor(jk.Player, RoleColor);
                }
            }
        }
        else if (lp.IsTeamImpostor() && WasTeamRed)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
        }
    }

    public static bool CanKill { get { return CustomOptionHolder.SidekickCanKill.GetBool(); } }
    public static bool CanUseVents { get { return CustomOptionHolder.SidekickCanUseVents.GetBool(); } }
    public static bool CanSabotageLights { get { return CustomOptionHolder.SidekickCanSabotageLights.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.SidekickHasImpostorVision.GetBool(); } }
    public static bool PromotesToJackal { get { return CustomOptionHolder.SidekickPromotesToJackal.GetBool(); } }

    public Sidekick()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Sidekick;
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
            if (Jackal.Exists)
            {
                var jkPlayers = Jackal.AllPlayers;
                for (var i = 0; i < jkPlayers.Count; i++)
                {
                    untargetablePlayers.Add(jkPlayers[i]);
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
            () => { return CanKill && PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { SidekickKillButton.Timer = SidekickKillButton.MaxTimer; },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel)
        );

        SidekickSabotageLightsButton = new CustomButton(
            () =>
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
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
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.AbilityButton,
            AbilitySlot.CommonAbilitySecondary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights)
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