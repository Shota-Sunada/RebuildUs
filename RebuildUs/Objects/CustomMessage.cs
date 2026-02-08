using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

public sealed class CustomMessage
{
    private static readonly List<CustomMessage> CUSTOM_MESSAGES = [];

    private static readonly Color YELLOW_COLOR = new(0.988f, 0.729f, 0.012f, 1f);
    private readonly TMP_Text _text;

    public CustomMessage(string message, float duration, Vector2 localPosition, MessageType type)
    {
        var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            var gameObject = Object.Instantiate(roomTracker.gameObject, FastDestroyableSingleton<HudManager>.Instance.transform, true);

            Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            _text = gameObject.GetComponent<TMP_Text>();
            _text.text = Tr.GetDynamic(message);

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new(localPosition.x, localPosition.y, gameObject.transform.localPosition.z);
            CUSTOM_MESSAGES.Add(this);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
            {
                if (_text == null || _text.gameObject == null) return;
                var even = (int)((p * duration) / 0.25f) % 2 == 0; // Bool flips every 0.25 seconds

                switch (type)
                {
                    case MessageType.GameMode:
                        _text.alignment = TextAlignmentOptions.Left;
                        while (MapSettings.GameMode is not CustomGameMode.Roles && MapSettings.GamemodeMatchDuration >= 0)
                        {
                            var prefix = "<color=#FF8000FF>";
                            switch (MapSettings.GameMode)
                            {
                                case CustomGameMode.Roles:
                                    _text.text = prefix + CaptureTheFlag.FlagpointCounter + "</color>";
                                    break;
                                case CustomGameMode.PoliceAndThieves:
                                    _text.text = prefix + PoliceAndThief.ThiefpointCounter + "</color>";
                                    break;
                                case CustomGameMode.HotPotato:
                                    _text.text = prefix + HotPotato.HotpotatopointCounter + "</color>";
                                    break;
                                case CustomGameMode.BattleRoyale:
                                    _text.text = prefix + BattleRoyale.BattleRoyalePointCounter + "</color>";
                                    break;
                            }

                            return;
                        }

                        _text.text = "";
                        break;
                    case MessageType.Normal:
                        break;
                }

                if (_text != null) _text.color = even ? YELLOW_COLOR : Color.red;

                if (Mathf.Approximately(p, 1f) && _text != null && _text.gameObject != null)
                {
                    Object.Destroy(_text.gameObject);
                    CUSTOM_MESSAGES.Remove(this);
                }
            })));
        }
    }
}
