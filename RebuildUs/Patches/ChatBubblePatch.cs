namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    internal static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        if (__instance == null) return;
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp == null || !lp.IsTeamImpostor()) return;

        foreach (PlayerControl sourcePlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (sourcePlayer.Data == null || !sourcePlayer.Data.PlayerName.Equals(playerName)) continue;
            if (sourcePlayer.IsRole(RoleType.Spy)
                || (sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.GetRole(sourcePlayer)?.WasTeamRed == true)
                || (sourcePlayer.IsRole(RoleType.Jackal) && Jackal.GetRole(sourcePlayer)?.WasTeamRed == true))
                __instance.NameText.color = Palette.ImpostorRed;

            break;
        }
    }
}