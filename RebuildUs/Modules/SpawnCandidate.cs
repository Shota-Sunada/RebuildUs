namespace RebuildUs.Modules;

public sealed class SpawnCandidate(StringNames locationKey, Vector2 location, Sprite sprite)
{
    public StringNames LocationKey = locationKey;
    public Vector2 SpawnLocation = location;
    public Sprite Sprite = sprite;
}
