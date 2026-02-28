namespace RebuildUs.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class RegisterModifierAttribute : Attribute
{
    public ModifierType ModifierType { get; }
    public Type ClassType { get; }
    public string NameColorPropertyName { get; }
    public string SpawnRatePropertyName { get; }

    public RegisterModifierAttribute(
        ModifierType modifierType,
        Type classType,
        string nameColorPropertyName,
        string spawnRatePropertyName)
    {
        ModifierType = modifierType;
        ClassType = classType;
        NameColorPropertyName = nameColorPropertyName;
        SpawnRatePropertyName = spawnRatePropertyName;
    }
}
