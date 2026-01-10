namespace RebuildUs.Patches;

[HarmonyPatch]
public static class UnityEngineObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), [typeof(GameObject)])]
    public static void Prefix(GameObject obj)
    {
        // submerged
        if (!SubmergedCompatibility.IsSubmerged) return;
        if (obj.name.Contains("ExileCutscene"))
        {
            Exile.WrapUpPostfix(obj.GetComponent<ExileController>().initData.networkedPlayer?.Object);
        }
        else if (obj.name.Contains("SpawnInMinigame"))
        {
            // AntiTeleport.setPosition();
            // Chameleon.lastMoved.Clear();
        }
    }
}