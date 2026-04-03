namespace RebuildUs.Roles.Crewmate;

[RegisterRole(RoleType.Bakery, RoleTeam.Crewmate, typeof(SingleRoleBase<Bakery>), nameof(CustomOptionHolder.BakerySpawnRate))]
internal class Bakery : SingleRoleBase<Bakery>
{
    public static Color Color = new Color32(233, 213, 172, byte.MaxValue);

    internal static bool WasBomb = false;

    internal static float BombRate { get { return CustomOptionHolder.BakeryBombRate.GetFloat(); } }
    internal static int BombType { get { return CustomOptionHolder.BakeryBombType.GetSelection(); } }

    public Bakery()
    {
        StaticRoleType = CurrentRoleType = RoleType.Bakery;
        WasBomb = false;
    }
}