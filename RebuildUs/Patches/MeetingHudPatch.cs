using Random = System.Random;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MeetingHudPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    internal static bool CheckForEndVotingPrefix(MeetingHud __instance)
    {
        return Meeting.CheckForEndVoting(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    internal static bool BloopAVoteIconPrefix(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        return Meeting.BloopAVoteIcon(__instance, voterPlayer, index, parent);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    internal static bool PopulateResultsPrefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        return Meeting.PopulateResults(__instance, states);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    internal static void UpdatePostfix(MeetingHud __instance)
    {
        Meeting.Update(__instance);
        ShowHost.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
    internal static bool SelectPrefix(ref bool __result, MeetingHud __instance, [HarmonyArgument(0)] int suspectStateIdx)
    {
        return Meeting.Select(ref __result, __instance, suspectStateIdx);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    internal static void VotingCompletePostfix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states, NetworkedPlayerInfo exiled, bool tie)
    {
        Meeting.VotingComplete(__instance, states, exiled, tie);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    internal static void ServerStartPostfix(MeetingHud __instance)
    {
        Meeting.PopulateButtonsPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
    internal static void DeserializePostfix(MeetingHud __instance, MessageReader reader, bool initialState)
    {
        Meeting.Deserialize(__instance, reader, initialState);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    internal static void StartPostfix(MeetingHud __instance)
    {
        ShowHost.Setup(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
    internal static void OnDestroyPostfix() { }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    internal static void ClosePostfix(MeetingHud __instance)
    {
        if (!Helpers.IsPolus || !CustomOptionHolder.PolusRandomSpawn.GetBool()) return;
        if (!AmongUsClient.Instance.AmHost) return;
        Random rand = RebuildUs.Rnd;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            int randVal = rand.Next(0, 6);
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PolusRandomSpawn);
            sender.Write(player.Data.PlayerId);
            sender.Write((byte)randVal);
            RPCProcedure.PolusRandomSpawn(player.Data.PlayerId, (byte)randVal);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateButtons))]
    internal static bool PopulateButtonsPrefix(MeetingHud __instance, byte reporter)
    {
        return false;
    }
}