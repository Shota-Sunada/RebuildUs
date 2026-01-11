namespace RebuildUs.Roles;

public partial class RoleInfo
{
    private static readonly Dictionary<ERoleType, RoleInfo> RoleDict = [];

    public static RoleInfo Get(ERoleType type) => RoleDict.GetValueOrDefault(type);

    public static RoleInfo Jackal => Get(ERoleType.Jackal);
    public static RoleInfo Crewmate => Get(ERoleType.Crewmate);
    public static RoleInfo Impostor => Get(ERoleType.Impostor);

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

        RoleDict[ERoleType.Crewmate] = new(Enum.GetName(ERoleType.Crewmate), Palette.CrewmateBlue, null, ERoleType.Crewmate);
        RoleDict[ERoleType.Impostor] = new(Enum.GetName(ERoleType.Impostor), Palette.ImpostorRed, null, ERoleType.Impostor);
    }
}