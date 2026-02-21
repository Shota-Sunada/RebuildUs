namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Sidekick : SingleRoleBase<Sidekick>
{
    // write configs here
    private static CustomButton _sidekickKillButton;
    private static CustomButton _sidekickSabotageLightsButton;
    private PlayerControl _currentTarget;
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

    private static bool CanKill { get => CustomOptionHolder.SidekickCanKill.GetBool(); }
    internal static bool CanUseVents { get => CustomOptionHolder.SidekickCanUseVents.GetBool(); }
    private static bool CanSabotageLights { get => CustomOptionHolder.SidekickCanSabotageLights.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SidekickHasImpostorVision.GetBool(); }
    internal static bool PromotesToJackal { get => CustomOptionHolder.SidekickPromotesToJackal.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
            if (!Jackal.Exists) return;

            if (Jackal.Local.Player) HudManagerPatch.SetPlayerNameColor(Jackal.Local.Player, RoleColor);
        }
        else if (lp.IsTeamImpostor() && WasTeamRed) HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Sidekick local = Local;
        if (local == null) return;
        List<PlayerControl> untargetablePlayers = [];
        if (Jackal.Exists)
        {
            untargetablePlayers.Add(Jackal.PlayerControl);
        }

        List<Mini> miniPlayers = Mini.Players;
        foreach (var mini in miniPlayers)
        {
            if (!Mini.IsGrownUp(mini.Player)) untargetablePlayers.Add(mini.Player);
        }

        _currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
        if (CanKill) Helpers.SetPlayerOutline(_currentTarget, Palette.ImpostorRed);
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _sidekickKillButton = new(() =>
                                  {
                                      if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local._currentTarget) == MurderAttemptResult.SuppressKill) return;
                                      _sidekickKillButton.Timer = _sidekickKillButton.MaxTimer;
                                      Local._currentTarget = null;
                                  },
                                  () => CanKill && PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && PlayerControl.LocalPlayer.IsAlive(),
                                  () => Local._currentTarget && PlayerControl.LocalPlayer.CanMove, () => { _sidekickKillButton.Timer = _sidekickKillButton.MaxTimer; }, hm.KillButton.graphic.sprite, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        _sidekickSabotageLightsButton = new(() =>
                                            {
                                                MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
                                            },
                                            () => PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive(), () =>
                                            {
                                                if (Helpers.SabotageTimer() > _sidekickSabotageLightsButton.Timer || Helpers.SabotageActive())
                                                {
                                                    // this will give imps time to do another sabotage.
                                                    _sidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                                                }

                                                return Helpers.CanUseSabotage();
                                            }, () =>
                                            {
                                                _sidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                                            }, AssetLoader.LightsOutButton, ButtonPosition.Layout, hm, hm.AbilityButton, AbilitySlot.CommonAbilitySecondary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights));
    }

    internal static void SetButtonCooldowns()
    {
        _sidekickKillButton.MaxTimer = Jackal.KillCooldown;
    }

    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}