namespace RebuildUs.Roles;

public static class RoleData
{
    public static (ERoleType RoleType, Type Type)[] AllRoleTypes = [
        (ERoleType.NoRole, typeof(RoleBase<>))
    ];
}