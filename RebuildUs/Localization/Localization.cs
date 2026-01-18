using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RebuildUs.Localization;

public static class Tr
{
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> Translations = [];

    public static void Initialize()
    {
        Translations.Clear();
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
                    foreach (var category in nestedDict)
                    {
                        if (category.Value.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var property in category.Value.EnumerateObject())
                            {
                                var key = category.Key == "General" ? property.Name : $"{category.Key}.{property.Name}";
                                flattened[key] = property.Value.GetString() ?? "";
                            }
                        }
                        else
                        {
                            flattened[category.Key] = category.Value.GetString() ?? "";
                        }
                    }
                    Translations[lang] = flattened;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load translation for {lang}: {ex.Message}");
            }
        }
    }

    public static string Get(string key, params object[] args)
    {
        if (string.IsNullOrEmpty(key)) return "";

        // Strip out color tags and leading dashes
        string keyClean = Regex.Replace(key, "<.*?>", "");
        keyClean = Regex.Replace(keyClean, "^-\\s*", "");
        keyClean = keyClean.Trim();

        var lang = TranslationController.InstanceExists ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID : SupportedLangs.English;

        string result = null;
        if (Translations.TryGetValue(lang, out var langDic) && langDic.TryGetValue(keyClean, out var translated))
        {
            result = translated;
        }
        else if (lang != SupportedLangs.English && Translations.TryGetValue(SupportedLangs.English, out var enDic) && enDic.TryGetValue(keyClean, out translated))
        {
            result = translated;
        }

        if (result == null)
        {
            return args.Length > 0 ? string.Format(key, args) : key;
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
}