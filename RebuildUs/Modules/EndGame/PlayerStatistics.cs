namespace RebuildUs.Modules.EndGame;

internal sealed class PlayerStatistics
{
    internal PlayerStatistics()
    {
        GetPlayerCounts();
    }

    internal int TeamImpostorsAlive { get; set; }
    internal int TeamJackalAlive { get; set; }
    internal int TeamLoversAlive { get; set; }
    internal int CouplesAlive { get; set; }
    internal int TeamCrew { get; set; }
    internal int NeutralAlive { get; set; }
    internal int TotalAlive { get; set; }
    internal int TeamImpostorLovers { get; set; }
    internal int TeamJackalLovers { get; set; }

    private bool IsLover(NetworkedPlayerInfo p)
    {
        foreach (var couple in Lovers.Couples)
        {
            if (p.PlayerId == couple.Lover1.PlayerId || p.PlayerId == couple.Lover2.PlayerId)
            {
                return true;
            }
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

        HashSet<byte> loversId = [];
        var couples = Lovers.Couples;
        foreach (var couple in couples)
        {
            if (couple == null)
            {
                continue;
            }
            loversId.Add(couple.Lover1.PlayerId);
            loversId.Add(couple.Lover2.PlayerId);
            if (couple.Alive)
            {
                numCouplesAlive++;
            }
        }

        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (playerInfo == null)
            {
                continue;
            }
            var obj = playerInfo.Object;

            if (playerInfo.Disconnected)
            {
                continue;
            }
            if (obj != null && obj.IsTeamCrewmate())
            {
                numCrewmate++;
            }
            if (playerInfo.IsDead || obj != null && obj.IsGm())
            {
                continue;
            }
            numTotalAlive++;

            var lover = loversId.Contains(playerInfo.PlayerId);
            if (lover)
            {
                numLoversAlive++;
            }

            if (playerInfo.Role.IsImpostor)
            {
                numImpostorsAlive++;
                if (lover)
                {
                    impLovers++;
                }
            }

            if (obj == null)
            {
                continue;
            }
            if (Jackal.Exists && obj.IsRole(RoleType.Jackal) || Sidekick.Exists && obj.IsRole(RoleType.Sidekick))
            {
                numJackalAlive++;
                if (lover)
                {
                    jackalLovers++;
                }
            }

            if (obj.IsNeutral())
            {
                numNeutralAlive++;
            }
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