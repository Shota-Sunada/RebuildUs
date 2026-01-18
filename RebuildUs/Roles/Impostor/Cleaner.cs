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
    public override void MakeButtons(HudManager hm)
    {
        CleanerCleanButton = new CustomButton(
                () =>
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    {
                                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.CleanBody);
                                        sender.Write(playerInfo.PlayerId);
                                    }
                                    RPCProcedure.CleanBody(playerInfo.PlayerId);

                                    Player.killTimer = CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Cleaner) && PlayerControl.LocalPlayer.IsAlive(); },
                () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
                () => { CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer; },
                AssetLoader.CleanButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("Hud.CleanText")
        };
    }
    public override void SetButtonCooldowns()
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