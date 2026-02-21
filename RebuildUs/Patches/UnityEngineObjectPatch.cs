using Object = UnityEngine.Object;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class UnityEngineObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(GameObject))]
    internal static void Prefix(GameObject obj)
    {
        Helpers.OnObjectDestroy(obj);
    }
}