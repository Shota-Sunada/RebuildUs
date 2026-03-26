namespace RebuildUs.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class RegisterModifierAttribute(ModifierType modifierType, Type classType, string spawnRatePropertyName, string modifierColorPropertyName = "Color") : Attribute
{
    public ModifierType ModifierType { get; } = modifierType;
    public Type ClassType { get; } = classType;
    public string NameColorPropertyName { get; } = modifierColorPropertyName;
    public string SpawnRatePropertyName { get; } = spawnRatePropertyName;
}