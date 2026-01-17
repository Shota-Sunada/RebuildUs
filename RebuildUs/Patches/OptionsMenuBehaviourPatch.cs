namespace RebuildUs.Patches;

public static class OptionsMenuBehaviourPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public static void StartPostfix(OptionsMenuBehaviour __instance)
    {
        ClientOptions.Start(__instance);
    }
}