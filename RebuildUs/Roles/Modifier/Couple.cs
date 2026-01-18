namespace RebuildUs.Roles.Modifier;

public class Couple(PlayerControl lover1, PlayerControl lover2, Color color)
{
    public PlayerControl Lover1 = lover1;
    public PlayerControl Lover2 = lover2;
    public Color Color = color;

    public string Icon { get { return Helpers.Cs(Color, " ♥"); } }
    public bool Existing { get { return Lover1 != null && Lover2 != null && !Lover1.Data.Disconnected && !Lover2.Data.Disconnected; } }
    public bool Alive { get { return Lover1 != null && Lover2 != null && Lover1.IsAlive() && Lover2.IsAlive(); } }
    public bool ExistingAndAlive { get { return Existing && Alive; } }
    public bool ExistingWithKiller
    {
        get
        {
            return Existing
                && (Lover1.IsRole(RoleType.Jackal) || Lover2.IsRole(RoleType.Jackal))
                && (Lover1.IsRole(RoleType.Sidekick) || Lover2.IsRole(RoleType.Sidekick))
                && (Lover1.IsTeamImpostor() || Lover2.IsTeamImpostor()
            );
        }
    }
    public bool HasAliveKillingLover { get { return ExistingAndAlive && ExistingWithKiller; } }
}