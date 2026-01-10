using RebuildUs.Roles.Crewmate;

namespace RebuildUs.Roles;

public partial class RoleInfo
{
    public static RoleInfo Jester;
    public static RoleInfo Mayor;
    public static RoleInfo Engineer;

    public static void Load()
    {
        AllRoleInfos.Add(Jester = new("Jester", Neutral.Jester.RoleColor, CustomOptionHolder.jesterSpawnRate, ERoleType.Jester));
        AllRoleInfos.Add(Mayor = new("Mayor", Crewmate.Mayor.RoleColor, CustomOptionHolder.mayorSpawnRate, ERoleType.Mayor));
        AllRoleInfos.Add(Engineer = new("Engineer", Crewmate.Engineer.RoleColor, CustomOptionHolder.engineerSpawnRate, ERoleType.Engineer));
    }
}