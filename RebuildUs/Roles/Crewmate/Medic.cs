namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
public class Medic : RoleBase<Medic>
{
    public static Color RoleColor = new Color32(126, 251, 194, byte.MaxValue);
    public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    private static CustomButton medicShieldButton;

    public PlayerControl currentTarget;
    public static PlayerControl shielded;
    public static PlayerControl futureShielded;
    public static bool usedShield = false;

    // write configs here
    public static int showShielded { get { return CustomOptionHolder.medicShowShielded.GetSelection(); } }
    public static bool showAttemptToShielded { get { return CustomOptionHolder.medicShowAttemptToShielded.GetBool(); } }
    public static bool setShieldAfterMeeting { get { return CustomOptionHolder.medicSetShieldAfterMeeting.GetBool(); } }
    public static bool showAttemptToMedic { get { return CustomOptionHolder.medicShowAttemptToMedic.GetBool(); } }

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
            if (!usedShield)
            {
                currentTarget = Helpers.SetTarget();
                Helpers.SetPlayerOutline(currentTarget, shieldedColor);
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        shielded = null;
    }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        medicShieldButton = new CustomButton(
            () =>
            {
                medicShieldButton.Timer = 0f;
                {
                    if (setShieldAfterMeeting)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetFutureShielded);
                        sender.Write(Local.currentTarget.PlayerId);
                        RPCProcedure.setFutureShielded(Local.currentTarget.PlayerId);
                    }
                    else
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.MedicSetShielded);
                        sender.Write(Local.currentTarget.PlayerId);
                        RPCProcedure.medicSetShielded(Local.currentTarget.PlayerId);
                    }
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return !usedShield && Local.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            AssetLoader.ShieldButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("ShieldText")
        };
    }
    public override void SetButtonCooldowns()
    {
        medicShieldButton.MaxTimer = 0f;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        shielded = null;
        futureShielded = null;
        usedShield = false;
    }
}