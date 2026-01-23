namespace RebuildUs.Modules.EndGame;

public enum ECustomGameOverReason
{
    JesterWin = 10,
    ArsonistWin,
    VultureWin,
    TeamJackalWin,
    MiniLose,
    LoversWin,
    ForceEnd
}
public enum EWinCondition
{
    Default,
    JesterWin,
    ArsonistWin,
    VultureWin,
    JackalWin,
    MiniLose,
    LoversTeamWin,
    LoversSoloWin,
    EveryoneDied,
    ForceEnd,
}