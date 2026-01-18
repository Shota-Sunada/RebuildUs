namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        var sourcePlayer = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor()
            && (sourcePlayer.IsRole(RoleType.Spy) || (sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.GetRole(sourcePlayer).WasTeamRed) || (sourcePlayer.IsRole(RoleType.Jackal) && Jackal.GetRole(sourcePlayer).WasTeamRed))
            && __instance != null)
        {
            __instance.NameText.color = Palette.ImpostorRed;
        }
    }
}