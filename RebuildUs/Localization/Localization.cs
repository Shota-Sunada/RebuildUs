using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using RebuildUs.Utilities;

namespace RebuildUs.Localization;

public static class Tr
{
    private static readonly Dictionary<string, Dictionary<SupportedLangs, string>> Translations = [];

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
                if (!Translations.TryGetValue(prefix, out var trans))
                {
                    trans = [];
                    Translations[prefix] = trans;
                }
                trans[lang] = element.GetString();
                break;
        }
    }

    public static string Get(string key, params object[] args) => GetInternal(key, args);
    public static string Get((string category, string key) key, params object[] args) => GetInternal($"{key.category}.{key.key}", args);

    private static string GetInternal(string key, object[] args)
    {
        if (string.IsNullOrEmpty(key)) return "";

        var lang = TranslationController.InstanceExists ? FastDestroyableSingleton<TranslationController>.Instance.currentLanguage.languageID : SupportedLangs.English;

        if (!Translations.TryGetValue(key, out var langDic))
        {
            // Try to find by suffix if not found (e.g. "OptionOn" -> "GameSettings.OptionOn")
            var alternativeKey = Translations.Keys.FirstOrDefault(k => k.EndsWith("." + key));
            if (alternativeKey != null)
            {
                langDic = Translations[alternativeKey];
            }
            else
            {
                Logger.LogWarn($"There are no translation data. key: {key}");
                return key;
            }
        }

        if (!langDic.TryGetValue(lang, out var str))
        {
            return !langDic.TryGetValue(SupportedLangs.English, out var enStr) ? key : args.Length > 0 ? string.Format(enStr, args) : enStr;
        }

        return args.Length > 0 ? string.Format(str, args) : str;
    }
}