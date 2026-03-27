namespace RebuildUs.Modules.CustomGameModes;

internal class BattleRoyaleMode : GameModeBase
{
    public override CustomGamemode Gamemode => CustomGamemode.BattleRoyale;

    // private static CustomButton _searchButton;
    internal static Color BattleRoyaleColor = new Color32(87, 249, 132, 255);

    // Meetings disabled in Battle Royale
    public override bool CanCallMeeting => false;
    public override bool UseTimerBar => true;
    public override Color GameModeColor => BattleRoyaleColor;
    public override bool SkipShowRole => true;

    internal static bool IsTimeUp = false;

    // internal static bool ButtonUsed = false;
    // internal static int CommonUsageLeft = 0;

    internal static float KillCooldown { get { return CustomOptionHolder.BattleRoyaleKillCooldown.GetFloat(); } }

    public override void AssignRoles()
    {
        // Add battle royale specific role assignment here
        // e.g., Set everyone to a Survivor role
    }

    public override void OnPlayerUpdate(PlayerControl player)
    {
        // Custom update logic for Battle Royale
    }

    public override bool OnBeginIntro(IntroCutscene instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        instance.TeamTitle.text = Tr.Get(TrKey.GameModeBattleRoyale);
        instance.TeamTitle.color = GameModeColor;
        instance.ImpostorText.gameObject.SetActive(true);
        instance.ImpostorText.text = Tr.Get(TrKey.GameModeBattleRoyaleBlurb);
        instance.ImpostorText.color = Color.white;
        instance.BackgroundBar.material.color = GameModeColor;

        return true; // We handled intro text
    }

    public override bool OnSetupRole(IntroCutscene instance)
    {
        return true;
    }

    public override void OnKill(PlayerControl target)
    {
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShowDeathPopup);
        sender.Write(target.PlayerId);
        RPCProcedure.ShowDeathPopup(target.PlayerId);
        PlayerControl.LocalPlayer.SetKillTimer(KillCooldown);
    }

    public override void OnTimerBarUpdate(float deltaTime) { }

    public override void OnIntroDestroyed()
    {
        Initialize();
    }

    internal static bool CheckAndEndGameForBattleRoyaleLastOne(PlayerStatistics statistics)
    {
        if (statistics.TotalAlive <= 1)
        {
            EndGameMain.UncheckedEndGame(CustomGameOverReason.BattleRoyaleLastOneStanding);
            return true;
        }

        return false;
    }

    internal static bool CheckAndEndGameForBattleRoyaleTimeUp()
    {
        if (IsTimeUp)
        {
            EndGameMain.UncheckedEndGame(CustomGameOverReason.BattleRoyaleTimeUp);
            return true;
        }

        return false;
    }

    private static void Initialize()
    {
        TimerBarManager.Settings.Enabled = true;
        TimerBarManager.Settings.OverrideUpdate = true;
        TimerBarManager.Settings.OverrideUpdateTimer = true;
        TimerBarManager.Settings.OverrideStartFinalHide = true;
        TimerBarManager.Settings.OverrideTaskComplete = true;
        TimerBarManager.Settings.LerpSpeed = 18f;
        TimerBarManager.Settings.ChunkFreezeSeconds = 0.2f;
        TimerBarManager.Settings.RootOffset = new(0f, 0f, 0f);
        TimerBarManager.Settings.NormalBarColor = Palette.AcceptedGreen;
        TimerBarManager.Settings.FinalBarColor = Palette.ImpostorRed;
        TimerBarManager.Settings.ChunkBarColor = Palette.CrewmateBlue;
        TimerBarManager.Settings.TimeTextColor = Color.white;
        TimerBarManager.Settings.TimeFormatter = (time, maxTime, isFinal) =>
        {
            var seconds = Mathf.CeilToInt(Mathf.Max(0f, time));
            return string.Format("{0}: {1}:{2:D2}", Tr.Get(TrKey.GameModeTimeLeft), Mathf.FloorToInt(seconds / 60f), seconds % 60);
        };

        TimerBarManager.CustomTimer.MinValue = 0f;
        TimerBarManager.CustomTimer.MaxValue = CustomOptionHolder.BattleRoyaleTimeLimit.GetFloat();
        TimerBarManager.CustomTimer.FinalStartsAtMinValue = true;
        TimerBarManager.CustomTimer.UseSeparateFinalBarRange = true;
        TimerBarManager.CustomTimer.FinalBarMinValue = 0f;
        TimerBarManager.CustomTimer.FinalBarMaxValue = 15f;
        TimerBarManager.CustomTimer.DecreasePerSecond = 1f;
        TimerBarManager.CustomTimer.FinalStartThreshold = 15f;
        TimerBarManager.CustomTimer.TaskCompletePulseInterval = 7f;
        TimerBarManager.CustomTimer.MinReachedBehavior = TimerMinReachedBehavior.Stop;
        TimerBarManager.CustomTimer.FinalCondition = null;
        TimerBarManager.CustomTimer.OnMinReached = ctx =>
        {
            IsTimeUp = true;
            Logger.LogInfo("[BattleRoyaleMode] Time's up!");
        };

        TimerBarManager.StartCustomTimer();

        // MakeButtons(FastDestroyableSingleton<HudManager>.Instance);

        // _searchButton.Timer = _searchButton.MaxTimer = CustomOptionHolder.BattleRoyaleButtonCooldown.GetFloat();
        // CommonUsageLeft = Mathf.CeilToInt(CustomOptionHolder.BattleRoyaleButtonUsage.GetFloat());
    }

    // internal static void MakeButtons(HudManager hm)
    // {
    //     _searchButton = new(
    //         () =>
    //         {
    //             using var _ = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.BattleRoyaleSearchPlayers);
    //             RPCProcedure.BattleRoyaleSearchPlayers();

    //             foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
    //             {
    //                 if (player.IsAlive() && player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
    //                 {
    //                     var arrow = new Arrow(BattleRoyaleColor);
    //                     hm.StartCoroutine(Effects.Lerp(5f, new Action<float>(_ =>
    //                     {
    //                         arrow.ArrowObject.Destroy();
    //                     })));
    //                 }
    //             }

    //             ButtonUsed = true;
    //         },
    //         () =>
    //         {
    //             return PlayerControl.LocalPlayer.IsAlive() && GameModeManager.CurrentGameMode == CustomGamemode.BattleRoyale && !ButtonUsed && CommonUsageLeft > 0;
    //         },
    //         () =>
    //         {
    //             return PlayerControl.LocalPlayer.CanMove;
    //         },
    //         () => { },
    //         AssetLoader.EmergencyButton,
    //         ButtonPosition.Layout,
    //         hm,
    //         hm.UseButton,
    //         AbilitySlot.CommonAbilityPrimary,
    //         true,
    //         Tr.Get(TrKey.SearchText));
    // }

    // internal static void ResetButtonCooldown()
    // {
    //     _searchButton.Timer = CustomOptionHolder.BattleRoyaleButtonCooldown.GetFloat();
    // }
}
