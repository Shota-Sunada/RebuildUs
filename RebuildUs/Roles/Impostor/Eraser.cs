namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Eraser, RoleTeam.Impostor, typeof(MultiRoleBase<Eraser>), nameof(Eraser.NameColor), nameof(CustomOptionHolder.EraserSpawnRate))]
internal class Eraser : MultiRoleBase<Eraser>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _eraserButton;

    // write configs here
    internal static List<PlayerControl> FutureErased = [];
    internal PlayerControl CurrentTarget;

    public Eraser()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Eraser;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static float Cooldown
    {
        get => CustomOptionHolder.EraserCooldown.GetFloat();
    }

    internal static float CooldownIncrease
    {
        get => CustomOptionHolder.EraserCooldownIncrease.GetFloat();
    }

    internal static bool CanEraseAnyone
    {
        get => CustomOptionHolder.EraserCanEraseAnyone.GetBool();
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetables = [];
            if (Spy.Exists)
            {
                untargetables.Add(Spy.PlayerControl);
            }

            if (Sidekick.Exists && Sidekick.Instance.WasTeamRed)
            {
                untargetables.Add(Sidekick.PlayerControl);
            }

            if (Jackal.Exists && Jackal.Instance.WasTeamRed)
            {
                untargetables.Add(Jackal.PlayerControl);
            }

            CurrentTarget = Helpers.SetTarget(!CanEraseAnyone, untargetablePlayers: CanEraseAnyone ? [] : untargetables);
            Helpers.SetPlayerOutline(CurrentTarget, NameColor);
        }
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _eraserButton = new(() =>
            {
                _eraserButton.MaxTimer += CooldownIncrease;
                _eraserButton.Timer = _eraserButton.MaxTimer;

                SetFutureErased(PlayerControl.LocalPlayer, Local.CurrentTarget.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Eraser) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null;
            },
            () =>
            {
                _eraserButton.Timer = _eraserButton.MaxTimer;
            },
            AssetLoader.EraserButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.EraserText));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _eraserButton.MaxTimer = Cooldown;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        FutureErased = [];
    }

    [MethodRpc((uint)CustomRPC.ErasePlayerRoles)]
    internal static void ErasePlayerRoles(PlayerControl sender, byte playerId)
    {
        ErasePlayerRolesLocal(playerId);
    }

    internal static void ErasePlayerRolesLocal(byte playerId, bool ignoreLovers = false, bool clearNeutralTasks = true)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }

        // Don't give a former neutral role tasks because that destroys the balance.
        if (player.IsNeutral() && clearNeutralTasks)
        {
            player.ClearAllTasks();
        }

        player.EraseAllRoles();
        player.EraseAllModifiers();

        if (!ignoreLovers && player.IsLovers())
        {
            // The whole Lover couple is being erased
            Lovers.EraseCouple(player);
        }
    }

    [MethodRpc((uint)CustomRPC.SetFutureErased)]
    internal static void SetFutureErased(PlayerControl sender, byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        FutureErased ??= [];
        if (player != null)
        {
            FutureErased.Add(player);
        }
    }
}