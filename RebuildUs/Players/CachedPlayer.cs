using System.Collections;
using System.Reflection;

namespace RebuildUs.Players;

internal class CachedPlayer
{
    public static readonly Dictionary<IntPtr, CachedPlayer> PlayerPtrs = new();
    public static readonly List<CachedPlayer> AllPlayers = [];
    public static CachedPlayer LocalPlayer;

    public Transform Transform;
    public PlayerControl PlayerControl;
    public PlayerPhysics PlayerPhysics;
    public CustomNetworkTransform NetTransform;
    public NetworkedPlayerInfo Data;
    public byte PlayerId;

    public static implicit operator bool(CachedPlayer player)
    {
        return player != null && player.PlayerControl;
    }

    public static implicit operator PlayerControl(CachedPlayer player) => player.PlayerControl;
    public static implicit operator PlayerPhysics(CachedPlayer player) => player.PlayerPhysics;
}

[HarmonyPatch]
public static class CachedPlayerPatches
{
    [HarmonyPatch]
    private class CacheLocalPlayerPatch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = typeof(PlayerControl).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("Start"));
            return AccessTools.Method(type, nameof(IEnumerator.MoveNext));
        }

        [HarmonyPostfix]
        public static void SetLocalPlayer()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (!localPlayer)
            {
                CachedPlayer.LocalPlayer = null;
                return;
            }

            var cached = CachedPlayer.AllPlayers.FirstOrDefault(p => p.PlayerControl.Pointer == localPlayer.Pointer);
            if (cached == null) return;
            CachedPlayer.LocalPlayer = cached;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    [HarmonyPostfix]
    public static void CachePlayerPatch(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        var player = new CachedPlayer
        {
            Transform = __instance.transform,
            PlayerControl = __instance,
            PlayerPhysics = __instance.MyPhysics,
            NetTransform = __instance.NetTransform,
            Data = __instance.Data,
            PlayerId = __instance.PlayerId
        };
        CachedPlayer.AllPlayers.Add(player);
        CachedPlayer.PlayerPtrs[__instance.Pointer] = player;

#if DEBUG
        if (!player.PlayerControl || !player.PlayerPhysics || !player.NetTransform || !player.Transform)
        {
            Logger.LogError($"CachedPlayer {player.PlayerControl.name} has null fields on Awake");
        }
#endif
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    [HarmonyPostfix]
    public static void RemoveCachedPlayerPatch(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        CachedPlayer.AllPlayers.RemoveAll(p => p.PlayerControl.Pointer == __instance.Pointer);
        CachedPlayer.PlayerPtrs.Remove(__instance.Pointer);

        if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl.Pointer == __instance.Pointer)
        {
            CachedPlayer.LocalPlayer = null;
        }
    }

    [HarmonyPatch(typeof(NetworkedPlayerInfo), nameof(NetworkedPlayerInfo.Deserialize))]
    [HarmonyPostfix]
    public static void AddCachedDataOnDeserialize()
    {
        // 効率化のため、必要に応じて特定のプレイヤーのみを更新する仕組みへの変更も検討可能だが、
        // 現時点では全件同期を維持しつつ、安全性を高める。
        foreach (CachedPlayer cachedPlayer in CachedPlayer.AllPlayers)
        {
            if (cachedPlayer.PlayerControl != null)
            {
                cachedPlayer.Data = cachedPlayer.PlayerControl.Data;
                cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
            }
        }
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.AddPlayer))]
    [HarmonyPostfix]
    public static void AddCachedDataOnAddPlayer()
    {
        foreach (CachedPlayer cachedPlayer in CachedPlayer.AllPlayers)
        {
            if (cachedPlayer.PlayerControl != null)
            {
                cachedPlayer.Data = cachedPlayer.PlayerControl.Data;
                cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Deserialize))]
    [HarmonyPostfix]
    public static void SetCachedPlayerId(PlayerControl __instance)
    {
        if (CachedPlayer.PlayerPtrs.TryGetValue(__instance.Pointer, out var cached))
        {
            cached.PlayerId = __instance.PlayerId;
            cached.Data = __instance.Data;
        }
    }
}