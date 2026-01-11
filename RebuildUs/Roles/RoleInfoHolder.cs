namespace RebuildUs.Roles;

public partial class RoleInfo
{
    public static RoleInfo Jester;
    public static RoleInfo Mayor;
    public static RoleInfo Engineer;
    public static RoleInfo BountyHunter;
    public static RoleInfo Arsonist;
    public static RoleInfo Vulture;
    public static RoleInfo Jackal;
    public static RoleInfo Sidekick;
    public static RoleInfo Spy;

    public static void Load()
    {
        AllRoleInfos.Add(Jester = new("Jester", Neutral.Jester.RoleColor, CustomOptionHolder.jesterSpawnRate, ERoleType.Jester));
        AllRoleInfos.Add(Mayor = new("Mayor", Crewmate.Mayor.RoleColor, CustomOptionHolder.mayorSpawnRate, ERoleType.Mayor));
        AllRoleInfos.Add(Engineer = new("Engineer", Crewmate.Engineer.RoleColor, CustomOptionHolder.engineerSpawnRate, ERoleType.Engineer));
        AllRoleInfos.Add(BountyHunter = new("BountyHunter", Impostor.BountyHunter.RoleColor, CustomOptionHolder.bountyHunterSpawnRate, ERoleType.BountyHunter));
        AllRoleInfos.Add(Arsonist = new("Arsonist", Neutral.Arsonist.RoleColor, CustomOptionHolder.arsonistSpawnRate, ERoleType.Arsonist));
        AllRoleInfos.Add(Jackal = new("Jackal", Neutral.Jackal.RoleColor, CustomOptionHolder.jackalSpawnRate, ERoleType.Jackal));
        AllRoleInfos.Add(Sidekick = new("Sidekick", Neutral.Jackal.RoleColor, CustomOptionHolder.jackalSpawnRate, ERoleType.Sidekick));
        AllRoleInfos.Add(Spy = new("Spy", Crewmate.Spy.RoleColor, CustomOptionHolder.spySpawnRate, ERoleType.Spy));
    }
}