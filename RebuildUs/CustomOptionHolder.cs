namespace RebuildUs;

public static class CustomOptionHolder
{
    public static readonly string[] RATES = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];

    public static readonly string[] PRESETS = [Tr.Get(TrKey.Preset1), Tr.Get(TrKey.Preset2), Tr.Get(TrKey.Preset3), Tr.Get(TrKey.Preset4), Tr.Get(TrKey.Preset5)];

#region SKELD OPTIONS

    public static CustomOption CustomSkeldMap;

#endregion

    internal static Dictionary<byte, byte[]> BlockedRolePairings = [];

    public static void Load()
    {
#region MOD OPTIONS

        PresetSelection = CustomOption.Header(0, CustomOptionType.General, TrKey.Preset, PRESETS, TrKey.Preset);
        ActivateRoles = CustomOption.Normal(1, CustomOptionType.General, TrKey.ActivateRoles, true);
        EnableRandomRandomNumberAlgorithm = CustomOption.Normal(2, CustomOptionType.General, TrKey.RandomRandomNumberAlgorithm, false);
        RandomNumberAlgorithm = CustomOption.Normal(3, CustomOptionType.General, TrKey.RandomNumberAlgorithm, [Tr.Get(TrKey.RND_Dotnet), Tr.Get(TrKey.RND_MT), Tr.Get(TrKey.RND_XOSHIRO256), Tr.Get(TrKey.RND_XOSHIRO256SS), Tr.Get(TrKey.RND_PCG64)], EnableRandomRandomNumberAlgorithm, 0, 0, 0, 0, true);
        EnableRandomRandomNumberAlgorithmDotnet = CustomOption.Normal(4, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmDotnet, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmMT = CustomOption.Normal(5, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmMT, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Pp = CustomOption.Normal(6, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256PP, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Ss = CustomOption.Normal(7, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256SS, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmPcg64 = CustomOption.Normal(8, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmPcg64, true, EnableRandomRandomNumberAlgorithm);

#endregion

#region GAME MODE

        GameMode = CustomOption.Normal(500, CustomOptionType.General, TrKey.GameMode, [Tr.Get(TrKey.GameModeRoles), Tr.Get(TrKey.GameModeCaptureTheFlag), Tr.Get(TrKey.GameModePoliceAndThief), Tr.Get(TrKey.GameModeHotPotato), Tr.Get(TrKey.GameModeBattleRoyale)]);

        GamemodeSettings = CustomOption.Normal(510, CustomOptionType.General, TrKey.GameModeGlobalSettings, true);
        GamemodeMatchDuration = CustomOption.Normal(511, CustomOptionType.General, TrKey.MatchDuration, 180f, 180f, 420f, 30f, GamemodeSettings);
        GamemodeKillCooldown = CustomOption.Normal(512, CustomOptionType.General, TrKey.KillCooldown, 10f, 10f, 20f, 1f, GamemodeSettings);
        GamemodeEnableFlashlight = CustomOption.Normal(513, CustomOptionType.General, TrKey.EnableFlashlightIfPossible, true, GamemodeSettings);
        GamemodeFlashlightRange = CustomOption.Normal(514, CustomOptionType.General, TrKey.FlashlightRange, 0.8f, 0.6f, 1.2f, 0.2f, GamemodeSettings);
        GamemodeReviveTime = CustomOption.Normal(515, CustomOptionType.General, TrKey.ReviveWaitTime, 8f, 7f, 15f, 1f, GamemodeSettings);
        GamemodeInvincibilityTimeAfterRevive = CustomOption.Normal(516, CustomOptionType.General, TrKey.InvincibilityTimeAfterRevive, 3f, 2f, 5f, 1f, GamemodeSettings);

        RequiredFlags = CustomOption.Normal(520, CustomOptionType.General, TrKey.CaptureTheFlagScoreNumber, 3f, 3f, 5f, 1f, GamemodeSettings);

        ThiefModerequiredJewels = CustomOption.Normal(530, CustomOptionType.General, TrKey.PoliceAndThievesJewelNumber, 15f, 8f, 15f, 1f, GamemodeSettings);
        ThiefModePoliceCatchCooldown = CustomOption.Normal(531, CustomOptionType.General, TrKey.PoliceAndThievesArrestCooldown, 10f, 5f, 15f, 1f, GamemodeSettings);
        ThiefModecaptureThiefTime = CustomOption.Normal(532, CustomOptionType.General, TrKey.PoliceAndThievesTimeToArrest, 3f, 2f, 3f, 1f, GamemodeSettings);
        ThiefModePoliceTaseCooldown = CustomOption.Normal(533, CustomOptionType.General, TrKey.PoliceAndThievesTaseCooldown, 15f, 10f, 15f, 1f, GamemodeSettings);
        ThiefModePoliceTaseDuration = CustomOption.Normal(534, CustomOptionType.General, TrKey.PoliceAndThievesTaseDuration, 3f, 3f, 5f, 1f, GamemodeSettings);
        ThiefModePoliceCanSeeJewels = CustomOption.Normal(535, CustomOptionType.General, TrKey.PoliceAndThievesPoliceCanSeeJewels, false, GamemodeSettings);
        ThiefModeWhoCanThiefsKill = CustomOption.Normal(536, CustomOptionType.General, TrKey.PoliceAndThievesWhoCanThievesKill, new[] { Tr.Get(TrKey.PoliceAndThievesTaser), Tr.Get(TrKey.PoliceAndThievesAll), Tr.Get(TrKey.PoliceAndThievesNobody) }, GamemodeSettings);

        HotPotatoTransferLimit = CustomOption.Normal(540, CustomOptionType.General, TrKey.HotPotatoTimeLimitForTransfer, 20f, 10f, 30f, 1f, GamemodeSettings);
        HotPotatoCooldown = CustomOption.Normal(541, CustomOptionType.General, TrKey.HotPotatoTransferCooldown, 5f, 5f, 10f, 1f, GamemodeSettings);
        HotPotatoResetTimeForTransfer = CustomOption.Normal(542, CustomOptionType.General, TrKey.HotPotatoResetTimerAfterTransfer, true, GamemodeSettings);
        HotPotatoIncreaseTimeIfNoReset = CustomOption.Normal(543, CustomOptionType.General, TrKey.HotPotatoExtraTimeWhenTimerDoesntReset, 10f, 10f, 15f, 1f, GamemodeSettings);

        BattleRoyaleMatchType = CustomOption.Normal(550, CustomOptionType.General, TrKey.BattleRoyaleMatchType, new[] { Tr.Get(TrKey.BattleRoyaleAllVsAll), Tr.Get(TrKey.BattleRoyaleTeamBattle), Tr.Get(TrKey.BattleRoyaleScoreBattle) }, GamemodeSettings);
        BattleRoyaleKillCooldown = CustomOption.Normal(551, CustomOptionType.General, TrKey.BattleRoyaleShootCooldown, 1f, 1f, 3f, 1f, GamemodeSettings);
        BattleRoyaleLifes = CustomOption.Normal(552, CustomOptionType.General, TrKey.BattleRoyaleFighterLives, 3f, 3f, 10f, 1f, GamemodeSettings);
        BattleRoyaleScoreNeeded = CustomOption.Normal(553, CustomOptionType.General, TrKey.BattleRoyaleScoreNumber, 200f, 100f, 300f, 10f, GamemodeSettings);

#endregion

#region GENERAL OPTIONS

        CrewmateRolesCountMin = CustomOption.Header(10, CustomOptionType.General, TrKey.CrewmateRolesCountMin, 0f, 0f, 15f, 1f, TrKey.RolesGeneral);
        CrewmateRolesCountMax = CustomOption.Normal(11, CustomOptionType.General, TrKey.CrewmateRolesCountMax, 0f, 0f, 15f, 1f);
        ImpostorRolesCountMin = CustomOption.Normal(12, CustomOptionType.General, TrKey.ImpostorRolesCountMin, 0f, 0f, 15f, 1f);
        ImpostorRolesCountMax = CustomOption.Normal(13, CustomOptionType.General, TrKey.ImpostorRolesCountMax, 0f, 0f, 15f, 1f);
        NeutralRolesCountMin = CustomOption.Normal(14, CustomOptionType.General, TrKey.NeutralRolesCountMin, 0f, 0f, 15f, 1f);
        NeutralRolesCountMax = CustomOption.Normal(15, CustomOptionType.General, TrKey.NeutralRolesCountMax, 0f, 0f, 15f, 1f);
        ModifiersCountMin = CustomOption.Normal(16, CustomOptionType.General, TrKey.ModifiersCountMin, 0f, 0f, 15f, 1f);
        ModifiersCountMax = CustomOption.Normal(17, CustomOptionType.General, TrKey.ModifiersCountMax, 0f, 0f, 15f, 1f);

#endregion

#region GAME OPTIONS

        GameOptions = CustomOption.Header(19, CustomOptionType.General, TrKey.GameOptions, true, TrKey.GameOptions);
        MaxNumberOfMeetings = CustomOption.Normal(20, CustomOptionType.General, TrKey.MaxNumberOfMeetings, 10, 0, 15, 1);
        BlockSkippingInEmergencyMeetings = CustomOption.Normal(21, CustomOptionType.General, TrKey.BlockSkippingInEmergencyMeetings, false);
        NoVoteIsSelfVote = CustomOption.Normal(22, CustomOptionType.General, TrKey.NoVoteIsSelfVote, false);
        HidePlayerNames = CustomOption.Normal(23, CustomOptionType.General, TrKey.HidePlayerNames, false);
        AllowParallelMedBayScans = CustomOption.Normal(24, CustomOptionType.General, TrKey.AllowParallelMedBayScans, false);
        HideOutOfSightNametags = CustomOption.Normal(25, CustomOptionType.General, TrKey.HideOutOfSightNametags, true);
        RefundVotesOnDeath = CustomOption.Normal(26, CustomOptionType.General, TrKey.RefundVotesOnDeath, true);
        DelayBeforeMeeting = CustomOption.Normal(27, CustomOptionType.General, TrKey.DelayBeforeMeeting, 0f, 0f, 10f, 0.25f);
        DisableVentAnimation = CustomOption.Normal(28, CustomOptionType.General, TrKey.DisableVentAnimation, false);
        StopCooldownOnFixingElecSabotage = CustomOption.Normal(29, CustomOptionType.General, TrKey.StopCooldownOnFixingElecSabotage, true);
        EnableHawkMode = CustomOption.Normal(30, CustomOptionType.General, TrKey.EnableHawkMode, true);
        CanWinByTaskWithoutLivingPlayer = CustomOption.Normal(31, CustomOptionType.General, TrKey.CanWinByTaskLivingPlayer, true);
        // DeadPlayerCanSeeCooldown = CustomOption.Normal(32, CustomOptionType.General, "DeadPlayerCanSeeCooldown", true);
        ImpostorCanIgnoreCommSabotage = CustomOption.Normal(33, CustomOptionType.General, TrKey.ImpostorCanIgnoreCommSabotage, false);
        // BlockSabotageFromDeadImpostors = CustomOption.Normal(34, CustomOptionType.General, "BlockSabotageFromDeadImpostors", false);
        // ShieldFirstKill = CustomOption.Normal(35, CustomOptionType.General, "ShieldFirstKill", false);

        AdditionalEmergencyCooldown = CustomOption.Normal(55, CustomOptionType.General, TrKey.AdditionalEmergencyCooldown, 0f, 0f, 15f, 1f);
        AdditionalEmergencyCooldownTime = CustomOption.Normal(56, CustomOptionType.General, TrKey.AdditionalEmergencyCooldownTime, 10f, 0f, 60f, 1f, AdditionalEmergencyCooldown);

        RestrictDevices = CustomOption.Normal(60, CustomOptionType.General, TrKey.RestrictDevices, [Tr.Get(TrKey.Off), Tr.Get(TrKey.RestrictPerTurn), Tr.Get(TrKey.RestrictPerGame)]);
        RestrictAdmin = CustomOption.Normal(61, CustomOptionType.General, TrKey.RestrictAdmin, true, RestrictDevices);
        RestrictAdminTime = CustomOption.Normal(62, CustomOptionType.General, TrKey.RestrictAdminTime, 30f, 0f, 600f, 1f, RestrictAdmin);
        RestrictAdminText = CustomOption.Normal(63, CustomOptionType.General, TrKey.RestrictAdminText, true, RestrictAdmin);
        RestrictCameras = CustomOption.Normal(64, CustomOptionType.General, TrKey.RestrictCameras, true, RestrictDevices);
        RestrictCamerasTime = CustomOption.Normal(65, CustomOptionType.General, TrKey.RestrictCamerasTime, 30f, 0f, 600f, 1f, RestrictCameras);
        RestrictCamerasText = CustomOption.Normal(66, CustomOptionType.General, TrKey.RestrictCamerasText, true, RestrictCameras);
        RestrictVitals = CustomOption.Normal(67, CustomOptionType.General, TrKey.RestrictVitals, true, RestrictDevices);
        RestrictVitalsTime = CustomOption.Normal(68, CustomOptionType.General, TrKey.RestrictVitalsTime, 30f, 0f, 600f, 1f, RestrictVitals);
        RestrictVitalsText = CustomOption.Normal(69, CustomOptionType.General, TrKey.RestrictVitalsText, true, RestrictVitals);

#endregion

#region DISCORD OPTIONS

        EnableDiscordAutoMute = CustomOption.Header(110, CustomOptionType.General, TrKey.EnableDiscordAutoMute, false, TrKey.DiscordOptions);
        EnableDiscordEmbed = CustomOption.Normal(111, CustomOptionType.General, TrKey.EnableDiscordEmbed, false);

#endregion

#region SKELD OPTIONS

        CustomSkeldMap = CustomOption.Header(120, CustomOptionType.General, TrKey.CustomSkeldMap, [Tr.Get(TrKey.Normal), Tr.Get(TrKey.Dleks)], TrKey.SkeldOptions);

#endregion

#region POLUS OPTIONS

        PolusAdditionalVents = CustomOption.Header(70, CustomOptionType.General, TrKey.PolusAdditionalVents, true, TrKey.PolusOptions);
        PolusSpecimenVital = CustomOption.Normal(71, CustomOptionType.General, TrKey.PolusSpecimenVital, true);
        PolusRandomSpawn = CustomOption.Normal(72, CustomOptionType.General, TrKey.PolusRandomSpawn, true);

#endregion

#region AIRSHIP OPTIONS

        AirshipOptimize = CustomOption.Header(80, CustomOptionType.General, TrKey.AirshipOptimize, false, TrKey.AirshipOptions);
        AirshipEnableWallCheck = CustomOption.Normal(81, CustomOptionType.General, TrKey.AirshipEnableWallCheck, true);
        AirshipReactorDuration = CustomOption.Normal(82, CustomOptionType.General, TrKey.AirshipReactorDuration, 60f, 0f, 600f, 1f);
        AirshipRandomSpawn = CustomOption.Normal(83, CustomOptionType.General, TrKey.AirshipRandomSpawn, false);
        AirshipAdditionalSpawn = CustomOption.Normal(84, CustomOptionType.General, TrKey.AirshipAdditionalSpawn, true);
        AirshipSynchronizedSpawning = CustomOption.Normal(85, CustomOptionType.General, TrKey.AirshipSynchronizedSpawning, true);
        AirshipSetOriginalCooldown = CustomOption.Normal(86, CustomOptionType.General, TrKey.AirshipSetOriginalCooldown, false);
        AirshipInitialDoorCooldown = CustomOption.Normal(87, CustomOptionType.General, TrKey.AirshipInitialDoorCooldown, 0f, 0f, 60f, 1f);
        AirshipInitialSabotageCooldown = CustomOption.Normal(88, CustomOptionType.General, TrKey.AirshipInitialSabotageCooldown, 15f, 0f, 60f, 1f);
        AirshipOldAdmin = CustomOption.Normal(89, CustomOptionType.General, TrKey.AirshipOldAdmin, false);
        AirshipRestrictedAdmin = CustomOption.Normal(90, CustomOptionType.General, TrKey.AirshipRestrictedAdmin, false);
        AirshipDisableGapSwitchBoard = CustomOption.Normal(91, CustomOptionType.General, TrKey.AirshipDisableGapSwitchBoard, false);
        AirshipDisableMovingPlatform = CustomOption.Normal(92, CustomOptionType.General, TrKey.AirshipDisableMovingPlatform, false);
        AirshipAdditionalLadder = CustomOption.Normal(93, CustomOptionType.General, TrKey.AirshipAdditionalLadder, false);
        AirshipOneWayLadder = CustomOption.Normal(94, CustomOptionType.General, TrKey.AirshipOneWayLadder, false);
        AirshipReplaceSafeTask = CustomOption.Normal(95, CustomOptionType.General, TrKey.AirshipReplaceSafeTask, false);
        AirshipAdditionalWireTask = CustomOption.Normal(96, CustomOptionType.General, TrKey.AirshipAdditionalWireTask, false);

#endregion

#region MAP OPTIONS

        RandomMap = CustomOption.Header(100, CustomOptionType.General, TrKey.RandomMap, false, TrKey.RandomMap);
        RandomMapEnableSkeld = CustomOption.Normal(101, CustomOptionType.General, TrKey.RandomMapEnableSkeld, true, RandomMap);
        RandomMapEnableMiraHq = CustomOption.Normal(102, CustomOptionType.General, TrKey.RandomMapEnableMiraHQ, true, RandomMap);
        RandomMapEnablePolus = CustomOption.Normal(103, CustomOptionType.General, TrKey.RandomMapEnablePolus, true, RandomMap);
        RandomMapEnableAirShip = CustomOption.Normal(104, CustomOptionType.General, TrKey.RandomMapEnableAirShip, true, RandomMap);
        RandomMapEnableFungle = CustomOption.Normal(105, CustomOptionType.General, TrKey.RandomMapEnableFungle, true, RandomMap);
        RandomMapEnableSubmerged = CustomOption.Normal(106, CustomOptionType.General, TrKey.RandomMapEnableSubmerged, true, RandomMap);

#endregion

#region ROLES CREWMATE

        MayorSpawnRate = new(1000, CustomOptionType.Crewmate, RoleType.Mayor, Mayor.NameColor);
        MayorNumVotes = CustomOption.Normal(1001, CustomOptionType.Crewmate, TrKey.MayorNumVotes, 2f, 2f, 10f, 1f, MayorSpawnRate);
        MayorCanSeeVoteColors = CustomOption.Normal(1002, CustomOptionType.Crewmate, TrKey.MayorCanSeeVoteColors, false, MayorSpawnRate);
        MayorTasksNeededToSeeVoteColors = CustomOption.Normal(1003, CustomOptionType.Crewmate, TrKey.MayorTasksNeededToSeeVoteColors, 3f, 1f, 10f, 1f, MayorCanSeeVoteColors);
        MayorMeetingButton = CustomOption.Normal(1004, CustomOptionType.Crewmate, TrKey.MayorMeetingButton, true, MayorSpawnRate);
        MayorMaxRemoteMeetings = CustomOption.Normal(1005, CustomOptionType.Crewmate, TrKey.MayorMaxRemoteMeetings, 1f, 0f, 10f, 1f, MayorMeetingButton);

        EngineerSpawnRate = new(1010, CustomOptionType.Crewmate, RoleType.Engineer, Engineer.NameColor);
        EngineerNumberOfFixes = CustomOption.Normal(1011, CustomOptionType.Crewmate, TrKey.EngineerNumberOfFixes, 1f, 0f, 3f, 1f, EngineerSpawnRate);
        EngineerHighlightForImpostors = CustomOption.Normal(1012, CustomOptionType.Crewmate, TrKey.EngineerHighlightForImpostors, true, EngineerSpawnRate);
        EngineerHighlightForTeamJackal = CustomOption.Normal(1013, CustomOptionType.Crewmate, TrKey.EngineerHighlightForTeamJackal, true, EngineerSpawnRate);

        SpySpawnRate = new(1020, CustomOptionType.Crewmate, RoleType.Spy, Spy.NameColor, 1);
        SpyCanDieToSheriff = CustomOption.Normal(1021, CustomOptionType.Crewmate, TrKey.SpyCanDieToSheriff, false, SpySpawnRate);
        SpyImpostorsCanKillAnyone = CustomOption.Normal(1022, CustomOptionType.Crewmate, TrKey.SpyImpostorsCanKillAnyone, true, SpySpawnRate);
        SpyCanEnterVents = CustomOption.Normal(1023, CustomOptionType.Crewmate, TrKey.SpyCanEnterVents, false, SpySpawnRate);
        SpyHasImpostorVision = CustomOption.Normal(1024, CustomOptionType.Crewmate, TrKey.SpyHasImpostorVision, false, SpySpawnRate);

        MedicSpawnRate = new(1030, CustomOptionType.Crewmate, RoleType.Medic, Medic.NameColor, 1);
        MedicShowShielded = CustomOption.Normal(1031, CustomOptionType.Crewmate, TrKey.MedicShowShielded, [Tr.Get(TrKey.MedicShowShieldedAll), Tr.Get(TrKey.MedicShowShieldedBoth), Tr.Get(TrKey.MedicShowShieldedMedic)], MedicSpawnRate);
        MedicShowAttemptToShielded = CustomOption.Normal(1032, CustomOptionType.Crewmate, TrKey.MedicShowAttemptToShielded, false, MedicSpawnRate);
        MedicSetShieldAfterMeeting = CustomOption.Normal(1033, CustomOptionType.Crewmate, TrKey.MedicSetShieldAfterMeeting, false, MedicSpawnRate);
        MedicShowAttemptToMedic = CustomOption.Normal(1034, CustomOptionType.Crewmate, TrKey.MedicSeesMurderAttempt, false, MedicSpawnRate);

        SeerSpawnRate = new(1040, CustomOptionType.Crewmate, RoleType.Seer, Seer.NameColor, 1);
        SeerMode = CustomOption.Normal(1041, CustomOptionType.Crewmate, TrKey.SeerMode, [Tr.Get(TrKey.SeerModeBoth), Tr.Get(TrKey.SeerModeFlash), Tr.Get(TrKey.SeerModeSouls)], SeerSpawnRate);
        SeerLimitSoulDuration = CustomOption.Normal(1042, CustomOptionType.Crewmate, TrKey.SeerLimitSoulDuration, false, SeerSpawnRate);
        SeerSoulDuration = CustomOption.Normal(1043, CustomOptionType.Crewmate, TrKey.SeerSoulDuration, 15f, 0f, 120f, 5f, SeerLimitSoulDuration);

        TimeMasterSpawnRate = new(1050, CustomOptionType.Crewmate, RoleType.TimeMaster, TimeMaster.NameColor, 1);
        TimeMasterCooldown = CustomOption.Normal(1051, CustomOptionType.Crewmate, TrKey.TimeMasterCooldown, 30f, 2.5f, 120f, 2.5f, TimeMasterSpawnRate);
        TimeMasterRewindTime = CustomOption.Normal(1052, CustomOptionType.Crewmate, TrKey.TimeMasterRewindTime, 3f, 1f, 10f, 1f, TimeMasterSpawnRate);
        TimeMasterShieldDuration = CustomOption.Normal(1053, CustomOptionType.Crewmate, TrKey.TimeMasterShieldDuration, 3f, 1f, 20f, 1f, TimeMasterSpawnRate);

        DetectiveSpawnRate = new(1060, CustomOptionType.Crewmate, RoleType.Detective, Detective.NameColor, 1);
        DetectiveAnonymousFootprints = CustomOption.Normal(1061, CustomOptionType.Crewmate, TrKey.DetectiveAnonymousFootprints, false, DetectiveSpawnRate);
        DetectiveFootprintInterval = CustomOption.Normal(1062, CustomOptionType.Crewmate, TrKey.DetectiveFootprintInterval, 0.5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveFootprintDuration = CustomOption.Normal(1063, CustomOptionType.Crewmate, TrKey.DetectiveFootprintDuration, 5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate);
        DetectiveReportNameDuration = CustomOption.Normal(1064, CustomOptionType.Crewmate, TrKey.DetectiveReportNameDuration, 10f, 0, 60, 2.5f, DetectiveSpawnRate);
        DetectiveReportColorDuration = CustomOption.Normal(1065, CustomOptionType.Crewmate, TrKey.DetectiveReportColorDuration, 20, 0, 120, 2.5f, DetectiveSpawnRate);

        MediumSpawnRate = new(1070, CustomOptionType.Crewmate, RoleType.Medium, Medium.NameColor, 1);
        MediumCooldown = CustomOption.Normal(1071, CustomOptionType.Crewmate, TrKey.MediumCooldown, 30f, 5f, 120f, 5f, MediumSpawnRate);
        MediumDuration = CustomOption.Normal(1072, CustomOptionType.Crewmate, TrKey.MediumDuration, 3f, 0f, 15f, 1f, MediumSpawnRate);
        MediumOneTimeUse = CustomOption.Normal(1073, CustomOptionType.Crewmate, TrKey.MediumOneTimeUse, false, MediumSpawnRate);

        HackerSpawnRate = new(1080, CustomOptionType.Crewmate, RoleType.Hacker, Hacker.NameColor, 1);
        HackerCooldown = CustomOption.Normal(1081, CustomOptionType.Crewmate, TrKey.HackerCooldown, 30f, 5f, 60f, 5f, HackerSpawnRate);
        HackerHackingDuration = CustomOption.Normal(1082, CustomOptionType.Crewmate, TrKey.HackerHackingDuration, 10f, 2.5f, 60f, 2.5f, HackerSpawnRate);
        HackerOnlyColorType = CustomOption.Normal(1083, CustomOptionType.Crewmate, TrKey.HackerOnlyColorType, false, HackerSpawnRate);
        HackerToolsNumber = CustomOption.Normal(1084, CustomOptionType.Crewmate, TrKey.HackerToolsNumber, 5f, 1f, 30f, 1f, HackerSpawnRate);
        HackerRechargeTasksNumber = CustomOption.Normal(1085, CustomOptionType.Crewmate, TrKey.HackerRechargeTasksNumber, 2f, 1f, 5f, 1f, HackerSpawnRate);
        HackerNoMove = CustomOption.Normal(1086, CustomOptionType.Crewmate, TrKey.HackerNoMove, true, HackerSpawnRate);

        TrackerSpawnRate = new(1090, CustomOptionType.Crewmate, RoleType.Tracker, Tracker.NameColor, 1);
        TrackerUpdateInterval = CustomOption.Normal(1091, CustomOptionType.Crewmate, TrKey.TrackerUpdateInterval, 5f, 1f, 30f, 1f, TrackerSpawnRate);
        TrackerResetTargetAfterMeeting = CustomOption.Normal(1092, CustomOptionType.Crewmate, TrKey.TrackerResetTargetAfterMeeting, false, TrackerSpawnRate);
        TrackerCanTrackCorpses = CustomOption.Normal(1093, CustomOptionType.Crewmate, TrKey.TrackerTrackCorpses, true, TrackerSpawnRate);
        TrackerCorpsesTrackingCooldown = CustomOption.Normal(1094, CustomOptionType.Crewmate, TrKey.TrackerCorpseCooldown, 30f, 0f, 120f, 5f, TrackerCanTrackCorpses);
        TrackerCorpsesTrackingDuration = CustomOption.Normal(1095, CustomOptionType.Crewmate, TrKey.TrackerCorpseDuration, 5f, 2.5f, 30f, 2.5f, TrackerCanTrackCorpses);

        SnitchSpawnRate = new(1100, CustomOptionType.Crewmate, RoleType.Snitch, Snitch.NameColor, 1);
        SnitchLeftTasksForReveal = CustomOption.Normal(1101, CustomOptionType.Crewmate, TrKey.SnitchLeftTasksForReveal, 1f, 0f, 5f, 1f, SnitchSpawnRate);
        SnitchIncludeTeamJackal = CustomOption.Normal(1102, CustomOptionType.Crewmate, TrKey.SnitchIncludeTeamJackal, false, SnitchSpawnRate);
        SnitchTeamJackalUseDifferentArrowColor = CustomOption.Normal(1103, CustomOptionType.Crewmate, TrKey.SnitchTeamJackalUseDifferentArrowColor, true, SnitchIncludeTeamJackal);

        LighterSpawnRate = new(1110, CustomOptionType.Crewmate, RoleType.Lighter, Lighter.NameColor);
        LighterModeLightsOnVision = CustomOption.Normal(1111, CustomOptionType.Crewmate, TrKey.LighterModeLightsOnVision, 2f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterModeLightsOffVision = CustomOption.Normal(1112, CustomOptionType.Crewmate, TrKey.LighterModeLightsOffVision, 0.75f, 0.25f, 5f, 0.25f, LighterSpawnRate);
        LighterCooldown = CustomOption.Normal(1113, CustomOptionType.Crewmate, TrKey.LighterCooldown, 30f, 5f, 120f, 5f, LighterSpawnRate);
        LighterDuration = CustomOption.Normal(1114, CustomOptionType.Crewmate, TrKey.LighterDuration, 5f, 2.5f, 60f, 2.5f, LighterSpawnRate);
        // lighterCanSeeNinja = CustomOption.Normal(1115, CustomOptionType.Crewmate, "lighterCanSeeNinja", true, lighterSpawnRate);

        SecurityGuardSpawnRate = new(1120, CustomOptionType.Crewmate, RoleType.SecurityGuard, SecurityGuard.NameColor, 1);
        SecurityGuardCooldown = CustomOption.Normal(1121, CustomOptionType.Crewmate, TrKey.SecurityGuardCooldown, 30f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardTotalScrews = CustomOption.Normal(1122, CustomOptionType.Crewmate, TrKey.SecurityGuardTotalScrews, 7f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamPrice = CustomOption.Normal(1123, CustomOptionType.Crewmate, TrKey.SecurityGuardCamPrice, 2f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardVentPrice = CustomOption.Normal(1124, CustomOptionType.Crewmate, TrKey.SecurityGuardVentPrice, 1f, 1f, 15f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamDuration = CustomOption.Normal(1125, CustomOptionType.Crewmate, TrKey.SecurityGuardCamDuration, 10f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate);
        SecurityGuardCamMaxCharges = CustomOption.Normal(1126, CustomOptionType.Crewmate, TrKey.SecurityGuardCamMaxCharges, 5f, 1f, 30f, 1f, SecurityGuardSpawnRate);
        SecurityGuardCamRechargeTasksNumber = CustomOption.Normal(1127, CustomOptionType.Crewmate, TrKey.SecurityGuardCamRechargeTasksNumber, 3f, 1f, 10f, 1f, SecurityGuardSpawnRate);
        SecurityGuardNoMove = CustomOption.Normal(1128, CustomOptionType.Crewmate, TrKey.SecurityGuardNoMove, true, SecurityGuardSpawnRate);

        SwapperSpawnRate = new(1130, CustomOptionType.Neutral, TrKey.Swapper, Swapper.NameColor, 1);
        SwapperIsImpRate = CustomOption.Normal(1131, CustomOptionType.Neutral, TrKey.SwapperIsImpRate, RATES, SwapperSpawnRate);
        SwapperNumSwaps = CustomOption.Normal(1132, CustomOptionType.Neutral, TrKey.SwapperNumSwaps, 2f, 1f, 15f, 1f, SwapperSpawnRate);
        SwapperCanCallEmergency = CustomOption.Normal(1133, CustomOptionType.Neutral, TrKey.SwapperCanCallEmergency, false, SwapperSpawnRate);
        SwapperCanOnlySwapOthers = CustomOption.Normal(1134, CustomOptionType.Neutral, TrKey.SwapperCanOnlySwapOthers, false, SwapperSpawnRate);

        BaitSpawnRate = new(1140, CustomOptionType.Crewmate, RoleType.Bait, Bait.NameColor, 1);
        BaitHighlightAllVents = CustomOption.Normal(1141, CustomOptionType.Crewmate, TrKey.BaitHighlightAllVents, false, BaitSpawnRate);
        BaitReportDelay = CustomOption.Normal(1142, CustomOptionType.Crewmate, TrKey.BaitReportDelay, 0f, 0f, 10f, 1f, BaitSpawnRate, format: "unitSeconds");
        BaitShowKillFlash = CustomOption.Normal(1143, CustomOptionType.Crewmate, TrKey.BaitShowKillFlash, true, BaitSpawnRate);

        ShifterSpawnRate = new(1150, CustomOptionType.Neutral, RoleType.Shifter, Shifter.NameColor, 1);
        ShifterIsNeutralRate = CustomOption.Normal(1151, CustomOptionType.Neutral, TrKey.ShifterIsNeutralRate, RATES, ShifterSpawnRate);
        ShifterShiftsModifiers = CustomOption.Normal(1152, CustomOptionType.Neutral, TrKey.ShifterShiftsModifiers, false, ShifterSpawnRate);
        ShifterPastShifters = CustomOption.Normal(1153, CustomOptionType.Neutral, TrKey.ShifterPastShifters, false, ShifterSpawnRate);

        SheriffSpawnRate = new(1160, CustomOptionType.Crewmate, RoleType.Sheriff, Sheriff.NameColor);
        SheriffCooldown = CustomOption.Normal(1161, CustomOptionType.Crewmate, TrKey.SheriffCooldown, 30f, 2.5f, 60f, 2.5f, SheriffSpawnRate, format: "unitSeconds");
        SheriffNumShots = CustomOption.Normal(1162, CustomOptionType.Crewmate, TrKey.SheriffNumShots, 2f, 1f, 15f, 1f, SheriffSpawnRate, format: "unitShots");
        SheriffMisfireKillsTarget = CustomOption.Normal(1163, CustomOptionType.Crewmate, TrKey.SheriffMisfireKillsTarget, false, SheriffSpawnRate);
        SheriffCanKillNoDeadBody = CustomOption.Normal(1164, CustomOptionType.Crewmate, TrKey.SheriffCanKillNoDeadBody, true, SheriffSpawnRate);
        SheriffCanKillNeutrals = CustomOption.Normal(1165, CustomOptionType.Crewmate, TrKey.SheriffCanKillNeutrals, false, SheriffSpawnRate);

        MadmateRoleSpawnRate = new(1170, CustomOptionType.Crewmate, RoleType.Madmate, MadmateRole.NameColor, 3);
        MadmateRoleCanDieToSheriff = CustomOption.Normal(1171, CustomOptionType.Crewmate, TrKey.MadmateCanDieToSheriff, true, MadmateRoleSpawnRate);
        MadmateRoleCanEnterVents = CustomOption.Normal(1172, CustomOptionType.Crewmate, TrKey.MadmateCanEnterVents, true, MadmateRoleSpawnRate);
        MadmateRoleHasImpostorVision = CustomOption.Normal(1173, CustomOptionType.Crewmate, TrKey.MadmateHasImpostorVision, false, MadmateRoleSpawnRate);
        MadmateRoleCanSabotage = CustomOption.Normal(1174, CustomOptionType.Crewmate, TrKey.MadmateCanSabotage, false, MadmateRoleSpawnRate);
        MadmateRoleCanFixComm = CustomOption.Normal(1175, CustomOptionType.Crewmate, TrKey.MadmateCanFixComm, false, MadmateRoleSpawnRate);
        MadmateRoleCanKnowImpostorAfterFinishTasks = CustomOption.Normal(1176, CustomOptionType.Crewmate, TrKey.MadmateCanKnowImpostorAfterFinishTasks, false, MadmateRoleSpawnRate);
        MadmateRoleTasks = new((1177, 1178, 1179), CustomOptionType.Crewmate, (3, 2, 3), MadmateRoleCanKnowImpostorAfterFinishTasks);

        SuiciderSpawnRate = new(1190, CustomOptionType.Crewmate, RoleType.Suicider, Suicider.NameColor, 3);
        SuiciderCanDieToSheriff = CustomOption.Normal(1191, CustomOptionType.Crewmate, TrKey.SuiciderCanDieToSheriff, true, SuiciderSpawnRate);
        SuiciderCanEnterVents = CustomOption.Normal(1192, CustomOptionType.Crewmate, TrKey.SuiciderCanEnterVents, true, SuiciderSpawnRate);
        SuiciderHasImpostorVision = CustomOption.Normal(1193, CustomOptionType.Crewmate, TrKey.SuiciderHasImpostorVision, false, SuiciderSpawnRate);
        SuiciderCanFixComm = CustomOption.Normal(1194, CustomOptionType.Crewmate, TrKey.SuiciderCanFixComm, false, SuiciderSpawnRate);
        SuiciderCanKnowImpostorAfterFinishTasks = CustomOption.Normal(1195, CustomOptionType.Crewmate, TrKey.SuiciderCanKnowImpostorAfterFinishTasks, false, SuiciderSpawnRate);
        SuiciderTasks = new((1196, 1197, 1198), CustomOptionType.Crewmate, (3, 2, 3), SuiciderCanKnowImpostorAfterFinishTasks);

#endregion

#region ROLES IMPOSTOR

        BountyHunterSpawnRate = new(2000, CustomOptionType.Impostor, RoleType.BountyHunter, BountyHunter.NameColor, 1);
        BountyHunterBountyDuration = CustomOption.Normal(2001, CustomOptionType.Impostor, TrKey.BountyHunterBountyDuration, 60f, 10f, 180f, 10f, BountyHunterSpawnRate);
        BountyHunterReducedCooldown = CustomOption.Normal(20002, CustomOptionType.Impostor, TrKey.BountyHunterReducedCooldown, 2.5f, 2.5f, 30f, 2.5f, BountyHunterSpawnRate);
        BountyHunterPunishmentTime = CustomOption.Normal(2003, CustomOptionType.Impostor, TrKey.BountyHunterPunishmentTime, 20f, 0f, 60f, 2.5f, BountyHunterSpawnRate);
        BountyHunterShowArrow = CustomOption.Normal(2004, CustomOptionType.Impostor, TrKey.BountyHunterShowArrow, true, BountyHunterSpawnRate);
        BountyHunterArrowUpdateInterval = CustomOption.Normal(2005, CustomOptionType.Impostor, TrKey.BountyHunterArrowUpdateInterval, 15f, 2.5f, 60f, 2.5f, BountyHunterShowArrow);

        MafiaSpawnRate = new(2010, CustomOptionType.Impostor, TrKey.Mafia, Mafia.NameColor, 1);
        MafiosoCanSabotage = CustomOption.Normal(2011, CustomOptionType.Impostor, TrKey.MafiosoCanSabotage, false, MafiaSpawnRate);
        MafiosoCanRepair = CustomOption.Normal(2012, CustomOptionType.Impostor, TrKey.MafiosoCanRepair, false, MafiaSpawnRate);
        MafiosoCanVent = CustomOption.Normal(2013, CustomOptionType.Impostor, TrKey.MafiosoCanVent, false, MafiaSpawnRate);
        JanitorCooldown = CustomOption.Normal(2014, CustomOptionType.Impostor, TrKey.JanitorCooldown, 30f, 2.5f, 60f, 2.5f, MafiaSpawnRate);
        JanitorCanSabotage = CustomOption.Normal(2015, CustomOptionType.Impostor, TrKey.JanitorCanSabotage, false, MafiaSpawnRate);
        JanitorCanRepair = CustomOption.Normal(2016, CustomOptionType.Impostor, TrKey.JanitorCanRepair, false, MafiaSpawnRate);
        JanitorCanVent = CustomOption.Normal(2017, CustomOptionType.Impostor, TrKey.JanitorCanVent, false, MafiaSpawnRate);

        TricksterSpawnRate = new(2020, CustomOptionType.Impostor, RoleType.Trickster, Trickster.NameColor, 1);
        TricksterPlaceBoxCooldown = CustomOption.Normal(2021, CustomOptionType.Impostor, TrKey.TricksterPlaceBoxCooldown, 10f, 2.5f, 30f, 2.5f, TricksterSpawnRate);
        TricksterLightsOutCooldown = CustomOption.Normal(2022, CustomOptionType.Impostor, TrKey.TricksterLightsOutCooldown, 30f, 5f, 60f, 5f, TricksterSpawnRate);
        TricksterLightsOutDuration = CustomOption.Normal(2023, CustomOptionType.Impostor, TrKey.TricksterLightsOutDuration, 15f, 5f, 60f, 2.5f, TricksterSpawnRate);

        EvilHackerSpawnRate = new(2030, CustomOptionType.Impostor, RoleType.EvilHacker, EvilHacker.NameColor, 1);
        EvilHackerCanHasBetterAdmin = CustomOption.Normal(2031, CustomOptionType.Impostor, TrKey.EvilHackerCanHasBetterAdmin, false, EvilHackerSpawnRate);
        EvilHackerCanMoveEvenIfUsesAdmin = CustomOption.Normal(2032, CustomOptionType.Impostor, TrKey.EvilHackerCanMoveEvenIfUsesAdmin, true, EvilHackerSpawnRate);
        EvilHackerCanInheritAbility = CustomOption.Normal(2033, CustomOptionType.Impostor, TrKey.EvilHackerCanInheritAbility, false, EvilHackerSpawnRate);
        EvilHackerCanSeeDoorStatus = CustomOption.Normal(2034, CustomOptionType.Impostor, TrKey.EvilHackerCanSeeDoorStatus, true, EvilHackerSpawnRate);
        EvilHackerCanCreateMadmate = CustomOption.Normal(2035, CustomOptionType.Impostor, TrKey.EvilHackerCanCreateMadmate, false, EvilHackerSpawnRate);
        CreatedMadmateCanDieToSheriff = CustomOption.Normal(2036, CustomOptionType.Impostor, TrKey.CreatedMadmateCanDieToSheriff, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanEnterVents = CustomOption.Normal(2037, CustomOptionType.Impostor, TrKey.CreatedMadmateCanEnterVents, false, EvilHackerCanCreateMadmate);
        EvilHackerCanCreateMadmateFromJackal = CustomOption.Normal(2038, CustomOptionType.Impostor, TrKey.EvilHackerCanCreateMadmateFromJackal, false, EvilHackerCanCreateMadmate);
        CreatedMadmateHasImpostorVision = CustomOption.Normal(2039, CustomOptionType.Impostor, TrKey.CreatedMadmateHasImpostorVision, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanSabotage = CustomOption.Normal(2040, CustomOptionType.Impostor, TrKey.CreatedMadmateCanSabotage, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanFixComm = CustomOption.Normal(2041, CustomOptionType.Impostor, TrKey.CreatedMadmateCanFixComm, true, EvilHackerCanCreateMadmate);
        CreatedMadmateAbility = CustomOption.Normal(2042, CustomOptionType.Impostor, TrKey.MadmateAbility, [Tr.Get(TrKey.MadmateNone), Tr.Get(TrKey.MadmateFanatic)], EvilHackerCanCreateMadmate);
        CreatedMadmateNumTasks = CustomOption.Normal(2043, CustomOptionType.Impostor, TrKey.CreatedMadmateNumTasks, 4f, 1f, 20f, 1f, CreatedMadmateAbility);
        CreatedMadmateExileCrewmate = CustomOption.Normal(2044, CustomOptionType.Impostor, TrKey.CreatedMadmateExileCrewmate, false, EvilHackerCanCreateMadmate);

        EvilTrackerSpawnRate = new(2050, CustomOptionType.Impostor, RoleType.EvilTracker, EvilTracker.NameColor, 3);
        EvilTrackerCooldown = CustomOption.Normal(2051, CustomOptionType.Impostor, TrKey.EvilTrackerCooldown, 10f, 0f, 60f, 1f, EvilTrackerSpawnRate);
        EvilTrackerResetTargetAfterMeeting = CustomOption.Normal(2052, CustomOptionType.Impostor, TrKey.EvilTrackerResetTargetAfterMeeting, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeDeathFlash = CustomOption.Normal(2053, CustomOptionType.Impostor, TrKey.EvilTrackerCanSeeDeathFlash, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetTask = CustomOption.Normal(2054, CustomOptionType.Impostor, TrKey.EvilTrackerCanSeeTargetTask, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetPosition = CustomOption.Normal(2055, CustomOptionType.Impostor, TrKey.EvilTrackerCanSeeTargetPosition, true, EvilTrackerSpawnRate);
        EvilTrackerCanSetTargetOnMeeting = CustomOption.Normal(2056, CustomOptionType.Impostor, TrKey.EvilTrackerCanSetTargetOnMeeting, true, EvilTrackerSpawnRate);

        EraserSpawnRate = new(2060, CustomOptionType.Impostor, RoleType.Eraser, Eraser.NameColor, 1);
        EraserCooldown = CustomOption.Normal(2061, CustomOptionType.Impostor, TrKey.EraserCooldown, 30f, 5f, 120f, 5f, EraserSpawnRate, format: "unitSeconds");
        EraserCooldownIncrease = CustomOption.Normal(2062, CustomOptionType.Impostor, TrKey.EraserCooldownIncrease, 10f, 0f, 120f, 2.5f, EraserSpawnRate, format: "unitSeconds");
        EraserCanEraseAnyone = CustomOption.Normal(2063, CustomOptionType.Impostor, TrKey.EraserCanEraseAnyone, false, EraserSpawnRate);

        MorphingSpawnRate = new(2070, CustomOptionType.Impostor, RoleType.Morphing, Morphing.NameColor, 1);
        MorphingCooldown = CustomOption.Normal(2071, CustomOptionType.Impostor, TrKey.MorphingCooldown, 30f, 2.5f, 60f, 2.5f, MorphingSpawnRate, format: "unitSeconds");
        MorphingDuration = CustomOption.Normal(2072, CustomOptionType.Impostor, TrKey.MorphingDuration, 10f, 1f, 20f, 0.5f, MorphingSpawnRate, format: "unitSeconds");

        CamouflagerSpawnRate = new(2080, CustomOptionType.Impostor, RoleType.Camouflager, Camouflager.NameColor, 1);
        CamouflagerCooldown = CustomOption.Normal(2081, CustomOptionType.Impostor, TrKey.CamouflagerCooldown, 30f, 2.5f, 60f, 2.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerDuration = CustomOption.Normal(2082, CustomOptionType.Impostor, TrKey.CamouflagerDuration, 10f, 1f, 20f, 0.5f, CamouflagerSpawnRate, format: "unitSeconds");
        CamouflagerRandomColors = CustomOption.Normal(2083, CustomOptionType.Impostor, TrKey.CamouflagerRandomColors, false, CamouflagerSpawnRate);

        CleanerSpawnRate = new(2090, CustomOptionType.Impostor, RoleType.Cleaner, Cleaner.NameColor, 1);
        CleanerCooldown = CustomOption.Normal(2091, CustomOptionType.Impostor, TrKey.CleanerCooldown, 30f, 2.5f, 60f, 2.5f, CleanerSpawnRate, format: "unitSeconds");

        WarlockSpawnRate = new(2100, CustomOptionType.Impostor, RoleType.Warlock, Warlock.NameColor, 1);
        WarlockCooldown = CustomOption.Normal(2101, CustomOptionType.Impostor, TrKey.WarlockCooldown, 30f, 2.5f, 60f, 2.5f, WarlockSpawnRate, format: "unitSeconds");
        WarlockRootTime = CustomOption.Normal(2102, CustomOptionType.Impostor, TrKey.WarlockRootTime, 5f, 0f, 15f, 1f, WarlockSpawnRate, format: "unitSeconds");

        WitchSpawnRate = new(2110, CustomOptionType.Impostor, RoleType.Witch, Witch.NameColor, 1);
        WitchCooldown = CustomOption.Normal(2111, CustomOptionType.Impostor, TrKey.WitchSpellCooldown, 30f, 2.5f, 120f, 2.5f, WitchSpawnRate, format: "unitSeconds");
        WitchAdditionalCooldown = CustomOption.Normal(2112, CustomOptionType.Impostor, TrKey.WitchAdditionalCooldown, 10f, 0f, 60f, 5f, WitchSpawnRate, format: "unitSeconds");
        WitchCanSpellAnyone = CustomOption.Normal(2113, CustomOptionType.Impostor, TrKey.WitchCanSpellAnyone, false, WitchSpawnRate);
        WitchSpellCastingDuration = CustomOption.Normal(2114, CustomOptionType.Impostor, TrKey.WitchSpellDuration, 1f, 0f, 10f, 1f, WitchSpawnRate, format: "unitSeconds");
        WitchTriggerBothCooldowns = CustomOption.Normal(2115, CustomOptionType.Impostor, TrKey.WitchTriggerBoth, true, WitchSpawnRate);
        WitchVoteSavesTargets = CustomOption.Normal(2116, CustomOptionType.Impostor, TrKey.WitchSaveTargets, true, WitchSpawnRate);

        VampireSpawnRate = new(2120, CustomOptionType.Impostor, RoleType.Vampire, Vampire.NameColor, 1);
        VampireKillDelay = CustomOption.Normal(2121, CustomOptionType.Impostor, TrKey.VampireKillDelay, 10f, 1f, 20f, 1f, VampireSpawnRate, format: "unitSeconds");
        VampireCooldown = CustomOption.Normal(2122, CustomOptionType.Impostor, TrKey.VampireCooldown, 30f, 2.5f, 60f, 2.5f, VampireSpawnRate, format: "unitSeconds");
        VampireCanKillNearGarlics = CustomOption.Normal(2123, CustomOptionType.Impostor, TrKey.VampireCanKillNearGarlics, true, VampireSpawnRate);

#endregion

#region ROLES NEUTRAL

        JesterSpawnRate = new(3000, CustomOptionType.Neutral, RoleType.Jester, Jester.NameColor, 1);
        JesterCanCallEmergency = CustomOption.Normal(3001, CustomOptionType.Neutral, TrKey.JesterCanCallEmergency, true, JesterSpawnRate);
        JesterCanSabotage = CustomOption.Normal(3002, CustomOptionType.Neutral, TrKey.JesterCanSabotage, true, JesterSpawnRate);
        JesterHasImpostorVision = CustomOption.Normal(3003, CustomOptionType.Neutral, TrKey.JesterHasImpostorVision, false, JesterSpawnRate);

        ArsonistSpawnRate = new(3010, CustomOptionType.Neutral, RoleType.Arsonist, Arsonist.NameColor, 1);
        ArsonistCooldown = CustomOption.Normal(3011, CustomOptionType.Neutral, TrKey.ArsonistCooldown, 12.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
        ArsonistDuration = CustomOption.Normal(3012, CustomOptionType.Neutral, TrKey.ArsonistDuration, 3f, 0f, 10f, 1f, ArsonistSpawnRate);
        ArsonistCanBeLovers = CustomOption.Normal(3013, CustomOptionType.Neutral, TrKey.ArsonistCanBeLovers, false, ArsonistSpawnRate);

        VultureSpawnRate = new(3020, CustomOptionType.Neutral, RoleType.Vulture, Vulture.NameColor, 1);
        VultureCooldown = CustomOption.Normal(3021, CustomOptionType.Neutral, TrKey.VultureCooldown, 15f, 2.5f, 60f, 2.5f, VultureSpawnRate);
        VultureNumberToWin = CustomOption.Normal(3022, CustomOptionType.Neutral, TrKey.VultureNumberToWin, 4f, 1f, 12f, 1f, VultureSpawnRate);
        VultureCanUseVents = CustomOption.Normal(3023, CustomOptionType.Neutral, TrKey.VultureCanUseVents, true, VultureSpawnRate);
        VultureShowArrows = CustomOption.Normal(3024, CustomOptionType.Neutral, TrKey.VultureShowArrows, true, VultureSpawnRate);

        JackalSpawnRate = new(3030, CustomOptionType.Neutral, RoleType.Jackal, Jackal.NameColor, 1);
        JackalKillCooldown = CustomOption.Normal(3031, CustomOptionType.Neutral, TrKey.JackalKillCooldown, 30f, 10f, 60f, 2.5f, JackalSpawnRate);
        JackalCanSabotageLights = CustomOption.Normal(3032, CustomOptionType.Neutral, TrKey.JackalCanSabotageLights, true, JackalSpawnRate);
        JackalCanUseVents = CustomOption.Normal(3033, CustomOptionType.Neutral, TrKey.JackalCanUseVents, true, JackalSpawnRate);
        JackalHasImpostorVision = CustomOption.Normal(3034, CustomOptionType.Neutral, TrKey.JackalHasImpostorVision, false, JackalSpawnRate);
        JackalCanCreateSidekick = CustomOption.Normal(3035, CustomOptionType.Neutral, TrKey.JackalCanCreateSidekick, false, JackalSpawnRate);
        JackalCreateSidekickCooldown = CustomOption.Normal(3036, CustomOptionType.Neutral, TrKey.JackalCreateSidekickCooldown, 30f, 10f, 60f, 2.5f, JackalCanCreateSidekick);
        SidekickCanKill = CustomOption.Normal(3038, CustomOptionType.Neutral, TrKey.SidekickCanKill, false, JackalCanCreateSidekick);
        SidekickCanUseVents = CustomOption.Normal(3039, CustomOptionType.Neutral, TrKey.SidekickCanUseVents, true, JackalCanCreateSidekick);
        SidekickCanSabotageLights = CustomOption.Normal(3040, CustomOptionType.Neutral, TrKey.SidekickCanSabotageLights, true, JackalCanCreateSidekick);
        SidekickHasImpostorVision = CustomOption.Normal(3041, CustomOptionType.Neutral, TrKey.SidekickHasImpostorVision, false, JackalCanCreateSidekick);
        SidekickPromotesToJackal = CustomOption.Normal(3037, CustomOptionType.Neutral, TrKey.SidekickPromotesToJackal, false, JackalCanCreateSidekick);
        JackalPromotedFromSidekickCanCreateSidekick = CustomOption.Normal(3042, CustomOptionType.Neutral, TrKey.JackalPromotedFromSidekickCanCreateSidekick, false, SidekickPromotesToJackal);
        JackalCanCreateSidekickFromImpostor = CustomOption.Normal(3043, CustomOptionType.Neutral, TrKey.JackalCanCreateSidekickFromImpostor, false, JackalCanCreateSidekick);

        GuesserSpawnRate = new(3050, CustomOptionType.Neutral, TrKey.Guesser, Guesser.NiceGuesser.NameColor, 1);
        GuesserIsImpGuesserRate = CustomOption.Normal(3051, CustomOptionType.Neutral, TrKey.GuesserIsImpGuesserRate, RATES, GuesserSpawnRate);
        GuesserSpawnBothRate = CustomOption.Normal(3052, CustomOptionType.Neutral, TrKey.GuesserSpawnBothRate, RATES, GuesserSpawnRate);
        GuesserNumberOfShots = CustomOption.Normal(3053, CustomOptionType.Neutral, TrKey.GuesserNumberOfShots, 2f, 1f, 15f, 1f, GuesserSpawnRate);
        GuesserOnlyAvailableRoles = CustomOption.Normal(3054, CustomOptionType.Neutral, TrKey.GuesserOnlyAvailableRoles, true, GuesserSpawnRate);
        GuesserHasMultipleShotsPerMeeting = CustomOption.Normal(3055, CustomOptionType.Neutral, TrKey.GuesserHasMultipleShotsPerMeeting, false, GuesserSpawnRate);
        GuesserShowInfoInGhostChat = CustomOption.Normal(3056, CustomOptionType.Neutral, TrKey.GuesserToGhostChat, true, GuesserSpawnRate);
        GuesserKillsThroughShield = CustomOption.Normal(3057, CustomOptionType.Neutral, TrKey.GuesserPierceShield, true, GuesserSpawnRate);
        GuesserEvilCanKillSpy = CustomOption.Normal(3058, CustomOptionType.Neutral, TrKey.GuesserEvilCanKillSpy, true, GuesserSpawnRate);

#endregion

#region MODIFIERS

        MadmateSpawnRate = new(4000, CustomOptionType.Modifier, ModifierType.Madmate, Madmate.NameColor);
        MadmateType = CustomOption.Normal(4001, CustomOptionType.Modifier, TrKey.MadmateType, [Tr.Get(TrKey.MadmateDefault), Tr.Get(TrKey.MadmateWithRole), Tr.Get(TrKey.MadmateRandom)], MadmateSpawnRate);
        MadmateFixedRole = new(4002, CustomOptionType.Modifier, TrKey.MadmateFixedRole, Madmate.ValidRoles, MadmateType);
        MadmateAbility = CustomOption.Normal(4003, CustomOptionType.Modifier, TrKey.MadmateAbility, [Tr.Get(TrKey.MadmateNone), Tr.Get(TrKey.MadmateFanatic)], MadmateSpawnRate);
        MadmateTasks = new((4004, 4005, 4006), CustomOptionType.Modifier, (1, 1, 3), MadmateAbility);
        MadmateCanDieToSheriff = CustomOption.Normal(4007, CustomOptionType.Modifier, TrKey.MadmateCanDieToSheriff, false, MadmateSpawnRate);
        MadmateCanEnterVents = CustomOption.Normal(4008, CustomOptionType.Modifier, TrKey.MadmateCanEnterVents, false, MadmateSpawnRate);
        MadmateHasImpostorVision = CustomOption.Normal(4009, CustomOptionType.Modifier, TrKey.MadmateHasImpostorVision, false, MadmateSpawnRate);
        MadmateCanSabotage = CustomOption.Normal(4010, CustomOptionType.Modifier, TrKey.MadmateCanSabotage, false, MadmateSpawnRate);
        MadmateCanFixComm = CustomOption.Normal(4011, CustomOptionType.Modifier, TrKey.MadmateCanFixComm, true, MadmateSpawnRate);
        MadmateExilePlayer = CustomOption.Normal(4012, CustomOptionType.Modifier, TrKey.MadmateExileCrewmate, false, MadmateSpawnRate);

        LastImpostorEnable = CustomOption.Header(4010, CustomOptionType.Modifier, TrKey.LastImpostorEnable, true, TrKey.LastImpostor);
        LastImpostorFunctions = CustomOption.Normal(4011, CustomOptionType.Modifier, TrKey.LastImpostorFunctions, [Tr.Get(TrKey.LastImpostorDivine), Tr.Get(TrKey.LastImpostorGuesser)], LastImpostorEnable);
        LastImpostorNumKills = CustomOption.Normal(4012, CustomOptionType.Modifier, TrKey.LastImpostorNumKills, 3f, 0f, 10f, 1f, LastImpostorEnable);
        LastImpostorResults = CustomOption.Normal(4013, CustomOptionType.Modifier, TrKey.FortuneTellerResults, [Tr.Get(TrKey.FortuneTellerResultCrew), Tr.Get(TrKey.FortuneTellerResultTeam), Tr.Get(TrKey.FortuneTellerResultRole)], LastImpostorEnable);
        LastImpostorNumShots = CustomOption.Normal(4014, CustomOptionType.Modifier, TrKey.LastImpostorNumShots, 1f, 1f, 15f, 1f, LastImpostorEnable);

        LoversSpawnRate = new(4020, CustomOptionType.Modifier, RoleType.Lovers, Lovers.Color, 1);
        LoversImpLoverRate = CustomOption.Normal(4021, CustomOptionType.Modifier, TrKey.LoversImpLoverRate, RATES, LoversSpawnRate);
        LoversNumCouples = CustomOption.Normal(4022, CustomOptionType.Modifier, TrKey.LoversNumCouples, 1f, 1f, 7f, 1f, LoversSpawnRate, format: "unitCouples");
        LoversBothDie = CustomOption.Normal(4023, CustomOptionType.Modifier, TrKey.LoversBothDie, true, LoversSpawnRate);
        LoversCanHaveAnotherRole = CustomOption.Normal(4024, CustomOptionType.Modifier, TrKey.LoversCanHaveAnotherRole, true, LoversSpawnRate);
        LoversSeparateTeam = CustomOption.Normal(4025, CustomOptionType.Modifier, TrKey.LoversSeparateTeam, true, LoversSpawnRate);
        LoversTasksCount = CustomOption.Normal(4026, CustomOptionType.Modifier, TrKey.LoversTasksCount, false, LoversSpawnRate);
        LoversEnableChat = CustomOption.Normal(4027, CustomOptionType.Modifier, TrKey.LoversEnableChat, true, LoversSpawnRate);

        MiniSpawnRate = new(180, CustomOptionType.Modifier, ModifierType.Mini, Mini.NameColor);
        MiniGrowingUpDuration = CustomOption.Normal(181, CustomOptionType.Modifier, TrKey.MiniGrowingUpDuration, 400f, 100f, 1500f, 100f, MiniSpawnRate, format: "unitSeconds");

        AntiTeleportSpawnRate = new(4030, CustomOptionType.Modifier, ModifierType.AntiTeleport, AntiTeleport.NameColor);

#endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }

#region MOD OPTIONS

    public static CustomOption PresetSelection;
    public static CustomOption ActivateRoles;
    public static CustomOption RandomNumberAlgorithm;
    public static CustomOption EnableRandomRandomNumberAlgorithm;
    public static CustomOption EnableRandomRandomNumberAlgorithmDotnet;
    public static CustomOption EnableRandomRandomNumberAlgorithmMT;
    public static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Pp;
    public static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Ss;
    public static CustomOption EnableRandomRandomNumberAlgorithmPcg64;

#endregion

#region GAME MODE

    public static CustomOption GameMode;

    public static CustomOption GamemodeSettings;
    public static CustomOption GamemodeMatchDuration;
    public static CustomOption GamemodeKillCooldown;
    public static CustomOption GamemodeEnableFlashlight;
    public static CustomOption GamemodeFlashlightRange;
    public static CustomOption GamemodeReviveTime;
    public static CustomOption GamemodeInvincibilityTimeAfterRevive;

    public static CustomOption RequiredFlags;

    public static CustomOption ThiefModerequiredJewels;
    public static CustomOption ThiefModePoliceCatchCooldown;
    public static CustomOption ThiefModecaptureThiefTime;
    public static CustomOption ThiefModePoliceTaseCooldown;
    public static CustomOption ThiefModePoliceTaseDuration;
    public static CustomOption ThiefModePoliceCanSeeJewels;
    public static CustomOption ThiefModeWhoCanThiefsKill;

    public static CustomOption HotPotatoTransferLimit;
    public static CustomOption HotPotatoCooldown;
    public static CustomOption HotPotatoResetTimeForTransfer;
    public static CustomOption HotPotatoIncreaseTimeIfNoReset;

    public static CustomOption BattleRoyaleMatchType;
    public static CustomOption BattleRoyaleKillCooldown;
    public static CustomOption BattleRoyaleLifes;
    public static CustomOption BattleRoyaleScoreNeeded;

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

    public static CustomOption EnableDiscordAutoMute;
    public static CustomOption EnableDiscordEmbed;

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
    public static CustomOption RandomMapEnableMiraHq;
    public static CustomOption RandomMapEnablePolus;
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

    public static CustomRoleOption MadmateRoleSpawnRate;
    public static CustomOption MadmateRoleCanDieToSheriff;
    public static CustomOption MadmateRoleCanEnterVents;
    public static CustomOption MadmateRoleHasImpostorVision;
    public static CustomOption MadmateRoleCanSabotage;
    public static CustomOption MadmateRoleCanFixComm;
    public static CustomOption MadmateRoleCanKnowImpostorAfterFinishTasks;
    public static CustomTasksOption MadmateRoleTasks;

    public static CustomRoleOption SuiciderSpawnRate;
    public static CustomOption SuiciderCanDieToSheriff;
    public static CustomOption SuiciderCanEnterVents;
    public static CustomOption SuiciderHasImpostorVision;
    public static CustomOption SuiciderCanFixComm;
    public static CustomOption SuiciderCanKnowImpostorAfterFinishTasks;
    public static CustomTasksOption SuiciderTasks;

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
}
