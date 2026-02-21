namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatNotificationPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
    internal static bool ChatNotificationSetupPrefix(ChatNotification __instance, PlayerControl sender, string text)
    {
        return CustomColors.ChatNotificationSetup(__instance, sender, text);
    }
}