namespace RebuildUs;

internal static class CustomOptionHolder
{
    internal static readonly object[] Rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    private static readonly string[] Presets = [Tr.Get(TrKey.Preset1), Tr.Get(TrKey.Preset2), Tr.Get(TrKey.Preset3), Tr.Get(TrKey.Preset4), Tr.Get(TrKey.Preset5)];

    internal static readonly Dictionary<byte, byte[]> BlockedRolePairings = [];

    internal static void Load()
    {
#region MOD OPTIONS

        PresetSelection = CustomOption.Header(0, CustomOptionType.General, TrKey.Preset, Presets, TrKey.Preset);
        ActivateRoles = CustomOption.Normal(1, CustomOptionType.General, TrKey.ActivateRoles, true);
        EnableRandomRandomNumberAlgorithm = CustomOption.Normal(2, CustomOptionType.General, TrKey.RandomRandomNumberAlgorithm, false);
        RandomNumberAlgorithm = CustomOption.Normal(3, CustomOptionType.General, TrKey.RandomNumberAlgorithm, [Tr.Get(TrKey.RndDotnet), Tr.Get(TrKey.RndMT), Tr.Get(TrKey.RndXoshiro256), Tr.Get(TrKey.RndXoshiro256Ss), Tr.Get(TrKey.RndPcg64)], EnableRandomRandomNumberAlgorithm, 0, 0, 0, 0, true);
        EnableRandomRandomNumberAlgorithmDotnet = CustomOption.Normal(4, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmDotnet, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmMT = CustomOption.Normal(5, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmMT, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Pp = CustomOption.Normal(6, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256Pp, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Ss = CustomOption.Normal(7, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256Ss, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmPcg64 = CustomOption.Normal(8, CustomOptionType.General, TrKey.EnableRandomRandomNumberAlgorithmPcg64, true, EnableRandomRandomNumberAlgorithm);

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
        RandomMapEnableMiraHq = CustomOption.Normal(102, CustomOptionType.General, TrKey.RandomMapEnableMiraHq, true, RandomMap);
        RandomMapEnablePolus = CustomOption.Normal(103, CustomOptionType.General, TrKey.RandomMapEnablePolus, true, RandomMap);
        RandomMapEnableDleks = CustomOption.Normal(104, CustomOptionType.General, TrKey.RandomMapEnableDleks, true, RandomMap);
        RandomMapEnableAirShip = CustomOption.Normal(105, CustomOptionType.General, TrKey.RandomMapEnableAirShip, true, RandomMap);
        RandomMapEnableFungle = CustomOption.Normal(106, CustomOptionType.General, TrKey.RandomMapEnableFungle, true, RandomMap);
        RandomMapEnableSubmerged = CustomOption.Normal(107, CustomOptionType.General, TrKey.RandomMapEnableSubmerged, true, RandomMap);

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
        SwapperIsImpRate = CustomOption.Normal(1131, CustomOptionType.Neutral, TrKey.SwapperIsImpRate, Rates, SwapperSpawnRate);
        SwapperNumSwaps = CustomOption.Normal(1132, CustomOptionType.Neutral, TrKey.SwapperNumSwaps, 2f, 1f, 15f, 1f, SwapperSpawnRate);
        SwapperCanCallEmergency = CustomOption.Normal(1133, CustomOptionType.Neutral, TrKey.SwapperCanCallEmergency, false, SwapperSpawnRate);
        SwapperCanOnlySwapOthers = CustomOption.Normal(1134, CustomOptionType.Neutral, TrKey.SwapperCanOnlySwapOthers, false, SwapperSpawnRate);

        BaitSpawnRate = new(1140, CustomOptionType.Crewmate, RoleType.Bait, Bait.NameColor, 1);
        BaitHighlightAllVents = CustomOption.Normal(1141, CustomOptionType.Crewmate, TrKey.BaitHighlightAllVents, false, BaitSpawnRate);
        BaitReportDelay = CustomOption.Normal(1142, CustomOptionType.Crewmate, TrKey.BaitReportDelay, 0f, 0f, 10f, 1f, BaitSpawnRate, format: "unitSeconds");
        BaitShowKillFlash = CustomOption.Normal(1143, CustomOptionType.Crewmate, TrKey.BaitShowKillFlash, true, BaitSpawnRate);

        ShifterSpawnRate = new(1150, CustomOptionType.Neutral, RoleType.Shifter, Shifter.NameColor, 1);
        ShifterIsNeutralRate = CustomOption.Normal(1151, CustomOptionType.Neutral, TrKey.ShifterIsNeutralRate, Rates, ShifterSpawnRate);
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
        GuesserIsImpGuesserRate = CustomOption.Normal(3051, CustomOptionType.Neutral, TrKey.GuesserIsImpGuesserRate, Rates, GuesserSpawnRate);
        GuesserSpawnBothRate = CustomOption.Normal(3052, CustomOptionType.Neutral, TrKey.GuesserSpawnBothRate, Rates, GuesserSpawnRate);
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
        LoversImpLoverRate = CustomOption.Normal(4021, CustomOptionType.Modifier, TrKey.LoversImpLoverRate, Rates, LoversSpawnRate);
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

    internal static CustomOption PresetSelection;
    internal static CustomOption ActivateRoles;
    internal static CustomOption RandomNumberAlgorithm;
    internal static CustomOption EnableRandomRandomNumberAlgorithm;
    internal static CustomOption EnableRandomRandomNumberAlgorithmDotnet;
    internal static CustomOption EnableRandomRandomNumberAlgorithmMT;
    internal static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Pp;
    internal static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Ss;
    internal static CustomOption EnableRandomRandomNumberAlgorithmPcg64;

#endregion

#region GENERAL OPTIONS

    internal static CustomOption CrewmateRolesCountMin;
    internal static CustomOption CrewmateRolesCountMax;
    internal static CustomOption ImpostorRolesCountMin;
    internal static CustomOption ImpostorRolesCountMax;
    internal static CustomOption NeutralRolesCountMin;
    internal static CustomOption NeutralRolesCountMax;
    internal static CustomOption ModifiersCountMin;
    internal static CustomOption ModifiersCountMax;

#endregion

#region GAME OPTIONS

    internal static CustomOption GameOptions;
    internal static CustomOption MaxNumberOfMeetings;
    internal static CustomOption BlockSkippingInEmergencyMeetings;
    internal static CustomOption NoVoteIsSelfVote;
    internal static CustomOption HidePlayerNames;
    internal static CustomOption AllowParallelMedBayScans;
    internal static CustomOption HideOutOfSightNametags;
    internal static CustomOption RefundVotesOnDeath;
    internal static CustomOption DelayBeforeMeeting;
    internal static CustomOption DisableVentAnimation;
    internal static CustomOption StopCooldownOnFixingElecSabotage;
    internal static CustomOption EnableHawkMode;

    internal static CustomOption CanWinByTaskWithoutLivingPlayer;

    // internal static CustomOption DeadPlayerCanSeeCooldown;
    internal static CustomOption ImpostorCanIgnoreCommSabotage;
    // internal static CustomOption BlockSabotageFromDeadImpostors;
    // internal static CustomOption ShieldFirstKill;

    internal static CustomOption AdditionalEmergencyCooldown;
    internal static CustomOption AdditionalEmergencyCooldownTime;

    internal static CustomOption RestrictDevices;
    internal static CustomOption RestrictAdmin;
    internal static CustomOption RestrictAdminTime;
    internal static CustomOption RestrictAdminText;
    internal static CustomOption RestrictCameras;
    internal static CustomOption RestrictCamerasTime;
    internal static CustomOption RestrictCamerasText;
    internal static CustomOption RestrictVitals;
    internal static CustomOption RestrictVitalsTime;
    internal static CustomOption RestrictVitalsText;

#endregion

#region POLUS OPTIONS

    internal static CustomOption PolusAdditionalVents;
    internal static CustomOption PolusSpecimenVital;
    internal static CustomOption PolusRandomSpawn;

#endregion

#region AIRSHIP OPTIONS

    internal static CustomOption AirshipOptimize;
    internal static CustomOption AirshipEnableWallCheck;
    internal static CustomOption AirshipReactorDuration;
    internal static CustomOption AirshipRandomSpawn;
    internal static CustomOption AirshipAdditionalSpawn;
    internal static CustomOption AirshipSynchronizedSpawning;
    internal static CustomOption AirshipSetOriginalCooldown;
    internal static CustomOption AirshipInitialDoorCooldown;
    internal static CustomOption AirshipInitialSabotageCooldown;
    internal static CustomOption AirshipOldAdmin;
    internal static CustomOption AirshipRestrictedAdmin;
    internal static CustomOption AirshipDisableGapSwitchBoard;
    internal static CustomOption AirshipDisableMovingPlatform;
    internal static CustomOption AirshipAdditionalLadder;
    internal static CustomOption AirshipOneWayLadder;
    internal static CustomOption AirshipReplaceSafeTask;
    internal static CustomOption AirshipAdditionalWireTask;

#endregion

#region MAP OPTIONS

    internal static CustomOption RandomMap;
    internal static CustomOption RandomMapEnableSkeld;
    internal static CustomOption RandomMapEnableMiraHq;
    internal static CustomOption RandomMapEnablePolus;
    internal static CustomOption RandomMapEnableDleks;
    internal static CustomOption RandomMapEnableAirShip;
    internal static CustomOption RandomMapEnableFungle;
    internal static CustomOption RandomMapEnableSubmerged;

#endregion

#region ROLES CREWMATE

    internal static CustomRoleOption MayorSpawnRate;
    internal static CustomOption MayorNumVotes;
    internal static CustomOption MayorCanSeeVoteColors;
    internal static CustomOption MayorTasksNeededToSeeVoteColors;
    internal static CustomOption MayorMeetingButton;
    internal static CustomOption MayorMaxRemoteMeetings;

    internal static CustomRoleOption EngineerSpawnRate;
    internal static CustomOption EngineerNumberOfFixes;
    internal static CustomOption EngineerHighlightForImpostors;
    internal static CustomOption EngineerHighlightForTeamJackal;

    internal static CustomRoleOption SpySpawnRate;
    internal static CustomOption SpyCanDieToSheriff;
    internal static CustomOption SpyImpostorsCanKillAnyone;
    internal static CustomOption SpyCanEnterVents;
    internal static CustomOption SpyHasImpostorVision;

    internal static CustomRoleOption MedicSpawnRate;
    internal static CustomOption MedicShowShielded;
    internal static CustomOption MedicShowAttemptToShielded;
    internal static CustomOption MedicSetShieldAfterMeeting;
    internal static CustomOption MedicShowAttemptToMedic;

    internal static CustomRoleOption SeerSpawnRate;
    internal static CustomOption SeerMode;
    internal static CustomOption SeerSoulDuration;
    internal static CustomOption SeerLimitSoulDuration;

    internal static CustomRoleOption TimeMasterSpawnRate;
    internal static CustomOption TimeMasterCooldown;
    internal static CustomOption TimeMasterRewindTime;
    internal static CustomOption TimeMasterShieldDuration;

    internal static CustomRoleOption DetectiveSpawnRate;
    internal static CustomOption DetectiveAnonymousFootprints;
    internal static CustomOption DetectiveFootprintInterval;
    internal static CustomOption DetectiveFootprintDuration;
    internal static CustomOption DetectiveReportNameDuration;
    internal static CustomOption DetectiveReportColorDuration;

    internal static CustomRoleOption MediumSpawnRate;
    internal static CustomOption MediumCooldown;
    internal static CustomOption MediumDuration;
    internal static CustomOption MediumOneTimeUse;

    internal static CustomRoleOption HackerSpawnRate;
    internal static CustomOption HackerCooldown;
    internal static CustomOption HackerHackingDuration;
    internal static CustomOption HackerOnlyColorType;
    internal static CustomOption HackerToolsNumber;
    internal static CustomOption HackerRechargeTasksNumber;
    internal static CustomOption HackerNoMove;

    internal static CustomRoleOption TrackerSpawnRate;
    internal static CustomOption TrackerUpdateInterval;
    internal static CustomOption TrackerResetTargetAfterMeeting;
    internal static CustomOption TrackerCanTrackCorpses;
    internal static CustomOption TrackerCorpsesTrackingCooldown;
    internal static CustomOption TrackerCorpsesTrackingDuration;

    internal static CustomRoleOption SnitchSpawnRate;
    internal static CustomOption SnitchLeftTasksForReveal;
    internal static CustomOption SnitchIncludeTeamJackal;
    internal static CustomOption SnitchTeamJackalUseDifferentArrowColor;

    internal static CustomRoleOption LighterSpawnRate;
    internal static CustomOption LighterModeLightsOnVision;
    internal static CustomOption LighterModeLightsOffVision;
    internal static CustomOption LighterCooldown;
    internal static CustomOption LighterDuration;

    internal static CustomRoleOption SecurityGuardSpawnRate;
    internal static CustomOption SecurityGuardCooldown;
    internal static CustomOption SecurityGuardTotalScrews;
    internal static CustomOption SecurityGuardCamPrice;
    internal static CustomOption SecurityGuardVentPrice;
    internal static CustomOption SecurityGuardCamDuration;
    internal static CustomOption SecurityGuardCamMaxCharges;
    internal static CustomOption SecurityGuardCamRechargeTasksNumber;
    internal static CustomOption SecurityGuardNoMove;

    internal static CustomRoleOption SwapperSpawnRate;
    internal static CustomOption SwapperIsImpRate;
    internal static CustomOption SwapperCanCallEmergency;
    internal static CustomOption SwapperCanOnlySwapOthers;
    internal static CustomOption SwapperNumSwaps;

    internal static CustomRoleOption BaitSpawnRate;
    internal static CustomOption BaitHighlightAllVents;
    internal static CustomOption BaitReportDelay;
    internal static CustomOption BaitShowKillFlash;

    internal static CustomRoleOption ShifterSpawnRate;
    internal static CustomOption ShifterIsNeutralRate;
    internal static CustomOption ShifterShiftsModifiers;
    internal static CustomOption ShifterPastShifters;

    internal static CustomRoleOption SheriffSpawnRate;
    internal static CustomOption SheriffCooldown;
    internal static CustomOption SheriffNumShots;
    internal static CustomOption SheriffCanKillNeutrals;
    internal static CustomOption SheriffMisfireKillsTarget;
    internal static CustomOption SheriffCanKillNoDeadBody;

    internal static CustomRoleOption MadmateRoleSpawnRate;
    internal static CustomOption MadmateRoleCanDieToSheriff;
    internal static CustomOption MadmateRoleCanEnterVents;
    internal static CustomOption MadmateRoleHasImpostorVision;
    internal static CustomOption MadmateRoleCanSabotage;
    internal static CustomOption MadmateRoleCanFixComm;
    internal static CustomOption MadmateRoleCanKnowImpostorAfterFinishTasks;
    internal static CustomTasksOption MadmateRoleTasks;

    internal static CustomRoleOption SuiciderSpawnRate;
    internal static CustomOption SuiciderCanDieToSheriff;
    internal static CustomOption SuiciderCanEnterVents;
    internal static CustomOption SuiciderHasImpostorVision;
    internal static CustomOption SuiciderCanFixComm;
    internal static CustomOption SuiciderCanKnowImpostorAfterFinishTasks;
    internal static CustomTasksOption SuiciderTasks;

#endregion

#region ROLES IMPOSTOR

    internal static CustomRoleOption BountyHunterSpawnRate;
    internal static CustomOption BountyHunterBountyDuration;
    internal static CustomOption BountyHunterReducedCooldown;
    internal static CustomOption BountyHunterPunishmentTime;
    internal static CustomOption BountyHunterShowArrow;
    internal static CustomOption BountyHunterArrowUpdateInterval;

    internal static CustomRoleOption MafiaSpawnRate;
    internal static CustomOption MafiosoCanSabotage;
    internal static CustomOption MafiosoCanRepair;
    internal static CustomOption MafiosoCanVent;
    internal static CustomOption JanitorCooldown;
    internal static CustomOption JanitorCanSabotage;
    internal static CustomOption JanitorCanRepair;
    internal static CustomOption JanitorCanVent;

    internal static CustomRoleOption TricksterSpawnRate;
    internal static CustomOption TricksterPlaceBoxCooldown;
    internal static CustomOption TricksterLightsOutCooldown;
    internal static CustomOption TricksterLightsOutDuration;

    internal static CustomRoleOption EvilHackerSpawnRate;
    internal static CustomOption EvilHackerCanHasBetterAdmin;
    internal static CustomOption EvilHackerCanCreateMadmate;
    internal static CustomOption EvilHackerCanCreateMadmateFromJackal;
    internal static CustomOption EvilHackerCanMoveEvenIfUsesAdmin;
    internal static CustomOption EvilHackerCanInheritAbility;
    internal static CustomOption EvilHackerCanSeeDoorStatus;
    internal static CustomOption CreatedMadmateCanDieToSheriff;
    internal static CustomOption CreatedMadmateCanEnterVents;
    internal static CustomOption CreatedMadmateHasImpostorVision;
    internal static CustomOption CreatedMadmateCanSabotage;
    internal static CustomOption CreatedMadmateCanFixComm;
    internal static CustomOption CreatedMadmateAbility;
    internal static CustomOption CreatedMadmateNumTasks;
    internal static CustomOption CreatedMadmateExileCrewmate;

    internal static CustomRoleOption EvilTrackerSpawnRate;
    internal static CustomOption EvilTrackerCooldown;
    internal static CustomOption EvilTrackerResetTargetAfterMeeting;
    internal static CustomOption EvilTrackerCanSeeDeathFlash;
    internal static CustomOption EvilTrackerCanSeeTargetTask;
    internal static CustomOption EvilTrackerCanSeeTargetPosition;
    internal static CustomOption EvilTrackerCanSetTargetOnMeeting;

    internal static CustomRoleOption EraserSpawnRate;
    internal static CustomOption EraserCooldown;
    internal static CustomOption EraserCooldownIncrease;
    internal static CustomOption EraserCanEraseAnyone;

    internal static CustomRoleOption MorphingSpawnRate;
    internal static CustomOption MorphingCooldown;
    internal static CustomOption MorphingDuration;

    internal static CustomRoleOption CamouflagerSpawnRate;
    internal static CustomOption CamouflagerCooldown;
    internal static CustomOption CamouflagerDuration;
    internal static CustomOption CamouflagerRandomColors;

    internal static CustomRoleOption CleanerSpawnRate;
    internal static CustomOption CleanerCooldown;

    internal static CustomRoleOption WarlockSpawnRate;
    internal static CustomOption WarlockCooldown;
    internal static CustomOption WarlockRootTime;

    internal static CustomRoleOption WitchSpawnRate;
    internal static CustomOption WitchCooldown;
    internal static CustomOption WitchAdditionalCooldown;
    internal static CustomOption WitchCanSpellAnyone;
    internal static CustomOption WitchSpellCastingDuration;
    internal static CustomOption WitchTriggerBothCooldowns;
    internal static CustomOption WitchVoteSavesTargets;

    internal static CustomRoleOption VampireSpawnRate;
    internal static CustomOption VampireKillDelay;
    internal static CustomOption VampireCooldown;
    internal static CustomOption VampireCanKillNearGarlics;

#endregion

#region ROLES NEUTRAL

    internal static CustomRoleOption JesterSpawnRate;
    internal static CustomOption JesterCanCallEmergency;
    internal static CustomOption JesterCanSabotage;
    internal static CustomOption JesterHasImpostorVision;

    internal static CustomRoleOption ArsonistSpawnRate;
    internal static CustomOption ArsonistCooldown;
    internal static CustomOption ArsonistDuration;
    internal static CustomOption ArsonistCanBeLovers;

    internal static CustomRoleOption VultureSpawnRate;
    internal static CustomOption VultureCooldown;
    internal static CustomOption VultureNumberToWin;
    internal static CustomOption VultureCanUseVents;
    internal static CustomOption VultureShowArrows;

    internal static CustomRoleOption JackalSpawnRate;
    internal static CustomOption JackalKillCooldown;
    internal static CustomOption JackalCreateSidekickCooldown;
    internal static CustomOption JackalCanSabotageLights;
    internal static CustomOption JackalCanUseVents;
    internal static CustomOption JackalCanCreateSidekick;
    internal static CustomOption JackalHasImpostorVision;
    internal static CustomOption SidekickPromotesToJackal;
    internal static CustomOption SidekickCanKill;
    internal static CustomOption SidekickCanUseVents;
    internal static CustomOption SidekickCanSabotageLights;
    internal static CustomOption SidekickHasImpostorVision;
    internal static CustomOption JackalPromotedFromSidekickCanCreateSidekick;
    internal static CustomOption JackalCanCreateSidekickFromImpostor;

    internal static CustomRoleOption GuesserSpawnRate;
    internal static CustomOption GuesserIsImpGuesserRate;
    internal static CustomOption GuesserNumberOfShots;
    internal static CustomOption GuesserOnlyAvailableRoles;
    internal static CustomOption GuesserHasMultipleShotsPerMeeting;
    internal static CustomOption GuesserShowInfoInGhostChat;
    internal static CustomOption GuesserKillsThroughShield;
    internal static CustomOption GuesserEvilCanKillSpy;
    internal static CustomOption GuesserSpawnBothRate;

#endregion

#region MODIFIERS

    internal static CustomModifierOption MadmateSpawnRate;
    internal static CustomOption MadmateCanDieToSheriff;
    internal static CustomOption MadmateCanEnterVents;
    internal static CustomOption MadmateHasImpostorVision;
    internal static CustomOption MadmateCanSabotage;
    internal static CustomOption MadmateCanFixComm;
    internal static CustomOption MadmateType;
    internal static CustomRoleSelectionOption MadmateFixedRole;
    internal static CustomOption MadmateAbility;
    internal static CustomTasksOption MadmateTasks;
    internal static CustomOption MadmateExilePlayer;

    internal static CustomOption LastImpostorEnable;
    internal static CustomOption LastImpostorNumKills;
    internal static CustomOption LastImpostorFunctions;
    internal static CustomOption LastImpostorResults;
    internal static CustomOption LastImpostorNumShots;

    internal static CustomRoleOption LoversSpawnRate;
    internal static CustomOption LoversNumCouples;
    internal static CustomOption LoversImpLoverRate;
    internal static CustomOption LoversBothDie;
    internal static CustomOption LoversCanHaveAnotherRole;
    internal static CustomOption LoversSeparateTeam;
    internal static CustomOption LoversTasksCount;
    internal static CustomOption LoversEnableChat;

    internal static CustomModifierOption MiniSpawnRate;
    internal static CustomOption MiniGrowingUpDuration;

    internal static CustomModifierOption AntiTeleportSpawnRate;

#endregion
}