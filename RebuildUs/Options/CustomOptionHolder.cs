using RebuildUs.Modules;
using RebuildUs.Modules.CustomOptions;
using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Modifier;
using RebuildUs.Roles.Neutral;

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

    public static CustomRoleOption medicSpawnRate;
    public static CustomOption medicShowShielded;
    public static CustomOption medicShowAttemptToShielded;
    public static CustomOption medicSetShieldAfterMeeting;
    public static CustomOption medicShowAttemptToMedic;

    public static CustomRoleOption seerSpawnRate;
    public static CustomOption seerMode;
    public static CustomOption seerSoulDuration;
    public static CustomOption seerLimitSoulDuration;

    public static CustomRoleOption timeMasterSpawnRate;
    public static CustomOption timeMasterCooldown;
    public static CustomOption timeMasterRewindTime;
    public static CustomOption timeMasterShieldDuration;

    public static CustomRoleOption detectiveSpawnRate;
    public static CustomOption detectiveAnonymousFootprints;
    public static CustomOption detectiveFootprintInterval;
    public static CustomOption detectiveFootprintDuration;
    public static CustomOption detectiveReportNameDuration;
    public static CustomOption detectiveReportColorDuration;

    public static CustomRoleOption mediumSpawnRate;
    public static CustomOption mediumCooldown;
    public static CustomOption mediumDuration;
    public static CustomOption mediumOneTimeUse;

    public static CustomRoleOption hackerSpawnRate;
    public static CustomOption hackerCooldown;
    public static CustomOption hackerHackingDuration;
    public static CustomOption hackerOnlyColorType;
    public static CustomOption hackerToolsNumber;
    public static CustomOption hackerRechargeTasksNumber;
    public static CustomOption hackerNoMove;

    public static CustomRoleOption trackerSpawnRate;
    public static CustomOption trackerUpdateInterval;
    public static CustomOption trackerResetTargetAfterMeeting;
    public static CustomOption trackerCanTrackCorpses;
    public static CustomOption trackerCorpsesTrackingCooldown;
    public static CustomOption trackerCorpsesTrackingDuration;

    public static CustomRoleOption snitchSpawnRate;
    public static CustomOption snitchLeftTasksForReveal;
    public static CustomOption snitchIncludeTeamJackal;
    public static CustomOption snitchTeamJackalUseDifferentArrowColor;

    public static CustomRoleOption lighterSpawnRate;
    public static CustomOption lighterModeLightsOnVision;
    public static CustomOption lighterModeLightsOffVision;
    public static CustomOption lighterCooldown;
    public static CustomOption lighterDuration;
    public static CustomOption lighterCanSeeNinja;

    public static CustomRoleOption securityGuardSpawnRate;
    public static CustomOption securityGuardCooldown;
    public static CustomOption securityGuardTotalScrews;
    public static CustomOption securityGuardCamPrice;
    public static CustomOption securityGuardVentPrice;
    public static CustomOption securityGuardCamDuration;
    public static CustomOption securityGuardCamMaxCharges;
    public static CustomOption securityGuardCamRechargeTasksNumber;
    public static CustomOption securityGuardNoMove;
    #endregion

    #region ROLES IMPOSTOR
    public static CustomRoleOption BountyHunterSpawnRate;
    public static CustomOption BountyHunterBountyDuration;
    public static CustomOption BountyHunterReducedCooldown;
    public static CustomOption BountyHunterPunishmentTime;
    public static CustomOption BountyHunterShowArrow;
    public static CustomOption BountyHunterArrowUpdateInterval;

    public static CustomRoleOption mafiaSpawnRate;
    public static CustomOption mafiosoCanSabotage;
    public static CustomOption mafiosoCanRepair;
    public static CustomOption mafiosoCanVent;
    public static CustomOption janitorCooldown;
    public static CustomOption janitorCanSabotage;
    public static CustomOption janitorCanRepair;
    public static CustomOption janitorCanVent;

    public static CustomRoleOption tricksterSpawnRate;
    public static CustomOption tricksterPlaceBoxCooldown;
    public static CustomOption tricksterLightsOutCooldown;
    public static CustomOption tricksterLightsOutDuration;

    public static CustomRoleOption evilHackerSpawnRate;
    public static CustomOption evilHackerCanHasBetterAdmin;
    public static CustomOption evilHackerCanCreateMadmate;
    public static CustomOption evilHackerCanCreateMadmateFromJackal;
    public static CustomOption evilHackerCanMoveEvenIfUsesAdmin;
    public static CustomOption evilHackerCanInheritAbility;
    public static CustomOption evilHackerCanSeeDoorStatus;
    public static CustomOption createdMadmateCanDieToSheriff;
    public static CustomOption createdMadmateCanEnterVents;
    public static CustomOption createdMadmateHasImpostorVision;
    public static CustomOption createdMadmateCanSabotage;
    public static CustomOption createdMadmateCanFixComm;
    public static CustomOption createdMadmateAbility;
    public static CustomOption createdMadmateNumTasks;
    public static CustomOption createdMadmateExileCrewmate;

    public static CustomRoleOption evilTrackerSpawnRate;
    public static CustomOption evilTrackerCooldown;
    public static CustomOption evilTrackerResetTargetAfterMeeting;
    public static CustomOption evilTrackerCanSeeDeathFlash;
    public static CustomOption evilTrackerCanSeeTargetTask;
    public static CustomOption evilTrackerCanSeeTargetPosition;
    public static CustomOption evilTrackerCanSetTargetOnMeeting;

    public static CustomRoleOption eraserSpawnRate;
    public static CustomOption eraserCooldown;
    public static CustomOption eraserCooldownIncrease;
    public static CustomOption eraserCanEraseAnyone;
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

    public static CustomRoleOption guesserSpawnRate;
    public static CustomOption guesserIsImpGuesserRate;
    public static CustomOption guesserNumberOfShots;
    public static CustomOption guesserOnlyAvailableRoles;
    public static CustomOption guesserHasMultipleShotsPerMeeting;
    public static CustomOption guesserShowInfoInGhostChat;
    public static CustomOption guesserKillsThroughShield;
    public static CustomOption guesserEvilCanKillSpy;
    public static CustomOption guesserSpawnBothRate;
    #endregion

    #region MODIFIERS
    public static CustomModifierOption madmateSpawnRate;
    public static CustomOption madmateCanDieToSheriff;
    public static CustomOption madmateCanEnterVents;
    public static CustomOption madmateHasImpostorVision;
    public static CustomOption madmateCanSabotage;
    public static CustomOption madmateCanFixComm;
    public static CustomOption madmateType;
    public static CustomRoleSelectionOption madmateFixedRole;
    public static CustomOption madmateAbility;
    public static CustomTasksOption madmateTasks;
    public static CustomOption madmateExilePlayer;
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
        MayorSpawnRate = new(1000, CustomOptionType.Crewmate, RoleType.Mayor, Mayor.RoleColor);
        MayorNumVotes = CustomOption.Normal(1001, CustomOptionType.Crewmate, "", 2f, 2f, 10f, 1f, MayorSpawnRate);
        MayorCanSeeVoteColors = CustomOption.Normal(1002, CustomOptionType.Crewmate, "", false, MayorSpawnRate);
        MayorTasksNeededToSeeVoteColors = CustomOption.Normal(1003, CustomOptionType.Crewmate, "", true, MayorCanSeeVoteColors);
        MayorMeetingButton = CustomOption.Normal(1004, CustomOptionType.Crewmate, "", true, MayorSpawnRate);
        MayorMaxRemoteMeetings = CustomOption.Normal(1005, CustomOptionType.Crewmate, "", 1f, 0f, 10f, 1f, MayorMeetingButton);

        EngineerSpawnRate = new(1010, CustomOptionType.Crewmate, RoleType.Engineer, Engineer.RoleColor);
        EngineerNumberOfFixes = CustomOption.Normal(1011, CustomOptionType.Crewmate, "", 1f, 0f, 3f, 1f, EngineerSpawnRate);
        EngineerHighlightForImpostors = CustomOption.Normal(1012, CustomOptionType.Crewmate, "", true, EngineerSpawnRate);
        EngineerHighlightForTeamJackal = CustomOption.Normal(1013, CustomOptionType.Crewmate, "", true, EngineerSpawnRate);

        SpySpawnRate = new(1020, CustomOptionType.Crewmate, RoleType.Spy, Spy.RoleColor, 1);
        SpyCanDieToSheriff = CustomOption.Normal(1021, CustomOptionType.Crewmate, "", false, SpySpawnRate);
        SpyImpostorsCanKillAnyone = CustomOption.Normal(1022, CustomOptionType.Crewmate, "", true, SpySpawnRate);
        SpyCanEnterVents = CustomOption.Normal(1023, CustomOptionType.Crewmate, "", false, SpySpawnRate);
        SpyHasImpostorVision = CustomOption.Normal(1024, CustomOptionType.Crewmate, "", false, SpySpawnRate);

        medicSpawnRate = new(1030, CustomOptionType.Crewmate, RoleType.Medic, Medic.RoleColor, 1);
        medicShowShielded = CustomOption.Normal(1031, CustomOptionType.Crewmate, "medicShowShielded", ["medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic"], medicSpawnRate);
        medicShowAttemptToShielded = CustomOption.Normal(1032, CustomOptionType.Crewmate, "medicShowAttemptToShielded", false, medicSpawnRate);
        medicSetShieldAfterMeeting = CustomOption.Normal(1033, CustomOptionType.Crewmate, "medicSetShieldAfterMeeting", false, medicSpawnRate);
        medicShowAttemptToMedic = CustomOption.Normal(1034, CustomOptionType.Crewmate, "medicSeesMurderAttempt", false, medicSpawnRate);

        seerSpawnRate = new(1040, CustomOptionType.Crewmate, RoleType.Seer, Seer.RoleColor, 1);
        seerMode = CustomOption.Normal(1041, CustomOptionType.Crewmate, "seerMode", ["seerModeBoth", "seerModeFlash", "seerModeSouls"], seerSpawnRate);
        seerLimitSoulDuration = CustomOption.Normal(1042, CustomOptionType.Crewmate, "seerLimitSoulDuration", false, seerSpawnRate);
        seerSoulDuration = CustomOption.Normal(1043, CustomOptionType.Crewmate, "seerSoulDuration", 15f, 0f, 120f, 5f, seerLimitSoulDuration);

        timeMasterSpawnRate = new(1050, CustomOptionType.Crewmate, RoleType.TimeMaster, TimeMaster.RoleColor, 1);
        timeMasterCooldown = CustomOption.Normal(1051, CustomOptionType.Crewmate, "timeMasterCooldown", 30f, 2.5f, 120f, 2.5f, timeMasterSpawnRate);
        timeMasterRewindTime = CustomOption.Normal(1052, CustomOptionType.Crewmate, "timeMasterRewindTime", 3f, 1f, 10f, 1f, timeMasterSpawnRate);
        timeMasterShieldDuration = CustomOption.Normal(1053, CustomOptionType.Crewmate, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, timeMasterSpawnRate);

        detectiveSpawnRate = new(1060, CustomOptionType.Crewmate, RoleType.Detective, Detective.RoleColor, 1);
        detectiveAnonymousFootprints = CustomOption.Normal(1061, CustomOptionType.Crewmate, "detectiveAnonymousFootprints", false, detectiveSpawnRate);
        detectiveFootprintInterval = CustomOption.Normal(1062, CustomOptionType.Crewmate, "detectiveFootprintInterval", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration = CustomOption.Normal(1063, CustomOptionType.Crewmate, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveReportNameDuration = CustomOption.Normal(1064, CustomOptionType.Crewmate, "detectiveReportNameDuration", 0, 0, 60, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = CustomOption.Normal(1065, CustomOptionType.Crewmate, "detectiveReportColorDuration", 20, 0, 120, 2.5f, detectiveSpawnRate);

        mediumSpawnRate = new(1070, CustomOptionType.Crewmate, RoleType.Medium, Medium.RoleColor, 1);
        mediumCooldown = CustomOption.Normal(1071, CustomOptionType.Crewmate, "mediumCooldown", 30f, 5f, 120f, 5f, mediumSpawnRate);
        mediumDuration = CustomOption.Normal(1072, CustomOptionType.Crewmate, "mediumDuration", 3f, 0f, 15f, 1f, mediumSpawnRate);
        mediumOneTimeUse = CustomOption.Normal(1073, CustomOptionType.Crewmate, "mediumOneTimeUse", false, mediumSpawnRate);

        hackerSpawnRate = new(1080, CustomOptionType.Crewmate, RoleType.Hacker, Hacker.RoleColor, 1);
        hackerCooldown = CustomOption.Normal(1081, CustomOptionType.Crewmate, "hackerCooldown", 30f, 5f, 60f, 5f, hackerSpawnRate);
        hackerHackingDuration = CustomOption.Normal(1082, CustomOptionType.Crewmate, "hackerHackingDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = CustomOption.Normal(1083, CustomOptionType.Crewmate, "hackerOnlyColorType", false, hackerSpawnRate);
        hackerToolsNumber = CustomOption.Normal(1084, CustomOptionType.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber = CustomOption.Normal(1085, CustomOptionType.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = CustomOption.Normal(1086, CustomOptionType.Crewmate, "hackerNoMove", true, hackerSpawnRate);

        trackerSpawnRate = new(1090, CustomOptionType.Crewmate, RoleType.Tracker, Tracker.RoleColor, 1);
        trackerUpdateInterval = CustomOption.Normal(1091, CustomOptionType.Crewmate, "trackerUpdateInterval", 5f, 1f, 30f, 1f, trackerSpawnRate);
        trackerResetTargetAfterMeeting = CustomOption.Normal(1092, CustomOptionType.Crewmate, "trackerResetTargetAfterMeeting", false, trackerSpawnRate);
        trackerCanTrackCorpses = CustomOption.Normal(1093, CustomOptionType.Crewmate, "trackerTrackCorpses", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown = CustomOption.Normal(1094, CustomOptionType.Crewmate, "trackerCorpseCooldown", 30f, 0f, 120f, 5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = CustomOption.Normal(1095, CustomOptionType.Crewmate, "trackerCorpseDuration", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);

        snitchSpawnRate = new(1100, CustomOptionType.Crewmate, RoleType.Snitch, Snitch.RoleColor, 1);
        snitchLeftTasksForReveal = CustomOption.Normal(1101, CustomOptionType.Crewmate, "snitchLeftTasksForReveal", 1f, 0f, 5f, 1f, snitchSpawnRate);
        snitchIncludeTeamJackal = CustomOption.Normal(1102, CustomOptionType.Crewmate, "snitchIncludeTeamJackal", false, snitchSpawnRate);
        snitchTeamJackalUseDifferentArrowColor = CustomOption.Normal(1103, CustomOptionType.Crewmate, "snitchTeamJackalUseDifferentArrowColor", true, snitchIncludeTeamJackal);

        lighterSpawnRate = new(1110, CustomOptionType.Crewmate, RoleType.Lighter, Lighter.RoleColor, 15);
        lighterModeLightsOnVision = CustomOption.Normal(1111, CustomOptionType.Crewmate, "lighterModeLightsOnVision", 2f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterModeLightsOffVision = CustomOption.Normal(1112, CustomOptionType.Crewmate, "lighterModeLightsOffVision", 0.75f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterCooldown = CustomOption.Normal(1113, CustomOptionType.Crewmate, "lighterCooldown", 30f, 5f, 120f, 5f, lighterSpawnRate);
        lighterDuration = CustomOption.Normal(1114, CustomOptionType.Crewmate, "lighterDuration", 5f, 2.5f, 60f, 2.5f, lighterSpawnRate);
        // lighterCanSeeNinja = CustomOption.Normal(1115, CustomOptionType.Crewmate, "lighterCanSeeNinja", true, lighterSpawnRate);

        securityGuardSpawnRate = new(1120, CustomOptionType.Crewmate, RoleType.SecurityGuard, SecurityGuard.RoleColor, 1);
        securityGuardCooldown = CustomOption.Normal(1121, CustomOptionType.Crewmate, "securityGuardCooldown", 30f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews = CustomOption.Normal(1122, CustomOptionType.Crewmate, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice = CustomOption.Normal(1123, CustomOptionType.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice = CustomOption.Normal(1124, CustomOptionType.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration = CustomOption.Normal(1125, CustomOptionType.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges = CustomOption.Normal(1126, CustomOptionType.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber = CustomOption.Normal(1127, CustomOptionType.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = CustomOption.Normal(1128, CustomOptionType.Crewmate, "securityGuardNoMove", true, securityGuardSpawnRate);
        #endregion

        #region ROLES IMPOSTOR
        BountyHunterSpawnRate = new(2000, CustomOptionType.Impostor, RoleType.BountyHunter, BountyHunter.RoleColor, 1);
        BountyHunterBountyDuration = CustomOption.Normal(2001, CustomOptionType.Impostor, "", 60f, 10f, 180f, 10f, BountyHunterSpawnRate);
        BountyHunterReducedCooldown = CustomOption.Normal(20002, CustomOptionType.Impostor, "", 2.5f, 2.5f, 30f, 2.5f, BountyHunterSpawnRate);
        BountyHunterPunishmentTime = CustomOption.Normal(2003, CustomOptionType.Impostor, "", 20f, 0f, 60f, 2.5f, BountyHunterSpawnRate);
        BountyHunterShowArrow = CustomOption.Normal(2004, CustomOptionType.Impostor, "", true, BountyHunterSpawnRate);
        BountyHunterArrowUpdateInterval = CustomOption.Normal(2005, CustomOptionType.Impostor, "", 15f, 2.5f, 60f, 2.5f, BountyHunterShowArrow);

        mafiaSpawnRate = new(2010, CustomOptionType.Impostor, RoleType.Godfather, Mafia.RoleColor, 1);
        mafiosoCanSabotage = CustomOption.Normal(2011, CustomOptionType.Impostor, "mafiosoCanSabotage", false, mafiaSpawnRate);
        mafiosoCanRepair = CustomOption.Normal(2012, CustomOptionType.Impostor, "mafiosoCanRepair", false, mafiaSpawnRate);
        mafiosoCanVent = CustomOption.Normal(2013, CustomOptionType.Impostor, "mafiosoCanVent", false, mafiaSpawnRate);
        janitorCooldown = CustomOption.Normal(2014, CustomOptionType.Impostor, "janitorCooldown", 30f, 2.5f, 60f, 2.5f, mafiaSpawnRate);
        janitorCanSabotage = CustomOption.Normal(2015, CustomOptionType.Impostor, "janitorCanSabotage", false, mafiaSpawnRate);
        janitorCanRepair = CustomOption.Normal(2016, CustomOptionType.Impostor, "janitorCanRepair", false, mafiaSpawnRate);
        janitorCanVent = CustomOption.Normal(2017, CustomOptionType.Impostor, "janitorCanVent", false, mafiaSpawnRate);

        tricksterSpawnRate = new(2020, CustomOptionType.Impostor, RoleType.Trickster, Trickster.RoleColor, 1);
        tricksterPlaceBoxCooldown = CustomOption.Normal(2021, CustomOptionType.Impostor, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown = CustomOption.Normal(2022, CustomOptionType.Impostor, "tricksterLightsOutCooldown", 30f, 5f, 60f, 5f, tricksterSpawnRate);
        tricksterLightsOutDuration = CustomOption.Normal(2023, CustomOptionType.Impostor, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, tricksterSpawnRate);

        evilHackerSpawnRate = new CustomRoleOption(2030, CustomOptionType.Impostor, RoleType.EvilHacker, EvilHacker.RoleColor, 1);
        evilHackerCanHasBetterAdmin = CustomOption.Normal(2031, CustomOptionType.Impostor, "evilHackerCanHasBetterAdmin", false, evilHackerSpawnRate);
        evilHackerCanMoveEvenIfUsesAdmin = CustomOption.Normal(2032, CustomOptionType.Impostor, "evilHackerCanMoveEvenIfUsesAdmin", true, evilHackerSpawnRate);
        evilHackerCanInheritAbility = CustomOption.Normal(2033, CustomOptionType.Impostor, "evilHackerCanInheritAbility", false, evilHackerSpawnRate);
        evilHackerCanSeeDoorStatus = CustomOption.Normal(2034, CustomOptionType.Impostor, "evilHackerCanSeeDoorStatus", true, evilHackerSpawnRate);
        evilHackerCanCreateMadmate = CustomOption.Normal(2035, CustomOptionType.Impostor, "evilHackerCanCreateMadmate", false, evilHackerSpawnRate);
        createdMadmateCanDieToSheriff = CustomOption.Normal(2036, CustomOptionType.Impostor, "createdMadmateCanDieToSheriff", false, evilHackerCanCreateMadmate);
        createdMadmateCanEnterVents = CustomOption.Normal(2037, CustomOptionType.Impostor, "createdMadmateCanEnterVents", false, evilHackerCanCreateMadmate);
        evilHackerCanCreateMadmateFromJackal = CustomOption.Normal(2038, CustomOptionType.Impostor, "evilHackerCanCreateMadmateFromJackal", false, evilHackerCanCreateMadmate);
        createdMadmateHasImpostorVision = CustomOption.Normal(2039, CustomOptionType.Impostor, "createdMadmateHasImpostorVision", false, evilHackerCanCreateMadmate);
        createdMadmateCanSabotage = CustomOption.Normal(2040, CustomOptionType.Impostor, "createdMadmateCanSabotage", false, evilHackerCanCreateMadmate);
        createdMadmateCanFixComm = CustomOption.Normal(2041, CustomOptionType.Impostor, "createdMadmateCanFixComm", true, evilHackerCanCreateMadmate);
        createdMadmateAbility = CustomOption.Normal(2042, CustomOptionType.Impostor, "madmateAbility", ["madmateNone", "madmateFanatic"], evilHackerCanCreateMadmate);
        createdMadmateNumTasks = CustomOption.Normal(2043, CustomOptionType.Impostor, "createdMadmateNumTasks", 4f, 1f, 20f, 1f, createdMadmateAbility);
        createdMadmateExileCrewmate = CustomOption.Normal(2044, CustomOptionType.Impostor, "createdMadmateExileCrewmate", false, evilHackerCanCreateMadmate);

        evilTrackerSpawnRate = new(2050, CustomOptionType.Impostor, RoleType.EvilTracker, EvilTracker.RoleColor, 3);
        evilTrackerCooldown = CustomOption.Normal(2051, CustomOptionType.Impostor, "evilTrackerCooldown", 10f, 0f, 60f, 1f, evilTrackerSpawnRate);
        evilTrackerResetTargetAfterMeeting = CustomOption.Normal(2052, CustomOptionType.Impostor, "evilTrackerResetTargetAfterMeeting", true, evilTrackerSpawnRate);
        evilTrackerCanSeeDeathFlash = CustomOption.Normal(2053, CustomOptionType.Impostor, "evilTrackerCanSeeDeathFlash", true, evilTrackerSpawnRate);
        evilTrackerCanSeeTargetTask = CustomOption.Normal(2054, CustomOptionType.Impostor, "evilTrackerCanSeeTargetTask", true, evilTrackerSpawnRate);
        evilTrackerCanSeeTargetPosition = CustomOption.Normal(2055, CustomOptionType.Impostor, "evilTrackerCanSeeTargetPosition", true, evilTrackerSpawnRate);
        evilTrackerCanSetTargetOnMeeting = CustomOption.Normal(2056, CustomOptionType.Impostor, "evilTrackerCanSetTargetOnMeeting", true, evilTrackerSpawnRate);

        eraserSpawnRate = new(2060, CustomOptionType.Impostor, RoleType.Eraser, Eraser.RoleColor, 1);
        eraserCooldown = CustomOption.Normal(2061, CustomOptionType.Impostor, "eraserCooldown", 30f, 5f, 120f, 5f, eraserSpawnRate, format: "unitSeconds");
        eraserCooldownIncrease = CustomOption.Normal(2062, CustomOptionType.Impostor, "eraserCooldownIncrease", 10f, 0f, 120f, 2.5f, eraserSpawnRate, format: "unitSeconds");
        eraserCanEraseAnyone = CustomOption.Normal(2063, CustomOptionType.Impostor, "eraserCanEraseAnyone", false, eraserSpawnRate);
        #endregion

        #region ROLES NEUTRAL
        JesterSpawnRate = new(3000, CustomOptionType.Neutral, RoleType.Jester, Jester.RoleColor, 1);
        JesterCanCallEmergency = CustomOption.Normal(3001, CustomOptionType.Neutral, "", true, JesterSpawnRate);
        JesterCanSabotage = CustomOption.Normal(3002, CustomOptionType.Neutral, "", true, JesterSpawnRate);
        JesterHasImpostorVision = CustomOption.Normal(3003, CustomOptionType.Neutral, "", false, JesterSpawnRate);

        ArsonistSpawnRate = new(3010, CustomOptionType.Neutral, RoleType.Arsonist, Arsonist.RoleColor, 1);
        ArsonistCooldown = CustomOption.Normal(3011, CustomOptionType.Neutral, "", 12.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
        ArsonistDuration = CustomOption.Normal(3012, CustomOptionType.Neutral, "", 3f, 0f, 10f, 1f, ArsonistSpawnRate);
        ArsonistCanBeLovers = CustomOption.Normal(3013, CustomOptionType.Neutral, "", false, ArsonistSpawnRate);

        VultureSpawnRate = new(3020, CustomOptionType.Neutral, RoleType.Vulture, Vulture.RoleColor, 1);
        VultureCooldown = CustomOption.Normal(3021, CustomOptionType.Neutral, "", 15f, 2.5f, 60f, 2.5f, VultureSpawnRate);
        VultureNumberToWin = CustomOption.Normal(3022, CustomOptionType.Neutral, "", 4f, 1f, 12f, 1f, VultureSpawnRate);
        VultureCanUseVents = CustomOption.Normal(3023, CustomOptionType.Neutral, "", true, VultureSpawnRate);
        VultureShowArrows = CustomOption.Normal(3024, CustomOptionType.Neutral, "", true, VultureSpawnRate);

        JackalSpawnRate = new(3030, CustomOptionType.Neutral, RoleType.Jackal, Jackal.RoleColor, 1);
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

        guesserSpawnRate = new(3050, CustomOptionType.Neutral, nameof(Guesser), Guesser.NiceGuesser.RoleColor, 1);
        guesserIsImpGuesserRate = CustomOption.Normal(3051, CustomOptionType.Neutral, "guesserIsImpGuesserRate", RATES, guesserSpawnRate);
        guesserSpawnBothRate = CustomOption.Normal(3052, CustomOptionType.Neutral, "guesserSpawnBothRate", RATES, guesserSpawnRate);
        guesserNumberOfShots = CustomOption.Normal(3053, CustomOptionType.Neutral, "guesserNumberOfShots", 2f, 1f, 15f, 1f, guesserSpawnRate);
        guesserOnlyAvailableRoles = CustomOption.Normal(3054, CustomOptionType.Neutral, "guesserOnlyAvailableRoles", true, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting = CustomOption.Normal(3055, CustomOptionType.Neutral, "guesserHasMultipleShotsPerMeeting", false, guesserSpawnRate);
        guesserShowInfoInGhostChat = CustomOption.Normal(3056, CustomOptionType.Neutral, "guesserToGhostChat", true, guesserSpawnRate);
        guesserKillsThroughShield = CustomOption.Normal(3057, CustomOptionType.Neutral, "guesserPierceShield", true, guesserSpawnRate);
        guesserEvilCanKillSpy = CustomOption.Normal(3058, CustomOptionType.Neutral, "guesserEvilCanKillSpy", true, guesserSpawnRate);
        #endregion

        #region MODIFIERS
        madmateSpawnRate = new(4000, CustomOptionType.Modifier, ModifierType.Madmate, Madmate.ModifierColor);
        madmateType = CustomOption.Normal(4001, CustomOptionType.Modifier, "madmateType", ["madmateDefault", "madmateWithRole", "madmateRandom"], madmateSpawnRate);
        madmateFixedRole = new CustomRoleSelectionOption(4002, CustomOptionType.Modifier, "madmateFixedRole", Madmate.validRoles, madmateType);
        madmateAbility = CustomOption.Normal(4003, CustomOptionType.Modifier, "madmateAbility", ["madmateNone", "madmateFanatic"], madmateSpawnRate);
        madmateTasks = new((4004, 4005, 4006), CustomOptionType.Modifier, (1, 1, 3), madmateAbility);
        madmateCanDieToSheriff = CustomOption.Normal(4007, CustomOptionType.Modifier, "madmateCanDieToSheriff", false, madmateSpawnRate);
        madmateCanEnterVents = CustomOption.Normal(4008, CustomOptionType.Modifier, "madmateCanEnterVents", false, madmateSpawnRate);
        madmateHasImpostorVision = CustomOption.Normal(4009, CustomOptionType.Modifier, "madmateHasImpostorVision", false, madmateSpawnRate);
        madmateCanSabotage = CustomOption.Normal(4010, CustomOptionType.Modifier, "madmateCanSabotage", false, madmateSpawnRate);
        madmateCanFixComm = CustomOption.Normal(4011, CustomOptionType.Modifier, "madmateCanFixComm", true, madmateSpawnRate);
        madmateExilePlayer = CustomOption.Normal(4012, CustomOptionType.Modifier, "madmateExileCrewmate", false, madmateSpawnRate);
        #endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }
}