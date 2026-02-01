namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Cleaner : RoleBase<Cleaner>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static CustomButton CleanerCleanButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.CleanerCooldown.GetFloat(); } }

    public Cleaner()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Cleaner;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Cleaner) && CleanerCleanButton != null)
        {
            CleanerCleanButton.Timer = Player.killTimer;
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        CleanerCleanButton = new CustomButton(
            () =>
            {
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDist = PlayerControl.LocalPlayer.MaxReportDistance;
                var bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();

                for (int i = 0; i < bodies.Length; i++)
                {
                    var body = bodies[i];
                    if (body == null || body.Reported) continue;

                    var bodyPosition = body.TruePosition;
                    var dist = Vector2.Distance(truePosition, bodyPosition);

                    if (dist <= maxDist && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, bodyPosition, Constants.ShipAndObjectsMask, false))
                    {
                        var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                        if (playerInfo == null) continue;

                        {
                            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.CleanBody);
                            sender.Write(playerInfo.PlayerId);
                        }
                        RPCProcedure.CleanBody(playerInfo.PlayerId);

                        Local.Player.killTimer = CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer;
                        break;
                    }
                }
            },
            () => { return Local != null && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
            () => { CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer; },
            AssetLoader.CleanButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get("CleanText")
        );
    }
    public static void SetButtonCooldowns()
    {
        CleanerCleanButton.MaxTimer = Cooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
