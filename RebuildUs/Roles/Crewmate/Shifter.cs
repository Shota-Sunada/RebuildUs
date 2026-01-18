namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Shifter : RoleBase<Shifter>
{
    public static Color NameColor = new Color32(102, 102, 102, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton shifterShiftButton;
    public static List<int> pastShifters = [];

    public static PlayerControl futureShift;
    public static PlayerControl currentTarget;

    public static bool isNeutral = false;

    public static bool shiftsModifiers { get { return CustomOptionHolder.shifterShiftsModifiers.GetBool(); } }
    public static bool shiftPastShifters { get { return CustomOptionHolder.shifterPastShifters.GetBool(); } }

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
        if (Shifter.isNeutral && !Shifter.shiftPastShifters)
        {
            blockShift = [];
            foreach (var playerId in Shifter.pastShifters)
            {
                blockShift.Add(Helpers.PlayerById((byte)playerId));
            }
        }

        Shifter.currentTarget = Helpers.SetTarget(untargetablePlayers: blockShift);
        if (Shifter.futureShift == null) Helpers.SetPlayerOutline(Shifter.currentTarget, RoleColor);
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (futureShift == player) futureShift = null;
    }
    public override void MakeButtons(HudManager hm)
    {
        shifterShiftButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureShifted, Hazel.SendOption.Reliable, -1);
                    writer.Write(Shifter.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Shifter) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return Shifter.currentTarget && Shifter.futureShift == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
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
        shifterShiftButton.MaxTimer = 0f;
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
