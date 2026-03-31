namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    private static void AwakePrefix()
    {
        if (!FastDestroyableSingleton<EOSManager>.Instance.isKWSMinor)
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    internal static void UpdatePostfix(ChatController __instance)
    {
        if (!__instance.freeChatField.textArea.hasFocus)
        {
            return;
        }

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.C) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.C))
        {
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
        }

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.V) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.V))
        {
            __instance.freeChatField.textArea.SetText(new StringBuilder(__instance.freeChatField.textArea.text).Append(GUIUtility.systemCopyBuffer).ToString());
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    internal static bool AddChatPrefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
    {
        if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat) return true;
        var localPlayer = PlayerControl.LocalPlayer;
        return localPlayer == null || MeetingHud.Instance != null || LobbyBehaviour.Instance != null || localPlayer.IsDead() || localPlayer.PlayerId == sourcePlayer.PlayerId || (Lovers.EnableChat && localPlayer.GetPartner() == sourcePlayer);
    }
}