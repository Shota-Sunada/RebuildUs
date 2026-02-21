namespace RebuildUs.Roles;

internal enum ModifierType : byte
{
    Madmate = 0,
    CreatedMadmate,
    LastImpostor,
    AntiTeleport,
    Mini,
    // Munou,

    NoModifier = byte.MaxValue,
}