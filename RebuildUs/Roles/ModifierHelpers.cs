using System.Reflection;

namespace RebuildUs.Roles;

public static class ModifierHelpers
{
    private static readonly Dictionary<ModifierType, (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)> METHOD_CACHE = [];

    private static (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier) GetMethods(ModifierType modType)
    {
        if (METHOD_CACHE.TryGetValue(modType, out var cached)) return cached;

        var modifiers = ModifierData.MODIFIERS;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.ModType == modType)
            {
                var type = reg.ClassType;
                if (type == null) break;

                var methods = (type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod, type.GetMethod("HasModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("AddModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ?? type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("SwapModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                METHOD_CACHE[modType] = methods;
                return methods;
            }
        }

        if (modType != ModifierType.NoModifier)
            Logger.LogWarn($"There is no modifier type registration for: {modType}", "GetMethods");

        var nullMethods = ((MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null);
        METHOD_CACHE[modType] = nullMethods;
        return nullMethods;
    }

    public static bool HasModifier(this PlayerControl player, ModifierType modType)
    {
        if (player == null || modType == ModifierType.NoModifier) return false;

        var mods = PlayerModifier.GetModifiers(player);
        for (var i = 0; i < mods.Count; i++)
        {
            if (mods[i].CurrentModifierType == modType)
                return true;
        }

        return false;
    }

    public static bool AddModifier(this PlayerControl player, ModifierType modType)
    {
        if (player == null || modType == ModifierType.NoModifier) return false;

        Logger.LogInfo($"{player.Data?.PlayerName}({player.PlayerId}): {Enum.GetName(typeof(ModifierType), modType)}");
        var methods = GetMethods(modType);
        if (methods.addModifier != null)
        {
            methods.addModifier.Invoke(null, [player]);
            return true;
        }

        Logger.LogWarn($"There is no modifier type: {modType}", "AddModifier");
        return false;
    }

    public static void EraseModifier(this PlayerControl player, ModifierType modType)
    {
        if (player == null || modType == ModifierType.NoModifier) return;

        if (player.HasModifier(modType))
        {
            var methods = GetMethods(modType);
            if (methods.eraseModifier != null)
            {
                methods.eraseModifier.Invoke(null, [player]);
                return;
            }

            Logger.LogWarn($"There is no modifier type: {modType}", "EraseModifier");
        }
    }

    public static void EraseAllModifiers(this PlayerControl player)
    {
        if (player == null) return;

        var modifiers = ModifierData.MODIFIERS;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.ClassType == null) continue;
            var methods = GetMethods(reg.ModType);
            methods.eraseModifier?.Invoke(null, [player]);
        }
    }

    public static void SwapModifiers(this PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;

        var modifiers = ModifierData.MODIFIERS;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.ClassType != null && (player.HasModifier(reg.ModType) || target.HasModifier(reg.ModType)))
            {
                var methods = GetMethods(reg.ModType);
                methods.swapModifier?.Invoke(null, [player, target]);
            }
        }
    }
}
