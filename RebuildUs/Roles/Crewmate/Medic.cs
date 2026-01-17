namespace RebuildUs.Roles.Crewmate;

// 保護対象が志望したときにリセットする処理を書いていないので、もしかしたらバグるかも？

[HarmonyPatch]
public class Medic : RoleBase<Medic>
{
    public static Color RoleColor = new Color32(126, 251, 194, byte.MaxValue);
    public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    private static CustomButton medicShieldButton;

    public PlayerControl currentTarget;
    public PlayerControl shielded;
    public PlayerControl futureShielded;
    public bool usedShield = false;

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
    public override void OnMeetingEnd()
    {
        // Medic shield
        if (AmongUsClient.Instance.AmHost && futureShielded != null && !Player.Data.IsDead)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
            writer.Write(futureShielded.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.medicSetShielded(Player.PlayerId, futureShielded.PlayerId);
        }
    }
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
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, Medic.setShieldAfterMeeting ? (byte)CustomRPC.SetFutureShielded : (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
                writer.Write(Local.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                if (Medic.setShieldAfterMeeting)
                {
                    RPCProcedure.setFutureShielded(Local.Player.PlayerId, Local.currentTarget.PlayerId);
                }
                else
                {
                    RPCProcedure.medicSetShielded(Local.Player.PlayerId, Local.currentTarget.PlayerId);
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return !Local.usedShield && Local.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            Medic.getButtonSprite(),
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
    }
}