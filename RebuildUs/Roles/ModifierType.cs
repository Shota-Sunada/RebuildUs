namespace RebuildUs.Roles;

internal enum ModifierType : byte
{
    Madmate = 0,
    CreatedMadmate,
    AntiTeleport,
    Mini,

    NoModifier = byte.MaxValue,
}