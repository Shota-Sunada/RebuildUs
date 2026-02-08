namespace RebuildUs.Roles;

public static class ModifierData
{
    public static readonly ModifierRegistration[] MODIFIERS = [new(ModifierType.Madmate, typeof(Madmate), () => Madmate.NameColor, () => CustomOptionHolder.MadmateSpawnRate), new(ModifierType.CreatedMadmate, typeof(CreatedMadmate), () => Madmate.NameColor, null), new(ModifierType.LastImpostor, typeof(LastImpostor), () => LastImpostor.NameColor, () => CustomOptionHolder.LastImpostorEnable), new(ModifierType.Mini, typeof(Mini), () => Mini.NameColor, () => CustomOptionHolder.MiniSpawnRate), new(ModifierType.AntiTeleport, typeof(AntiTeleport), () => AntiTeleport.NameColor, null)];

    public static (ModifierType ModifierType, Type Type)[] AllModifierTypes
    {
        get => [.. MODIFIERS.Select(r => (modType: r.ModType, classType: r.ClassType))];
    }

    public sealed record ModifierRegistration(ModifierType ModType, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}
