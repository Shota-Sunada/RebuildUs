namespace RebuildUs.Modules.CustomOptions;

public class CustomModifierOption : CustomRoleOption
{
    public CustomModifierOption(int baseId, CustomOptionType type, ModifierType modType, Color color, int max = 15, bool roleEnabled = true) :
    base(baseId, type, Enum.TryParse<TrKey>(Enum.GetName(modType), out var key) ? key : TrKey.None, color, max, roleEnabled)
    {
        IsRoleEnabled = roleEnabled;
        IsHeader = true;
        HeaderKey = NameKey;

        if (max <= 0 || !roleEnabled)
        {
            IsRoleEnabled = false;
        }

        if (max > 1)
        {
            NumberOfRoleOption = Normal(baseId + 10000, type, TrKey.NumberOfRole, 1f, 1f, 15f, 1f, this);
        }
    }
}