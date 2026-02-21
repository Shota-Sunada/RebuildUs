namespace RebuildUs.Objects;

internal sealed class Garlic
{
    internal static List<Garlic> Garlics = [];
    private readonly GameObject _background;

    internal readonly GameObject GarlicObject;

    internal Garlic(Vector2 p)
    {
        GarlicObject = new("Garlic") { layer = 11 };
        GarlicObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
        _background = new("Background") { layer = 11 };
        _background.transform.SetParent(GarlicObject.transform);
        Vector3 position = new(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        GarlicObject.transform.position = position;
        _background.transform.localPosition = new(0, 0, -1f); // before player

        SpriteRenderer garlicRenderer = GarlicObject.AddComponent<SpriteRenderer>();
        garlicRenderer.sprite = AssetLoader.Garlic;
        SpriteRenderer backgroundRenderer = _background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = AssetLoader.GarlicBackground;

        GarlicObject.SetActive(true);
        Garlics.Add(this);
    }

    internal static void ClearGarlics()
    {
        Garlics = [];
    }

    internal static void UpdateAll()
    {
        for (int i = 0; i < Garlics.Count; i++) Garlics[i]?.Update();
    }

    private void Update()
    {
        _background?.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}