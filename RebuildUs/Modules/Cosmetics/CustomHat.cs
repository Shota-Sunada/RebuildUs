namespace RebuildUs.Modules.Cosmetics;

internal sealed class CustomHat : CustomHatHashes
{
    [JsonPropertyName("author")] internal string Author { get; set; }

    [JsonPropertyName("bounce")] internal bool Bounce { get; set; }

    [JsonPropertyName("climbresource")] internal string ClimbResource { get; set; }

    [JsonPropertyName("condition")] internal string Condition { get; set; }

    [JsonPropertyName("name")] internal string Name { get; set; }

    [JsonPropertyName("package")] internal string Package { get; set; }

    [JsonPropertyName("resource")] internal string Resource { get; set; }

    [JsonPropertyName("adaptive")] internal bool Adaptive { get; set; }

    [JsonPropertyName("behind")] internal bool Behind { get; set; }

    [JsonPropertyName("backresource")] internal string BackResource { get; set; }

    [JsonPropertyName("backflipresource")] internal string BackFlipResource { get; set; }

    [JsonPropertyName("flipresource")] internal string FlipResource { get; set; }
}