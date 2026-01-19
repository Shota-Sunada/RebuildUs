using System.Reflection;

namespace RebuildUs.Roles;

public static class ModifierHelpers
{
    public static bool HasModifier(this PlayerControl player, ModifierType modType)
    {
        foreach (var type in ModifierData.AllModifierTypes)
        {
            if (modType == type.ModifierType)
            {
                return type.Type != null
                    && (bool)(type.Type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? false)
                    && (bool)(type.Type.GetMethod("HasModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]) ?? false);
            }
        }

        Logger.LogWarn($"There is no modifier type: {modType}", "HasModifier");

        return false;
    }

    public static bool AddModifier(this PlayerControl player, ModifierType modType)
    {
        Logger.LogInfo($"{player?.Data?.PlayerName}({player?.PlayerId}): {Enum.GetName(typeof(ModifierType), modType)}");
        foreach (var type in ModifierData.AllModifierTypes)
        {
            if (modType == type.ModifierType)
            {
                if (type.Type == null) return false;
                var method = type.Type.GetMethod("AddModifier", BindingFlags.Public | BindingFlags.Static)
                            ?? type.Type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, [player]);
                    return true;
                }
            }
        }

        Logger.LogWarn($"There is no modifier type: {modType}", "AddModifier");

        return false;
    }

    public static void EraseModifier(this PlayerControl player, ModifierType modType)
    {
        if (HasModifier(player, modType))
        {
            foreach (var type in ModifierData.AllModifierTypes)
            {
                if (modType == type.ModifierType)
                {
                    if (type.Type == null) return;
                    type.Type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
                    return;
                }
            }

            Logger.LogWarn($"There is no modifier type: {modType}", "EraseModifier");
        }
    }

    public static void EraseAllModifiers(this PlayerControl player)
    {
        foreach (var type in ModifierData.AllModifierTypes)
        {
            if (type.Type == null) continue;
            type.Type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
        }

        // if (player.IsRole(RoleType.Mayor)) Mayor.clearAndReload();
    }

    public static void SwapModifiers(this PlayerControl player, PlayerControl target)
    {
        foreach (var type in ModifierData.AllModifierTypes)
        {
            if (type.Type != null && player.HasModifier(type.ModifierType))
            {
                type.Type.GetMethod("SwapModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player, target]);
            }
        }
    }
}