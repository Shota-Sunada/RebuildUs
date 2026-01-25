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
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Shifter)) return;

        List<PlayerControl> blockShift = null;
        if (Shifter.IsNeutral && !Shifter.ShiftPastShifters)
        {
            blockShift = [];
            for (int i = 0; i < Shifter.PastShifters.Count; i++)
            {
                var p = Helpers.PlayerById((byte)Shifter.PastShifters[i]);
                if (p != null) blockShift.Add(p);
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
    public static void MakeButtons(HudManager hm)
    {
        ShifterShiftButton = new CustomButton(
            () =>
            {
                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShifted);
                    sender.Write(Shifter.CurrentTarget.PlayerId);
                }
                RPCProcedure.SetFutureShifted(Shifter.CurrentTarget.PlayerId);
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Shifter) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return Shifter.CurrentTarget && Shifter.FutureShift == null && PlayerControl.LocalPlayer.CanMove; },
            () => { },
            AssetLoader.ShiftButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            KeyCode.F,
            false,
            Tr.Get("Hud.ShiftText")
        );
    }
    public static void SetButtonCooldowns()
    {
        ShifterShiftButton.MaxTimer = 0f;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}