namespace RebuildUs.Modules.EndGame;

internal abstract class AdditionalTempData
{
    internal static WinCondition WinCondition = WinCondition.Default;
    internal static readonly List<WinCondition> AdditionalWinConditions = [];
    internal static readonly Dictionary<byte, PlayerRoleInfo> PlayerRoles = [];
    internal static readonly bool IsGm = false;
    internal static GameOverReason GameOverReason;
    internal static bool HasGameEndProcessed = false;
    internal static float Timer;

    internal static void Clear()
    {
        PlayerRoles.Clear();
        AdditionalWinConditions.Clear();
        WinCondition = WinCondition.Default;
        Timer = 0;
        HasGameEndProcessed = false;
    }
}