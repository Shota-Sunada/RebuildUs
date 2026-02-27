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
                out (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier) cached))
        {
            return cached;
        }

        ModifierData.ModifierRegistration[] modifiers = ModifierData.Modifiers;
        foreach ((ModifierType modifierType, Type type, _, _) in modifiers)
        {
            if (modifierType != modType)
            {
                continue;
            }
            if (type == null)
            {
                break;
            }

            (MethodInfo GetMethod, MethodInfo, MethodInfo, MethodInfo, MethodInfo) methods = (
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

            List<PlayerModifier> mods = PlayerModifier.GetModifiers(player);
            foreach (PlayerModifier t in mods)
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
            (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)
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
            (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)
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

            ModifierData.ModifierRegistration[] modifiers = ModifierData.Modifiers;
            foreach (ModifierData.ModifierRegistration reg in modifiers)
            {
                if (reg.ClassType == null)
                {
                    continue;
                }
                (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)
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

            ModifierData.ModifierRegistration[] modifiers = ModifierData.Modifiers;
            foreach (ModifierData.ModifierRegistration reg in modifiers)
            {
                if (reg.ClassType == null || !player.HasModifier(reg.ModType) && !target.HasModifier(reg.ModType))
                {
                    continue;
                }
                (MethodInfo exists, MethodInfo hasModifier, MethodInfo addModifier, MethodInfo eraseModifier, MethodInfo swapModifier)
                    methods = GetMethods(reg.ModType);
                methods.swapModifier?.Invoke(null, [player, target]);
            }
        }
    }
}