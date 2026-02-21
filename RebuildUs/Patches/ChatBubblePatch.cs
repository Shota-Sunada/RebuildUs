namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    internal static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        Update.SetChatBubbleColor(__instance, playerName);
    }
}