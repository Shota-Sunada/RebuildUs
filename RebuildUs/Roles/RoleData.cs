namespace RebuildUs.Roles;

internal static class RoleData
{
    private static RoleRegistration[] _roles;
    internal static RoleRegistration[] Roles => _roles ??= InitializeRoles();

    private static RoleRegistration[] InitializeRoles()
    {
        var roles = new List<RoleRegistration>();
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var type in assembly.GetTypes())
        {
            var attributes = type.GetCustomAttributes<RegisterRoleAttribute>();
            foreach (var attr in attributes)
            {
                var roleType = attr.RoleType;
                var roleTeam = attr.RoleTeam;
                var classType = attr.ClassType;

                // NameColorを取得するためのFuncを作成
                Func<Color> getColor = () =>
                {
                    try
                    {
                        var property = attr.ClassType.GetProperty(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (property != null)
                        {
                            return (Color)property.GetValue(null);
                        }
                        var field = attr.ClassType.GetField(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (field != null)
                        {
                            return (Color)field.GetValue(null);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to get color for {attr.RoleType}: {e.Message}", "RoleData");
                    }
                    return Color.white;
                };

                // SpawnRateを取得するためのFuncを作成
                Func<CustomOption> getOption = () =>
                {
                    try
                    {
                        var property = typeof(CustomOptionHolder).GetProperty(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (property != null)
                        {
                            var val = property.GetValue(null);
                            if (val != null) return (CustomOption)val;
                        }
                        var field = typeof(CustomOptionHolder).GetField(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (field != null)
                        {
                            var val = field.GetValue(null);
                            if (val != null) return (CustomOption)val;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to get option for {attr.RoleType} ({attr.SpawnRatePropertyName}): {e.Message}", "RoleData");
                    }
                    return null!;
                };

                roles.Add(new RoleRegistration(roleType, roleTeam, classType, getColor, getOption));
            }
        }

        Logger.LogInfo("Registering Roles");
        foreach (var role in roles)
        {
            Logger.LogInfo(role.RoleType.ToString());
        }
        Logger.LogInfo("Finish Registering Roles");

        return [.. roles];
    }

    internal sealed record RoleRegistration(RoleType RoleType, RoleTeam RoleTeam, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}
