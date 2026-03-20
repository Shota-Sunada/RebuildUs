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
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Shifter;
    }


    internal static bool ShiftsModifiers
    {
        get => CustomOptionHolder.ShifterShiftsModifiers.GetBool();
    }

    internal static bool ShiftPastShifters
    {
        get => CustomOptionHolder.ShifterPastShifters.GetBool();
    }

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
        _shifterShiftButton = new(() =>
            {
                SetFutureShifted(PlayerControl.LocalPlayer, _currentTarget.PlayerId);
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

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _shifterShiftButton.MaxTimer = 0f;
    }

    [MethodRpc((uint)CustomRPC.ShifterShift)]
    internal static void ShifterShift(PlayerControl sender, byte targetId)
    {
        if (Players.Count == 0)
        {
            return;
        }
        var oldShifter = Players[0];
        var player = Helpers.PlayerById(targetId);
        if (player == null || oldShifter == null)
        {
            return;
        }

        var oldShifterPlayer = oldShifter.Player;
        FutureShift = null;

        // Suicide (exile) when impostor or impostor variants
        if (!IsNeutral
            && (player.Data.Role.IsImpostor
                || player.IsNeutral()
                || player.HasModifier(ModifierType.Madmate)
                || player.HasModifier(ModifierType.CreatedMadmate)))
        {
            oldShifterPlayer.Exiled();
            GameHistory.FinalStatuses[oldShifterPlayer.PlayerId] = FinalStatus.Suicide;
            return;
        }

        if (ShiftsModifiers)
        {
            // Switch shield
            if (Medic.Shielded != null && Medic.Shielded == player)
            {
                Medic.Shielded = oldShifterPlayer;
            }
            else if (Medic.Shielded != null && Medic.Shielded == oldShifterPlayer)
            {
                Medic.Shielded = player;
            }

            player.SwapModifiers(oldShifterPlayer);
            Lovers.SwapLovers(oldShifterPlayer, player);
        }

        // Shift roles (now a true swap)
        player.SwapRoles(oldShifterPlayer);

        if (IsNeutral)
        {
            PastShifters.Add(oldShifterPlayer.PlayerId);

            if (player.Data.Role.IsImpostor)
            {
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(oldShifterPlayer, RoleTypes.Impostor);
            }
        }
        else
        {
            // For Crewmate Shifter, the original target (who now has the Shifter role due to the swap) should lose it and become a plain Crewmate.
            player.EraseRole(RoleType.Shifter);
        }

        // Set cooldowns to max for both players
        if (PlayerControl.LocalPlayer == oldShifterPlayer || PlayerControl.LocalPlayer == player)
        {
            CustomButton.ResetAllCooldowns();
        }
    }

    [MethodRpc((uint)CustomRPC.SetFutureShifted)]
    internal static void SetFutureShifted(PlayerControl sender, byte playerId)
    {
        if (IsNeutral && !ShiftPastShifters && PastShifters.Contains(playerId))
        {
            return;
        }
        FutureShift = Helpers.PlayerById(playerId);
    }

    [MethodRpc((uint)CustomRPC.SetShifterType)]
    internal static void SetShifterType(PlayerControl sender, bool isNeutral)
    {
        IsNeutral = isNeutral;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}