using RebuildUs.Modules;
using RebuildUs.Modules.CustomOptions;
using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
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
    #endregion

    #region MODIFIERS
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
        #endregion

        #region MODIFIERS
        #endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }
}