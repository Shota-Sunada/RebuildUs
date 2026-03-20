namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomModifierOption : CustomRoleOption
{
    internal CustomModifierOption(int baseId, CustomOptionType type, ModifierType modType, Color color, int max = 15, bool roleEnabled = true) : base(
        baseId,
        type,
        Enum.TryParse(Enum.GetName(modType), out TrKey key) ? key : TrKey.None,
        color,
        max,
        roleEnabled)
    {
        IsRoleEnabled = roleEnabled;

        if (max <= 0 || !roleEnabled)
        {
            IsRoleEnabled = false;
        }
    }
}