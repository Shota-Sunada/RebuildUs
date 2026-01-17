namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilHacker : RoleBase<EvilHacker>
{
    public static Color RoleColor = Palette.ImpostorRed;
    public PlayerControl currentTarget;
    public PlayerControl fakeMadmate;
    public bool canCreateMadmate = false;
    private static CustomButton evilHackerButton;
    private static CustomButton evilHackerCreatesMadmateButton;

    // write configs here

    public static bool canHasBetterAdmin { get { return CustomOptionHolder.evilHackerCanHasBetterAdmin.GetBool(); } }
    public static bool canMoveEvenIfUsesAdmin { get { return CustomOptionHolder.evilHackerCanMoveEvenIfUsesAdmin.GetBool(); } }
    public static bool canInheritAbility { get { return CustomOptionHolder.evilHackerCanInheritAbility.GetBool(); } }
    public static bool canSeeDoorStatus { get { return CustomOptionHolder.evilHackerCanSeeDoorStatus.GetBool(); } }
    public static bool createdMadmateCanDieToSheriff { get { return CustomOptionHolder.createdMadmateCanDieToSheriff.GetBool(); } }
    public static bool createdMadmateCanEnterVents { get { return CustomOptionHolder.createdMadmateCanEnterVents.GetBool(); } }
    public static bool canCreateMadmateFromJackal { get { return CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.GetBool(); } }
    public static bool createdMadmateHasImpostorVision { get { return CustomOptionHolder.createdMadmateHasImpostorVision.GetBool(); } }
    public static bool createdMadmateCanSabotage { get { return CustomOptionHolder.createdMadmateCanSabotage.GetBool(); } }
    public static bool createdMadmateCanFixComm { get { return CustomOptionHolder.createdMadmateCanFixComm.GetBool(); } }
    public static int createdMadmateAbility { get { return CustomOptionHolder.createdMadmateAbility.GetSelection(); } }
    public static float createdMadmateNumTasks { get { return CustomOptionHolder.createdMadmateNumTasks.GetFloat(); } }
    public static bool createdMadmateExileCrewmate { get { return CustomOptionHolder.createdMadmateExileCrewmate.GetBool(); } }

    public EvilHacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.EvilHacker;
        canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilHacker))
        {
            currentTarget = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(currentTarget, RoleColor);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        evilHackerButton = new CustomButton(
            () =>
            {
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt();
                Admin.isEvilHackerAdmin = true;
                FastDestroyableSingleton<MapBehaviour>.Instance.ShowCountOverlay(true, true, true);
            },
            () =>
            {
                return ((CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilHacker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive()) ||
                        (isInherited() && CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor())) && !RebuildUs.BetterSabotageMap.Value;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            EvilHacker.getButtonSprite(),
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

        evilHackerCreatesMadmateButton = new CustomButton(
            () =>
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EvilHackerCreatesMadmate, Hazel.SendOption.Reliable, -1);
                writer.Write(currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.evilHackerCreatesMadmate(currentTarget.PlayerId, Player.PlayerId);
            },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilHacker) && canCreateMadmate && CachedPlayer.LocalPlayer.PlayerControl.IsAlive();
            },
            () => { return currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { },
            EvilHacker.getMadmateButtonSprite(),
            new Vector3(-2.7f, -0.06f, 0),
            hm,
            hm.KillButton,
            null
        )
        {
            ButtonText = Tr.Get("MadmateText")
        };
    }
    public override void SetButtonCooldowns()
    {
        evilHackerButton.MaxTimer = 0f;
        evilHackerCreatesMadmateButton.MaxTimer = 0f;
    }

    // write functions here
    public static bool isInherited()
    {
        return canInheritAbility && Exists && LivingPlayers.Count == 0 && CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor();
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}