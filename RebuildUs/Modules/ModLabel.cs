namespace RebuildUs.Modules;

internal static class ModLabel
{
    private static readonly StringBuilder PingStringBuilder = new();

    private static GameObject Object;
    private static TextMeshPro ModText;

    internal static bool IsInitialized => Object != null;

    internal static void Initialize()
    {
        if (IsInitialized) return;

        Object = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.roomTracker.gameObject.transform.parent.FindChild("PingTrackerTMP").gameObject, FastDestroyableSingleton<HudManager>.Instance.roomTracker.gameObject.transform.parent);
        Object.name = "RebuildUs_TMP";
        ModText = Object.GetComponent<TextMeshPro>();
        Object.GetComponent<AspectPosition>().Destroy();
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

            ModText.alignment = TextAlignmentOptions.BottomLeft;
            ModText.transform.localPosition = new(-FastDestroyableSingleton<HudManager>.Instance.SettingsButton.gameObject.transform.localPosition.x - 0.95f, -2.4f, 0f);

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

            ModText.alignment = TextAlignmentOptions.TopLeft;
            ModText.transform.localPosition = new(-FastDestroyableSingleton<HudManager>.Instance.SettingsButton.gameObject.transform.localPosition.x - 0.95f, 3.0f, 0f);
        }
    }
}