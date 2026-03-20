namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.NiceSwapper, RoleTeam.Crewmate, typeof(SingleRoleBase<Swapper>), nameof(CustomOptionHolder.SwapperSpawnRate))]
[RegisterRole(RoleType.EvilSwapper, RoleTeam.Impostor, typeof(SingleRoleBase<Swapper>), nameof(CustomOptionHolder.SwapperSpawnRate))]
internal class Swapper : SingleRoleBase<Swapper>
{
    internal static int RemainSwaps = 2;

    internal static Color ImpostorRoleColor => Palette.ImpostorRed;

    internal static byte PlayerId1 = byte.MaxValue;
    internal static byte PlayerId2 = byte.MaxValue;

    public Swapper()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.NiceSwapper;
        RemainSwaps = NumSwaps;
    }

    internal static Color Color
    {
        get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? Palette.ImpostorRed : new Color32(134, 55, 86, byte.MaxValue);
    }

    // write configs here
    internal static int NumSwaps
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SwapperNumSwaps.GetFloat());
    }

    internal static bool CanCallEmergency
    {
        get => CustomOptionHolder.SwapperCanCallEmergency.GetBool();
    }

    internal static bool CanOnlySwapOthers
    {
        get => CustomOptionHolder.SwapperCanOnlySwapOthers.GetBool();
    }

    internal override void OnUpdateRoleColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
        }
    }

    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;

        PlayerId1 = byte.MaxValue;
        PlayerId2 = byte.MaxValue;
        RemainSwaps = 2;
    }
}