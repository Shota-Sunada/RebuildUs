namespace RebuildUs.Modules.EndGame;

public class PlayerStatistics
{
    public int TeamImpostorsAlive { get; set; }
    public int TeamJackalAlive { get; set; }
    public int TeamLoversAlive { get; set; }
    public int CouplesAlive { get; set; }
    public int TeamCrew { get; set; }
    public int NeutralAlive { get; set; }
    public int TotalAlive { get; set; }
    public int TeamImpostorLovers { get; set; }
    public int TeamJackalLovers { get; set; }

    public PlayerStatistics()
    {
        GetPlayerCounts();
    }

    private bool IsLover(NetworkedPlayerInfo p)
    {
        foreach (var couple in Lovers.Couples)
        {
            if (p.PlayerId == couple.Lover1.PlayerId || p.PlayerId == couple.Lover2.PlayerId) return true;
        }
        return false;
    }

    private void GetPlayerCounts()
    {
        int numJackalAlive = 0;
        int numImpostorsAlive = 0;
        int numTotalAlive = 0;
        int numNeutralAlive = 0;
        int numCrewmate = 0;

        int numLoversAlive = 0;
        int numCouplesAlive = 0;
        int impLovers = 0;
        int jackalLovers = 0;

        foreach (var playerInfo in GameData.Instance.AllPlayers)
        {
            if (!playerInfo.Disconnected)
            {
                if (playerInfo.Object.IsTeamCrewmate()) numCrewmate++;
                if (!playerInfo.IsDead && !playerInfo.Object.IsGM())
                {
                    numTotalAlive++;

                    bool lover = IsLover(playerInfo);
                    if (lover) numLoversAlive++;

                    if (playerInfo.Role.IsImpostor)
                    {
                        numImpostorsAlive++;
                        if (lover) impLovers++;
                    }
                    if (Jackal.Exists && playerInfo.Object.IsRole(RoleType.Jackal))
                    {
                        numJackalAlive++;
                        if (lover) jackalLovers++;
                    }
                    if (Sidekick.Exists && playerInfo.Object.IsRole(RoleType.Sidekick))
                    {
                        numJackalAlive++;
                        if (lover) jackalLovers++;
                    }

                    if (playerInfo.Object.IsNeutral()) numNeutralAlive++;
                }
            }
        }

        foreach (var couple in Lovers.Couples)
        {
            if (couple.Alive) numCouplesAlive++;
        }

        // In the special case of Mafia being enabled, but only the janitor's left alive,
        // count it as zero impostors alive bc they can't actually do anything.
        if (Mafia.IsGodfatherDead && Mafia.IsMafiosoDead && !Mafia.IsJanitorDead)
        {
            numImpostorsAlive = 0;
        }

        TeamCrew = numCrewmate;
        TeamJackalAlive = numJackalAlive;
        TeamImpostorsAlive = numImpostorsAlive;
        TeamLoversAlive = numLoversAlive;
        NeutralAlive = numNeutralAlive;
        TotalAlive = numTotalAlive;
        CouplesAlive = numCouplesAlive;
        TeamImpostorLovers = impLovers;
        TeamJackalLovers = jackalLovers;
    }
}