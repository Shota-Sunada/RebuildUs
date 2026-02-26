namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class UnityEngineObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityObject), nameof(UnityObject.Destroy), typeof(GameObject))]
    internal static void Prefix(GameObject obj)
    {
        Helpers.OnObjectDestroy(obj);
    }
}