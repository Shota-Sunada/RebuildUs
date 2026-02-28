namespace RebuildUs.Roles;

internal static class ModifierHelpers
{
    private static readonly
        Dictionary<ModifierType, (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier
            )> MethodCache = [];

    private static (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier) GetMethods(
        ModifierType modType)
    {
        if (MethodCache.TryGetValue(modType,
                out var cached))
        {
            return cached;
        }

        var modifiers = ModifierData.Modifiers;
        foreach ((var modifierType, var type, _, _) in modifiers)
        {
            if (modifierType != modType)
            {
                continue;
            }
            if (type == null)
            {
                break;
            }

            var methods = (
                type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod,
                type.GetMethod("HasModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                type.GetMethod("AddModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                ?? type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                type.GetMethod("EraseModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                type.GetMethod("SwapModifier", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            MethodCache[modType] = methods;
            return methods;
        }

        if (modType != ModifierType.NoModifier)
        {
            Logger.LogWarn($"There is no modifier type registration for: {modType}", "GetMethods");
        }
        (MethodInfo, MethodInfo, MethodInfo, MethodInfo, MethodInfo) nullMethods = (null, null, null, null, null);
        MethodCache[modType] = nullMethods;
        return nullMethods;
    }

    extension(PlayerControl player)
    {
        internal bool HasModifier(ModifierType modType)
        {
            if (player == null || modType == ModifierType.NoModifier)
            {
                return false;
            }

            var mods = PlayerModifier.GetModifiers(player);
            foreach (var t in mods)
            {
                if (t.CurrentModifierType == modType)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool AddModifier(ModifierType modType)
        {
            if (player == null || modType == ModifierType.NoModifier)
            {
                return false;
            }

            Logger.LogInfo($"{player.Data?.PlayerName}({player.PlayerId}): {Enum.GetName(typeof(ModifierType), modType)}");
            var
                methods = GetMethods(modType);
            if (methods.addModifier != null)
            {
                methods.addModifier.Invoke(null, [player]);
                return true;
            }

            Logger.LogWarn($"There is no modifier type: {modType}", "AddModifier");
            return false;
        }

        internal void EraseModifier(ModifierType modType)
        {
            if (player == null || modType == ModifierType.NoModifier)
            {
                return;
            }

            if (!player.HasModifier(modType))
            {
                return;
            }
            var
                methods = GetMethods(modType);
            if (methods.eraseModifier != null)
            {
                methods.eraseModifier.Invoke(null, [player]);
                return;
            }

            Logger.LogWarn($"There is no modifier type: {modType}", "EraseModifier");
        }

        internal void EraseAllModifiers()
        {
            if (player == null)
            {
                return;
            }

            var modifiers = ModifierData.Modifiers;
            foreach (var reg in modifiers)
            {
                if (reg.ClassType == null)
                {
                    continue;
                }
                var
                    methods = GetMethods(reg.ModType);
                methods.eraseModifier?.Invoke(null, [player]);
            }
        }

        internal void SwapModifiers(PlayerControl target)
        {
            if (player == null || target == null)
            {
                return;
            }

            var modifiers = ModifierData.Modifiers;
            foreach (var reg in modifiers)
            {
                if (reg.ClassType == null || !player.HasModifier(reg.ModType) && !target.HasModifier(reg.ModType))
                {
                    continue;
                }
                var
                    methods = GetMethods(reg.ModType);
                methods.swapModifier?.Invoke(null, [player, target]);
            }
        }
    }
}