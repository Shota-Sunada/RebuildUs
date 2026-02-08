namespace RebuildUs.Objects;

public sealed class Garlic
{
    public static List<Garlic> Garlics = [];
    private readonly GameObject _background;

    public GameObject GarlicObject;

    public Garlic(Vector2 p)
    {
        GarlicObject = new("Garlic") { layer = 11 };
        GarlicObject.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        _background = new("Background") { layer = 11 };
        _background.transform.SetParent(GarlicObject.transform);
        Vector3 position = new(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        GarlicObject.transform.position = position;
        _background.transform.localPosition = new(0, 0, -1f); // before player

        var garlicRenderer = GarlicObject.AddComponent<SpriteRenderer>();
        garlicRenderer.sprite = AssetLoader.Garlic;
        var backgroundRenderer = _background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = AssetLoader.GarlicBackground;

        GarlicObject.SetActive(true);
        Garlics.Add(this);
    }

    public static void ClearGarlics()
    {
        Garlics = [];
    }

    public static void UpdateAll()
    {
        for (var i = 0; i < Garlics.Count; i++) Garlics[i]?.Update();
    }

    public void Update()
    {
        _background?.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}
