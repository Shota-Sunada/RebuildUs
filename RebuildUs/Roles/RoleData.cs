using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;
using RebuildUs.Options;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Roles;

public static class RoleData
{
    public record RoleRegistration(
        ERoleType roleType,
        Type classType,
        Func<Color> getColor,
        Func<CustomOption> getOption
    );

    public static readonly RoleRegistration[] Roles =
    [
        new(ERoleType.Jester,       typeof(RoleBase<Jester>),       () => Jester.RoleColor,         () => CustomOptionHolder.JesterSpawnRate),
        new(ERoleType.Mayor,        typeof(RoleBase<Mayor>),        () => Mayor.RoleColor,          () => CustomOptionHolder.MayorSpawnRate),
        new(ERoleType.Engineer,     typeof(RoleBase<Engineer>),     () => Engineer.RoleColor,       () => CustomOptionHolder.EngineerSpawnRate),
        new(ERoleType.BountyHunter, typeof(RoleBase<BountyHunter>), () => BountyHunter.RoleColor,   () => CustomOptionHolder.BountyHunterSpawnRate),
        new(ERoleType.Arsonist,     typeof(RoleBase<Arsonist>),     () => Arsonist.RoleColor,       () => CustomOptionHolder.ArsonistSpawnRate),
        new(ERoleType.Vulture,      typeof(RoleBase<Vulture>),      () => Vulture.RoleColor,        () => CustomOptionHolder.VultureSpawnRate),
        new(ERoleType.Jackal,       typeof(RoleBase<Jackal>),       () => Jackal.RoleColor,         () => CustomOptionHolder.JackalSpawnRate),
        new(ERoleType.Sidekick,     typeof(RoleBase<Sidekick>),     () => Jackal.RoleColor,         () => CustomOptionHolder.JackalSpawnRate),
        new(ERoleType.Spy,          typeof(RoleBase<Spy>),          () => Spy.RoleColor,            () => CustomOptionHolder.SpySpawnRate),
    ];

    public static (ERoleType RoleType, Type Type)[] AllRoleTypes => [.. Roles.Select(r => (r.roleType, r.classType))];
}