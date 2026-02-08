using System.Text.Json.Serialization;

namespace RebuildUs.Modules.Cosmetics;

public sealed class SkinsConfigFile
{
    [JsonPropertyName("hats")] public List<CustomHat> Hats { get; set; }
}
