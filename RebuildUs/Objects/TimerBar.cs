namespace RebuildUs.Objects;

internal sealed class TimerBar
{
    private static readonly FieldInfo TimeTextField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timeText");
    private static readonly FieldInfo TimerBarField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timerBar");
    private static readonly FieldInfo TimerBarRendererField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timerBarRenderer");
    private static readonly FieldInfo ChunkBarField = AccessTools.Field(typeof(HideAndSeekTimerBar), "chunkBar");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private readonly Vector3 _defaultChunkLocalPosition;
    private readonly Vector3 _defaultChunkLocalScale;
    private readonly Vector3 _defaultLocalPosition;
    private readonly Vector3 _defaultLocalScale;
    private readonly Vector3 _defaultTimerLocalPosition;
    private readonly Vector3 _defaultTimerLocalScale;

    private Material _chunkMaterial;
    private float _freezeChunkUntil;
    private float _targetBarSize;

    private Material _timerMaterial;

    internal TimerBar(HideAndSeekTimerBar source)
    {
        Source = source;
        TimeText = ResolveTimeText(source);
        TimerTransform = ResolveTimerTransform(source);
        TimerRenderer = ResolveTimerRenderer(source, TimerTransform);
        ChunkTransform = ResolveChunkTransform(source);
        ChunkRenderer = ResolveChunkRenderer(ChunkTransform);

        var root = source?.transform;
        _defaultLocalPosition = root != null ? root.localPosition : Vector3.zero;
        _defaultLocalScale = root != null ? root.localScale : Vector3.one;
        _defaultTimerLocalPosition = TimerTransform != null ? TimerTransform.localPosition : Vector3.zero;
        _defaultTimerLocalScale = TimerTransform != null ? TimerTransform.localScale : Vector3.one;
        _defaultChunkLocalPosition = ChunkTransform != null ? ChunkTransform.localPosition : Vector3.zero;
        _defaultChunkLocalScale = ChunkTransform != null ? ChunkTransform.localScale : Vector3.one;
        _targetBarSize = TimerTransform != null ? TimerTransform.localScale.x : 1f;
    }

    internal HideAndSeekTimerBar Source { get; }

    internal bool IsAlive
    {
        get => Source != null;
    }

    internal bool IsFinalCountdown { get; set; }

    internal float TargetBarSize
    {
        get => _targetBarSize;
        set => _targetBarSize = Mathf.Clamp01(value);
    }

    internal Transform RootTransform
    {
        get => Source?.transform;
    }

    internal Transform TimerTransform { get; }

    internal Transform ChunkTransform { get; }

    internal TextMeshPro TimeText { get; }

    internal MeshRenderer TimerRenderer { get; }

    internal MeshRenderer ChunkRenderer { get; }

    internal bool CustomUpdate(TimerBarSettings settings)
    {
        if (Source == null)
        {
            return false;
        }
        if (TimerTransform == null && ChunkTransform == null)
        {
            return false;
        }

        ApplyRootTransform(settings);
        ApplyCustomColors(settings);

        var lerp = Mathf.Clamp01(Time.deltaTime * Mathf.Max(0f, settings.LerpSpeed));
        if (TimerTransform != null)
        {
            var timerScale = TimerTransform.localScale;
            var targetScaleX = _defaultTimerLocalScale.x * _targetBarSize;
            timerScale.x = Mathf.Lerp(timerScale.x, targetScaleX, lerp);
            TimerTransform.localScale = timerScale;
            ApplyRightToLeftShrink(TimerTransform, _defaultTimerLocalPosition, _defaultTimerLocalScale, timerScale, settings);
        }

        if (ChunkTransform != null && Time.time >= _freezeChunkUntil)
        {
            var chunkScale = ChunkTransform.localScale;
            var targetScaleX = _defaultChunkLocalScale.x * _targetBarSize;
            chunkScale.x = Mathf.Lerp(chunkScale.x, targetScaleX, lerp);
            ChunkTransform.localScale = chunkScale;
            if (settings.ShrinkChunkFromRightToLeft)
            {
                ApplyRightToLeftShrink(ChunkTransform, _defaultChunkLocalPosition, _defaultChunkLocalScale, chunkScale, settings);
            }
        }

        if (ChunkTransform != null && settings.AlignChunkWithTimerX && TimerTransform != null)
        {
            AlignChunkLeftEdgeWithTimer(ChunkTransform, TimerTransform);
        }

        return true;
    }

    internal bool CustomUpdateTimer(float time, float maxTime, TimerBarSettings settings)
    {
        if (Source == null)
        {
            return false;
        }
        if (TimeText == null && TimerTransform == null && ChunkTransform == null)
        {
            return false;
        }

        var progress = settings.ProgressResolver?.Invoke(time, maxTime, IsFinalCountdown)
                         ?? TimerBarSettings.DefaultProgressResolver(time, maxTime, IsFinalCountdown);
        if (float.IsNaN(progress) || float.IsInfinity(progress))
        {
            progress = 0f;
        }
        _targetBarSize = Mathf.Clamp01(progress);

        if (TimeText != null)
        {
            var text = settings.TimeFormatter?.Invoke(time, maxTime, IsFinalCountdown)
                          ?? TimerBarSettings.DefaultTimeFormatter(time, maxTime, IsFinalCountdown);
            TimeText.text = text;
            if (settings.TimeTextColor.HasValue)
            {
                TimeText.color = settings.TimeTextColor.Value;
            }
        }

        return true;
    }

    internal bool CustomStartFinalHide(TimerBarSettings settings)
    {
        if (Source == null)
        {
            return false;
        }

        IsFinalCountdown = true;
        _freezeChunkUntil = 0f;

        var handled = false;
        if (ChunkTransform != null && settings.HideChunkOnFinal)
        {
            ChunkTransform.gameObject.SetActive(false);
            handled = true;
        }

        if (TimerRenderer != null && settings.FinalBarColor.HasValue)
        {
            ApplyColorOverride(TimerRenderer, ref _timerMaterial, settings.FinalBarColor.Value);
            handled = true;
        }

        return handled;
    }

    internal bool CustomTaskComplete(TimerBarSettings settings)
    {
        if (Source == null || ChunkTransform == null)
        {
            return false;
        }

        if (!IsFinalCountdown || !settings.HideChunkOnFinal)
        {
            if (!ChunkTransform.gameObject.activeSelf)
            {
                ChunkTransform.gameObject.SetActive(true);
            }
            if (settings.AlignChunkWithTimerX && TimerTransform != null)
            {
                AlignChunkLeftEdgeWithTimer(ChunkTransform, TimerTransform);
            }
        }

        _freezeChunkUntil = Time.time + Mathf.Max(0f, settings.ChunkFreezeSeconds);
        return true;
    }

    private void ApplyRootTransform(TimerBarSettings settings)
    {
        var root = RootTransform;
        if (root == null)
        {
            return;
        }

        root.localPosition = _defaultLocalPosition + settings.RootOffset;
        root.localScale = Vector3.Scale(_defaultLocalScale, settings.RootScaleMultiplier);
    }

    private void ApplyCustomColors(TimerBarSettings settings)
    {
        if (TimerRenderer != null)
        {
            var barColor = IsFinalCountdown ? settings.FinalBarColor : settings.NormalBarColor;
            if (barColor.HasValue)
            {
                ApplyColorOverride(TimerRenderer, ref _timerMaterial, barColor.Value);
            }
        }

        if (ChunkRenderer != null && settings.ChunkBarColor.HasValue)
        {
            ApplyColorOverride(ChunkRenderer, ref _chunkMaterial, settings.ChunkBarColor.Value);
        }

        if (TimeText != null && settings.TimeTextColor.HasValue)
        {
            TimeText.color = settings.TimeTextColor.Value;
        }
    }

    private static void ApplyColorOverride(MeshRenderer renderer, ref Material runtimeMaterial, Color color)
    {
        if (renderer == null)
        {
            return;
        }
        runtimeMaterial ??= CreateRuntimeMaterial(renderer);
        if (runtimeMaterial == null)
        {
            return;
        }
        runtimeMaterial.SetColor(ColorId, color);
    }

    private static Material CreateRuntimeMaterial(MeshRenderer renderer)
    {
        if (renderer == null)
        {
            return null;
        }
        var sourceMaterial = renderer.sharedMaterial;
        if (sourceMaterial == null)
        {
            return null;
        }

        Material runtimeMaterial = new(sourceMaterial);
        renderer.material = runtimeMaterial;
        return runtimeMaterial;
    }

    private static T ResolveField<T>(FieldInfo field, HideAndSeekTimerBar source) where T : class
    {
        if (field == null || source == null)
        {
            return null;
        }
        return field.GetValue(source) as T;
    }

    private static TextMeshPro ResolveTimeText(HideAndSeekTimerBar source)
    {
        return ResolveField<TextMeshPro>(TimeTextField, source) ?? source?.GetComponentInChildren<TextMeshPro>(true);
    }

    private static Transform ResolveTimerTransform(HideAndSeekTimerBar source)
    {
        var fromField = ResolveField<Transform>(TimerBarField, source);
        if (fromField != null)
        {
            return fromField;
        }
        if (source == null)
        {
            return null;
        }

        var named = source.transform.Find("TimerBar");
        if (named != null)
        {
            return named;
        }

        var renderer = source.GetComponentInChildren<MeshRenderer>(true);
        return renderer?.transform;
    }

    private static MeshRenderer ResolveTimerRenderer(HideAndSeekTimerBar source, Transform timerTransform)
    {
        var fromField = ResolveField<MeshRenderer>(TimerBarRendererField, source);
        if (fromField != null)
        {
            return fromField;
        }
        if (timerTransform == null)
        {
            return source?.GetComponentInChildren<MeshRenderer>(true);
        }
        return timerTransform.GetComponent<MeshRenderer>() ?? timerTransform.GetComponentInChildren<MeshRenderer>(true);
    }

    private static Transform ResolveChunkTransform(HideAndSeekTimerBar source)
    {
        var fromField = ResolveField<Transform>(ChunkBarField, source);
        if (fromField != null)
        {
            return fromField;
        }
        if (source == null)
        {
            return null;
        }

        var named = source.transform.Find("ChunkBar");
        if (named != null)
        {
            return named;
        }

        foreach (var child in source.transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.IndexOf("chunk", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return child;
            }
        }

        return null;
    }

    private static MeshRenderer ResolveChunkRenderer(Transform chunkTransform)
    {
        if (chunkTransform == null)
        {
            return null;
        }
        return chunkTransform.GetComponent<MeshRenderer>() ?? chunkTransform.GetComponentInChildren<MeshRenderer>(true);
    }

    private static void ApplyRightToLeftShrink(Transform target,
                                               Vector3 defaultPos,
                                               Vector3 defaultScale,
                                               Vector3 currentScale,
                                               TimerBarSettings settings)
    {
        if (target == null || !settings.ShrinkFromRightToLeft)
        {
            return;
        }
        if (Mathf.Approximately(defaultScale.x, 0f))
        {
            return;
        }

        var pos = defaultPos;
        pos.x = defaultPos.x + (currentScale.x - defaultScale.x) * 0.5f;
        target.localPosition = pos;
    }

    private static void AlignChunkLeftEdgeWithTimer(Transform chunk, Transform timer)
    {
        var timerScale = timer.localScale;
        var chunkScale = chunk.localScale;

        var timerLeft = timer.localPosition.x - timerScale.x * 0.5f;
        var chunkPos = chunk.localPosition;
        chunkPos.x = timerLeft + chunkScale.x * 0.5f;
        chunk.localPosition = chunkPos;
    }
}

internal sealed class TimerBarSettings
{
    internal bool Enabled { get; set; } = true;
    internal bool OverrideUpdate { get; set; } = true;
    internal bool OverrideUpdateTimer { get; set; } = true;
    internal bool OverrideStartFinalHide { get; set; } = true;
    internal bool OverrideTaskComplete { get; set; } = true;

    internal float LerpSpeed { get; set; } = 10f;
    internal float ChunkFreezeSeconds { get; set; } = 1f;
    internal bool HideChunkOnFinal { get; set; } = true;

    internal Vector3 RootOffset { get; set; } = Vector3.zero;
    internal Vector3 RootScaleMultiplier { get; set; } = Vector3.one;

    internal Color? NormalBarColor { get; set; }
    internal Color? FinalBarColor { get; set; } = Palette.ImpostorRed;
    internal Color? ChunkBarColor { get; set; }
    internal Color? TimeTextColor { get; set; }
    internal bool ShrinkFromRightToLeft { get; set; } = true;
    internal bool ShrinkChunkFromRightToLeft { get; set; }
    internal bool AlignChunkWithTimerX { get; set; }

    internal Func<float, float, bool, float> ProgressResolver { get; set; } = DefaultProgressResolver;
    internal Func<float, float, bool, string> TimeFormatter { get; set; } = DefaultTimeFormatter;

    internal void Reset()
    {
        Enabled = true;
        OverrideUpdate = true;
        OverrideUpdateTimer = true;
        OverrideStartFinalHide = true;
        OverrideTaskComplete = true;

        LerpSpeed = 10f;
        ChunkFreezeSeconds = 1f;
        HideChunkOnFinal = true;

        RootOffset = Vector3.zero;
        RootScaleMultiplier = Vector3.one;

        NormalBarColor = null;
        FinalBarColor = Palette.ImpostorRed;
        ChunkBarColor = null;
        TimeTextColor = null;
        ShrinkFromRightToLeft = true;
        ShrinkChunkFromRightToLeft = false;
        AlignChunkWithTimerX = false;

        ProgressResolver = DefaultProgressResolver;
        TimeFormatter = DefaultTimeFormatter;
    }

    internal static float DefaultProgressResolver(float time, float maxTime, bool _)
    {
        if (maxTime <= 0f)
        {
            return 0f;
        }
        return Mathf.Clamp01(time / maxTime);
    }

    internal static string DefaultTimeFormatter(float time, float _, bool __)
    {
        return TimeSpan.FromSeconds(Mathf.Max(0f, time)).ToString(@"m\:ss");
    }
}