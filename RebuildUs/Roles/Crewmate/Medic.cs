namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
[RegisterRole(RoleType.Medic, RoleTeam.Crewmate, typeof(SingleRoleBase<Medic>), nameof(Medic.NameColor), nameof(CustomOptionHolder.MedicSpawnRate))]
internal class Medic : SingleRoleBase<Medic>
{
    internal static Color NameColor = new Color32(126, 251, 194, byte.MaxValue);

    internal static Color ShieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    private static CustomButton _medicShieldButton;
    internal static PlayerControl Shielded;
    internal static PlayerControl FutureShielded;
    internal static bool UsedShield;

    private PlayerControl _currentTarget;

    public Medic()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medic;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static int ShowShielded
    {
        get => CustomOptionHolder.MedicShowShielded.GetSelection();
    }

    internal static bool ShowAttemptToShielded
    {
        get => CustomOptionHolder.MedicShowAttemptToShielded.GetBool();
    }

    private static bool SetShieldAfterMeeting
    {
        get => CustomOptionHolder.MedicSetShieldAfterMeeting.GetBool();
    }

    internal static bool ShowAttemptToMedic
    {
        get => CustomOptionHolder.MedicShowAttemptToMedic.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (UsedShield)
        {
            return;
        }
        _currentTarget = Helpers.SetTarget();
        Helpers.SetPlayerOutline(_currentTarget, ShieldedColor);
    }

    internal override void OnKill(PlayerControl target) { }

    internal override void OnDeath(PlayerControl killer = null)
    {
        Shielded = null;
    }

    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _medicShieldButton = new(() =>
            {
                Medic local = Local;
                if (local == null)
                {
                    return;
                }
                _medicShieldButton.Timer = 0f;
                {
                    if (SetShieldAfterMeeting)
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShielded);
                        sender.Write(local._currentTarget.PlayerId);
                        RPCProcedure.SetFutureShielded(local._currentTarget.PlayerId);
                    }
                    else
                    {
                        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.MedicSetShielded);
                        sender.Write(local._currentTarget.PlayerId);
                        RPCProcedure.MedicSetShielded(local._currentTarget.PlayerId);
                    }
                }
            },
            () => Local != null && !UsedShield && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                Medic local = Local;
                return !UsedShield && local != null && local._currentTarget && PlayerControl.LocalPlayer.CanMove;
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

    internal static void SetButtonCooldowns()
    {
        _medicShieldButton.MaxTimer = 0f;
    }

    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;

        Shielded = null;
        FutureShielded = null;
        UsedShield = false;
    }
}