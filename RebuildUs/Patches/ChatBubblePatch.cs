using RebuildUs.Roles.Neutral;

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
            && (sourcePlayer.IsRole(ERoleType.Spy) || (sourcePlayer.IsRole(ERoleType.Sidekick) && Sidekick.GetRole(sourcePlayer).WasTeamRed) || (sourcePlayer.IsRole(ERoleType.Jackal) && Jackal.GetRole(sourcePlayer).WasTeamRed))
            && __instance != null)
        {
            __instance.NameText.color = Palette.ImpostorRed;
        }
    }
}