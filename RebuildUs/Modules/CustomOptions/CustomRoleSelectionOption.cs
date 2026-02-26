namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomRoleSelectionOption : CustomOption<string>
{
    private readonly RoleType[] _roleTypes;

    internal CustomRoleSelectionOption(int id, CustomOptionType type, TrKey nameKey, RoleType[] roleTypes = null, CustomOption parent = null)
        : base(id, type, nameKey, [], string.Empty, parent, false, "", Color.white)
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

        // reinitialize selections for this option
        Selections = strings;
        int index = Array.IndexOf(strings, strings.Length > 0 ? strings[0] : string.Empty);
        DefaultSelection = index >= 0 ? index : 0;
        if (Id != 0)
        {
            Entry = global::RebuildUs.RebuildUs.Instance.Config.Bind($"Preset{Preset}", Id.ToString(), DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, strings.Length - 1);
        }
        else
        {
            Entry = global::RebuildUs.RebuildUs.Instance.Config.Bind("General", "Selected Preset", DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, strings.Length - 1);
            Preset = Selection;
        }
    }

    internal RoleType Role
    {
        get => _roleTypes[GetSelectionIndex()];
    }
}