using RebuildUs.Options;
using RebuildUs.Roles;

namespace RebuildUs.Modules.CustomOptions;

public class CustomRoleOption : CustomOption
{
    public CustomOption NumberOfRoleOption = null;
    public bool IsRoleEnabled = true;

    public override bool Enabled { get { return Helpers.RolesEnabled && IsRoleEnabled && Selection > 0; } }
    public int Rate { get { return Enabled ? Selection : 0; } }
    public int Count { get { return !Enabled ? 0 : NumberOfRoleOption != null ? Mathf.RoundToInt(NumberOfRoleOption.GetFloat()) : 1; } }
    public (int rate, int count) Data { get { return (Rate, Count); } }

    public CustomRoleOption(int baseId, CustomOptionType type, RoleInfo roleInfo, int max = 15, bool roleEnabled = true) :
    base(baseId, type, Helpers.Cs(roleInfo.Color, roleInfo.NameKey), CustomOptionHolder.RATES, "", null, true, false, "")
    {
        IsRoleEnabled = roleEnabled;

        if (max <= 0 || !roleEnabled)
        {
            IsRoleEnabled = false;
        }

        if (max > 1)
        {
            NumberOfRoleOption = Normal(baseId + 10000, type, "CustomOption.NumberOfRole", 1f, 1f, 15f, 1f, this);
        }
    }
}