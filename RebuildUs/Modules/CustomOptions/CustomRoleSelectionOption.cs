namespace RebuildUs.Modules.CustomOptions;

public class CustomRoleSelectionOption : CustomOption
{
    public RoleType[] roleTypes;
    public RoleType Role
    {
        get
        {
            return roleTypes[Selection];
        }
    }

    public CustomRoleSelectionOption(int id, CustomOptionType type, string name, RoleType[] roleTypes = null, CustomOption parent = null)
    {
        roleTypes ??= [.. Enum.GetValues(typeof(RoleType)).Cast<RoleType>()];

        this.roleTypes = roleTypes;
        var strings = roleTypes.Select(
            x =>
                x == RoleType.NoRole ? "optionOff" :
                Tr.Get(x.ToString()[..1].ToLower() + x.ToString()[1..])
            // RoleInfo.allRoleInfos.First(y => y.roleType == x).nameColored
            ).ToArray();

        Normal(id, type, name, strings, parent);
    }
}