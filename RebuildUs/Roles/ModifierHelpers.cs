using System.Reflection;

namespace RebuildUs.Roles;

public static class ModifierHelpers
{
    private static readonly Dictionary<ModifierType, (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)> MethodCache = [];

    private static (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier) GetMethods(ModifierType modType)
    {
        if (MethodCache.TryGetValue(modType, out var cached)) return cached;

        var modifiers = ModifierData.Modifiers;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.modType == modType)
            {
                var type = reg.classType;
                if (type == null) break;

                var methods = (
                    type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod,
                    type.GetMethod("HasModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("AddModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ?? type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("SwapModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                );
                MethodCache[modType] = methods;
                return methods;
            }
        }

        if (modType != ModifierType.NoModifier)
            Logger.LogWarn($"There is no modifier type registration for: {modType}", "GetMethods");

        var nullMethods = ((MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null);
        MethodCache[modType] = nullMethods;
        return nullMethods;
    }

    public static bool HasModifier(this PlayerControl player, ModifierType modType)
    {
        if (player == null || modType == ModifierType.NoModifier) return false;

        var mods = PlayerModifier.GetModifiers(player);
        for (int i = 0; i < mods.Count; i++)
        {
            if (mods[i].CurrentModifierType == modType) return true;
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

        if (HasModifier(player, modType))
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

        var modifiers = ModifierData.Modifiers;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.classType == null) continue;
            var methods = GetMethods(reg.modType);
            methods.eraseModifier?.Invoke(null, [player]);
        }
    }

    public static void SwapModifiers(this PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;

        var modifiers = ModifierData.Modifiers;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.classType != null && player.HasModifier(reg.modType))
            {
                var methods = GetMethods(reg.modType);
                methods.swapModifier?.Invoke(null, [player, target]);
            }
        }
    }
}