namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatNotificationPatch
{
    private static readonly StringBuilder ColorStringBuilder = new();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
    internal static bool ChatNotificationSetupPrefix(ChatNotification __instance, PlayerControl sender, string text)
    {
        if (MapUtilities.CachedShipStatus && !MapSettings.ShowChatNotifications) return false;

        __instance.timeOnScreen = 5f;
        __instance.gameObject.SetActive(true);
        __instance.SetCosmetics(sender.Data);
        string str;
        Color color;
        try
        {
            str = ColorUtility.ToHtmlStringRGB(Palette.TextColors[__instance.player.ColorId]);
            color = Palette.TextOutlineColors[__instance.player.ColorId];
        }
        catch
        {
            Color32 c = Palette.PlayerColors[__instance.player.ColorId];
            str = ColorUtility.ToHtmlStringRGB(c);

            color = c.r + c.g + c.b > 180 ? Palette.Black : Palette.White;
        }

        __instance.playerColorText.text = __instance.player.ColorBlindName;

        ColorStringBuilder.Clear();
        ColorStringBuilder.Append("<color=#").Append(str).Append('>');
        if (string.IsNullOrEmpty(sender.Data.PlayerName)) ColorStringBuilder.Append("...");
        else ColorStringBuilder.Append(sender.Data.PlayerName);

        string playerName = ColorStringBuilder.ToString();
        if (__instance.playerNameText.text != playerName) __instance.playerNameText.text = playerName;
        __instance.playerNameText.outlineColor = color;
        __instance.chatText.text = text;
        return false;
    }
}