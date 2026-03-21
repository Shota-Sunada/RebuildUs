using System.Text.Json;

namespace RebuildUs.Localization;

internal static class Tr
{
    private const string BLANK = "[BLANK]";
    private const string NO_KEY = "[NO KEY]";
    private const string NOTFOUND = "[NOTFOUND]";
    private const string NO_VALUE = "[NO VALUE]";
    private const string ERROR = "[ERROR]";

    private const string LANGUAGES_FOLDER = "Languages";

    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> InternalTranslations = [];
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> CustomTranslations = [];
    private static readonly HashSet<string> MissingKeys = [];

    internal static void Initialize()
    {
        InternalTranslations.Clear();
        foreach (SupportedLangs lang in Enum.GetValues(typeof(SupportedLangs)))
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("RebuildUs.Localization.Translations.{0}.json", Enum.GetName(lang)));
            if (stream == null)
            {
                continue;
            }

            try
            {
                var nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream);
                if (nestedDict != null)
                {
                    Dictionary<string, string> flattened = new(StringComparer.OrdinalIgnoreCase);
                    FlattenTranslations(nestedDict, flattened);
                    InternalTranslations[lang] = flattened;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[Localization] Failed to load internal translation for {0}: {1}", Enum.GetName(lang), ex.Message);
            }
        }

        LoadCustomTranslations();
    }

    private static void FlattenTranslations(Dictionary<string, JsonElement> nestedDict, IDictionary<string, string> target)
    {
        foreach (var category in nestedDict)
        {
            if (category.Value.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in category.Value.EnumerateObject())
                {
                    var key = category.Key == "General" ? property.Name : string.Format("{0}.{1}", category.Key, property.Name);
                    target[key] = property.Value.GetString() ?? "";
                }
            }
            else
            {
                target[category.Key] = category.Value.GetString() ?? "";
            }
        }
    }

    private static void LoadCustomTranslations()
    {
        CustomTranslations.Clear();

        if (!Directory.Exists(LANGUAGES_FOLDER))
        {
            try
            {
                Directory.CreateDirectory(LANGUAGES_FOLDER);
            }
            catch
            {
                // ignored
            }

            return;
        }

        foreach (var file in Directory.GetFiles(LANGUAGES_FOLDER, "*.json"))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            if (!Enum.TryParse(fileName, true, out SupportedLangs lang))
            {
                continue;
            }

            try
            {
                var json = File.ReadAllText(file);
                var nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (nestedDict != null)
                {
                    Dictionary<string, string> flattened = new(StringComparer.OrdinalIgnoreCase);
                    FlattenTranslations(nestedDict, flattened);
                    CustomTranslations[lang] = flattened;
                    Logger.LogInfo("[LoadCustomTranslations] Loaded custom translation for {0} from {1}", Enum.GetName(lang), file);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[LoadCustomTranslations] Failed to load custom translation from {0}: {1}", file, ex.Message);
            }
        }
    }

    internal static string Get(TrKey key, params object[] args)
    {
        var keyStr = key.ToString();
        var lang = FastDestroyableSingleton<TranslationController>.InstanceExists
            ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID
            : SupportedLangs.English;

        string result = null;
        if (CustomTranslations.TryGetValue(lang, out var customLang) && customLang.TryGetValue(keyStr, out result)) { }
        else if (InternalTranslations.TryGetValue(lang, out var internalLang) && internalLang.TryGetValue(keyStr, out result)) { }
        else if (CustomTranslations.TryGetValue(SupportedLangs.English, out var customEn) && customEn.TryGetValue(keyStr, out result)) { }
        else if (InternalTranslations.TryGetValue(SupportedLangs.English, out var internalEn) && internalEn.TryGetValue(keyStr, out result)) { }

        if (result == null)
        {
            if (MissingKeys.Add(keyStr))
            {
                Logger.LogWarn("[Get] Translation key not found: {0}", keyStr);
            }

            return NOTFOUND;
        }

        if (string.IsNullOrEmpty(result))
        {
            Logger.LogWarn("[Get] Translation value is null or empty: {0}", keyStr);
            return NO_VALUE;
        }

        try
        {
            return args.Length > 0 ? string.Format(result, args) : result;
        }
        catch (Exception ex)
        {
            Logger.LogError("[Get] Failed to format string for key {0}: {1}", keyStr, ex.Message);
            return result;
        }
    }

    /// <summary>
    ///     Helper to get translation from a string key. Use this only for dynamic keys.
    ///     TODO: This method should be removed and all calls should be replaced with TranslateKey.
    /// </summary>
    internal static string GetDynamic(string keyStr, params object[] args)
    {
        if (Enum.TryParse(keyStr, out TrKey key))
        {
            return Get(key, args);
        }

        Logger.LogWarn("[GetDynamic] Dynamic translation key not found in enum: {0}", keyStr);
        return NOTFOUND;
    }

    internal static void Update()
    {
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.L) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.L))
        {
            DumpMissingKeys();
        }

        if (!Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.T) && !Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.T))
        {
            return;
        }

        Initialize();
        Logger.LogInfo("[Update] Translations reloaded.");
    }

    private static void DumpMissingKeys()
    {
        if (MissingKeys.Count == 0)
        {
            Logger.LogInfo("[DumpMissingKeys] No missing translation keys found.");
            return;
        }

        try
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "MissingKeys.json");
            var json = JsonSerializer.Serialize(MissingKeys,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            File.WriteAllText(path, json);
            Logger.LogInfo("[DumpMissingKeys] Missing translation keys dumped to {0}", path);
        }
        catch (Exception ex)
        {
            Logger.LogError("[DumpMissingKeys] Failed to dump missing keys: {0}", ex.Message);
        }
    }
}