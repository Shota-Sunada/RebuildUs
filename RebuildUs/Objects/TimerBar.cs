using System.Reflection;

namespace RebuildUs.Objects;

internal sealed class TimerBar
{
    private static readonly FieldInfo TimeTextField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timeText");
    private static readonly FieldInfo TimerBarField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timerBar");
    private static readonly FieldInfo TimerBarRendererField = AccessTools.Field(typeof(HideAndSeekTimerBar), "timerBarRenderer");
    private static readonly FieldInfo ChunkBarField = AccessTools.Field(typeof(HideAndSeekTimerBar), "chunkBar");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private readonly HideAndSeekTimerBar _source;
    private readonly TextMeshPro _timeText;
    private readonly Transform _timerTransform;
    private readonly MeshRenderer _timerRenderer;
    private readonly Transform _chunkTransform;
    private readonly MeshRenderer _chunkRenderer;
    private readonly Vector3 _defaultLocalPosition;
    private readonly Vector3 _defaultLocalScale;
    private readonly Vector3 _defaultTimerLocalPosition;
    private readonly Vector3 _defaultTimerLocalScale;
    private readonly Vector3 _defaultChunkLocalPosition;
    private readonly Vector3 _defaultChunkLocalScale;

    private Material _timerMaterial;
    private Material _chunkMaterial;
    private float _targetBarSize;
    private float _freezeChunkUntil;
    private bool _isFinalCountdown;

    internal TimerBar(HideAndSeekTimerBar source)
    {
        _source = source;
        _timeText = ResolveTimeText(source);
        _timerTransform = ResolveTimerTransform(source);
        _timerRenderer = ResolveTimerRenderer(source, _timerTransform);
        _chunkTransform = ResolveChunkTransform(source);
        _chunkRenderer = ResolveChunkRenderer(_chunkTransform);

        Transform root = source?.transform;
        _defaultLocalPosition = root != null ? root.localPosition : Vector3.zero;
        _defaultLocalScale = root != null ? root.localScale : Vector3.one;
        _defaultTimerLocalPosition = _timerTransform != null ? _timerTransform.localPosition : Vector3.zero;
        _defaultTimerLocalScale = _timerTransform != null ? _timerTransform.localScale : Vector3.one;
        _defaultChunkLocalPosition = _chunkTransform != null ? _chunkTransform.localPosition : Vector3.zero;
        _defaultChunkLocalScale = _chunkTransform != null ? _chunkTransform.localScale : Vector3.one;
        _targetBarSize = _timerTransform != null ? _timerTransform.localScale.x : 1f;
    }

    internal HideAndSeekTimerBar Source
    {
        get => _source;
    }

    internal bool IsAlive
    {
        get => _source != null;
    }

    internal bool IsFinalCountdown
    {
        get => _isFinalCountdown;
        set => _isFinalCountdown = value;
    }

    internal float TargetBarSize
    {
        get => _targetBarSize;
        set => _targetBarSize = Mathf.Clamp01(value);
    }

    internal Transform RootTransform
    {
        get => _source?.transform;
    }

    internal Transform TimerTransform
    {
        get => _timerTransform;
    }

    internal Transform ChunkTransform
    {
        get => _chunkTransform;
    }

    internal TextMeshPro TimeText
    {
        get => _timeText;
    }

    internal MeshRenderer TimerRenderer
    {
        get => _timerRenderer;
    }

    internal MeshRenderer ChunkRenderer
    {
        get => _chunkRenderer;
    }

    internal bool CustomUpdate(TimerBarSettings settings)
    {
        if (_source == null) return false;
        if (_timerTransform == null && _chunkTransform == null) return false;

        ApplyRootTransform(settings);
        ApplyCustomColors(settings);

        float lerp = Mathf.Clamp01(Time.deltaTime * Mathf.Max(0f, settings.LerpSpeed));
        if (_timerTransform != null)
        {
            Vector3 timerScale = _timerTransform.localScale;
            float targetScaleX = _defaultTimerLocalScale.x * _targetBarSize;
            timerScale.x = Mathf.Lerp(timerScale.x, targetScaleX, lerp);
            _timerTransform.localScale = timerScale;
            ApplyRightToLeftShrink(_timerTransform, _defaultTimerLocalPosition, _defaultTimerLocalScale, timerScale, settings);
        }

        if (_chunkTransform != null && Time.time >= _freezeChunkUntil)
        {
            Vector3 chunkScale = _chunkTransform.localScale;
            float targetScaleX = _defaultChunkLocalScale.x * _targetBarSize;
            chunkScale.x = Mathf.Lerp(chunkScale.x, targetScaleX, lerp);
            _chunkTransform.localScale = chunkScale;
            if (settings.ShrinkChunkFromRightToLeft)
                ApplyRightToLeftShrink(_chunkTransform, _defaultChunkLocalPosition, _defaultChunkLocalScale, chunkScale, settings);
        }

        if (_chunkTransform != null && settings.AlignChunkWithTimerX && _timerTransform != null)
        {
            AlignChunkLeftEdgeWithTimer(_chunkTransform, _timerTransform);
        }

        return true;
    }

    internal bool CustomUpdateTimer(float time, float maxTime, TimerBarSettings settings)
    {
        if (_source == null) return false;
        if (_timeText == null && _timerTransform == null && _chunkTransform == null) return false;

        float progress = settings.ProgressResolver?.Invoke(time, maxTime, _isFinalCountdown) ?? TimerBarSettings.DefaultProgressResolver(time, maxTime, _isFinalCountdown);
        if (float.IsNaN(progress) || float.IsInfinity(progress)) progress = 0f;
        _targetBarSize = Mathf.Clamp01(progress);

        if (_timeText != null)
        {
            string text = settings.TimeFormatter?.Invoke(time, maxTime, _isFinalCountdown) ?? TimerBarSettings.DefaultTimeFormatter(time, maxTime, _isFinalCountdown);
            _timeText.text = text;
            if (settings.TimeTextColor.HasValue) _timeText.color = settings.TimeTextColor.Value;
        }

        return true;
    }

    internal bool CustomStartFinalHide(TimerBarSettings settings)
    {
        if (_source == null) return false;

        _isFinalCountdown = true;
        _freezeChunkUntil = 0f;

        bool handled = false;
        if (_chunkTransform != null && settings.HideChunkOnFinal)
        {
            _chunkTransform.gameObject.SetActive(false);
            handled = true;
        }

        if (_timerRenderer != null && settings.FinalBarColor.HasValue)
        {
            ApplyColorOverride(_timerRenderer, ref _timerMaterial, settings.FinalBarColor.Value);
            handled = true;
        }

        return handled;
    }

    internal bool CustomTaskComplete(TimerBarSettings settings)
    {
        if (_source == null || _chunkTransform == null) return false;

        if (!_isFinalCountdown || !settings.HideChunkOnFinal)
        {
            if (!_chunkTransform.gameObject.activeSelf) _chunkTransform.gameObject.SetActive(true);
            if (settings.AlignChunkWithTimerX && _timerTransform != null)
                AlignChunkLeftEdgeWithTimer(_chunkTransform, _timerTransform);
        }

        _freezeChunkUntil = Time.time + Mathf.Max(0f, settings.ChunkFreezeSeconds);
        return true;
    }

    private void ApplyRootTransform(TimerBarSettings settings)
    {
        Transform root = RootTransform;
        if (root == null) return;

        root.localPosition = _defaultLocalPosition + settings.RootOffset;
        root.localScale = Vector3.Scale(_defaultLocalScale, settings.RootScaleMultiplier);
    }

    private void ApplyCustomColors(TimerBarSettings settings)
    {
        if (_timerRenderer != null)
        {
            Color? barColor = _isFinalCountdown ? settings.FinalBarColor : settings.NormalBarColor;
            if (barColor.HasValue) ApplyColorOverride(_timerRenderer, ref _timerMaterial, barColor.Value);
        }

        if (_chunkRenderer != null && settings.ChunkBarColor.HasValue)
            ApplyColorOverride(_chunkRenderer, ref _chunkMaterial, settings.ChunkBarColor.Value);

        if (_timeText != null && settings.TimeTextColor.HasValue)
            _timeText.color = settings.TimeTextColor.Value;
    }

    private static void ApplyColorOverride(MeshRenderer renderer, ref Material runtimeMaterial, Color color)
    {
        if (renderer == null) return;
        runtimeMaterial ??= CreateRuntimeMaterial(renderer);
        if (runtimeMaterial == null) return;
        runtimeMaterial.SetColor(ColorId, color);
    }

    private static Material CreateRuntimeMaterial(MeshRenderer renderer)
    {
        if (renderer == null) return null;
        Material sourceMaterial = renderer.sharedMaterial;
        if (sourceMaterial == null) return null;

        Material runtimeMaterial = new(sourceMaterial);
        renderer.material = runtimeMaterial;
        return runtimeMaterial;
    }

    private static T ResolveField<T>(FieldInfo field, HideAndSeekTimerBar source) where T : class
    {
        if (field == null || source == null) return null;
        return field.GetValue(source) as T;
    }

    private static TextMeshPro ResolveTimeText(HideAndSeekTimerBar source)
    {
        return ResolveField<TextMeshPro>(TimeTextField, source) ?? source?.GetComponentInChildren<TextMeshPro>(true);
    }

    private static Transform ResolveTimerTransform(HideAndSeekTimerBar source)
    {
        Transform fromField = ResolveField<Transform>(TimerBarField, source);
        if (fromField != null) return fromField;
        if (source == null) return null;

        Transform named = source.transform.Find("TimerBar");
        if (named != null) return named;

        MeshRenderer renderer = source.GetComponentInChildren<MeshRenderer>(true);
        return renderer?.transform;
    }

    private static MeshRenderer ResolveTimerRenderer(HideAndSeekTimerBar source, Transform timerTransform)
    {
        MeshRenderer fromField = ResolveField<MeshRenderer>(TimerBarRendererField, source);
        if (fromField != null) return fromField;
        if (timerTransform == null) return source?.GetComponentInChildren<MeshRenderer>(true);
        return timerTransform.GetComponent<MeshRenderer>() ?? timerTransform.GetComponentInChildren<MeshRenderer>(true);
    }

    private static Transform ResolveChunkTransform(HideAndSeekTimerBar source)
    {
        Transform fromField = ResolveField<Transform>(ChunkBarField, source);
        if (fromField != null) return fromField;
        if (source == null) return null;

        Transform named = source.transform.Find("ChunkBar");
        if (named != null) return named;

        foreach (Transform child in source.transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.IndexOf("chunk", StringComparison.OrdinalIgnoreCase) >= 0) return child;
        }

        return null;
    }

    private static MeshRenderer ResolveChunkRenderer(Transform chunkTransform)
    {
        if (chunkTransform == null) return null;
        return chunkTransform.GetComponent<MeshRenderer>() ?? chunkTransform.GetComponentInChildren<MeshRenderer>(true);
    }

    private static void ApplyRightToLeftShrink(Transform target, Vector3 defaultPos, Vector3 defaultScale, Vector3 currentScale, TimerBarSettings settings)
    {
        if (target == null || !settings.ShrinkFromRightToLeft) return;
        if (Mathf.Approximately(defaultScale.x, 0f)) return;

        Vector3 pos = defaultPos;
        pos.x = defaultPos.x + (currentScale.x - defaultScale.x) * 0.5f;
        target.localPosition = pos;
    }

    private static void AlignChunkLeftEdgeWithTimer(Transform chunk, Transform timer)
    {
        Vector3 timerScale = timer.localScale;
        Vector3 chunkScale = chunk.localScale;

        float timerLeft = timer.localPosition.x - (timerScale.x * 0.5f);
        Vector3 chunkPos = chunk.localPosition;
        chunkPos.x = timerLeft + (chunkScale.x * 0.5f);
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
        if (maxTime <= 0f) return 0f;
        return Mathf.Clamp01(time / maxTime);
    }

    internal static string DefaultTimeFormatter(float time, float _, bool __)
    {
        return TimeSpan.FromSeconds(Mathf.Max(0f, time)).ToString(@"m\:ss");
    }
}