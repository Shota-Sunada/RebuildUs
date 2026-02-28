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
                    var property = attr.ClassType.GetProperty(attr.NameColorPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    return (Color)property.GetValue(null);
                };

                // SpawnRateを取得するためのFuncを作成
                Func<CustomOption> getOption = () =>
                {
                    var property = typeof(CustomOptionHolder).GetProperty(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (property == null)
                    {
                        // TODO: プロパティが見つからない場合のフォールバック（現在はフィールドも探す）
                        var field = typeof(CustomOptionHolder).GetField(attr.SpawnRatePropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        return (CustomOption)field.GetValue(null);
                    }
                    return (CustomOption)property.GetValue(null);
                };

                roles.Add(new RoleRegistration(roleType, roleTeam, classType, getColor, getOption));
            }
        }

        return [.. roles];
    }

    internal sealed record RoleRegistration(RoleType RoleType, RoleTeam RoleTeam, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}
