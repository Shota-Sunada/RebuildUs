using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;
using RebuildUs.Options;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Roles;

public static class RoleData
{
    public record RoleRegistration(
        RoleType roleType,
        Type classType,
        Func<Color> getColor,
        Func<CustomOption> getOption
    );

    public static readonly RoleRegistration[] Roles =
    [
        new(RoleType.Jester,       typeof(RoleBase<Jester>),       () => Jester.RoleColor,         () => CustomOptionHolder.JesterSpawnRate),
        new(RoleType.Mayor,        typeof(RoleBase<Mayor>),        () => Mayor.RoleColor,          () => CustomOptionHolder.MayorSpawnRate),
        new(RoleType.Engineer,     typeof(RoleBase<Engineer>),     () => Engineer.RoleColor,       () => CustomOptionHolder.EngineerSpawnRate),
        new(RoleType.BountyHunter, typeof(RoleBase<BountyHunter>), () => BountyHunter.RoleColor,   () => CustomOptionHolder.BountyHunterSpawnRate),
        new(RoleType.Arsonist,     typeof(RoleBase<Arsonist>),     () => Arsonist.RoleColor,       () => CustomOptionHolder.ArsonistSpawnRate),
        new(RoleType.Vulture,      typeof(RoleBase<Vulture>),      () => Vulture.RoleColor,        () => CustomOptionHolder.VultureSpawnRate),
        new(RoleType.Jackal,       typeof(RoleBase<Jackal>),       () => Jackal.RoleColor,         () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Sidekick,     typeof(RoleBase<Sidekick>),     () => Jackal.RoleColor,         () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Spy,          typeof(RoleBase<Spy>),          () => Spy.RoleColor,            () => CustomOptionHolder.SpySpawnRate),
    ];

    public static (RoleType RoleType, Type Type)[] AllRoleTypes => [.. Roles.Select(r => (r.roleType, r.classType))];
}