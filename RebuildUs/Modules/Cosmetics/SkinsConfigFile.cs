using System.Text.Json.Serialization;

namespace RebuildUs.Modules.Cosmetics;

internal sealed class SkinsConfigFile
{
    [JsonPropertyName("hats")] internal List<CustomHat> Hats { get; set; }
}