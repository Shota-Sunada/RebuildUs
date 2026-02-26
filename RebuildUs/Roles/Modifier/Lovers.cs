namespace RebuildUs.Roles.Modifier;

internal static class Lovers
{
    internal static List<Couple> Couples = [];
    internal static Color Color = new Color32(232, 57, 185, byte.MaxValue);

    internal static Color[] LoverIconColors =
    [
        Color, // pink
        new Color32(255, 165, 0, 255), // orange
        new Color32(255, 255, 0, 255), // yellow
        new Color32(0, 255, 0, 255), // green
        new Color32(0, 0, 255, 255), // blue
        new Color32(0, 255, 255, 255), // light blue
        new Color32(255, 0, 0, 255), // red
    ];

    internal static bool BothDie { get => CustomOptionHolder.LoversBothDie.GetBool(); }
    internal static bool SeparateTeam { get => CustomOptionHolder.LoversSeparateTeam.GetBool(); }
    internal static bool TasksCount { get => CustomOptionHolder.LoversTasksCount.GetBool(); }
    internal static bool EnableChat { get => CustomOptionHolder.LoversEnableChat.GetBool(); }
    internal static bool HasTasks { get => TasksCount; }

    internal static string GetIcon(PlayerControl player)
    {
        if (IsLovers(player))
        {
            for (int i = 0; i < Couples.Count; i++)
            {
                Couple couple = Couples[i];
                if (couple.Lover1 == player || couple.Lover2 == player) return couple.Icon;
            }
        }

        return "";
    }

    internal static void AddCouple(PlayerControl player1, PlayerControl player2)
    {
        List<Color> availableColors = new(LoverIconColors);
        for (int i = 0; i < Couples.Count; i++)
        {
            Color color = Couples[i].Color;
            for (int j = availableColors.Count - 1; j >= 0; j--)
            {
                if (availableColors[j] == color)
                {
                    availableColors.RemoveAt(j);
                    break;
                }
            }
        }

        if (availableColors.Count > 0) Couples.Add(new(player1, player2, availableColors[0]));
    }

    internal static void EraseCouple(PlayerControl player)
    {
        Couples.RemoveAll(x => x.Lover1 == player || x.Lover2 == player);
    }

    internal static void SwapLovers(PlayerControl player1, PlayerControl player2)
    {
        int couple1 = Couples.FindIndex(x => x.Lover1 == player1 || x.Lover2 == player1);
        int couple2 = Couples.FindIndex(x => x.Lover1 == player2 || x.Lover2 == player2);

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

    internal static void KillLovers(PlayerControl player, PlayerControl killer = null)
    {
        if (!player.IsLovers()) return;

        if (SeparateTeam && TasksCount) player.ClearAllTasks();

        if (!BothDie) return;

        PlayerControl partner = GetPartner(player);
        if (partner != null)
        {
            if (!partner.Data.IsDead)
            {
                if (killer != null)
                    partner.MurderPlayer(partner);
                else
                    partner.Exiled();

                GameHistory.FinalStatuses[partner.PlayerId] = FinalStatus.Suicide;
            }

            if (SeparateTeam && TasksCount) partner.ClearAllTasks();
        }
    }

    internal static PlayerControl GetPartner(PlayerControl player)
    {
        Couple couple = GetCouple(player);
        if (couple != null) return player?.PlayerId == couple.Lover1?.PlayerId ? couple.Lover2 : couple.Lover1;

        return null;
    }

    internal static bool IsLovers(PlayerControl player)
    {
        return GetCouple(player) != null;
    }

    internal static void SetRole(PlayerControl player) { }

    internal static Couple GetCouple(PlayerControl player)
    {
        if (player == null) return null;
        for (int i = 0; i < Couples.Count; i++)
        {
            Couple pair = Couples[i];
            if (pair.Lover1?.PlayerId == player.PlayerId || pair.Lover2?.PlayerId == player.PlayerId) return pair;
        }

        return null;
    }

    internal static bool Existing(PlayerControl player)
    {
        return GetCouple(player)?.Existing == true;
    }

    internal static bool AnyAlive()
    {
        for (int i = 0; i < Couples.Count; i++)
        {
            if (Couples[i].Alive)
                return true;
        }

        return false;
    }

    internal static bool AnyNonKillingCouples()
    {
        for (int i = 0; i < Couples.Count; i++)
        {
            if (!Couples[i].HasAliveKillingLover)
                return true;
        }

        return false;
    }

    internal static bool ExistingAndAlive(PlayerControl player)
    {
        return GetCouple(player)?.ExistingAndAlive == true;
    }

    internal static bool ExistingWithKiller(PlayerControl player)
    {
        return GetCouple(player)?.ExistingWithKiller == true;
    }

    internal static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        EraseCouple(player);
    }

    internal static void Clear()
    {
        Couples = [];
    }
}