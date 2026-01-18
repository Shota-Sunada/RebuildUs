namespace RebuildUs.Roles.Modifier;

public static class Lovers
{
    public static List<Couple> Couples = [];
    public static Color Color = new Color32(232, 57, 185, byte.MaxValue);

    public static Color[] LoverIconColors = [
        Color,                  // pink
        new Color32(255, 165, 0, 255), // orange
        new Color32(255, 255, 0, 255), // yellow
        new Color32(0, 255, 0, 255),   // green
        new Color32(0, 0, 255, 255),   // blue
        new Color32(0, 255, 255, 255), // light blue
        new Color32(255, 0, 0, 255),   // red
    ];

    public static bool BothDie { get { return CustomOptionHolder.LoversBothDie.GetBool(); } }
    public static bool SeparateTeam { get { return CustomOptionHolder.LoversSeparateTeam.GetBool(); } }
    public static bool TasksCount { get { return CustomOptionHolder.LoversTasksCount.GetBool(); } }
    public static bool EnableChat { get { return CustomOptionHolder.LoversEnableChat.GetBool(); } }
    public static bool HasTasks { get { return TasksCount; } }

    public static string GetIcon(PlayerControl player)
    {
        if (IsLovers(player))
        {
            var couple = Couples.Find(x => x.Lover1 == player || x.Lover2 == player);
            return couple.Icon;
        }
        return "";
    }

    public static void AddCouple(PlayerControl player1, PlayerControl player2)
    {
        var availableColors = new List<Color>(LoverIconColors);
        foreach (var couple in Couples)
        {
            availableColors.RemoveAll(x => x == couple.Color);
        }
        Couples.Add(new Couple(player1, player2, availableColors[0]));
    }

    public static void EraseCouple(PlayerControl player)
    {
        Couples.RemoveAll(x => x.Lover1 == player || x.Lover2 == player);
    }

    public static void SwapLovers(PlayerControl player1, PlayerControl player2)
    {
        var couple1 = Couples.FindIndex(x => x.Lover1 == player1 || x.Lover2 == player1);
        var couple2 = Couples.FindIndex(x => x.Lover1 == player2 || x.Lover2 == player2);

        // trying to swap within the same couple, just ignore
        if (couple1 == couple2) return;

        if (couple1 >= 0)
        {
            if (Couples[couple1].Lover1 == player1) Couples[couple1].Lover1 = player2;
            if (Couples[couple1].Lover2 == player1) Couples[couple1].Lover2 = player2;
        }

        if (couple2 >= 0)
        {
            if (Couples[couple2].Lover1 == player2) Couples[couple2].Lover1 = player1;
            if (Couples[couple2].Lover2 == player2) Couples[couple2].Lover2 = player1;
        }
    }

    public static void KillLovers(PlayerControl player, PlayerControl killer = null)
    {
        if (!player.IsLovers()) return;

        if (SeparateTeam && TasksCount)
        {
            player.ClearAllTasks();
        }

        if (!BothDie) return;

        var partner = GetPartner(player);
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

            if (SeparateTeam && TasksCount)
            {
                partner.ClearAllTasks();
            }
        }
    }

    public static PlayerControl GetPartner(PlayerControl player)
    {
        var couple = GetCouple(player);
        if (couple != null)
        {
            return player?.PlayerId == couple.Lover1?.PlayerId ? couple.Lover2 : couple.Lover1;
        }
        return null;
    }

    public static bool IsLovers(PlayerControl player)
    {
        return GetCouple(player) != null;
    }

    public static void SetRole(PlayerControl player)
    {
    }

    public static Couple GetCouple(PlayerControl player)
    {
        foreach (var pair in Couples)
        {
            if (pair.Lover1?.PlayerId == player?.PlayerId || pair.Lover2?.PlayerId == player?.PlayerId) return pair;
        }
        return null;
    }

    public static bool Existing(PlayerControl player)
    {
        return GetCouple(player)?.Existing == true;
    }

    public static bool AnyAlive()
    {
        foreach (var couple in Couples)
        {
            if (couple.Alive) return true;
        }
        return false;
    }

    public static bool AnyNonKillingCouples()
    {
        foreach (var couple in Couples)
        {
            if (!couple.HasAliveKillingLover) return true;
        }
        return false;
    }

    public static bool ExistingAndAlive(PlayerControl player)
    {
        return GetCouple(player)?.ExistingAndAlive == true;
    }

    public static bool ExistingWithKiller(PlayerControl player)
    {
        return GetCouple(player)?.ExistingWithKiller == true;
    }

    public static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        EraseCouple(player);
    }

    public static void Clear()
    {
        Couples = [];
    }
}