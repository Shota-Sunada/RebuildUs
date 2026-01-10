using RebuildUs.Roles.Crewmate;

namespace RebuildUs.Modules;

public static class Meeting
{
    private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
    {
        var dictionary = new Dictionary<byte, int>();
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.VotedFor != 252 && playerVoteArea.VotedFor != 255 && playerVoteArea.VotedFor != 254)
            {
                var player = Helpers.PlayerById(playerVoteArea.TargetPlayerId);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

                var additionalVotes = (Mayor.Exists && Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(ERoleType.Mayor)) ? Mayor.numVotes : 1; // Mayor vote
                dictionary[playerVoteArea.VotedFor] = dictionary.TryGetValue(playerVoteArea.VotedFor, out int currentVotes) ? currentVotes + additionalVotes : additionalVotes;
            }
        }

        return dictionary;
    }

    public static bool CheckForEndVoting(MeetingHud __instance)
    {
        if (__instance.playerStates.All(ps => ps.AmDead || ps.DidVote))
        {

            var self = CalculateVotes(__instance);
            var max = self.MaxPair(out bool tie);
            var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key && !v.IsDead);

            MeetingHud.VoterState[] array = new MeetingHud.VoterState[__instance.playerStates.Length];
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                array[i] = new MeetingHud.VoterState
                {
                    VoterId = playerVoteArea.TargetPlayerId,
                    VotedForId = playerVoteArea.VotedFor
                };
            }

            // RPCVotingComplete
            __instance.RpcVotingComplete(array, exiled, tie);
        }
        return false;
    }

    public static bool BloopAVoteIcon(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        var spriteRenderer = UnityEngine.Object.Instantiate(__instance.PlayerVotePrefab);
        var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() ||
                            (PlayerControl.LocalPlayer.Data.IsDead && MapOptions.GhostsSeeVotes) ||
                            (PlayerControl.LocalPlayer.IsRole(ERoleType.Mayor) && Mayor.mayorCanSeeVoteColors && TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data).Item1 >= Mayor.mayorTasksNeededToSeeVoteColors);
        if (showVoteColors)
        {
            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
        }
        else
        {
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        }

        var transform = spriteRenderer.transform;
        transform.SetParent(parent);
        transform.localScale = Vector3.zero;
        var component = parent.GetComponent<PlayerVoteArea>();
        if (component != null)
        {
            spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
        }

        __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }

    public static bool PopulateVotes(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        // // Swapper swap
        // PlayerVoteArea swapped1 = null;
        // PlayerVoteArea swapped2 = null;
        // foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
        // {
        //     if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
        //     if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
        // }
        // bool doSwap = swapped1 != null && swapped2 != null && Swapper.swapper != null && !Swapper.swapper.Data.IsDead;
        // if (doSwap)
        // {
        //     __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, swapped1.transform.localPosition, swapped2.transform.localPosition, 1.5f));
        //     __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition, swapped1.transform.localPosition, 1.5f));
        // }

        __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
        var num = 0;
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            var targetPlayerId = playerVoteArea.TargetPlayerId;
            // Swapper change playerVoteArea that gets the votes
            // if (doSwap && playerVoteArea.TargetPlayerId == swapped1.TargetPlayerId) playerVoteArea = swapped2;
            // else if (doSwap && playerVoteArea.TargetPlayerId == swapped2.TargetPlayerId) playerVoteArea = swapped1;

            playerVoteArea.ClearForResults();
            var num2 = 0;
            Dictionary<int, int> votesApplied = [];
            for (int j = 0; j < states.Length; j++)
            {
                var voterState = states[j];
                var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                var voter = Helpers.PlayerById(voterState.VoterId);
                if (playerById == null)
                {
                    Debug.LogError(string.Format("Couldn't find player info for voter: {0}", voterState.VoterId));
                }
                else if (i == 0 && voterState.SkippedVote && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                    num++;
                }
                else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                    num2++;
                }

                if (!votesApplied.ContainsKey(voter.PlayerId))
                {
                    votesApplied[voter.PlayerId] = 0;
                }
                votesApplied[voter.PlayerId]++;

                // Major vote, redo this iteration to place a second vote
                if (voter.IsRole(ERoleType.Mayor) && votesApplied[voter.PlayerId] < Mayor.numVotes)
                {
                    j--;
                }
            }
        }
        return false;
    }
}