namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class CreateGameOptionsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
    internal static void StartPostfix(CreateGameOptions __instance)
    {
        CreateGame.Customize(__instance);
    }
}