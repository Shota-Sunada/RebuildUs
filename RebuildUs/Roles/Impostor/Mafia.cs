namespace RebuildUs.Roles.Impostor;

public static class Mafia
{
    public static Color RoleColor = Palette.ImpostorRed;

    public static bool IsGodfatherDead = false;
    public static bool IsMafiosoDead = false;
    public static bool IsJanitorDead = false;

    public static void ClearAndReload()
    {
        IsGodfatherDead = false;
        IsMafiosoDead = false;
        IsJanitorDead = false;
    }

    [HarmonyPatch]
    public class Godfather : RoleBase<Godfather>
    {
        public Godfather()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Godfather;
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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

        // write functions here

        public override void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class Mafioso : RoleBase<Mafioso>
    {
        // write configs here

        public static bool canSabotage { get { return canKill || CustomOptionHolder.mafiosoCanSabotage.GetBool(); } }
        public static bool canRepair { get { return canKill || CustomOptionHolder.mafiosoCanRepair.GetBool(); } }
        public static bool canVent { get { return canKill || CustomOptionHolder.mafiosoCanVent.GetBool(); } }
        public static bool canKill { get { return !Godfather.Exists || IsGodfatherDead; } }

        public Mafioso()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Mafioso;
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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

        // write functions here

        public override void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class Janitor : RoleBase<Janitor>
    {
        // write configs here
        private static CustomButton janitorCleanButton;
        public static float cooldown { get { return CustomOptionHolder.janitorCooldown.GetFloat(); } }
        public static bool canSabotage { get { return CustomOptionHolder.janitorCanSabotage.GetBool(); } }
        public static bool canRepair { get { return CustomOptionHolder.janitorCanRepair.GetBool(); } }
        public static bool canVent { get { return CustomOptionHolder.janitorCanVent.GetBool(); } }

        public Janitor()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Janitor;
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
        public override void MakeButtons(HudManager hm)
        {
            janitorCleanButton = new CustomButton(
                () =>
                {
                    foreach (var collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
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
                                    janitorCleanButton.Timer = janitorCleanButton.MaxTimer;

                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Janitor) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return hm.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { janitorCleanButton.Timer = janitorCleanButton.MaxTimer; },
                Janitor.getButtonSprite(),
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
            janitorCleanButton.MaxTimer = Janitor.cooldown;
        }

        // write functions here

        public override void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}