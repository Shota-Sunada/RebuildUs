namespace RebuildUs.Modules;

internal static class CreateGame
{
    internal static void Customize(CreateGameOptions __instance)
    {
        // Disable HideNSeek
        __instance.SelectMode(0);
        __instance.modeButtons[1].gameObject.SetActive(false);
    }
}