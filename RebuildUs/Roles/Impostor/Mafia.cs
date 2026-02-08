using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Impostor;

public static class Mafia
{
    public static Color NameColor = Palette.ImpostorRed;

    public static bool IsGodfatherDead;
    public static bool IsMafiosoDead;
    public static bool IsJanitorDead;

    public static void ClearAndReload()
    {
        IsGodfatherDead = false;
        IsMafiosoDead = false;
        IsJanitorDead = false;
        Godfather.Clear();
        Mafioso.Clear();
        Janitor.Clear();
    }

    [HarmonyPatch]
    public class Godfather : RoleBase<Godfather>
    {
        public Godfather()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Godfather;
        }

        public override Color RoleColor
        {
            get => NameColor;
        }

        public override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaG)})" : "";
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void OnIntroEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer = null)
        {
            IsGodfatherDead = true;
        }

        public override void OnFinishShipStatusBegin() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class Mafioso : RoleBase<Mafioso>
    {
        public Mafioso()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Mafioso;
        }

        public override Color RoleColor
        {
            get => NameColor;
        }

        // write configs here

        public static bool CanSabotage
        {
            get => CanKill || CustomOptionHolder.MafiosoCanSabotage.GetBool();
        }

        public static bool CanRepair
        {
            get => CanKill || CustomOptionHolder.MafiosoCanRepair.GetBool();
        }

        public static bool CanVent
        {
            get => CanKill || CustomOptionHolder.MafiosoCanVent.GetBool();
        }

        public static bool CanKill
        {
            get => !Godfather.Exists || IsGodfatherDead;
        }

        public override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaM)})" : "";
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void OnIntroEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer = null)
        {
            IsMafiosoDead = true;
        }

        public override void OnFinishShipStatusBegin() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class Janitor : RoleBase<Janitor>
    {
        // write configs here
        private static CustomButton _janitorCleanButton;

        public Janitor()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Janitor;
        }

        public override Color RoleColor
        {
            get => NameColor;
        }

        public static float Cooldown
        {
            get => CustomOptionHolder.JanitorCooldown.GetFloat();
        }

        public static bool CanSabotage
        {
            get => CustomOptionHolder.JanitorCanSabotage.GetBool();
        }

        public static bool CanRepair
        {
            get => CustomOptionHolder.JanitorCanRepair.GetBool();
        }

        public static bool CanVent
        {
            get => CustomOptionHolder.JanitorCanVent.GetBool();
        }

        public override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaJ)})" : "";
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void OnIntroEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }

        public override void OnDeath(PlayerControl killer = null)
        {
            IsJanitorDead = true;
        }

        public override void OnFinishShipStatusBegin() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        public static void MakeButtons(HudManager hm)
        {
            _janitorCleanButton = new(() =>
            {
                var bodies = Object.FindObjectsOfType<DeadBody>();
                var local = PlayerControl.LocalPlayer;
                var truePosition = local.GetTruePosition();
                var maxDist = local.MaxReportDistance;

                for (var i = 0; i < bodies.Count; i++)
                {
                    var body = bodies[i];
                    if (body == null || body.Reported) continue;

                    var bodyPos = body.TruePosition;
                    if (Vector2.Distance(bodyPos, truePosition) <= maxDist && local.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                    {
                        var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                        if (playerInfo != null)
                        {
                            using var sender = new RPCSender(local.NetId, CustomRPC.CleanBody);
                            sender.Write(playerInfo.PlayerId);
                            RPCProcedure.CleanBody(playerInfo.PlayerId);

                            _janitorCleanButton.Timer = _janitorCleanButton.MaxTimer;
                            break;
                        }
                    }
                }
            }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Janitor) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; }, () => { _janitorCleanButton.Timer = _janitorCleanButton.MaxTimer; }, AssetLoader.CleanButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.CleanText));
        }

        public static void SetButtonCooldowns()
        {
            _janitorCleanButton.MaxTimer = Cooldown;
        }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}
