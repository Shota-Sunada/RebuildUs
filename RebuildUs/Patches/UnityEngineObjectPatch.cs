namespace RebuildUs.Patches;

[HarmonyPatch]
public static class UnityEngineObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), [typeof(GameObject)])]
    public static void Prefix(GameObject obj)
    {
        Helpers.OnObjectDestroy(obj);
    }
}