using System.Text.Json.Serialization;

namespace RebuildUs.Modules.Cosmetics;

internal class CustomHatHashes
{
    [JsonPropertyName("reshasha")] internal string ResHashA { get; set; }

    [JsonPropertyName("reshashb")] internal string ResHashB { get; set; }

    [JsonPropertyName("reshashbf")] internal string ResHashBf { get; set; }

    [JsonPropertyName("reshashc")] internal string ResHashC { get; set; }

    [JsonPropertyName("reshashf")] internal string ResHashF { get; set; }
}