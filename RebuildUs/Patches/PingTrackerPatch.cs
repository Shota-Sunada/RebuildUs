namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PingTrackerPatch
{
    private static readonly StringBuilder PingStringBuilder = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    internal static void UpdatePostfix(PingTracker __instance)
    {
        __instance.text.alignment = TextAlignmentOptions.Right;
        var position = __instance.GetComponent<AspectPosition>();
        position.Alignment = AspectPosition.EdgeAlignments.Top;

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
            PingStringBuilder.Append(__instance.text.text);
            var newText = PingStringBuilder.ToString();
            if (__instance.text.text != newText)
            {
                __instance.text.text = newText;
            }
            position.DistanceFromEdge = MeetingHud.Instance ? new(1.4f, 0f, 0) : new(1.55f, 0f, 0);
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
            PingStringBuilder.Append(__instance.text.text);
            var newText = PingStringBuilder.ToString();
            if (__instance.text.text != newText)
            {
                __instance.text.text = newText;
            }
            position.DistanceFromEdge = new(0f, 0f, 0);
        }

        position.AdjustPosition();
    }
}