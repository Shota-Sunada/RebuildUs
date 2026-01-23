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
        if (roleTypes == null)
        {
            var values = Enum.GetValues(typeof(RoleType));
            roleTypes = new RoleType[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                roleTypes[i] = (RoleType)values.GetValue(i);
            }
        }

        this.RoleTypes = roleTypes;
        var strings = new string[roleTypes.Length];
        for (int i = 0; i < roleTypes.Length; i++)
        {
            var x = roleTypes[i];
            strings[i] = x == RoleType.NoRole ? Tr.Get("Option.NoRole") : Tr.Get("Role." + x.ToString());
        }

        Normal(id, type, name, strings, parent);
    }
}