namespace RebuildUs.Roles.Impostor;

public static class Mafia
{
    public static Color NameColor = Palette.ImpostorRed;

    public static bool IsGodfatherDead = false;
    public static bool IsMafiosoDead = false;
    public static bool IsJanitorDead = false;

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
        public override Color RoleColor => NameColor;

        public Godfather()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Godfather;
        }

        public override string NameTag => (PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false) ? $" ({Tr.Get("Role.MafiaG")})" : "";

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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

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
        public override Color RoleColor => NameColor;

        // write configs here

        public static bool CanSabotage { get { return CanKill || CustomOptionHolder.MafiosoCanSabotage.GetBool(); } }
        public static bool CanRepair { get { return CanKill || CustomOptionHolder.MafiosoCanRepair.GetBool(); } }
        public static bool CanVent { get { return CanKill || CustomOptionHolder.MafiosoCanVent.GetBool(); } }
        public static bool CanKill { get { return !Godfather.Exists || IsGodfatherDead; } }

        public Mafioso()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Mafioso;
        }

        public override string NameTag => (PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false) ? $" ({Tr.Get("Role.MafiaM")})" : "";

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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

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
        public override Color RoleColor => NameColor;

        // write configs here
        private static CustomButton JanitorCleanButton;
        public static float Cooldown { get { return CustomOptionHolder.JanitorCooldown.GetFloat(); } }
        public static bool CanSabotage { get { return CustomOptionHolder.JanitorCanSabotage.GetBool(); } }
        public static bool CanRepair { get { return CustomOptionHolder.JanitorCanRepair.GetBool(); } }
        public static bool CanVent { get { return CustomOptionHolder.JanitorCanVent.GetBool(); } }

        public Janitor()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Janitor;
        }

        public override string NameTag => (PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false) ? $" ({Tr.Get("Role.MafiaJ")})" : "";

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
        public override void MakeButtons(HudManager hm)
        {
            JanitorCleanButton = new CustomButton(
                () =>
                {
                    foreach (var collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
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
                                        RPCProcedure.CleanBody(playerInfo.PlayerId);
                                    }
                                    JanitorCleanButton.Timer = JanitorCleanButton.MaxTimer;

                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Janitor) && PlayerControl.LocalPlayer.IsAlive(); },
                () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove; },
                () => { JanitorCleanButton.Timer = JanitorCleanButton.MaxTimer; },
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
            JanitorCleanButton.MaxTimer = Janitor.Cooldown;
        }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}