namespace RebuildUs.Roles.Modifier;

public sealed class Couple(PlayerControl lover1, PlayerControl lover2, Color color)
{
    public Color Color = color;
    public PlayerControl Lover1 = lover1;
    public PlayerControl Lover2 = lover2;

    public string Icon
    {
        get => Helpers.Cs(Color, " ♥");
    }

    public bool Existing
    {
        get => Lover1 != null && Lover2 != null && !Lover1.Data.Disconnected && !Lover2.Data.Disconnected;
    }

    public bool Alive
    {
        get => Lover1 != null && Lover2 != null && Lover1.IsAlive() && Lover2.IsAlive();
    }

    public bool ExistingAndAlive
    {
        get => Existing && Alive;
    }

    public bool ExistingWithKiller
    {
        get => Existing && (Lover1.IsRole(RoleType.Jackal) || Lover2.IsRole(RoleType.Jackal)) && (Lover1.IsRole(RoleType.Sidekick) || Lover2.IsRole(RoleType.Sidekick)) && (Lover1.IsTeamImpostor() || Lover2.IsTeamImpostor());
    }

    public bool HasAliveKillingLover
    {
        get => ExistingAndAlive && ExistingWithKiller;
    }
}
