namespace RebuildUs.Patches;

public static class Credits
{
    public static void UpdatePingText(PingTracker __instance)
    {
        __instance.text.alignment = TextAlignmentOptions.Top;
        var position = __instance.GetComponent<AspectPosition>();
        position.Alignment = AspectPosition.EdgeAlignments.Top;

        if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
        {
            __instance.text.text = $"{RebuildUs.MOD_NAME} v{RebuildUs.MOD_VERSION}\n{__instance.text.text}";
            position.DistanceFromEdge = MeetingHud.Instance ? new(1.25f, 0.15f, 0) : new(1.55f, 0.15f, 0);
        }
        else
        {
            __instance.text.text = $"{RebuildUs.MOD_NAME} v{RebuildUs.MOD_VERSION}\n<size=50%>By {RebuildUs.MOD_DEVELOPER}</size>\n{__instance.text.text}";
            position.DistanceFromEdge = new(0f, 0.1f, 0);
        }

        position.AdjustPosition();
    }

    public static void EditMainMenu()
    {
        var ruLogo = new GameObject("RULogo");
        ruLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
        ruLogo.transform.localPosition = new(-0.4f, 1f, 5f);

        var credits = new GameObject("RUModCredits");
        var text = credits.AddComponent<TextMeshPro>();
        text.SetText($"{RebuildUs.MOD_NAME} v{RebuildUs.MOD_VERSION}\n<size=50%>By {RebuildUs.MOD_DEVELOPER}</size>");
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize *= 0.05f;

        text.transform.SetParent(ruLogo.transform);
        text.transform.localPosition = Vector3.down * 1.25f;
    }
}