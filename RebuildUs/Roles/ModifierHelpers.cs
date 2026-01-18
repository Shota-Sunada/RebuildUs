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
                return (bool)type.Type.GetMethod("HasModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
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
                return (bool)type.Type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
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
            type.Type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
        }

        // if (player.IsRole(RoleType.Mayor)) Mayor.clearAndReload();
    }

    public static void SwapModifiers(this PlayerControl player, PlayerControl target)
    {
        foreach (var type in ModifierData.AllModifierTypes)
        {
            if (player.HasModifier(type.ModifierType))
            {
                type.Type.GetMethod("SwapModifier", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player, target]);
            }
        }
    }
}
