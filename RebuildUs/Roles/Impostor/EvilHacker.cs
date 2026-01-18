namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilHacker : RoleBase<EvilHacker>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public PlayerControl CurrentTarget;
    public PlayerControl FakeMadmate;
    public bool CanCreateMadmate = false;
    private static CustomButton EvilHackerButton;
    private static CustomButton EvilHackerCreatesMadmateButton;

    // write configs here

    public static bool CanHasBetterAdmin { get { return CustomOptionHolder.EvilHackerCanHasBetterAdmin.GetBool(); } }
    public static bool CanMoveEvenIfUsesAdmin { get { return CustomOptionHolder.EvilHackerCanMoveEvenIfUsesAdmin.GetBool(); } }
    public static bool CanInheritAbility { get { return CustomOptionHolder.EvilHackerCanInheritAbility.GetBool(); } }
    public static bool CanSeeDoorStatus { get { return CustomOptionHolder.EvilHackerCanSeeDoorStatus.GetBool(); } }
    public static bool CreatedMadmateCanDieToSheriff { get { return CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool(); } }
    public static bool CreatedMadmateCanEnterVents { get { return CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool(); } }
    public static bool CanCreateMadmateFromJackal { get { return CustomOptionHolder.EvilHackerCanCreateMadmateFromJackal.GetBool(); } }
    public static bool CreatedMadmateHasImpostorVision { get { return CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool(); } }
    public static bool CreatedMadmateCanSabotage { get { return CustomOptionHolder.CreatedMadmateCanSabotage.GetBool(); } }
    public static bool CreatedMadmateCanFixComm { get { return CustomOptionHolder.CreatedMadmateCanFixComm.GetBool(); } }
    public static int CreatedMadmateAbility { get { return CustomOptionHolder.CreatedMadmateAbility.GetSelection(); } }
    public static float CreatedMadmateNumTasks { get { return CustomOptionHolder.CreatedMadmateNumTasks.GetFloat(); } }
    public static bool CreatedMadmateExileCrewmate { get { return CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool(); } }

    public EvilHacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.EvilHacker;
        CanCreateMadmate = CustomOptionHolder.EvilHackerCanCreateMadmate.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {

        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker))
        {
            CurrentTarget = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        EvilHackerButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt();
                Admin.IsEvilHackerAdmin = true;
                FastDestroyableSingleton<MapBehaviour>.Instance.ShowCountOverlay(true, true, true);
            },
            () =>
            {
                return ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && PlayerControl.LocalPlayer.IsAlive()) ||
                        (IsInherited() && PlayerControl.LocalPlayer.IsTeamImpostor())) && !RebuildUs.BetterSabotageMap.Value;
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { },
            Hacker.GetAdminSprite(),
            new Vector3(0f, 2.0f, 0),
            hm,
            hm.KillButton,
            KeyCode.F,
            false,
            0f,
            () => { },
            Helpers.GetOption(ByteOptionNames.MapId) == 3,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
        );

        EvilHackerCreatesMadmateButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.EvilHackerCreatesMadmate);
                sender.Write(CurrentTarget.PlayerId);
                RPCProcedure.EvilHackerCreatesMadmate(CurrentTarget.PlayerId, Player.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && CanCreateMadmate && PlayerControl.LocalPlayer.IsAlive();
            },
            () => { return CurrentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { },
            AssetLoader.SidekickButton,
            new Vector3(-2.7f, -0.06f, 0),
            hm,
            hm.KillButton,
            null
        )
        {
            ButtonText = Tr.Get("Role.Madmate")
        };
    }
    public override void SetButtonCooldowns()
    {
        EvilHackerButton.MaxTimer = 0f;
        EvilHackerCreatesMadmateButton.MaxTimer = 0f;
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