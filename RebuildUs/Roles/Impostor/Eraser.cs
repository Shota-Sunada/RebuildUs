using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Eraser : RoleBase<Eraser>
{
    public static Color RoleColor = Palette.ImpostorRed;
    private static CustomButton eraserButton;

    // write configs here
    public static List<PlayerControl> futureErased = [];
    public PlayerControl currentTarget;
    public static float cooldown { get { return CustomOptionHolder.eraserCooldown.GetFloat(); } }
    public static float cooldownIncrease { get { return CustomOptionHolder.eraserCooldownIncrease.GetFloat(); } }
    public static bool canEraseAnyone { get { return CustomOptionHolder.eraserCanEraseAnyone.GetBool(); } }

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
            currentTarget = Helpers.SetTarget(onlyCrewmates: !canEraseAnyone, untargetablePlayers: canEraseAnyone ? [] : untargetables);
            Helpers.SetPlayerOutline(currentTarget, Eraser.RoleColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        eraserButton = new CustomButton(
                () =>
                {
                    eraserButton.MaxTimer += cooldownIncrease;
                    eraserButton.Timer = eraserButton.MaxTimer;

                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetFutureErased);
                    sender.Write(currentTarget.PlayerId);
                    RPCProcedure.setFutureErased(currentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Eraser) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && currentTarget != null; },
                () => { eraserButton.Timer = eraserButton.MaxTimer; },
                AssetLoader.EraserButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("EraserText")
        };
    }
    public override void SetButtonCooldowns()
    {
        eraserButton.MaxTimer = cooldown;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}