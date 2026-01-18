namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MeetingHudPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    public static bool CheckForEndVotingPrefix(MeetingHud __instance)
    {
        return Meeting.CheckForEndVoting(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static bool BloopAVoteIconPrefix(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        return Meeting.BloopAVoteIcon(__instance, voterPlayer, index, parent);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    public static bool PopulateResultsPrefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        return Meeting.PopulateVotes(__instance, states);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        Meeting.Update(__instance);
        ShowHost.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
    public static bool SelectPrefix(ref bool __result, MeetingHud __instance, [HarmonyArgument(0)] int suspectStateIdx)
    {
        return Meeting.Select(ref __result, __instance, suspectStateIdx);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static void VotingCompletePostfix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states, NetworkedPlayerInfo exiled, bool tie)
    {
        Meeting.VotingComplete(__instance, states, exiled, tie);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    public static void ServerStartPostfix(MeetingHud __instance)
    {
        Meeting.populateButtonsPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
    public static void DeserializePostfix(MeetingHud __instance, MessageReader reader, bool initialState)
    {
        Meeting.Deserialize(__instance, reader, initialState);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static void StartPostfix(MeetingHud __instance)
    {
        ShowHost.Setup(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static void ClosePostfix(MeetingHud __instance)
    {
        if (Helpers.IsPolus && CustomOptionHolder.PolusRandomSpawn.GetBool())
        {
            if (AmongUsClient.Instance.AmHost)
            {
                foreach (PlayerControl player in CachedPlayer.AllPlayers)
                {
                    System.Random rand = new();
                    int randVal = rand.Next(0, 6);
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PolusRandomSpawn);
                    sender.Write(player.Data.PlayerId);
                    sender.Write((byte)randVal);
                    RPCProcedure.PolusRandomSpawn(player.Data.PlayerId, (byte)randVal);
                }
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateButtons))]
    public static bool PopulateButtonsPrefix(MeetingHud __instance, byte reporter)
    {
        return false;
    }
}