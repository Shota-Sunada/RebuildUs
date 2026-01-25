namespace RebuildUs;

public static class CustomOptionHolder
{
    public static readonly string[] RATES = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    public static readonly string[] PRESETS = [Tr.Get("Option.Preset1"), Tr.Get("Option.Preset2"), Tr.Get("Option.Preset3"), Tr.Get("Option.Preset4"), Tr.Get("Option.Preset5")];

    #region MOD OPTIONS
    public static CustomOption PresetSelection;
    public static CustomOption ActivateRoles;
    #endregion

    #region GENERAL OPTIONS
    public static CustomOption CrewmateRolesCountMin;
    public static CustomOption CrewmateRolesCountMax;
    public static CustomOption ImpostorRolesCountMin;
    public static CustomOption ImpostorRolesCountMax;
    public static CustomOption NeutralRolesCountMin;
    public static CustomOption NeutralRolesCountMax;
    public static CustomOption ModifiersCountMin;
    public static CustomOption ModifiersCountMax;
    #endregion

    #region GAME OPTIONS
    public static CustomOption GameOptions;
    public static CustomOption MaxNumberOfMeetings;
    public static CustomOption BlockSkippingInEmergencyMeetings;
    public static CustomOption NoVoteIsSelfVote;
    public static CustomOption HidePlayerNames;
    public static CustomOption AllowParallelMedBayScans;
    public static CustomOption HideOutOfSightNametags;
    public static CustomOption RefundVotesOnDeath;
    public static CustomOption DelayBeforeMeeting;
    public static CustomOption DisableVentAnimation;
    public static CustomOption StopCooldownOnFixingElecSabotage;
    public static CustomOption EnableHawkMode;
    public static CustomOption CanWinByTaskWithoutLivingPlayer;
    // public static CustomOption DeadPlayerCanSeeCooldown;
    public static CustomOption ImpostorCanIgnoreCommSabotage;
    // public static CustomOption BlockSabotageFromDeadImpostors;
    // public static CustomOption ShieldFirstKill;

    public static CustomOption RandomWireTask;
    public static CustomOption NumWireTask;

    public static CustomOption AdditionalEmergencyCooldown;
    public static CustomOption AdditionalEmergencyCooldownTime;

    public static CustomOption RestrictDevices;
    public static CustomOption RestrictAdmin;
    public static CustomOption RestrictAdminTime;
    public static CustomOption RestrictAdminText;
    public static CustomOption RestrictCameras;
    public static CustomOption RestrictCamerasTime;
    public static CustomOption RestrictCamerasText;
    public static CustomOption RestrictVitals;
    public static CustomOption RestrictVitalsTime;
    public static CustomOption RestrictVitalsText;
    #endregion

    #region DISCORD OPTIONS
    public static CustomOption EnableDiscordOptions;
    public static CustomOption EnableDiscordAutoMute;
    public static CustomOption EnableSendFinalStatusToDiscord;
    #endregion

    #region POLUS OPTIONS
    public static CustomOption PolusAdditionalVents;
    public static CustomOption PolusSpecimenVital;
    public static CustomOption PolusRandomSpawn;
    #endregion

    #region AIRSHIP OPTIONS
    public static CustomOption AirshipOptimize;
    public static CustomOption AirshipEnableWallCheck;
    public static CustomOption AirshipReactorDuration;
    public static CustomOption AirshipRandomSpawn;
    public static CustomOption AirshipAdditionalSpawn;
    public static CustomOption AirshipSynchronizedSpawning;
    public static CustomOption AirshipSetOriginalCooldown;
    public static CustomOption AirshipInitialDoorCooldown;
    public static CustomOption AirshipInitialSabotageCooldown;
    public static CustomOption AirshipOldAdmin;
    public static CustomOption AirshipRestrictedAdmin;
    public static CustomOption AirshipDisableGapSwitchBoard;
    public static CustomOption AirshipDisableMovingPlatform;
    public static CustomOption AirshipAdditionalLadder;
    public static CustomOption AirshipOneWayLadder;
    public static CustomOption AirshipReplaceSafeTask;
    public static CustomOption AirshipAdditionalWireTask;
    #endregion

    #region MAP OPTIONS
    public static CustomOption RandomMap;
    public static CustomOption RandomMapEnableSkeld;
    public static CustomOption RandomMapEnableMiraHQ;
    public static CustomOption RandomMapEnablePolus;
    public static CustomOption RandomMapEnableDleks;
    public static CustomOption RandomMapEnableAirShip;
    public static CustomOption RandomMapEnableFungle;
    public static CustomOption RandomMapEnableSubmerged;
    #endregion

    #region ROLES CREWMATE
    public static CustomRoleOption MayorSpawnRate;
    public static CustomOption MayorNumVotes;
    public static CustomOption MayorCanSeeVoteColors;
    public static CustomOption MayorTasksNeededToSeeVoteColors;
    public static CustomOption MayorMeetingButton;
    public static CustomOption MayorMaxRemoteMeetings;

    public static CustomRoleOption EngineerSpawnRate;
    public static CustomOption EngineerNumberOfFixes;
    public static CustomOption EngineerHighlightForImpostors;
    public static CustomOption EngineerHighlightForTeamJackal;

    public static CustomRoleOption SpySpawnRate;
    public static CustomOption SpyCanDieToSheriff;
    public static CustomOption SpyImpostorsCanKillAnyone;
    public static CustomOption SpyCanEnterVents;
    public static CustomOption SpyHasImpostorVision;

    public static CustomRoleOption MedicSpawnRate;
    public static CustomOption MedicShowShielded;
    public static CustomOption MedicShowAttemptToShielded;
    public static CustomOption MedicSetShieldAfterMeeting;
    public static CustomOption MedicShowAttemptToMedic;

    public static CustomRoleOption SeerSpawnRate;
    public static CustomOption SeerMode;
    public static CustomOption SeerSoulDuration;
    public static CustomOption SeerLimitSoulDuration;

    public static CustomRoleOption TimeMasterSpawnRate;
    public static CustomOption TimeMasterCooldown;
    public static CustomOption TimeMasterRewindTime;
    public static CustomOption TimeMasterShieldDuration;

    public static CustomRoleOption DetectiveSpawnRate;
    public static CustomOption DetectiveAnonymousFootprints;
    public static CustomOption DetectiveFootprintInterval;
    public static CustomOption DetectiveFootprintDuration;
    public static CustomOption DetectiveReportNameDuration;
    public static CustomOption DetectiveReportColorDuration;

    public static CustomRoleOption MediumSpawnRate;
    public static CustomOption MediumCooldown;
    public static CustomOption MediumDuration;
    public static CustomOption MediumOneTimeUse;

    public static CustomRoleOption HackerSpawnRate;
    public static CustomOption HackerCooldown;
    public static CustomOption HackerHackingDuration;
    public static CustomOption HackerOnlyColorType;
    public static CustomOption HackerToolsNumber;
    public static CustomOption HackerRechargeTasksNumber;
    public static CustomOption HackerNoMove;

    public static CustomRoleOption TrackerSpawnRate;
    public static CustomOption TrackerUpdateInterval;
    public static CustomOption TrackerResetTargetAfterMeeting;
    public static CustomOption TrackerCanTrackCorpses;
    public static CustomOption TrackerCorpsesTrackingCooldown;
    public static CustomOption TrackerCorpsesTrackingDuration;

    public static CustomRoleOption SnitchSpawnRate;
    public static CustomOption SnitchLeftTasksForReveal;
    public static CustomOption SnitchIncludeTeamJackal;
    public static CustomOption SnitchTeamJackalUseDifferentArrowColor;

    public static CustomRoleOption LighterSpawnRate;
    public static CustomOption LighterModeLightsOnVision;
    public static CustomOption LighterModeLightsOffVision;
    public static CustomOption LighterCooldown;
    public static CustomOption LighterDuration;
    public static CustomOption LighterCanSeeNinja;

    public static CustomRoleOption SecurityGuardSpawnRate;
    public static CustomOption SecurityGuardCooldown;
    public static CustomOption SecurityGuardTotalScrews;
    public static CustomOption SecurityGuardCamPrice;
    public static CustomOption SecurityGuardVentPrice;
    public static CustomOption SecurityGuardCamDuration;
    public static CustomOption SecurityGuardCamMaxCharges;
    public static CustomOption SecurityGuardCamRechargeTasksNumber;
    public static CustomOption SecurityGuardNoMove;

    public static CustomRoleOption SwapperSpawnRate;
    public static CustomOption SwapperIsImpRate;
    public static CustomOption SwapperCanCallEmergency;
    public static CustomOption SwapperCanOnlySwapOthers;
    public static CustomOption SwapperNumSwaps;

    public static CustomRoleOption BaitSpawnRate;
    public static CustomOption BaitHighlightAllVents;
    public static CustomOption BaitReportDelay;
    public static CustomOption BaitShowKillFlash;

    public static CustomRoleOption ShifterSpawnRate;
    public static CustomOption ShifterIsNeutralRate;
    public static CustomOption ShifterShiftsModifiers;
    public static CustomOption ShifterPastShifters;

    public static CustomRoleOption SheriffSpawnRate;
    public static CustomOption SheriffCooldown;
    public static CustomOption SheriffNumShots;
    public static CustomOption SheriffCanKillNeutrals;
    public static CustomOption SheriffMisfireKillsTarget;
    public static CustomOption SheriffCanKillNoDeadBody;
    #endregion

    #region ROLES IMPOSTOR
    public static CustomRoleOption BountyHunterSpawnRate;
    public static CustomOption BountyHunterBountyDuration;
    public static CustomOption BountyHunterReducedCooldown;
    public static CustomOption BountyHunterPunishmentTime;
    public static CustomOption BountyHunterShowArrow;
    public static CustomOption BountyHunterArrowUpdateInterval;

    public static CustomRoleOption MafiaSpawnRate;
    public static CustomOption MafiosoCanSabotage;
    public static CustomOption MafiosoCanRepair;
    public static CustomOption MafiosoCanVent;
    public static CustomOption JanitorCooldown;
    public static CustomOption JanitorCanSabotage;
    public static CustomOption JanitorCanRepair;
    public static CustomOption JanitorCanVent;

    public static CustomRoleOption TricksterSpawnRate;
    public static CustomOption TricksterPlaceBoxCooldown;
    public static CustomOption TricksterLightsOutCooldown;
    public static CustomOption TricksterLightsOutDuration;

    public static CustomRoleOption EvilHackerSpawnRate;
    public static CustomOption EvilHackerCanHasBetterAdmin;
    public static CustomOption EvilHackerCanCreateMadmate;
    public static CustomOption EvilHackerCanCreateMadmateFromJackal;
    public static CustomOption EvilHackerCanMoveEvenIfUsesAdmin;
    public static CustomOption EvilHackerCanInheritAbility;
    public static CustomOption EvilHackerCanSeeDoorStatus;
    public static CustomOption CreatedMadmateCanDieToSheriff;
    public static CustomOption CreatedMadmateCanEnterVents;
    public static CustomOption CreatedMadmateHasImpostorVision;
    public static CustomOption CreatedMadmateCanSabotage;
    public static CustomOption CreatedMadmateCanFixComm;
    public static CustomOption CreatedMadmateAbility;
    public static CustomOption CreatedMadmateNumTasks;
    public static CustomOption CreatedMadmateExileCrewmate;

    public static CustomRoleOption EvilTrackerSpawnRate;
    public static CustomOption EvilTrackerCooldown;
    public static CustomOption EvilTrackerResetTargetAfterMeeting;
    public static CustomOption EvilTrackerCanSeeDeathFlash;
    public static CustomOption EvilTrackerCanSeeTargetTask;
    public static CustomOption EvilTrackerCanSeeTargetPosition;
    public static CustomOption EvilTrackerCanSetTargetOnMeeting;

    public static CustomRoleOption EraserSpawnRate;
    public static CustomOption EraserCooldown;
    public static CustomOption EraserCooldownIncrease;
    public static CustomOption EraserCanEraseAnyone;

    public static CustomRoleOption MorphingSpawnRate;
    public static CustomOption MorphingCooldown;
    public static CustomOption MorphingDuration;

    public static CustomRoleOption CamouflagerSpawnRate;
    public static CustomOption CamouflagerCooldown;
    public static CustomOption CamouflagerDuration;
    public static CustomOption CamouflagerRandomColors;

    public static CustomRoleOption CleanerSpawnRate;
    public static CustomOption CleanerCooldown;

    public static CustomRoleOption WarlockSpawnRate;
    public static CustomOption WarlockCooldown;
    public static CustomOption WarlockRootTime;

    public static CustomRoleOption WitchSpawnRate;
    public static CustomOption WitchCooldown;
    public static CustomOption WitchAdditionalCooldown;
    public static CustomOption WitchCanSpellAnyone;
    public static CustomOption WitchSpellCastingDuration;
    public static CustomOption WitchTriggerBothCooldowns;
    public static CustomOption WitchVoteSavesTargets;

    public static CustomRoleOption VampireSpawnRate;
    public static CustomOption VampireKillDelay;
    public static CustomOption VampireCooldown;
    public static CustomOption VampireCanKillNearGarlics;
    #endregion

    #region ROLES NEUTRAL
    public static CustomRoleOption JesterSpawnRate;
    public static CustomOption JesterCanCallEmergency;
    public static CustomOption JesterCanSabotage;
    public static CustomOption JesterHasImpostorVision;

    public static CustomRoleOption ArsonistSpawnRate;
    public static CustomOption ArsonistCooldown;
    public static CustomOption ArsonistDuration;
    public static CustomOption ArsonistCanBeLovers;

    public static CustomRoleOption VultureSpawnRate;
    public static CustomOption VultureCooldown;
    public static CustomOption VultureNumberToWin;
    public static CustomOption VultureCanUseVents;
    public static CustomOption VultureShowArrows;

    public static CustomRoleOption JackalSpawnRate;
    public static CustomOption JackalKillCooldown;
    public static CustomOption JackalCreateSidekickCooldown;
    public static CustomOption JackalCanSabotageLights;
    public static CustomOption JackalCanUseVents;
    public static CustomOption JackalCanCreateSidekick;
    public static CustomOption JackalHasImpostorVision;
    public static CustomOption SidekickPromotesToJackal;
    public static CustomOption SidekickCanKill;
    public static CustomOption SidekickCanUseVents;
    public static CustomOption SidekickCanSabotageLights;
    public static CustomOption SidekickHasImpostorVision;
    public static CustomOption JackalPromotedFromSidekickCanCreateSidekick;
    public static CustomOption JackalCanCreateSidekickFromImpostor;

    public static CustomRoleOption GuesserSpawnRate;
    public static CustomOption GuesserIsImpGuesserRate;
    public static CustomOption GuesserNumberOfShots;
    public static CustomOption GuesserOnlyAvailableRoles;
    public static CustomOption GuesserHasMultipleShotsPerMeeting;
    public static CustomOption GuesserShowInfoInGhostChat;
    public static CustomOption GuesserKillsThroughShield;
    public static CustomOption GuesserEvilCanKillSpy;
    public static CustomOption GuesserSpawnBothRate;
    #endregion

    #region MODIFIERS
    public static CustomModifierOption MadmateSpawnRate;
    public static CustomOption MadmateCanDieToSheriff;
    public static CustomOption MadmateCanEnterVents;
    public static CustomOption MadmateHasImpostorVision;
    public static CustomOption MadmateCanSabotage;
    public static CustomOption MadmateCanFixComm;
    public static CustomOption MadmateType;
    public static CustomRoleSelectionOption MadmateFixedRole;
    public static CustomOption MadmateAbility;
    public static CustomTasksOption MadmateTasks;
    public static CustomOption MadmateExilePlayer;

    public static CustomOption LastImpostorEnable;
    public static CustomOption LastImpostorNumKills;
    public static CustomOption LastImpostorFunctions;
    public static CustomOption LastImpostorResults;
    public static CustomOption LastImpostorNumShots;

    public static CustomRoleOption LoversSpawnRate;
    public static CustomOption LoversNumCouples;
    public static CustomOption LoversImpLoverRate;
    public static CustomOption LoversBothDie;
    public static CustomOption LoversCanHaveAnotherRole;
    public static CustomOption LoversSeparateTeam;
    public static CustomOption LoversTasksCount;
    public static CustomOption LoversEnableChat;

    public static CustomModifierOption MiniSpawnRate;
    public static CustomOption MiniGrowingUpDuration;

    public static CustomModifierOption AntiTeleportSpawnRate;
    #endregion

    internal static Dictionary<byte, byte[]> BlockedRolePairings = [];

    public static void Load()
    {
        #region MOD OPTIONS
        PresetSelection = CustomOption.Header(0, CustomOptionType.General, "Option.Preset", PRESETS, Tr.Get("Option.Preset"));
        ActivateRoles = CustomOption.Normal(1, CustomOptionType.General, "Option.ActivateRoles", true, null);
        #endregion

        #region GENERAL OPTIONS
        CrewmateRolesCountMin = CustomOption.Header(10, CustomOptionType.General, "Option.CrewmateRolesCountMin", 0f, 0f, 15f, 1f, Tr.Get("Option.RolesGeneral"));
        CrewmateRolesCountMax = CustomOption.Normal(11, CustomOptionType.General, "Option.CrewmateRolesCountMax", 0f, 0f, 15f, 1f);
        ImpostorRolesCountMin = CustomOption.Normal(12, CustomOptionType.General, "Option.ImpostorRolesCountMin", 0f, 0f, 15f, 1f);
        ImpostorRolesCountMax = CustomOption.Normal(13, CustomOptionType.General, "Option.ImpostorRolesCountMax", 0f, 0f, 15f, 1f);
        NeutralRolesCountMin = CustomOption.Normal(14, CustomOptionType.General, "Option.NeutralRolesCountMin", 0f, 0f, 15f, 1f);
        NeutralRolesCountMax = CustomOption.Normal(15, CustomOptionType.General, "Option.NeutralRolesCountMax", 0f, 0f, 15f, 1f);
        ModifiersCountMin = CustomOption.Normal(16, CustomOptionType.General, "Option.ModifiersCountMin", 0f, 0f, 15f, 1f);
        ModifiersCountMax = CustomOption.Normal(17, CustomOptionType.General, "Option.ModifiersCountMax", 0f, 0f, 15f, 1f);
        #endregion

        #region GAME OPTIONS
        GameOptions = CustomOption.Header(19, CustomOptionType.General, "Option.GameOptions", true, Tr.Get("Option.GameOptions"));
        MaxNumberOfMeetings = CustomOption.Normal(20, CustomOptionType.General, "Option.MaxNumberOfMeetings", 10, 0, 15, 1);
        BlockSkippingInEmergencyMeetings = CustomOption.Normal(21, CustomOptionType.General, "Option.BlockSkippingInEmergencyMeetings", false);
        NoVoteIsSelfVote = CustomOption.Normal(22, CustomOptionType.General, "Option.NoVoteIsSelfVote", false);
        HidePlayerNames = CustomOption.Normal(23, CustomOptionType.General, "Option.HidePlayerNames", false);
        AllowParallelMedBayScans = CustomOption.Normal(24, CustomOptionType.General, "Option.AllowParallelMedBayScans", false);
        HideOutOfSightNametags = CustomOption.Normal(25, CustomOptionType.General, "Option.HideOutOfSightNametags", true);
        RefundVotesOnDeath = CustomOption.Normal(26, CustomOptionType.General, "Option.RefundVotesOnDeath", true);
        DelayBeforeMeeting = CustomOption.Normal(27, CustomOptionType.General, "Option.DelayBeforeMeeting", 0f, 0f, 10f, 0.25f);
        DisableVentAnimation = CustomOption.Normal(28, CustomOptionType.General, "Option.DisableVentAnimation", false);
        StopCooldownOnFixingElecSabotage = CustomOption.Normal(29, CustomOptionType.General, "Option.StopCooldownOnFixingElecSabotage", true);
        EnableHawkMode = CustomOption.Normal(30, CustomOptionType.General, "Option.EnableHawkMode", true);
        CanWinByTaskWithoutLivingPlayer = CustomOption.Normal(31, CustomOptionType.General, "Option.CanWinByTaskLivingPlayer", true);
        // DeadPlayerCanSeeCooldown = CustomOption.Normal(32, CustomOptionType.General, "DeadPlayerCanSeeCooldown", true);
        ImpostorCanIgnoreCommSabotage = CustomOption.Normal(33, CustomOptionType.General, "Option.ImpostorCanIgnoreCommSabotage", false);
        // BlockSabotageFromDeadImpostors = CustomOption.Normal(34, CustomOptionType.General, "BlockSabotageFromDeadImpostors", false);
        // ShieldFirstKill = CustomOption.Normal(35, CustomOptionType.General, "ShieldFirstKill", false);

        RandomWireTask = CustomOption.Normal(50, CustomOptionType.General, "Option.RandomWireTask", false);
        NumWireTask = CustomOption.Normal(51, CustomOptionType.General, "Option.NumWireTask", 3f, 1f, 10f, 1f, RandomWireTask);

        AdditionalEmergencyCooldown = CustomOption.Normal(55, CustomOptionType.General, "Option.AdditionalEmergencyCooldown", 0f, 0f, 15f, 1f);
        AdditionalEmergencyCooldownTime = CustomOption.Normal(56, CustomOptionType.General, "Option.AdditionalEmergencyCooldownTime", 10f, 0f, 60f, 1f, AdditionalEmergencyCooldown);

        RestrictDevices = CustomOption.Normal(60, CustomOptionType.General, "Option.RestrictDevices", [Tr.Get("Option.Off"), Tr.Get("Option.RestrictPerTurn"), Tr.Get("Option.RestrictPerGame")]);
        RestrictAdmin = CustomOption.Normal(61, CustomOptionType.General, "Option.RestrictAdmin", true, RestrictDevices);
        RestrictAdminTime = CustomOption.Normal(62, CustomOptionType.General, "Option.RestrictAdminTime", 30f, 0f, 600f, 1f, RestrictAdmin);
        RestrictAdminText = CustomOption.Normal(63, CustomOptionType.General, "Option.RestrictAdminText", true, RestrictAdmin);
        RestrictCameras = CustomOption.Normal(64, CustomOptionType.General, "Option.RestrictCameras", true, RestrictDevices);
        RestrictCamerasTime = CustomOption.Normal(65, CustomOptionType.General, "Option.RestrictCamerasTime", 30f, 0f, 600f, 1f, RestrictCameras);
        RestrictCamerasText = CustomOption.Normal(66, CustomOptionType.General, "Option.RestrictCamerasText", true, RestrictCameras);
        RestrictVitals = CustomOption.Normal(67, CustomOptionType.General, "Option.RestrictVitals", true, RestrictDevices);
        RestrictVitalsTime = CustomOption.Normal(68, CustomOptionType.General, "Option.RestrictVitalsTime", 30f, 0f, 600f, 1f, RestrictVitals);
        RestrictVitalsText = CustomOption.Normal(69, CustomOptionType.General, "Option.RestrictVitalsText", true, RestrictVitals);
        #endregion

        #region DISCORD OPTIONS
        EnableDiscordAutoMute = CustomOption.Header(110, CustomOptionType.General, "Option.EnableDiscordAutoMute", true, Tr.Get("Option.DiscordOptions"));
        EnableSendFinalStatusToDiscord = CustomOption.Normal(111, CustomOptionType.General, "Option.EnableSendFinalStatusToDiscord", true);
        #endregion

        #region POLUS OPTIONS
        PolusAdditionalVents = CustomOption.Header(70, CustomOptionType.General, "Option.PolusAdditionalVents", true, Tr.Get("Option.PolusOptions"));
        PolusSpecimenVital = CustomOption.Normal(71, CustomOptionType.General, "Option.PolusSpecimenVital", true);
        PolusRandomSpawn = CustomOption.Normal(72, CustomOptionType.General, "Option.PolusRandomSpawn", true);
        #endregion

        #region AIRSHIP OPTIONS
        AirshipOptimize = CustomOption.Header(80, CustomOptionType.General, "Option.AirshipOptimize", false, Tr.Get("Option.AirshipOptions"));
        AirshipEnableWallCheck = CustomOption.Normal(81, CustomOptionType.General, "Option.AirshipEnableWallCheck", true);
        AirshipReactorDuration = CustomOption.Normal(82, CustomOptionType.General, "Option.AirshipReactorDuration", 60f, 0f, 600f, 1f);
        AirshipRandomSpawn = CustomOption.Normal(83, CustomOptionType.General, "Option.AirshipRandomSpawn", false);
        AirshipAdditionalSpawn = CustomOption.Normal(84, CustomOptionType.General, "Option.AirshipAdditionalSpawn", true);
        AirshipSynchronizedSpawning = CustomOption.Normal(85, CustomOptionType.General, "Option.AirshipSynchronizedSpawning", true);
        AirshipSetOriginalCooldown = CustomOption.Normal(86, CustomOptionType.General, "Option.AirshipSetOriginalCooldown", false);
        AirshipInitialDoorCooldown = CustomOption.Normal(87, CustomOptionType.General, "Option.AirshipInitialDoorCooldown", 0f, 0f, 60f, 1f);
        AirshipInitialSabotageCooldown = CustomOption.Normal(88, CustomOptionType.General, "Option.AirshipInitialSabotageCooldown", 15f, 0f, 60f, 1f);
        AirshipOldAdmin = CustomOption.Normal(89, CustomOptionType.General, "Option.AirshipOldAdmin", false);
        AirshipRestrictedAdmin = CustomOption.Normal(90, CustomOptionType.General, "Option.AirshipRestrictedAdmin", false);
        AirshipDisableGapSwitchBoard = CustomOption.Normal(91, CustomOptionType.General, "Option.AirshipDisableGapSwitchBoard", false);
        AirshipDisableMovingPlatform = CustomOption.Normal(92, CustomOptionType.General, "Option.AirshipDisableMovingPlatform", false);
        AirshipAdditionalLadder = CustomOption.Normal(93, CustomOptionType.General, "Option.AirshipAdditionalLadder", false);
        AirshipOneWayLadder = CustomOption.Normal(94, CustomOptionType.General, "Option.AirshipOneWayLadder", false);
        AirshipReplaceSafeTask = CustomOption.Normal(95, CustomOptionType.General, "Option.AirshipReplaceSafeTask", false);
        AirshipAdditionalWireTask = CustomOption.Normal(96, CustomOptionType.General, "Option.AirshipAdditionalWireTask", false);
        #endregion

        #region MAP OPTIONS
        RandomMap = CustomOption.Header(100, CustomOptionType.General, "Option.RandomMap", false, Tr.Get("Option.RandomMap"));
        RandomMapEnableSkeld = CustomOption.Normal(101, CustomOptionType.General, "Option.RandomMapEnableSkeld", true, RandomMap);
        RandomMapEnableMiraHQ = CustomOption.Normal(102, CustomOptionType.General, "Option.RandomMapEnableMiraHQ", true, RandomMap);
        RandomMapEnablePolus = CustomOption.Normal(103, CustomOptionType.General, "Option.RandomMapEnablePolus", true, RandomMap);
        RandomMapEnableDleks = CustomOption.Normal(104, CustomOptionType.General, "Option.RandomMapEnableDleks", true, RandomMap);
        RandomMapEnableAirShip = CustomOption.Normal(105, CustomOptionType.General, "Option.RandomMapEnableAirShip", true, RandomMap);
        RandomMapEnableFungle = CustomOption.Normal(106, CustomOptionType.General, "Option.RandomMapEnableFungle", true, RandomMap);
        RandomMapEnableSubmerged = CustomOption.Normal(107, CustomOptionType.General, "Option.RandomMapEnableSubmerged", true, RandomMap);
        #endregion

        #region ROLES CREWMATE
        MayorSpawnRate = new(1000, CustomOptionType.Crewmate, RoleType.Mayor, Mayor.NameColor);
        MayorNumVotes = CustomOption.Normal(1001, CustomOptionType.Crewmate, "Option.MayorNumVotes", 2f, 2f, 10f, 1f, MayorSpawnRate);
        MayorCanSeeVoteColors = CustomOption.Normal(1002, CustomOptionType.Crewmate, "Option.MayorCanSeeVoteColors", false, MayorSpawnRate);
        MayorTasksNeededToSeeVoteColors = CustomOption.Normal(1003, CustomOptionType.Crewmate, "Option.MayorTasksNeededToSeeVoteColors", 3f, 1f, 10f, 1f, MayorCanSeeVoteColors);
        MayorMeetingButton = CustomOption.Normal(1004, CustomOptionType.Crewmate, "Option.MayorMeetingButton", true, MayorSpawnRate);
        MayorMaxRemoteMeetings = CustomOption.Normal(1005, CustomOptionType.Crewmate, "Option.MayorMaxRemoteMeetings", 1f, 0f, 10f, 1f, MayorMeetingButton);

        EngineerSpawnRate = new(1010, CustomOptionType.Crewmate, RoleType.Engineer, Engineer.NameColor);
        EngineerNumberOfFixes = CustomOption.Normal(1011, CustomOptionType.Crewmate, "Option.EngineerNumberOfFixes", 1f, 0f, 3f, 1f, EngineerSpawnRate);
        EngineerHighlightForImpostors = CustomOption.Normal(1012, CustomOptionType.Crewmate, "Option.EngineerHighlightForImpostors", true, EngineerSpawnRate);
        EngineerHighlightForTeamJackal = CustomOption.Normal(1013, CustomOptionType.Crewmate, "Option.EngineerHighlightForTeamJackal", true, EngineerSpawnRate);

        SpySpawnRate = new(1020, CustomOptionType.Crewmate, RoleType.Spy, Spy.NameColor, 1);
        SpyCanDieToSheriff = CustomOption.Normal(1021, CustomOptionType.Crewmate, "Option.SpyCanDieToSheriff", false, SpySpawnRate);
        SpyImpostorsCanKillAnyone = CustomOption.Normal(1022, CustomOptionType.Crewmate, "Option.SpyImpostorsCanKillAnyone", true, SpySpawnRate);
        SpyCanEnterVents = CustomOption.Normal(1023, CustomOptionType.Crewmate, "Option.SpyCanEnterVents", false, SpySpawnRate);
        SpyHasImpostorVision = CustomOption.Normal(1024, CustomOptionType.Crewmate, "Option.SpyHasImpostorVision", false, SpySpawnRate);

        MedicSpawnRate = new(1030, CustomOptionType.Crewmate, RoleType.Medic, Medic.NameColor, 1);
        MedicShowShielded = CustomOption.Normal(1031, CustomOptionType.Crewmate, "Option.MedicShowShielded", [Tr.Get("Option.MedicShowShieldedAll"), Tr.Get("Option.MedicShowShieldedBoth"), Tr.Get("Option.MedicShowShieldedMedic")], MedicSpawnRate);
        MedicShowAttemptToShielded = CustomOption.Normal(1032, CustomOptionType.Crewmate, "Option.MedicShowAttemptToShielded", false, MedicSpawnRate);
        MedicSetShieldAfterMeeting = CustomOption.Normal(1033, CustomOptionType.Crewmate, "Option.MedicSetShieldAfterMeeting", false, MedicSpawnRate);
        MedicShowAttemptToMedic = CustomOption.Normal(1034, CustomOptionType.Crewmate, "Option.MedicSeesMurderAttempt", false, MedicSpawnRate);

        SeerSpawnRate = new(1040, CustomOptionType.Crewmate, RoleType.Seer, Seer.NameColor, 1);
        SeerMode = CustomOption.Normal(1041, CustomOptionType.Crewmate, "Option.SeerMode", [Tr.Get("Option.SeerModeBoth"), Tr.Get("Option.SeerModeFlash"), Tr.Get("Option.SeerModeSouls")], SeerSpawnRate);
        SeerLimitSoulDuration = CustomOption.Normal(1042, CustomOptionType.Crewmate, "Option.SeerLimitSoulDuration", false, SeerSpawnRate);
        SeerSoulDuration = CustomOption.Normal(1043, CustomOptionType.Crewmate, "Option.SeerSoulDuration", 15f, 0f, 120f, 5f, SeerLimitSoulDuration);

        TimeMasterSpawnRate = new(1050, CustomOptionType.Crewmate, RoleType.TimeMaster, TimeMaster.NameColor, 1);
        TimeMasterCooldown = CustomOption.Normal(1051, CustomOptionType.Crewmate, "Option.TimeMasterCooldown", 30f, 2.5f, 120f, 2.5f, TimeMasterSpawnRate);
        TimeMasterRewindTime = CustomOption.Normal(1052, CustomOptionType.Crewmate, "Option.TimeMasterRewindTime", 3f, 1f, 10f, 1f, TimeMasterSpawnRate);
        TimeMasterShieldDuration = CustomOption.Normal(1053, CustomOptionType.Crewmate, "Option.TimeMasterShieldDuration", 3f, 1f, 20f, 1f, TimeMasterSpawnRate);

        DetectiveSpawnRate = new(1060, CustomOptionType.Crewmate, RoleType.Detective, Detective.NameColor, 1);
        DetectiveAnonymousFootprints = CustomOption.Normal(1061, CustomOptionType.Crewmate, "Option.DetectiveAnonymousFootprints", false, DetectiveSpawnRate);
        DetectiveFootprintInterval = CustomOption.Normal(1062, CustomOptionType.Crewmate, "Option.DetectiveFootprintInterval", 0.5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveFootprintDuration = CustomOption.Normal(1063, CustomOptionType.Crewmate, "Option.DetectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveReportNameDuration = CustomOption.Normal(1064, CustomOptionType.Crewmate, "Option.DetectiveReportNameDuration", 0, 0, 60, 2.5f, DetectiveSpawnRate);
        DetectiveReportColorDuration = CustomOption.Normal(1065, CustomOptionType.Crewmate, "Option.DetectiveReportColorDuration", 20, 0, 120, 2.5f, DetectiveSpawnRate);

        MediumSpawnRate = new(1070, CustomOptionType.Crewmate, RoleType.Medium, Medium.NameColor, 1);
        MediumCooldown = CustomOption.Normal(1071, CustomOptionType.Crewmate, "Option.MediumCooldown", 30f, 5f, 120f, 5f, MediumSpawnRate);
        MediumDuration = CustomOption.Normal(1072, CustomOptionType.Crewmate, "Option.MediumDuration", 3f, 0f, 15f, 1f, MediumSpawnRate);
        MediumOneTimeUse = CustomOption.Normal(1073, CustomOptionType.Crewmate, "Option.MediumOneTimeUse", false, MediumSpawnRate);

        HackerSpawnRate = new(1080, CustomOptionType.Crewmate, RoleType.Hacker, Hacker.NameColor, 1);
        HackerCooldown = CustomOption.Normal(1081, CustomOptionType.Crewmate, "Option.HackerCooldown", 30f, 5f, 60f, 5f, HackerSpawnRate);
        HackerHackingDuration = CustomOption.Normal(1082, CustomOptionType.Crewmate, "Option.HackerHackingDuration", 10f, 2.5f, 60f, 2.5f, HackerSpawnRate);
        HackerOnlyColorType = CustomOption.Normal(1083, CustomOptionType.Crewmate, "Option.HackerOnlyColorType", false, HackerSpawnRate);
        HackerToolsNumber = CustomOption.Normal(1084, CustomOptionType.Crewmate, "Option.HackerToolsNumber", 5f, 1f, 30f, 1f, HackerSpawnRate);
        HackerRechargeTasksNumber = CustomOption.Normal(1085, CustomOptionType.Crewmate, "Option.HackerRechargeTasksNumber", 2f, 1f, 5f, 1f, HackerSpawnRate);
        HackerNoMove = CustomOption.Normal(1086, CustomOptionType.Crewmate, "Option.HackerNoMove", true, HackerSpawnRate);

        TrackerSpawnRate = new(1090, CustomOptionType.Crewmate, RoleType.Tracker, Tracker.NameColor, 1);
        TrackerUpdateInterval = CustomOption.Normal(1091, CustomOptionType.Crewmate, "Option.TrackerUpdateInterval", 5f, 1f, 30f, 1f, TrackerSpawnRate);
        TrackerResetTargetAfterMeeting = CustomOption.Normal(1092, CustomOptionType.Crewmate, "Option.TrackerResetTargetAfterMeeting", false, TrackerSpawnRate);
        TrackerCanTrackCorpses = CustomOption.Normal(1093, CustomOptionType.Crewmate, "Option.TrackerTrackCorpses", true, TrackerSpawnRate);
        TrackerCorpsesTrackingCooldown = CustomOption.Normal(1094, CustomOptionType.Crewmate, "Option.TrackerCorpseCooldown", 30f, 0f, 120f, 5f, TrackerCanTrackCorpses);
        TrackerCorpsesTrackingDuration = CustomOption.Normal(1095, CustomOptionType.Crewmate, "Option.TrackerCorpseDuration", 5f, 2.5f, 30f, 2.5f, TrackerCanTrackCorpses);

        SnitchSpawnRate = new(1100, CustomOptionType.Crewmate, RoleType.Snitch, Snitch.NameColor, 1);
        SnitchLeftTasksForReveal = CustomOption.Normal(1101, CustomOptionType.Crewmate, "Option.SnitchLeftTasksForReveal", 1f, 0f, 5f, 1f, SnitchSpawnRate);
        SnitchIncludeTeamJackal = CustomOption.Normal(1102, CustomOptionType.Crewmate, "Option.SnitchIncludeTeamJackal", false, SnitchSpawnRate);
        SnitchTeamJackalUseDifferentArrowColor = CustomOption.Normal(1103, CustomOptionType.Crewmate, "Option.SnitchTeamJackalUseDifferentArrowColor", true, SnitchIncludeTeamJackal);

        LighterSpawnRate = new(1110, CustomOptionType.Crewmate, RoleType.Lighter, Lighter.NameColor, 15);
        LighterModeLightsOnVision = CustomOption.Normal(1111, CustomOptionType.Crewmate, "Option.LighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterModeLightsOffVision = CustomOption.Normal(1112, CustomOptionType.Crewmate, "Option.LighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterCooldown = CustomOption.Normal(1113, CustomOptionType.Crewmate, "Option.LighterCooldown", 30f, 5f, 120f, 5f, LighterSpawnRate);
        LighterDuration = CustomOption.Normal(1114, CustomOptionType.Crewmate, "Option.LighterDuration", 5f, 2.5f, 60f, 2.5f, LighterSpawnRate);
        // lighterCanSeeNinja = CustomOption.Normal(1115, CustomOptionType.Crewmate, "lighterCanSeeNinja", true, lighterSpawnRate);

        SecurityGuardSpawnRate = new(1120, CustomOptionType.Crewmate, RoleType.SecurityGuard, SecurityGuard.NameColor, 1);
        SecurityGuardCooldown = CustomOption.Normal(1121, CustomOptionType.Crewmate, "Option.SecurityGuardCooldown", 30f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardTotalScrews = CustomOption.Normal(1122, CustomOptionType.Crewmate, "Option.SecurityGuardTotalScrews", 7f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamPrice = CustomOption.Normal(1123, CustomOptionType.Crewmate, "Option.SecurityGuardCamPrice", 2f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardVentPrice = CustomOption.Normal(1124, CustomOptionType.Crewmate, "Option.SecurityGuardVentPrice", 1f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamDuration = CustomOption.Normal(1125, CustomOptionType.Crewmate, "Option.SecurityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardCamMaxCharges = CustomOption.Normal(1126, CustomOptionType.Crewmate, "Option.SecurityGuardCamMaxCharges", 5f, 1f, 30f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamRechargeTasksNumber = CustomOption.Normal(1127, CustomOptionType.Crewmate, "Option.SecurityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, SecurityGuardSpawnRate);
        SecurityGuardNoMove = CustomOption.Normal(1128, CustomOptionType.Crewmate, "Option.SecurityGuardNoMove", true, SecurityGuardSpawnRate);

        SwapperSpawnRate = new(1130, CustomOptionType.Neutral, RoleType.Swapper, Swapper.NameColor, 1);
        SwapperIsImpRate = CustomOption.Normal(1131, CustomOptionType.Neutral, "Option.SwapperIsImpRate", RATES, SwapperSpawnRate);
        SwapperNumSwaps = CustomOption.Normal(1132, CustomOptionType.Neutral, "Option.SwapperNumSwaps", 2f, 1f, 15f, 1f, SwapperSpawnRate);
        SwapperCanCallEmergency = CustomOption.Normal(1133, CustomOptionType.Neutral, "Option.SwapperCanCallEmergency", false, SwapperSpawnRate);
        SwapperCanOnlySwapOthers = CustomOption.Normal(1134, CustomOptionType.Neutral, "Option.SwapperCanOnlySwapOthers", false, SwapperSpawnRate);

        BaitSpawnRate = new(1140, CustomOptionType.Crewmate, RoleType.Bait, Bait.NameColor, 1);
        BaitHighlightAllVents = CustomOption.Normal(1141, CustomOptionType.Crewmate, "Option.BaitHighlightAllVents", false, BaitSpawnRate);
        BaitReportDelay = CustomOption.Normal(1142, CustomOptionType.Crewmate, "Option.BaitReportDelay", 0f, 0f, 10f, 1f, BaitSpawnRate, format: "unitSeconds");
        BaitShowKillFlash = CustomOption.Normal(1143, CustomOptionType.Crewmate, "Option.BaitShowKillFlash", true, BaitSpawnRate);

        ShifterSpawnRate = new(1150, CustomOptionType.Neutral, RoleType.Shifter, Shifter.NameColor, 1);
        ShifterIsNeutralRate = CustomOption.Normal(1151, CustomOptionType.Neutral, "Option.ShifterIsNeutralRate", RATES, ShifterSpawnRate);
        ShifterShiftsModifiers = CustomOption.Normal(1152, CustomOptionType.Neutral, "Option.ShifterShiftsModifiers", false, ShifterSpawnRate);
        ShifterPastShifters = CustomOption.Normal(1153, CustomOptionType.Neutral, "Option.ShifterPastShifters", false, ShifterSpawnRate);

        SheriffSpawnRate = new(1160, CustomOptionType.Crewmate, RoleType.Sheriff, Sheriff.NameColor, 15);
        SheriffCooldown = CustomOption.Normal(1161, CustomOptionType.Crewmate, "Option.SheriffCooldown", 30f, 2.5f, 60f, 2.5f, SheriffSpawnRate, format: "unitSeconds");
        SheriffNumShots = CustomOption.Normal(1162, CustomOptionType.Crewmate, "Option.SheriffNumShots", 2f, 1f, 15f, 1f, SheriffSpawnRate, format: "unitShots");
        SheriffMisfireKillsTarget = CustomOption.Normal(1163, CustomOptionType.Crewmate, "Option.SheriffMisfireKillsTarget", false, SheriffSpawnRate);
        SheriffCanKillNoDeadBody = CustomOption.Normal(1164, CustomOptionType.Crewmate, "Option.SheriffCanKillNoDeadBody", true, SheriffSpawnRate);
        SheriffCanKillNeutrals = CustomOption.Normal(1165, CustomOptionType.Crewmate, "Option.SheriffCanKillNeutrals", false, SheriffSpawnRate);
        #endregion

        #region ROLES IMPOSTOR
        BountyHunterSpawnRate = new(2000, CustomOptionType.Impostor, RoleType.BountyHunter, BountyHunter.NameColor, 1);
        BountyHunterBountyDuration = CustomOption.Normal(2001, CustomOptionType.Impostor, "Option.BountyHunterBountyDuration", 60f, 10f, 180f, 10f, BountyHunterSpawnRate);
        BountyHunterReducedCooldown = CustomOption.Normal(20002, CustomOptionType.Impostor, "Option.BountyHunterReducedCooldown", 2.5f, 2.5f, 30f, 2.5f, BountyHunterSpawnRate);
        BountyHunterPunishmentTime = CustomOption.Normal(2003, CustomOptionType.Impostor, "Option.BountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, BountyHunterSpawnRate);
        BountyHunterShowArrow = CustomOption.Normal(2004, CustomOptionType.Impostor, "Option.BountyHunterShowArrow", true, BountyHunterSpawnRate);
        BountyHunterArrowUpdateInterval = CustomOption.Normal(2005, CustomOptionType.Impostor, "Option.BountyHunterArrowUpdateInterval", 15f, 2.5f, 60f, 2.5f, BountyHunterShowArrow);

        MafiaSpawnRate = new(2010, CustomOptionType.Impostor, $"Role.{nameof(Mafia)}", Mafia.NameColor, 1);
        MafiosoCanSabotage = CustomOption.Normal(2011, CustomOptionType.Impostor, "Option.MafiosoCanSabotage", false, MafiaSpawnRate);
        MafiosoCanRepair = CustomOption.Normal(2012, CustomOptionType.Impostor, "Option.MafiosoCanRepair", false, MafiaSpawnRate);
        MafiosoCanVent = CustomOption.Normal(2013, CustomOptionType.Impostor, "Option.MafiosoCanVent", false, MafiaSpawnRate);
        JanitorCooldown = CustomOption.Normal(2014, CustomOptionType.Impostor, "Option.JanitorCooldown", 30f, 2.5f, 60f, 2.5f, MafiaSpawnRate);
        JanitorCanSabotage = CustomOption.Normal(2015, CustomOptionType.Impostor, "Option.JanitorCanSabotage", false, MafiaSpawnRate);
        JanitorCanRepair = CustomOption.Normal(2016, CustomOptionType.Impostor, "Option.JanitorCanRepair", false, MafiaSpawnRate);
        JanitorCanVent = CustomOption.Normal(2017, CustomOptionType.Impostor, "Option.JanitorCanVent", false, MafiaSpawnRate);

        TricksterSpawnRate = new(2020, CustomOptionType.Impostor, RoleType.Trickster, Trickster.NameColor, 1);
        TricksterPlaceBoxCooldown = CustomOption.Normal(2021, CustomOptionType.Impostor, "Option.TricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, TricksterSpawnRate);
        TricksterLightsOutCooldown = CustomOption.Normal(2022, CustomOptionType.Impostor, "Option.TricksterLightsOutCooldown", 30f, 5f, 60f, 5f, TricksterSpawnRate);
        TricksterLightsOutDuration = CustomOption.Normal(2023, CustomOptionType.Impostor, "Option.TricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, TricksterSpawnRate);

        EvilHackerSpawnRate = new CustomRoleOption(2030, CustomOptionType.Impostor, RoleType.EvilHacker, EvilHacker.NameColor, 1);
        EvilHackerCanHasBetterAdmin = CustomOption.Normal(2031, CustomOptionType.Impostor, "Option.EvilHackerCanHasBetterAdmin", false, EvilHackerSpawnRate);
        EvilHackerCanMoveEvenIfUsesAdmin = CustomOption.Normal(2032, CustomOptionType.Impostor, "Option.EvilHackerCanMoveEvenIfUsesAdmin", true, EvilHackerSpawnRate);
        EvilHackerCanInheritAbility = CustomOption.Normal(2033, CustomOptionType.Impostor, "Option.EvilHackerCanInheritAbility", false, EvilHackerSpawnRate);
        EvilHackerCanSeeDoorStatus = CustomOption.Normal(2034, CustomOptionType.Impostor, "Option.EvilHackerCanSeeDoorStatus", true, EvilHackerSpawnRate);
        EvilHackerCanCreateMadmate = CustomOption.Normal(2035, CustomOptionType.Impostor, "Option.EvilHackerCanCreateMadmate", false, EvilHackerSpawnRate);
        CreatedMadmateCanDieToSheriff = CustomOption.Normal(2036, CustomOptionType.Impostor, "Option.CreatedMadmateCanDieToSheriff", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanEnterVents = CustomOption.Normal(2037, CustomOptionType.Impostor, "Option.CreatedMadmateCanEnterVents", false, EvilHackerCanCreateMadmate);
        EvilHackerCanCreateMadmateFromJackal = CustomOption.Normal(2038, CustomOptionType.Impostor, "Option.EvilHackerCanCreateMadmateFromJackal", false, EvilHackerCanCreateMadmate);
        CreatedMadmateHasImpostorVision = CustomOption.Normal(2039, CustomOptionType.Impostor, "Option.CreatedMadmateHasImpostorVision", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanSabotage = CustomOption.Normal(2040, CustomOptionType.Impostor, "Option.CreatedMadmateCanSabotage", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanFixComm = CustomOption.Normal(2041, CustomOptionType.Impostor, "Option.CreatedMadmateCanFixComm", true, EvilHackerCanCreateMadmate);
        CreatedMadmateAbility = CustomOption.Normal(2042, CustomOptionType.Impostor, "Option.MadmateAbility", [Tr.Get("Option.MadmateNone"), Tr.Get("Option.MadmateFanatic")], EvilHackerCanCreateMadmate);
        CreatedMadmateNumTasks = CustomOption.Normal(2043, CustomOptionType.Impostor, "Option.CreatedMadmateNumTasks", 4f, 1f, 20f, 1f, CreatedMadmateAbility);
        CreatedMadmateExileCrewmate = CustomOption.Normal(2044, CustomOptionType.Impostor, "Option.CreatedMadmateExileCrewmate", false, EvilHackerCanCreateMadmate);

        EvilTrackerSpawnRate = new(2050, CustomOptionType.Impostor, RoleType.EvilTracker, EvilTracker.NameColor, 3);
        EvilTrackerCooldown = CustomOption.Normal(2051, CustomOptionType.Impostor, "Option.EvilTrackerCooldown", 10f, 0f, 60f, 1f, EvilTrackerSpawnRate);
        EvilTrackerResetTargetAfterMeeting = CustomOption.Normal(2052, CustomOptionType.Impostor, "Option.EvilTrackerResetTargetAfterMeeting", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeDeathFlash = CustomOption.Normal(2053, CustomOptionType.Impostor, "Option.EvilTrackerCanSeeDeathFlash", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetTask = CustomOption.Normal(2054, CustomOptionType.Impostor, "Option.EvilTrackerCanSeeTargetTask", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetPosition = CustomOption.Normal(2055, CustomOptionType.Impostor, "Option.EvilTrackerCanSeeTargetPosition", true, EvilTrackerSpawnRate);
        EvilTrackerCanSetTargetOnMeeting = CustomOption.Normal(2056, CustomOptionType.Impostor, "Option.EvilTrackerCanSetTargetOnMeeting", true, EvilTrackerSpawnRate);

        EraserSpawnRate = new(2060, CustomOptionType.Impostor, RoleType.Eraser, Eraser.NameColor, 1);
        EraserCooldown = CustomOption.Normal(2061, CustomOptionType.Impostor, "Option.EraserCooldown", 30f, 5f, 120f, 5f, EraserSpawnRate, format: "unitSeconds");
        EraserCooldownIncrease = CustomOption.Normal(2062, CustomOptionType.Impostor, "Option.EraserCooldownIncrease", 10f, 0f, 120f, 2.5f, EraserSpawnRate, format: "unitSeconds");
        EraserCanEraseAnyone = CustomOption.Normal(2063, CustomOptionType.Impostor, "Option.EraserCanEraseAnyone", false, EraserSpawnRate);

        MorphingSpawnRate = new(2070, CustomOptionType.Impostor, RoleType.Morphing, Morphing.NameColor, 1);
        MorphingCooldown = CustomOption.Normal(2071, CustomOptionType.Impostor, "Option.MorphingCooldown", 30f, 2.5f, 60f, 2.5f, MorphingSpawnRate, format: "unitSeconds");
        MorphingDuration = CustomOption.Normal(2072, CustomOptionType.Impostor, "Option.MorphingDuration", 10f, 1f, 20f, 0.5f, MorphingSpawnRate, format: "unitSeconds");

        CamouflagerSpawnRate = new(2080, CustomOptionType.Impostor, RoleType.Camouflager, Camouflager.NameColor, 1);
        CamouflagerCooldown = CustomOption.Normal(2081, CustomOptionType.Impostor, "Option.CamouflagerCooldown", 30f, 2.5f, 60f, 2.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerDuration = CustomOption.Normal(2082, CustomOptionType.Impostor, "Option.CamouflagerDuration", 10f, 1f, 20f, 0.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerRandomColors = CustomOption.Normal(2083, CustomOptionType.Impostor, "Option.CamouflagerRandomColors", false, CamouflagerSpawnRate);

        CleanerSpawnRate = new(2090, CustomOptionType.Impostor, RoleType.Cleaner, Cleaner.NameColor, 1);
        CleanerCooldown = CustomOption.Normal(2091, CustomOptionType.Impostor, "Option.CleanerCooldown", 30f, 2.5f, 60f, 2.5f, CleanerSpawnRate, format: "unitSeconds");

        WarlockSpawnRate = new(2100, CustomOptionType.Impostor, RoleType.Warlock, Warlock.NameColor, 1);
        WarlockCooldown = CustomOption.Normal(2101, CustomOptionType.Impostor, "Option.WarlockCooldown", 30f, 2.5f, 60f, 2.5f, WarlockSpawnRate, format: "unitSeconds");
        WarlockRootTime = CustomOption.Normal(2102, CustomOptionType.Impostor, "Option.WarlockRootTime", 5f, 0f, 15f, 1f, WarlockSpawnRate, format: "unitSeconds");

        WitchSpawnRate = new(2110, CustomOptionType.Impostor, RoleType.Witch, Witch.NameColor, 1);
        WitchCooldown = CustomOption.Normal(2111, CustomOptionType.Impostor, "Option.WitchSpellCooldown", 30f, 2.5f, 120f, 2.5f, WitchSpawnRate, format: "unitSeconds");
        WitchAdditionalCooldown = CustomOption.Normal(2112, CustomOptionType.Impostor, "Option.WitchAdditionalCooldown", 10f, 0f, 60f, 5f, WitchSpawnRate, format: "unitSeconds");
        WitchCanSpellAnyone = CustomOption.Normal(2113, CustomOptionType.Impostor, "Option.WitchCanSpellAnyone", false, WitchSpawnRate);
        WitchSpellCastingDuration = CustomOption.Normal(2114, CustomOptionType.Impostor, "Option.WitchSpellDuration", 1f, 0f, 10f, 1f, WitchSpawnRate, format: "unitSeconds");
        WitchTriggerBothCooldowns = CustomOption.Normal(2115, CustomOptionType.Impostor, "Option.WitchTriggerBoth", true, WitchSpawnRate);
        WitchVoteSavesTargets = CustomOption.Normal(2116, CustomOptionType.Impostor, "Option.WitchSaveTargets", true, WitchSpawnRate);

        VampireSpawnRate = new(2120, CustomOptionType.Impostor, RoleType.Vampire, Vampire.NameColor, 1);
        VampireKillDelay = CustomOption.Normal(2121, CustomOptionType.Impostor, "Option.VampireKillDelay", 10f, 1f, 20f, 1f, VampireSpawnRate, format: "unitSeconds");
        VampireCooldown = CustomOption.Normal(2122, CustomOptionType.Impostor, "Option.VampireCooldown", 30f, 2.5f, 60f, 2.5f, VampireSpawnRate, format: "unitSeconds");
        VampireCanKillNearGarlics = CustomOption.Normal(2123, CustomOptionType.Impostor, "Option.VampireCanKillNearGarlics", true, VampireSpawnRate);
        #endregion

        #region ROLES NEUTRAL
        JesterSpawnRate = new(3000, CustomOptionType.Neutral, RoleType.Jester, Jester.NameColor, 1);
        JesterCanCallEmergency = CustomOption.Normal(3001, CustomOptionType.Neutral, "Option.JesterCanCallEmergency", true, JesterSpawnRate);
        JesterCanSabotage = CustomOption.Normal(3002, CustomOptionType.Neutral, "Option.JesterCanSabotage", true, JesterSpawnRate);
        JesterHasImpostorVision = CustomOption.Normal(3003, CustomOptionType.Neutral, "Option.JesterHasImpostorVision", false, JesterSpawnRate);

        ArsonistSpawnRate = new(3010, CustomOptionType.Neutral, RoleType.Arsonist, Arsonist.NameColor, 1);
        ArsonistCooldown = CustomOption.Normal(3011, CustomOptionType.Neutral, "Option.ArsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
        ArsonistDuration = CustomOption.Normal(3012, CustomOptionType.Neutral, "Option.ArsonistDuration", 3f, 0f, 10f, 1f, ArsonistSpawnRate);
        ArsonistCanBeLovers = CustomOption.Normal(3013, CustomOptionType.Neutral, "Option.ArsonistCanBeLovers", false, ArsonistSpawnRate);

        VultureSpawnRate = new(3020, CustomOptionType.Neutral, RoleType.Vulture, Vulture.NameColor, 1);
        VultureCooldown = CustomOption.Normal(3021, CustomOptionType.Neutral, "Option.VultureCooldown", 15f, 2.5f, 60f, 2.5f, VultureSpawnRate);
        VultureNumberToWin = CustomOption.Normal(3022, CustomOptionType.Neutral, "Option.VultureNumberToWin", 4f, 1f, 12f, 1f, VultureSpawnRate);
        VultureCanUseVents = CustomOption.Normal(3023, CustomOptionType.Neutral, "Option.VultureCanUseVents", true, VultureSpawnRate);
        VultureShowArrows = CustomOption.Normal(3024, CustomOptionType.Neutral, "Option.VultureShowArrows", true, VultureSpawnRate);

        JackalSpawnRate = new(3030, CustomOptionType.Neutral, RoleType.Jackal, Jackal.NameColor, 1);
        JackalKillCooldown = CustomOption.Normal(3031, CustomOptionType.Neutral, "Option.JackalKillCooldown", 30f, 10f, 60f, 2.5f, JackalSpawnRate);
        JackalCanSabotageLights = CustomOption.Normal(3032, CustomOptionType.Neutral, "Option.JackalCanSabotageLights", true, JackalSpawnRate);
        JackalCanUseVents = CustomOption.Normal(3033, CustomOptionType.Neutral, "Option.JackalCanUseVents", true, JackalSpawnRate);
        JackalHasImpostorVision = CustomOption.Normal(3034, CustomOptionType.Neutral, "Option.JackalHasImpostorVision", false, JackalSpawnRate);
        JackalCanCreateSidekick = CustomOption.Normal(3035, CustomOptionType.Neutral, "Option.JackalCanCreateSidekick", false, JackalSpawnRate);
        JackalCreateSidekickCooldown = CustomOption.Normal(3036, CustomOptionType.Neutral, "Option.JackalCreateSidekickCooldown", 30f, 10f, 60f, 2.5f, JackalCanCreateSidekick);
        SidekickCanKill = CustomOption.Normal(3038, CustomOptionType.Neutral, "Option.SidekickCanKill", false, JackalCanCreateSidekick);
        SidekickCanUseVents = CustomOption.Normal(3039, CustomOptionType.Neutral, "Option.SidekickCanUseVents", true, JackalCanCreateSidekick);
        SidekickCanSabotageLights = CustomOption.Normal(3040, CustomOptionType.Neutral, "Option.SidekickCanSabotageLights", true, JackalCanCreateSidekick);
        SidekickHasImpostorVision = CustomOption.Normal(3041, CustomOptionType.Neutral, "Option.SidekickHasImpostorVision", false, JackalCanCreateSidekick);
        SidekickPromotesToJackal = CustomOption.Normal(3037, CustomOptionType.Neutral, "Option.SidekickPromotesToJackal", false, JackalCanCreateSidekick);
        JackalPromotedFromSidekickCanCreateSidekick = CustomOption.Normal(3042, CustomOptionType.Neutral, "Option.JackalPromotedFromSidekickCanCreateSidekick", false, SidekickPromotesToJackal);
        JackalCanCreateSidekickFromImpostor = CustomOption.Normal(3043, CustomOptionType.Neutral, "Option.JackalCanCreateSidekickFromImpostor", false, JackalCanCreateSidekick);

        GuesserSpawnRate = new(3050, CustomOptionType.Neutral, $"Role.{nameof(Guesser)}", Guesser.NiceGuesser.NameColor, 1);
        GuesserIsImpGuesserRate = CustomOption.Normal(3051, CustomOptionType.Neutral, "Option.GuesserIsImpGuesserRate", RATES, GuesserSpawnRate);
        GuesserSpawnBothRate = CustomOption.Normal(3052, CustomOptionType.Neutral, "Option.GuesserSpawnBothRate", RATES, GuesserSpawnRate);
        GuesserNumberOfShots = CustomOption.Normal(3053, CustomOptionType.Neutral, "Option.GuesserNumberOfShots", 2f, 1f, 15f, 1f, GuesserSpawnRate);
        GuesserOnlyAvailableRoles = CustomOption.Normal(3054, CustomOptionType.Neutral, "Option.GuesserOnlyAvailableRoles", true, GuesserSpawnRate);
        GuesserHasMultipleShotsPerMeeting = CustomOption.Normal(3055, CustomOptionType.Neutral, "Option.GuesserHasMultipleShotsPerMeeting", false, GuesserSpawnRate);
        GuesserShowInfoInGhostChat = CustomOption.Normal(3056, CustomOptionType.Neutral, "Option.GuesserToGhostChat", true, GuesserSpawnRate);
        GuesserKillsThroughShield = CustomOption.Normal(3057, CustomOptionType.Neutral, "Option.GuesserPierceShield", true, GuesserSpawnRate);
        GuesserEvilCanKillSpy = CustomOption.Normal(3058, CustomOptionType.Neutral, "Option.GuesserEvilCanKillSpy", true, GuesserSpawnRate);
        #endregion

        #region MODIFIERS
        MadmateSpawnRate = new(4000, CustomOptionType.Modifier, ModifierType.Madmate, Madmate.NameColor);
        MadmateType = CustomOption.Normal(4001, CustomOptionType.Modifier, "Option.MadmateType", [Tr.Get("Option.MadmateDefault"), Tr.Get("Option.MadmateWithRole"), Tr.Get("Option.MadmateRandom")], MadmateSpawnRate);
        MadmateFixedRole = new CustomRoleSelectionOption(4002, CustomOptionType.Modifier, "Option.MadmateFixedRole", Madmate.ValidRoles, MadmateType);
        MadmateAbility = CustomOption.Normal(4003, CustomOptionType.Modifier, "Option.MadmateAbility", [Tr.Get("Option.MadmateNone"), Tr.Get("Option.MadmateFanatic")], MadmateSpawnRate);
        MadmateTasks = new((4004, 4005, 4006), CustomOptionType.Modifier, (1, 1, 3), MadmateAbility);
        MadmateCanDieToSheriff = CustomOption.Normal(4007, CustomOptionType.Modifier, "Option.MadmateCanDieToSheriff", false, MadmateSpawnRate);
        MadmateCanEnterVents = CustomOption.Normal(4008, CustomOptionType.Modifier, "Option.MadmateCanEnterVents", false, MadmateSpawnRate);
        MadmateHasImpostorVision = CustomOption.Normal(4009, CustomOptionType.Modifier, "Option.MadmateHasImpostorVision", false, MadmateSpawnRate);
        MadmateCanSabotage = CustomOption.Normal(4010, CustomOptionType.Modifier, "Option.MadmateCanSabotage", false, MadmateSpawnRate);
        MadmateCanFixComm = CustomOption.Normal(4011, CustomOptionType.Modifier, "Option.MadmateCanFixComm", true, MadmateSpawnRate);
        MadmateExilePlayer = CustomOption.Normal(4012, CustomOptionType.Modifier, "Option.MadmateExileCrewmate", false, MadmateSpawnRate);

        LastImpostorEnable = CustomOption.Header(4010, CustomOptionType.Modifier, "Option.LastImpostorEnable", true, Helpers.Cs(LastImpostor.NameColor, Tr.Get("Modifier.LastImpostor")));
        LastImpostorFunctions = CustomOption.Normal(4011, CustomOptionType.Modifier, "Option.LastImpostorFunctions", [Tr.Get("Option.LastImpostorDivine"), Tr.Get("Option.LastImpostorGuesser")], LastImpostorEnable);
        LastImpostorNumKills = CustomOption.Normal(4012, CustomOptionType.Modifier, "Option.LastImpostorNumKills", 3f, 0f, 10f, 1f, LastImpostorEnable);
        LastImpostorResults = CustomOption.Normal(4013, CustomOptionType.Modifier, "Option.FortuneTellerResults", [Tr.Get("Option.FortuneTellerResultCrew"), Tr.Get("Option.FortuneTellerResultTeam"), Tr.Get("Option.FortuneTellerResultRole")], LastImpostorEnable);
        LastImpostorNumShots = CustomOption.Normal(4014, CustomOptionType.Modifier, "Option.LastImpostorNumShots", 1f, 1f, 15f, 1f, LastImpostorEnable);

        LoversSpawnRate = new(4020, CustomOptionType.Modifier, RoleType.Lovers, Lovers.Color, 1);
        LoversImpLoverRate = CustomOption.Normal(4021, CustomOptionType.Modifier, "Option.LoversImpLoverRate", RATES, LoversSpawnRate);
        LoversNumCouples = CustomOption.Normal(4022, CustomOptionType.Modifier, "Option.LoversNumCouples", 1f, 1f, 7f, 1f, LoversSpawnRate, format: "unitCouples");
        LoversBothDie = CustomOption.Normal(4023, CustomOptionType.Modifier, "Option.LoversBothDie", true, LoversSpawnRate);
        LoversCanHaveAnotherRole = CustomOption.Normal(4024, CustomOptionType.Modifier, "Option.LoversCanHaveAnotherRole", true, LoversSpawnRate);
        LoversSeparateTeam = CustomOption.Normal(4025, CustomOptionType.Modifier, "Option.LoversSeparateTeam", true, LoversSpawnRate);
        LoversTasksCount = CustomOption.Normal(4026, CustomOptionType.Modifier, "Option.LoversTasksCount", false, LoversSpawnRate);
        LoversEnableChat = CustomOption.Normal(4027, CustomOptionType.Modifier, "Option.LoversEnableChat", true, LoversSpawnRate);

        MiniSpawnRate = new(180, CustomOptionType.Modifier, ModifierType.Mini, Mini.NameColor, 15);
        MiniGrowingUpDuration = CustomOption.Normal(181, CustomOptionType.Modifier, "Option.MiniGrowingUpDuration", 400f, 100f, 1500f, 100f, MiniSpawnRate, format: "unitSeconds");

        AntiTeleportSpawnRate = new(4030, CustomOptionType.Modifier, ModifierType.AntiTeleport, AntiTeleport.NameColor, 15);

        #endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }
}