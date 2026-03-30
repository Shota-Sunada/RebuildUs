namespace RebuildUs.UI;

internal static class PingText
{
    internal static void Update(PingTracker __instance)
    {
        __instance.text.alignment = TextAlignmentOptions.Center;
        var position = __instance.GetComponent<AspectPosition>();
        position.Alignment = AspectPosition.EdgeAlignments.Bottom;
        position.DistanceFromEdge = new(0f, 0.5f, 0f);
        position.AdjustPosition();

        if (ModLabel.IsInitialized) ModLabel.Update();
    }
}