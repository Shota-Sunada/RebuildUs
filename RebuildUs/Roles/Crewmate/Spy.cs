namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Spy, RoleTeam.Crewmate, typeof(SingleRoleBase<Spy>), nameof(CustomOptionHolder.SpySpawnRate))]
internal class Spy : SingleRoleBase<Spy>
{
    public static Color Color = Palette.ImpostorRed;

    public Spy()
    {
        StaticRoleType = CurrentRoleType = RoleType.Spy;
    }

    internal static bool ImpostorsCanKillAnyone { get => CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool(); }
    internal static bool CanEnterVents { get => CustomOptionHolder.SpyCanEnterVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SpyHasImpostorVision.GetBool(); }

    internal override void OnUpdateRoleColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && localPlayer.IsTeamImpostor())
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
        }
    }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}