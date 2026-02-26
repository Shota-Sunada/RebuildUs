namespace RebuildUs.Objects;

internal sealed class CustomMessage
{
    private static readonly List<CustomMessage> CustomMessages = [];

    private static readonly Color YellowColor = new(0.988f, 0.729f, 0.012f, 1f);

    internal CustomMessage(string message, float duration)
    {
        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            GameObject gameObject = UnityObject.Instantiate(roomTracker.gameObject, FastDestroyableSingleton<HudManager>.Instance.transform, true);

            UnityObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            TMP_Text text = gameObject.GetComponent<TMP_Text>();
            text.text = Tr.GetDynamic(message);

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new(0, -1.8f, gameObject.transform.localPosition.z);
            CustomMessages.Add(this);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
            {
                if (text == null || text.gameObject == null) return;
                bool even = (int)((p * duration) / 0.25f) % 2 == 0; // Bool flips every 0.25 seconds
                text.color = even ? YellowColor : Color.red;

                if (!Mathf.Approximately(p, 1f)) return;
                UnityObject.Destroy(text.gameObject);
                CustomMessages.Remove(this);
            })));
        }
    }
}