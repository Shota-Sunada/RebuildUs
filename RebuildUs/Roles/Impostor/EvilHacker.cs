namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class EvilHacker : RoleBase<EvilHacker>
{
    internal static Color NameColor = Palette.ImpostorRed;
    private static CustomButton _evilHackerButton;
    private static CustomButton _evilHackerCreatesMadmateButton;
    internal bool CanCreateMadmate;

    internal PlayerControl CurrentTarget;
    internal PlayerControl FakeMadmate;

    public EvilHacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.EvilHacker;
        CanCreateMadmate = CustomOptionHolder.EvilHackerCanCreateMadmate.GetBool();
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here

    internal static bool CanHasBetterAdmin { get => CustomOptionHolder.EvilHackerCanHasBetterAdmin.GetBool(); }
    internal static bool CanMoveEvenIfUsesAdmin { get => CustomOptionHolder.EvilHackerCanMoveEvenIfUsesAdmin.GetBool(); }
    internal static bool CanInheritAbility { get => CustomOptionHolder.EvilHackerCanInheritAbility.GetBool(); }
    internal static bool CanSeeDoorStatus { get => CustomOptionHolder.EvilHackerCanSeeDoorStatus.GetBool(); }
    internal static bool CreatedMadmateCanDieToSheriff { get => CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool(); }
    internal static bool CreatedMadmateCanEnterVents { get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool(); }
    internal static bool CanCreateMadmateFromJackal { get => CustomOptionHolder.EvilHackerCanCreateMadmateFromJackal.GetBool(); }
    internal static bool CreatedMadmateHasImpostorVision { get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool(); }
    internal static bool CreatedMadmateCanSabotage { get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool(); }
    internal static bool CreatedMadmateCanFixComm { get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool(); }
    internal static int CreatedMadmateAbility { get => CustomOptionHolder.CreatedMadmateAbility.GetSelection(); }
    internal static float CreatedMadmateNumTasks { get => CustomOptionHolder.CreatedMadmateNumTasks.GetFloat(); }
    internal static bool CreatedMadmateExileCrewmate { get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        EvilHacker local = Local;
        if (local != null)
        {
            CurrentTarget = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _evilHackerButton = new(() =>
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
            Admin.IsEvilHackerAdmin = true;
            HudManager.Instance.ToggleMapVisible(new()
            {
                Mode = MapOptions.Modes.CountOverlay,
                AllowMovementWhileMapOpen = CanMoveEvenIfUsesAdmin,
                ShowLivePlayerPosition = true,
                IncludeDeadBodies = true,
            });
        }, () =>
        {
            return ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && PlayerControl.LocalPlayer.IsAlive()) || (IsInherited() && PlayerControl.LocalPlayer.IsTeamImpostor())) && !RebuildUs.BetterSabotageMap.Value;
        }, () => { return PlayerControl.LocalPlayer.CanMove; }, () => { }, Hacker.GetAdminSprite(), ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, 0f, () => { }, Helpers.GetOption(ByteOptionNames.MapId) == 3, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin));

        _evilHackerCreatesMadmateButton = new(() =>
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.EvilHackerCreatesMadmate);
            sender.Write(Local.CurrentTarget.PlayerId);
            RPCProcedure.EvilHackerCreatesMadmate(Local.CurrentTarget.PlayerId, Local.Player.PlayerId);
        }, () =>
        {
            return PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && Local.CanCreateMadmate && PlayerControl.LocalPlayer.IsAlive();
        }, () => { return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove; }, () => { }, AssetLoader.SidekickButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilitySecondary, false, Tr.Get(TrKey.Madmate));
    }

    internal static void SetButtonCooldowns()
    {
        _evilHackerButton.MaxTimer = 0f;
        _evilHackerCreatesMadmateButton.MaxTimer = 0f;
    }

    // write functions here
    internal static bool IsInherited()
    {
        return CanInheritAbility && Exists && LivingPlayers.Count == 0 && PlayerControl.LocalPlayer.IsTeamImpostor();
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}