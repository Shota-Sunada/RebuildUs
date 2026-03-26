namespace RebuildUs.Modules;

internal static class ModLabel
{
    private static readonly StringBuilder PingStringBuilder = new();

    private static GameObject Object;
    private static TextMeshPro ModText;
    private static AspectPosition Position;

    internal static bool IsInitialized => Object != null;

    internal static void Initialize()
    {
        if (IsInitialized) return;

        Object = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.roomTracker.gameObject.transform.parent.FindChild("PingTrackerTMP").gameObject, FastDestroyableSingleton<HudManager>.Instance.roomTracker.gameObject.transform.parent);
        Object.name = "RebuildUs_TMP";
        ModText = Object.GetComponent<TextMeshPro>();
        Position = Object.GetComponent<AspectPosition>();
        Object.GetComponent<PingTracker>().Destroy(); ;
    }

    internal static void Update()
    {
        PingStringBuilder.Clear();
        if (Helpers.IsGameStarted)
        {
            PingStringBuilder
                .Append("<color=#1684B0>")
                .Append(RebuildUs.MOD_NAME)
                .Append("</color> v")
                .Append(RebuildUs.MOD_VERSION)
                .Append('\n');
            if (CustomOptionHolder.DontFinishGame.GetBool())
            {
                PingStringBuilder
                    .Append("<color=#FF0000>")
                    .Append(Tr.Get(TrKey.DontFinishGameMode))
                    .Append("</color>\n");
            }
            var newText = PingStringBuilder.ToString();
            if (ModText.text != newText)
            {
                ModText.text = newText;
            }
            Position.DistanceFromEdge = MeetingHud.Instance ? new(1.4f, 0f, 0f) : new(1.55f, 0f, 0f);
        }
        else
        {
            PingStringBuilder
                .Append("<color=#1684B0>")
                .Append(RebuildUs.MOD_NAME)
                .Append("</color> v")
                .Append(RebuildUs.MOD_VERSION)
                .Append("\n<size=70%>By ")
                .Append(RebuildUs.MOD_DEVELOPER)
                .Append("</size>\n");
            if (CustomOptionHolder.DontFinishGame.GetBool())
            {
                PingStringBuilder
                    .Append("<color=#FF0000>")
                    .Append(Tr.Get(TrKey.DontFinishGameMode))
                    .Append("</color>\n");
            }
            var newText = PingStringBuilder.ToString();
            if (ModText.text != newText)
            {
                ModText.text = newText;
            }
            ModText.alignment = TextAlignmentOptions.Left;
            ModText.verticalAlignment = VerticalAlignmentOptions.Top;
            ModText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            Position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
            Position.DistanceFromEdge = new(0.5f, 0f, 0f);
        }
    }
}