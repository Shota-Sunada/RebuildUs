namespace RebuildUs.Roles.Impostor;

internal static class Mafia
{
    public static Color Color = Palette.ImpostorRed;

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
    [RegisterRole(RoleType.Godfather, RoleTeam.Impostor, typeof(SingleRoleBase<Godfather>), nameof(CustomOptionHolder.MafiaSpawnRate))]
    internal class Godfather : SingleRoleBase<Godfather>
    {
        public static Color Color => Mafia.Color;

        public Godfather()
        {
            StaticRoleType = CurrentRoleType = RoleType.Godfather;
        }

        internal override string NameTag { get => PlayerControl.LocalPlayer.IsTeamImpostor() ? string.Format(" ({0})", Tr.Get(TrKey.MafiaG)) : ""; }

        [CustomEvent(CustomEventType.OnDeath)]
        internal void OnDeath(PlayerControl killer)
        {
            IsGodfatherDead = true;
        }

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }

    [HarmonyPatch]
    [RegisterRole(RoleType.Mafioso, RoleTeam.Impostor, typeof(SingleRoleBase<Mafioso>), nameof(CustomOptionHolder.MafiaSpawnRate))]
    internal class Mafioso : SingleRoleBase<Mafioso>
    {
        public static Color Color => Mafia.Color;

        public Mafioso()
        {
            StaticRoleType = CurrentRoleType = RoleType.Mafioso;
        }

        internal static bool CanSabotage { get => CanKill || CustomOptionHolder.MafiosoCanSabotage.GetBool(); }
        internal static bool CanRepair { get => CanKill || CustomOptionHolder.MafiosoCanRepair.GetBool(); }
        internal static bool CanVent { get => CanKill || CustomOptionHolder.MafiosoCanVent.GetBool(); }
        internal static bool CanKill { get => !Godfather.Exists || IsGodfatherDead; }
        internal override string NameTag { get => PlayerControl.LocalPlayer.IsTeamImpostor() ? string.Format(" ({0})", Tr.Get(TrKey.MafiaM)) : ""; }

        [CustomEvent(CustomEventType.OnDeath)]
        internal void OnDeath(PlayerControl killer)
        {
            IsMafiosoDead = true;
        }

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }

    [HarmonyPatch]
    [RegisterRole(RoleType.Janitor, RoleTeam.Impostor, typeof(SingleRoleBase<Janitor>), nameof(CustomOptionHolder.MafiaSpawnRate))]
    internal class Janitor : SingleRoleBase<Janitor>
    {
        public static Color Color => Mafia.Color;

        private static CustomButton JanitorCleanButton;

        public Janitor()
        {
            StaticRoleType = CurrentRoleType = RoleType.Janitor;
        }

        private static float Cooldown { get => CustomOptionHolder.JanitorCooldown.GetFloat(); }
        internal static bool CanSabotage { get => CustomOptionHolder.JanitorCanSabotage.GetBool(); }
        internal static bool CanRepair { get => CustomOptionHolder.JanitorCanRepair.GetBool(); }
        internal static bool CanVent { get => CustomOptionHolder.JanitorCanVent.GetBool(); }
        internal override string NameTag { get => PlayerControl.LocalPlayer.IsTeamImpostor() ? string.Format(" ({0})", Tr.Get(TrKey.MafiaJ)) : ""; }

        [CustomEvent(CustomEventType.OnDeath)]
        internal void OnDeath(PlayerControl killer)
        {
            IsJanitorDead = true;
        }

        [RegisterCustomButton]
        internal static void MakeButtons(HudManager hm)
        {
            JanitorCleanButton = new(
                nameof(JanitorCleanButton),
                () =>
                {
                    var bodies = UnityObject.FindObjectsOfType<DeadBody>();
                    var local = PlayerControl.LocalPlayer;
                    var truePosition = local.GetTruePosition();
                    var maxDist = local.MaxReportDistance;

                    foreach (var body in bodies)
                    {
                        if (body == null || body.Reported)
                        {
                            continue;
                        }

                        var bodyPos = body.TruePosition;
                        if (!(Vector2.Distance(bodyPos, truePosition) <= maxDist)
                            || !local.CanMove
                            || PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                        {
                            continue;
                        }
                        var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                        if (playerInfo == null)
                        {
                            continue;
                        }
                        using RPCSender sender = new(local.NetId, CustomRPC.CleanBody);
                        sender.Write(playerInfo.PlayerId);
                        RPCProcedure.CleanBody(playerInfo.PlayerId);

                        JanitorCleanButton.Timer = JanitorCleanButton.MaxTimer;
                        break;
                    }
                },
                () => PlayerControl.LocalPlayer.IsRole(RoleType.Janitor) && PlayerControl.LocalPlayer.IsAlive(),
                () => hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove,
                () =>
                {
                    JanitorCleanButton.Timer = JanitorCleanButton.MaxTimer;
                },
                AssetLoader.CleanButton,
                ButtonPosition.Layout,
                hm,
                hm.KillButton,
                AbilitySlot.ImpostorAbilityPrimary,
                false,
                Tr.Get(TrKey.CleanText));
        }

        [SetCustomButtonTimer]
        internal static void SetButtonCooldowns()
        {
            JanitorCleanButton.MaxTimer = Cooldown;
        }

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }
}