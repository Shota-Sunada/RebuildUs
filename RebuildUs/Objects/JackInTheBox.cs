namespace RebuildUs.Objects;

public class JackInTheBox
{
    public static List<JackInTheBox> AllJackInTheBoxes = [];
    public static int JackInTheBoxLimit = 3;
    public static bool BoxesConvertedToVents = false;

    public static Sprite GetBoxAnimationSprite(int index)
    {
        if (AssetLoader.TricksterAnimations == null || AssetLoader.TricksterAnimations.Count == 0) return null;
        index = Mathf.Clamp(index, 0, AssetLoader.TricksterAnimations.Count - 1);
        return AssetLoader.TricksterAnimations[index];
    }

    public static void StartAnimation(int ventId)
    {
        JackInTheBox box = AllJackInTheBoxes.FirstOrDefault((x) => x?.Vent != null && x.Vent.Id == ventId);
        if (box == null) return;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.6f, new Action<float>((p) =>
        {
            if (box.BoxRenderer != null)
            {
                box.BoxRenderer.sprite = GetBoxAnimationSprite((int)(p * AssetLoader.TricksterAnimations.Count));
                if (p == 1f) box.BoxRenderer.sprite = GetBoxAnimationSprite(0);
            }
        })));
    }

    private readonly GameObject GameObject;
    public Vent Vent;
    private readonly SpriteRenderer BoxRenderer;

    public JackInTheBox(Vector2 p)
    {
        GameObject = new GameObject("JackInTheBox") { layer = 11 };
        GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        Vector3 position = new(p.x, p.y, p.y / 1000f + 0.01f);
        position += (Vector3)PlayerControl.LocalPlayer.Collider.offset; // Add collider offset that DoMove moves the player up at a valid position
        // Create the marker
        GameObject.transform.position = position;
        BoxRenderer = GameObject.AddComponent<SpriteRenderer>();
        BoxRenderer.sprite = GetBoxAnimationSprite(0);

        // Create the vent
        var referenceVent = UnityEngine.Object.FindObjectOfType<Vent>();
        Vent = UnityEngine.Object.Instantiate(referenceVent);
        Vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        Vent.transform.position = GameObject.transform.position;
        Vent.Left = null;
        Vent.Right = null;
        Vent.Center = null;
        Vent.EnterVentAnim = null;
        Vent.ExitVentAnim = null;
        Vent.Offset = new Vector3(0f, 0.25f, 0f);
        Vent.GetComponent<PowerTools.SpriteAnim>()?.Stop();
        Vent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
        var ventRenderer = Vent.GetComponent<SpriteRenderer>();
        ventRenderer.sprite = null;
        Vent.myRend = ventRenderer;
        var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
        allVentsList.Add(Vent);
        MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
        Vent.gameObject.SetActive(false);
        Vent.name = "JackInTheBoxVent_" + Vent.Id;

        // Only render the box for the Trickster
        // var playerIsTrickster = PlayerControl.LocalPlayer == Trickster.trickster;
        // gameObject.SetActive(playerIsTrickster);

        AllJackInTheBoxes.Add(this);
    }

    public static void UpdateStates()
    {
        if (BoxesConvertedToVents == true) return;
        foreach (var box in AllJackInTheBoxes)
        {
            // var playerIsTrickster = PlayerControl.LocalPlayer == Trickster.trickster;
            // box.gameObject.SetActive(playerIsTrickster);
        }
    }

    public void ConvertToVent()
    {
        GameObject.SetActive(true);
        Vent.gameObject.SetActive(true);
        return;
    }

    public static void ConvertToVents()
    {
        foreach (var box in AllJackInTheBoxes)
        {
            box.ConvertToVent();
        }
        ConnectVents();
        BoxesConvertedToVents = true;
        return;
    }

    public static bool HasJackInTheBoxLimitReached()
    {
        return AllJackInTheBoxes.Count >= JackInTheBoxLimit;
    }

    private static void ConnectVents()
    {
        for (var i = 0; i < AllJackInTheBoxes.Count - 1; i++)
        {
            var a = AllJackInTheBoxes[i];
            var b = AllJackInTheBoxes[i + 1];
            a.Vent.Right = b.Vent;
            b.Vent.Left = a.Vent;
        }
        // Connect first with last
        AllJackInTheBoxes.First().Vent.Left = AllJackInTheBoxes.Last().Vent;
        AllJackInTheBoxes.Last().Vent.Right = AllJackInTheBoxes.First().Vent;
    }

    public static void ClearJackInTheBoxes()
    {
        BoxesConvertedToVents = false;
        AllJackInTheBoxes = [];
    }
}