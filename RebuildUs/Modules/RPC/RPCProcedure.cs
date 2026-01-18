using RebuildUs.Modules;
using AmongUs.GameOptions;
using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;
using RebuildUs.Objects;
using Epic.OnlineServices;
using UnityEngine.UIElements;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Modifier;

namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
    public static void ResetVariables()
    {
        RoleAssignment.IsAssigned = false;
        Garlic.ClearGarlics();
        JackInTheBox.ClearJackInTheBoxes();
        ModMapOptions.ClearAndReloadMapOptions();
        RebuildUs.ClearAndReloadRoles();
        GameHistory.ClearGameHistory();
        RebuildUs.SetButtonCooldowns();
        Admin.ResetData();
        SecurityCamera.ResetData();
        Vitals.ResetData();
        Map.reset();
        CustomOverlays.resetOverlays();
        SpecimenVital.clearAndReload();
        AdditionalVents.clearAndReload();
        SpawnIn.reset();
        Map.resetRealTasks();

        KillAnimationPatch.HideNextAnimation = false;

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishResetVariables);
        sender.Write(PlayerControl.LocalPlayer.PlayerId);
        FinishResetVariables(PlayerControl.LocalPlayer.PlayerId);
    }

    private static void ShareOptions(MessageReader reader)
    {
        byte amount = reader.ReadByte();
        for (int i = 0; i < amount; i++)
        {
            uint id = reader.ReadPackedUInt32();
            uint selection = reader.ReadPackedUInt32();
            var option = CustomOption.AllOptions.FirstOrDefault(x => x.Id == (int)id);
            option?.UpdateSelection((int)selection);
        }
    }

    public static void WorkaroundSetRoles(byte numberOfRoles, MessageReader reader)
    {
        for (int i = 0; i < numberOfRoles; i++)
        {
            byte playerId = (byte)reader.ReadPackedUInt32();
            byte roleId = (byte)reader.ReadPackedUInt32();
            try
            {
                SetRole(roleId, playerId);
            }
            catch (Exception e)
            {
                Logger.LogError("Error while deserializing roles: " + e.Message);
            }
        }
    }

    public static void SetRole(byte roleId, byte playerId)
    {
        Logger.LogInfo($"{GameData.Instance.GetPlayerById(playerId).PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), roleId)}", "setRole");
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
            x => x.PlayerId == playerId,
            x => x.SetRole((RoleType)roleId)
        );
    }

    public static void AddModifier(byte modId, byte playerId)
    {
        // PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
        //     x => x.PlayerId == playerId,
        //     x => x.addModifier((ModifierType)modId)
        // );
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, int clientId)
    {
        var ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStart.PlayerVersions[clientId] = ver;
    }

    public static void FinishSetRole()
    {
        RoleAssignment.IsAssigned = true;
    }

    public static void UpdateMeeting(byte targetId, bool dead = true)
    {
        if (MeetingHud.Instance)
        {
            foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
            {
                if (pva.TargetPlayerId == targetId)
                {
                    pva.SetDead(pva.DidReport, dead);
                    pva.Overlay.gameObject.SetActive(dead);
                }

                // Give players back their vote if target is shot dead
                if (Helpers.RefundVotes && dead)
                {
                    if (pva.VotedFor != targetId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.PlayerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingHud.Instance.CheckForEndVoting();
            }
        }
    }

    public static void UncheckedCmdReportDeadBody(byte sourceId, byte targetId)
    {
        var source = Helpers.PlayerById(sourceId);
        var target = targetId == byte.MaxValue ? null : Helpers.PlayerById(targetId).Data;
        source?.ReportDeadBody(target);
    }

    public static void UseUncheckedVent(int ventId, byte playerId, byte isEnter)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null) return;
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the coroutines cannot be accessed
        var reader = new MessageReader();
        byte[] bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.StartAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    public static void UncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
        var source = Helpers.PlayerById(sourceId);
        var target = Helpers.PlayerById(targetId);
        if (source != null && target != null)
        {
            if (showAnimation == 0) KillAnimationPatch.HideNextAnimation = true;
            source.MurderPlayer(target);
        }
    }

    public static void UncheckedExilePlayer(byte targetId)
    {
        var target = Helpers.PlayerById(targetId);
        target?.Exiled();
    }

    public static void DynamicMapOption(byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.SetByte(ByteOptionNames.MapId, mapId);
    }

    public static void ShareGamemode(byte gameMode)
    {
        ModMapOptions.GameMode = (CustomGamemodes)gameMode;
        GameStart.SendGamemode = false;
    }

    public static void SetGameStarting()
    {
        GameStart.StartingTimer = 5f;
    }

    public static void StopStart()
    {
        SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
        if (AmongUsClient.Instance.AmHost)
        {
            GameStartManager.Instance.ResetStartState();
        }
    }

    public static void FinishResetVariables(byte playerId)
    {
        var checkList = RoleAssignment.CheckList;
        if (checkList != null)
        {
            if (checkList.ContainsKey(playerId))
            {
                checkList[playerId] = true;
            }
        }
    }

    public static void SetLovers(byte playerId1, byte playerId2)
    {
        // Lovers.addCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
    }

    public static void OverrideNativeRole(byte playerId, byte roleType)
    {
        var player = Helpers.PlayerById(playerId);
        player.roleAssigned = false;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
    }

    public static void UncheckedEndGame(byte reason)
    {
        AmongUsClient.Instance.GameState = InnerNet.InnerNetClient.GameStates.Ended;
        var obj2 = AmongUsClient.Instance.allClients;
        lock (obj2)
        {
            AmongUsClient.Instance.allClients.Clear();
        }

        var obj = AmongUsClient.Instance.Dispatcher;
        lock (obj)
        {
            AmongUsClient.Instance.Dispatcher.Add(new Action(() =>
            {
                GameManager.Instance.enabled = false;
                GameManager.Instance.ShouldCheckForGameEnd = false;
                AmongUsClient.Instance.OnGameEnd(new EndGameResult((GameOverReason)reason, false));

                if (AmongUsClient.Instance.AmHost)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)reason, false);
                }
            }));
        }
    }

    public static void UncheckedSetTasks(byte playerId, byte[] taskTypeIds)
    {
        var player = Helpers.PlayerById(playerId);
        player.ClearAllTasks();

        player.Data.SetTasks(taskTypeIds);
    }

    public static void FinishShipStatusBegin()
    {
        PlayerControl.LocalPlayer.OnFinishShipStatusBegin();
    }

    public static void EngineerFixLights()
    {
        SwitchSystem switchSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    public static void EngineerFixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void EngineerUsedRepair(byte engineerId)
    {
        var engineer = Helpers.PlayerById(engineerId);
        Engineer.GetRole(engineer).RemainingFixes--;
    }

    public static void ArsonistDouse(byte playerId, byte arsonistId)
    {
        var arsonist = Helpers.PlayerById(arsonistId);
        Arsonist.GetRole(arsonist).DousedPlayers.Add(Helpers.PlayerById(playerId));
    }

    public static void ArsonistWin(byte arsonistId)
    {
        var arsonist = Helpers.PlayerById(arsonistId);
        Arsonist.TriggerArsonistWin = true;
        var livingPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(p => !p.IsRole(RoleType.Arsonist) && p.IsAlive());
        foreach (var p in livingPlayers)
        {
            p.Exiled();
            GameHistory.FinalStatuses[p.PlayerId] = EFinalStatus.Torched;
        }
    }

    public static void CleanBody(byte playerId)
    {
        DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
        for (int i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId)
            {
                UnityEngine.Object.Destroy(array[i].gameObject);
            }
        }
    }

    public static void VultureEat(byte playerId, byte vultureId)
    {
        CleanBody(playerId);
        var vulture = Helpers.PlayerById(vultureId);
        Vulture.GetRole(vulture).EatenBodies++;
    }

    public static void VultureWin()
    {
        Vulture.TriggerVultureWin = true;
    }

    public static void ErasePlayerRoles(byte playerId, bool ignoreLovers = false, bool clearNeutralTasks = true)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null) return;

        // Don't give a former neutral role tasks because that destroys the balance.
        if (player.IsNeutral() && clearNeutralTasks)
        {
            player.ClearAllTasks();
        }

        player.EraseAllRoles();
        // player.EraseAllModifiers();

        // if (!ignoreLovers && player.IsLovers())
        // {
        //     // The whole Lover couple is being erased
        //     Lovers.eraseCouple(player);
        // }
    }

    public static void JackalCreatesSidekick(byte targetId, byte jackalId)
    {
        var target = Helpers.PlayerById(targetId);
        var jackalPlayer = Helpers.PlayerById(jackalId);
        var jackal = Jackal.GetRole(jackalPlayer);
        if (target == null) return;

        if (!Jackal.CanCreateSidekickFromImpostor && target.Data.Role.IsImpostor)
        {
            jackal.FakeSidekick = target;
        }
        else
        {
            var wasSpy = target.IsRole(RoleType.Spy);
            var wasImpostor = target.IsTeamImpostor(); // This can only be reached if impostors can be sidekicked.
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
            ErasePlayerRoles(target.PlayerId, true);
            target.SetRole(RoleType.Sidekick);
            if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
            if (wasSpy || wasImpostor) Sidekick.GetRole(target).WasTeamRed = true;
            Sidekick.GetRole(target).WasSpy = wasSpy;
            Sidekick.GetRole(target).WasImpostor = wasImpostor;
        }
        jackal.CanSidekick = false;
        jackal.MySidekick = target;
    }

    public static void SidekickPromotes(byte sidekickId)
    {
        var sidekickPlayer = Helpers.PlayerById(sidekickId);
        var sidekick = Sidekick.GetRole(sidekickPlayer);
        var wasTeamRed = sidekick.WasTeamRed;
        var wasImpostor = sidekick.WasImpostor;
        var wasSpy = sidekick.WasSpy;
        Jackal.RemoveCurrentJackal();
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(sidekickPlayer, RoleTypes.Crewmate);
        ErasePlayerRoles(sidekickPlayer.PlayerId, true);
        sidekickPlayer.SetRole(RoleType.Jackal);
        var newJackal = Jackal.GetRole(sidekickPlayer);
        newJackal.CanSidekick = Jackal.JackalPromotedFromSidekickCanCreateSidekick;
        newJackal.WasTeamRed = wasTeamRed;
        newJackal.WasImpostor = wasImpostor;
        newJackal.WasSpy = wasSpy;
        sidekick.Clear();
        return;
    }

    public static void medicSetShielded(byte shieldedId)
    {
        Medic.usedShield = true;
        Medic.shielded = Helpers.PlayerById(shieldedId);
        Medic.futureShielded = null;
    }

    public static void shieldedMurderAttempt()
    {
        if (!Medic.Exists || Medic.shielded == null) return;

        bool isShieldedAndShow = Medic.shielded == CachedPlayer.LocalPlayer.PlayerControl && Medic.showAttemptToShielded;
        bool isMedicAndShow = CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic) && Medic.showAttemptToMedic;

        if ((isShieldedAndShow || isMedicAndShow) && FastDestroyableSingleton<HudManager>.Instance?.FullScreen != null)
        {
            var c = Palette.ImpostorRed;
            Helpers.showFlash(new Color(c.r, c.g, c.b));
        }
    }

    public static void setFutureShielded(byte playerId)
    {
        Medic.futureShielded = Helpers.PlayerById(playerId);
        Medic.usedShield = true;
    }

    public static void timeMasterRewindTime()
    {
        TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.TimeMaster))
        {
            TimeMaster.resetTimeMasterButton();
        }
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) =>
        {
            if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
        })));

        if (!TimeMaster.Exists || CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.TimeMaster)) return; // Time Master himself does not rewind
        if (CachedPlayer.LocalPlayer.PlayerControl.IsGM()) return; // GM does not rewind

        TimeMaster.isRewinding = true;

        if (MapBehaviour.Instance)
        {
            MapBehaviour.Instance.Close();
        }
        if (Minigame.Instance)
        {
            Minigame.Instance.ForceClose();
        }
        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
    }

    public static void timeMasterShield()
    {
        TimeMaster.shieldActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) =>
        {
            if (p == 1f) TimeMaster.shieldActive = false;
        })));
    }

    public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleType)
    {
        var killer = Helpers.PlayerById(killerId);
        var dyingTarget = Helpers.PlayerById(dyingTargetId);
        if (dyingTarget == null) return;
        dyingTarget.Exiled();
        var dyingLoverPartner = Lovers.bothDie ? dyingTarget.GetPartner() : null; // Lover check

        Guesser.remainingShots(killer, true);

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);

        PlayerControl guesser = Helpers.PlayerById(killerId);
        if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == dyingTarget)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
            }
            else if (dyingLoverPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingLoverPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);
            }
        }

        var guessedTarget = Helpers.PlayerById(guessedTargetId);
        if (Guesser.showInfoInGhostChat && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && guessedTarget != null)
        {
            var roleInfo = RoleInfo.AllRoleInfos.FirstOrDefault(x => (byte)x.RoleType == guessedRoleType);
            string msg = string.Format(Tr.Get("guesserGuessChat"), roleInfo.Name, guessedTarget.Data.PlayerName);
            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(guesser, msg);
            }
            if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
            {
                FastDestroyableSingleton<Assets.CoreScripts.UnityTelemetry>.Instance.SendWho();
            }
        }
    }

    public static void placeJackInTheBox(byte[] buff)
    {
        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new JackInTheBox(position);
    }

    public static void lightsOut()
    {
        Trickster.lightsOutTimer = Trickster.lightsOutDuration;
        // If the local player is impostor indicate lights out
        if (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            new CustomMessage(Tr.Get("tricksterLightsOutText"), Trickster.lightsOutDuration);
        }
    }

    public static void evilHackerCreatesMadmate(byte targetId, byte evilHackerId)
    {
        var targetPlayer = Helpers.PlayerById(targetId);
        var evilHackerPlayer = Helpers.PlayerById(evilHackerId);
        var evilHacker = EvilHacker.GetRole(evilHackerPlayer);
        if (!EvilHacker.canCreateMadmateFromJackal && targetPlayer.IsRole(RoleType.Jackal))
        {
            evilHacker.fakeMadmate = targetPlayer;
        }
        else
        {
            // Jackalバグ対応
            List<PlayerControl> tmpFormerJackals = [.. Jackal.FormerJackals];

            // タスクがないプレイヤーがMadmateになった場合はショートタスクを必要数割り当てる
            if (Helpers.HasFakeTasks(targetPlayer))
            {
                if (CreatedMadmate.hasTasks)
                {
                    Helpers.ClearAllTasks(targetPlayer);
                    targetPlayer.GenerateAndAssignTasks(0, CreatedMadmate.numTasks, 0);
                }
            }

            FastDestroyableSingleton<RoleManager>.Instance.SetRole(targetPlayer, RoleTypes.Crewmate);
            ErasePlayerRoles(targetPlayer.PlayerId, true, false);

            // Jackalバグ対応
            Jackal.FormerJackals = tmpFormerJackals;

            targetPlayer.AddModifier(ModifierType.CreatedMadmate);
        }
        evilHacker.canCreateMadmate = false;
        return;
    }

    public static void UseAdminTime(float time)
    {
        ModMapOptions.restrictAdminTime -= time;
    }

    public static void UseCameraTime(float time)
    {
        ModMapOptions.restrictCamerasTime -= time;
    }

    public static void UseVitalsTime(float time)
    {
        ModMapOptions.restrictVitalsTime -= time;
    }

    public static void trackerUsedTracker(byte targetId, byte trackerId)
    {
        var trackerPlayer = Helpers.PlayerById(trackerId);
        var tracker = Tracker.GetRole(trackerPlayer);
        tracker.usedTracker = true;
        foreach (var player in CachedPlayer.AllPlayers)
        {
            if (player.PlayerId == targetId)
            {
                tracker.tracked = player;
            }
        }
    }

    public static void setFutureErased(byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        Eraser.futureErased ??= [];
        if (player != null)
        {
            Eraser.futureErased.Add(player);
        }
    }

    public static void vampireSetBitten(byte targetId, byte performReset)
    {
        if (performReset != 0)
        {
            Vampire.bitten = null;
            return;
        }

        if (!Vampire.Exists) return;
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (player.PlayerId == targetId && !player.Data.IsDead)
            {
                Vampire.bitten = player;
            }
        }
    }

    public static void shareRealTasks(MessageReader reader)
    {
        var count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            var playerId = reader.ReadByte();
            var taskTmp = reader.ReadBytes(4);
            var x = System.BitConverter.ToSingle(taskTmp, 0);
            taskTmp = reader.ReadBytes(4);
            var y = System.BitConverter.ToSingle(taskTmp, 0);
            var pos = new Vector2(x, y);
            if (!Map.realTasks.ContainsKey(playerId)) Map.realTasks[playerId] = new Il2CppSystem.Collections.Generic.List<Vector2>();
            Map.realTasks[playerId].Add(pos);
        }
    }
    public static void PolusRandomSpawn(byte playerId, byte locId)
    {
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
        {
            // Delayed action
            if (p == 1f)
            {
                Vector2 InitialSpawnCenter = new(16.64f, -2.46f);
                Vector2 MeetingSpawnCenter = new(17.4f, -16.286f);
                Vector2 ElectricalSpawn = new(5.53f, -9.84f);
                Vector2 O2Spawn = new(3.28f, -21.67f);
                Vector2 SpecimenSpawn = new(36.54f, -20.84f);
                Vector2 LaboratorySpawn = new(34.91f, -6.50f);
                var loc = locId switch
                {
                    0 => InitialSpawnCenter,
                    1 => MeetingSpawnCenter,
                    2 => ElectricalSpawn,
                    3 => O2Spawn,
                    4 => SpecimenSpawn,
                    5 => LaboratorySpawn,
                    _ => InitialSpawnCenter,
                };
                foreach (var player in CachedPlayer.AllPlayers)
                {
                    if (player.Data.PlayerId == playerId)
                    {
                        player.PlayerControl.transform.position = loc;
                        break;
                    }
                }
            }
        })));
    }
    public static void synchronize(byte playerId, int tag)
    {
        SpawnIn.synchronizeData.Synchronize((SynchronizeTag)tag, playerId);
    }

    public static void placeCamera(byte[] buff, byte roomId, byte sgId)
    {
        var player = Helpers.PlayerById(sgId);
        var sg = SecurityGuard.GetRole(player);

        var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        sg.remainingScrews -= SecurityGuard.camPrice;
        sg.placedCameras++;

        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        SystemTypes roomType = (SystemTypes)roomId;

        var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
        camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {sg.placedCameras}";
        camera.Offset = new Vector3(0f, 0f, camera.Offset.z);

        camera.NewName = roomType switch
        {
            SystemTypes.Hallway => StringNames.Hallway,
            SystemTypes.Storage => StringNames.Storage,
            SystemTypes.Cafeteria => StringNames.Cafeteria,
            SystemTypes.Reactor => StringNames.Reactor,
            SystemTypes.UpperEngine => StringNames.UpperEngine,
            SystemTypes.Nav => StringNames.Nav,
            SystemTypes.Admin => StringNames.Admin,
            SystemTypes.Electrical => StringNames.Electrical,
            SystemTypes.LifeSupp => StringNames.LifeSupp,
            SystemTypes.Shields => StringNames.Shields,
            SystemTypes.MedBay => StringNames.MedBay,
            SystemTypes.Security => StringNames.Security,
            SystemTypes.Weapons => StringNames.Weapons,
            SystemTypes.LowerEngine => StringNames.LowerEngine,
            SystemTypes.Comms => StringNames.Comms,
            SystemTypes.Decontamination => StringNames.Decontamination,
            SystemTypes.Launchpad => StringNames.Launchpad,
            SystemTypes.LockerRoom => StringNames.LockerRoom,
            SystemTypes.Laboratory => StringNames.Laboratory,
            SystemTypes.Balcony => StringNames.Balcony,
            SystemTypes.Office => StringNames.Office,
            SystemTypes.Greenhouse => StringNames.Greenhouse,
            SystemTypes.Dropship => StringNames.Dropship,
            SystemTypes.Decontamination2 => StringNames.Decontamination2,
            SystemTypes.Outside => StringNames.Outside,
            SystemTypes.Specimens => StringNames.Specimens,
            SystemTypes.BoilerRoom => StringNames.BoilerRoom,
            SystemTypes.VaultRoom => StringNames.VaultRoom,
            SystemTypes.Cockpit => StringNames.Cockpit,
            SystemTypes.Armory => StringNames.Armory,
            SystemTypes.Kitchen => StringNames.Kitchen,
            SystemTypes.ViewingDeck => StringNames.ViewingDeck,
            SystemTypes.HallOfPortraits => StringNames.HallOfPortraits,
            SystemTypes.CargoBay => StringNames.CargoBay,
            SystemTypes.Ventilation => StringNames.Ventilation,
            SystemTypes.Showers => StringNames.Showers,
            SystemTypes.Engine => StringNames.Engine,
            SystemTypes.Brig => StringNames.Brig,
            SystemTypes.MeetingRoom => StringNames.MeetingRoom,
            SystemTypes.Records => StringNames.Records,
            SystemTypes.Lounge => StringNames.Lounge,
            SystemTypes.GapRoom => StringNames.GapRoom,
            SystemTypes.MainHall => StringNames.MainHall,
            SystemTypes.Medical => StringNames.Medical,
            _ => StringNames.ExitButton,
        };
        if (Helpers.GetOption(ByteOptionNames.MapId) is 2 or 4) camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship

        if (CachedPlayer.LocalPlayer.PlayerControl.PlayerId == sgId)
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }
        ModMapOptions.CamerasToAdd.Add(camera);
    }

    public static void sealVent(int ventId, byte sgId)
    {
        var player = Helpers.PlayerById(sgId);
        var sg = SecurityGuard.GetRole(player);

        Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
        if (vent == null) return;

        sg.remainingScrews -= SecurityGuard.ventPrice;
        if (CachedPlayer.LocalPlayer.PlayerControl.PlayerId == sgId)
        {
            PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            vent.myRend.sprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            vent.myRend.color = new Color(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        ModMapOptions.VentsToSeal.Add(vent);
    }

    public static void morphingMorph(byte playerId, byte morphId)
    {
        var morphPlayer = Helpers.PlayerById(morphId);
        var target = Helpers.PlayerById(playerId);
        if (morphPlayer == null || target == null) return;
        Morphing.GetRole(morphPlayer).startMorph(target);
    }

    public static void camouflagerCamouflage()
    {
        if (!Camouflager.Exists) return;
        Camouflager.startCamouflage();
    }
    public static void swapperSwap(byte playerId1, byte playerId2)
    {
        if (MeetingHud.Instance)
        {
            Swapper.playerId1 = playerId1;
            Swapper.playerId2 = playerId2;
        }
    }

    public static void swapperAnimate()
    {
        Meeting.animateSwap = true;
    }

    public static void setFutureSpelled(byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        Witch.futureSpelled ??= [];
        if (player != null)
        {
            Witch.futureSpelled.Add(player);
        }
    }

    public static void witchSpellCast(byte playerId)
    {
        UncheckedExilePlayer(playerId);
        GameHistory.FinalStatuses[playerId] = EFinalStatus.Spelled;
    }

    public static void placeGarlic(byte[] buff)
    {
        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        _ = new Garlic(position);
    }

    public static void impostorPromotesToLastImpostor(byte targetId)
    {
        var player = Helpers.PlayerById(targetId);
        player.AddModifier(ModifierType.LastImpostor);
    }
}