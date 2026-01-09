using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ChatNotificationPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
    public static bool ChatNotificationSetupPrefix(ChatNotification __instance, PlayerControl sender, string text)
    {
        return CustomColors.ChatNotificationSetup(__instance, sender, text);
    }
}