namespace RebuildUs.Modules.CustomOptions;

public class CustomRoleSelectionOption : CustomOption
{
    public RoleType[] RoleTypes;
    public RoleType Role
    {
        get
        {
            return RoleTypes[Selection];
        }
    }

    public CustomRoleSelectionOption(int id, CustomOptionType type, string name, RoleType[] roleTypes = null, CustomOption parent = null)
    {
        roleTypes ??= [.. Enum.GetValues(typeof(RoleType)).Cast<RoleType>()];

        this.RoleTypes = roleTypes;
        var strings = roleTypes.Select(
            x =>
                x == RoleType.NoRole ? Tr.Get("Option.NoRole") :
                Tr.Get("Role." + x.ToString())
            // RoleInfo.allRoleInfos.First(y => y.roleType == x).nameColored
            ).ToArray();

        Normal(id, type, name, strings, parent);
    }
}