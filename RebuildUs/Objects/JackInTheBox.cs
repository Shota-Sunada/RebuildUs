using PowerTools;

namespace RebuildUs.Objects;

internal sealed class JackInTheBox
{
    private const int JACK_IN_THE_BOX_LIMIT = 3;
    private static List<JackInTheBox> _allJackInTheBoxes = [];
    internal static bool BoxesConvertedToVents;
    private readonly SpriteRenderer _boxRenderer;

    private readonly GameObject _gameObject;
    private readonly Vent _vent;

    internal JackInTheBox(Vector2 p)
    {
        _gameObject = new("JackInTheBox")
        {
            layer = 11,
        };
        _gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
        Vector3 position = new(p.x, p.y, p.y / 1000f + 0.01f);
        position += (Vector3)PlayerControl.LocalPlayer.Collider.offset; // Add collider offset that DoMove moves the player up at a valid position
        // Create the marker
        _gameObject.transform.position = position;
        _boxRenderer = _gameObject.AddComponent<SpriteRenderer>();
        _boxRenderer.sprite = GetBoxAnimationSprite(0);

        // Create the vent
        Vent referenceVent = UnityObject.FindObjectOfType<Vent>();
        _vent = UnityObject.Instantiate(referenceVent);
        _vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
        _vent.transform.position = _gameObject.transform.position;
        _vent.Left = null;
        _vent.Right = null;
        _vent.Center = null;
        _vent.EnterVentAnim = null;
        _vent.ExitVentAnim = null;
        _vent.Offset = new(0f, 0.25f, 0f);
        _vent.GetComponent<SpriteAnim>()?.Stop();

        int maxId = -1;
        Il2CppReferenceArray<Vent> allVents = MapUtilities.CachedShipStatus.AllVents;
        foreach (Vent t in allVents)
        {
            if (t.Id > maxId)
            {
                maxId = t.Id;
            }
        }

        _vent.Id = maxId + 1; // Make sure we have a unique id

        SpriteRenderer ventRenderer = _vent.GetComponent<SpriteRenderer>();
        ventRenderer.sprite = null;
        _vent.myRend = ventRenderer;

        Vent[] newVents = new Vent[allVents.Length + 1];
        for (int i = 0; i < allVents.Length; i++)
        {
            newVents[i] = allVents[i];
        }
        newVents[^1] = _vent;
        MapUtilities.CachedShipStatus.AllVents = newVents;

        _vent.gameObject.SetActive(false);
        _vent.name = "JackInTheBoxVent_" + _vent.Id;

        // Only render the box for the Trickster
        if (!BoxesConvertedToVents && !PlayerControl.LocalPlayer.IsRole(RoleType.Trickster))
        {
            _gameObject.SetActive(false);
        }

        _allJackInTheBoxes.Add(this);
    }

    private static Sprite GetBoxAnimationSprite(int index)
    {
        if (AssetLoader.TricksterAnimations == null || AssetLoader.TricksterAnimations.Count == 0)
        {
            return null;
        }
        index = Mathf.Clamp(index, 0, AssetLoader.TricksterAnimations.Count - 1);
        return AssetLoader.TricksterAnimations[index];
    }

    internal static void StartAnimation(int ventId)
    {
        JackInTheBox box = null;
        foreach (JackInTheBox b in _allJackInTheBoxes)
        {
            if (b?._vent == null || b._vent.Id != ventId)
            {
                continue;
            }
            box = b;
            break;
        }

        if (box == null)
        {
            return;
        }

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.6f,
            new Action<float>(p =>
            {
                if (box._boxRenderer == null)
                {
                    return;
                }
                box._boxRenderer.sprite = GetBoxAnimationSprite((int)(p * AssetLoader.TricksterAnimations.Count));
                if (Mathf.Approximately(p, 1f))
                {
                    box._boxRenderer.sprite = GetBoxAnimationSprite(0);
                }
            })));
    }

    internal static void UpdateStates()
    {
        if (BoxesConvertedToVents)
        {
            return;
        }
        foreach (JackInTheBox box in _allJackInTheBoxes)
        {
            bool playerIsTrickster = PlayerControl.LocalPlayer.IsRole(RoleType.Trickster);
            box._gameObject.SetActive(playerIsTrickster);
        }
    }

    private void ConvertToVent()
    {
        _gameObject.SetActive(true);
        _vent.gameObject.SetActive(true);
    }

    internal static void ConvertToVents()
    {
        foreach (JackInTheBox t in _allJackInTheBoxes)
        {
            t.ConvertToVent();
        }

        ConnectVents();
        BoxesConvertedToVents = true;
    }

    internal static bool HasJackInTheBoxLimitReached()
    {
        return _allJackInTheBoxes.Count >= JACK_IN_THE_BOX_LIMIT;
    }

    private static void ConnectVents()
    {
        for (int i = 0; i < _allJackInTheBoxes.Count - 1; i++)
        {
            JackInTheBox a = _allJackInTheBoxes[i];
            JackInTheBox b = _allJackInTheBoxes[i + 1];
            a._vent.Right = b._vent;
            b._vent.Left = a._vent;
        }

        // Connect first with last
        if (_allJackInTheBoxes.Count <= 0)
        {
            return;
        }
        JackInTheBox first = _allJackInTheBoxes[0];
        JackInTheBox last = _allJackInTheBoxes[^1];
        first._vent.Left = last._vent;
        last._vent.Right = first._vent;
    }

    internal static void ClearJackInTheBoxes()
    {
        BoxesConvertedToVents = false;
        _allJackInTheBoxes = [];
    }
}