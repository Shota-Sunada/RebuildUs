namespace RebuildUs.Roles.Modifier;

public static class Lovers
{
    public static List<Couple> couples = [];
    public static Color color = new Color32(232, 57, 185, byte.MaxValue);

    public static Color[] loverIconColors = [
        color,                  // pink
        new Color32(255, 165, 0, 255), // orange
        new Color32(255, 255, 0, 255), // yellow
        new Color32(0, 255, 0, 255),   // green
        new Color32(0, 0, 255, 255),   // blue
        new Color32(0, 255, 255, 255), // light blue
        new Color32(255, 0, 0, 255),   // red
    ];

    public static bool bothDie { get { return CustomOptionHolder.loversBothDie.GetBool(); } }
    public static bool separateTeam { get { return CustomOptionHolder.loversSeparateTeam.GetBool(); } }
    public static bool tasksCount { get { return CustomOptionHolder.loversTasksCount.GetBool(); } }
    public static bool enableChat { get { return CustomOptionHolder.loversEnableChat.GetBool(); } }
    public static bool hasTasks { get { return tasksCount; } }

    public static string getIcon(PlayerControl player)
    {
        if (isLovers(player))
        {
            var couple = couples.Find(x => x.lover1 == player || x.lover2 == player);
            return couple.icon;
        }
        return "";
    }

    public static void addCouple(PlayerControl player1, PlayerControl player2)
    {
        var availableColors = new List<Color>(loverIconColors);
        foreach (var couple in couples)
        {
            availableColors.RemoveAll(x => x == couple.color);
        }
        couples.Add(new Couple(player1, player2, availableColors[0]));
    }

    public static void eraseCouple(PlayerControl player)
    {
        couples.RemoveAll(x => x.lover1 == player || x.lover2 == player);
    }

    public static void swapLovers(PlayerControl player1, PlayerControl player2)
    {
        var couple1 = couples.FindIndex(x => x.lover1 == player1 || x.lover2 == player1);
        var couple2 = couples.FindIndex(x => x.lover1 == player2 || x.lover2 == player2);

        // trying to swap within the same couple, just ignore
        if (couple1 == couple2) return;

        if (couple1 >= 0)
        {
            if (couples[couple1].lover1 == player1) couples[couple1].lover1 = player2;
            if (couples[couple1].lover2 == player1) couples[couple1].lover2 = player2;
        }

        if (couple2 >= 0)
        {
            if (couples[couple2].lover1 == player2) couples[couple2].lover1 = player1;
            if (couples[couple2].lover2 == player2) couples[couple2].lover2 = player1;
        }
    }

    public static void killLovers(PlayerControl player, PlayerControl killer = null)
    {
        if (!player.IsLovers()) return;

        if (separateTeam && tasksCount)
        {
            player.ClearAllTasks();
        }

        if (!bothDie) return;

        var partner = getPartner(player);
        if (partner != null)
        {
            if (!partner.Data.IsDead)
            {
                if (killer != null)
                {
                    partner.MurderPlayer(partner);
                }
                else
                {
                    partner.Exiled();
                }

                GameHistory.FinalStatuses[partner.PlayerId] = EFinalStatus.Suicide;
            }

            if (separateTeam && tasksCount)
            {
                partner.ClearAllTasks();
            }
        }
    }

    public static PlayerControl getPartner(PlayerControl player)
    {
        var couple = getCouple(player);
        if (couple != null)
        {
            return player?.PlayerId == couple.lover1?.PlayerId ? couple.lover2 : couple.lover1;
        }
        return null;
    }

    public static bool isLovers(PlayerControl player)
    {
        return getCouple(player) != null;
    }

    public static Couple getCouple(PlayerControl player)
    {
        foreach (var pair in couples)
        {
            if (pair.lover1?.PlayerId == player?.PlayerId || pair.lover2?.PlayerId == player?.PlayerId) return pair;
        }
        return null;
    }

    public static bool existing(PlayerControl player)
    {
        return getCouple(player)?.existing == true;
    }

    public static bool anyAlive()
    {
        foreach (var couple in couples)
        {
            if (couple.alive) return true;
        }
        return false;
    }

    public static bool anyNonKillingCouples()
    {
        foreach (var couple in couples)
        {
            if (!couple.hasAliveKillingLover) return true;
        }
        return false;
    }

    public static bool existingAndAlive(PlayerControl player)
    {
        return getCouple(player)?.existingAndAlive == true;
    }

    public static bool existingWithKiller(PlayerControl player)
    {
        return getCouple(player)?.existingWithKiller == true;
    }

    public static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        eraseCouple(player);
    }

    public static void Clear()
    {
        couples = [];
    }
}
