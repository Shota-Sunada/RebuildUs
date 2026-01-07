using RebuildUs.Modules.RPC;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerControlPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static void PlayerControlHandleRpcPostfix(byte callId, MessageReader reader)
    {
        RPCProcedure.Handle((CustomRPC)callId, reader);
    }
}