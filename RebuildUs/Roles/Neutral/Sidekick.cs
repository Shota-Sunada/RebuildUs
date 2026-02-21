namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Sidekick : RoleBase<Sidekick>
{
    // write configs here
    internal static CustomButton SidekickKillButton;
    internal static CustomButton SidekickSabotageLightsButton;
    internal PlayerControl CurrentTarget;
    internal bool WasImpostor = false;
    internal bool WasSpy = false;
    internal bool WasTeamRed = false;

    public Sidekick()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Sidekick;
    }

    internal override Color RoleColor
    {
        get => Jackal.NameColor;
    }

    internal static bool CanKill { get => CustomOptionHolder.SidekickCanKill.GetBool(); }
    internal static bool CanUseVents { get => CustomOptionHolder.SidekickCanUseVents.GetBool(); }
    internal static bool CanSabotageLights { get => CustomOptionHolder.SidekickCanSabotageLights.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SidekickHasImpostorVision.GetBool(); }
    internal static bool PromotesToJackal { get => CustomOptionHolder.SidekickPromotesToJackal.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
            if (Jackal.Exists)
            {
                List<Jackal> jkPlayers = Jackal.Players;
                if (jkPlayers.Count > 0)
                {
                    Jackal jk = jkPlayers[0];
                    if (jk != null) Update.SetPlayerNameColor(jk.Player, RoleColor);
                }
            }
        }
        else if (lp.IsTeamImpostor() && WasTeamRed) Update.SetPlayerNameColor(Player, RoleColor);
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Sidekick local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetablePlayers = new();
            if (Jackal.Exists)
            {
                List<PlayerControl> jkPlayers = Jackal.AllPlayers;
                for (int i = 0; i < jkPlayers.Count; i++) untargetablePlayers.Add(jkPlayers[i]);
            }

            List<Mini> miniPlayers = Mini.Players;
            for (int i = 0; i < miniPlayers.Count; i++)
            {
                Mini mini = miniPlayers[i];
                if (!Mini.IsGrownUp(mini.Player)) untargetablePlayers.Add(mini.Player);
            }

            CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
            if (CanKill) Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        SidekickKillButton = new(() =>
        {
            if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local.CurrentTarget) == MurderAttemptResult.SuppressKill) return;
            SidekickKillButton.Timer = SidekickKillButton.MaxTimer;
            Local.CurrentTarget = null;
        }, () => { return CanKill && PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove; }, () => { SidekickKillButton.Timer = SidekickKillButton.MaxTimer; }, hm.KillButton.graphic.sprite, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        SidekickSabotageLightsButton = new(() =>
        {
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
        }, () =>
        {
            return PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
        }, () =>
        {
            if (Helpers.SabotageTimer() > SidekickSabotageLightsButton.Timer || Helpers.SabotageActive())
            {
                // this will give imps time to do another sabotage.
                SidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
            }

            return Helpers.CanUseSabotage();
        }, () =>
        {
            SidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
        }, AssetLoader.LightsOutButton, ButtonPosition.Layout, hm, hm.AbilityButton, AbilitySlot.CommonAbilitySecondary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights));
    }

    internal static void SetButtonCooldowns()
    {
        SidekickKillButton.MaxTimer = Jackal.KillCooldown;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}