namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
[RegisterRole(RoleType.Medic, RoleTeam.Crewmate, typeof(SingleRoleBase<Medic>), nameof(CustomOptionHolder.MedicSpawnRate))]
internal class Medic : SingleRoleBase<Medic>
{
    internal static new Color RoleColor = new Color32(126, 251, 194, byte.MaxValue);

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
        _medicShieldButton = new(() =>
            {
                var local = Local;
                if (local == null)
                {
                    return;
                }
                _medicShieldButton.Timer = 0f;
                {
                    if (SetShieldAfterMeeting)
                    {
                        SetFutureShielded(PlayerControl.LocalPlayer, local._currentTarget.PlayerId);
                    }
                    else
                    {
                        MedicSetShielded(PlayerControl.LocalPlayer, local._currentTarget.PlayerId);
                    }
                }
            },
            () => Local != null && !UsedShield && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                var local = Local;
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

    [RegisterCustomButton]
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

    [MethodRpc((uint)CustomRPC.MedicSetShielded)]
    internal static void MedicSetShielded(PlayerControl sender, byte shieldedId)
    {
        UsedShield = true;
        Shielded = Helpers.PlayerById(shieldedId);
        FutureShielded = null;
    }

    [MethodRpc((uint)CustomRPC.ShieldedMurderAttempt)]
    internal static void ShieldedMurderAttempt(PlayerControl sender)
    {
        if (!Exists || Shielded == null)
        {
            return;
        }

        var isShieldedAndShow = Shielded == PlayerControl.LocalPlayer && ShowAttemptToShielded;
        var isMedicAndShow = PlayerControl.LocalPlayer.IsRole(RoleType.Medic) && ShowAttemptToMedic;

        if (!isShieldedAndShow && !isMedicAndShow || FastDestroyableSingleton<HudManager>.Instance?.FullScreen == null)
        {
            return;
        }
        var c = Palette.ImpostorRed;
        Helpers.ShowFlash(new(c.r, c.g, c.b));
    }

    [MethodRpc((uint)CustomRPC.SetFutureShielded)]
    internal static void SetFutureShielded(PlayerControl sender, byte playerId)
    {
        FutureShielded = Helpers.PlayerById(playerId);
        UsedShield = true;
    }
}