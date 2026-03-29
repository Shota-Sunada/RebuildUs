namespace RebuildUs.Modules.CustomGameModes;

internal static class GameModeManager
{
    private static readonly Dictionary<CustomGamemode, GameModeBase> _gameModes = new();

    public static CustomGamemode CurrentGameMode { get; private set; }
    public static GameModeBase CurrentGameModeInstance => _gameModes.ContainsKey(CurrentGameMode) ? _gameModes[CurrentGameMode] : null;

    internal static void Register(GameModeBase gameMode)
    {
        _gameModes[gameMode.Gamemode] = gameMode;
    }

    public static void SetGameMode(CustomGamemode gamemode)
    {
        if (!_gameModes.ContainsKey(gamemode))
        {
            CurrentGameMode = CustomGamemode.Normal;
        }
        else
        {
            CurrentGameMode = gamemode;
        }
    }

    // Temporary logic to setup game mode for testing, since options aren't added yet.
    public static void Initialize()
    {
        Register(new HideNSeekMode());
        Register(new BattleRoyaleMode());
        Register(new HotPotatoMode());

        // For now, uncomment one of these to test:
        // SetGameMode("BattleRoyale");
        // SetGameMode("HotPotato");
        SetGameMode(CustomGamemode.Normal); // Defaults to standard normal mode
    }

    internal static void OnIntroDestroyed()
    {
        if (CurrentGameMode != CustomGamemode.Normal)
        {
            CurrentGameModeInstance?.OnIntroDestroyed();
        }
    }
}
