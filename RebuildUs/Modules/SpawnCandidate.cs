namespace RebuildUs.Modules;

public class SpawnCandidate(StringNames locationKey, Vector2 location, Sprite sprite)
{
    public Vector2 SpawnLocation = location;
    public Sprite Sprite = sprite;
    public StringNames LocationKey = locationKey;
}