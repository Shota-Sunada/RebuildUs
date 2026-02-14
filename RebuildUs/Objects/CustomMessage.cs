namespace RebuildUs.Objects;

public class CustomMessage
{
    private readonly TMP_Text Text;
    private static readonly List<CustomMessage> CustomMessages = [];

    private static readonly Color YellowColor = new(0.988f, 0.729f, 0.012f, 1f);

    public CustomMessage(string message, float duration, Vector2 localPosition, MessageType type)
    {
        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject, FastDestroyableSingleton<HudManager>.Instance.transform, true);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            Text = gameObject.GetComponent<TMP_Text>();
            Text.text = Tr.GetDynamic(message);

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y, gameObject.transform.localPosition.z);
            CustomMessages.Add(this);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                if (Text == null || Text.gameObject == null) return;
                bool even = ((int)(p * duration / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds

                switch (type)
                {
                    case MessageType.GameMode:
                        Text.alignment = TMPro.TextAlignmentOptions.Left;
                        while (MapSettings.GameMode is not CustomGameMode.Roles && MapSettings.gamemodeMatchDuration >= 0) {
                            string prefix = ("<color=#FF8000FF>");
                            switch (MapSettings.GameMode)
                            {
                                case CustomGameMode.Roles:
                                    Text.text = prefix + CaptureTheFlag.flagpointCounter + "</color>";
                                    break;
                                case CustomGameMode.PoliceAndThieves:
                                    Text.text = prefix + PoliceAndThief.thiefpointCounter + "</color>";
                                    break;
                                case CustomGameMode.HotPotato:
                                    Text.text = prefix + HotPotato.hotpotatopointCounter + "</color>";
                                    break;
                                case CustomGameMode.BattleRoyale:
                                    Text.text = prefix + BattleRoyale.battleRoyalePointCounter + "</color>";
                                    break;
                            }
                            return;
                        }
                        Text.text = "";
                        break;
                    case MessageType.Normal:
                        break;
                }

                if (Text != null) Text.color = even ? YellowColor : Color.red;

                if (Mathf.Approximately(p, 1f) && Text != null && Text.gameObject != null)
                {
                    UnityEngine.Object.Destroy(Text.gameObject);
                    CustomMessages.Remove(this);
                }
            })));
        }
    }
}