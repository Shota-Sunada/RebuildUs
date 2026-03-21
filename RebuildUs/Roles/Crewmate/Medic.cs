namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
[RegisterRole(RoleType.Medic, RoleTeam.Crewmate, typeof(SingleRoleBase<Medic>), nameof(CustomOptionHolder.MedicSpawnRate))]
internal class Medic : SingleRoleBase<Medic>
{
    public static Color Color = new Color32(126, 251, 194, byte.MaxValue);

    internal static Color ShieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    private static CustomButton _medicShieldButton;
    internal static PlayerControl Shielded;
    internal static PlayerControl FutureShielded;
    internal static bool UsedShield;

    private PlayerControl _currentTarget;

    public Medic()
    {
        StaticRoleType = CurrentRoleType = RoleType.Medic;
    }

    internal static int ShowShielded { get => CustomOptionHolder.MedicShowShielded.GetSelection(); }
    internal static bool ShowAttemptToShielded { get => CustomOptionHolder.MedicShowAttemptToShielded.GetBool(); }
    private static bool SetShieldAfterMeeting { get => CustomOptionHolder.MedicSetShieldAfterMeeting.GetBool(); }
    internal static bool ShowAttemptToMedic { get => CustomOptionHolder.MedicShowAttemptToMedic.GetBool(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (UsedShield)
        {
            return;
        }
        _currentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(_currentTarget, ShieldedColor);
    }

    [CustomEvent(CustomEventType.OnDeath)]
    internal void OnDeath(PlayerControl killer)
    {
        Shielded = null;
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _medicShieldButton = new(
            () =>
            {
                if (Local == null)
                {
                    return;
                }
                _medicShieldButton.Timer = 0f;
                {
                    if (SetShieldAfterMeeting)
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShielded);
                        sender.Write(Local._currentTarget.PlayerId);
                        RPCProcedure.SetFutureShielded(Local._currentTarget.PlayerId);
                    }
                    else
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.MedicSetShielded);
                        sender.Write(Local._currentTarget.PlayerId);
                        RPCProcedure.MedicSetShielded(Local._currentTarget.PlayerId);
                    }
                }
            },
            () => Local != null && !UsedShield && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                return !UsedShield && Local != null && Local._currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.ShieldButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.ShieldText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        _medicShieldButton.MaxTimer = 0f;
    }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;

        Shielded = null;
        FutureShielded = null;
        UsedShield = false;
    }
}