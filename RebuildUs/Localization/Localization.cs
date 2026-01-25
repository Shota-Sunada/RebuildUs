using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RebuildUs.Localization;

public static class Tr
{
    private const string BLANK = "[BLANK]";
    private const string NO_KEY = "[NO KEY]";
    private const string NOTFOUND = "[NOTFOUND]";
    private const string NO_VALUE = "[NO VALUE]";
    private const string ERROR = "[ERROR]";

    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> InternalTranslations = [];
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> CustomTranslations = [];
    private static readonly HashSet<string> MissingKeys = [];

    public static void Initialize()
    {
        InternalTranslations.Clear();
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
            {
                target[category.Key] = category.Value.GetString() ?? "";
            }
        }
    }

    private static void LoadCustomTranslations()
    {
        CustomTranslations.Clear();
        var modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var langDir = Path.Combine(modDir, "Language");

        if (!Directory.Exists(langDir))
        {
            try { Directory.CreateDirectory(langDir); } catch { }
            return;
        }

        foreach (var file in Directory.GetFiles(langDir, "*.json"))
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

    public static string Get(string key, params object[] args)
    {
        if (string.IsNullOrEmpty(key)) return NO_KEY;
        if (key is BLANK) return string.Empty;

        // Strip out color tags and leading dashes
        string keyClean = Regex.Replace(key, "<.*?>", "");
        keyClean = Regex.Replace(keyClean, "^-\\s*", "");
        keyClean = keyClean.Trim();

        int dotCount = 0;
        foreach (char c in keyClean) if (c == '.') dotCount++;

        if (dotCount != 1)
        {
            if (MissingKeys.Add(keyClean))
            {
                Logger.LogError($"Invalid translation key: {keyClean}");
            }
            return ERROR;
        }

        var lang = TranslationController.InstanceExists ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID : SupportedLangs.English;

        string result = null;
        if (CustomTranslations.TryGetValue(lang, out var customLang) && customLang.TryGetValue(keyClean, out result)) { }
        else if (lang != SupportedLangs.English && CustomTranslations.TryGetValue(SupportedLangs.English, out var customEn) && customEn.TryGetValue(keyClean, out result)) { }
        else if (InternalTranslations.TryGetValue(lang, out var internalLang) && internalLang.TryGetValue(keyClean, out result)) { }
        else if (lang != SupportedLangs.English && InternalTranslations.TryGetValue(SupportedLangs.English, out var internalEn) && internalEn.TryGetValue(keyClean, out result)) { }

        if (result == null)
        {
            if (MissingKeys.Add(keyClean))
            {
                Logger.LogWarn($"Translation key not found: {keyClean}");
            }
            return NOTFOUND;
        }
        else if (string.IsNullOrEmpty(result))
        {
            Logger.LogWarn($"Translation value is null or empty: {keyClean}");
            return NO_VALUE;
        }

        string finalStr = key.Replace(keyClean, result);

        try
        {
            return args.Length > 0 ? string.Format(finalStr, args) : finalStr;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to format string for key {key}: {ex.Message}");
            return finalStr;
        }
    }

    public static void Update()
    {
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.L) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.L))
        {
            DumpMissingKeys();
        }
        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.T) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.T))
        {
            Initialize();
            Logger.LogInfo("Translations reloaded.");
        }
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), "MissingKeys.json");
            var json = JsonSerializer.Serialize(MissingKeys, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Logger.LogInfo($"Missing translation keys dumped to {path}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump missing keys: {ex.Message}");
        }
    }
}