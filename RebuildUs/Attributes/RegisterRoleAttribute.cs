namespace RebuildUs.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class RegisterRoleAttribute : Attribute
{
    public RoleType RoleType { get; }
    public RoleTeam RoleTeam { get; }
    public Type ClassType { get; }
    public string NameColorPropertyName { get; }
    public string SpawnRatePropertyName { get; }

    public RegisterRoleAttribute(
        RoleType roleType,
        RoleTeam roleTeam,
        Type classType,
        string nameColorPropertyName,
        string spawnRatePropertyName)
    {
        RoleType = roleType;
        RoleTeam = roleTeam;
        ClassType = classType;
        NameColorPropertyName = nameColorPropertyName;
        SpawnRatePropertyName = spawnRatePropertyName;
    }
}
