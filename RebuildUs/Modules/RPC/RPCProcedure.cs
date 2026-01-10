using RebuildUs.Modules;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Modules.RPC;

public static class RPCProcedure
{
    public static void Handle(CustomRPC callId, MessageReader reader)
    {
        switch (callId)
        {
            case CustomRPC.ShareOptions:
                HandleShareOptions(reader);
                break;
            case CustomRPC.UncheckedSetTasks:
                UncheckedSetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
                break;
            default:
                break;
        }
    }

    private static void HandleShareOptions(MessageReader reader)
    {
        byte amount = reader.ReadByte();
        for (int i = 0; i < amount; i++)
        {
            uint id = reader.ReadPackedUInt32();
            uint selection = reader.ReadPackedUInt32();
            var option = CustomOption.AllOptions.FirstOrDefault(x => x.Id == (int)id);
            option?.UpdateSelection((int)selection);
        }
    }

    public static void UpdateMeeting(byte targetId, bool dead = true)
    {
        if (MeetingHud.Instance)
        {
            foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
            {
                if (pva.TargetPlayerId == targetId)
                {
                    pva.SetDead(pva.DidReport, dead);
                    pva.Overlay.gameObject.SetActive(dead);
                }

                // Give players back their vote if target is shot dead
                if (Helpers.RefundVotes && dead)
                {
                    if (pva.VotedFor != targetId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.PlayerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingHud.Instance.CheckForEndVoting();
            }
        }
    }

    public static void UncheckedSetTasks(byte playerId, byte[] taskTypeIds)
    {
        var player = Helpers.PlayerById(playerId);
        player.ClearAllTasks();

        player.Data.SetTasks(taskTypeIds);
    }
}