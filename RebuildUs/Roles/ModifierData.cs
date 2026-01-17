using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;
using RebuildUs.Options;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Roles;

public static class ModifierData
{
    public record ModifierRegistration(
        ModifierType modType,
        Type classType,
        Func<Color> getColor,
        Func<CustomOption> getOption
    );

    public static readonly ModifierRegistration[] Roles =
    [
        // new(RoleType.Jester, typeof(RoleBase<Jester>), () => Jester.RoleColor, () => CustomOptionHolder.JesterSpawnRate),
    ];

    public static (ModifierType ModifierType, Type Type)[] AllModifierTypes => [.. Roles.Select(r => (r.modType, r.classType))];
}