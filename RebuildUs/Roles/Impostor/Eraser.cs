namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Eraser, RoleTeam.Impostor, typeof(MultiRoleBase<Eraser>), nameof(CustomOptionHolder.EraserSpawnRate))]
internal class Eraser : MultiRoleBase<Eraser>
{
    public static Color Color = Palette.ImpostorRed;

    private static CustomButton EraserButton;

    internal static List<PlayerControl> FutureErased = [];
    internal PlayerControl CurrentTarget;

    public Eraser()
    {
        StaticRoleType = CurrentRoleType = RoleType.Eraser;
    }

    internal static float Cooldown { get => CustomOptionHolder.EraserCooldown.GetFloat(); }
    internal static float CooldownIncrease { get => CustomOptionHolder.EraserCooldownIncrease.GetFloat(); }
    internal static bool CanEraseAnyone { get => CustomOptionHolder.EraserCanEraseAnyone.GetBool(); }

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
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        EraserButton = new(
            nameof(EraserButton),
            () =>
            {
                EraserButton.MaxTimer += CooldownIncrease;
                EraserButton.Timer = EraserButton.MaxTimer;

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureErased);
                sender.Write(Local.CurrentTarget.PlayerId);
                RPCProcedure.SetFutureErased(Local.CurrentTarget.PlayerId);
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
                EraserButton.Timer = EraserButton.MaxTimer;
            },
            AssetLoader.EraserButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.EraserText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        EraserButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        Players.Clear();
        FutureErased = [];
    }
}