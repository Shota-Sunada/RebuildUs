namespace RebuildUs;

internal static class CustomOptionHolder
{
    internal static readonly string[] Rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    private static readonly TrKey[] Presets = [TrKey.Preset1, TrKey.Preset2, TrKey.Preset3, TrKey.Preset4, TrKey.Preset5];

    internal static readonly Dictionary<byte, byte[]> BlockedRolePairings = [];

    internal static void Load()
    {
        var presetHeader = new CustomOptionHeader(COType.General, TrKey.Preset);
        var rolesGeneralHeader = new CustomOptionHeader(COType.General, TrKey.RolesGeneral);
        var gameOptionsHeader = new CustomOptionHeader(COType.General, TrKey.GameOptions);
        var polusOptionsHeader = new CustomOptionHeader(COType.General, TrKey.PolusOptions);
        var airshipOptionsHeader = new CustomOptionHeader(COType.General, TrKey.AirshipOptions);
        var randomMapHeader = new CustomOptionHeader(COType.General, TrKey.RandomMap);
        var gameModeHeader = new CustomOptionHeader(COType.General, TrKey.GameMode);

        var hideNSeekHeader = new CustomOptionHeader(COType.HideNSeek, TrKey.HideNSeek);
        var battleRoyaleHeader = new CustomOptionHeader(COType.General, TrKey.BattleRoyale);
        var hotPotatoHeader = new CustomOptionHeader(COType.General, TrKey.HotPotato);

        #region MOD OPTIONS

        PresetSelection = CustomOption.Normal(COID.PresetSelection, COType.General, TrKey.Preset, Presets, 0, header: presetHeader);
        ActivateRoles = CustomOption.Normal(COID.ActivateRoles, COType.General, TrKey.ActivateRoles, true);
        GameModeSelection = CustomOption.Normal(COID.GameModeSelection, COType.General, TrKey.GameMode, [TrKey.GameModeNormal, TrKey.GameModeHideNSeek, TrKey.GameModeBattleRoyale, TrKey.GameModeHotPotato], 0, header: gameModeHeader);
        RandomNumberAlgorithm = CustomOption.Normal(COID.RandomNumberAlgorithm, COType.General, TrKey.RandomNumberAlgorithm, RandomMain.RNAs, 0);
        EnableRandomRandomNumberAlgorithm = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithm, COType.General, TrKey.RandomRandomNumberAlgorithm, false);
        EnableRandomRandomNumberAlgorithmDotnet = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithmDotnet, COType.General, TrKey.EnableRandomRandomNumberAlgorithmDotnet, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmMT = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithmMT, COType.General, TrKey.EnableRandomRandomNumberAlgorithmMT, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Pp = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithmXorshiro256Pp, COType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256PP, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmXorshiro256Ss = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithmXorshiro256Ss, COType.General, TrKey.EnableRandomRandomNumberAlgorithmXorshiro256SS, true, EnableRandomRandomNumberAlgorithm);
        EnableRandomRandomNumberAlgorithmPcg64 = CustomOption.Normal(COID.EnableRandomRandomNumberAlgorithmPcg64, COType.General, TrKey.EnableRandomRandomNumberAlgorithmPcg64, true, EnableRandomRandomNumberAlgorithm);
        SendEmbedToDiscord = CustomOption.Normal(COID.SendEmbedToDiscord, COType.General, TrKey.SendEmbedToDiscord, false);

        #endregion

        #region GENERAL OPTIONS

        CrewmateRolesCountMin = CustomOption.Normal(COID.CrewmateRolesCountMin, COType.General, TrKey.CrewmateRolesCountMin, 0f, 0f, 15f, 1f, header: rolesGeneralHeader, format: TrKey.UnitPlayers);
        CrewmateRolesCountMax = CustomOption.Normal(COID.CrewmateRolesCountMax, COType.General, TrKey.CrewmateRolesCountMax, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        ImpostorRolesCountMin = CustomOption.Normal(COID.ImpostorRolesCountMin, COType.General, TrKey.ImpostorRolesCountMin, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        ImpostorRolesCountMax = CustomOption.Normal(COID.ImpostorRolesCountMax, COType.General, TrKey.ImpostorRolesCountMax, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        NeutralRolesCountMin = CustomOption.Normal(COID.NeutralRolesCountMin, COType.General, TrKey.NeutralRolesCountMin, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        NeutralRolesCountMax = CustomOption.Normal(COID.NeutralRolesCountMax, COType.General, TrKey.NeutralRolesCountMax, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        ModifiersCountMin = CustomOption.Normal(COID.ModifiersCountMin, COType.General, TrKey.ModifiersCountMin, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        ModifiersCountMax = CustomOption.Normal(COID.ModifiersCountMax, COType.General, TrKey.ModifiersCountMax, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);

        #endregion

        #region GAME OPTIONS

        MaxNumberOfMeetings = CustomOption.Normal(COID.MaxNumberOfMeetings, COType.General, TrKey.MaxNumberOfMeetings, 10, 0, 15, 1, header: gameOptionsHeader, format: TrKey.UnitTimes);
        BlockSkippingInEmergencyMeetings = CustomOption.Normal(COID.BlockSkippingInEmergencyMeetings, COType.General, TrKey.BlockSkippingInEmergencyMeetings, false);
        NoVoteIsSelfVote = CustomOption.Normal(COID.NoVoteIsSelfVote, COType.General, TrKey.NoVoteIsSelfVote, false);
        HidePlayerNames = CustomOption.Normal(COID.HidePlayerNames, COType.General, TrKey.HidePlayerNames, false);
        AllowParallelMedBayScans = CustomOption.Normal(COID.AllowParallelMedBayScans, COType.General, TrKey.AllowParallelMedBayScans, false);
        HideOutOfSightNametags = CustomOption.Normal(COID.HideOutOfSightNametags, COType.General, TrKey.HideOutOfSightNametags, true);
        RefundVotesOnDeath = CustomOption.Normal(COID.RefundVotesOnDeath, COType.General, TrKey.RefundVotesOnDeath, true);
        DelayBeforeMeeting = CustomOption.Normal(COID.DelayBeforeMeeting, COType.General, TrKey.DelayBeforeMeeting, 0f, 0f, 10f, 0.25f, format: TrKey.UnitSeconds);
        DisableVentAnimation = CustomOption.Normal(COID.DisableVentAnimation, COType.General, TrKey.DisableVentAnimation, false);
        StopCooldownOnFixingElecSabotage = CustomOption.Normal(COID.StopCooldownOnFixingElecSabotage, COType.General, TrKey.StopCooldownOnFixingElecSabotage, true);
        EnableHawkMode = CustomOption.Normal(COID.EnableHawkMode, COType.General, TrKey.EnableHawkMode, true);
        CanWinByTaskWithoutLivingPlayer = CustomOption.Normal(COID.CanWinByTaskWithoutLivingPlayer, COType.General, TrKey.CanWinByTaskLivingPlayer, true);
        // DeadPlayerCanSeeCooldown = CustomOption.Normal(32, CustomOptionType.General, "DeadPlayerCanSeeCooldown", true);
        ImpostorCanIgnoreCommSabotage = CustomOption.Normal(COID.ImpostorCanIgnoreCommSabotage, COType.General, TrKey.ImpostorCanIgnoreCommSabotage, false);
        // BlockSabotageFromDeadImpostors = CustomOption.Normal(34, CustomOptionType.General, "BlockSabotageFromDeadImpostors", false);
        // ShieldFirstKill = CustomOption.Normal(35, CustomOptionType.General, "ShieldFirstKill", false);
        DontFinishGame = CustomOption.Normal(COID.DontFinishGame, COType.General, TrKey.DontFinishGame, false);
        RandomSpawn = CustomOption.Normal(COID.RandomSpawn, COType.General, TrKey.RandomSpawn, false);

        AdditionalEmergencyCooldown = CustomOption.Normal(COID.AdditionalEmergencyCooldown, COType.General, TrKey.AdditionalEmergencyCooldown, 0f, 0f, 15f, 1f, format: TrKey.UnitPlayers);
        AdditionalEmergencyCooldownTime = CustomOption.Normal(COID.AdditionalEmergencyCooldownTime, COType.General, TrKey.AdditionalEmergencyCooldownTime, 10f, 0f, 60f, 1f, AdditionalEmergencyCooldown, format: TrKey.UnitSeconds);

        RestrictDevices = CustomOption.Normal(COID.RestrictDevices, COType.General, TrKey.RestrictDevices, [TrKey.Off, TrKey.RestrictPerTurn, TrKey.RestrictPerGame], 0);
        RestrictAdmin = CustomOption.Normal(COID.RestrictAdmin, COType.General, TrKey.RestrictAdmin, true, RestrictDevices);
        RestrictAdminTime = CustomOption.Normal(COID.RestrictAdminTime, COType.General, TrKey.RestrictAdminTime, 30f, 0f, 600f, 1f, RestrictAdmin, format: TrKey.UnitSeconds);
        RestrictAdminText = CustomOption.Normal(COID.RestrictAdminText, COType.General, TrKey.RestrictAdminText, true, RestrictAdmin);
        RestrictCameras = CustomOption.Normal(COID.RestrictCameras, COType.General, TrKey.RestrictCameras, true, RestrictDevices);
        RestrictCamerasTime = CustomOption.Normal(COID.RestrictCamerasTime, COType.General, TrKey.RestrictCamerasTime, 30f, 0f, 600f, 1f, RestrictCameras, format: TrKey.UnitSeconds);
        RestrictCamerasText = CustomOption.Normal(COID.RestrictCamerasText, COType.General, TrKey.RestrictCamerasText, true, RestrictCameras);
        RestrictVitals = CustomOption.Normal(COID.RestrictVitals, COType.General, TrKey.RestrictVitals, true, RestrictDevices);
        RestrictVitalsTime = CustomOption.Normal(COID.RestrictVitalsTime, COType.General, TrKey.RestrictVitalsTime, 30f, 0f, 600f, 1f, RestrictVitals, format: TrKey.UnitSeconds);
        RestrictVitalsText = CustomOption.Normal(COID.RestrictVitalsText, COType.General, TrKey.RestrictVitalsText, true, RestrictVitals);

        #endregion

        #region POLUS OPTIONS

        PolusAdditionalVents = CustomOption.Normal(COID.PolusAdditionalVents, COType.General, TrKey.PolusAdditionalVents, true, header: polusOptionsHeader);
        PolusSpecimenVital = CustomOption.Normal(COID.PolusSpecimenVital, COType.General, TrKey.PolusSpecimenVital, true);

        #endregion

        #region AIRSHIP OPTIONS

        AirshipOptimize = CustomOption.Normal(COID.AirshipOptimize, COType.General, TrKey.AirshipOptimize, false, header: airshipOptionsHeader);
        AirshipEnableWallCheck = CustomOption.Normal(COID.AirshipEnableWallCheck, COType.General, TrKey.AirshipEnableWallCheck, true);
        AirshipReactorDuration = CustomOption.Normal(COID.AirshipReactorDuration, COType.General, TrKey.AirshipReactorDuration, 60f, 0f, 600f, 1f, format: TrKey.UnitSeconds);
        AirshipRandomSpawn = CustomOption.Normal(COID.AirshipRandomSpawn, COType.General, TrKey.AirshipRandomSpawn, false);
        AirshipAdditionalSpawn = CustomOption.Normal(COID.AirshipAdditionalSpawn, COType.General, TrKey.AirshipAdditionalSpawn, true);
        AirshipSynchronizedSpawning = CustomOption.Normal(COID.AirshipSynchronizedSpawning, COType.General, TrKey.AirshipSynchronizedSpawning, true);
        AirshipSetOriginalCooldown = CustomOption.Normal(COID.AirshipSetOriginalCooldown, COType.General, TrKey.AirshipSetOriginalCooldown, false);
        AirshipInitialDoorCooldown = CustomOption.Normal(COID.AirshipInitialDoorCooldown, COType.General, TrKey.AirshipInitialDoorCooldown, 0f, 0f, 60f, 1f, format: TrKey.UnitSeconds);
        AirshipInitialSabotageCooldown = CustomOption.Normal(COID.AirshipInitialSabotageCooldown, COType.General, TrKey.AirshipInitialSabotageCooldown, 15f, 0f, 60f, 1f, format: TrKey.UnitSeconds);
        AirshipOldAdmin = CustomOption.Normal(COID.AirshipOldAdmin, COType.General, TrKey.AirshipOldAdmin, false);
        AirshipRestrictedAdmin = CustomOption.Normal(COID.AirshipRestrictedAdmin, COType.General, TrKey.AirshipRestrictedAdmin, false);
        AirshipDisableGapSwitchBoard = CustomOption.Normal(COID.AirshipDisableGapSwitchBoard, COType.General, TrKey.AirshipDisableGapSwitchBoard, false);
        AirshipDisableMovingPlatform = CustomOption.Normal(COID.AirshipDisableMovingPlatform, COType.General, TrKey.AirshipDisableMovingPlatform, false);
        AirshipAdditionalLadder = CustomOption.Normal(COID.AirshipAdditionalLadder, COType.General, TrKey.AirshipAdditionalLadder, false);
        AirshipOneWayLadder = CustomOption.Normal(COID.AirshipOneWayLadder, COType.General, TrKey.AirshipOneWayLadder, false);
        AirshipReplaceSafeTask = CustomOption.Normal(COID.AirshipReplaceSafeTask, COType.General, TrKey.AirshipReplaceSafeTask, false);
        AirshipAdditionalWireTask = CustomOption.Normal(COID.AirshipAdditionalWireTask, COType.General, TrKey.AirshipAdditionalWireTask, false);

        #endregion

        #region MAP OPTIONS

        RandomMap = CustomOption.Normal(COID.RandomMap, COType.General, TrKey.RandomMap, false, header: randomMapHeader);
        RandomMapEnableSkeld = CustomOption.Normal(COID.RandomMapEnableSkeld, COType.General, TrKey.RandomMapEnableSkeld, true, RandomMap);
        RandomMapEnableMiraHq = CustomOption.Normal(COID.RandomMapEnableMiraHq, COType.General, TrKey.RandomMapEnableMiraHQ, true, RandomMap);
        RandomMapEnablePolus = CustomOption.Normal(COID.RandomMapEnablePolus, COType.General, TrKey.RandomMapEnablePolus, true, RandomMap);
        RandomMapEnableDleks = CustomOption.Normal(COID.RandomMapEnableDleks, COType.General, TrKey.RandomMapEnableDleks, true, RandomMap);
        RandomMapEnableAirShip = CustomOption.Normal(COID.RandomMapEnableAirShip, COType.General, TrKey.RandomMapEnableAirShip, true, RandomMap);
        RandomMapEnableFungle = CustomOption.Normal(COID.RandomMapEnableFungle, COType.General, TrKey.RandomMapEnableFungle, true, RandomMap);
        RandomMapEnableSubmerged = CustomOption.Normal(COID.RandomMapEnableSubmerged, COType.General, TrKey.RandomMapEnableSubmerged, true, RandomMap);

        #endregion

        #region ROLES CREWMATE

        MayorSpawnRate = new(COID.MayorSpawnRate, COType.Crewmate, RoleType.Mayor, Mayor.Color);
        MayorNumVotes = CustomOption.Normal(COID.MayorNumVotes, COType.Crewmate, TrKey.MayorNumVotes, 2f, 2f, 10f, 1f, MayorSpawnRate, format: TrKey.UnitTimes);
        MayorCanSeeVoteColors = CustomOption.Normal(COID.MayorCanSeeVoteColors, COType.Crewmate, TrKey.MayorCanSeeVoteColors, false, MayorSpawnRate);
        MayorTasksNeededToSeeVoteColors = CustomOption.Normal(COID.MayorTasksNeededToSeeVoteColors, COType.Crewmate, TrKey.MayorTasksNeededToSeeVoteColors, 3f, 1f, 10f, 1f, MayorCanSeeVoteColors);
        MayorMeetingButton = CustomOption.Normal(COID.MayorMeetingButton, COType.Crewmate, TrKey.MayorMeetingButton, true, MayorSpawnRate);
        MayorMaxRemoteMeetings = CustomOption.Normal(COID.MayorMaxRemoteMeetings, COType.Crewmate, TrKey.MayorMaxRemoteMeetings, 1f, 0f, 10f, 1f, MayorMeetingButton, format: TrKey.UnitTimes);

        EngineerSpawnRate = new(COID.EngineerSpawnRate, COType.Crewmate, RoleType.Engineer, Engineer.Color);
        EngineerNumberOfFixes = CustomOption.Normal(COID.EngineerNumberOfFixes, COType.Crewmate, TrKey.EngineerNumberOfFixes, 1f, 0f, 3f, 1f, EngineerSpawnRate, format: TrKey.UnitTimes);
        EngineerHighlightForImpostors = CustomOption.Normal(COID.EngineerHighlightForImpostors, COType.Crewmate, TrKey.EngineerHighlightForImpostors, true, EngineerSpawnRate);
        EngineerHighlightForTeamJackal = CustomOption.Normal(COID.EngineerHighlightForTeamJackal, COType.Crewmate, TrKey.EngineerHighlightForTeamJackal, true, EngineerSpawnRate);

        SpySpawnRate = new(COID.SpySpawnRate, COType.Crewmate, RoleType.Spy, Spy.Color, 1);
        SpyCanDieToSheriff = CustomOption.Normal(COID.SpyCanDieToSheriff, COType.Crewmate, TrKey.SpyCanDieToSheriff, false, SpySpawnRate);
        SpyImpostorsCanKillAnyone = CustomOption.Normal(COID.SpyImpostorsCanKillAnyone, COType.Crewmate, TrKey.SpyImpostorsCanKillAnyone, true, SpySpawnRate);
        SpyCanEnterVents = CustomOption.Normal(COID.SpyCanEnterVents, COType.Crewmate, TrKey.SpyCanEnterVents, false, SpySpawnRate);
        SpyHasImpostorVision = CustomOption.Normal(COID.SpyHasImpostorVision, COType.Crewmate, TrKey.SpyHasImpostorVision, false, SpySpawnRate);

        MedicSpawnRate = new(COID.MedicSpawnRate, COType.Crewmate, RoleType.Medic, Medic.Color, 1);
        MedicShowShielded = CustomOption.Normal(COID.MedicShowShielded, COType.Crewmate, TrKey.MedicShowShielded, [TrKey.MedicShowShieldedAll, TrKey.MedicShowShieldedBoth, TrKey.MedicShowShieldedMedic], 0, MedicSpawnRate);
        MedicShowAttemptToShielded = CustomOption.Normal(COID.MedicShowAttemptToShielded, COType.Crewmate, TrKey.MedicShowAttemptToShielded, false, MedicSpawnRate);
        MedicSetShieldAfterMeeting = CustomOption.Normal(COID.MedicSetShieldAfterMeeting, COType.Crewmate, TrKey.MedicSetShieldAfterMeeting, false, MedicSpawnRate);
        MedicShowAttemptToMedic = CustomOption.Normal(COID.MedicShowAttemptToMedic, COType.Crewmate, TrKey.MedicSeesMurderAttempt, false, MedicSpawnRate);

        SeerSpawnRate = new(COID.SeerSpawnRate, COType.Crewmate, RoleType.Seer, Seer.Color, 1);
        SeerMode = CustomOption.Normal(COID.SeerMode, COType.Crewmate, TrKey.SeerMode, [TrKey.SeerModeBoth, TrKey.SeerModeFlash, TrKey.SeerModeSouls], 0, SeerSpawnRate);
        SeerLimitSoulDuration = CustomOption.Normal(COID.SeerLimitSoulDuration, COType.Crewmate, TrKey.SeerLimitSoulDuration, false, SeerSpawnRate);
        SeerSoulDuration = CustomOption.Normal(COID.SeerSoulDuration, COType.Crewmate, TrKey.SeerSoulDuration, 15f, 0f, 120f, 5f, SeerLimitSoulDuration, format: TrKey.UnitSeconds);

        TimeMasterSpawnRate = new(COID.TimeMasterSpawnRate, COType.Crewmate, RoleType.TimeMaster, TimeMaster.Color, 1);
        TimeMasterCooldown = CustomOption.Normal(COID.TimeMasterCooldown, COType.Crewmate, TrKey.TimeMasterCooldown, 30f, 2.5f, 120f, 2.5f, TimeMasterSpawnRate, format: TrKey.UnitSeconds);
        TimeMasterRewindTime = CustomOption.Normal(COID.TimeMasterRewindTime, COType.Crewmate, TrKey.TimeMasterRewindTime, 3f, 1f, 10f, 1f, TimeMasterSpawnRate, format: TrKey.UnitSeconds);
        TimeMasterShieldDuration = CustomOption.Normal(COID.TimeMasterShieldDuration, COType.Crewmate, TrKey.TimeMasterShieldDuration, 3f, 1f, 20f, 1f, TimeMasterSpawnRate, format: TrKey.UnitSeconds);

        DetectiveSpawnRate = new(COID.DetectiveSpawnRate, COType.Crewmate, RoleType.Detective, Detective.Color, 1);
        DetectiveAnonymousFootprints = CustomOption.Normal(COID.DetectiveAnonymousFootprints, COType.Crewmate, TrKey.DetectiveAnonymousFootprints, false, DetectiveSpawnRate);
        DetectiveFootprintInterval = CustomOption.Normal(COID.DetectiveFootprintInterval, COType.Crewmate, TrKey.DetectiveFootprintInterval, 0.5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate, format: TrKey.UnitSeconds);
        DetectiveFootprintDuration = CustomOption.Normal(COID.DetectiveFootprintDuration, COType.Crewmate, TrKey.DetectiveFootprintDuration, 5f, 0.25f, 10f, 0.25f, DetectiveSpawnRate, format: TrKey.UnitSeconds);
        DetectiveReportNameDuration = CustomOption.Normal(COID.DetectiveReportNameDuration, COType.Crewmate, TrKey.DetectiveReportNameDuration, 10f, 0, 60, 2.5f, DetectiveSpawnRate, format: TrKey.UnitSeconds);
        DetectiveReportColorDuration = CustomOption.Normal(COID.DetectiveReportColorDuration, COType.Crewmate, TrKey.DetectiveReportColorDuration, 20, 0, 120, 2.5f, DetectiveSpawnRate, format: TrKey.UnitSeconds);

        MediumSpawnRate = new(COID.MediumSpawnRate, COType.Crewmate, RoleType.Medium, Medium.Color, 1);
        MediumCooldown = CustomOption.Normal(COID.MediumCooldown, COType.Crewmate, TrKey.MediumCooldown, 30f, 5f, 120f, 5f, MediumSpawnRate, format: TrKey.UnitSeconds);
        MediumDuration = CustomOption.Normal(COID.MediumDuration, COType.Crewmate, TrKey.MediumDuration, 3f, 0f, 15f, 1f, MediumSpawnRate, format: TrKey.UnitSeconds);
        MediumOneTimeUse = CustomOption.Normal(COID.MediumOneTimeUse, COType.Crewmate, TrKey.MediumOneTimeUse, false, MediumSpawnRate);

        HackerSpawnRate = new(COID.HackerSpawnRate, COType.Crewmate, RoleType.Hacker, Hacker.Color, 1);
        HackerCooldown = CustomOption.Normal(COID.HackerCooldown, COType.Crewmate, TrKey.HackerCooldown, 30f, 5f, 60f, 5f, HackerSpawnRate, format: TrKey.UnitSeconds);
        HackerHackingDuration = CustomOption.Normal(COID.HackerHackingDuration, COType.Crewmate, TrKey.HackerHackingDuration, 10f, 2.5f, 60f, 2.5f, HackerSpawnRate, format: TrKey.UnitSeconds);
        HackerOnlyColorType = CustomOption.Normal(COID.HackerOnlyColorType, COType.Crewmate, TrKey.HackerOnlyColorType, false, HackerSpawnRate);
        HackerToolsNumber = CustomOption.Normal(COID.HackerToolsNumber, COType.Crewmate, TrKey.HackerToolsNumber, 5f, 1f, 30f, 1f, HackerSpawnRate, format: TrKey.UnitTimes);
        HackerRechargeTasksNumber = CustomOption.Normal(COID.HackerRechargeTasksNumber, COType.Crewmate, TrKey.HackerRechargeTasksNumber, 2f, 1f, 5f, 1f, HackerSpawnRate);
        HackerNoMove = CustomOption.Normal(COID.HackerNoMove, COType.Crewmate, TrKey.HackerNoMove, true, HackerSpawnRate);

        TrackerSpawnRate = new(COID.TrackerSpawnRate, COType.Crewmate, RoleType.Tracker, Tracker.Color, 1);
        TrackerUpdateInterval = CustomOption.Normal(COID.TrackerUpdateInterval, COType.Crewmate, TrKey.TrackerUpdateInterval, 5f, 1f, 30f, 1f, TrackerSpawnRate, format: TrKey.UnitSeconds);
        TrackerResetTargetAfterMeeting = CustomOption.Normal(COID.TrackerResetTargetAfterMeeting, COType.Crewmate, TrKey.TrackerResetTargetAfterMeeting, false, TrackerSpawnRate);
        TrackerCanTrackCorpses = CustomOption.Normal(COID.TrackerCanTrackCorpses, COType.Crewmate, TrKey.TrackerTrackCorpses, true, TrackerSpawnRate);
        TrackerCorpsesTrackingCooldown = CustomOption.Normal(COID.TrackerCorpsesTrackingCooldown, COType.Crewmate, TrKey.TrackerCorpseCooldown, 30f, 0f, 120f, 5f, TrackerCanTrackCorpses, format: TrKey.UnitSeconds);
        TrackerCorpsesTrackingDuration = CustomOption.Normal(COID.TrackerCorpsesTrackingDuration, COType.Crewmate, TrKey.TrackerCorpseDuration, 5f, 2.5f, 30f, 2.5f, TrackerCanTrackCorpses, format: TrKey.UnitSeconds);

        SnitchSpawnRate = new(COID.SnitchSpawnRate, COType.Crewmate, RoleType.Snitch, Snitch.Color, 1);
        SnitchLeftTasksForReveal = CustomOption.Normal(COID.SnitchLeftTasksForReveal, COType.Crewmate, TrKey.SnitchLeftTasksForReveal, 1f, 0f, 5f, 1f, SnitchSpawnRate);
        SnitchIncludeTeamJackal = CustomOption.Normal(COID.SnitchIncludeTeamJackal, COType.Crewmate, TrKey.SnitchIncludeTeamJackal, false, SnitchSpawnRate);
        SnitchTeamJackalUseDifferentArrowColor = CustomOption.Normal(COID.SnitchTeamJackalUseDifferentArrowColor, COType.Crewmate, TrKey.SnitchTeamJackalUseDifferentArrowColor, true, SnitchIncludeTeamJackal);

        LighterSpawnRate = new(COID.LighterSpawnRate, COType.Crewmate, RoleType.Lighter, Lighter.Color);
        LighterModeLightsOnVision = CustomOption.Normal(COID.LighterModeLightsOnVision, COType.Crewmate, TrKey.LighterModeLightsOnVision, 2f, 0.25f, 5f, 0.25f, LighterSpawnRate, format: TrKey.UnitMultiplies);
        LighterModeLightsOffVision = CustomOption.Normal(COID.LighterModeLightsOffVision, COType.Crewmate, TrKey.LighterModeLightsOffVision, 0.75f, 0.25f, 5f, 0.25f, LighterSpawnRate, format: TrKey.UnitMultiplies);
        LighterCooldown = CustomOption.Normal(COID.LighterCooldown, COType.Crewmate, TrKey.LighterCooldown, 30f, 5f, 120f, 5f, LighterSpawnRate, format: TrKey.UnitSeconds);
        LighterDuration = CustomOption.Normal(COID.LighterDuration, COType.Crewmate, TrKey.LighterDuration, 5f, 2.5f, 60f, 2.5f, LighterSpawnRate, format: TrKey.UnitSeconds);
        // lighterCanSeeNinja = CustomOption.Normal(1115, CustomOptionType.Crewmate, "lighterCanSeeNinja", true, lighterSpawnRate);

        SecurityGuardSpawnRate = new(COID.SecurityGuardSpawnRate, COType.Crewmate, RoleType.SecurityGuard, SecurityGuard.Color, 1);
        SecurityGuardCooldown = CustomOption.Normal(COID.SecurityGuardCooldown, COType.Crewmate, TrKey.SecurityGuardCooldown, 30f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate, format: TrKey.UnitSeconds);
        SecurityGuardTotalScrews = CustomOption.Normal(COID.SecurityGuardTotalScrews, COType.Crewmate, TrKey.SecurityGuardTotalScrews, 7f, 1f, 15f, 1f, SecurityGuardSpawnRate, format: TrKey.UnitScrews);
        SecurityGuardCamPrice = CustomOption.Normal(COID.SecurityGuardCamPrice, COType.Crewmate, TrKey.SecurityGuardCamPrice, 2f, 1f, 15f, 1f, SecurityGuardSpawnRate, format: TrKey.UnitScrews);
        SecurityGuardVentPrice = CustomOption.Normal(COID.SecurityGuardVentPrice, COType.Crewmate, TrKey.SecurityGuardVentPrice, 1f, 1f, 15f, 1f, SecurityGuardSpawnRate, format: TrKey.UnitScrews);
        SecurityGuardCamDuration = CustomOption.Normal(COID.SecurityGuardCamDuration, COType.Crewmate, TrKey.SecurityGuardCamDuration, 10f, 2.5f, 60f, 2.5f, SecurityGuardSpawnRate, format: TrKey.UnitSeconds);
        SecurityGuardCamMaxCharges = CustomOption.Normal(COID.SecurityGuardCamMaxCharges, COType.Crewmate, TrKey.SecurityGuardCamMaxCharges, 5f, 1f, 30f, 1f, SecurityGuardSpawnRate, format: TrKey.UnitTimes);
        SecurityGuardCamRechargeTasksNumber = CustomOption.Normal(COID.SecurityGuardCamRechargeTasksNumber, COType.Crewmate, TrKey.SecurityGuardCamRechargeTasksNumber, 3f, 1f, 10f, 1f, SecurityGuardSpawnRate);

        SwapperSpawnRate = new(COID.SwapperSpawnRate, COType.Neutral, TrKey.Swapper, Swapper.Color, 1);
        SwapperIsImpRate = CustomOption.Normal(COID.SwapperIsImpRate, COType.Neutral, TrKey.SwapperIsImpRate, Rates, 0, SwapperSpawnRate);
        SwapperNumSwaps = CustomOption.Normal(COID.SwapperNumSwaps, COType.Neutral, TrKey.SwapperNumSwaps, 2f, 1f, 15f, 1f, SwapperSpawnRate, format: TrKey.UnitTimes);
        SwapperCanCallEmergency = CustomOption.Normal(COID.SwapperCanCallEmergency, COType.Neutral, TrKey.SwapperCanCallEmergency, false, SwapperSpawnRate);
        SwapperCanOnlySwapOthers = CustomOption.Normal(COID.SwapperCanOnlySwapOthers, COType.Neutral, TrKey.SwapperCanOnlySwapOthers, false, SwapperSpawnRate);

        BaitSpawnRate = new(COID.BaitSpawnRate, COType.Crewmate, RoleType.Bait, Bait.Color, 1);
        BaitHighlightAllVents = CustomOption.Normal(COID.BaitHighlightAllVents, COType.Crewmate, TrKey.BaitHighlightAllVents, false, BaitSpawnRate);
        BaitReportDelay = CustomOption.Normal(COID.BaitReportDelay, COType.Crewmate, TrKey.BaitReportDelay, 0f, 0f, 10f, 1f, BaitSpawnRate, format: TrKey.UnitSeconds);
        BaitShowKillFlash = CustomOption.Normal(COID.BaitShowKillFlash, COType.Crewmate, TrKey.BaitShowKillFlash, true, BaitSpawnRate);

        ShifterSpawnRate = new(COID.ShifterSpawnRate, COType.Neutral, RoleType.Shifter, Shifter.Color, 1);
        ShifterIsNeutralRate = CustomOption.Normal(COID.ShifterIsNeutralRate, COType.Neutral, TrKey.ShifterIsNeutralRate, Rates, 0, ShifterSpawnRate);
        ShifterShiftsModifiers = CustomOption.Normal(COID.ShifterShiftsModifiers, COType.Neutral, TrKey.ShifterShiftsModifiers, false, ShifterSpawnRate);
        ShifterPastShifters = CustomOption.Normal(COID.ShifterPastShifters, COType.Neutral, TrKey.ShifterPastShifters, false, ShifterSpawnRate);

        SheriffSpawnRate = new(COID.SheriffSpawnRate, COType.Crewmate, RoleType.Sheriff, Sheriff.Color);
        SheriffCooldown = CustomOption.Normal(COID.SheriffCooldown, COType.Crewmate, TrKey.SheriffCooldown, 30f, 2.5f, 60f, 2.5f, SheriffSpawnRate, format: TrKey.UnitSeconds);
        SheriffNumShots = CustomOption.Normal(COID.SheriffNumShots, COType.Crewmate, TrKey.SheriffNumShots, 2f, 1f, 15f, 1f, SheriffSpawnRate, format: TrKey.UnitShots);
        SheriffMisfireKillsTarget = CustomOption.Normal(COID.SheriffMisfireKillsTarget, COType.Crewmate, TrKey.SheriffMisfireKillsTarget, false, SheriffSpawnRate);
        SheriffCanKillNoDeadBody = CustomOption.Normal(COID.SheriffCanKillNoDeadBody, COType.Crewmate, TrKey.SheriffCanKillNoDeadBody, true, SheriffSpawnRate);
        SheriffCanKillNeutrals = CustomOption.Normal(COID.SheriffCanKillNeutrals, COType.Crewmate, TrKey.SheriffCanKillNeutrals, false, SheriffSpawnRate);

        BakerySpawnRate = new(COID.BakerySpawnRate, COType.Crewmate, RoleType.Bakery, Bakery.Color, 1);
        BakeryBombRate = CustomOption.Normal(COID.BakeryBombRate, COType.Crewmate, TrKey.BakeryBombRate, 0f, 0f, 100f, 5f, BakerySpawnRate, format: TrKey.UnitPercent);
        BakeryBombType = CustomOption.Normal(COID.BakeryBombType, COType.Crewmate, TrKey.BakeryBombType, [TrKey.BakeryBombSuicide, TrKey.BakeryBombShip], 0, BakeryBombRate);

        #endregion

        #region ROLES IMPOSTOR

        BountyHunterSpawnRate = new(COID.BountyHunterSpawnRate, COType.Impostor, RoleType.BountyHunter, BountyHunter.Color, 1);
        BountyHunterBountyDuration = CustomOption.Normal(COID.BountyHunterBountyDuration, COType.Impostor, TrKey.BountyHunterBountyDuration, 60f, 10f, 180f, 10f, BountyHunterSpawnRate, format: TrKey.UnitSeconds);
        BountyHunterReducedCooldown = CustomOption.Normal(COID.BountyHunterReducedCooldown, COType.Impostor, TrKey.BountyHunterReducedCooldown, 2.5f, 2.5f, 30f, 2.5f, BountyHunterSpawnRate, format: TrKey.UnitSeconds);
        BountyHunterPunishmentTime = CustomOption.Normal(COID.BountyHunterPunishmentTime, COType.Impostor, TrKey.BountyHunterPunishmentTime, 20f, 0f, 60f, 2.5f, BountyHunterSpawnRate, format: TrKey.UnitSeconds);
        BountyHunterShowArrow = CustomOption.Normal(COID.BountyHunterShowArrow, COType.Impostor, TrKey.BountyHunterShowArrow, true, BountyHunterSpawnRate);
        BountyHunterArrowUpdateInterval = CustomOption.Normal(COID.BountyHunterArrowUpdateInterval, COType.Impostor, TrKey.BountyHunterArrowUpdateInterval, 15f, 2.5f, 60f, 2.5f, BountyHunterShowArrow, format: TrKey.UnitSeconds);

        MafiaSpawnRate = new(COID.MafiaSpawnRate, COType.Impostor, TrKey.Mafia, Mafia.Color, 1);
        MafiosoCanSabotage = CustomOption.Normal(COID.MafiosoCanSabotage, COType.Impostor, TrKey.MafiosoCanSabotage, false, MafiaSpawnRate);
        MafiosoCanRepair = CustomOption.Normal(COID.MafiosoCanRepair, COType.Impostor, TrKey.MafiosoCanRepair, false, MafiaSpawnRate);
        MafiosoCanVent = CustomOption.Normal(COID.MafiosoCanVent, COType.Impostor, TrKey.MafiosoCanVent, false, MafiaSpawnRate);
        JanitorCooldown = CustomOption.Normal(COID.JanitorCooldown, COType.Impostor, TrKey.JanitorCooldown, 30f, 2.5f, 60f, 2.5f, MafiaSpawnRate, format: TrKey.UnitSeconds);
        JanitorCanSabotage = CustomOption.Normal(COID.JanitorCanSabotage, COType.Impostor, TrKey.JanitorCanSabotage, false, MafiaSpawnRate);
        JanitorCanRepair = CustomOption.Normal(COID.JanitorCanRepair, COType.Impostor, TrKey.JanitorCanRepair, false, MafiaSpawnRate);
        JanitorCanVent = CustomOption.Normal(COID.JanitorCanVent, COType.Impostor, TrKey.JanitorCanVent, false, MafiaSpawnRate);

        TricksterSpawnRate = new(COID.TricksterSpawnRate, COType.Impostor, RoleType.Trickster, Trickster.Color, 1);
        TricksterPlaceBoxCooldown = CustomOption.Normal(COID.TricksterPlaceBoxCooldown, COType.Impostor, TrKey.TricksterPlaceBoxCooldown, 10f, 2.5f, 30f, 2.5f, TricksterSpawnRate, format: TrKey.UnitSeconds);
        TricksterLightsOutCooldown = CustomOption.Normal(COID.TricksterLightsOutCooldown, COType.Impostor, TrKey.TricksterLightsOutCooldown, 30f, 5f, 60f, 5f, TricksterSpawnRate, format: TrKey.UnitSeconds);
        TricksterLightsOutDuration = CustomOption.Normal(COID.TricksterLightsOutDuration, COType.Impostor, TrKey.TricksterLightsOutDuration, 15f, 5f, 60f, 2.5f, TricksterSpawnRate, format: TrKey.UnitSeconds);

        EvilHackerSpawnRate = new(COID.EvilHackerSpawnRate, COType.Impostor, RoleType.EvilHacker, EvilHacker.Color, 1);
        EvilHackerCanHasBetterAdmin = CustomOption.Normal(COID.EvilHackerCanHasBetterAdmin, COType.Impostor, TrKey.EvilHackerCanHasBetterAdmin, false, EvilHackerSpawnRate);
        EvilHackerCanMoveEvenIfUsesAdmin = CustomOption.Normal(COID.EvilHackerCanMoveEvenIfUsesAdmin, COType.Impostor, TrKey.EvilHackerCanMoveEvenIfUsesAdmin, true, EvilHackerSpawnRate);
        EvilHackerCanInheritAbility = CustomOption.Normal(COID.EvilHackerCanInheritAbility, COType.Impostor, TrKey.EvilHackerCanInheritAbility, false, EvilHackerSpawnRate);
        EvilHackerCanSeeDoorStatus = CustomOption.Normal(COID.EvilHackerCanSeeDoorStatus, COType.Impostor, TrKey.EvilHackerCanSeeDoorStatus, true, EvilHackerSpawnRate);
        EvilHackerCanCreateMadmate = CustomOption.Normal(COID.EvilHackerCanCreateMadmate, COType.Impostor, TrKey.EvilHackerCanCreateMadmate, false, EvilHackerSpawnRate);
        CreatedMadmateCanDieToSheriff = CustomOption.Normal(COID.CreatedMadmateCanDieToSheriff, COType.Impostor, TrKey.CreatedMadmateCanDieToSheriff, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanEnterVents = CustomOption.Normal(COID.CreatedMadmateCanEnterVents, COType.Impostor, TrKey.CreatedMadmateCanEnterVents, false, EvilHackerCanCreateMadmate);
        EvilHackerCanCreateMadmateFromJackal = CustomOption.Normal(COID.EvilHackerCanCreateMadmateFromJackal, COType.Impostor, TrKey.EvilHackerCanCreateMadmateFromJackal, false, EvilHackerCanCreateMadmate);
        CreatedMadmateHasImpostorVision = CustomOption.Normal(COID.CreatedMadmateHasImpostorVision, COType.Impostor, TrKey.CreatedMadmateHasImpostorVision, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanSabotage = CustomOption.Normal(COID.CreatedMadmateCanSabotage, COType.Impostor, TrKey.CreatedMadmateCanSabotage, false, EvilHackerCanCreateMadmate);
        CreatedMadmateCanFixComm = CustomOption.Normal(COID.CreatedMadmateCanFixComm, COType.Impostor, TrKey.CreatedMadmateCanFixComm, true, EvilHackerCanCreateMadmate);
        CreatedMadmateAbility = CustomOption.Normal(COID.CreatedMadmateAbility, COType.Impostor, TrKey.MadmateAbility, [TrKey.MadmateNone, TrKey.MadmateFanatic, TrKey.Suicider], 0, EvilHackerCanCreateMadmate);
        CreatedMadmateNumTasks = CustomOption.Normal(COID.CreatedMadmateNumTasks, COType.Impostor, TrKey.CreatedMadmateNumTasks, 4f, 1f, 20f, 1f, CreatedMadmateAbility);
        CreatedMadmateExileCrewmate = CustomOption.Normal(COID.CreatedMadmateExileCrewmate, COType.Impostor, TrKey.CreatedMadmateExileCrewmate, false, EvilHackerCanCreateMadmate);

        EvilTrackerSpawnRate = new(COID.EvilTrackerSpawnRate, COType.Impostor, RoleType.EvilTracker, EvilTracker.Color, 3);
        EvilTrackerCooldown = CustomOption.Normal(COID.EvilTrackerCooldown, COType.Impostor, TrKey.EvilTrackerCooldown, 10f, 0f, 60f, 1f, EvilTrackerSpawnRate, format: TrKey.UnitSeconds);
        EvilTrackerResetTargetAfterMeeting = CustomOption.Normal(COID.EvilTrackerResetTargetAfterMeeting, COType.Impostor, TrKey.EvilTrackerResetTargetAfterMeeting, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeDeathFlash = CustomOption.Normal(COID.EvilTrackerCanSeeDeathFlash, COType.Impostor, TrKey.EvilTrackerCanSeeDeathFlash, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetTask = CustomOption.Normal(COID.EvilTrackerCanSeeTargetTask, COType.Impostor, TrKey.EvilTrackerCanSeeTargetTask, true, EvilTrackerSpawnRate);
        EvilTrackerCanSeeTargetPosition = CustomOption.Normal(COID.EvilTrackerCanSeeTargetPosition, COType.Impostor, TrKey.EvilTrackerCanSeeTargetPosition, true, EvilTrackerSpawnRate);
        EvilTrackerCanSetTargetOnMeeting = CustomOption.Normal(COID.EvilTrackerCanSetTargetOnMeeting, COType.Impostor, TrKey.EvilTrackerCanSetTargetOnMeeting, true, EvilTrackerSpawnRate);

        EraserSpawnRate = new(COID.EraserSpawnRate, COType.Impostor, RoleType.Eraser, Eraser.Color, 1);
        EraserCooldown = CustomOption.Normal(COID.EraserCooldown, COType.Impostor, TrKey.EraserCooldown, 30f, 5f, 120f, 5f, EraserSpawnRate, format: TrKey.UnitSeconds);
        EraserCooldownIncrease = CustomOption.Normal(COID.EraserCooldownIncrease, COType.Impostor, TrKey.EraserCooldownIncrease, 10f, 0f, 120f, 2.5f, EraserSpawnRate, format: TrKey.UnitSeconds);
        EraserCanEraseAnyone = CustomOption.Normal(COID.EraserCanEraseAnyone, COType.Impostor, TrKey.EraserCanEraseAnyone, false, EraserSpawnRate);

        MorphingSpawnRate = new(COID.MorphingSpawnRate, COType.Impostor, RoleType.Morphing, Morphing.Color, 1);
        MorphingCooldown = CustomOption.Normal(COID.MorphingCooldown, COType.Impostor, TrKey.MorphingCooldown, 30f, 2.5f, 60f, 2.5f, MorphingSpawnRate, format: TrKey.UnitSeconds);
        MorphingDuration = CustomOption.Normal(COID.MorphingDuration, COType.Impostor, TrKey.MorphingDuration, 10f, 1f, 20f, 0.5f, MorphingSpawnRate, format: TrKey.UnitSeconds);

        CamouflagerSpawnRate = new(COID.CamouflagerSpawnRate, COType.Impostor, RoleType.Camouflager, Camouflager.Color, 1);
        CamouflagerCooldown = CustomOption.Normal(COID.CamouflagerCooldown, COType.Impostor, TrKey.CamouflagerCooldown, 30f, 2.5f, 60f, 2.5f, CamouflagerSpawnRate, format: TrKey.UnitSeconds);
        CamouflagerDuration = CustomOption.Normal(COID.CamouflagerDuration, COType.Impostor, TrKey.CamouflagerDuration, 10f, 1f, 20f, 0.5f, CamouflagerSpawnRate, format: TrKey.UnitSeconds);
        CamouflagerRandomColors = CustomOption.Normal(COID.CamouflagerRandomColors, COType.Impostor, TrKey.CamouflagerRandomColors, false, CamouflagerSpawnRate);

        CleanerSpawnRate = new(COID.CleanerSpawnRate, COType.Impostor, RoleType.Cleaner, Cleaner.Color, 1);
        CleanerCooldown = CustomOption.Normal(COID.CleanerCooldown, COType.Impostor, TrKey.CleanerCooldown, 30f, 2.5f, 60f, 2.5f, CleanerSpawnRate, format: TrKey.UnitSeconds);

        WarlockSpawnRate = new(COID.WarlockSpawnRate, COType.Impostor, RoleType.Warlock, Warlock.Color, 1);
        WarlockCooldown = CustomOption.Normal(COID.WarlockCooldown, COType.Impostor, TrKey.WarlockCooldown, 30f, 2.5f, 60f, 2.5f, WarlockSpawnRate, format: TrKey.UnitSeconds);
        WarlockRootTime = CustomOption.Normal(COID.WarlockRootTime, COType.Impostor, TrKey.WarlockRootTime, 5f, 0f, 15f, 1f, WarlockSpawnRate, format: TrKey.UnitSeconds);

        WitchSpawnRate = new(COID.WitchSpawnRate, COType.Impostor, RoleType.Witch, Witch.Color, 1);
        WitchCooldown = CustomOption.Normal(COID.WitchCooldown, COType.Impostor, TrKey.WitchSpellCooldown, 30f, 2.5f, 120f, 2.5f, WitchSpawnRate, format: TrKey.UnitSeconds);
        WitchAdditionalCooldown = CustomOption.Normal(COID.WitchAdditionalCooldown, COType.Impostor, TrKey.WitchAdditionalCooldown, 10f, 0f, 60f, 5f, WitchSpawnRate, format: TrKey.UnitSeconds);
        WitchCanSpellAnyone = CustomOption.Normal(COID.WitchCanSpellAnyone, COType.Impostor, TrKey.WitchCanSpellAnyone, false, WitchSpawnRate);
        WitchSpellCastingDuration = CustomOption.Normal(COID.WitchSpellCastingDuration, COType.Impostor, TrKey.WitchSpellDuration, 1f, 0f, 10f, 1f, WitchSpawnRate, format: TrKey.UnitSeconds);
        WitchTriggerBothCooldowns = CustomOption.Normal(COID.WitchTriggerBothCooldowns, COType.Impostor, TrKey.WitchTriggerBoth, true, WitchSpawnRate);
        WitchVoteSavesTargets = CustomOption.Normal(COID.WitchVoteSavesTargets, COType.Impostor, TrKey.WitchSaveTargets, true, WitchSpawnRate);

        VampireSpawnRate = new(COID.VampireSpawnRate, COType.Impostor, RoleType.Vampire, Vampire.Color, 1);
        VampireKillDelay = CustomOption.Normal(COID.VampireKillDelay, COType.Impostor, TrKey.VampireKillDelay, 10f, 1f, 20f, 1f, VampireSpawnRate, format: TrKey.UnitSeconds);
        VampireCooldown = CustomOption.Normal(COID.VampireCooldown, COType.Impostor, TrKey.VampireCooldown, 30f, 2.5f, 60f, 2.5f, VampireSpawnRate, format: TrKey.UnitSeconds);
        VampireCanKillNearGarlics = CustomOption.Normal(COID.VampireCanKillNearGarlics, COType.Impostor, TrKey.VampireCanKillNearGarlics, true, VampireSpawnRate);

        #endregion

        #region ROLES NEUTRAL

        JesterSpawnRate = new(COID.JesterSpawnRate, COType.Neutral, RoleType.Jester, Jester.Color, 1);
        JesterCanCallEmergency = CustomOption.Normal(COID.JesterCanCallEmergency, COType.Neutral, TrKey.JesterCanCallEmergency, true, JesterSpawnRate);
        JesterCanSabotage = CustomOption.Normal(COID.JesterCanSabotage, COType.Neutral, TrKey.JesterCanSabotage, true, JesterSpawnRate);
        JesterHasImpostorVision = CustomOption.Normal(COID.JesterHasImpostorVision, COType.Neutral, TrKey.JesterHasImpostorVision, false, JesterSpawnRate);
        JesterCanFixSabotage = CustomOption.Normal(COID.JesterCanFixSabotage, COType.Neutral, TrKey.JesterCanFixSabotage, false, JesterSpawnRate);

        ArsonistSpawnRate = new(COID.ArsonistSpawnRate, COType.Neutral, RoleType.Arsonist, Arsonist.Color, 1);
        ArsonistCooldown = CustomOption.Normal(COID.ArsonistCooldown, COType.Neutral, TrKey.ArsonistCooldown, 12.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate, format: TrKey.UnitSeconds);
        ArsonistDuration = CustomOption.Normal(COID.ArsonistDuration, COType.Neutral, TrKey.ArsonistDuration, 3f, 0f, 10f, 1f, ArsonistSpawnRate, format: TrKey.UnitSeconds);
        ArsonistCanBeLovers = CustomOption.Normal(COID.ArsonistCanBeLovers, COType.Neutral, TrKey.ArsonistCanBeLovers, false, ArsonistSpawnRate);
        ArsonistCanFixSabotage = CustomOption.Normal(COID.ArsonistCanFixSabotage, COType.Neutral, TrKey.ArsonistCanFixSabotage, false, ArsonistSpawnRate);

        VultureSpawnRate = new(COID.VultureSpawnRate, COType.Neutral, RoleType.Vulture, Vulture.Color, 1);
        VultureCooldown = CustomOption.Normal(COID.VultureCooldown, COType.Neutral, TrKey.VultureCooldown, 15f, 2.5f, 60f, 2.5f, VultureSpawnRate, format: TrKey.UnitSeconds);
        VultureNumberToWin = CustomOption.Normal(COID.VultureNumberToWin, COType.Neutral, TrKey.VultureNumberToWin, 4f, 1f, 12f, 1f, VultureSpawnRate, format: TrKey.UnitTimes);
        VultureCanUseVents = CustomOption.Normal(COID.VultureCanUseVents, COType.Neutral, TrKey.VultureCanUseVents, true, VultureSpawnRate);
        VultureShowArrows = CustomOption.Normal(COID.VultureShowArrows, COType.Neutral, TrKey.VultureShowArrows, true, VultureSpawnRate);

        JackalSpawnRate = new(COID.JackalSpawnRate, COType.Neutral, RoleType.Jackal, Jackal.Color, 1);
        JackalKillCooldown = CustomOption.Normal(COID.JackalKillCooldown, COType.Neutral, TrKey.JackalKillCooldown, 30f, 10f, 60f, 2.5f, JackalSpawnRate, format: TrKey.UnitSeconds);
        JackalCanSabotageLights = CustomOption.Normal(COID.JackalCanSabotageLights, COType.Neutral, TrKey.JackalCanSabotageLights, true, JackalSpawnRate);
        JackalCanUseVents = CustomOption.Normal(COID.JackalCanUseVents, COType.Neutral, TrKey.JackalCanUseVents, true, JackalSpawnRate);
        JackalHasImpostorVision = CustomOption.Normal(COID.JackalHasImpostorVision, COType.Neutral, TrKey.JackalHasImpostorVision, false, JackalSpawnRate);
        JackalCanCreateSidekick = CustomOption.Normal(COID.JackalCanCreateSidekick, COType.Neutral, TrKey.JackalCanCreateSidekick, false, JackalSpawnRate);
        JackalCreateSidekickCooldown = CustomOption.Normal(COID.JackalCreateSidekickCooldown, COType.Neutral, TrKey.JackalCreateSidekickCooldown, 30f, 10f, 60f, 2.5f, JackalCanCreateSidekick, format: TrKey.UnitSeconds);
        SidekickCanKill = CustomOption.Normal(COID.SidekickCanKill, COType.Neutral, TrKey.SidekickCanKill, false, JackalCanCreateSidekick);
        SidekickCanUseVents = CustomOption.Normal(COID.SidekickCanUseVents, COType.Neutral, TrKey.SidekickCanUseVents, true, JackalCanCreateSidekick);
        SidekickCanSabotageLights = CustomOption.Normal(COID.SidekickCanSabotageLights, COType.Neutral, TrKey.SidekickCanSabotageLights, true, JackalCanCreateSidekick);
        SidekickHasImpostorVision = CustomOption.Normal(COID.SidekickHasImpostorVision, COType.Neutral, TrKey.SidekickHasImpostorVision, false, JackalCanCreateSidekick);
        SidekickPromotesToJackal = CustomOption.Normal(COID.SidekickPromotesToJackal, COType.Neutral, TrKey.SidekickPromotesToJackal, false, JackalCanCreateSidekick);
        JackalPromotedFromSidekickCanCreateSidekick = CustomOption.Normal(COID.JackalPromotedFromSidekickCanCreateSidekick, COType.Neutral, TrKey.JackalPromotedFromSidekickCanCreateSidekick, false, SidekickPromotesToJackal);
        JackalCanCreateSidekickFromImpostor = CustomOption.Normal(COID.JackalCanCreateSidekickFromImpostor, COType.Neutral, TrKey.JackalCanCreateSidekickFromImpostor, false, JackalCanCreateSidekick);

        GuesserSpawnRate = new(COID.GuesserSpawnRate, COType.Neutral, TrKey.Guesser, Guesser.NiceGuesser.Color, 1);
        GuesserIsImpGuesserRate = CustomOption.Normal(COID.GuesserIsImpGuesserRate, COType.Neutral, TrKey.GuesserIsImpGuesserRate, Rates, 0, GuesserSpawnRate);
        GuesserSpawnBothRate = CustomOption.Normal(COID.GuesserSpawnBothRate, COType.Neutral, TrKey.GuesserSpawnBothRate, Rates, 0, GuesserSpawnRate);
        GuesserNumberOfShots = CustomOption.Normal(COID.GuesserNumberOfShots, COType.Neutral, TrKey.GuesserNumberOfShots, 2f, 1f, 15f, 1f, GuesserSpawnRate, format: TrKey.UnitShots);
        GuesserOnlyAvailableRoles = CustomOption.Normal(COID.GuesserOnlyAvailableRoles, COType.Neutral, TrKey.GuesserOnlyAvailableRoles, true, GuesserSpawnRate);
        GuesserHasMultipleShotsPerMeeting = CustomOption.Normal(COID.GuesserHasMultipleShotsPerMeeting, COType.Neutral, TrKey.GuesserHasMultipleShotsPerMeeting, false, GuesserSpawnRate);
        GuesserShowInfoInGhostChat = CustomOption.Normal(COID.GuesserShowInfoInGhostChat, COType.Neutral, TrKey.GuesserToGhostChat, true, GuesserSpawnRate);
        GuesserKillsThroughShield = CustomOption.Normal(COID.GuesserKillsThroughShield, COType.Neutral, TrKey.GuesserPierceShield, true, GuesserSpawnRate);
        GuesserEvilCanKillSpy = CustomOption.Normal(COID.GuesserEvilCanKillSpy, COType.Neutral, TrKey.GuesserEvilCanKillSpy, true, GuesserSpawnRate);

        #endregion

        #region MODIFIERS

        MadmateSpawnRate = new(COID.MadmateSpawnRate, COType.Modifier, ModifierType.Madmate, Madmate.Color);
        MadmateType = CustomOption.Normal(COID.MadmateType, COType.Modifier, TrKey.MadmateType, [TrKey.MadmateDefault, TrKey.MadmateWithRole, TrKey.MadmateRandom], 0, MadmateSpawnRate);
        MadmateFixedRole = new(COID.MadmateFixedRole, COType.Modifier, TrKey.MadmateFixedRole, Madmate.ValidRoles, MadmateType);
        MadmateAbility = CustomOption.Normal(COID.MadmateAbility, COType.Modifier, TrKey.MadmateAbility, [TrKey.MadmateNone, TrKey.MadmateFanatic, TrKey.Suicider], 0, MadmateSpawnRate);
        MadmateTasks = new((COID.MadmateTasksCommon, COID.MadmateTasksShort, COID.MadmateTasksLong), COType.Modifier, (1, 1, 3), MadmateAbility);
        MadmateCanDieToSheriff = CustomOption.Normal(COID.MadmateCanDieToSheriff, COType.Modifier, TrKey.MadmateCanDieToSheriff, false, MadmateSpawnRate);
        MadmateCanEnterVents = CustomOption.Normal(COID.MadmateCanEnterVents, COType.Modifier, TrKey.MadmateCanEnterVents, false, MadmateSpawnRate);
        MadmateHasImpostorVision = CustomOption.Normal(COID.MadmateHasImpostorVision, COType.Modifier, TrKey.MadmateHasImpostorVision, false, MadmateSpawnRate);
        MadmateCanSabotage = CustomOption.Normal(COID.MadmateCanSabotage, COType.Modifier, TrKey.MadmateCanSabotage, false, MadmateSpawnRate);
        MadmateCanFixComm = CustomOption.Normal(COID.MadmateCanFixComm, COType.Modifier, TrKey.MadmateCanFixComm, true, MadmateSpawnRate);
        MadmateExilePlayer = CustomOption.Normal(COID.MadmateExilePlayer, COType.Modifier, TrKey.MadmateExileCrewmate, false, MadmateSpawnRate);

        LoversSpawnRate = new(COID.LoversSpawnRate, COType.Modifier, RoleType.Lovers, Lovers.Color, 1);
        LoversImpLoverRate = CustomOption.Normal(COID.LoversImpLoverRate, COType.Modifier, TrKey.LoversImpLoverRate, Rates, 0, LoversSpawnRate);
        LoversNumCouples = CustomOption.Normal(COID.LoversNumCouples, COType.Modifier, TrKey.LoversNumCouples, 1f, 1f, 7f, 1f, LoversSpawnRate, format: TrKey.UnitCouples);
        LoversBothDie = CustomOption.Normal(COID.LoversBothDie, COType.Modifier, TrKey.LoversBothDie, true, LoversSpawnRate);
        LoversCanHaveAnotherRole = CustomOption.Normal(COID.LoversCanHaveAnotherRole, COType.Modifier, TrKey.LoversCanHaveAnotherRole, true, LoversSpawnRate);
        LoversSeparateTeam = CustomOption.Normal(COID.LoversSeparateTeam, COType.Modifier, TrKey.LoversSeparateTeam, true, LoversSpawnRate);
        LoversTasksCount = CustomOption.Normal(COID.LoversTasksCount, COType.Modifier, TrKey.LoversTasksCount, false, LoversSpawnRate);
        LoversEnableChat = CustomOption.Normal(COID.LoversEnableChat, COType.Modifier, TrKey.LoversEnableChat, true, LoversSpawnRate);

        MiniSpawnRate = new(COID.MiniSpawnRate, COType.Modifier, ModifierType.Mini, Mini.Color);
        MiniGrowingUpDuration = CustomOption.Normal(COID.MiniGrowingUpDuration, COType.Modifier, TrKey.MiniGrowingUpDuration, 400f, 100f, 1500f, 100f, MiniSpawnRate, format: TrKey.UnitSeconds);

        AntiTeleportSpawnRate = new(COID.AntiTeleportSpawnRate, COType.Modifier, ModifierType.AntiTeleport, AntiTeleport.Color);

        #endregion

        #region HIDE AND SEEK

        HideNSeekHideTime = CustomOption.Normal(COID.HideNSeekHideTime, COType.HideNSeek, TrKey.HideNSeekHideTime, 220f, 160f, 300f, 20f, header: hideNSeekHeader, format: TrKey.UnitSeconds);
        HideNSeekCrewmateVision = CustomOption.Normal(COID.HideNSeekCrewmateVision, COType.HideNSeek, TrKey.HideNSeekCrewmateVision, 0.5f, 0.25f, 1f, 0.05f, format: TrKey.UnitMultiplies);
        HideNSeekMaxVentUses = CustomOption.Normal(COID.HideNSeekMaxVentUses, COType.HideNSeek, TrKey.HideNSeekMaxVentUses, 3f, 0f, 5f, 1f, format: TrKey.UnitTimes);
        HideNSeekFlashlightMode = CustomOption.Normal(COID.HideNSeekFlashlightMode, COType.HideNSeek, TrKey.HideNSeekFlashlightMode, false);
        HideNSeekMaxTimeInVent = CustomOption.Normal(COID.HideNSeekMaxTimeInVent, COType.HideNSeek, TrKey.HideNSeekMaxTimeInVent, 5f, 1f, 10f, 1f, format: TrKey.UnitSeconds);
        HideNSeekCrewmateFlashlightSize = CustomOption.Normal(COID.HideNSeekCrewmateFlashlightSize, COType.HideNSeek, TrKey.HideNSeekCrewmateFlashlightSize, 0.4f, 0.1f, 0.5f, 0.05f, format: TrKey.UnitMultiplies);
        // HideNSeekImpostor = CustomOption.Player(7007, CustomOptionType.HideNSeek, TrKey.HideNSeekImpostor, true);
        HideNSeekImpostorFlashlightSize = CustomOption.Normal(COID.HideNSeekImpostorFlashlightSize, COType.HideNSeek, TrKey.HideNSeekImpostorFlashlightSize, 0.3f, 0.1f, 0.5f, 0.05f, format: TrKey.UnitMultiplies);
        HideNSeekImpostorVision = CustomOption.Normal(COID.HideNSeekImpostorVision, COType.HideNSeek, TrKey.HideNSeekImpostorVision, 0.3f, 0.25f, 1f, 0.05f, format: TrKey.UnitMultiplies);
        HideNSeekFinalHideTime = CustomOption.Normal(COID.HideNSeekFinalHideTime, COType.HideNSeek, TrKey.HideNSeekFinalHideTime, 40f, 30f, 120f, 5f, format: TrKey.UnitSeconds);
        HideNSeekFinalHidePings = CustomOption.Normal(COID.HideNSeekFinalHidePings, COType.HideNSeek, TrKey.HideNSeekFinalHidePings, true);
        HideNSeekFinalHideImpostorSpeed = CustomOption.Normal(COID.HideNSeekFinalHideImpostorSpeed, COType.HideNSeek, TrKey.HideNSeekFinalHideImpostorSpeed, 1.5f, 1.0f, 3.0f, 0.05f, format: TrKey.UnitMultiplies);
        HideNSeekPingInterval = CustomOption.Normal(COID.HideNSeekPingInterval, COType.HideNSeek, TrKey.HideNSeekPingInterval, 5f, 3f, 10f, 1f, format: TrKey.UnitSeconds);
        HideNSeekFinalHideSeekMap = CustomOption.Normal(COID.HideNSeekFinalHideSeekMap, COType.HideNSeek, TrKey.HideNSeekFinalHideSeekMap, true);
        HideNSeekTasks = new CustomTasksOption((COID.HideNSeekTasksCommon, COID.HideNSeekTasksShort, COID.HideNSeekTasksLong), COType.HideNSeek, (2, 1, 2));

        #endregion

        #region BATTLE ROYALE

        BattleRoyaleTimeLimit = CustomOption.Normal(COID.BattleRoyaleTimeLimit, COType.BattleRoyale, TrKey.BattleRoyaleTimeLimit, 300f, 60f, 1800f, 60f, header: battleRoyaleHeader, format: TrKey.UnitSeconds);
        BattleRoyaleKillCooldown = CustomOption.Normal(COID.BattleRoyaleKillCooldown, COType.BattleRoyale, TrKey.BattleRoyaleKillCooldown, 1f, 1f, 5f, 0.5f, format: TrKey.UnitSeconds);
        BattleRoyaleVisionRange = CustomOption.Normal(COID.BattleRoyaleVisionRange, COType.BattleRoyale, TrKey.BattleRoyaleVisionRange, 1.5f, 0.25f, 2.0f, 0.25f, format: TrKey.UnitMultiplies);
        // BattleRoyaleButtonCooldown = CustomOption.Normal(5005, CustomOptionType.BattleRoyale, TrKey.BattleRoyaleButtonCooldown, 10f, 5f, 30f, 1f, format: TrKey.UnitSeconds);
        // BattleRoyaleButtonUsage = CustomOption.Normal(5006, CustomOptionType.BattleRoyale, TrKey.BattleRoyaleButtonUsage, 1f, 1f, 3f, 1f, format: TrKey.UnitTimes);

        #endregion

        #region HOT POTATO

        HotPotatoTimeLimit = CustomOption.Normal(COID.HotPotatoTimeLimit, COType.HotPotato, TrKey.HotPotatoTimeLimit, 300f, 60f, 1800f, 60f, header: hotPotatoHeader, format: TrKey.UnitSeconds);
        HotPotatoExplodeTime = CustomOption.Normal(COID.HotPotatoExplodeTime, COType.HotPotato, TrKey.HotPotatoExplodeTime, 30f, 10f, 120f, 5f, format: TrKey.UnitSeconds);

        #endregion

        BlockedRolePairings.Add((byte)RoleType.Vulture, [(byte)RoleType.Cleaner]);
        BlockedRolePairings.Add((byte)RoleType.Cleaner, [(byte)RoleType.Vulture]);
    }

    #region MOD OPTIONS

    internal static CustomOption PresetSelection;
    internal static CustomOption ActivateRoles;
    internal static CustomOption GameModeSelection;
    internal static CustomOption RandomNumberAlgorithm;
    internal static CustomOption EnableRandomRandomNumberAlgorithm;
    internal static CustomOption EnableRandomRandomNumberAlgorithmDotnet;
    internal static CustomOption EnableRandomRandomNumberAlgorithmMT;
    internal static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Pp;
    internal static CustomOption EnableRandomRandomNumberAlgorithmXorshiro256Ss;
    internal static CustomOption EnableRandomRandomNumberAlgorithmPcg64;
    internal static CustomOption SendEmbedToDiscord;

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
    internal static CustomOption DontFinishGame;
    internal static CustomOption RandomSpawn;

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

    #region HIDE AND SEEK

    internal static CustomOption HideNSeekHideTime;
    internal static CustomOption HideNSeekCrewmateVision;
    internal static CustomOption HideNSeekMaxVentUses;
    internal static CustomOption HideNSeekFlashlightMode;
    internal static CustomOption HideNSeekMaxTimeInVent;
    internal static CustomOption HideNSeekCrewmateFlashlightSize;
    internal static CustomOption HideNSeekImpostorFlashlightSize;
    internal static CustomOption HideNSeekImpostorVision;
    internal static CustomOption HideNSeekFinalHideTime;
    internal static CustomOption HideNSeekFinalHidePings;
    internal static CustomOption HideNSeekFinalHideImpostorSpeed;
    internal static CustomOption HideNSeekPingInterval;
    internal static CustomOption HideNSeekFinalHideSeekMap;
    internal static CustomTasksOption HideNSeekTasks;

    #endregion

    #region BATTLE ROYALE

    internal static CustomOption BattleRoyaleTimeLimit;
    internal static CustomOption BattleRoyaleKillCooldown;
    internal static CustomOption BattleRoyaleVisionRange;
    // internal static CustomOption BattleRoyaleButtonCooldown;
    // internal static CustomOption BattleRoyaleButtonUsage;

    #endregion

    #region HOT POTATO

    internal static CustomOption HotPotatoTimeLimit;
    internal static CustomOption HotPotatoExplodeTime;

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

    internal static CustomRoleOption BakerySpawnRate;
    internal static CustomOption BakeryBombRate;
    internal static CustomOption BakeryBombType;

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
    internal static CustomOption JesterCanFixSabotage;

    internal static CustomRoleOption ArsonistSpawnRate;
    internal static CustomOption ArsonistCooldown;
    internal static CustomOption ArsonistDuration;
    internal static CustomOption ArsonistCanBeLovers;
    internal static CustomOption ArsonistCanFixSabotage;

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
