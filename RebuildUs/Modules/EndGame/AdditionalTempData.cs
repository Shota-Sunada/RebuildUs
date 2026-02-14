namespace RebuildUs.Modules.EndGame;

public class AdditionalTempData
{
    public static WinCondition WinCondition = WinCondition.Default;
    public static List<WinCondition> AdditionalWinConditions = [];
    public static List<PlayerRoleInfo> PlayerRoles = [];
    public static bool IsGM = false;
    public static GameOverReason GameOverReason;
    public static float Timer = 0;

    public static void Clear()
    {
        PlayerRoles.Clear();
        AdditionalWinConditions.Clear();
        WinCondition = WinCondition.Default;
        Timer = 0;
    }
}