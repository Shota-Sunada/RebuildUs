using System.Reflection;
using System.Text.Json;

namespace RebuildUs.Localization;

public static class Tr
{
    private const string BLANK = "[BLANK]";
    private const string NO_KEY = "[NO KEY]";
    private const string NOTFOUND = "[NOTFOUND]";
    private const string NO_VALUE = "[NO VALUE]";
    private const string ERROR = "[ERROR]";

    private const string LANGUAGE_FOLDER = "Languages";

    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> INTERNAL_TRANSLATIONS = [];
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> CUSTOM_TRANSLATIONS = [];
    private static readonly HashSet<string> MISSING_KEYS = [];

    public static void Initialize()
    {
        INTERNAL_TRANSLATIONS.Clear();
        foreach (SupportedLangs lang in Enum.GetValues(typeof(SupportedLangs)))
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"RebuildUs.Localization.Translations.{lang}.json");
            if (stream == null) continue;

            try
            {
                var nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream);
                if (nestedDict != null)
                {
                    var flattened = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    FlattenTranslations(nestedDict, flattened);
                    INTERNAL_TRANSLATIONS[lang] = flattened;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load internal translation for {lang}: {ex.Message}");
            }
        }

        LoadCustomTranslations();
    }

    private static void FlattenTranslations(Dictionary<string, JsonElement> nestedDict, Dictionary<string, string> target)
    {
        foreach (var category in nestedDict)
        {
            if (category.Value.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in category.Value.EnumerateObject())
                {
                    var key = category.Key == "General" ? property.Name : $"{category.Key}.{property.Name}";
                    target[key] = property.Value.GetString() ?? "";
                }
            }
            else
                target[category.Key] = category.Value.GetString() ?? "";
        }
    }

    private static void LoadCustomTranslations()
    {
        CUSTOM_TRANSLATIONS.Clear();

        if (!Directory.Exists(LANGUAGE_FOLDER))
        {
            try
            {
                Directory.CreateDirectory(LANGUAGE_FOLDER);
            }
            catch { }

            return;
        }

        foreach (var file in Directory.GetFiles(LANGUAGE_FOLDER, "*.json"))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            if (Enum.TryParse<SupportedLangs>(fileName, true, out var lang))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    if (nestedDict != null)
                    {
                        var flattened = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        FlattenTranslations(nestedDict, flattened);
                        CUSTOM_TRANSLATIONS[lang] = flattened;
                        Logger.LogInfo($"Loaded custom translation for {lang} from {file}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to load custom translation from {file}: {ex.Message}");
                }
            }
        }
    }

    public static string Get(TrKey key, params object[] args)
    {
        var keyStr = key.ToString();
        var lang = TranslationController.InstanceExists ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID : SupportedLangs.English;

        string result = null;
        if (CUSTOM_TRANSLATIONS.TryGetValue(lang, out var customLang) && customLang.TryGetValue(keyStr, out result)) { }
        else if (lang != SupportedLangs.English && CUSTOM_TRANSLATIONS.TryGetValue(SupportedLangs.English, out var customEn) && customEn.TryGetValue(keyStr, out result)) { }
        else if (INTERNAL_TRANSLATIONS.TryGetValue(lang, out var internalLang) && internalLang.TryGetValue(keyStr, out result)) { }
        else if (lang != SupportedLangs.English && INTERNAL_TRANSLATIONS.TryGetValue(SupportedLangs.English, out var internalEn) && internalEn.TryGetValue(keyStr, out result)) { }

        if (result == null)
        {
            if (MISSING_KEYS.Add(keyStr)) Logger.LogWarn($"Translation key not found: {keyStr}");
            return NOTFOUND;
        }

        if (string.IsNullOrEmpty(result))
        {
            Logger.LogWarn($"Translation value is null or empty: {keyStr}");
            return NO_VALUE;
        }

        try
        {
            return args.Length > 0 ? string.Format(result, args) : result;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to format string for key {keyStr}: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    ///     Helper to get translation from a string key. Use this only for dynamic keys.
    ///     TODO: This method should be removed and all calls should be replaced with TranslateKey.
    /// </summary>
    public static string GetDynamic(string keyStr, params object[] args)
    {
        if (Enum.TryParse<TrKey>(keyStr, out var key)) return Get(key, args);
        Logger.LogWarn($"Dynamic translation key not found in enum: {keyStr}");
        return NOTFOUND;
    }

    public static void Update()
    {
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.L) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.L)) DumpMissingKeys();
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.T) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.T))
        {
            Initialize();
            Logger.LogInfo("Translations reloaded.");
        }
    }

    private static void DumpMissingKeys()
    {
        if (MISSING_KEYS.Count == 0)
        {
            Logger.LogInfo("No missing translation keys found.");
            return;
        }

        try
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "MissingKeys.json");
            var json = JsonSerializer.Serialize(MISSING_KEYS, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Logger.LogInfo($"Missing translation keys dumped to {path}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump missing keys: {ex.Message}");
        }
    }
}
