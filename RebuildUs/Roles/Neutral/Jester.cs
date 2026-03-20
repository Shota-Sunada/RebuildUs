namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Jester, RoleTeam.Neutral, typeof(SingleRoleBase<Jester>), nameof(CustomOptionHolder.JesterSpawnRate))]
internal class Jester : SingleRoleBase<Jester>
{
    internal static Color Color = new Color32(236, 98, 165, byte.MaxValue);

    internal static bool TriggerJesterWin;

    public Jester()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Jester;
    }

    internal static bool CanCallEmergency
    {
        get => CustomOptionHolder.JesterCanCallEmergency.GetBool();
    }

    internal static bool CanSabotage
    {
        get => CustomOptionHolder.JesterCanSabotage.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.JesterHasImpostorVision.GetBool();
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        TriggerJesterWin = false;

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}