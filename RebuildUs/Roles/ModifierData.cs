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
            var attributes = type.GetCustomAttributes<RegisterModifierAttribute>();
            foreach (var attr in attributes)
            {
                var modType = attr.ModifierType;
                var classType = attr.ClassType;

                // NameColorを取得するためのFuncを作成
                Func<Color> getColor = () =>
                {
                    var property = attr.ClassType.GetProperty(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    return (Color)property.GetValue(null);
                };

                // SpawnRateを取得するためのFuncを作成
                Func<CustomOption> getOption = null;
                if (!string.IsNullOrEmpty(attr.SpawnRatePropertyName))
                {
                    getOption = () =>
                    {
                        var property = typeof(CustomOptionHolder).GetProperty(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (property == null)
                        {
                            var field = typeof(CustomOptionHolder).GetField(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            return (CustomOption)field.GetValue(null);
                        }
                        return (CustomOption)property.GetValue(null);
                    };
                }

                modifiers.Add(new ModifierRegistration(modType, classType, getColor, getOption));
            }
        }

        return [.. modifiers];
    }

    internal static (ModifierType ModifierType, Type Type)[] AllModifierTypes
    {
        get => [.. Modifiers.Select(r => (modType: r.ModType, classType: r.ClassType))];
    }

    internal sealed record ModifierRegistration(ModifierType ModType, Type ClassType, Func<Color> GetColor, Func<CustomOption>? GetOption);
}
