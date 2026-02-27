using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

internal static class TimerBarManager
{
    private static readonly FieldInfo TimerBarPrefabField = AccessTools.Field(typeof(HideAndSeekManager), "TimerBarPrefab");
    private static readonly Dictionary<int, TimerBar> ActiveBars = [];
    private static HideAndSeekTimerBar _standaloneBar;
    private static bool _standaloneFinalTriggered;
    private static float _customTimerValue;
    private static bool _customTimerRunning;
    private static bool _customTimerReachedMin;
    private static float _customTimerPulseElapsed;

    internal static TimerBarSettings Settings { get; } = new();
    internal static CustomTimerSettings CustomTimer { get; } = new();

    internal static event Action<TimerBar> BarCreated;
    internal static event Action<TimerBar> BeforeUpdate;
    internal static event Action<TimerBar, float, float> BeforeUpdateTimer;
    internal static event Action<TimerBar> BeforeStartFinalHide;
    internal static event Action<TimerBar> BeforeTaskComplete;

    internal static void Clear()
    {
        ActiveBars.Clear();
        DestroyStandaloneBar();
        StopCustomTimer(hideBar: false);
    }

    internal static bool EnsureStandaloneBar()
    {
        if (_standaloneBar != null)
        {
            return true;
        }

        HideAndSeekTimerBar prefab = ResolveTimerBarPrefab();
        HudManager hud = DestroyableSingleton<HudManager>.Instance;
        if (prefab == null || hud == null)
        {
            return false;
        }

        Transform parent = hud.transform.parent ?? hud.transform;
        _standaloneBar = Object.Instantiate(prefab, parent);
        _standaloneFinalTriggered = false;
        return _standaloneBar != null;
    }

    internal static void DestroyStandaloneBar()
    {
        _standaloneBar?.gameObject.Destroy();

        _standaloneBar = null;
        _standaloneFinalTriggered = false;
    }

    internal static bool UpdateStandalone(float time, float maxTime, bool isFinalCountdown, bool pulseTaskComplete)
    {
        if (!EnsureStandaloneBar())
        {
            return false;
        }

        if (pulseTaskComplete)
        {
            TaskComplete(_standaloneBar);
        }

        if (isFinalCountdown && !_standaloneFinalTriggered)
        {
            StartFinalHide(_standaloneBar);
            _standaloneFinalTriggered = true;
        }
        else if (!isFinalCountdown)
        {
            _standaloneFinalTriggered = false;
        }

        UpdateTimer(_standaloneBar, time, maxTime);
        return true;
    }

    internal static void StartCustomTimer(float? startValue = null)
    {
        _customTimerRunning = true;
        _customTimerReachedMin = false;
        _customTimerPulseElapsed = 0f;
        _customTimerValue = Mathf.Clamp(startValue ?? CustomTimer.MaxValue, CustomTimer.MinValue, CustomTimer.MaxValue);

        bool isFinal = EvaluateFinalState(_customTimerValue);
        CustomTimer.OnStarted?.Invoke(new(_customTimerValue, CustomTimer.MinValue, CustomTimer.MaxValue, isFinal, _customTimerRunning));
        RenderCustomTimer(isFinal, false);
    }

    internal static bool DecreaseCustomTimer(float amount)
    {
        if (amount <= 0f)
        {
            return false;
        }
        if (!_customTimerRunning)
        {
            return false;
        }

        _customTimerValue = Mathf.Clamp(_customTimerValue - amount, CustomTimer.MinValue, CustomTimer.MaxValue);
        bool isFinal = EvaluateFinalState(_customTimerValue);

        bool reachedMin = _customTimerValue <= CustomTimer.MinValue;
        if (reachedMin && !_customTimerReachedMin)
        {
            _customTimerReachedMin = true;
            HandleCustomTimerMinReached(isFinal);
            isFinal = EvaluateFinalState(_customTimerValue);
        }
        else if (!reachedMin)
        {
            _customTimerReachedMin = false;
        }

        RenderCustomTimer(isFinal, true);
        CustomTimer.OnTick?.Invoke(new(_customTimerValue, CustomTimer.MinValue, CustomTimer.MaxValue, isFinal, _customTimerRunning));
        return true;
    }

    internal static void StopCustomTimer(bool hideBar)
    {
        _customTimerRunning = false;
        _customTimerReachedMin = false;
        _customTimerPulseElapsed = 0f;
        if (hideBar)
        {
            DestroyStandaloneBar();
        }
    }

    internal static void TickCustomTimer(float deltaTime)
    {
        if (!_customTimerRunning)
        {
            return;
        }
        if (!Helpers.GameStarted)
        {
            return;
        }
        if (!CustomTimer.RunInHideAndSeek && Helpers.IsHideNSeekMode)
        {
            return;
        }
        if (!CustomTimer.RunInNormalMode && !Helpers.IsHideNSeekMode)
        {
            return;
        }

        _customTimerValue -= Mathf.Max(0f, CustomTimer.DecreasePerSecond) * Mathf.Max(0f, deltaTime);
        _customTimerValue = Mathf.Clamp(_customTimerValue, CustomTimer.MinValue, CustomTimer.MaxValue);

        bool reachedMin = _customTimerValue <= CustomTimer.MinValue;
        bool isFinal = EvaluateFinalState(_customTimerValue);

        if (reachedMin && !_customTimerReachedMin)
        {
            _customTimerReachedMin = true;
            HandleCustomTimerMinReached(isFinal);
            isFinal = EvaluateFinalState(_customTimerValue);
        }
        else if (!reachedMin)
        {
            _customTimerReachedMin = false;
        }

        _customTimerPulseElapsed += Mathf.Max(0f, deltaTime);
        bool pulseTaskComplete = false;
        if (CustomTimer.TaskCompletePulseInterval > 0f && _customTimerPulseElapsed >= CustomTimer.TaskCompletePulseInterval)
        {
            _customTimerPulseElapsed = 0f;
            pulseTaskComplete = true;
        }

        RenderCustomTimer(isFinal, pulseTaskComplete);
        CustomTimer.OnTick?.Invoke(new(_customTimerValue, CustomTimer.MinValue, CustomTimer.MaxValue, isFinal, _customTimerRunning));
    }

    internal static bool Update(HideAndSeekTimerBar instance)
    {
        if (!Settings.Enabled || !Settings.OverrideUpdate)
        {
            return true;
        }
        if (!TryGetBar(instance, out TimerBar timerBar))
        {
            return true;
        }

        try
        {
            BeforeUpdate?.Invoke(timerBar);
            return !timerBar.CustomUpdate(Settings);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[TimerBarManager] Update error: {ex}");
            return true;
        }
    }

    internal static bool UpdateTimer(HideAndSeekTimerBar instance, float time, float maxTime)
    {
        if (!Settings.Enabled || !Settings.OverrideUpdateTimer)
        {
            return true;
        }
        if (!TryGetBar(instance, out TimerBar timerBar))
        {
            return true;
        }

        try
        {
            BeforeUpdateTimer?.Invoke(timerBar, time, maxTime);
            return !timerBar.CustomUpdateTimer(time, maxTime, Settings);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[TimerBarManager] UpdateTimer error: {ex}");
            return true;
        }
    }

    internal static bool StartFinalHide(HideAndSeekTimerBar instance)
    {
        if (!Settings.Enabled || !Settings.OverrideStartFinalHide)
        {
            return true;
        }
        if (!TryGetBar(instance, out TimerBar timerBar))
        {
            return true;
        }

        try
        {
            BeforeStartFinalHide?.Invoke(timerBar);
            return !timerBar.CustomStartFinalHide(Settings);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[TimerBarManager] StartFinalHide error: {ex}");
            return true;
        }
    }

    internal static bool TaskComplete(HideAndSeekTimerBar instance)
    {
        if (!Settings.Enabled || !Settings.OverrideTaskComplete)
        {
            return true;
        }
        if (!TryGetBar(instance, out TimerBar timerBar))
        {
            return true;
        }

        try
        {
            BeforeTaskComplete?.Invoke(timerBar);
            return !timerBar.CustomTaskComplete(Settings);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[TimerBarManager] TaskComplete error: {ex}");
            return true;
        }
    }

    private static bool TryGetBar(HideAndSeekTimerBar instance, out TimerBar timerBar)
    {
        timerBar = null;
        if (instance == null)
        {
            return false;
        }

        PruneDeadBars();

        int instanceId = instance.GetInstanceID();
        if (ActiveBars.TryGetValue(instanceId, out timerBar))
        {
            return true;
        }

        timerBar = new(instance);
        ActiveBars[instanceId] = timerBar;
        BarCreated?.Invoke(timerBar);
        return true;
    }

    private static void PruneDeadBars()
    {
        if (ActiveBars.Count == 0)
        {
            return;
        }

        List<int> deadKeys = null;
        foreach (KeyValuePair<int, TimerBar> pair in ActiveBars)
        {
            if (pair.Value != null && pair.Value.IsAlive)
            {
                continue;
            }
            deadKeys ??= [];
            deadKeys.Add(pair.Key);
        }

        if (deadKeys == null)
        {
            return;
        }
        foreach (int key in deadKeys)
        {
            ActiveBars.Remove(key);
        }
    }

    private static void HandleCustomTimerMinReached(bool isFinal)
    {
        CustomTimer.OnMinReached?.Invoke(new(_customTimerValue, CustomTimer.MinValue, CustomTimer.MaxValue, isFinal, _customTimerRunning));

        switch (CustomTimer.MinReachedBehavior)
        {
            case TimerMinReachedBehavior.Stop:
                _customTimerValue = CustomTimer.MinValue;
                _customTimerRunning = false;
                break;

            case TimerMinReachedBehavior.ResetToMax:
                _customTimerValue = CustomTimer.MaxValue;
                _customTimerReachedMin = false;
                _customTimerPulseElapsed = 0f;
                break;

            case TimerMinReachedBehavior.KeepRunning:
            default:
                _customTimerValue = CustomTimer.MinValue;
                break;
        }
    }

    private static bool EvaluateFinalState(float currentValue)
    {
        if (CustomTimer.FinalStartsAtMinValue)
        {
            return currentValue <= CustomTimer.MinValue;
        }

        return CustomTimer.FinalCondition?.Invoke(currentValue, CustomTimer.MinValue, CustomTimer.MaxValue)
               ?? CustomTimerSettings.DefaultFinalCondition(currentValue,
                   CustomTimer.MinValue,
                   CustomTimer.MaxValue,
                   CustomTimer.FinalStartThreshold);
    }

    private static void RenderCustomTimer(bool isFinal, bool pulseTaskComplete)
    {
        float minValue = CustomTimer.MinValue;
        float maxValue = CustomTimer.MaxValue;
        if (isFinal && CustomTimer.UseSeparateFinalBarRange)
        {
            minValue = CustomTimer.FinalBarMinValue;
            maxValue = CustomTimer.FinalBarMaxValue;
        }

        if (maxValue < minValue)
        {
            (minValue, maxValue) = (maxValue, minValue);
        }

        float maxSpan = Mathf.Max(0.0001f, maxValue - minValue);
        float displayValue = Mathf.Clamp(_customTimerValue - minValue, 0f, maxSpan);
        UpdateStandalone(displayValue, maxSpan, isFinal, pulseTaskComplete);
    }

    private static HideAndSeekTimerBar ResolveTimerBarPrefab()
    {
        HideAndSeekManager fromCurrent = GameManager.Instance?.TryCast<HideAndSeekManager>();
        if (fromCurrent != null && TimerBarPrefabField != null)
        {
            HideAndSeekTimerBar prefab = TimerBarPrefabField.GetValue(fromCurrent) as HideAndSeekTimerBar;
            if (prefab != null)
            {
                return prefab;
            }
        }

        HideAndSeekManager fromCreator = GameManagerCreator.Instance?.HideAndSeekManagerPrefab;
        if (fromCreator != null && TimerBarPrefabField != null)
        {
            HideAndSeekTimerBar prefab = TimerBarPrefabField.GetValue(fromCreator) as HideAndSeekTimerBar;
            if (prefab != null)
            {
                return prefab;
            }
        }

        Object[] allBars = Resources.FindObjectsOfTypeAll(Il2CppType.Of<HideAndSeekTimerBar>());
        foreach (Object obj in allBars)
        {
            HideAndSeekTimerBar found = obj.TryCast<HideAndSeekTimerBar>();
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}

internal enum TimerMinReachedBehavior
{
    Stop,
    ResetToMax,
    KeepRunning,
}

internal readonly struct CustomTimerContext(float currentValue, float minValue, float maxValue, bool isFinalCountdown, bool isRunning)
{
    internal float CurrentValue { get; } = currentValue;
    internal float MinValue { get; } = minValue;
    internal float MaxValue { get; } = maxValue;
    internal bool IsFinalCountdown { get; } = isFinalCountdown;
    internal bool IsRunning { get; } = isRunning;
}

internal sealed class CustomTimerSettings
{
    internal float MinValue { get; set; } = 0f;
    internal float MaxValue { get; set; } = 90f;
    internal bool FinalStartsAtMinValue { get; set; } = true;
    internal bool UseSeparateFinalBarRange { get; set; } = true;
    internal float FinalBarMinValue { get; set; } = 0f;
    internal float FinalBarMaxValue { get; set; } = 15f;
    internal float DecreasePerSecond { get; set; } = 1f;
    internal float FinalStartThreshold { get; set; } = 15f;
    internal float TaskCompletePulseInterval { get; set; } = 7f;
    internal bool RunInNormalMode { get; set; } = true;
    internal bool RunInHideAndSeek { get; set; }
    internal TimerMinReachedBehavior MinReachedBehavior { get; set; } = TimerMinReachedBehavior.ResetToMax;

    // args: currentValue, minValue, maxValue
    internal Func<float, float, float, bool> FinalCondition { get; set; }
    internal Action<CustomTimerContext> OnStarted { get; set; }
    internal Action<CustomTimerContext> OnTick { get; set; }
    internal Action<CustomTimerContext> OnMinReached { get; set; }

    internal static bool DefaultFinalCondition(float currentValue, float minValue, float _, float threshold)
    {
        return currentValue <= minValue + threshold;
    }
}