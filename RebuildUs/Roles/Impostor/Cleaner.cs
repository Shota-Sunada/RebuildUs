namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Cleaner : RoleBase<Cleaner>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static CustomButton cleanerCleanButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.cleanerCooldown.GetFloat(); } }

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
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Cleaner) && cleanerCleanButton != null)
        {
            cleanerCleanButton.Timer = Player.killTimer;
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        cleanerCleanButton = new CustomButton(
                () =>
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance && CachedPlayer.LocalPlayer.PlayerControl.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.CleanBody(playerInfo.PlayerId);

                                    Player.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Cleaner) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
                AssetLoader.CleanButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("CleanText")
        };
    }
    public override void SetButtonCooldowns()
    {
        cleanerCleanButton.MaxTimer = cooldown;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
