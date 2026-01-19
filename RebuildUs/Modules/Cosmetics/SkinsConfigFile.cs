using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RebuildUs.Modules.Cosmetics;

public class SkinsConfigFile
{
    [JsonPropertyName("hats")] public List<CustomHat> Hats { get; set; }
}