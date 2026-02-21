using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Cleaner : MultiRoleBase<Cleaner>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static CustomButton CleanerCleanButton;

    public Cleaner()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Cleaner;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown { get => CustomOptionHolder.CleanerCooldown.GetFloat(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }

    internal override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Cleaner) && CleanerCleanButton != null) CleanerCleanButton.Timer = Player.killTimer;
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        CleanerCleanButton = new(() =>
                                 {
                                     Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                     float maxDist = PlayerControl.LocalPlayer.MaxReportDistance;
                                     Il2CppArrayBase<DeadBody> bodies = Object.FindObjectsOfType<DeadBody>();

                                     foreach (var body in bodies)
                                     {
                                         if (body == null || body.Reported) continue;

                                         Vector2 bodyPosition = body.TruePosition;
                                         float dist = Vector2.Distance(truePosition, bodyPosition);

                                         if (dist <= maxDist && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, bodyPosition, Constants.ShipAndObjectsMask, false))
                                         {
                                             NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                                             if (playerInfo == null) continue;

                                             {
                                                 using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.CleanBody);
                                                 sender.Write(playerInfo.PlayerId);
                                             }
                                             RPCProcedure.CleanBody(playerInfo.PlayerId);

                                             Local.Player.killTimer = CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer;
                                             break;
                                         }
                                     }
                                 }, () => Local != null && PlayerControl.LocalPlayer.IsAlive(), () => hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove,
                                 () => { CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer; },
                                 AssetLoader.CleanButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.CleanText));
    }

    internal static void SetButtonCooldowns()
    {
        CleanerCleanButton.MaxTimer = Cooldown;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}