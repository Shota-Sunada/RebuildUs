namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Spy, RoleTeam.Crewmate, typeof(SingleRoleBase<Spy>), nameof(CustomOptionHolder.SpySpawnRate))]
internal class Spy : SingleRoleBase<Spy>
{
    internal static Color Color = Palette.ImpostorRed;

    public Spy()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Spy;
    }


    // write configs here
    internal static bool ImpostorsCanKillAnyone
    {
        get => CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool();
    }

    internal static bool CanEnterVents
    {
        get => CustomOptionHolder.SpyCanEnterVents.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.SpyHasImpostorVision.GetBool();
    }

    internal override void OnUpdateRoleColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null && localPlayer.IsTeamImpostor())
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
        }
    }



    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}