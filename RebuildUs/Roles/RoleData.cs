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
                    Logger.LogError("[RoleData] Failed to get color for {0}: {1}", attr.RoleType, e.Message);
                }

                RoleColorRegistry.RegisterRoleColor(roleType, resolvedColor);
                Func<Color> getColor = () => RoleColorRegistry.GetRoleColor(roleType, resolvedColor);

                // SpawnRateを取得するためのFuncを作成
                CustomOption getOption()
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
                        Logger.LogError("[RoleData] Failed to get option for {0} ({1}): {2}", attr.RoleType, attr.SpawnRatePropertyName, e.Message);
                    }
                    return null;
                }

                roles.Add(new RoleRegistration(roleType, roleTeam, classType, getColor, getOption));
            }
        }

        Logger.LogInfo("[InitRole] Registering Roles");
        foreach (var role in roles)
        {
            Logger.LogInfo("[InitRole] {0}", Enum.GetName(role.RoleType));
        }
        Logger.LogInfo("[InitRole] Finish Registering Roles");

        return [.. roles];
    }

    internal sealed record RoleRegistration(RoleType RoleType, RoleTeam RoleTeam, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}