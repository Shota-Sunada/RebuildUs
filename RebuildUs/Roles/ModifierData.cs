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

            // RoleColorを取得するためのFuncを作成
            Func<Color> getColor = () =>
            {
                var property = attr.ClassType.GetProperty(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                return (Color)property.GetValue(null);
            };

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
        var modifiers = Modifiers;
        for (var i = 0; i < modifiers.Length; i++)
        {
            var reg = modifiers[i];
            if (reg.ModType == type)
            {
                return reg.GetColor?.Invoke() ?? Color.white;
            }
        }
        return Color.white;
    }

    internal sealed record ModifierRegistration(ModifierType ModType, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}