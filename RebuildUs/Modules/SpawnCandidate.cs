namespace RebuildUs.Modules;

internal sealed class SpawnCandidate(StringNames locationKey, Vector2 location, Sprite sprite)
{
    internal readonly StringNames LocationKey = locationKey;
    internal readonly Sprite Sprite = sprite;
    internal Vector2 SpawnLocation = location;
}