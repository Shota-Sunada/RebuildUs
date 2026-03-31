namespace RebuildUs.Features.CustomOptions;

internal sealed class CustomRoleSelectionOption : CustomGeneralOption<string>
{
    private readonly RoleType[] _roleTypes;

    internal CustomRoleSelectionOption(COID id, COType type, TrKey nameKey, RoleType[] roleTypes = null, CustomOption parent = null)
    : base(id, type, nameKey, [], string.Empty, parent, false, TrKey.None, Color.white)
    {
        if (roleTypes == null)
        {
            var values = Enum.GetValues(typeof(RoleType));
            roleTypes = new RoleType[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                roleTypes[i] = (RoleType)values.GetValue(i)!;
            }
        }

        _roleTypes = roleTypes;
        var strings = new string[roleTypes.Length];
        for (var i = 0; i < roleTypes.Length; i++)
        {
            var x = roleTypes[i];
            strings[i] = x == RoleType.NoRole ? Tr.Get(TrKey.NoRole) : Tr.GetDynamic(x.ToString());
        }

        // reinitialize selections for this option
        Selections = strings;
        var index = Array.IndexOf(strings, strings.Length > 0 ? strings[0] : string.Empty);
        DefaultSelection = index >= 0 ? index : 0;
        if (Id != 0)
        {
            var presetData = CustomOptionPresetManager.LoadPreset(Preset);
            var value = DefaultSelection;
            if (presetData.TryGetValue(Id.ToString(), out var savedIndex))
            {
                value = savedIndex;
            }
            Selection = Mathf.Clamp(value, 0, strings.Length - 1);
        }
        else
        {
            var savedIndex = CustomOptionPresetManager.LoadCurrentPresetIndex(DefaultSelection);
            Selection = Mathf.Clamp(savedIndex, 0, strings.Length - 1);
            Preset = Selection;
        }
    }

    internal RoleType Role
    {
        get => _roleTypes[GetSelectionIndex()];
    }
}