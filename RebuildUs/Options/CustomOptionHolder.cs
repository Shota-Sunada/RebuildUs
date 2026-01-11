using RebuildUs.Modules;
using RebuildUs.Modules.CustomOptions;

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
    public static CustomOption BlockSkippingInEmergencyMeeting;
    public static CustomOption NoVoteIsSelfVote;
    public static CustomOption HidePlayerName;
    public static CustomOption AllowParallelMedBayScans;
    public static CustomOption HideOutOfSightNametags;
    public static CustomOption RefundVotesOnDeath;
    public static CustomOption DelayBeforeMeeting;
    public static CustomOption RandomWireTask;
    public static CustomOption NumWireTask;
    public static CustomOption DisableVentAnimation;
    public static CustomOption RestrictDevices;
    public static CustomOption RestrictAdmin;
    public static CustomOption RestrictAdminTime;
    public static CustomOption RestrictAdminText;
    public static CustomOption RestrictCamerasTime;
    public static CustomOption RestrictCamerasText;
    public static CustomOption RestrictVitalsTime;
    public static CustomOption RestrictVitalsText;
    public static CustomOption StopCooldownOnFixingElecSabotage;
    public static CustomOption AdditionalEmergencyCooldown;
    public static CustomOption AdditionalEmergencyCooldownTime;
    public static CustomOption EnableHawkMode;
    public static CustomOption CanWinByTaskWithoutLivingPlayer;
    public static CustomOption DeadPlayerCanSeeCooldown;
    public static CustomOption ImpostorCanIgnoreCommSabotage;
    public static CustomOption BlockSabotageFromDeadImpostors;
    public static CustomOption ShieldFirstKill;
    public static CustomOption FinishTasksBeforeHauntingOrZoomingOut;
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
    public static CustomRoleOption mayorSpawnRate;
    public static CustomOption mayorNumVotes;
    public static CustomOption mayorCanSeeVoteColors;
    public static CustomOption mayorTasksNeededToSeeVoteColors;
    public static CustomOption mayorMeetingButton;
    public static CustomOption mayorMaxRemoteMeetings;

    public static CustomRoleOption engineerSpawnRate;
    public static CustomOption engineerNumberOfFixes;
    public static CustomOption engineerHighlightForImpostors;
    public static CustomOption engineerHighlightForTeamJackal;

    public static CustomRoleOption jackalSpawnRate;
    public static CustomOption jackalKillCooldown;
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalCanSabotageLights;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption jackalHasImpostorVision;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption sidekickCanSabotageLights;
    public static CustomOption sidekickHasImpostorVision;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
    public static CustomOption jackalCanCreateSidekickFromImpostor;
    #endregion

    #region ROLES IMPOSTOR
    public static CustomRoleOption bountyHunterSpawnRate;
    public static CustomOption bountyHunterBountyDuration;
    public static CustomOption bountyHunterReducedCooldown;
    public static CustomOption bountyHunterPunishmentTime;
    public static CustomOption bountyHunterShowArrow;
    public static CustomOption bountyHunterArrowUpdateInterval;
    #endregion

    #region ROLES NEUTRAL
    public static CustomRoleOption jesterSpawnRate;
    public static CustomOption jesterCanCallEmergency;
    public static CustomOption jesterCanSabotage;
    public static CustomOption jesterHasImpostorVision;

    public static CustomRoleOption arsonistSpawnRate;
    public static CustomOption arsonistCooldown;
    public static CustomOption arsonistDuration;
    public static CustomOption arsonistCanBeLovers;

    public static CustomRoleOption vultureSpawnRate;
    public static CustomOption vultureCooldown;
    public static CustomOption vultureNumberToWin;
    public static CustomOption vultureCanUseVents;
    public static CustomOption vultureShowArrows;
    #endregion

    #region MODIFIERS
    #endregion

    internal static Dictionary<byte, byte[]> blockedRolePairings = [];

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
        BlockSkippingInEmergencyMeeting = CustomOption.Normal(21, CustomOptionType.General, "BlockSkippingInEmergencyMeeting", false);
        NoVoteIsSelfVote = CustomOption.Normal(22, CustomOptionType.General, "NoVoteIsSelfVote", false);
        HidePlayerName = CustomOption.Normal(23, CustomOptionType.General, "HidePlayerName", false);
        AllowParallelMedBayScans = CustomOption.Normal(24, CustomOptionType.General, "AllowParallelMedBayScans", false);
        HideOutOfSightNametags = CustomOption.Normal(25, CustomOptionType.General, "HideOutOfSightNametags", true);
        RefundVotesOnDeath = CustomOption.Normal(26, CustomOptionType.General, "RefundVotesOnDeath", true);
        DelayBeforeMeeting = CustomOption.Normal(27, CustomOptionType.General, "DelayBeforeMeeting", true);
        DisableVentAnimation = CustomOption.Normal(28, CustomOptionType.General, "DisableVentAnimation", false);
        StopCooldownOnFixingElecSabotage = CustomOption.Normal(29, CustomOptionType.General, "StopCooldownOnFixingElecSabotage", true);
        EnableHawkMode = CustomOption.Normal(30, CustomOptionType.General, "EnableHawkMode", true);
        CanWinByTaskWithoutLivingPlayer = CustomOption.Normal(31, CustomOptionType.General, "CanWinByTaskLivingPlayer", true);
        DeadPlayerCanSeeCooldown = CustomOption.Normal(32, CustomOptionType.General, "DeadPlayerCanSeeCooldown", true);
        ImpostorCanIgnoreCommSabotage = CustomOption.Normal(33, CustomOptionType.General, "ImpostorCanIgnoreCommSabotage", false);
        BlockSabotageFromDeadImpostors = CustomOption.Normal(34, CustomOptionType.General, "BlockSabotageFromDeadImpostors", false);
        ShieldFirstKill = CustomOption.Normal(35, CustomOptionType.General, "ShieldFirstKill", false);

        RandomWireTask = CustomOption.Normal(50, CustomOptionType.General, "RandomWireTask", false);
        NumWireTask = CustomOption.Normal(51, CustomOptionType.General, "NumWireTask", 3f, 1f, 10f, 1f, RandomWireTask);

        AdditionalEmergencyCooldown = CustomOption.Normal(55, CustomOptionType.General, "AdditionalEmergencyCooldown", 0f, 0f, 15f, 1f);
        AdditionalEmergencyCooldownTime = CustomOption.Normal(56, CustomOptionType.General, "AdditionalEmergencyCooldownTime", 10f, 0f, 60f, 1f, AdditionalEmergencyCooldown);

        RestrictDevices = CustomOption.Normal(60, CustomOptionType.General, "RestrictDevices", ["Off", "RestrictPerTurn", "RestrictPerGame"]);
        RestrictAdmin = CustomOption.Normal(61, CustomOptionType.General, "RestrictAdmin", true, RestrictDevices);
        RestrictAdminTime = CustomOption.Normal(62, CustomOptionType.General, "RestrictAdminTime", true, RestrictDevices);
        RestrictAdminText = CustomOption.Normal(63, CustomOptionType.General, "RestrictAdminText", true, RestrictDevices);
        RestrictCamerasTime = CustomOption.Normal(64, CustomOptionType.General, "RestrictCamerasTime", true, RestrictDevices);
        RestrictCamerasText = CustomOption.Normal(65, CustomOptionType.General, "RestrictCamerasText", true, RestrictDevices);
        RestrictVitalsTime = CustomOption.Normal(66, CustomOptionType.General, "RestrictVitalsTime", true, RestrictDevices);
        RestrictVitalsText = CustomOption.Normal(67, CustomOptionType.General, "RestrictVitalsText", true, RestrictDevices);
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
        mayorSpawnRate = new(1000, CustomOptionType.Crewmate, RoleInfo.Mayor);
        mayorNumVotes = CustomOption.Normal(1001, CustomOptionType.Crewmate, "", 2f, 2f, 10f, 1f, mayorSpawnRate);
        mayorCanSeeVoteColors = CustomOption.Normal(1002, CustomOptionType.Crewmate, "", false, mayorSpawnRate);
        mayorTasksNeededToSeeVoteColors = CustomOption.Normal(1003, CustomOptionType.Crewmate, "", true, mayorCanSeeVoteColors);
        mayorMeetingButton = CustomOption.Normal(1004, CustomOptionType.Crewmate, "", true, mayorSpawnRate);
        mayorMaxRemoteMeetings = CustomOption.Normal(1005, CustomOptionType.Crewmate, "", 1f, 0f, 10f, 1f, mayorMeetingButton);

        engineerSpawnRate = new(1010, CustomOptionType.Crewmate, RoleInfo.Engineer);
        engineerNumberOfFixes = CustomOption.Normal(1011, CustomOptionType.Crewmate, "", 1f, 0f, 3f, 1f, engineerSpawnRate);
        engineerHighlightForImpostors = CustomOption.Normal(1012, CustomOptionType.Crewmate, "", true, engineerSpawnRate);
        engineerHighlightForTeamJackal = CustomOption.Normal(1013, CustomOptionType.Crewmate, "", true, engineerSpawnRate);
        #endregion

        #region ROLES IMPOSTOR
        bountyHunterSpawnRate = new(2000, CustomOptionType.Impostor, RoleInfo.BountyHunter, 1);
        bountyHunterBountyDuration = CustomOption.Normal(2001, CustomOptionType.Impostor, "", 60f, 10f, 180f, 10f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = CustomOption.Normal(20002, CustomOptionType.Impostor, "", 2.5f, 2.5f, 30f, 2.5f, bountyHunterSpawnRate);
        bountyHunterPunishmentTime = CustomOption.Normal(2003, CustomOptionType.Impostor, "", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = CustomOption.Normal(2004, CustomOptionType.Impostor, "", true, bountyHunterShowArrow);
        bountyHunterArrowUpdateInterval = CustomOption.Normal(2005, CustomOptionType.Impostor, "", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow);
        #endregion

        #region ROLES NEUTRAL
        jesterSpawnRate = new(3000, CustomOptionType.Neutral, RoleInfo.Jester, 1);
        jesterCanCallEmergency = CustomOption.Normal(3001, CustomOptionType.Neutral, "", true, jesterSpawnRate);
        jesterCanSabotage = CustomOption.Normal(3002, CustomOptionType.Neutral, "", true, jesterSpawnRate);
        jesterHasImpostorVision = CustomOption.Normal(3003, CustomOptionType.Neutral, "", false, jesterSpawnRate);

        arsonistSpawnRate = new(3010, CustomOptionType.Neutral, RoleInfo.Arsonist, 1);
        arsonistCooldown = CustomOption.Normal(3011, CustomOptionType.Neutral, "", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = CustomOption.Normal(3012, CustomOptionType.Neutral, "", 3f, 0f, 10f, 1f, arsonistSpawnRate);
        arsonistCanBeLovers = CustomOption.Normal(3013, CustomOptionType.Neutral, "", false, arsonistSpawnRate);

        vultureSpawnRate = new(3020, CustomOptionType.Neutral, RoleInfo.Vulture, 1);
        vultureCooldown = CustomOption.Normal(3021, CustomOptionType.Neutral, "", 15f, 2.5f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = CustomOption.Normal(3022, CustomOptionType.Neutral, "", 4f, 1f, 12f, 1f, vultureSpawnRate);
        vultureCanUseVents = CustomOption.Normal(3023, CustomOptionType.Neutral, "", true, vultureSpawnRate);
        vultureShowArrows = CustomOption.Normal(3024, CustomOptionType.Neutral, "", true, vultureSpawnRate);

        jackalSpawnRate = new(3030, CustomOptionType.Neutral, RoleInfo.Jackal, 1);
        jackalKillCooldown = CustomOption.Normal(3031, CustomOptionType.Neutral, "", 30f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalCanSabotageLights = CustomOption.Normal(3032, CustomOptionType.Neutral, "", true, jackalSpawnRate);
        jackalCanUseVents = CustomOption.Normal(3033, CustomOptionType.Neutral, "", true, jackalSpawnRate);
        jackalHasImpostorVision = CustomOption.Normal(3034, CustomOptionType.Neutral, "", false, jackalSpawnRate);
        jackalCanCreateSidekick = CustomOption.Normal(3035, CustomOptionType.Neutral, "", false, jackalSpawnRate);
        jackalCreateSidekickCooldown = CustomOption.Normal(3036, CustomOptionType.Neutral, "", 30f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        sidekickCanKill = CustomOption.Normal(3038, CustomOptionType.Neutral, "", false, jackalCanCreateSidekick);
        sidekickCanUseVents = CustomOption.Normal(3039, CustomOptionType.Neutral, "", true, jackalCanCreateSidekick);
        sidekickCanSabotageLights = CustomOption.Normal(3040, CustomOptionType.Neutral, "", true, jackalCanCreateSidekick);
        sidekickHasImpostorVision = CustomOption.Normal(3041, CustomOptionType.Neutral, "", false, jackalCanCreateSidekick);
        sidekickPromotesToJackal = CustomOption.Normal(3037, CustomOptionType.Neutral, "", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Normal(3042, CustomOptionType.Neutral, "", false, sidekickPromotesToJackal);
        jackalCanCreateSidekickFromImpostor = CustomOption.Normal(3043, CustomOptionType.Neutral, "", false, jackalCanCreateSidekick);

        #endregion

        #region MODIFIERS
        #endregion

        blockedRolePairings.Add((byte)ERoleType.Vulture, [(byte)ERoleType.Cleaner]);
        blockedRolePairings.Add((byte)ERoleType.Cleaner, [(byte)ERoleType.Vulture]);
    }
}