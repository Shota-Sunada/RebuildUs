namespace RebuildUs.Options;

public static partial class CustomOptionHolder
{
    public static readonly string[] RATES = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    public static readonly string[] PRESETS = ["Preset 1", "Preset 2", "Preset 3", "Preset 4", "Preset 5"];

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

    #region POLUS OPTIONS
    public static CustomOption PolusOptions;
    public static CustomOption PolusAdditionalVents;
    public static CustomOption PolisSpecimenVital;
    public static CustomOption PolusRandomSpawn;
    #endregion

    #region AIRSHIP OPTIONS
    public static CustomOption AirshipOptions;
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
        PresetSelection = CustomOption.Header(0, CustomOptionType.General, "Preset", PRESETS, "Preset");
        ActivateRoles = CustomOption.Normal(1, CustomOptionType.General, "ActivateRoles", true, null);
        #endregion

        #region GENERAL OPTIONS
        CrewmateRolesCountMin = CustomOption.Header(10, CustomOptionType.General, "CrewmateRolesCountMin", 0f, 0f, 15f, 1f, "RolesGeneral");
        CrewmateRolesCountMax = CustomOption.Normal(11, CustomOptionType.General, "CrewmateRolesCountMax", 0f, 0f, 15f, 1f);
        ImpostorRolesCountMin = CustomOption.Normal(12, CustomOptionType.General, "ImpostorRolesCountMin", 0f, 0f, 15f, 1f);
        ImpostorRolesCountMax = CustomOption.Normal(13, CustomOptionType.General, "ImpostorRolesCountMax", 0f, 0f, 15f, 1f);
        NeutralRolesCountMin = CustomOption.Normal(14, CustomOptionType.General, "NeutralRolesCountMin", 0f, 0f, 15f, 1f);
        NeutralRolesCountMax = CustomOption.Normal(15, CustomOptionType.General, "NeutralRolesCountMax", 0f, 0f, 15f, 1f);
        ModifiersCountMin = CustomOption.Normal(16, CustomOptionType.General, "ModifiersCountMin", 0f, 0f, 15f, 1f);
        ModifiersCountMax = CustomOption.Normal(17, CustomOptionType.General, "ModifiersCountMax", 0f, 0f, 15f, 1f);
        #endregion

        #region GAME OPTIONS
        MaxNumberOfMeetings = CustomOption.Header(20, CustomOptionType.General, "MaxNumberOfMeetings", 10, 0, 15, 1, "GameOptions");
        BlockSkippingInEmergencyMeetings = CustomOption.Normal(21, CustomOptionType.General, "BlockSkippingInEmergencyMeetings", false);
        NoVoteIsSelfVote = CustomOption.Normal(22, CustomOptionType.General, "NoVoteIsSelfVote", false);
        HidePlayerNames = CustomOption.Normal(23, CustomOptionType.General, "HidePlayerNames", false);
        AllowParallelMedBayScans = CustomOption.Normal(24, CustomOptionType.General, "AllowParallelMedBayScans", false);
        HideOutOfSightNametags = CustomOption.Normal(25, CustomOptionType.General, "HideOutOfSightNametags", true);
        RefundVotesOnDeath = CustomOption.Normal(26, CustomOptionType.General, "RefundVotesOnDeath", true);
        DelayBeforeMeeting = CustomOption.Normal(27, CustomOptionType.General, "DelayBeforeMeeting", 0f, 0f, 10f, 0.25f);
        DisableVentAnimation = CustomOption.Normal(28, CustomOptionType.General, "DisableVentAnimation", false);
        StopCooldownOnFixingElecSabotage = CustomOption.Normal(29, CustomOptionType.General, "StopCooldownOnFixingElecSabotage", true);
        EnableHawkMode = CustomOption.Normal(30, CustomOptionType.General, "EnableHawkMode", true);
        CanWinByTaskWithoutLivingPlayer = CustomOption.Normal(31, CustomOptionType.General, "CanWinByTaskLivingPlayer", true);
        // DeadPlayerCanSeeCooldown = CustomOption.Normal(32, CustomOptionType.General, "DeadPlayerCanSeeCooldown", true);
        ImpostorCanIgnoreCommSabotage = CustomOption.Normal(33, CustomOptionType.General, "ImpostorCanIgnoreCommSabotage", false);
        // BlockSabotageFromDeadImpostors = CustomOption.Normal(34, CustomOptionType.General, "BlockSabotageFromDeadImpostors", false);
        // ShieldFirstKill = CustomOption.Normal(35, CustomOptionType.General, "ShieldFirstKill", false);

        RandomWireTask = CustomOption.Normal(50, CustomOptionType.General, "RandomWireTask", false);
        NumWireTask = CustomOption.Normal(51, CustomOptionType.General, "NumWireTask", 3f, 1f, 10f, 1f, RandomWireTask);

        AdditionalEmergencyCooldown = CustomOption.Normal(55, CustomOptionType.General, "AdditionalEmergencyCooldown", 0f, 0f, 15f, 1f);
        AdditionalEmergencyCooldownTime = CustomOption.Normal(56, CustomOptionType.General, "AdditionalEmergencyCooldownTime", 10f, 0f, 60f, 1f, AdditionalEmergencyCooldown);

        RestrictDevices = CustomOption.Normal(60, CustomOptionType.General, "RestrictDevices", ["Off", "RestrictPerTurn", "RestrictPerGame"]);
        RestrictAdmin = CustomOption.Normal(61, CustomOptionType.General, "RestrictAdmin", true, RestrictDevices);
        RestrictAdminTime = CustomOption.Normal(62, CustomOptionType.General, "RestrictAdminTime", true, RestrictAdmin);
        RestrictAdminText = CustomOption.Normal(63, CustomOptionType.General, "RestrictAdminText", true, RestrictAdmin);
        RestrictCameras = CustomOption.Normal(64, CustomOptionType.General, "RestrictCameras", true, RestrictDevices);
        RestrictCamerasTime = CustomOption.Normal(65, CustomOptionType.General, "RestrictCamerasTime", true, RestrictCameras);
        RestrictCamerasText = CustomOption.Normal(66, CustomOptionType.General, "RestrictCamerasText", true, RestrictCameras);
        RestrictVitals = CustomOption.Normal(67, CustomOptionType.General, "RestrictVitals", true, RestrictDevices);
        RestrictVitalsTime = CustomOption.Normal(68, CustomOptionType.General, "RestrictVitalsTime", true, RestrictVitals);
        RestrictVitalsText = CustomOption.Normal(69, CustomOptionType.General, "RestrictVitalsText", true, RestrictVitals);
        #endregion

        #region POLUS OPTIONS
        PolusOptions = CustomOption.Header(70, CustomOptionType.General, "PolusOptions", false, "PolusOptions");
        PolusAdditionalVents = CustomOption.Normal(71, CustomOptionType.General, "PolusAdditionalVents", true, PolusOptions);
        PolisSpecimenVital = CustomOption.Normal(72, CustomOptionType.General, "PolisSpecimenVital", true, PolusOptions);
        PolusRandomSpawn = CustomOption.Normal(73, CustomOptionType.General, "PolusRandomSpawn", true, PolusOptions);
        #endregion

        #region AIRSHIP OPTIONS
        AirshipOptions = CustomOption.Header(80, CustomOptionType.General, "AirshipOptions", false, "AirshipOptions");
        AirshipEnableWallCheck = CustomOption.Normal(81, CustomOptionType.General, "AirshipEnableWallCheck", true, AirshipOptions);
        AirshipReactorDuration = CustomOption.Normal(82, CustomOptionType.General, "AirshipReactorDuration", 60f, 0f, 600f, 1f, AirshipOptions);
        AirshipRandomSpawn = CustomOption.Normal(83, CustomOptionType.General, "AirshipRandomSpawn", false, AirshipOptions);
        AirshipAdditionalSpawn = CustomOption.Normal(84, CustomOptionType.General, "AirshipAdditionalSpawn", true, AirshipOptions);
        AirshipSynchronizedSpawning = CustomOption.Normal(85, CustomOptionType.General, "AirshipSynchronizedSpawning", true, AirshipOptions);
        AirshipSetOriginalCooldown = CustomOption.Normal(86, CustomOptionType.General, "AirshipSetOriginalCooldown", false, AirshipOptions);
        AirshipInitialDoorCooldown = CustomOption.Normal(87, CustomOptionType.General, "AirshipInitialDoorCooldown", 0f, 0f, 60f, 1f, AirshipOptions);
        AirshipInitialSabotageCooldown = CustomOption.Normal(88, CustomOptionType.General, "AirshipInitialSabotageCooldown", 15f, 0f, 60f, 1f, AirshipOptions);
        AirshipOldAdmin = CustomOption.Normal(89, CustomOptionType.General, "AirshipOldAdmin", false, AirshipOptions);
        AirshipRestrictedAdmin = CustomOption.Normal(90, CustomOptionType.General, "AirshipRestrictedAdmin", false, AirshipOptions);
        AirshipDisableGapSwitchBoard = CustomOption.Normal(91, CustomOptionType.General, "AirshipDisableGapSwitchBoard", false, AirshipOptions);
        AirshipDisableMovingPlatform = CustomOption.Normal(92, CustomOptionType.General, "AirshipDisableMovingPlatform", false, AirshipOptions);
        AirshipAdditionalLadder = CustomOption.Normal(93, CustomOptionType.General, "AirshipAdditionalLadder", false, AirshipOptions);
        AirshipOneWayLadder = CustomOption.Normal(94, CustomOptionType.General, "AirshipOneWayLadder", false, AirshipOptions);
        AirshipReplaceSafeTask = CustomOption.Normal(95, CustomOptionType.General, "AirshipReplaceSafeTask", false, AirshipOptions);
        AirshipAdditionalWireTask = CustomOption.Normal(96, CustomOptionType.General, "AirshipAdditionalWireTask", false, AirshipOptions);
        #endregion

        #region MAP OPTIONS
        RandomMap = CustomOption.Header(100, CustomOptionType.General, "RandomMap", false, "RandomMap");
        RandomMapEnableSkeld = CustomOption.Normal(101, CustomOptionType.General, "RandomMapEnableSkeld", true, RandomMap);
        RandomMapEnableMiraHQ = CustomOption.Normal(102, CustomOptionType.General, "RandomMapEnableMiraHQ", true, RandomMap);
        RandomMapEnablePolus = CustomOption.Normal(103, CustomOptionType.General, "RandomMapEnablePolus", true, RandomMap);
        RandomMapEnableDleks = CustomOption.Normal(104, CustomOptionType.General, "RandomMapEnableDleks", true, RandomMap);
        RandomMapEnableAirShip = CustomOption.Normal(105, CustomOptionType.General, "RandomMapEnableAirShip", true, RandomMap);
        RandomMapEnableFungle = CustomOption.Normal(106, CustomOptionType.General, "RandomMapEnableFungle", true, RandomMap);
        RandomMapEnableSubmerged = CustomOption.Normal(107, CustomOptionType.General, "RandomMapEnableSubmerged", true, RandomMap);
        #endregion

        #region ROLES CREWMATE
        MayorSpawnRate = new(1000, CustomOptionType.Crewmate, RoleType.Mayor, Mayor.NameColor);
        MayorNumVotes = CustomOption.Normal(1001, CustomOptionType.Crewmate, "", 2f, 2f, 10f, 1f, MayorSpawnRate);
        MayorCanSeeVoteColors = CustomOption.Normal(1002, CustomOptionType.Crewmate, "", false, MayorSpawnRate);
        MayorTasksNeededToSeeVoteColors = CustomOption.Normal(1003, CustomOptionType.Crewmate, "", true, MayorCanSeeVoteColors);
        MayorMeetingButton = CustomOption.Normal(1004, CustomOptionType.Crewmate, "", true, MayorSpawnRate);
        MayorMaxRemoteMeetings = CustomOption.Normal(1005, CustomOptionType.Crewmate, "", 1f, 0f, 10f, 1f, MayorMeetingButton);

        EngineerSpawnRate = new(1010, CustomOptionType.Crewmate, RoleType.Engineer, Engineer.NameColor);
        EngineerNumberOfFixes = CustomOption.Normal(1011, CustomOptionType.Crewmate, "", 1f, 0f, 3f, 1f, EngineerSpawnRate);
        EngineerHighlightForImpostors = CustomOption.Normal(1012, CustomOptionType.Crewmate, "", true, EngineerSpawnRate);
        EngineerHighlightForTeamJackal = CustomOption.Normal(1013, CustomOptionType.Crewmate, "", true, EngineerSpawnRate);

        SpySpawnRate = new(1020, CustomOptionType.Crewmate, RoleType.Spy, Spy.NameColor, 1);
        SpyCanDieToSheriff = CustomOption.Normal(1021, CustomOptionType.Crewmate, "", false, SpySpawnRate);
        SpyImpostorsCanKillAnyone = CustomOption.Normal(1022, CustomOptionType.Crewmate, "", true, SpySpawnRate);
        SpyCanEnterVents = CustomOption.Normal(1023, CustomOptionType.Crewmate, "", false, SpySpawnRate);
        SpyHasImpostorVision = CustomOption.Normal(1024, CustomOptionType.Crewmate, "", false, SpySpawnRate);

        MedicSpawnRate = new(1030, CustomOptionType.Crewmate, RoleType.Medic, Medic.NameColor, 1);
        MedicShowShielded = CustomOption.Normal(1031, CustomOptionType.Crewmate, "medicShowShielded", ["medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic"], MedicSpawnRate);
        MedicShowAttemptToShielded = CustomOption.Normal(1032, CustomOptionType.Crewmate, "medicShowAttemptToShielded", false, MedicSpawnRate);
        MedicSetShieldAfterMeeting = CustomOption.Normal(1033, CustomOptionType.Crewmate, "medicSetShieldAfterMeeting", false, MedicSpawnRate);
        MedicShowAttemptToMedic = CustomOption.Normal(1034, CustomOptionType.Crewmate, "medicSeesMurderAttempt", false, MedicSpawnRate);

        SeerSpawnRate = new(1040, CustomOptionType.Crewmate, RoleType.Seer, Seer.NameColor, 1);
        SeerMode = CustomOption.Normal(1041, CustomOptionType.Crewmate, "seerMode", ["seerModeBoth", "seerModeFlash", "seerModeSouls"], SeerSpawnRate);
        SeerLimitSoulDuration = CustomOption.Normal(1042, CustomOptionType.Crewmate, "seerLimitSoulDuration", false, SeerSpawnRate);
        SeerSoulDuration = CustomOption.Normal(1043, CustomOptionType.Crewmate, "seerSoulDuration", 15f, 0f, 120f, 5f, SeerLimitSoulDuration);

        TimeMasterSpawnRate = new(1050, CustomOptionType.Crewmate, RoleType.TimeMaster, TimeMaster.NameColor, 1);
        TimeMasterCooldown = CustomOption.Normal(1051, CustomOptionType.Crewmate, "timeMasterCooldown", 30f, 2.5f, 120f, 2.5f, TimeMasterSpawnRate);
        TimeMasterRewindTime = CustomOption.Normal(1052, CustomOptionType.Crewmate, "timeMasterRewindTime", 3f, 1f, 10f, 1f, TimeMasterSpawnRate);
        TimeMasterShieldDuration = CustomOption.Normal(1053, CustomOptionType.Crewmate, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, TimeMasterSpawnRate);

        DetectiveSpawnRate = new(1060, CustomOptionType.Crewmate, RoleType.Detective, Detective.NameColor, 1);
        DetectiveAnonymousFootprints = CustomOption.Normal(1061, CustomOptionType.Crewmate, "detectiveAnonymousFootprints", false, DetectiveSpawnRate);
        DetectiveFootprintInterval = CustomOption.Normal(1062, CustomOptionType.Crewmate, "detectiveFootprintInterval", 0.5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveFootprintDuration = CustomOption.Normal(1063, CustomOptionType.Crewmate, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveReportNameDuration = CustomOption.Normal(1064, CustomOptionType.Crewmate, "detectiveReportNameDuration", 0, 0, 60, 2.5f, DetectiveSpawnRate);
        DetectiveReportColorDuration = CustomOption.Normal(1065, CustomOptionType.Crewmate, "detectiveReportColorDuration", 20, 0, 120, 2.5f, DetectiveSpawnRate);

        MediumSpawnRate = new(1070, CustomOptionType.Crewmate, RoleType.Medium, Medium.NameColor, 1);
        MediumCooldown = CustomOption.Normal(1071, CustomOptionType.Crewmate, "mediumCooldown", 30f, 5f, 120f, 5f, MediumSpawnRate);
        MediumDuration = CustomOption.Normal(1072, CustomOptionType.Crewmate, "mediumDuration", 3f, 0f, 15f, 1f, MediumSpawnRate);
        MediumOneTimeUse = CustomOption.Normal(1073, CustomOptionType.Crewmate, "mediumOneTimeUse", false, MediumSpawnRate);

        HackerSpawnRate = new(1080, CustomOptionType.Crewmate, RoleType.Hacker, Hacker.NameColor, 1);
        HackerCooldown = CustomOption.Normal(1081, CustomOptionType.Crewmate, "hackerCooldown", 30f, 5f, 60f, 5f, HackerSpawnRate);
        HackerHackingDuration = CustomOption.Normal(1082, CustomOptionType.Crewmate, "hackerHackingDuration", 10f, 2.5f, 60f, 2.5f, HackerSpawnRate);
        HackerOnlyColorType = CustomOption.Normal(1083, CustomOptionType.Crewmate, "hackerOnlyColorType", false, HackerSpawnRate);
        HackerToolsNumber = CustomOption.Normal(1084, CustomOptionType.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, HackerSpawnRate);
        HackerRechargeTasksNumber = CustomOption.Normal(1085, CustomOptionType.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, HackerSpawnRate);
        HackerNoMove = CustomOption.Normal(1086, CustomOptionType.Crewmate, "hackerNoMove", true, HackerSpawnRate);

        TrackerSpawnRate = new(1090, CustomOptionType.Crewmate, RoleType.Tracker, Tracker.NameColor, 1);
        TrackerUpdateInterval = CustomOption.Normal(1091, CustomOptionType.Crewmate, "trackerUpdateInterval", 5f, 1f, 30f, 1f, TrackerSpawnRate);
        TrackerResetTargetAfterMeeting = CustomOption.Normal(1092, CustomOptionType.Crewmate, "trackerResetTargetAfterMeeting", false, TrackerSpawnRate);
        TrackerCanTrackCorpses = CustomOption.Normal(1093, CustomOptionType.Crewmate, "trackerTrackCorpses", true, TrackerSpawnRate);
        TrackerCorpsesTrackingCooldown = CustomOption.Normal(1094, CustomOptionType.Crewmate, "trackerCorpseCooldown", 30f, 0f, 120f, 5f, TrackerCanTrackCorpses);
        TrackerCorpsesTrackingDuration = CustomOption.Normal(1095, CustomOptionType.Crewmate, "trackerCorpseDuration", 5f, 2.5f, 30f, 2.5f, TrackerCanTrackCorpses);

        SnitchSpawnRate = new(1100, CustomOptionType.Crewmate, RoleType.Snitch, Snitch.NameColor, 1);
        SnitchLeftTasksForReveal = CustomOption.Normal(1101, CustomOptionType.Crewmate, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, SnitchSpawnRate);
        SnitchIncludeTeamJackal = CustomOption.Normal(1102, CustomOptionType.Crewmate, "snitchIncludeTeamJackal", false, SnitchSpawnRate);
        SnitchTeamJackalUseDifferentArrowColor = CustomOption.Normal(1103, CustomOptionType.Crewmate, "snitchTeamJackalUseDifferentArrowColor", true, SnitchIncludeTeamJackal);

        LighterSpawnRate = new(1110, CustomOptionType.Crewmate, RoleType.Lighter, Lighter.NameColor, 15);
        LighterModeLightsOnVision = CustomOption.Normal(1111, CustomOptionType.Crewmate, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterModeLightsOffVision = CustomOption.Normal(1112, CustomOptionType.Crewmate, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterCooldown = CustomOption.Normal(1113, CustomOptionType.Crewmate, "lighterCooldown", 30f, 5f, 120f, 5f, LighterSpawnRate);
        LighterDuration = CustomOption.Normal(1114, CustomOptionType.Crewmate, "lighterDuration", 5f, 2.5f, 60f, 2.5f, LighterSpawnRate);
        // lighterCanSeeNinja = CustomOption.Normal(1115, CustomOptionType.Crewmate, "lighterCanSeeNinja", true, lighterSpawnRate);

        SecurityGuardSpawnRate = new(1120, CustomOptionType.Crewmate, RoleType.SecurityGuard, SecurityGuard.NameColor, 1);
        SecurityGuardCooldown = CustomOption.Normal(1121, CustomOptionType.Crewmate, "securityGuardCooldown", 30f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardTotalScrews = CustomOption.Normal(1122, CustomOptionType.Crewmate, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamPrice = CustomOption.Normal(1123, CustomOptionType.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardVentPrice = CustomOption.Normal(1124, CustomOptionType.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamDuration = CustomOption.Normal(1125, CustomOptionType.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardCamMaxCharges = CustomOption.Normal(1126, CustomOptionType.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamRechargeTasksNumber = CustomOption.Normal(1127, CustomOptionType.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, SecurityGuardSpawnRate);
        SecurityGuardNoMove = CustomOption.Normal(1128, CustomOptionType.Crewmate, "securityGuardNoMove", true, SecurityGuardSpawnRate);

        SwapperSpawnRate = new(1130, CustomOptionType.Neutral, RoleType.Swapper, Swapper.NameColor, 1);
        SwapperIsImpRate = CustomOption.Normal(1131, CustomOptionType.Neutral, "swapperIsImpRate", RATES, SwapperSpawnRate);
        SwapperNumSwaps = CustomOption.Normal(1132, CustomOptionType.Neutral, "swapperNumSwaps", 2f, 1f, 15f, 1f, SwapperSpawnRate);
        SwapperCanCallEmergency = CustomOption.Normal(1133, CustomOptionType.Neutral, "swapperCanCallEmergency", false, SwapperSpawnRate);
        SwapperCanOnlySwapOthers = CustomOption.Normal(1134, CustomOptionType.Neutral, "swapperCanOnlySwapOthers", false, SwapperSpawnRate);

        BaitSpawnRate = new(1140, CustomOptionType.Crewmate, RoleType.Bait, Bait.NameColor, 1);
        BaitHighlightAllVents = CustomOption.Normal(1141, CustomOptionType.Crewmate, "baitHighlightAllVents", false, BaitSpawnRate);
        BaitReportDelay = CustomOption.Normal(1142, CustomOptionType.Crewmate, "baitReportDelay", 0f, 0f, 10f, 1f, BaitSpawnRate, format: "unitSeconds");
        BaitShowKillFlash = CustomOption.Normal(1143, CustomOptionType.Crewmate, "baitShowKillFlash", true, BaitSpawnRate);

        ShifterSpawnRate = new(1150, CustomOptionType.Neutral, "shifter", Shifter.NameColor, 1);
        ShifterIsNeutralRate = CustomOption.Normal(1151, CustomOptionType.Neutral, "shifterIsNeutralRate", RATES, ShifterSpawnRate);
        ShifterShiftsModifiers = CustomOption.Normal(1152, CustomOptionType.Neutral, "shifterShiftsModifiers", false, ShifterSpawnRate);
        ShifterPastShifters = CustomOption.Normal(1153, CustomOptionType.Neutral, "shifterPastShifters", false, ShifterSpawnRate);

        #endregion

        #region ROLES IMPOSTOR
        BountyHunterSpawnRate = new(2000, CustomOptionType.Impostor, RoleType.BountyHunter, BountyHunter.NameColor, 1);
        BountyHunterBountyDuration = CustomOption.Normal(2001, CustomOptionType.Impostor, "", 60f, 10f, 180f, 10f, BountyHunterSpawnRate);
        BountyHunterReducedCooldown = CustomOption.Normal(20002, CustomOptionType.Impostor, "", 2.5f, 2.5f, 30f, 2.5f, BountyHunterSpawnRate);
        BountyHunterPunishmentTime = CustomOption.Normal(2003, CustomOptionType.Impostor, "", 20f, 0f, 60f, 2.5f, BountyHunterSpawnRate);
        BountyHunterShowArrow = CustomOption.Normal(2004, CustomOptionType.Impostor, "", true, BountyHunterSpawnRate);
        BountyHunterArrowUpdateInterval = CustomOption.Normal(2005, CustomOptionType.Impostor, "", 15f, 2.5f, 60f, 2.5f, BountyHunterShowArrow);

        MafiaSpawnRate = new(2010, CustomOptionType.Impostor, RoleType.Godfather, Mafia.NameColor, 1);
        MafiosoCanSabotage = CustomOption.Normal(2011, CustomOptionType.Impostor, "mafiosoCanSabotage", false, MafiaSpawnRate);
        MafiosoCanRepair = CustomOption.Normal(2012, CustomOptionType.Impostor, "mafiosoCanRepair", false, MafiaSpawnRate);
        MafiosoCanVent = CustomOption.Normal(2013, CustomOptionType.Impostor, "mafiosoCanVent", false, MafiaSpawnRate);
        JanitorCooldown = CustomOption.Normal(2014, CustomOptionType.Impostor, "janitorCooldown", 30f, 2.5f, 60f, 2.5f, MafiaSpawnRate);
        JanitorCanSabotage = CustomOption.Normal(2015, CustomOptionType.Impostor, "janitorCanSabotage", false, MafiaSpawnRate);
        JanitorCanRepair = CustomOption.Normal(2016, CustomOptionType.Impostor, "janitorCanRepair", false, MafiaSpawnRate);
        JanitorCanVent = CustomOption.Normal(2017, CustomOptionType.Impostor, "janitorCanVent", false, MafiaSpawnRate);

        TricksterSpawnRate = new(2020, CustomOptionType.Impostor, RoleType.Trickster, Trickster.NameColor, 1);
        TricksterPlaceBoxCooldown = CustomOption.Normal(2021, CustomOptionType.Impostor, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, TricksterSpawnRate);
        TricksterLightsOutCooldown = CustomOption.Normal(2022, CustomOptionType.Impostor, "tricksterLightsOutCooldown", 30f, 5f, 60f, 5f, TricksterSpawnRate);
        TricksterLightsOutDuration = CustomOption.Normal(2023, CustomOptionType.Impostor, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, TricksterSpawnRate);

        EvilHackerSpawnRate = new CustomRoleOption(2030, CustomOptionType.Impostor, RoleType.EvilHacker, EvilHacker.NameColor, 1);
        EvilHackerCanHasBetterAdmin = CustomOption.Normal(2031, CustomOptionType.Impostor, "evilHackerCanHasBetterAdmin", false, EvilHackerSpawnRate);
        EvilHackerCanMoveEvenIfUsesAdmin = CustomOption.Normal(2032, CustomOptionType.Impostor, "evilHackerCanMoveEvenIfUsesAdmin", true, EvilHackerSpawnRate);
        EvilHackerCanInheritAbility = CustomOption.Normal(2033, CustomOptionType.Impostor, "evilHackerCanInheritAbility", false, EvilHackerSpawnRate);
        EvilHackerCanSeeDoorStatus = CustomOption.Normal(2034, CustomOptionType.Impostor, "evilHackerCanSeeDoorStatus", true, EvilHackerSpawnRate);
        EvilHackerCanCreateMadmate = CustomOption.Normal(2035, CustomOptionType.Impostor, "evilHackerCanCreateMadmate", false, EvilHackerSpawnRate);
        CreatedMadmateCanDieToSheriff = CustomOption.Normal(2036, CustomOptionType.Impostor, "createdMadmateCanDieToSheriff", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanEnterVents = CustomOption.Normal(2037, CustomOptionType.Impostor, "createdMadmateCanEnterVents", false, EvilHackerCanCreateMadmate);
        EvilHackerCanCreateMadmateFromJackal = CustomOption.Normal(2038, CustomOptionType.Impostor, "evilHackerCanCreateMadmateFromJackal", false, EvilHackerCanCreateMadmate);
        CreatedMadmateHasImpostorVision = CustomOption.Normal(2039, CustomOptionType.Impostor, "createdMadmateHasImpostorVision", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanSabotage = CustomOption.Normal(2040, CustomOptionType.Impostor, "createdMadmateCanSabotage", false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanFixComm = CustomOption.Normal(2041, CustomOptionType.Impostor, "createdMadmateCanFixComm", true, EvilHackerCanCreateMadmate);
        CreatedMadmateAbility = CustomOption.Normal(2042, CustomOptionType.Impostor, "madmateAbility", ["madmateNone", "madmateFanatic"], EvilHackerCanCreateMadmate);
        CreatedMadmateNumTasks = CustomOption.Normal(2043, CustomOptionType.Impostor, "createdMadmateNumTasks", 4f, 1f, 20f, 1f, CreatedMadmateAbility);
        CreatedMadmateExileCrewmate = CustomOption.Normal(2044, CustomOptionType.Impostor, "createdMadmateExileCrewmate", false, EvilHackerCanCreateMadmate);

        EvilTrackerSpawnRate = new(2050, CustomOptionType.Impostor, RoleType.EvilTracker, EvilTracker.NameColor, 3);
        EvilTrackerCooldown = CustomOption.Normal(2051, CustomOptionType.Impostor, "evilTrackerCooldown", 10f, 0f, 60f, 1f, EvilTrackerSpawnRate);
        EvilTrackerResetTargetAfterMeeting = CustomOption.Normal(2052, CustomOptionType.Impostor, "evilTrackerResetTargetAfterMeeting", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeDeathFlash = CustomOption.Normal(2053, CustomOptionType.Impostor, "evilTrackerCanSeeDeathFlash", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetTask = CustomOption.Normal(2054, CustomOptionType.Impostor, "evilTrackerCanSeeTargetTask", true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetPosition = CustomOption.Normal(2055, CustomOptionType.Impostor, "evilTrackerCanSeeTargetPosition", true, EvilTrackerSpawnRate);
        EvilTrackerCanSetTargetOnMeeting = CustomOption.Normal(2056, CustomOptionType.Impostor, "evilTrackerCanSetTargetOnMeeting", true, EvilTrackerSpawnRate);

        EraserSpawnRate = new(2060, CustomOptionType.Impostor, RoleType.Eraser, Eraser.NameColor, 1);
        EraserCooldown = CustomOption.Normal(2061, CustomOptionType.Impostor, "eraserCooldown", 30f, 5f, 120f, 5f, EraserSpawnRate, format: "unitSeconds");
        EraserCooldownIncrease = CustomOption.Normal(2062, CustomOptionType.Impostor, "eraserCooldownIncrease", 10f, 0f, 120f, 2.5f, EraserSpawnRate, format: "unitSeconds");
        EraserCanEraseAnyone = CustomOption.Normal(2063, CustomOptionType.Impostor, "eraserCanEraseAnyone", false, EraserSpawnRate);

        MorphingSpawnRate = new(2070, CustomOptionType.Impostor, RoleType.Morphing, Morphing.NameColor, 1);
        MorphingCooldown = CustomOption.Normal(2071, CustomOptionType.Impostor, "morphingCooldown", 30f, 2.5f, 60f, 2.5f, MorphingSpawnRate, format: "unitSeconds");
        MorphingDuration = CustomOption.Normal(2072, CustomOptionType.Impostor, "morphingDuration", 10f, 1f, 20f, 0.5f, MorphingSpawnRate, format: "unitSeconds");

        CamouflagerSpawnRate = new(2080, CustomOptionType.Impostor, RoleType.Camouflager, Camouflager.NameColor, 1);
        CamouflagerCooldown = CustomOption.Normal(2081, CustomOptionType.Impostor, "camouflagerCooldown", 30f, 2.5f, 60f, 2.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerDuration = CustomOption.Normal(2082, CustomOptionType.Impostor, "camouflagerDuration", 10f, 1f, 20f, 0.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerRandomColors = CustomOption.Normal(2083, CustomOptionType.Impostor, "camouflagerRandomColors", false, CamouflagerSpawnRate);

        CleanerSpawnRate = new(2090, CustomOptionType.Impostor, RoleType.Cleaner, Cleaner.NameColor, 1);
        CleanerCooldown = CustomOption.Normal(2091, CustomOptionType.Impostor, "cleanerCooldown", 30f, 2.5f, 60f, 2.5f, CleanerSpawnRate, format: "unitSeconds");

        WarlockSpawnRate = new(2100, CustomOptionType.Impostor, RoleType.Warlock, Warlock.NameColor, 1);
        WarlockCooldown = CustomOption.Normal(2101, CustomOptionType.Impostor, "warlockCooldown", 30f, 2.5f, 60f, 2.5f, WarlockSpawnRate, format: "unitSeconds");
        WarlockRootTime = CustomOption.Normal(2102, CustomOptionType.Impostor, "warlockRootTime", 5f, 0f, 15f, 1f, WarlockSpawnRate, format: "unitSeconds");

        WitchSpawnRate = new(2110, CustomOptionType.Impostor, RoleType.Witch, Witch.NameColor, 1);
        WitchCooldown = CustomOption.Normal(2111, CustomOptionType.Impostor, "witchSpellCooldown", 30f, 2.5f, 120f, 2.5f, WitchSpawnRate, format: "unitSeconds");
        WitchAdditionalCooldown = CustomOption.Normal(2112, CustomOptionType.Impostor, "witchAdditionalCooldown", 10f, 0f, 60f, 5f, WitchSpawnRate, format: "unitSeconds");
        WitchCanSpellAnyone = CustomOption.Normal(2113, CustomOptionType.Impostor, "witchCanSpellAnyone", false, WitchSpawnRate);
        WitchSpellCastingDuration = CustomOption.Normal(2114, CustomOptionType.Impostor, "witchSpellDuration", 1f, 0f, 10f, 1f, WitchSpawnRate, format: "unitSeconds");
        WitchTriggerBothCooldowns = CustomOption.Normal(2115, CustomOptionType.Impostor, "witchTriggerBoth", true, WitchSpawnRate);
        WitchVoteSavesTargets = CustomOption.Normal(2116, CustomOptionType.Impostor, "witchSaveTargets", true, WitchSpawnRate);

        VampireSpawnRate = new(2120, CustomOptionType.Impostor, RoleType.Vampire, Vampire.NameColor, 1);
        VampireKillDelay = CustomOption.Normal(2121, CustomOptionType.Impostor, "vampireKillDelay", 10f, 1f, 20f, 1f, VampireSpawnRate, format: "unitSeconds");
        VampireCooldown = CustomOption.Normal(2122, CustomOptionType.Impostor, "vampireCooldown", 30f, 2.5f, 60f, 2.5f, VampireSpawnRate, format: "unitSeconds");
        VampireCanKillNearGarlics = CustomOption.Normal(2123, CustomOptionType.Impostor, "vampireCanKillNearGarlics", true, VampireSpawnRate);
        #endregion

        #region ROLES NEUTRAL
        JesterSpawnRate = new(3000, CustomOptionType.Neutral, RoleType.Jester, Jester.NameColor, 1);
        JesterCanCallEmergency = CustomOption.Normal(3001, CustomOptionType.Neutral, "", true, JesterSpawnRate);
        JesterCanSabotage = CustomOption.Normal(3002, CustomOptionType.Neutral, "", true, JesterSpawnRate);
        JesterHasImpostorVision = CustomOption.Normal(3003, CustomOptionType.Neutral, "", false, JesterSpawnRate);

        ArsonistSpawnRate = new(3010, CustomOptionType.Neutral, RoleType.Arsonist, Arsonist.NameColor, 1);
        ArsonistCooldown = CustomOption.Normal(3011, CustomOptionType.Neutral, "", 12.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
        ArsonistDuration = CustomOption.Normal(3012, CustomOptionType.Neutral, "", 3f, 0f, 10f, 1f, ArsonistSpawnRate);
        ArsonistCanBeLovers = CustomOption.Normal(3013, CustomOptionType.Neutral, "", false, ArsonistSpawnRate);

        VultureSpawnRate = new(3020, CustomOptionType.Neutral, RoleType.Vulture, Vulture.NameColor, 1);
        VultureCooldown = CustomOption.Normal(3021, CustomOptionType.Neutral, "", 15f, 2.5f, 60f, 2.5f, VultureSpawnRate);
        VultureNumberToWin = CustomOption.Normal(3022, CustomOptionType.Neutral, "", 4f, 1f, 12f, 1f, VultureSpawnRate);
        VultureCanUseVents = CustomOption.Normal(3023, CustomOptionType.Neutral, "", true, VultureSpawnRate);
        VultureShowArrows = CustomOption.Normal(3024, CustomOptionType.Neutral, "", true, VultureSpawnRate);

        JackalSpawnRate = new(3030, CustomOptionType.Neutral, RoleType.Jackal, Jackal.NameColor, 1);
        JackalKillCooldown = CustomOption.Normal(3031, CustomOptionType.Neutral, "", 30f, 10f, 60f, 2.5f, JackalSpawnRate);
        JackalCanSabotageLights = CustomOption.Normal(3032, CustomOptionType.Neutral, "", true, JackalSpawnRate);
        JackalCanUseVents = CustomOption.Normal(3033, CustomOptionType.Neutral, "", true, JackalSpawnRate);
        JackalHasImpostorVision = CustomOption.Normal(3034, CustomOptionType.Neutral, "", false, JackalSpawnRate);
        JackalCanCreateSidekick = CustomOption.Normal(3035, CustomOptionType.Neutral, "", false, JackalSpawnRate);
        JackalCreateSidekickCooldown = CustomOption.Normal(3036, CustomOptionType.Neutral, "", 30f, 10f, 60f, 2.5f, JackalCanCreateSidekick);
        SidekickCanKill = CustomOption.Normal(3038, CustomOptionType.Neutral, "", false, JackalCanCreateSidekick);
        SidekickCanUseVents = CustomOption.Normal(3039, CustomOptionType.Neutral, "", true, JackalCanCreateSidekick);
        SidekickCanSabotageLights = CustomOption.Normal(3040, CustomOptionType.Neutral, "", true, JackalCanCreateSidekick);
        SidekickHasImpostorVision = CustomOption.Normal(3041, CustomOptionType.Neutral, "", false, JackalCanCreateSidekick);
        SidekickPromotesToJackal = CustomOption.Normal(3037, CustomOptionType.Neutral, "", false, JackalCanCreateSidekick);
        JackalPromotedFromSidekickCanCreateSidekick = CustomOption.Normal(3042, CustomOptionType.Neutral, "", false, SidekickPromotesToJackal);
        JackalCanCreateSidekickFromImpostor = CustomOption.Normal(3043, CustomOptionType.Neutral, "", false, JackalCanCreateSidekick);

        GuesserSpawnRate = new(3050, CustomOptionType.Neutral, nameof(Guesser), Guesser.NiceGuesser.NameColor, 1);
        GuesserIsImpGuesserRate = CustomOption.Normal(3051, CustomOptionType.Neutral, "guesserIsImpGuesserRate", RATES, GuesserSpawnRate);
        GuesserSpawnBothRate = CustomOption.Normal(3052, CustomOptionType.Neutral, "guesserSpawnBothRate", RATES, GuesserSpawnRate);
        GuesserNumberOfShots = CustomOption.Normal(3053, CustomOptionType.Neutral, "guesserNumberOfShots", 2f, 1f, 15f, 1f, GuesserSpawnRate);
        GuesserOnlyAvailableRoles = CustomOption.Normal(3054, CustomOptionType.Neutral, "guesserOnlyAvailableRoles", true, GuesserSpawnRate);
        GuesserHasMultipleShotsPerMeeting = CustomOption.Normal(3055, CustomOptionType.Neutral, "guesserHasMultipleShotsPerMeeting", false, GuesserSpawnRate);
        GuesserShowInfoInGhostChat = CustomOption.Normal(3056, CustomOptionType.Neutral, "guesserToGhostChat", true, GuesserSpawnRate);
        GuesserKillsThroughShield = CustomOption.Normal(3057, CustomOptionType.Neutral, "guesserPierceShield", true, GuesserSpawnRate);
        GuesserEvilCanKillSpy = CustomOption.Normal(3058, CustomOptionType.Neutral, "guesserEvilCanKillSpy", true, GuesserSpawnRate);
        #endregion

        #region MODIFIERS
        MadmateSpawnRate = new(4000, CustomOptionType.Modifier, ModifierType.Madmate, Madmate.NameColor);
        MadmateType = CustomOption.Normal(4001, CustomOptionType.Modifier, "madmateType", ["madmateDefault", "madmateWithRole", "madmateRandom"], MadmateSpawnRate);
        MadmateFixedRole = new CustomRoleSelectionOption(4002, CustomOptionType.Modifier, "madmateFixedRole", Madmate.ValidRoles, MadmateType);
        MadmateAbility = CustomOption.Normal(4003, CustomOptionType.Modifier, "madmateAbility", ["madmateNone", "madmateFanatic"], MadmateSpawnRate);
        MadmateTasks = new((4004, 4005, 4006), CustomOptionType.Modifier, (1, 1, 3), MadmateAbility);
        MadmateCanDieToSheriff = CustomOption.Normal(4007, CustomOptionType.Modifier, "madmateCanDieToSheriff", false, MadmateSpawnRate);
        MadmateCanEnterVents = CustomOption.Normal(4008, CustomOptionType.Modifier, "madmateCanEnterVents", false, MadmateSpawnRate);
        MadmateHasImpostorVision = CustomOption.Normal(4009, CustomOptionType.Modifier, "madmateHasImpostorVision", false, MadmateSpawnRate);
        MadmateCanSabotage = CustomOption.Normal(4010, CustomOptionType.Modifier, "madmateCanSabotage", false, MadmateSpawnRate);
        MadmateCanFixComm = CustomOption.Normal(4011, CustomOptionType.Modifier, "madmateCanFixComm", true, MadmateSpawnRate);
        MadmateExilePlayer = CustomOption.Normal(4012, CustomOptionType.Modifier, "madmateExileCrewmate", false, MadmateSpawnRate);

        LastImpostorEnable = CustomOption.Header(4010, CustomOptionType.Modifier, "lastImpostorEnable", true, nameof(LastImpostor));
        LastImpostorFunctions = CustomOption.Normal(4011, CustomOptionType.Modifier, "lastImpostorFunctions", ["lastImpostorDivine", "lastImpostorGuesser"], LastImpostorEnable);
        LastImpostorNumKills = CustomOption.Normal(4012, CustomOptionType.Modifier, "lastImpostorNumKills", 3f, 0f, 10f, 1f, LastImpostorEnable);
        LastImpostorResults = CustomOption.Normal(4013, CustomOptionType.Modifier, "fortuneTellerResults ", ["fortuneTellerResultCrew", "fortuneTellerResultTeam", "fortuneTellerResultRole"], LastImpostorEnable);
        LastImpostorNumShots = CustomOption.Normal(4014, CustomOptionType.Modifier, "lastImpostorNumShots", 1f, 1f, 15f, 1f, LastImpostorEnable);

        LoversSpawnRate = new(4020, CustomOptionType.Modifier, RoleType.Lovers, Lovers.Color, 1);
        LoversImpLoverRate = CustomOption.Normal(4021, CustomOptionType.Modifier, "loversImpLoverRate", RATES, LoversSpawnRate);
        LoversNumCouples = CustomOption.Normal(4022, CustomOptionType.Modifier, "loversNumCouples", 1f, 1f, 7f, 1f, LoversSpawnRate, format: "unitCouples");
        LoversBothDie = CustomOption.Normal(4023, CustomOptionType.Modifier, "loversBothDie", true, LoversSpawnRate);
        LoversCanHaveAnotherRole = CustomOption.Normal(4024, CustomOptionType.Modifier, "loversCanHaveAnotherRole", true, LoversSpawnRate);
        LoversSeparateTeam = CustomOption.Normal(4025, CustomOptionType.Modifier, "loversSeparateTeam", true, LoversSpawnRate);
        LoversTasksCount = CustomOption.Normal(4026, CustomOptionType.Modifier, "loversTasksCount", false, LoversSpawnRate);
        LoversEnableChat = CustomOption.Normal(4027, CustomOptionType.Modifier, "loversEnableChat", true, LoversSpawnRate);

        MiniSpawnRate = new(180, CustomOptionType.Modifier, ModifierType.Mini, Mini.NameColor, 15);
        MiniGrowingUpDuration = CustomOption.Normal(181, CustomOptionType.Modifier, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, MiniSpawnRate, format: "unitSeconds");

        AntiTeleportSpawnRate = new(4030, CustomOptionType.Modifier, ModifierType.AntiTeleport, AntiTeleport.NameColor, 15);

        #endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }
}