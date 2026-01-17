using RebuildUs.Options;
using RebuildUs.Roles;

namespace RebuildUs.Modules.CustomOptions;

public class CustomModifierOption(int baseId, CustomOptionType type, ModifierType modType, Color color, int max = 15, bool roleEnabled = true) : CustomRoleOption(baseId, type, Enum.GetName(modType), color, max, roleEnabled)
{
}