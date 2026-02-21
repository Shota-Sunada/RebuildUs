namespace RebuildUs.Roles.Modifier;

internal sealed class Couple(PlayerControl lover1, PlayerControl lover2, Color color)
{
    internal Color Color = color;
    internal PlayerControl Lover1 = lover1;
    internal PlayerControl Lover2 = lover2;

    internal string Icon { get => Helpers.Cs(Color, " ♥"); }
    internal bool Existing { get => Lover1 != null && Lover2 != null && !Lover1.Data.Disconnected && !Lover2.Data.Disconnected; }
    internal bool Alive { get => Lover1 != null && Lover2 != null && Lover1.IsAlive() && Lover2.IsAlive(); }
    internal bool ExistingAndAlive { get => Existing && Alive; }

    internal bool ExistingWithKiller
    {
        get => Existing && (Lover1.IsRole(RoleType.Jackal) || Lover2.IsRole(RoleType.Jackal)) && (Lover1.IsRole(RoleType.Sidekick) || Lover2.IsRole(RoleType.Sidekick)) && (Lover1.IsTeamImpostor() || Lover2.IsTeamImpostor());
    }

    internal bool HasAliveKillingLover { get => ExistingAndAlive && ExistingWithKiller; }
}