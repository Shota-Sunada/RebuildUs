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
        new(RoleType.Jester, typeof(RoleBase<Jester>), () => Jester.NameColor, () => CustomOptionHolder.JesterSpawnRate),
        new(RoleType.Mayor, typeof(RoleBase<Mayor>), () => Mayor.NameColor, () => CustomOptionHolder.MayorSpawnRate),
        new(RoleType.Engineer, typeof(RoleBase<Engineer>), () => Engineer.NameColor, () => CustomOptionHolder.EngineerSpawnRate),
        new(RoleType.BountyHunter, typeof(RoleBase<BountyHunter>), () => BountyHunter.NameColor, () => CustomOptionHolder.BountyHunterSpawnRate),
        new(RoleType.Arsonist, typeof(RoleBase<Arsonist>), () => Arsonist.NameColor, () => CustomOptionHolder.ArsonistSpawnRate),
        new(RoleType.Vulture, typeof(RoleBase<Vulture>), () => Vulture.NameColor, () => CustomOptionHolder.VultureSpawnRate),
        new(RoleType.Jackal, typeof(RoleBase<Jackal>), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Sidekick, typeof(RoleBase<Sidekick>), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Spy, typeof(RoleBase<Spy>), () => Spy.NameColor, () => CustomOptionHolder.SpySpawnRate),
        new(RoleType.Godfather, typeof(RoleBase<Mafia.Godfather>), () => Mafia.NameColor, () => CustomOptionHolder.mafiaSpawnRate),
        new(RoleType.Mafioso, typeof(RoleBase<Mafia.Mafioso>), () => Mafia.NameColor, () => CustomOptionHolder.mafiaSpawnRate),
        new(RoleType.Janitor, typeof(RoleBase<Mafia.Janitor>), () => Mafia.NameColor, () => CustomOptionHolder.mafiaSpawnRate),
    ];

    public static (RoleType RoleType, Type Type)[] AllRoleTypes => [.. Roles.Select(r => (r.roleType, r.classType))];
}
