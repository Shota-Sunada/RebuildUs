namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ChatBubblePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
    {
        var sourcePlayer = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor() && ((Spy.spy != null && sourcePlayer.PlayerId == Spy.spy.PlayerId) || (Sidekick.sidekick != null && Sidekick.wasTeamRed && sourcePlayer.PlayerId == Sidekick.sidekick.PlayerId) || (Jackal.jackal != null && Jackal.wasTeamRed && sourcePlayer.PlayerId == Jackal.jackal.PlayerId)) && __instance != null)
        {
            __instance.NameText.color = Palette.ImpostorRed;
        }
    }
}