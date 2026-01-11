using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;
using RebuildUs.Options;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Roles;

public static class RoleData
{
    public record RoleRegistration(
        ERoleType RoleType,
        Type ClassType,
        Func<Color> GetColor,
        Func<CustomOption> GetOption
    );

    public static readonly RoleRegistration[] Roles =
    [
        new(ERoleType.Jester,       typeof(RoleBase<Jester>),       () => Jester.RoleColor,         () => CustomOptionHolder.jesterSpawnRate),
        new(ERoleType.Mayor,        typeof(RoleBase<Mayor>),        () => Mayor.RoleColor,          () => CustomOptionHolder.mayorSpawnRate),
        new(ERoleType.Engineer,     typeof(RoleBase<Engineer>),     () => Engineer.RoleColor,       () => CustomOptionHolder.engineerSpawnRate),
        new(ERoleType.BountyHunter, typeof(RoleBase<BountyHunter>), () => BountyHunter.RoleColor,   () => CustomOptionHolder.bountyHunterSpawnRate),
        new(ERoleType.Arsonist,     typeof(RoleBase<Arsonist>),     () => Arsonist.RoleColor,       () => CustomOptionHolder.arsonistSpawnRate),
        new(ERoleType.Vulture,      typeof(RoleBase<Vulture>),      () => Vulture.RoleColor,        () => CustomOptionHolder.vultureSpawnRate),
        new(ERoleType.Jackal,       typeof(RoleBase<Jackal>),       () => Jackal.RoleColor,         () => CustomOptionHolder.jackalSpawnRate),
        new(ERoleType.Sidekick,     typeof(RoleBase<Sidekick>),     () => Jackal.RoleColor,         () => CustomOptionHolder.jackalSpawnRate),
        new(ERoleType.Spy,          typeof(RoleBase<Spy>),          () => Spy.RoleColor,            () => CustomOptionHolder.spySpawnRate),
    ];

    public static (ERoleType RoleType, Type Type)[] AllRoleTypes => [.. Roles.Select(r => (r.RoleType, r.ClassType))];
}