namespace RebuildUs.Modules.EndGame;

public class PlayerRoleInfo
{
    public string PlayerName { get; set; }
    public string NameSuffix { get; set; }
    public List<RoleInfo> Roles { get; set; }
    public string RoleNames { get; set; }
    public int ColorId = 0;
    public int TasksCompleted { get; set; }
    public int TasksTotal { get; set; }
    public EFinalStatus Status { get; set; }
    public int PlayerId { get; set; }
}