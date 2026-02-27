namespace RebuildUs.Roles.Impostor;

internal static class Mafia
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static bool IsGodfatherDead;
    internal static bool IsMafiosoDead;
    internal static bool IsJanitorDead;

    internal static void ClearAndReload()
    {
        IsGodfatherDead = false;
        IsMafiosoDead = false;
        IsJanitorDead = false;
        Godfather.Clear();
        Mafioso.Clear();
        Janitor.Clear();
    }

    [HarmonyPatch]
    internal class Godfather : SingleRoleBase<Godfather>
    {
        public Godfather()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Godfather;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        internal override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaG)})" : "";
        }

        internal override void OnMeetingStart() { }
        internal override void OnMeetingEnd() { }
        internal override void OnIntroEnd() { }
        internal override void FixedUpdate() { }
        internal override void OnKill(PlayerControl target) { }

        internal override void OnDeath(PlayerControl killer = null)
        {
            IsGodfatherDead = true;
        }

        internal override void OnFinishShipStatusBegin() { }

        internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }

    [HarmonyPatch]
    internal class Mafioso : SingleRoleBase<Mafioso>
    {
        public Mafioso()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Mafioso;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        // write configs here

        internal static bool CanSabotage
        {
            get => CanKill || CustomOptionHolder.MafiosoCanSabotage.GetBool();
        }

        internal static bool CanRepair
        {
            get => CanKill || CustomOptionHolder.MafiosoCanRepair.GetBool();
        }

        internal static bool CanVent
        {
            get => CanKill || CustomOptionHolder.MafiosoCanVent.GetBool();
        }

        internal static bool CanKill
        {
            get => !Godfather.Exists || IsGodfatherDead;
        }

        internal override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaM)})" : "";
        }

        internal override void OnMeetingStart() { }
        internal override void OnMeetingEnd() { }
        internal override void OnIntroEnd() { }
        internal override void FixedUpdate() { }
        internal override void OnKill(PlayerControl target) { }

        internal override void OnDeath(PlayerControl killer = null)
        {
            IsMafiosoDead = true;
        }

        internal override void OnFinishShipStatusBegin() { }

        internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }

    [HarmonyPatch]
    internal class Janitor : SingleRoleBase<Janitor>
    {
        // write configs here
        private static CustomButton _janitorCleanButton;

        public Janitor()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.Janitor;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        private static float Cooldown
        {
            get => CustomOptionHolder.JanitorCooldown.GetFloat();
        }

        internal static bool CanSabotage
        {
            get => CustomOptionHolder.JanitorCanSabotage.GetBool();
        }

        internal static bool CanRepair
        {
            get => CustomOptionHolder.JanitorCanRepair.GetBool();
        }

        internal static bool CanVent
        {
            get => CustomOptionHolder.JanitorCanVent.GetBool();
        }

        internal override string NameTag
        {
            get => PlayerControl.LocalPlayer?.Data.Role.IsImpostor ?? false ? $" ({Tr.Get(TrKey.MafiaJ)})" : "";
        }

        internal override void OnMeetingStart() { }
        internal override void OnMeetingEnd() { }
        internal override void OnIntroEnd() { }
        internal override void FixedUpdate() { }
        internal override void OnKill(PlayerControl target) { }

        internal override void OnDeath(PlayerControl killer = null)
        {
            IsJanitorDead = true;
        }

        internal override void OnFinishShipStatusBegin() { }

        internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        internal static void MakeButtons(HudManager hm)
        {
            _janitorCleanButton = new(() =>
                {
                    Il2CppArrayBase<DeadBody> bodies = UnityObject.FindObjectsOfType<DeadBody>();
                    PlayerControl local = PlayerControl.LocalPlayer;
                    Vector2 truePosition = local.GetTruePosition();
                    float maxDist = local.MaxReportDistance;

                    foreach (DeadBody body in bodies)
                    {
                        if (body == null || body.Reported)
                        {
                            continue;
                        }

                        Vector2 bodyPos = body.TruePosition;
                        if (!(Vector2.Distance(bodyPos, truePosition) <= maxDist)
                            || !local.CanMove
                            || PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                        {
                            continue;
                        }
                        NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                        if (playerInfo == null)
                        {
                            continue;
                        }
                        using RPCSender sender = new(local.NetId, CustomRPC.CleanBody);
                        sender.Write(playerInfo.PlayerId);
                        RPCProcedure.CleanBody(playerInfo.PlayerId);

                        _janitorCleanButton.Timer = _janitorCleanButton.MaxTimer;
                        break;
                    }
                },
                () => PlayerControl.LocalPlayer.IsRole(RoleType.Janitor) && PlayerControl.LocalPlayer.IsAlive(),
                () => hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove,
                () =>
                {
                    _janitorCleanButton.Timer = _janitorCleanButton.MaxTimer;
                },
                AssetLoader.CleanButton,
                ButtonPosition.Layout,
                hm,
                hm.KillButton,
                AbilitySlot.ImpostorAbilityPrimary,
                false,
                Tr.Get(TrKey.CleanText));
        }

        internal static void SetButtonCooldowns()
        {
            _janitorCleanButton.MaxTimer = Cooldown;
        }

        // write functions here

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }
}