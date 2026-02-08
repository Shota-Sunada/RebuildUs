using PowerTools;
using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

public sealed class JackInTheBox
{
    public static List<JackInTheBox> AllJackInTheBoxes = [];
    public static int JackInTheBoxLimit = 3;
    public static bool BoxesConvertedToVents;
    private readonly SpriteRenderer _boxRenderer;

    private readonly GameObject _gameObject;
    public Vent Vent;

    public JackInTheBox(Vector2 p)
    {
        _gameObject = new("JackInTheBox") { layer = 11 };
        _gameObject.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        Vector3 position = new(p.x, p.y, (p.y / 1000f) + 0.01f);
        position += (Vector3)PlayerControl.LocalPlayer.Collider.offset; // Add collider offset that DoMove moves the player up at a valid position
                                                                        // Create the marker
        _gameObject.transform.position = position;
        _boxRenderer = _gameObject.AddComponent<SpriteRenderer>();
        _boxRenderer.sprite = GetBoxAnimationSprite(0);

        // Create the vent
        var referenceVent = Object.FindObjectOfType<Vent>();
        Vent = Object.Instantiate(referenceVent);
        Vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        Vent.transform.position = _gameObject.transform.position;
        Vent.Left = null;
        Vent.Right = null;
        Vent.Center = null;
        Vent.EnterVentAnim = null;
        Vent.ExitVentAnim = null;
        Vent.Offset = new(0f, 0.25f, 0f);
        Vent.GetComponent<SpriteAnim>()?.Stop();

        var maxId = -1;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        for (var i = 0; i < allVents.Length; i++)
        {
            if (allVents[i].Id > maxId)
                maxId = allVents[i].Id;
        }

        Vent.Id = maxId + 1; // Make sure we have a unique id

        var ventRenderer = Vent.GetComponent<SpriteRenderer>();
        ventRenderer.sprite = null;
        Vent.myRend = ventRenderer;

        var newVents = new Vent[allVents.Length + 1];
        for (var i = 0; i < allVents.Length; i++) newVents[i] = allVents[i];
        newVents[newVents.Length - 1] = Vent;
        MapUtilities.CachedShipStatus.AllVents = newVents;

        Vent.gameObject.SetActive(false);
        Vent.name = "JackInTheBoxVent_" + Vent.Id;

        // Only render the box for the Trickster
        if (!BoxesConvertedToVents && !PlayerControl.LocalPlayer.IsRole(RoleType.Trickster)) _gameObject.SetActive(false);

        AllJackInTheBoxes.Add(this);
    }

    public static Sprite GetBoxAnimationSprite(int index)
    {
        if (AssetLoader.TricksterAnimations == null || AssetLoader.TricksterAnimations.Count == 0) return null;
        index = Mathf.Clamp(index, 0, AssetLoader.TricksterAnimations.Count - 1);
        return AssetLoader.TricksterAnimations[index];
    }

    public static void StartAnimation(int ventId)
    {
        JackInTheBox box = null;
        for (var i = 0; i < AllJackInTheBoxes.Count; i++)
        {
            var b = AllJackInTheBoxes[i];
            if (b?.Vent != null && b.Vent.Id == ventId)
            {
                box = b;
                break;
            }
        }

        if (box == null) return;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.6f, new Action<float>(p =>
        {
            if (box._boxRenderer != null)
            {
                box._boxRenderer.sprite = GetBoxAnimationSprite((int)(p * AssetLoader.TricksterAnimations.Count));
                if (p == 1f) box._boxRenderer.sprite = GetBoxAnimationSprite(0);
            }
        })));
    }

    public static void UpdateStates()
    {
        if (BoxesConvertedToVents) return;
        for (var i = 0; i < AllJackInTheBoxes.Count; i++)
        {
            var box = AllJackInTheBoxes[i];
            var playerIsTrickster = PlayerControl.LocalPlayer.IsRole(RoleType.Trickster);
            box._gameObject.SetActive(playerIsTrickster);
        }
    }

    public void ConvertToVent()
    {
        _gameObject.SetActive(true);
        Vent.gameObject.SetActive(true);
    }

    public static void ConvertToVents()
    {
        for (var i = 0; i < AllJackInTheBoxes.Count; i++) AllJackInTheBoxes[i].ConvertToVent();
        ConnectVents();
        BoxesConvertedToVents = true;
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
        if (AllJackInTheBoxes.Count > 0)
        {
            var first = AllJackInTheBoxes[0];
            var last = AllJackInTheBoxes[AllJackInTheBoxes.Count - 1];
            first.Vent.Left = last.Vent;
            last.Vent.Right = first.Vent;
        }
    }

    public static void ClearJackInTheBoxes()
    {
        BoxesConvertedToVents = false;
        AllJackInTheBoxes = [];
    }
}
