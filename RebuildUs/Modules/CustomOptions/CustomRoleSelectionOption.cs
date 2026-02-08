namespace RebuildUs.Modules.CustomOptions;

public sealed class CustomRoleSelectionOption : CustomOption
{
    public RoleType[] RoleTypes;

    public CustomRoleSelectionOption(int id, CustomOptionType type, TrKey nameKey, RoleType[] roleTypes = null, CustomOption parent = null)
    {
        if (roleTypes == null)
        {
            var values = Enum.GetValues(typeof(RoleType));
            roleTypes = new RoleType[values.Length];
            for (var i = 0; i < values.Length; i++) roleTypes[i] = (RoleType)values.GetValue(i);
        }

        RoleTypes = roleTypes;
        var strings = new string[roleTypes.Length];
        for (var i = 0; i < roleTypes.Length; i++)
        {
            var x = roleTypes[i];
            strings[i] = x == RoleType.NoRole ? Tr.Get(TrKey.NoRole) : Tr.GetDynamic("" + x);
        }

        var opt = Normal(id, type, nameKey, strings, parent);
        Id = opt.Id;
        NameKey = opt.NameKey;
        Selections = opt.Selections;
        DefaultSelection = opt.DefaultSelection;
        Entry = opt.Entry;
        Selection = opt.Selection;
        Parent = opt.Parent;
        Type = opt.Type;
    }

    public RoleType Role
    {
        get => RoleTypes[Selection];
    }
}
