namespace RebuildUs.Roles;

public partial class RoleInfo
{
    private static readonly Dictionary<ERoleType, RoleInfo> RoleDict = [];

    public static RoleInfo Get(ERoleType type) => RoleDict.GetValueOrDefault(type);

    public static RoleInfo Jackal => Get(ERoleType.Jackal);

    public static void Load()
    {
        RoleDict.Clear();
        AllRoleInfos.Clear();

        foreach (var reg in RoleData.Roles)
        {
            var info = new RoleInfo(Enum.GetName(reg.RoleType), reg.GetColor(), reg.GetOption(), reg.RoleType);
            RoleDict[reg.RoleType] = info;
            AllRoleInfos.Add(info);
        }
    }
}