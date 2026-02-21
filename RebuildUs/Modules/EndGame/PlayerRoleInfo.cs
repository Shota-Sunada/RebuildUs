namespace RebuildUs.Modules.EndGame;

internal sealed class PlayerRoleInfo
{
    internal int ColorId = 0;
    internal string PlayerName { get; set; }
    internal string NameSuffix { get; set; }
    internal List<RoleInfo> Roles { get; set; }
    internal string RoleNames { get; set; }
    internal int TasksCompleted { get; set; }
    internal int TasksTotal { get; set; }
    internal FinalStatus Status { get; set; }
    internal int PlayerId { get; set; }
}