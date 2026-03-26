namespace RebuildUs.Modules.CustomGameModes;

internal static class GameModeManager
{
    private static readonly Dictionary<string, GameModeBase> _gameModes = new();

    public static GameModeBase CurrentGameMode { get; private set; }

    internal static void Register(GameModeBase gameMode)
    {
        _gameModes[gameMode.InternalName] = gameMode;
    }

    public static void SetGameMode(string name)
    {
        if (name == null || !_gameModes.ContainsKey(name))
        {
            CurrentGameMode = null;
        }
        else
        {
            CurrentGameMode = _gameModes[name];
        }
    }

    // Temporary logic to setup game mode for testing, since options aren't added yet.
    public static void InitTesting()
    {
        Register(new BattleRoyaleMode());
        Register(new HotPotatoMode());

        // For now, uncomment one of these to test:
        // SetGameMode("BattleRoyale");
        // SetGameMode("HotPotato");
        SetGameMode(null); // Defaults to standard normal mode
    }
}
