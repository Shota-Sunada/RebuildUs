namespace RebuildUs.UI;

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
        Common();
        if (!Helpers.IsGameStarted) PingStringBuilder.Append("<size=70%>By ").Append(RebuildUs.MOD_DEVELOPER).Append("</size>\n");
        DontFinishGame();
        GameMode();

        var newText = PingStringBuilder.ToString();
        if (ModText.text != newText)
        {
            ModText.text = newText;
        }

        if (Helpers.IsGameStarted)
        {
            ModText.alignment = TextAlignmentOptions.BottomLeft;
            ModText.transform.localPosition = new(-FastDestroyableSingleton<HudManager>.Instance.SettingsButton.gameObject.transform.localPosition.x - 0.95f, -2.4f, 0f);
        }
        else
        {
            ModText.alignment = TextAlignmentOptions.TopLeft;
            ModText.transform.localPosition = new(-FastDestroyableSingleton<HudManager>.Instance.SettingsButton.gameObject.transform.localPosition.x - 0.95f, 3.0f, 0f);
        }
    }

    private static void Common()
    {
        PingStringBuilder.Append("<color=#1684B0>").Append(RebuildUs.MOD_NAME).Append("</color> v").Append(RebuildUs.MOD_VERSION).Append('\n');
    }

    private static void DontFinishGame()
    {
        if (CustomOptionHolder.DontFinishGame.GetBool())
        {
            PingStringBuilder.Append("<color=#FF0000>").Append(Tr.Get(TrKey.DontFinishGameMode)).Append("</color>\n");
        }
    }

    private static void GameMode()
    {
        if (CustomOptionHolder.GameModeSelection.GetSelection() == 0) return;

        PingStringBuilder.Append("<color=#2E7BBD>").Append(Tr.Get(TrKey.GameMode)).Append(": ");

        switch (CustomOptionHolder.GameModeSelection.GetSelection())
        {
            case 1:
                PingStringBuilder.Append(Tr.Get(TrKey.GameModeHideNSeek)).Append("</color>\n");
                break;
            case 2:
                PingStringBuilder.Append(Tr.Get(TrKey.GameModeBattleRoyale)).Append("</color>\n");
                break;
            case 3:
                PingStringBuilder.Append(Tr.Get(TrKey.GameModeHotPotato)).Append("</color>\n");
                break;
        }
    }
}