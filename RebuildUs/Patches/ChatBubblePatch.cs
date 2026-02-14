namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        Update.SetChatBubbleColor(__instance, playerName);
    }
}