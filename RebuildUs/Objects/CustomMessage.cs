namespace RebuildUs.Objects;

public class CustomMessage
{
    private readonly TMP_Text Text;
    private static readonly List<CustomMessage> CustomMessages = [];

    private static readonly Color YellowColor = new(0.988f, 0.729f, 0.012f, 1f);

    public CustomMessage(string message, float duration)
    {
        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
        if (roomTracker != null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            Text = gameObject.GetComponent<TMP_Text>();
            Text.text = Tr.GetDynamic(message);

            // Use local position to place it in the player's view instead of the world location
            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
            CustomMessages.Add(this);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                if (Text == null || Text.gameObject == null) return;
                bool even = ((int)(p * duration / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                Text.color = even ? YellowColor : Color.red;

                if (p == 1f)
                {
                    UnityEngine.Object.Destroy(Text.gameObject);
                    CustomMessages.Remove(this);
                }
            })));
        }
    }
}
