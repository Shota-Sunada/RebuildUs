namespace RebuildUs.Modules;

public class SpawnCandidate
{
#nullable enable
    private static readonly Il2CppArrayBase<UnityEngine.Object>? audioClips = null;

    public Vector2 SpawnLocation;
    public Texture2D? Texture;
    public Sprite[] Sprites;
    public string TextureAddress;
    public StringNames LocationKey;
    public AudioClip? AudioClip;
    public string? AudioClipName;

    public Texture2D GetTexture()
    {
        if (Texture) return Texture!;
        Texture = Helpers.loadTextureFromResources(TextureAddress);
        ReloadSprites();
        return Texture;
    }

    public void ReloadTexture()
    {
        if (!Texture) Texture = Helpers.loadTextureFromResources(TextureAddress);
        ReloadSprites();
    }

    private void ReloadSprites()
    {
        foreach (Sprite sprite in Sprites)
        {
            if (sprite) UnityEngine.Object.Destroy(sprite);
        }
        Sprites = new Sprite[Texture!.width / 200];

        for (int i = 0; i < Sprites.Length; i++)
        {
            Sprites[i] = Helpers.loadSpriteFromResources(Texture, 100f, new Rect(i * 200, 0f, 200f, -200f), new Vector2(0.5f, 1f));
        }
    }

    public Sprite? GetSprite()
    {
        GetTexture();
        if (Sprites.Length > 0) return Sprites[0];
        return null;
    }

    public Il2CppSystem.Collections.IEnumerator GetEnumerator(SpriteRenderer renderer)
    {
        GetTexture();
        return Effects.Lerp(Sprites.Length * 0.06f, new Action<float>((t) =>
        {
            if (!renderer) return;
            int num = (int)(t * Sprites.Length);
            if (num < Sprites.Length) renderer.sprite = Sprites[num];
        }));
    }

    public SpawnCandidate(StringNames locationKey, Vector2 location, string textureAddress, string? audioClip)
    {
        SpawnLocation = location;
        LocationKey = locationKey;
        TextureAddress = textureAddress;
        Sprites = [];

        AudioClip = null;
        AudioClipName = audioClip;
    }
}