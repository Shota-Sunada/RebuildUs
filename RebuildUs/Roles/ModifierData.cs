namespace RebuildUs.Roles;

internal static class ModifierData
{
    private static ModifierRegistration[] _modifiers;
    internal static ModifierRegistration[] Modifiers => _modifiers ??= InitializeModifiers();

    private static ModifierRegistration[] InitializeModifiers()
    {
        var modifiers = new List<ModifierRegistration>();
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var type in assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<RegisterModifierAttribute>();
            if (attr is null) continue;

            var modType = attr.ModifierType;
            var classType = attr.ClassType;

            // 起動時に一度だけ色を解決してレジストリへ登録する
            var resolvedColor = Color.white;
            try
            {
                var property = type.GetProperty(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (property != null)
                {
                    resolvedColor = (Color)property.GetValue(null);
                }
                else
                {
                    var field = type.GetField(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (field != null)
                    {
                        resolvedColor = (Color)field.GetValue(null);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("[ModifierData] Failed to get color for {0}: {1}", attr.ModifierType, e.Message);
            }

            RoleColorRegistry.RegisterModifierColor(modType, resolvedColor);
            Func<Color> getColor = () => RoleColorRegistry.GetModifierColor(modType, resolvedColor);

            // SpawnRateを取得するためのFuncを作成
            var spawnRatePropertyName = attr.SpawnRatePropertyName;
            Func<CustomOption> getOption = null;
            if (!string.IsNullOrEmpty(spawnRatePropertyName))
            {
                getOption = () =>
                {
                    var property = typeof(CustomOptionHolder).GetProperty(spawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (property is null)
                    {
                        var field = typeof(CustomOptionHolder).GetField(spawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        return (CustomOption)field.GetValue(null);
                    }
                    return (CustomOption)property.GetValue(null);
                };
            }

            modifiers.Add(new ModifierRegistration(modType, classType, getColor, getOption));
        }

        return [.. modifiers];
    }

    internal static (ModifierType ModifierType, Type Type)[] AllModifierTypes
    {
        get
        {
            var modifiers = Modifiers;
            var result = new (ModifierType ModifierType, Type Type)[modifiers.Length];
            for (var i = 0; i < modifiers.Length; i++)
            {
                var r = modifiers[i];
                result[i] = (r.ModType, r.ClassType);
            }
            return result;
        }
    }

    internal static Color GetColor(ModifierType type)
    {
        _ = Modifiers;
        return RoleColorRegistry.GetModifierColor(type, Color.white);
    }

    internal sealed record ModifierRegistration(ModifierType ModType, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}