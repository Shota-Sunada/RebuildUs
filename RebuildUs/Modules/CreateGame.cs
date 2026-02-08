namespace RebuildUs.Modules;

public static class CreateGame
{
    public static void Customize(CreateGameOptions __instance)
    {
        // Disable HideNSeek
        __instance.SelectMode(0);
        __instance.modeButtons[1].gameObject.SetActive(false);
    }
}
