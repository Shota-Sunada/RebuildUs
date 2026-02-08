namespace RebuildUs.Modules.EndGame;

public sealed class PlayerRoleInfo
{
    public int ColorId = 0;
    public string PlayerName { get; set; }
    public string NameSuffix { get; set; }
    public List<RoleInfo> Roles { get; set; }
    public string RoleNames { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public FinalStatus Status { get; set; }
    public int PlayerId { get; set; }
}
