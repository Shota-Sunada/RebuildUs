namespace RebuildUs.Modules;

internal static class ShortcutCommands
{
    private static bool _timerBarDebugEnabled;
    private static bool _timerBarDebugHooksInstalled;
    private static int _timerBarCreateCount;
    private static int _timerBarUpdateCount;
    private static int _timerBarUpdateTimerCount;
    private static int _timerBarFinalHideCount;
    private static int _timerBarTaskCompleteCount;

    internal static void HostCommands()
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F5) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F5)) GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ForceEnd, false);

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F6) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F6)) && MeetingHud.Instance && Helpers.GameStarted) MeetingHud.Instance.RpcClose();

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F7) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F7)) && !MeetingHud.Instance && Helpers.GameStarted) MapUtilities.CachedShipStatus.StartMeeting(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data);

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Helpers.IsCountdown)
        {
            GameStartManager.Instance.countDownTimer = 0;
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }

#if DEBUG
        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F4) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F4))
        {
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Reloaded Random Number Generation Algorithm.");
            RebuildUs.RefreshRnd((int)DateTime.Now.Ticks);
        }

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F8) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F8))
        {
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Measured Random Number Quality. Check logs for details.");
            RandomMain.LogScore();
        }
#endif
    }

    internal static void DebugCommands()
    {
        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F10) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F10))
            ToggleTimerBarDebug();

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F11) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F11))
            DumpTimerBarDebugStats();

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F12) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F12))
            DecreaseTimerBarDebugValue();

        bool triggerDeathPopup = Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F9) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F9);
        if (!triggerDeathPopup) return;

        HudManager hud = HudManager.Instance;
        PlayerControl localPlayer = PlayerControl.LocalPlayer;

        if (localPlayer == null || hud?.Chat == null) return;
        if (!Helpers.GameStarted)
        {
            Logger.LogInfo("DeathPopup debug failed: game is not started.");
            return;
        }

        int result = DeathPopup.TryShow(localPlayer, out HideAndSeekDeathPopup popup);
        string reason = DeathPopup.ExplainResult(result);
        if (result != DeathPopup.RESULT_SUCCESS)
        {
            Logger.LogInfo($"DeathPopup debug result={result} ({reason})");
            return;
        }

        if (popup == null)
        {
            Logger.LogInfo("DeathPopup debug success via fallback path (popup instance unavailable for text verification).");
            return;
        }

        Logger.LogInfo("DeathPopup debug success.");
    }

    internal static void OpenAirshipToilet()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 79);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 80);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 81);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 82);
    }

    private static void ToggleTimerBarDebug()
    {
        if (!Helpers.GameStarted)
        {
            Logger.LogInfo("TimerBar debug failed: game is not started.");
            return;
        }

        EnsureTimerBarDebugHooks();

        _timerBarDebugEnabled = !_timerBarDebugEnabled;
        if (_timerBarDebugEnabled)
        {
            TimerBarManager.Settings.Enabled = true;
            TimerBarManager.Settings.OverrideUpdate = true;
            TimerBarManager.Settings.OverrideUpdateTimer = true;
            TimerBarManager.Settings.OverrideStartFinalHide = true;
            TimerBarManager.Settings.OverrideTaskComplete = true;
            TimerBarManager.Settings.LerpSpeed = 18f;
            TimerBarManager.Settings.ChunkFreezeSeconds = 0.2f;
            TimerBarManager.Settings.RootOffset = new(0f, -0.5f, 0f);
            TimerBarManager.Settings.NormalBarColor = new(0.2f, 0.85f, 1f, 1f);
            TimerBarManager.Settings.FinalBarColor = new(1f, 0.25f, 0.25f, 1f);
            TimerBarManager.Settings.ChunkBarColor = new(1f, 0.95f, 0.4f, 1f);
            TimerBarManager.Settings.TimeTextColor = Color.white;
            TimerBarManager.Settings.TimeFormatter = (time, _, isFinal) =>
            {
                int seconds = Mathf.CeilToInt(Mathf.Max(0f, time));
                return isFinal ? $"FINAL {seconds}s" : $"ESCAPE {seconds}s";
            };

            TimerBarManager.CustomTimer.MinValue = 0f;
            TimerBarManager.CustomTimer.MaxValue = 90f;
            TimerBarManager.CustomTimer.FinalStartsAtMinValue = true;
            TimerBarManager.CustomTimer.UseSeparateFinalBarRange = true;
            TimerBarManager.CustomTimer.FinalBarMinValue = 0f;
            TimerBarManager.CustomTimer.FinalBarMaxValue = 15f;
            TimerBarManager.CustomTimer.DecreasePerSecond = 1f;
            TimerBarManager.CustomTimer.FinalStartThreshold = 15f;
            TimerBarManager.CustomTimer.TaskCompletePulseInterval = 7f;
            TimerBarManager.CustomTimer.MinReachedBehavior = TimerMinReachedBehavior.ResetToMax;
            TimerBarManager.CustomTimer.FinalCondition = null;
            TimerBarManager.CustomTimer.OnMinReached = ctx =>
            {
                Logger.LogInfo(
                               $"TimerBar custom timer reached min. current={ctx.CurrentValue:F2}, min={ctx.MinValue:F2}, max={ctx.MaxValue:F2}, isFinal={ctx.IsFinalCountdown}, behavior={TimerBarManager.CustomTimer.MinReachedBehavior}");
            };

            TimerBarManager.StartCustomTimer();

            Logger.LogInfo("TimerBar debug enabled (works in non-HnS too). (Ctrl+F11 to dump stats)");
            return;
        }

        TimerBarManager.StopCustomTimer(hideBar: true);
        TimerBarManager.Settings.Reset();
        Logger.LogInfo("TimerBar debug disabled.");
    }

    private static void DumpTimerBarDebugStats()
    {
        Logger.LogInfo(
                       $"TimerBar stats create={_timerBarCreateCount}, update={_timerBarUpdateCount}, updateTimer={_timerBarUpdateTimerCount}, finalHide={_timerBarFinalHideCount}, taskComplete={_timerBarTaskCompleteCount}, debugEnabled={_timerBarDebugEnabled}");
        Logger.LogInfo(
                       $"TimerBar custom settings min={TimerBarManager.CustomTimer.MinValue}, max={TimerBarManager.CustomTimer.MaxValue}, finalAtMin={TimerBarManager.CustomTimer.FinalStartsAtMinValue}, finalMin={TimerBarManager.CustomTimer.FinalBarMinValue}, finalMax={TimerBarManager.CustomTimer.FinalBarMaxValue}, decreasePerSec={TimerBarManager.CustomTimer.DecreasePerSecond}, minBehavior={TimerBarManager.CustomTimer.MinReachedBehavior}");
    }

    private static void EnsureTimerBarDebugHooks()
    {
        if (_timerBarDebugHooksInstalled) return;
        _timerBarDebugHooksInstalled = true;

        TimerBarManager.BarCreated += _ => _timerBarCreateCount++;
        TimerBarManager.BeforeUpdate += _ => _timerBarUpdateCount++;
        TimerBarManager.BeforeUpdateTimer += (_, _, _) => _timerBarUpdateTimerCount++;
        TimerBarManager.BeforeStartFinalHide += _ => _timerBarFinalHideCount++;
        TimerBarManager.BeforeTaskComplete += _ => _timerBarTaskCompleteCount++;
    }

    private static void DecreaseTimerBarDebugValue()
    {
        if (!_timerBarDebugEnabled)
        {
            Logger.LogInfo("TimerBar debug value decrease skipped: debug is disabled.");
            return;
        }

        const float debugDecreaseValue = 5f;
        bool ok = TimerBarManager.DecreaseCustomTimer(debugDecreaseValue);
        if (ok) Logger.LogInfo($"TimerBar debug value decreased by {debugDecreaseValue} (Ctrl+F12).");
        else Logger.LogInfo("TimerBar debug value decrease skipped: custom timer is not running.");
    }
}