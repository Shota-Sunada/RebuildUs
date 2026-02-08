namespace RebuildUs.Modules.CustomOptions;

public class CustomRoleOption : CustomOption
{
    public bool IsRoleEnabled = true;
    public CustomOption NumberOfRoleOption;

    public CustomRoleOption(int baseId, CustomOptionType type, RoleType roleType, Color color, int max = 15, bool roleEnabled = true) : base(baseId, type, Enum.TryParse<TrKey>(Enum.GetName(roleType), out var key) ? key : TrKey.None, CustomOptionHolder.RATES, 0, null, false, "", color)
    {
        IsRoleEnabled = roleEnabled;
        IsHeader = true;
        HeaderKey = NameKey;

        if (max <= 0 || !roleEnabled) IsRoleEnabled = false;

        if (max > 1) NumberOfRoleOption = Normal(baseId + 10000, type, TrKey.NumberOfRole, 1f, 1f, 15f, 1f, this);
    }

    public CustomRoleOption(int baseId, CustomOptionType type, TrKey nameKey, Color color, int max = 15, bool roleEnabled = true) : base(baseId, type, nameKey, CustomOptionHolder.RATES, 0, null, false, "", color)
    {
        IsRoleEnabled = roleEnabled;
        IsHeader = true;
        HeaderKey = nameKey;

        if (max <= 0 || !roleEnabled) IsRoleEnabled = false;

        if (max > 1) NumberOfRoleOption = Normal(baseId + 10000, type, TrKey.NumberOfRole, 1f, 1f, 15f, 1f, this);
    }

    public override bool Enabled
    {
        get => Helpers.RolesEnabled && IsRoleEnabled && Selection > 0;
    }

    public int Rate
    {
        get => Enabled ? Selection : 0;
    }

    public int Count
    {
        get => !Enabled ? 0 : NumberOfRoleOption != null ? Mathf.RoundToInt(NumberOfRoleOption.GetFloat()) : 1;
    }

    public (int rate, int count) Data
    {
        get => (Rate, Count);
    }
}
