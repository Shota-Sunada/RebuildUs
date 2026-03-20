namespace RebuildUs.Modules.CustomOptions;

internal class CustomRoleOption : CustomGeneralOption<string>
{
    internal bool IsRoleEnabled;
    internal CustomNumberOption NumberOfRoleOption;

    internal CustomRoleOption(int baseId, CustomOptionType type, RoleType roleType, Color color, int max = 15, bool roleEnabled = true) : base(baseId,
        type,
        Enum.TryParse(Enum.GetName(roleType), out TrKey key) ? key : TrKey.None,
        Helpers.CastArray<string>(CustomOptionHolder.Rates),
        CustomOptionHolder.Rates.Length > 0 ? (CustomOptionHolder.Rates[0] as string) ?? string.Empty : string.Empty,
        null,
        false,
        "",
        color,
        new CustomOptionHeader(baseId, type, Enum.TryParse(Enum.GetName(roleType), out TrKey key2) ? key2 : TrKey.None, color))
    {
        IsRoleEnabled = roleEnabled;
        UseSpawnChanceLabel = true;

        if (max <= 0 || !roleEnabled)
        {
            IsRoleEnabled = false;
        }

        if (max > 1)
        {
            NumberOfRoleOption = Normal(baseId + 10000, type, TrKey.NumberOfRole, 1f, 1f, 15f, 1f, this);
        }
    }

    internal CustomRoleOption(int baseId, CustomOptionType type, TrKey nameKey, Color color, int max = 15, bool roleEnabled = true) : base(baseId,
        type,
        nameKey,
        Helpers.CastArray<string>(CustomOptionHolder.Rates),
        CustomOptionHolder.Rates.Length > 0 ? (CustomOptionHolder.Rates[0] as string) ?? string.Empty : string.Empty,
        null,
        false,
        "",
        color,
        new CustomOptionHeader(baseId, type, nameKey, color))
    {
        IsRoleEnabled = roleEnabled;
        UseSpawnChanceLabel = true;

        if (max <= 0 || !roleEnabled)
        {
            IsRoleEnabled = false;
        }

        if (max > 1)
        {
            NumberOfRoleOption = Normal(baseId + 10000, type, TrKey.NumberOfRole, 1f, 1f, 15f, 1f, this);
        }
    }

    internal override bool Enabled
    {
        get => Helpers.RolesEnabled && IsRoleEnabled && GetSelectionIndex() > 0;
    }

    internal int Rate
    {
        get => Enabled ? GetSelectionIndex() : 0;
    }

    internal int Count
    {
        get => !Enabled ? 0 : NumberOfRoleOption != null ? Mathf.RoundToInt(NumberOfRoleOption.GetFloat()) : 1;
    }

    internal (int rate, int count) Data
    {
        get => (Rate, Count);
    }
}