using Hazel.Udp;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class UnityUdpClientConnectionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityUdpClientConnection), nameof(UnityUdpClientConnection.ConnectAsync))]
    public static void ConnectAsyncPrefix(UnityUdpClientConnection __instance, Il2CppStructArray<byte> bytes)
    {
        __instance.KeepAliveInterval = 2000;
        __instance.DisconnectTimeoutMs = __instance.EndPoint.Port == 22025 ? 0 : 15000;
    }
}
