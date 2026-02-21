namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomRoleSelectionOption : CustomOption
{
    private readonly RoleType[] _roleTypes;

    internal CustomRoleSelectionOption(int id, CustomOptionType type, TrKey nameKey, RoleType[] roleTypes = null, CustomOption parent = null)
    {
        if (roleTypes == null)
        {
            Array values = Enum.GetValues(typeof(RoleType));
            roleTypes = new RoleType[values.Length];
            for (int i = 0; i < values.Length; i++) roleTypes[i] = (RoleType)values.GetValue(i)!;
        }

        _roleTypes = roleTypes;
        string[] strings = new string[roleTypes.Length];
        for (int i = 0; i < roleTypes.Length; i++)
        {
            RoleType x = roleTypes[i];
            strings[i] = x == RoleType.NoRole ? Tr.Get(TrKey.NoRole) : Tr.GetDynamic(x.ToString());
        }

        CustomOption opt = Normal(id, type, nameKey, strings, parent);
        Id = opt.Id;
        NameKey = opt.NameKey;
        Selections = opt.Selections;
        DefaultSelection = opt.DefaultSelection;
        Entry = opt.Entry;
        Selection = opt.Selection;
        Parent = opt.Parent;
        Type = opt.Type;
    }

    internal RoleType Role
    {
        get => _roleTypes[Selection];
    }
}