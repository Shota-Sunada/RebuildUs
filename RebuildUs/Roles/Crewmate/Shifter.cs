namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Shifter : RoleBase<Shifter>
{
    public static Color NameColor = new Color32(102, 102, 102, byte.MaxValue);

    private static CustomButton _shifterShiftButton;
    public static List<int> PastShifters = [];

    public static PlayerControl FutureShift;
    public static PlayerControl CurrentTarget;

    public static bool IsNeutral = false;

    public Shifter()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Shifter;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    public static bool ShiftsModifiers
    {
        get => CustomOptionHolder.ShifterShiftsModifiers.GetBool();
    }

    public static bool ShiftPastShifters
    {
        get => CustomOptionHolder.ShifterPastShifters.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Shifter)) return;

        List<PlayerControl> blockShift = null;
        if (IsNeutral && !ShiftPastShifters)
        {
            blockShift = [];
            for (var i = 0; i < PastShifters.Count; i++)
            {
                var p = Helpers.PlayerById((byte)PastShifters[i]);
                if (p != null) blockShift.Add(p);
            }
        }

        CurrentTarget = Helpers.SetTarget(untargetablePlayers: blockShift);
        if (FutureShift == null) Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
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
        _shifterShiftButton = new(() =>
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShifted);
                sender.Write(CurrentTarget.PlayerId);
            }
            RPCProcedure.SetFutureShifted(CurrentTarget.PlayerId);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Shifter) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return CurrentTarget && FutureShift == null && PlayerControl.LocalPlayer.CanMove; }, () => { }, AssetLoader.ShiftButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.ShiftText));
    }

    public static void SetButtonCooldowns()
    {
        _shifterShiftButton.MaxTimer = 0f;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
