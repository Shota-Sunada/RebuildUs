namespace RebuildUs.Modules.EndGame;

public class AdditionalTempData
{
    public static EWinCondition WinCondition = EWinCondition.Default;
    public static List<EWinCondition> AdditionalWinConditions = [];
    public static List<PlayerRoleInfo> PlayerRoles = [];
    public static float Timer = 0;

    public static void Clear()
    {
        PlayerRoles.Clear();
        AdditionalWinConditions.Clear();
        WinCondition = EWinCondition.Default;
        Timer = 0;
    }
}