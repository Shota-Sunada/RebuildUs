namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Shifter, RoleTeam.Crewmate, typeof(MultiRoleBase<Shifter>), nameof(CustomOptionHolder.ShifterSpawnRate))]
internal class Shifter : MultiRoleBase<Shifter>
{
    public static Color Color = new Color32(102, 102, 102, byte.MaxValue);

    private static CustomButton _shifterShiftButton;
    internal static readonly List<int> PastShifters = [];

    internal static PlayerControl FutureShift;
    private static PlayerControl _currentTarget;

    internal static bool IsNeutral = false;

    public Shifter()
    {
        StaticRoleType = CurrentRoleType = RoleType.Shifter;
    }

    internal static bool ShiftsModifiers { get => CustomOptionHolder.ShifterShiftsModifiers.GetBool(); }
    internal static bool ShiftPastShifters { get => CustomOptionHolder.ShifterPastShifters.GetBool(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Shifter))
        {
            return;
        }

        List<PlayerControl> blockShift = null;
        if (IsNeutral && !ShiftPastShifters)
        {
            blockShift = [];
            for (var i = 0; i < PastShifters.Count; i++)
            {
                var p = Helpers.PlayerById((byte)PastShifters[i]);
                if (p != null)
                {
                    blockShift.Add(p);
                }
            }
        }

        _currentTarget = Helpers.SetTarget(untargetablePlayers: blockShift);
        if (FutureShift == null)
        {
            Helpers.SetPlayerOutline(_currentTarget, RoleColor);
        }
    }

    [CustomEvent(CustomEventType.HandleDisconnect)]
    internal void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (FutureShift == player)
        {
            FutureShift = null;
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _shifterShiftButton = new(
            () =>
            {
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureShifted);
                    sender.Write(_currentTarget.PlayerId);
                }
                RPCProcedure.SetFutureShifted(_currentTarget.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Shifter) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return _currentTarget && FutureShift == null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.ShiftButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.ShiftText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        _shifterShiftButton.MaxTimer = 0f;
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}