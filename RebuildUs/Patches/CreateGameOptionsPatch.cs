namespace RebuildUs.Patches;

[HarmonyPatch]
public static class CreateGameOptionsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
    public static void StartPostfix(CreateGameOptions __instance)
    {
        CreateGame.Customize(__instance);
    }
}