namespace RebuildUs.Roles.Modifier;

public class Couple(PlayerControl lover1, PlayerControl lover2, Color color)
{
    public PlayerControl lover1 = lover1;
    public PlayerControl lover2 = lover2;
    public Color color = color;

    public string icon { get { return Helpers.Cs(color, " ♥"); } }
    public bool existing { get { return lover1 != null && lover2 != null && !lover1.Data.Disconnected && !lover2.Data.Disconnected; } }
    public bool alive { get { return lover1 != null && lover2 != null && lover1.IsAlive() && lover2.IsAlive(); } }
    public bool existingAndAlive { get { return existing && alive; } }
    public bool existingWithKiller
    {
        get
        {
            return existing
                && (lover1.IsRole(RoleType.Jackal) || lover2.IsRole(RoleType.Jackal))
                && (lover1.IsRole(RoleType.Sidekick) || lover2.IsRole(RoleType.Sidekick))
                && (lover1.IsTeamImpostor() || lover2.IsTeamImpostor()
            );
        }
    }
    public bool hasAliveKillingLover { get { return existingAndAlive && existingWithKiller; } }
}
