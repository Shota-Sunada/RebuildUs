namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Eraser : RoleBase<Eraser>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton EraserButton;

    // write configs here
    public static List<PlayerControl> FutureErased = [];
    public PlayerControl CurrentTarget;
    public static float Cooldown { get { return CustomOptionHolder.EraserCooldown.GetFloat(); } }
    public static float CooldownIncrease { get { return CustomOptionHolder.EraserCooldownIncrease.GetFloat(); } }
    public static bool CanEraseAnyone { get { return CustomOptionHolder.EraserCanEraseAnyone.GetBool(); } }

    public Eraser()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Eraser;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Eraser))
        {
            List<PlayerControl> untargetables = [];
            if (Spy.Exists) untargetables.AddRange(Spy.AllPlayers);
            if (Sidekick.Exists)
            {
                foreach (var sidekick in Sidekick.Players)
                {
                    if (sidekick.WasTeamRed)
                    {
                        untargetables.Add(sidekick.Player);
                    }
                }
            }
            if (Jackal.Exists)
            {
                foreach (var jackal in Jackal.Players)
                {
                    if (jackal.WasTeamRed)
                    {
                        untargetables.Add(jackal.Player);
                    }
                }
            }
            CurrentTarget = Helpers.SetTarget(onlyCrewmates: !CanEraseAnyone, untargetablePlayers: CanEraseAnyone ? [] : untargetables);
            Helpers.SetPlayerOutline(CurrentTarget, NameColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        EraserButton = new CustomButton(
                () =>
                {
                    EraserButton.MaxTimer += CooldownIncrease;
                    EraserButton.Timer = EraserButton.MaxTimer;

                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetFutureErased);
                    sender.Write(CurrentTarget.PlayerId);
                    RPCProcedure.SetFutureErased(CurrentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Eraser) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && CurrentTarget != null; },
                () => { EraserButton.Timer = EraserButton.MaxTimer; },
                AssetLoader.EraserButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("Hud.EraserText")
        };
    }
    public override void SetButtonCooldowns()
    {
        EraserButton.MaxTimer = Cooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}