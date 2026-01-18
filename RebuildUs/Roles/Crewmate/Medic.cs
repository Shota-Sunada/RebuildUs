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
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic))
        {
            if (!UsedShield)
            {
                CurrentTarget = Helpers.SetTarget();
                Helpers.SetPlayerOutline(CurrentTarget, ShieldedColor);
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        Shielded = null;
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        MedicShieldButton = new CustomButton(
            () =>
            {
                MedicShieldButton.Timer = 0f;
                {
                    if (SetShieldAfterMeeting)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetFutureShielded);
                        sender.Write(Local.CurrentTarget.PlayerId);
                        RPCProcedure.SetFutureShielded(Local.CurrentTarget.PlayerId);
                    }
                    else
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.MedicSetShielded);
                        sender.Write(Local.CurrentTarget.PlayerId);
                        RPCProcedure.MedicSetShielded(Local.CurrentTarget.PlayerId);
                    }
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return !UsedShield && Local.CurrentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            AssetLoader.ShieldButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("Hud.ShieldText")
        };
    }
    public override void SetButtonCooldowns()
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