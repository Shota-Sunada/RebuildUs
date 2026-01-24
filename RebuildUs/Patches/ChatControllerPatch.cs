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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static void UpdatePostfix(ChatController __instance)
    {
        if (!__instance.freeChatField.textArea.hasFocus) return;

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
        {
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
        {
            __instance.freeChatField.textArea.SetText(new StringBuilder(__instance.freeChatField.textArea.text).Append(GUIUtility.systemCopyBuffer).ToString());
        }
    }
}