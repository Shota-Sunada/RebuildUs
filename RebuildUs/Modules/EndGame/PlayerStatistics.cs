namespace RebuildUs.Modules.EndGame;

public sealed class PlayerStatistics
{
    public PlayerStatistics()
    {
        GetPlayerCounts();
    }

    public int TeamImpostorsAlive { get; set; }
    public int TeamJackalAlive { get; set; }
    public int TeamLoversAlive { get; set; }
    public int CouplesAlive { get; set; }
    public int TeamCrew { get; set; }
    public int NeutralAlive { get; set; }
    public int TotalAlive { get; set; }
    public int TeamImpostorLovers { get; set; }
    public int TeamJackalLovers { get; set; }

    private bool IsLover(NetworkedPlayerInfo p)
    {
        foreach (var couple in Lovers.Couples)
        {
            if (p.PlayerId == couple.Lover1.PlayerId || p.PlayerId == couple.Lover2.PlayerId)
                return true;
        }

        return false;
    }

    private void GetPlayerCounts()
    {
        var numJackalAlive = 0;
        var numImpostorsAlive = 0;
        var numTotalAlive = 0;
        var numNeutralAlive = 0;
        var numCrewmate = 0;

        var numLoversAlive = 0;
        var numCouplesAlive = 0;
        var impLovers = 0;
        var jackalLovers = 0;

        var loversId = new HashSet<byte>();
        var couples = Lovers.Couples;
        for (var i = 0; i < couples.Count; i++)
        {
            var couple = couples[i];
            if (couple == null) continue;
            loversId.Add(couple.Lover1.PlayerId);
            loversId.Add(couple.Lover2.PlayerId);
            if (couple.Alive) numCouplesAlive++;
        }

        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (playerInfo == null) continue;
            var obj = playerInfo.Object;

            if (!playerInfo.Disconnected)
            {
                if (obj != null && obj.IsTeamCrewmate()) numCrewmate++;
                if (!playerInfo.IsDead && (obj == null || !obj.IsGm()))
                {
                    numTotalAlive++;

                    var lover = loversId.Contains(playerInfo.PlayerId);
                    if (lover) numLoversAlive++;

                    if (playerInfo.Role.IsImpostor)
                    {
                        numImpostorsAlive++;
                        if (lover) impLovers++;
                    }

                    if (obj != null)
                    {
                        if (Jackal.Exists && obj.IsRole(RoleType.Jackal))
                        {
                            numJackalAlive++;
                            if (lover) jackalLovers++;
                        }
                        else if (Sidekick.Exists && obj.IsRole(RoleType.Sidekick))
                        {
                            numJackalAlive++;
                            if (lover) jackalLovers++;
                        }

                        if (obj.IsNeutral()) numNeutralAlive++;
                    }
                }
            }
        }

        // In the special case of Mafia being enabled, but only the janitor's left alive,
        // count it as zero impostors alive bc they can't actually do anything.
        if (Mafia.IsGodfatherDead && Mafia.IsMafiosoDead && !Mafia.IsJanitorDead) numImpostorsAlive = 0;

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
