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
        var local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetables = [];
            if (Spy.Exists)
            {
                var spyPlayers = Spy.AllPlayers;
                for (var i = 0; i < spyPlayers.Count; i++)
                {
                    untargetables.Add(spyPlayers[i]);
                }
            }
            if (Sidekick.Exists)
            {
                var sidekickPlayers = Sidekick.Players;
                for (var i = 0; i < sidekickPlayers.Count; i++)
                {
                    var sidekick = sidekickPlayers[i];
                    if (sidekick.WasTeamRed)
                    {
                        untargetables.Add(sidekick.Player);
                    }
                }
            }
            if (Jackal.Exists)
            {
                var jackalPlayers = Jackal.Players;
                for (var i = 0; i < jackalPlayers.Count; i++)
                {
                    var jackal = jackalPlayers[i];
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
    public static void MakeButtons(HudManager hm)
    {
        EraserButton = new CustomButton(
            () =>
            {
                EraserButton.MaxTimer += CooldownIncrease;
                EraserButton.Timer = EraserButton.MaxTimer;

                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetFutureErased);
                sender.Write(Local.CurrentTarget.PlayerId);
                RPCProcedure.SetFutureErased(Local.CurrentTarget.PlayerId);
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Eraser) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null; },
            () => { EraserButton.Timer = EraserButton.MaxTimer; },
            AssetLoader.EraserButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("Hud.EraserText")
        };
    }
    public static void SetButtonCooldowns()
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