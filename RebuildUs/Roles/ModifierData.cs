namespace RebuildUs.Roles;

public static class ModifierData
{
    public record ModifierRegistration(
        ModifierType modType,
        Type classType,
        Func<Color> getColor,
        Func<CustomOption> getOption
    );

    public static readonly ModifierRegistration[] Modifiers =
    [
        new(ModifierType.Madmate, typeof(Madmate), () => Madmate.NameColor, () => CustomOptionHolder.MadmateSpawnRate),
        new(ModifierType.CreatedMadmate, typeof(CreatedMadmate), () => Madmate.NameColor, null),
        new(ModifierType.LastImpostor, typeof(LastImpostor), () => LastImpostor.NameColor, () => CustomOptionHolder.LastImpostorEnable),
        new(ModifierType.Mini, typeof(Mini), () => Mini.NameColor, () => CustomOptionHolder.MiniSpawnRate),
        new(ModifierType.AntiTeleport, typeof(AntiTeleport), () => AntiTeleport.NameColor, null),
    ];

    public static (ModifierType ModifierType, Type Type)[] AllModifierTypes => [.. Modifiers.Select(r => (r.modType, r.classType))];
}