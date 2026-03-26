namespace RebuildUs.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class RegisterRoleAttribute(RoleType roleType, RoleTeam roleTeam, Type classType, string spawnRatePropertyName, string nameColorPropertyName = "Color") : Attribute
{
    public RoleType RoleType { get; } = roleType;
    public RoleTeam RoleTeam { get; } = roleTeam;
    public Type ClassType { get; } = classType;
    public string NameColorPropertyName { get; } = nameColorPropertyName;
    public string SpawnRatePropertyName { get; } = spawnRatePropertyName;
}