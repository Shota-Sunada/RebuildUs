namespace RebuildUs.Roles;

public partial class RoleInfo
{
    private static readonly Dictionary<RoleType, RoleInfo> RoleDict = [];

    public static RoleInfo Get(RoleType type) => RoleDict.GetValueOrDefault(type);

    public static RoleInfo Jackal => Get(RoleType.Jackal);
    public static RoleInfo Crewmate => Get(RoleType.Crewmate);
    public static RoleInfo Impostor => Get(RoleType.Impostor);

    public static void Load()
    {
        RoleDict.Clear();
        AllRoleInfos.Clear();

        foreach (var reg in RoleData.Roles)
        {
            var info = new RoleInfo(Enum.GetName(reg.roleType), reg.getColor(), reg.getOption(), reg.roleType);
            RoleDict[reg.roleType] = info;
            AllRoleInfos.Add(info);
        }

        RoleDict[RoleType.Crewmate] = new(Enum.GetName(RoleType.Crewmate), Palette.CrewmateBlue, null, RoleType.Crewmate);
        RoleDict[RoleType.Impostor] = new(Enum.GetName(RoleType.Impostor), Palette.ImpostorRed, null, RoleType.Impostor);
    }
}