using AmongUs.Data;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ChatControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    private static void AwakePrefix()
    {
        if (!EOSManager.Instance.isKWSMinor)
        {
            DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
        }
    }
}