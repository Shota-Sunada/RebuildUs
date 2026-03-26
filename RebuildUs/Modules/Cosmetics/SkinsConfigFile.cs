namespace RebuildUs.Modules.Cosmetics;

internal sealed class SkinsConfigFile
{
    [JsonPropertyName("hats")]
    public List<CustomHat> Hats { get; set; }
}