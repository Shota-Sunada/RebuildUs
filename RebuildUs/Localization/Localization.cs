using System.Reflection;
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
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"RebuildUs.Localization.Translations.{lang}.json");
            if (stream == null) continue;

            try
            {
                Dictionary<string, JsonElement> nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream);
                if (nestedDict != null)
                {
                    Dictionary<string, string> flattened = new(StringComparer.OrdinalIgnoreCase);
                    FlattenTranslations(nestedDict, flattened);
                    InternalTranslations[lang] = flattened;
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
        foreach (KeyValuePair<string, JsonElement> category in nestedDict)
        {
            if (category.Value.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in category.Value.EnumerateObject())
                {
                    string key = category.Key == "General" ? property.Name : $"{category.Key}.{property.Name}";
                    target[key] = property.Value.GetString() ?? "";
                }
            }
            else
                target[category.Key] = category.Value.GetString() ?? "";
        }
    }

    private static void LoadCustomTranslations()
    {
        CustomTranslations.Clear();

        if (!Directory.Exists(LANGUAGES_FOLDER))
        {
            try { Directory.CreateDirectory(LANGUAGES_FOLDER); }
            catch { }

            return;
        }

        foreach (string file in Directory.GetFiles(LANGUAGES_FOLDER, "*.json"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (Enum.TryParse<SupportedLangs>(fileName, true, out SupportedLangs lang))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    Dictionary<string, JsonElement> nestedDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    if (nestedDict != null)
                    {
                        Dictionary<string, string> flattened = new(StringComparer.OrdinalIgnoreCase);
                        FlattenTranslations(nestedDict, flattened);
                        CustomTranslations[lang] = flattened;
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

    internal static string Get(TrKey key, params object[] args)
    {
        string keyStr = key.ToString();
        SupportedLangs lang = TranslationController.InstanceExists ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID : SupportedLangs.English;

        string result = null;
        if (CustomTranslations.TryGetValue(lang, out Dictionary<string, string> customLang) && customLang.TryGetValue(keyStr, out result)) { }
        else if (lang != SupportedLangs.English && CustomTranslations.TryGetValue(SupportedLangs.English, out Dictionary<string, string> customEn) && customEn.TryGetValue(keyStr, out result)) { }
        else if (InternalTranslations.TryGetValue(lang, out Dictionary<string, string> internalLang) && internalLang.TryGetValue(keyStr, out result)) { }
        else if (lang != SupportedLangs.English && InternalTranslations.TryGetValue(SupportedLangs.English, out Dictionary<string, string> internalEn) && internalEn.TryGetValue(keyStr, out result)) { }

        if (result == null)
        {
            if (MissingKeys.Add(keyStr)) Logger.LogWarn($"Translation key not found: {keyStr}");

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
    internal static string GetDynamic(string keyStr, params object[] args)
    {
        if (Enum.TryParse<TrKey>(keyStr, out TrKey key)) return Get(key, args);

        Logger.LogWarn($"Dynamic translation key not found in enum: {keyStr}");
        return NOTFOUND;
    }

    internal static void Update()
    {
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.L) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.L)) DumpMissingKeys();

        if (!Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.T) && !Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.T)) return;

        Initialize();
        Logger.LogInfo("Translations reloaded.");
    }

    private static void DumpMissingKeys()
    {
        if (MissingKeys.Count == 0)
        {
            Logger.LogInfo("No missing translation keys found.");
            return;
        }

        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "MissingKeys.json");
            string json = JsonSerializer.Serialize(MissingKeys, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Logger.LogInfo($"Missing translation keys dumped to {path}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump missing keys: {ex.Message}");
        }
    }
}