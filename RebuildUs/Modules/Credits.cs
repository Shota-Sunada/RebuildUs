using InnerNet;

namespace RebuildUs.Modules;

internal static class Credits
{
    private static readonly StringBuilder PingStringBuilder = new();

    internal static void UpdatePingText(PingTracker __instance)
    {
        __instance.text.alignment = TextAlignmentOptions.Right;
        AspectPosition position = __instance.GetComponent<AspectPosition>();
        position.Alignment = AspectPosition.EdgeAlignments.Top;

        PingStringBuilder.Clear();
        if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
        {
            PingStringBuilder.Append("<color=#1684B0>").Append(RebuildUs.MOD_NAME).Append("</color> v").Append(RebuildUs.MOD_VERSION).Append('\n').Append(__instance.text.text);
            string newText = PingStringBuilder.ToString();
            if (__instance.text.text != newText) __instance.text.text = newText;
            position.DistanceFromEdge = MeetingHud.Instance ? new(1.4f, 0f, 0) : new(1.55f, 0f, 0);
        }
        else
        {
            PingStringBuilder.Append("<color=#1684B0>").Append(RebuildUs.MOD_NAME).Append("</color> v").Append(RebuildUs.MOD_VERSION).Append("\n<size=70%>By ").Append(RebuildUs.MOD_DEVELOPER).Append("</size>\n").Append(__instance.text.text);
            string newText = PingStringBuilder.ToString();
            if (__instance.text.text != newText) __instance.text.text = newText;
            position.DistanceFromEdge = new(0f, 0f, 0);
        }

        position.AdjustPosition();
    }

    internal static void EditMainMenu(MainMenuManager __instance)
    {
        GameObject ruLogo = new("RULogo");
        ruLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
        ruLogo.transform.localPosition = new(-0.4f, 1f, 5f);

        GameObject credits = new("RUModCredits");
        TextMeshPro text = credits.AddComponent<TextMeshPro>();
        text.SetText($"<color=#1684B0>{RebuildUs.MOD_NAME}</color> v{RebuildUs.MOD_VERSION}\n<size=70%>By {RebuildUs.MOD_DEVELOPER}</size>");
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize *= 0.07f;

        text.transform.SetParent(ruLogo.transform);
        text.transform.localPosition = Vector3.down * 1.25f;

        PassiveButton howToPlayButton = __instance.howToPlayButton;
        PassiveButton freePlayButton = __instance.freePlayButton;
        howToPlayButton.gameObject.SetActive(false);
        freePlayButton.gameObject.SetActive(false);

        PassiveButton createGameButton = __instance.createGameButton;
        // var enterCodeButtons = __instance.enterCodeButtons;
        Transform enterCodeButtons = createGameButton.transform.parent.Find("Enter Code Button");
        FindGameButton findGameButton = __instance.findGameButton;

        // remove line
        findGameButton.gameObject.transform.parent.Find("Line")?.gameObject.SetActive(false);
        findGameButton.gameObject.SetActive(false);

        createGameButton.transform.SetLocalX(0);
        enterCodeButtons?.transform.SetLocalX(0);
    }
}