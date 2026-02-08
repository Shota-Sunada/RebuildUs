namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilHacker : RoleBase<EvilHacker>
{
    public static Color NameColor = Palette.ImpostorRed;
    private static CustomButton _evilHackerButton;
    private static CustomButton _evilHackerCreatesMadmateButton;
    public bool CanCreateMadmate;

    public PlayerControl CurrentTarget;
    public PlayerControl FakeMadmate;

    public EvilHacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.EvilHacker;
        CanCreateMadmate = CustomOptionHolder.EvilHackerCanCreateMadmate.GetBool();
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here

    public static bool CanHasBetterAdmin
    {
        get => CustomOptionHolder.EvilHackerCanHasBetterAdmin.GetBool();
    }

    public static bool CanMoveEvenIfUsesAdmin
    {
        get => CustomOptionHolder.EvilHackerCanMoveEvenIfUsesAdmin.GetBool();
    }

    public static bool CanInheritAbility
    {
        get => CustomOptionHolder.EvilHackerCanInheritAbility.GetBool();
    }

    public static bool CanSeeDoorStatus
    {
        get => CustomOptionHolder.EvilHackerCanSeeDoorStatus.GetBool();
    }

    public static bool CreatedMadmateCanDieToSheriff
    {
        get => CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool();
    }

    public static bool CreatedMadmateCanEnterVents
    {
        get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool();
    }

    public static bool CanCreateMadmateFromJackal
    {
        get => CustomOptionHolder.EvilHackerCanCreateMadmateFromJackal.GetBool();
    }

    public static bool CreatedMadmateHasImpostorVision
    {
        get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool();
    }

    public static bool CreatedMadmateCanSabotage
    {
        get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool();
    }

    public static bool CreatedMadmateCanFixComm
    {
        get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool();
    }

    public static int CreatedMadmateAbility
    {
        get => CustomOptionHolder.CreatedMadmateAbility.GetSelection();
    }

    public static float CreatedMadmateNumTasks
    {
        get => CustomOptionHolder.CreatedMadmateNumTasks.GetFloat();
    }

    public static bool CreatedMadmateExileCrewmate
    {
        get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            CurrentTarget = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
    }

    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
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
        }, () => { return ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && PlayerControl.LocalPlayer.IsAlive()) || (IsInherited() && PlayerControl.LocalPlayer.IsTeamImpostor())) && !RebuildUs.BetterSabotageMap.Value; }, () => { return PlayerControl.LocalPlayer.CanMove; }, () => { }, Hacker.GetAdminSprite(), ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, 0f, () => { }, Helpers.GetOption(ByteOptionNames.MapId) == 3, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin));

        _evilHackerCreatesMadmateButton = new(() =>
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.EvilHackerCreatesMadmate);
            sender.Write(Local.CurrentTarget.PlayerId);
            RPCProcedure.EvilHackerCreatesMadmate(Local.CurrentTarget.PlayerId, Local.Player.PlayerId);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && Local.CanCreateMadmate && PlayerControl.LocalPlayer.IsAlive(); }, () => { return Local.CurrentTarget && PlayerControl.LocalPlayer.CanMove; }, () => { }, AssetLoader.SidekickButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilitySecondary, false, Tr.Get(TrKey.Madmate));
    }

    public static void SetButtonCooldowns()
    {
        _evilHackerButton.MaxTimer = 0f;
        _evilHackerCreatesMadmateButton.MaxTimer = 0f;
    }

    // write functions here
    public static bool IsInherited()
    {
        return CanInheritAbility && Exists && LivingPlayers.Count == 0 && PlayerControl.LocalPlayer.IsTeamImpostor();
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
