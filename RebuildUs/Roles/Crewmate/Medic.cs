namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
public class Medic : RoleBase<Medic>
{
    public static Color NameColor = new Color32(126, 251, 194, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static Color ShieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    private static CustomButton MedicShieldButton;

    public PlayerControl CurrentTarget;
    public static PlayerControl Shielded;
    public static PlayerControl FutureShielded;
    public static bool UsedShield = false;

    // write configs here
    public static int ShowShielded { get { return CustomOptionHolder.MedicShowShielded.GetSelection(); } }
    public static bool ShowAttemptToShielded { get { return CustomOptionHolder.MedicShowAttemptToShielded.GetBool(); } }
    public static bool SetShieldAfterMeeting { get { return CustomOptionHolder.MedicSetShieldAfterMeeting.GetBool(); } }
    public static bool ShowAttemptToMedic { get { return CustomOptionHolder.MedicShowAttemptToMedic.GetBool(); } }

    public Medic()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medic;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!UsedShield)
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, ShieldedColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        Shielded = null;
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        MedicShieldButton = new CustomButton(
            () =>
            {
                var local = Local;
                if (local == null) return;
                MedicShieldButton.Timer = 0f;
                {
                    if (SetShieldAfterMeeting)
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShielded);
                        sender.Write(local.CurrentTarget.PlayerId);
                        RPCProcedure.SetFutureShielded(local.CurrentTarget.PlayerId);
                    }
                    else
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.MedicSetShielded);
                        sender.Write(local.CurrentTarget.PlayerId);
                        RPCProcedure.MedicSetShielded(local.CurrentTarget.PlayerId);
                    }
                }
            },
            () => { return Local != null && !UsedShield && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                var local = Local;
                return !UsedShield && local != null && local.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.ShieldButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TranslateKey.ShieldText)
        );
    }
    public static void SetButtonCooldowns()
    {
        MedicShieldButton.MaxTimer = 0f;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        Shielded = null;
        FutureShielded = null;
        UsedShield = false;
    }
}