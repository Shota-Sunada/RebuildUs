using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using RebuildUs.Utilities;

namespace RebuildUs.Localization;

public static class Tr
{
    private static readonly Dictionary<SupportedLangs, Dictionary<string, string>> Translations = [];

    public static void Initialize()
    {
        Translations.Clear();
        for (var i = SupportedLangs.English; i <= SupportedLangs.Irish; i++)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"RebuildUs.Localization.Translations.{i}.json");
            if (stream == null) continue;

            try
            {
                using var document = JsonDocument.Parse(stream);
                LoadElement(document.RootElement, "", i);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load translation for {i}: {ex.Message}");
            }
        }
    }

    private static void LoadElement(JsonElement element, string prefix, SupportedLangs lang)
    {
        if (!Translations.TryGetValue(lang, out var langDict))
        {
            langDict = [];
            Translations[lang] = langDict;
        }

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                    LoadElement(property.Value, key, lang);
                }
                break;
            case JsonValueKind.String:
                langDict[prefix] = element.GetString();
                break;
        }
    }

    public static string Get(string key, params object[] args) => GetInternal(key, args);

    private static string GetInternal(string key, object[] args)
    {
        if (string.IsNullOrEmpty(key)) return "[NO KEY]";

        var lang = SupportedLangs.English;
        if (TranslationController.InstanceExists)
        {
            var tc = FastDestroyableSingleton<TranslationController>.Instance;
            if (tc?.currentLanguage != null)
            {
                lang = tc.currentLanguage.languageID;
            }
        }

        if (Translations.TryGetValue(lang, out var langDict) && langDict.TryGetValue(key, out var str))
        {
            return args?.Length > 0 ? string.Format(str, args) : str;
        }

        if (lang != SupportedLangs.English && Translations.TryGetValue(SupportedLangs.English, out var enDict) && enDict.TryGetValue(key, out str))
        {
            return args?.Length > 0 ? string.Format(str, args) : str;
        }

        Logger.LogWarn($"Translation key not found: {key}");
        return $"[KEY NOT FOUND: {key}]";
    }
}