using Object = UnityEngine.Object;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class UnityEngineObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(GameObject))]
    public static void Prefix(GameObject obj)
    {
        Helpers.OnObjectDestroy(obj);
    }
}
