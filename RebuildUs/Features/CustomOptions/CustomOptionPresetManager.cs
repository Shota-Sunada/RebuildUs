namespace RebuildUs.Features.CustomOptions;

internal static class CustomOptionPresetManager
{
    private static readonly string PresetsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "RebuildUs", "Presets");
    private static readonly Dictionary<int, Dictionary<string, int>> _presetCache = [];
    private static readonly HashSet<int> _dirtyPresets = [];
    private static int? _currentPresetIndex;
    private static bool _isGlobalDirty = false;

    internal static void SavePreset(int presetId, Dictionary<string, int> presetData)
    {
        _presetCache[presetId] = new Dictionary<string, int>(presetData);
        _dirtyPresets.Add(presetId);
    }

    internal static void SaveAllPresets()
    {
        if (_dirtyPresets.Count == 0 && !_isGlobalDirty)
            return;

        try
        {
            if (!Directory.Exists(PresetsDirectory))
            {
                Directory.CreateDirectory(PresetsDirectory);
            }

            foreach (var presetId in _dirtyPresets)
            {
                if (_presetCache.TryGetValue(presetId, out var data))
                {
                    var filePath = Path.Combine(PresetsDirectory, string.Format("Preset{0}.json", presetId));
                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    Logger.LogInfo(string.Format("[CustomOptionPresetManager] Saved preset {0}", presetId));
                }
            }
            _dirtyPresets.Clear();

            if (_isGlobalDirty && _currentPresetIndex.HasValue)
            {
                var filePath = Path.Combine(PresetsDirectory, "Global.json");
                var data = new Dictionary<string, int> { { "SelectedPreset", _currentPresetIndex.Value } };
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                _isGlobalDirty = false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("[CustomOptionPresetManager] Failed to save presets: {0}", ex.ToString());
        }
    }

    internal static Dictionary<string, int> LoadPreset(int presetId)
    {
        if (_presetCache.TryGetValue(presetId, out var cachedData))
        {
            return cachedData;
        }

        try
        {
            if (!Directory.Exists(PresetsDirectory))
            {
                Directory.CreateDirectory(PresetsDirectory);
            }

            var filePath = Path.Combine(PresetsDirectory, string.Format("Preset{0}.json", presetId));
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
                _presetCache[presetId] = loaded;
                return loaded;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(string.Format("[CustomOptionPresetManager] Failed to load preset {0}", presetId), ex.ToString());
        }

        var empty = new Dictionary<string, int>();
        _presetCache[presetId] = empty;
        return empty;
    }

    internal static void SaveCurrentPresetIndex(int index)
    {
        _currentPresetIndex = index;
        _isGlobalDirty = true;
    }

    internal static int LoadCurrentPresetIndex(int defaultIndex)
    {
        if (_currentPresetIndex.HasValue)
        {
            return _currentPresetIndex.Value;
        }

        try
        {
            var filePath = Path.Combine(PresetsDirectory, "Global.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                if (data != null && data.TryGetValue("SelectedPreset", out var index))
                {
                    _currentPresetIndex = index;
                    return index;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("[CustomOptionPresetManager] Failed to load current preset index", ex.ToString());
        }

        _currentPresetIndex = defaultIndex;
        return defaultIndex;
    }
}
