namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Eraser : RoleBase<Eraser>
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

    internal static float Cooldown { get => CustomOptionHolder.EraserCooldown.GetFloat(); }
    internal static float CooldownIncrease { get => CustomOptionHolder.EraserCooldownIncrease.GetFloat(); }
    internal static bool CanEraseAnyone { get => CustomOptionHolder.EraserCanEraseAnyone.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Eraser local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetables = [];
            if (Spy.Exists)
            {
                List<PlayerControl> spyPlayers = Spy.AllPlayers;
                for (int i = 0; i < spyPlayers.Count; i++) untargetables.Add(spyPlayers[i]);
            }

            if (Sidekick.Exists)
            {
                List<Sidekick> sidekickPlayers = Sidekick.Players;
                for (int i = 0; i < sidekickPlayers.Count; i++)
                {
                    Sidekick sidekick = sidekickPlayers[i];
                    if (sidekick.WasTeamRed) untargetables.Add(sidekick.Player);
                }
            }

            if (Jackal.Exists)
            {
                List<Jackal> jackalPlayers = Jackal.Players;
                for (int i = 0; i < jackalPlayers.Count; i++)
                {
                    Jackal jackal = jackalPlayers[i];
                    if (jackal.WasTeamRed) untargetables.Add(jackal.Player);
                }
            }

            CurrentTarget = Helpers.SetTarget(!CanEraseAnyone, untargetablePlayers: CanEraseAnyone ? [] : untargetables);
            Helpers.SetPlayerOutline(CurrentTarget, NameColor);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _eraserButton = new(() =>
        {
            _eraserButton.MaxTimer += CooldownIncrease;
            _eraserButton.Timer = _eraserButton.MaxTimer;

            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureErased);
            sender.Write(Local.CurrentTarget.PlayerId);
            RPCProcedure.SetFutureErased(Local.CurrentTarget.PlayerId);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Eraser) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null; }, () => { _eraserButton.Timer = _eraserButton.MaxTimer; }, AssetLoader.EraserButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.EraserText));
    }

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
}