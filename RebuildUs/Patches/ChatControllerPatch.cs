using AmongUs.Data;
using InnerNet;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    private static void AwakePrefix()
    {
        if (!EOSManager.Instance.isKWSMinor) DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    internal static void UpdatePostfix(ChatController __instance)
    {
        if (!__instance.freeChatField.textArea.hasFocus) return;

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.C) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.C)) ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.V) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.V)) __instance.freeChatField.textArea.SetText(new StringBuilder(__instance.freeChatField.textArea.text).Append(GUIUtility.systemCopyBuffer).ToString());
    }
}