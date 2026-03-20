namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Cleaner, RoleTeam.Impostor, typeof(MultiRoleBase<Cleaner>), nameof(CustomOptionHolder.CleanerSpawnRate))]
internal class Cleaner : MultiRoleBase<Cleaner>
{
    public static Color Color = Palette.ImpostorRed;

    internal static CustomButton CleanerCleanButton;

    public Cleaner()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Cleaner;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.CleanerCooldown.GetFloat();
    }

    [CustomEvent(CustomEventType.OnKill)]
    internal void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Cleaner) && CleanerCleanButton != null)
        {
            CleanerCleanButton.Timer = Player.killTimer;
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        CleanerCleanButton = new(() =>
            {
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDist = PlayerControl.LocalPlayer.MaxReportDistance;
                var bodies = UnityObject.FindObjectsOfType<DeadBody>();

                foreach (var body in bodies)
                {
                    if (body == null || body.Reported)
                    {
                        continue;
                    }

                    var bodyPosition = body.TruePosition;
                    var dist = Vector2.Distance(truePosition, bodyPosition);

                    if (dist <= maxDist
                        && PlayerControl.LocalPlayer.CanMove
                        && !PhysicsHelpers.AnythingBetween(truePosition, bodyPosition, Constants.ShipAndObjectsMask, false))
                    {
                        var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                        if (playerInfo == null)
                        {
                            continue;
                        }

                        {
                            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.CleanBody);
                            sender.Write(playerInfo.PlayerId);
                        }
                        RPCProcedure.CleanBody(playerInfo.PlayerId);

                        Local.Player.killTimer = CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer;
                        break;
                    }
                }
            },
            () => Local != null && PlayerControl.LocalPlayer.IsAlive(),
            () => hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove,
            () =>
            {
                CleanerCleanButton.Timer = CleanerCleanButton.MaxTimer;
            },
            AssetLoader.CleanButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.CleanText));
    }

    [RegisterCustomButton]
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