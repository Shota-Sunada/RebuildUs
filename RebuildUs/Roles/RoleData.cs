using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles;

public static class RoleData
{
    public static (ERoleType RoleType, Type Type)[] AllRoleTypes = [
        (ERoleType.Jester, typeof(RoleBase<Jester>)),
        (ERoleType.Mayor, typeof(RoleBase<Mayor>)),
        (ERoleType.Engineer, typeof(RoleBase<Engineer>)),
        (ERoleType.BountyHunter, typeof(RoleBase<BountyHunter>)),
        (ERoleType.Arsonist, typeof(RoleBase<Arsonist>)),
        (ERoleType.Vulture, typeof(RoleBase<Vulture>)),
        (ERoleType.Jackal, typeof(RoleBase<Jackal>)),
        (ERoleType.Sidekick, typeof(RoleBase<Sidekick>)),
    ];
}