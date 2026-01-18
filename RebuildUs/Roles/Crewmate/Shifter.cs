namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Shifter : RoleBase<Shifter>
{
    public static Color NameColor = new Color32(102, 102, 102, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton ShifterShiftButton;
    public static List<int> PastShifters = [];

    public static PlayerControl FutureShift;
    public static PlayerControl CurrentTarget;

    public static bool IsNeutral = false;

    public static bool ShiftsModifiers { get { return CustomOptionHolder.ShifterShiftsModifiers.GetBool(); } }
    public static bool ShiftPastShifters { get { return CustomOptionHolder.ShifterPastShifters.GetBool(); } }

    public Shifter()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Shifter;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Shifter)) return;

        List<PlayerControl> blockShift = null;
        if (Shifter.IsNeutral && !Shifter.ShiftPastShifters)
        {
            blockShift = [];
            foreach (var playerId in Shifter.PastShifters)
            {
                blockShift.Add(Helpers.PlayerById((byte)playerId));
            }
        }

        Shifter.CurrentTarget = Helpers.SetTarget(untargetablePlayers: blockShift);
        if (Shifter.FutureShift == null) Helpers.SetPlayerOutline(Shifter.CurrentTarget, RoleColor);
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (FutureShift == player) FutureShift = null;
    }
    public override void MakeButtons(HudManager hm)
    {
        ShifterShiftButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureShifted, Hazel.SendOption.Reliable, -1);
                    writer.Write(Shifter.CurrentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.SetFutureShifted(Shifter.CurrentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Shifter) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return Shifter.CurrentTarget && Shifter.FutureShift == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { },
                AssetLoader.ShiftButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.UseButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("ShiftText")
        };
    }
    public override void SetButtonCooldowns()
    {
        ShifterShiftButton.MaxTimer = 0f;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}