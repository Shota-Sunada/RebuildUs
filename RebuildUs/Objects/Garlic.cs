namespace RebuildUs.Objects;

public class Garlic
{
    public static List<Garlic> Garlics = [];

    public GameObject GarlicObject;
    private readonly GameObject Background;

    public Garlic(Vector2 p)
    {
        GarlicObject = new GameObject("Garlic") { layer = 11 };
        GarlicObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        Background = new GameObject("Background") { layer = 11 };
        Background.transform.SetParent(GarlicObject.transform);
        Vector3 position = new(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
        GarlicObject.transform.position = position;
        Background.transform.localPosition = new Vector3(0, 0, -1f); // before player

        var garlicRenderer = GarlicObject.AddComponent<SpriteRenderer>();
        garlicRenderer.sprite = AssetLoader.Garlic;
        var backgroundRenderer = Background.AddComponent<SpriteRenderer>();
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
        foreach (Garlic garlic in Garlics)
        {
            garlic?.Update();
        }
    }

    public void Update()
    {
        Background?.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}