namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    internal static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        if (__instance == null)
        {
            return;
        }
        var lp = PlayerControl.LocalPlayer;
        if (lp == null || !lp.IsTeamImpostor())
        {
            return;
        }

        foreach (var sourcePlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (sourcePlayer.Data == null || !sourcePlayer.Data.PlayerName.Equals(playerName))
            {
                continue;
            }
            if (sourcePlayer.IsRole(RoleType.Spy)
                || sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.Instance.WasTeamRed
                || sourcePlayer.IsRole(RoleType.Jackal) && Jackal.Instance.WasTeamRed)
            {
                __instance.NameText.color = Palette.ImpostorRed;
            }

            break;
        }
    }
}