using Assets.CoreScripts;
using InnerNet;
using PowerTools;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules.RPC;

internal static partial class RPCProcedure
{
    public static PlayerControl OldHotPotato;

    public static void ResetVariables()
    {
        RoleAssignment.IsAssigned = false;
        Garlic.ClearGarlics();
        JackInTheBox.ClearJackInTheBoxes();
        MapSettings.ClearAndReloadMapOptions();
        RebuildUs.ClearAndReloadRoles();
        GameHistory.ClearGameHistory();
        EndGameMain.IsO2Win = false;
        RebuildUs.SetButtonCooldowns();
        Admin.ResetData();
        SecurityCamera.ResetData();
        Vitals.ResetData();
        Map.Reset();
        CustomOverlays.ResetOverlays();
        SpecimenVital.ClearAndReload();
        AdditionalVents.ClearAndReload();
        SpawnIn.Reset();
        Map.ResetRealTasks();

        KillAnimationPatch.HideNextAnimation = false;
        KillAnimationPatch.AvoidNextKillMovement = false;

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishResetVariables);
        sender.Write(PlayerControl.LocalPlayer.PlayerId);
        FinishResetVariables(PlayerControl.LocalPlayer.PlayerId);
    }

    private static void ShareOptions(MessageReader reader)
    {
        var amount = reader.ReadByte();
        for (var i = 0; i < amount; i++)
        {
            var id = reader.ReadPackedUInt32();
            var selection = reader.ReadPackedUInt32();
            if (CustomOption.AllOptionsById.TryGetValue((int)id, out var option))
                option.UpdateSelection((int)selection, option.GetOptionIcon());
        }
    }

    public static void WorkaroundSetRoles(byte numberOfRoles, MessageReader reader)
    {
        for (var i = 0; i < numberOfRoles; i++)
        {
            var playerId = (byte)reader.ReadPackedUInt32();
            var roleId = (byte)reader.ReadPackedUInt32();
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
        var player = Helpers.PlayerById(playerId);
        if (player == null) return;
        Logger.LogInfo($"{player.Data.PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), roleId)}", "setRole");
        player.SetRole((RoleType)roleId);
    }

    public static void AddModifier(byte modId, byte playerId)
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.PlayerId == playerId)
            {
                p.AddModifier((ModifierType)modId);
            }
        }
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, int clientId, Guid guid)
    {
        GameStart.PlayerVersions[clientId] = new(major, minor, build, revision, guid);
    }

    public static void FinishSetRole()
    {
        RoleAssignment.IsAssigned = true;
    }

    public static void UpdateMeeting(byte targetId, bool dead = true)
    {
        if (MeetingHud.Instance)
        {
            foreach (var pva in MeetingHud.Instance.playerStates)
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

            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
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
        var bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.StartAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    public static void UncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
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
        MapSettings.GameMode = (CustomGameMode)gameMode;
        GameStart.SendGamemode = false;
    }

    public static void SetGameStarting()
    {
        GameStart.StartingTimer = 5f;
    }

    public static void StopStart()
    {
        StopStartSound();
        if (AmongUsClient.Instance.AmHost) GameStartManager.Instance.ResetStartState();
    }

    public static void StopStartSound()
    {
        SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
    }

    public static void FinishResetVariables(byte playerId)
    {
        var checkList = RoleAssignment.CheckList;
        if (checkList != null)
        {
            if (checkList.ContainsKey(playerId))
                checkList[playerId] = true;
        }
    }

    public static void SetLovers(byte playerId1, byte playerId2)
    {
        Lovers.AddCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
    }

    public static void OverrideNativeRole(byte playerId, byte roleType)
    {
        var player = Helpers.PlayerById(playerId);
        player.roleAssigned = false;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
    }

    public static void UncheckedEndGame(byte reason, bool isO2Win = false)
    {
        EndGameMain.IsO2Win = isO2Win;
        AmongUsClient.Instance.GameState = InnerNetClient.GameStates.Ended;
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
                AmongUsClient.Instance.OnGameEnd(new((GameOverReason)reason, false));

                if (AmongUsClient.Instance.AmHost) GameManager.Instance.RpcEndGame((GameOverReason)reason, false);
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
        var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    public static void EngineerFixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void EngineerUsedRepair(byte engineerId)
    {
        var engineerPlayer = Helpers.PlayerById(engineerId);
        if (engineerPlayer == null) return;
        var engineer = Engineer.GetRole(engineerPlayer);
        if (engineer != null) engineer.RemainingFixes--;
    }

    public static void ArsonistDouse(byte playerId, byte arsonistId)
    {
        var arsonistPlayer = Helpers.PlayerById(arsonistId);
        if (arsonistPlayer == null) return;
        var arsonist = Arsonist.GetRole(arsonistPlayer);
        if (arsonist == null) return;
        var target = Helpers.PlayerById(playerId);
        if (target != null) arsonist.DousedPlayers.Add(target);
    }

    public static void ArsonistWin(byte arsonistId)
    {
        Arsonist.TriggerArsonistWin = true;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null && p.IsAlive() && !p.IsRole(RoleType.Arsonist))
            {
                p.Exiled();
                GameHistory.FINAL_STATUSES[p.PlayerId] = FinalStatus.Torched;
            }
        }
    }

    public static void CleanBody(byte playerId)
    {
        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            var info = GameData.Instance.GetPlayerById(array[i].ParentId);
            if (info != null && info.PlayerId == playerId) Object.Destroy(array[i].gameObject);
        }
    }

    public static void VultureEat(byte playerId, byte vultureId)
    {
        CleanBody(playerId);
        var vulturePlayer = Helpers.PlayerById(vultureId);
        if (vulturePlayer == null) return;
        var vulture = Vulture.GetRole(vulturePlayer);
        if (vulture != null) vulture.EatenBodies++;
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
        if (player.IsNeutral() && clearNeutralTasks) player.ClearAllTasks();

        player.EraseAllRoles();
        player.EraseAllModifiers();

        if (!ignoreLovers && player.IsLovers())
        {
            // The whole Lover couple is being erased
            Lovers.EraseCouple(player);
        }
    }

    public static void JackalCreatesSidekick(byte targetId, byte jackalId)
    {
        var target = Helpers.PlayerById(targetId);
        var jackalPlayer = Helpers.PlayerById(jackalId);
        var jackal = Jackal.GetRole(jackalPlayer);
        if (target == null) return;

        if (!Jackal.CanCreateSidekickFromImpostor && target.Data.Role.IsImpostor)
            jackal?.FakeSidekick = target;
        else
        {
            var wasSpy = target.IsRole(RoleType.Spy);
            var wasImpostor = target.IsTeamImpostor(); // This can only be reached if impostors can be sidekicked.
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
            ErasePlayerRoles(target.PlayerId, true);
            if (target.SetRole(RoleType.Sidekick))
            {
                var sidekick = Sidekick.GetRole(target);
                if (sidekick != null)
                {
                    if (wasSpy || wasImpostor) sidekick.WasTeamRed = true;
                    sidekick.WasSpy = wasSpy;
                    sidekick.WasImpostor = wasImpostor;
                }
            }

            if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
        }

        if (jackal != null)
        {
            jackal.CanSidekick = false;
            jackal.MySidekick = target;
        }
    }

    public static void SidekickPromotes(byte sidekickId)
    {
        var sidekickPlayer = Helpers.PlayerById(sidekickId);
        if (sidekickPlayer == null) return;
        var sidekick = Sidekick.GetRole(sidekickPlayer);
        if (sidekick == null) return;

        var wasTeamRed = sidekick.WasTeamRed;
        var wasImpostor = sidekick.WasImpostor;
        var wasSpy = sidekick.WasSpy;
        Jackal.RemoveCurrentJackal();
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(sidekickPlayer, RoleTypes.Crewmate);
        ErasePlayerRoles(sidekickPlayer.PlayerId, true);
        if (sidekickPlayer.SetRole(RoleType.Jackal))
        {
            var newJackal = Jackal.GetRole(sidekickPlayer);
            if (newJackal != null)
            {
                newJackal.CanSidekick = Jackal.JackalPromotedFromSidekickCanCreateSidekick;
                newJackal.WasTeamRed = wasTeamRed;
                newJackal.WasImpostor = wasImpostor;
                newJackal.WasSpy = wasSpy;
            }
        }

        Sidekick.Clear();
    }

    public static void MedicSetShielded(byte shieldedId)
    {
        Medic.UsedShield = true;
        Medic.Shielded = Helpers.PlayerById(shieldedId);
        Medic.FutureShielded = null;
    }

    public static void ShieldedMurderAttempt()
    {
        if (!Medic.Exists || Medic.Shielded == null) return;

        var isShieldedAndShow = Medic.Shielded == PlayerControl.LocalPlayer && Medic.ShowAttemptToShielded;
        var isMedicAndShow = PlayerControl.LocalPlayer.IsRole(RoleType.Medic) && Medic.ShowAttemptToMedic;

        if ((isShieldedAndShow || isMedicAndShow) && FastDestroyableSingleton<HudManager>.Instance?.FullScreen != null)
        {
            var c = Palette.ImpostorRed;
            Helpers.ShowFlash(new(c.r, c.g, c.b));
        }
    }

    public static void SetFutureShielded(byte playerId)
    {
        Medic.FutureShielded = Helpers.PlayerById(playerId);
        Medic.UsedShield = true;
    }

    public static void TimeMasterRewindTime()
    {
        TimeMaster.ShieldActive = false; // Shield is no longer active when rewinding
        if (PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster)) TimeMaster.ResetTimeMasterButton();
        var hm = FastDestroyableSingleton<HudManager>.Instance;
        hm.FullScreen.color = new(0f, 0.5f, 0.8f, 0.3f);
        hm.FullScreen.enabled = true;
        hm.FullScreen.gameObject.SetActive(true);
        hm.StartCoroutine(Effects.Lerp(TimeMaster.RewindTime / 2, new Action<float>(p =>
        {
            if (p == 1f) hm.FullScreen.enabled = false;
        })));

        if (!TimeMaster.Exists || PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster))
            return; // Time Master himself does not rewind
        if (PlayerControl.LocalPlayer.IsGm()) return; // GM does not rewind

        TimeMaster.IsRewinding = true;

        if (MapBehaviour.Instance) MapBehaviour.Instance.Close();
        if (Minigame.Instance) Minigame.Instance.ForceClose();
        PlayerControl.LocalPlayer.moveable = false;
    }

    public static void TimeMasterShield()
    {
        TimeMaster.ShieldActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.ShieldDuration,
                                                                         new Action<float>(p =>
                                                                         {
                                                                             if (p == 1f)
                                                                                 TimeMaster.ShieldActive = false;
                                                                         })));
    }

    public static void GuesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleType)
    {
        var killer = Helpers.PlayerById(killerId);
        var dyingTarget = Helpers.PlayerById(dyingTargetId);
        if (dyingTarget == null) return;
        dyingTarget.Exiled();
        var dyingLoverPartner = Lovers.BothDie ? dyingTarget.GetPartner() : null; // Lover check

        if (killer != null) Guesser.RemainingShots(killer, true);

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);

        if (FastDestroyableSingleton<HudManager>.Instance != null && killer != null)
        {
            if (PlayerControl.LocalPlayer == dyingTarget)
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data,
                    dyingTarget.Data);
            else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner)
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data,
                    dyingLoverPartner.Data);
        }

        var guessedTarget = Helpers.PlayerById(guessedTargetId);
        if (Guesser.ShowInfoInGhostChat && PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null)
        {
            RoleInfo roleInfo = null;
            foreach (var r in RoleInfo.AllRoleInfos)
            {
                if ((byte)r.RoleType == guessedRoleType)
                {
                    roleInfo = r;
                    break;
                }
            }

            if (roleInfo != null)
            {
                var msg = string.Format(Tr.Get(TrKey.GuesserGuessChat), roleInfo.Name, guessedTarget.Data.PlayerName);
                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(killer, msg);
                if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                    FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }
        }
    }

    public static void PlaceJackInTheBox(byte[] buff)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new JackInTheBox(position);
    }

    public static void LightsOut()
    {
        Trickster.LightsOutTimer = Trickster.LightsOutDuration;
        // If the local player is impostor indicate lights out
        if (Helpers.HasImpostorVision(PlayerControl.LocalPlayer))
            new CustomMessage("TricksterLightsOutText", Trickster.LightsOutDuration, new(0f, 1.8f), MessageType.Normal);
    }

    public static void EvilHackerCreatesMadmate(byte targetId, byte evilHackerId)
    {
        var targetPlayer = Helpers.PlayerById(targetId);
        var evilHackerPlayer = Helpers.PlayerById(evilHackerId);
        if (targetPlayer == null || evilHackerPlayer == null) return;
        var evilHacker = EvilHacker.GetRole(evilHackerPlayer);
        if (evilHacker == null) return;
        if (!EvilHacker.CanCreateMadmateFromJackal && targetPlayer.IsRole(RoleType.Jackal))
            evilHacker.FakeMadmate = targetPlayer;
        else
        {
            // Jackalバグ対応
            List<PlayerControl> tmpFormerJackals = [.. Jackal.FormerJackals];

            // タスクがないプレイヤーがMadmateになった場合はショートタスクを必要数割り当てる
            if (targetPlayer.HasFakeTasks())
            {
                if (CreatedMadmate.HasTasks)
                {
                    targetPlayer.ClearAllTasks();
                    targetPlayer.GenerateAndAssignTasks(0, CreatedMadmate.NumTasks, 0);
                }
            }

            FastDestroyableSingleton<RoleManager>.Instance.SetRole(targetPlayer, RoleTypes.Crewmate);
            ErasePlayerRoles(targetPlayer.PlayerId, true, false);

            // Jackalバグ対応
            Jackal.FormerJackals = tmpFormerJackals;

            targetPlayer.AddModifier(ModifierType.CreatedMadmate);
        }

        evilHacker.CanCreateMadmate = false;
    }

    public static void UseAdminTime(float time)
    {
        MapSettings.RestrictAdminTime -= time;
    }

    public static void UseCameraTime(float time)
    {
        MapSettings.RestrictCamerasTime -= time;
    }

    public static void UseVitalsTime(float time)
    {
        MapSettings.RestrictVitalsTime -= time;
    }

    public static void TrackerUsedTracker(byte targetId, byte trackerId)
    {
        var trackerPlayer = Helpers.PlayerById(trackerId);
        if (trackerPlayer == null) return;
        var tracker = Tracker.GetRole(trackerPlayer);
        if (tracker == null) return;

        tracker.UsedTracker = true;
        tracker.Tracked = Helpers.PlayerById(targetId);
    }

    public static void SetFutureErased(byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        Eraser.FutureErased ??= [];
        if (player != null) Eraser.FutureErased.Add(player);
    }

    public static void VampireSetBitten(byte targetId, byte performReset)
    {
        if (performReset != 0)
        {
            Vampire.Bitten = null;
            return;
        }

        if (!Vampire.Exists) return;
        var player = Helpers.PlayerById(targetId);
        if (player != null && !player.Data.IsDead) Vampire.Bitten = player;
    }

    public static void ShareRealTasks(MessageReader reader)
    {
        var count = reader.ReadByte();
        for (var i = 0; i < count; i++)
        {
            var playerId = reader.ReadByte();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            Vector2 pos = new(x, y);

            if (!Map.RealTasks.TryGetValue(playerId, out var list))
            {
                list = new();
                Map.RealTasks[playerId] = list;
            }

            list.Add(pos);
        }
    }

    public static void PolusRandomSpawn(byte playerId, byte locId)
    {
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>(p =>
        {
            // Delayed action
            if (p == 1f)
            {
                Vector2 initialSpawnCenter = new(16.64f, -2.46f);
                Vector2 meetingSpawnCenter = new(17.4f, -16.286f);
                Vector2 electricalSpawn = new(5.53f, -9.84f);
                Vector2 o2Spawn = new(3.28f, -21.67f);
                Vector2 specimenSpawn = new(36.54f, -20.84f);
                Vector2 laboratorySpawn = new(34.91f, -6.50f);
                var loc = locId switch
                {
                    0 => initialSpawnCenter,
                    1 => meetingSpawnCenter,
                    2 => electricalSpawn,
                    3 => o2Spawn,
                    4 => specimenSpawn,
                    5 => laboratorySpawn,
                    _ => initialSpawnCenter,
                };
                var player = Helpers.PlayerById(playerId);
                player?.transform.position = loc;
            }
        })));
    }

    public static void Synchronize(byte playerId, int tag)
    {
        SpawnIn.SynchronizeData.Synchronize((SynchronizeTag)tag, playerId);
    }

    public static void PlaceCamera(float x, float y, byte roomId, byte sgId)
    {
        var player = Helpers.PlayerById(sgId);
        var sg = SecurityGuard.GetRole(player);

        var referenceCamera = Object.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        sg.RemainingScrews -= SecurityGuard.CamPrice;
        sg.PlacedCameras++;

        Vector3 position = new(x, y);

        var roomType = (SystemTypes)roomId;

        var camera = Object.Instantiate(referenceCamera);
        camera.transform.position = new(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {sg.PlacedCameras}";
        camera.Offset = new(0f, 0f, camera.Offset.z);

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
        if (Helpers.GetOption(ByteOptionNames.MapId) is 2 or 4)
            camera.transform.localRotation = new(0, 0, 1, 1); // Polus and Airship

        if (PlayerControl.LocalPlayer.PlayerId == sgId)
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, 0.5f);
        }
        else
            camera.gameObject.SetActive(false);

        MapSettings.CamerasToAdd.Add(camera);
    }

    public static void SealVent(int ventId, byte sgId)
    {
        var player = Helpers.PlayerById(sgId);
        var sg = SecurityGuard.GetRole(player);

        Vent vent = null;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        for (var i = 0; i < allVents.Count; i++)
        {
            var v = allVents[i];
            if (v != null && v.Id == ventId)
            {
                vent = v;
                break;
            }
        }

        if (vent == null) return;

        sg.RemainingScrews -= SecurityGuard.VentPrice;
        if (PlayerControl.LocalPlayer.PlayerId == sgId)
        {
            var animator = vent.GetComponent<SpriteAnim>();
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            vent.myRend.sprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0)
                vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14)
                vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            vent.myRend.color = new(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        MapSettings.VentsToSeal.Add(vent);
    }

    public static void MorphingMorph(byte playerId, byte morphId)
    {
        var morphPlayer = Helpers.PlayerById(morphId);
        var target = Helpers.PlayerById(playerId);
        if (morphPlayer == null || target == null) return;
        Morphing.GetRole(morphPlayer).StartMorph(target);
    }

    public static void CamouflagerCamouflage()
    {
        if (!Camouflager.Exists) return;
        Camouflager.StartCamouflage();
    }

    public static void SwapperSwap(byte playerId1, byte playerId2)
    {
        if (MeetingHud.Instance)
        {
            Swapper.PlayerId1 = playerId1;
            Swapper.PlayerId2 = playerId2;
        }
    }

    public static void SwapperAnimate()
    {
        Meeting.AnimateSwap = true;
    }

    public static void SetFutureSpelled(byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        Witch.FutureSpelled ??= [];
        if (player != null) Witch.FutureSpelled.Add(player);
    }

    public static void WitchSpellCast(byte playerId)
    {
        UncheckedExilePlayer(playerId);
        GameHistory.FINAL_STATUSES[playerId] = FinalStatus.Spelled;
    }

    public static void PlaceGarlic(float x, float y)
    {
        _ = new Garlic(new Vector3(x, y));
    }

    public static void ImpostorPromotesToLastImpostor(byte targetId)
    {
        var player = Helpers.PlayerById(targetId);
        player.AddModifier(ModifierType.LastImpostor);
    }

    public static void ShifterShift(byte targetId)
    {
        if (Shifter.Players.Count == 0) return;
        var oldShifter = Shifter.Players[0];
        var player = Helpers.PlayerById(targetId);
        if (player == null || oldShifter == null) return;

        var oldShifterPlayer = oldShifter.Player;
        Shifter.FutureShift = null;

        // Suicide (exile) when impostor or impostor variants
        if (!Shifter.IsNeutral
            && (player.Data.Role.IsImpostor
                || player.IsNeutral()
                || player.HasModifier(ModifierType.Madmate)
                || player.IsRole(RoleType.Madmate)
                || player.IsRole(RoleType.Suicider)
                || player.HasModifier(ModifierType.CreatedMadmate)))
        {
            oldShifterPlayer.Exiled();
            GameHistory.FINAL_STATUSES[oldShifterPlayer.PlayerId] = FinalStatus.Suicide;
            return;
        }

        if (Shifter.ShiftsModifiers)
        {
            // Switch shield
            if (Medic.Shielded != null && Medic.Shielded == player)
                Medic.Shielded = oldShifterPlayer;
            else if (Medic.Shielded != null && Medic.Shielded == oldShifterPlayer) Medic.Shielded = player;

            player.SwapModifiers(oldShifterPlayer);
            Lovers.SwapLovers(oldShifterPlayer, player);
        }

        // Shift roles (now a true swap)
        player.SwapRoles(oldShifterPlayer);

        if (Shifter.IsNeutral)
        {
            Shifter.PastShifters.Add(oldShifterPlayer.PlayerId);

            if (player.Data.Role.IsImpostor)
            {
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(oldShifterPlayer, RoleTypes.Impostor);
            }
        }
        else
        {
            // For Crewmate Shifter, the original target (who now has the Shifter role due to the swap) should lose it and become a plain Crewmate.
            player.EraseRole(RoleType.Shifter);
        }

        // Set cooldowns to max for both players
        if (PlayerControl.LocalPlayer == oldShifterPlayer || PlayerControl.LocalPlayer == player)
            CustomButton.ResetAllCooldowns();
    }

    public static void SetFutureShifted(byte playerId)
    {
        if (Shifter.IsNeutral && !Shifter.ShiftPastShifters && Shifter.PastShifters.Contains(playerId))
            return;
        Shifter.FutureShift = Helpers.PlayerById(playerId);
    }

    public static void SetShifterType(bool isNeutral)
    {
        Shifter.IsNeutral = isNeutral;
    }

    public static void FortuneTellerUsedDivine(byte killerId, byte targetId)
    {
        LastImpostor.NumUsed += 1;
    }

    public static void SheriffKillRequest(byte sheriffId, byte targetId)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            var sheriff = Helpers.PlayerById(sheriffId);
            var target = Helpers.PlayerById(targetId);
            if (sheriff == null || target == null) return;

            var misfire = Sheriff.CheckKill(target);

            using var killSender = new RPCSender(sheriff.NetId, CustomRPC.SheriffKill);
            killSender.Write(sheriffId);
            killSender.Write(targetId);
            killSender.Write(misfire);

            SheriffKill(sheriffId, targetId, misfire);
        }
    }

    public static void SheriffKill(byte sheriffId, byte targetId, bool misfire)
    {
        var sheriff = Helpers.PlayerById(sheriffId);
        var target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null) return;

        var role = Sheriff.GetRole(sheriff);
        if (role != null) role.NumShots--;

        if (misfire)
        {
            sheriff.MurderPlayer(sheriff);
            GameHistory.FINAL_STATUSES[sheriffId] = FinalStatus.Misfire;

            if (!Sheriff.MisfireKillsTarget) return;
            GameHistory.FINAL_STATUSES[targetId] = FinalStatus.Misfire;
        }

        sheriff.MurderPlayer(target);
    }

    public static void GamemodeKills(byte targetId, byte sourceId)
    {
        var killer = Helpers.PlayerById(sourceId);
        var murdered = Helpers.PlayerById(targetId);

        switch (MapSettings.GameMode)
        {
            // CTF
            case CustomGameMode.CaptureTheFlag:
                if (CaptureTheFlag.StealerPlayer != null && sourceId == CaptureTheFlag.StealerPlayer.PlayerId)
                {
                    if (CaptureTheFlag.RedPlayerWhoHasBlueFlag != null
                        && murdered.PlayerId == CaptureTheFlag.RedPlayerWhoHasBlueFlag.PlayerId)
                    {
                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(CaptureTheFlag.StealerPlayer.PlayerId));
                        if (CaptureTheFlag.Redplayer01 != null
                            && CaptureTheFlag.Redplayer01 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer01);
                            CaptureTheFlag.Redplayer01 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer01.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer01.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer02 != null
                                 && CaptureTheFlag.Redplayer02 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer02);
                            CaptureTheFlag.Redplayer02 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer02.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer02.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer03 != null
                                 && CaptureTheFlag.Redplayer03 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer03);
                            CaptureTheFlag.Redplayer03 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer03.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer03.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer04 != null
                                 && CaptureTheFlag.Redplayer04 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer04);
                            CaptureTheFlag.Redplayer04 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer04.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer04.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer05 != null
                                 && CaptureTheFlag.Redplayer05 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer05);
                            CaptureTheFlag.Redplayer05 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer05.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer05.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer06 != null
                                 && CaptureTheFlag.Redplayer06 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer06);
                            CaptureTheFlag.Redplayer06 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer06.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer06.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Redplayer07 != null
                                 && CaptureTheFlag.Redplayer07 == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.RedteamFlag.Remove(CaptureTheFlag.Redplayer07);
                            CaptureTheFlag.Redplayer07 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.RedteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Redplayer07.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Redplayer07.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                    MurderResultFlags.Succeeded
                                                                    | MurderResultFlags.DecisionByHost);
                        }
                    }
                    else if (CaptureTheFlag.BluePlayerWhoHasRedFlag != null
                             && murdered.PlayerId == CaptureTheFlag.BluePlayerWhoHasRedFlag.PlayerId)
                    {
                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(CaptureTheFlag.StealerPlayer.PlayerId));
                        if (CaptureTheFlag.Blueplayer01 != null
                            && CaptureTheFlag.Blueplayer01 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer01);
                            CaptureTheFlag.Blueplayer01 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer01.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer01.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer02 != null
                                 && CaptureTheFlag.Blueplayer02 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer02);
                            CaptureTheFlag.Blueplayer02 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer02.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer02.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer03 != null
                                 && CaptureTheFlag.Blueplayer03 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer03);
                            CaptureTheFlag.Blueplayer03 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer03.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer03.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer04 != null
                                 && CaptureTheFlag.Blueplayer04 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer04);
                            CaptureTheFlag.Blueplayer04 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer04.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer04.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer05 != null
                                 && CaptureTheFlag.Blueplayer05 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer05);
                            CaptureTheFlag.Blueplayer05 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer05.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer05.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer06 != null
                                 && CaptureTheFlag.Blueplayer06 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer06);
                            CaptureTheFlag.Blueplayer06 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer06.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer06.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.Blueplayer07 != null
                                 && CaptureTheFlag.Blueplayer07 == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.BlueteamFlag.Remove(CaptureTheFlag.Blueplayer07);
                            CaptureTheFlag.Blueplayer07 = CaptureTheFlag.StealerPlayer;
                            CaptureTheFlag.BlueteamFlag.Add(CaptureTheFlag.StealerPlayer);
                            CaptureTheFlag.StealerPlayer = murdered;
                            CaptureTheFlag.Blueplayer07.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.Blueplayer07.MurderPlayer(CaptureTheFlag.StealerPlayer,
                                                                     MurderResultFlags.Succeeded
                                                                     | MurderResultFlags.DecisionByHost);
                        }
                    }
                    else
                    {
                        CaptureTheFlag.StealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                        killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        CaptureTheFlag.StealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    }
                }
                else
                    killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);

                break;
            // PAT
            case CustomGameMode.PoliceAndThieves:
                killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                break;
            // BR
            case CustomGameMode.BattleRoyale:
                // Hit sound
                if (murdered == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(AssetLoader.RoyaleGetHit, false, 100f);

                if (BattleRoyale.MatchType == 0)
                {
                    new BattleRoyaleFootprint(murdered, 0);

                    // Remove 1 life and check remaining lifes
                    if (BattleRoyale.SoloPlayer01 != null && murdered == BattleRoyale.SoloPlayer01)
                    {
                        BattleRoyale.SoloPlayer01Lifes -= 1;
                        if (BattleRoyale.SoloPlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer02 != null && murdered == BattleRoyale.SoloPlayer02)
                    {
                        BattleRoyale.SoloPlayer02Lifes -= 1;
                        if (BattleRoyale.SoloPlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer03 != null && murdered == BattleRoyale.SoloPlayer03)
                    {
                        BattleRoyale.SoloPlayer03Lifes -= 1;
                        if (BattleRoyale.SoloPlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer04 != null && murdered == BattleRoyale.SoloPlayer04)
                    {
                        BattleRoyale.SoloPlayer04Lifes -= 1;
                        if (BattleRoyale.SoloPlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer05 != null && murdered == BattleRoyale.SoloPlayer05)
                    {
                        BattleRoyale.SoloPlayer05Lifes -= 1;
                        if (BattleRoyale.SoloPlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer06 != null && murdered == BattleRoyale.SoloPlayer06)
                    {
                        BattleRoyale.SoloPlayer06Lifes -= 1;
                        if (BattleRoyale.SoloPlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer07 != null && murdered == BattleRoyale.SoloPlayer07)
                    {
                        BattleRoyale.SoloPlayer07Lifes -= 1;
                        if (BattleRoyale.SoloPlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer08 != null && murdered == BattleRoyale.SoloPlayer08)
                    {
                        BattleRoyale.SoloPlayer08Lifes -= 1;
                        if (BattleRoyale.SoloPlayer08Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer09 != null && murdered == BattleRoyale.SoloPlayer09)
                    {
                        BattleRoyale.SoloPlayer09Lifes -= 1;
                        if (BattleRoyale.SoloPlayer09Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer10 != null && murdered == BattleRoyale.SoloPlayer10)
                    {
                        BattleRoyale.SoloPlayer10Lifes -= 1;
                        if (BattleRoyale.SoloPlayer10Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer11 != null && murdered == BattleRoyale.SoloPlayer11)
                    {
                        BattleRoyale.SoloPlayer11Lifes -= 1;
                        if (BattleRoyale.SoloPlayer11Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer12 != null && murdered == BattleRoyale.SoloPlayer12)
                    {
                        BattleRoyale.SoloPlayer12Lifes -= 1;
                        if (BattleRoyale.SoloPlayer12Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer13 != null && murdered == BattleRoyale.SoloPlayer13)
                    {
                        BattleRoyale.SoloPlayer13Lifes -= 1;
                        if (BattleRoyale.SoloPlayer13Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer14 != null && murdered == BattleRoyale.SoloPlayer14)
                    {
                        BattleRoyale.SoloPlayer14Lifes -= 1;
                        if (BattleRoyale.SoloPlayer14Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.SoloPlayer15 != null && murdered == BattleRoyale.SoloPlayer15)
                    {
                        BattleRoyale.SoloPlayer15Lifes -= 1;
                        if (BattleRoyale.SoloPlayer15Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            BattleRoyaleCheckWin(0);
                            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                }
                else
                {
                    if (BattleRoyale.SerialKiller != null && killer == BattleRoyale.SerialKiller)
                        BattleRoyale.SerialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                    // Remove 1 life and check remaining lifes
                    if (BattleRoyale.LimePlayer01 != null && murdered == BattleRoyale.LimePlayer01)
                    {
                        BattleRoyale.LimePlayer01Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer02 != null && murdered == BattleRoyale.LimePlayer02)
                    {
                        BattleRoyale.LimePlayer02Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer03 != null && murdered == BattleRoyale.LimePlayer03)
                    {
                        BattleRoyale.LimePlayer03Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer04 != null && murdered == BattleRoyale.LimePlayer04)
                    {
                        BattleRoyale.LimePlayer04Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer05 != null && murdered == BattleRoyale.LimePlayer05)
                    {
                        BattleRoyale.LimePlayer05Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer06 != null && murdered == BattleRoyale.LimePlayer06)
                    {
                        BattleRoyale.LimePlayer06Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.LimePlayer07 != null && murdered == BattleRoyale.LimePlayer07)
                    {
                        BattleRoyale.LimePlayer07Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.LimePlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(1);
                                    Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 1);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer01 != null && murdered == BattleRoyale.PinkPlayer01)
                    {
                        BattleRoyale.PinkPlayer01Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer02 != null && murdered == BattleRoyale.PinkPlayer02)
                    {
                        BattleRoyale.PinkPlayer02Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer03 != null && murdered == BattleRoyale.PinkPlayer03)
                    {
                        BattleRoyale.PinkPlayer03Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer04 != null && murdered == BattleRoyale.PinkPlayer04)
                    {
                        BattleRoyale.PinkPlayer04Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer05 != null && murdered == BattleRoyale.PinkPlayer05)
                    {
                        BattleRoyale.PinkPlayer05Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer06 != null && murdered == BattleRoyale.PinkPlayer06)
                    {
                        BattleRoyale.PinkPlayer06Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.PinkPlayer07 != null && murdered == BattleRoyale.PinkPlayer07)
                    {
                        BattleRoyale.PinkPlayer07Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.PinkPlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(2);
                                    Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.SerialKiller != null
                                        && sourceId == BattleRoyale.SerialKiller.PlayerId)
                                    {
                                        BattleRoyaleScoreCheck(3, 1);
                                        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(1, 1);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.SerialKiller != null && murdered == BattleRoyale.SerialKiller)
                    {
                        BattleRoyale.SerialKillerLifes -= 1;
                        new BattleRoyaleFootprint(murdered, 3);
                        if (BattleRoyale.SerialKillerLifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.MatchType)
                            {
                                case 1:
                                    BattleRoyaleCheckWin(3);
                                    Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if ((BattleRoyale.LimePlayer01 != null
                                         && sourceId == BattleRoyale.LimePlayer01.PlayerId)
                                        || (BattleRoyale.LimePlayer02 != null
                                            && sourceId == BattleRoyale.LimePlayer02.PlayerId)
                                        || (BattleRoyale.LimePlayer03 != null
                                            && sourceId == BattleRoyale.LimePlayer03.PlayerId)
                                        || (BattleRoyale.LimePlayer04 != null
                                            && sourceId == BattleRoyale.LimePlayer04.PlayerId)
                                        || (BattleRoyale.LimePlayer05 != null
                                            && sourceId == BattleRoyale.LimePlayer05.PlayerId)
                                        || (BattleRoyale.LimePlayer06 != null
                                            && sourceId == BattleRoyale.LimePlayer06.PlayerId)
                                        || (BattleRoyale.LimePlayer07 != null
                                            && sourceId == BattleRoyale.LimePlayer07.PlayerId))
                                    {
                                        BattleRoyaleScoreCheck(1, 3);
                                        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        BattleRoyaleScoreCheck(2, 3);
                                        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }

                                    break;
                            }
                        }
                    }

                    if (BattleRoyale.SerialKiller != null && killer == BattleRoyale.SerialKiller)
                        BattleRoyale.SerialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                }

                break;
        }
    }

    public static void CaptureTheFlagWhoTookTheFlag(byte playerWhoStoleTheFlag, int redorblue)
    {
        var playerWhoGotTheFlag = Helpers.PlayerById(playerWhoStoleTheFlag);

        // Red team steal blue flag
        if (redorblue == 1)
        {
            CaptureTheFlag.Blueflagtaken = true;
            CaptureTheFlag.RedPlayerWhoHasBlueFlag = playerWhoGotTheFlag;
            CaptureTheFlag.Blueflag.transform.parent = playerWhoGotTheFlag.transform;
            CaptureTheFlag.Blueflag.transform.localPosition = new(0f, 0f, -0.1f);
            Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(CaptureTheFlag.RedPlayerWhoHasBlueFlag.PlayerId));
        }

        // Blue team steal red flag
        if (redorblue == 2)
        {
            CaptureTheFlag.Redflagtaken = true;
            CaptureTheFlag.BluePlayerWhoHasRedFlag = playerWhoGotTheFlag;
            CaptureTheFlag.Redflag.transform.parent = playerWhoGotTheFlag.transform;
            CaptureTheFlag.Redflag.transform.localPosition = new(0f, 0f, -0.1f);
            Helpers.ShowGamemodesPopUp(4, Helpers.PlayerById(CaptureTheFlag.BluePlayerWhoHasRedFlag.PlayerId));
        }
    }

    public static void CaptureTheFlagWhichTeamScored(int whichteam)
    {
        // Red team
        if (whichteam == 1)
        {
            Helpers.ShowGamemodesPopUp(5, Helpers.PlayerById(CaptureTheFlag.RedPlayerWhoHasBlueFlag.PlayerId));
            CaptureTheFlag.Blueflagtaken = false;
            CaptureTheFlag.RedPlayerWhoHasBlueFlag = null;
            CaptureTheFlag.Blueflag.transform.parent = CaptureTheFlag.Blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        CaptureTheFlag.Blueflag.transform.position = new(7.7f, -1.15f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        CaptureTheFlag.Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    else
                        CaptureTheFlag.Blueflag.transform.position = new(16.5f, -4.65f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.Blueflag.transform.position = new(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.Blueflag.transform.position = new(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.Blueflag.transform.position = new(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.Blueflag.transform.position = new(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.Blueflag.transform.position = new(12.5f, -31.45f, -0.011f);
                    break;
            }

            CaptureTheFlag.CurrentRedTeamPoints += 1;
            CaptureTheFlag.FlagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>")
                .Append(CaptureTheFlag.CurrentRedTeamPoints)
                .Append("</color> - <color=#0000FFFF>")
                .Append(CaptureTheFlag.CurrentBlueTeamPoints)
                .Append("</color>")
                .ToString();
            if (CaptureTheFlag.CurrentRedTeamPoints >= CaptureTheFlag.RequiredFlags)
            {
                CaptureTheFlag.TriggerRedTeamWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RedTeamFlagWin, false);
            }
        }

        // Blue team
        if (whichteam == 2)
        {
            Helpers.ShowGamemodesPopUp(6, Helpers.PlayerById(CaptureTheFlag.BluePlayerWhoHasRedFlag.PlayerId));
            CaptureTheFlag.Redflagtaken = false;
            CaptureTheFlag.BluePlayerWhoHasRedFlag = null;
            CaptureTheFlag.Redflag.transform.parent = CaptureTheFlag.Redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        CaptureTheFlag.Redflag.transform.position = new(-17.5f, -1.35f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        CaptureTheFlag.Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    else
                        CaptureTheFlag.Redflag.transform.position = new(-20.5f, -5.35f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.Redflag.transform.position = new(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.Redflag.transform.position = new(36.4f, -21.7f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.Redflag.transform.position = new(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.Redflag.transform.position = new(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.Redflag.transform.position = new(-8.35f, 28.05f, -0.011f);
                    break;
            }

            CaptureTheFlag.CurrentBlueTeamPoints += 1;
            CaptureTheFlag.FlagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>")
                .Append(CaptureTheFlag.CurrentRedTeamPoints)
                .Append("</color> - <color=#0000FFFF>")
                .Append(CaptureTheFlag.CurrentBlueTeamPoints)
                .Append("</color>")
                .ToString();
            ;
            if (CaptureTheFlag.CurrentBlueTeamPoints >= CaptureTheFlag.RequiredFlags)
            {
                CaptureTheFlag.TriggerBlueTeamWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BlueTeamFlagWin, false);
            }
        }
    }

    public static void PoliceandThiefJail(byte thiefId)
    {
        var capturedThief = Helpers.PlayerById(thiefId);

        if (capturedThief.inVent)
        {
            foreach (var vent in ShipStatus.Instance.AllVents)
            {
                bool canUse;
                bool couldUse;
                vent.CanUse(capturedThief.Data, out canUse, out couldUse);
                if (canUse)
                {
                    capturedThief.MyPhysics.RpcExitVent(vent.Id);
                    vent.SetButtons(false);
                }
            }
        }

        if (PlayerControl.LocalPlayer == capturedThief)
            SoundManager.Instance.PlaySound(AssetLoader.JailSound, false, 100f);
        PoliceAndThief.ThiefArrested.Add(capturedThief);
        if (PoliceAndThief.Thiefplayer01 != null && thiefId == PoliceAndThief.Thiefplayer01.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer01IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer01JewelId);
        }
        else if (PoliceAndThief.Thiefplayer02 != null && thiefId == PoliceAndThief.Thiefplayer02.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer02IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer02JewelId);
        }
        else if (PoliceAndThief.Thiefplayer03 != null && thiefId == PoliceAndThief.Thiefplayer03.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer03IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer03JewelId);
        }
        else if (PoliceAndThief.Thiefplayer04 != null && thiefId == PoliceAndThief.Thiefplayer04.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer04IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer04JewelId);
        }
        else if (PoliceAndThief.Thiefplayer05 != null && thiefId == PoliceAndThief.Thiefplayer05.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer05IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer05JewelId);
        }
        else if (PoliceAndThief.Thiefplayer06 != null && thiefId == PoliceAndThief.Thiefplayer06.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer06IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer06JewelId);
        }
        else if (PoliceAndThief.Thiefplayer07 != null && thiefId == PoliceAndThief.Thiefplayer07.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer07IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer07JewelId);
        }
        else if (PoliceAndThief.Thiefplayer08 != null && thiefId == PoliceAndThief.Thiefplayer08.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer08IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer08JewelId);
        }
        else if (PoliceAndThief.Thiefplayer09 != null && thiefId == PoliceAndThief.Thiefplayer09.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer09IsStealing)
                PoliceandThiefRevertedJewelPosition(thiefId, PoliceAndThief.Thiefplayer09JewelId);
        }

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                    capturedThief.transform.position = new(-12f, 7.15f, capturedThief.transform.position.z);
                else if (RebuildUs.ActivatedDleks)
                    capturedThief.transform.position = new(10.2f, 3.6f, capturedThief.transform.position.z);
                else
                    capturedThief.transform.position = new(-10.2f, 3.6f, capturedThief.transform.position.z);
                break;
            // MiraHQ
            case 1:
                capturedThief.transform.position = new(1.8f, 1.25f, capturedThief.transform.position.z);
                break;
            // Polus
            case 2:
                capturedThief.transform.position = new(8.25f, -5f, capturedThief.transform.position.z);
                break;
            // Dleks
            case 3:
                capturedThief.transform.position = new(10.2f, 3.6f, capturedThief.transform.position.z);
                break;
            // Airship
            case 4:
                capturedThief.transform.position = new(-18.5f, 3.5f, capturedThief.transform.position.z);
                break;
            // Fungle
            case 5:
                capturedThief.transform.position = new(-26.75f, -0.5f, capturedThief.transform.position.z);
                break;
            // Submerged
            case 6:
                if (capturedThief.transform.position.y > 0)
                    capturedThief.transform.position = new(-6f, 32f, capturedThief.transform.position.z);
                else
                    capturedThief.transform.position = new(-14.1f, -39f, capturedThief.transform.position.z);
                break;
        }

        PoliceAndThief.CurrentThiefsCaptured += 1;
        Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(capturedThief.PlayerId));
        PoliceAndThief.ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>")
            .Append(PoliceAndThief.CurrentJewelsStoled)
            .Append(" / ")
            .Append(PoliceAndThief.RequiredJewels)
            .Append("</color> | ")
            .Append(Tr.Get(TrKey.CapturedThieves))
            .Append("<color=#928B55FF>")
            .Append(PoliceAndThief.CurrentThiefsCaptured)
            .Append(" / ")
            .Append(PoliceAndThief.ThiefTeam.Count)
            .Append("</color>")
            .ToString();
        if (PoliceAndThief.CurrentThiefsCaptured == PoliceAndThief.ThiefTeam.Count)
        {
            PoliceAndThief.TriggerPoliceWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
        }
    }

    public static void PoliceandThiefFreeThief()
    {
        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                    PoliceAndThief.ThiefArrested[0].transform.position = new(13.75f, -0.2f,
                                                                             PoliceAndThief.ThiefArrested[0].transform
                                                                                 .position.z);
                else if (RebuildUs.ActivatedDleks)
                    PoliceAndThief.ThiefArrested[0].transform.position = new(1.31f, -16.25f,
                                                                             PoliceAndThief.ThiefArrested[0].transform
                                                                                 .position.z);
                else
                    PoliceAndThief.ThiefArrested[0].transform.position = new(-1.31f, -16.25f,
                                                                             PoliceAndThief.ThiefArrested[0].transform
                                                                                 .position.z);

                break;
            // MiraHQ
            case 1:
                PoliceAndThief.ThiefArrested[0].transform.position =
                    new(17.75f, 11.5f, PoliceAndThief.ThiefArrested[0].transform.position.z);
                break;
            // Polus
            case 2:
                PoliceAndThief.ThiefArrested[0].transform.position =
                    new(30f, -15.75f, PoliceAndThief.ThiefArrested[0].transform.position.z);
                break;
            // Dleks
            case 3:
                PoliceAndThief.ThiefArrested[0].transform.position = new(1.31f, -16.25f,
                                                                         PoliceAndThief.ThiefArrested[0].transform
                                                                             .position.z);
                break;
            // Airship
            case 4:
                PoliceAndThief.ThiefArrested[0].transform.position =
                    new(7.15f, -14.5f, PoliceAndThief.ThiefArrested[0].transform.position.z);
                break;
            // Fungle
            case 5:
                PoliceAndThief.ThiefArrested[0].transform.position =
                    new(20f, 11f, PoliceAndThief.ThiefArrested[0].transform.position.z);
                break;
            // Submerged
            case 6:
                if (PoliceAndThief.ThiefArrested[0].transform.position.y > 0)
                    PoliceAndThief.ThiefArrested[0].transform.position =
                        new(1f, 10f, PoliceAndThief.ThiefArrested[0].transform.position.z);
                else
                    PoliceAndThief.ThiefArrested[0].transform.position = new(12.5f, -31.75f,
                                                                             PoliceAndThief.ThiefArrested[0].transform
                                                                                 .position.z);

                break;
        }

        Helpers.ShowGamemodesPopUp(2, Helpers.PlayerById(PoliceAndThief.ThiefArrested[0].PlayerId));
        PoliceAndThief.ThiefArrested.RemoveAt(0);
        PoliceAndThief.CurrentThiefsCaptured = PoliceAndThief.CurrentThiefsCaptured - 1;
        PoliceAndThief.ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>")
            .Append(PoliceAndThief.CurrentJewelsStoled)
            .Append(" / ")
            .Append(PoliceAndThief.RequiredJewels)
            .Append("</color> | ")
            .Append(Tr.Get(TrKey.CapturedThieves))
            .Append("<color=#928B55FF>")
            .Append(PoliceAndThief.CurrentThiefsCaptured)
            .Append(" / ")
            .Append(PoliceAndThief.ThiefTeam.Count)
            .Append("</color>")
            .ToString();
        ;
    }

    public static void PoliceandThiefTakeJewel(byte thiefWhoTookATreasure, byte jewelId)
    {
        var thiefTookJewel = Helpers.PlayerById(thiefWhoTookATreasure);

        if (PoliceAndThief.Thiefplayer01 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer01.PlayerId)
        {
            PoliceAndThief.Thiefplayer01IsStealing = true;
            PoliceAndThief.Thiefplayer01JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer02 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer02.PlayerId)
        {
            PoliceAndThief.Thiefplayer02IsStealing = true;
            PoliceAndThief.Thiefplayer02JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer03 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer03.PlayerId)
        {
            PoliceAndThief.Thiefplayer03IsStealing = true;
            PoliceAndThief.Thiefplayer03JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer04 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer04.PlayerId)
        {
            PoliceAndThief.Thiefplayer04IsStealing = true;
            PoliceAndThief.Thiefplayer04JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer05 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer05.PlayerId)
        {
            PoliceAndThief.Thiefplayer05IsStealing = true;
            PoliceAndThief.Thiefplayer05JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer06 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer06.PlayerId)
        {
            PoliceAndThief.Thiefplayer06IsStealing = true;
            PoliceAndThief.Thiefplayer06JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer07 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer07.PlayerId)
        {
            PoliceAndThief.Thiefplayer07IsStealing = true;
            PoliceAndThief.Thiefplayer07JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer08 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer08.PlayerId)
        {
            PoliceAndThief.Thiefplayer08IsStealing = true;
            PoliceAndThief.Thiefplayer08JewelId = jewelId;
        }
        else if (PoliceAndThief.Thiefplayer09 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer09.PlayerId)
        {
            PoliceAndThief.Thiefplayer09IsStealing = true;
            PoliceAndThief.Thiefplayer09JewelId = jewelId;
        }

        switch (jewelId)
        {
            case 1:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel01);
                PoliceAndThief.Jewel01BeingStealed = thiefTookJewel;
                break;
            case 2:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel02);
                PoliceAndThief.Jewel02BeingStealed = thiefTookJewel;
                break;
            case 3:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel03);
                PoliceAndThief.Jewel03BeingStealed = thiefTookJewel;
                break;
            case 4:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel04);
                PoliceAndThief.Jewel04BeingStealed = thiefTookJewel;
                break;
            case 5:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel05);
                PoliceAndThief.Jewel05BeingStealed = thiefTookJewel;
                break;
            case 6:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel06);
                PoliceAndThief.Jewel06BeingStealed = thiefTookJewel;
                break;
            case 7:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel07);
                PoliceAndThief.Jewel07BeingStealed = thiefTookJewel;
                break;
            case 8:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel08);
                PoliceAndThief.Jewel08BeingStealed = thiefTookJewel;
                break;
            case 9:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel09);
                PoliceAndThief.Jewel09BeingStealed = thiefTookJewel;
                break;
            case 10:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel10);
                PoliceAndThief.Jewel10BeingStealed = thiefTookJewel;
                break;
            case 11:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel11);
                PoliceAndThief.Jewel11BeingStealed = thiefTookJewel;
                break;
            case 12:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel12);
                PoliceAndThief.Jewel12BeingStealed = thiefTookJewel;
                break;
            case 13:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel13);
                PoliceAndThief.Jewel13BeingStealed = thiefTookJewel;
                break;
            case 14:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel14);
                PoliceAndThief.Jewel14BeingStealed = thiefTookJewel;
                break;
            case 15:
                ThiefCarryJewel(thiefTookJewel, PoliceAndThief.Jewel15);
                PoliceAndThief.Jewel15BeingStealed = thiefTookJewel;
                break;
        }
    }

    public static void ThiefCarryJewel(PlayerControl thief, GameObject jewel)
    {
        jewel.SetActive(true);
        jewel.transform.parent = thief.transform;
        jewel.transform.localPosition = new(0f, 0.7f, -0.1f);
    }

    public static void PoliceandThiefDeliverJewel(byte thiefWhoTookATreasure, byte jewelId)
    {
        var thiefDeliverJewel = Helpers.PlayerById(thiefWhoTookATreasure);

        // Thief player steal a jewel
        if (PoliceAndThief.Thiefplayer01 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer01.PlayerId)
        {
            PoliceAndThief.Thiefplayer01IsStealing = false;
            PoliceAndThief.Thiefplayer01JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer02 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer02.PlayerId)
        {
            PoliceAndThief.Thiefplayer02IsStealing = false;
            PoliceAndThief.Thiefplayer02JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer03 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer03.PlayerId)
        {
            PoliceAndThief.Thiefplayer03IsStealing = false;
            PoliceAndThief.Thiefplayer03JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer04 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer04.PlayerId)
        {
            PoliceAndThief.Thiefplayer04IsStealing = false;
            PoliceAndThief.Thiefplayer04JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer05 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer05.PlayerId)
        {
            PoliceAndThief.Thiefplayer05IsStealing = false;
            PoliceAndThief.Thiefplayer05JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer06 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer06.PlayerId)
        {
            PoliceAndThief.Thiefplayer06IsStealing = false;
            PoliceAndThief.Thiefplayer06JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer07 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer07.PlayerId)
        {
            PoliceAndThief.Thiefplayer07IsStealing = false;
            PoliceAndThief.Thiefplayer07JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer08 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer08.PlayerId)
        {
            PoliceAndThief.Thiefplayer08IsStealing = false;
            PoliceAndThief.Thiefplayer08JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer09 != null && thiefWhoTookATreasure == PoliceAndThief.Thiefplayer09.PlayerId)
        {
            PoliceAndThief.Thiefplayer09IsStealing = false;
            PoliceAndThief.Thiefplayer09JewelId = 0;
        }

        GameObject myJewel = null;
        var isDiamond = true;
        switch (jewelId)
        {
            case 1:
                myJewel = PoliceAndThief.Jewel01;
                PoliceAndThief.Jewel01BeingStealed = null;
                break;
            case 2:
                myJewel = PoliceAndThief.Jewel02;
                PoliceAndThief.Jewel02BeingStealed = null;
                break;
            case 3:
                myJewel = PoliceAndThief.Jewel03;
                PoliceAndThief.Jewel03BeingStealed = null;
                break;
            case 4:
                myJewel = PoliceAndThief.Jewel04;
                PoliceAndThief.Jewel04BeingStealed = null;
                break;
            case 5:
                myJewel = PoliceAndThief.Jewel05;
                PoliceAndThief.Jewel05BeingStealed = null;
                break;
            case 6:
                myJewel = PoliceAndThief.Jewel06;
                PoliceAndThief.Jewel06BeingStealed = null;
                break;
            case 7:
                myJewel = PoliceAndThief.Jewel07;
                PoliceAndThief.Jewel07BeingStealed = null;
                break;
            case 8:
                myJewel = PoliceAndThief.Jewel08;
                PoliceAndThief.Jewel08BeingStealed = null;
                break;
            case 9:
                myJewel = PoliceAndThief.Jewel09;
                PoliceAndThief.Jewel09BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 10:
                myJewel = PoliceAndThief.Jewel10;
                PoliceAndThief.Jewel10BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 11:
                myJewel = PoliceAndThief.Jewel11;
                PoliceAndThief.Jewel11BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 12:
                myJewel = PoliceAndThief.Jewel12;
                PoliceAndThief.Jewel12BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 13:
                myJewel = PoliceAndThief.Jewel13;
                PoliceAndThief.Jewel13BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 14:
                myJewel = PoliceAndThief.Jewel14;
                PoliceAndThief.Jewel14BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 15:
                myJewel = PoliceAndThief.Jewel15;
                PoliceAndThief.Jewel15BeingStealed = null;
                isDiamond = !isDiamond;
                break;
        }

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                {
                    if (isDiamond)
                        myJewel.transform.position = new(15.25f, -0.33f, thiefDeliverJewel.transform.position.z);
                    else
                        myJewel.transform.position = new(15.7f, -0.33f, thiefDeliverJewel.transform.position.z);
                }
                else if (RebuildUs.ActivatedDleks)
                {
                    if (isDiamond)
                        myJewel.transform.position = new(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                    else
                        myJewel.transform.position = new(-0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    if (isDiamond)
                        myJewel.transform.position = new(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                    else
                        myJewel.transform.position = new(0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                }

                break;
            // MiraHQ
            case 1:
                if (isDiamond)
                    myJewel.transform.position = new(19.65f, 13.9f, thiefDeliverJewel.transform.position.z);
                else
                    myJewel.transform.position = new(20.075f, 13.9f, thiefDeliverJewel.transform.position.z);
                break;
            // Polus
            case 2:
                if (isDiamond)
                    myJewel.transform.position = new(33.6f, 13.9f, thiefDeliverJewel.transform.position.z);
                else
                    myJewel.transform.position = new(34.05f, -15.9f, thiefDeliverJewel.transform.position.z);
                break;
            // Dleks
            case 3:
                if (isDiamond)
                    myJewel.transform.position = new(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                else
                    myJewel.transform.position = new(-0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                break;
            // Airship
            case 4:
                if (isDiamond)
                    myJewel.transform.position = new(11.75f, -16.35f, thiefDeliverJewel.transform.position.z);
                else
                    myJewel.transform.position = new(12.2f, -16.35f, thiefDeliverJewel.transform.position.z);
                break;
            // Fungle
            case 5:
                if (isDiamond)
                    myJewel.transform.position = new(17.25f, 8.95f, thiefDeliverJewel.transform.position.z);
                else
                    myJewel.transform.position = new(17.7f, 8.95f, thiefDeliverJewel.transform.position.z);
                break;
            // Submerged
            case 6:
                if (isDiamond)
                {
                    if (myJewel.transform.position.y > 0)
                        myJewel.transform.position = new(-1.4f, 8.65f, thiefDeliverJewel.transform.position.z);
                    else
                        myJewel.transform.position = new(12.7f, -35.35f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    if (myJewel.transform.position.y > 0)
                        myJewel.transform.position = new(-1f, 8.65f, thiefDeliverJewel.transform.position.z);
                    else
                        myJewel.transform.position = new(13.1f, -35.35f, thiefDeliverJewel.transform.position.z);
                }

                break;
        }

        myJewel.transform.SetParent(null);
        PoliceAndThief.CurrentJewelsStoled += 1;
        Helpers.ShowGamemodesPopUp(3, Helpers.PlayerById(thiefDeliverJewel.PlayerId));
        PoliceAndThief.ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>")
            .Append(PoliceAndThief.CurrentJewelsStoled)
            .Append(" / ")
            .Append(PoliceAndThief.RequiredJewels)
            .Append("</color> | ")
            .Append(Tr.Get(TrKey.CapturedThieves))
            .Append("<color=#928B55FF>")
            .Append(PoliceAndThief.CurrentThiefsCaptured)
            .Append(" / ")
            .Append(PoliceAndThief.ThiefTeam.Count)
            .Append("</color>")
            .ToString();
        if (PoliceAndThief.CurrentJewelsStoled >= PoliceAndThief.RequiredJewels)
        {
            PoliceAndThief.TriggerThiefWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModeThiefWin, false);
        }
    }

    public static void PoliceandThiefRevertedJewelPosition(byte thiefWhoLostJewel, byte jewelRevertedId)
    {
        if (PoliceAndThief.Thiefplayer01 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer01.PlayerId)
        {
            PoliceAndThief.Thiefplayer01IsStealing = false;
            PoliceAndThief.Thiefplayer01JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer02 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer02.PlayerId)
        {
            PoliceAndThief.Thiefplayer02IsStealing = false;
            PoliceAndThief.Thiefplayer02JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer03 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer03.PlayerId)
        {
            PoliceAndThief.Thiefplayer03IsStealing = false;
            PoliceAndThief.Thiefplayer03JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer04 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer04.PlayerId)
        {
            PoliceAndThief.Thiefplayer04IsStealing = false;
            PoliceAndThief.Thiefplayer04JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer05 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer05.PlayerId)
        {
            PoliceAndThief.Thiefplayer05IsStealing = false;
            PoliceAndThief.Thiefplayer05JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer06 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer06.PlayerId)
        {
            PoliceAndThief.Thiefplayer06IsStealing = false;
            PoliceAndThief.Thiefplayer06JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer07 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer07.PlayerId)
        {
            PoliceAndThief.Thiefplayer07IsStealing = false;
            PoliceAndThief.Thiefplayer07JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer08 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer08.PlayerId)
        {
            PoliceAndThief.Thiefplayer08IsStealing = false;
            PoliceAndThief.Thiefplayer08JewelId = 0;
        }
        else if (PoliceAndThief.Thiefplayer09 != null && thiefWhoLostJewel == PoliceAndThief.Thiefplayer09.PlayerId)
        {
            PoliceAndThief.Thiefplayer09IsStealing = false;
            PoliceAndThief.Thiefplayer09JewelId = 0;
        }

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.Jewel01.transform.SetParent(null);
                            PoliceAndThief.Jewel01.transform.position = new(6.95f, 4.95f, 1f);
                            PoliceAndThief.Jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.Jewel02.transform.SetParent(null);
                            PoliceAndThief.Jewel02.transform.position = new(-3.75f, 5.35f, 1f);
                            PoliceAndThief.Jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.Jewel03.transform.SetParent(null);
                            PoliceAndThief.Jewel03.transform.position = new(-7.7f, 11.3f, 1f);
                            PoliceAndThief.Jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.Jewel04.transform.SetParent(null);
                            PoliceAndThief.Jewel04.transform.position = new(-19.65f, 5.3f, 1f);
                            PoliceAndThief.Jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.Jewel05.transform.SetParent(null);
                            PoliceAndThief.Jewel05.transform.position = new(-19.65f, -8, 1f);
                            PoliceAndThief.Jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.Jewel06.transform.SetParent(null);
                            PoliceAndThief.Jewel06.transform.position = new(-5.45f, -13f, 1f);
                            PoliceAndThief.Jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.Jewel07.transform.SetParent(null);
                            PoliceAndThief.Jewel07.transform.position = new(-7.65f, -4.2f, 1f);
                            PoliceAndThief.Jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.Jewel08.transform.SetParent(null);
                            PoliceAndThief.Jewel08.transform.position = new(2f, -6.75f, 1f);
                            PoliceAndThief.Jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.Jewel09.transform.SetParent(null);
                            PoliceAndThief.Jewel09.transform.position = new(8.9f, 1.45f, 1f);
                            PoliceAndThief.Jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.Jewel10.transform.SetParent(null);
                            PoliceAndThief.Jewel10.transform.position = new(4.6f, -2.25f, 1f);
                            PoliceAndThief.Jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.Jewel11.transform.SetParent(null);
                            PoliceAndThief.Jewel11.transform.position = new(-5.05f, -0.88f, 1f);
                            PoliceAndThief.Jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.Jewel12.transform.SetParent(null);
                            PoliceAndThief.Jewel12.transform.position = new(-8.25f, -0.45f, 1f);
                            PoliceAndThief.Jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.Jewel13.transform.SetParent(null);
                            PoliceAndThief.Jewel13.transform.position = new(-19.75f, -1.55f, 1f);
                            PoliceAndThief.Jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.Jewel14.transform.SetParent(null);
                            PoliceAndThief.Jewel14.transform.position = new(-12.1f, -13.15f, 1f);
                            PoliceAndThief.Jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.Jewel15.transform.SetParent(null);
                            PoliceAndThief.Jewel15.transform.position = new(7.15f, -14.45f, 1f);
                            PoliceAndThief.Jewel15BeingStealed = null;
                            break;
                    }
                }
                else if (RebuildUs.ActivatedDleks)
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.Jewel01.transform.SetParent(null);
                            PoliceAndThief.Jewel01.transform.position = new(18.65f, -9.9f, 1f);
                            PoliceAndThief.Jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.Jewel02.transform.SetParent(null);
                            PoliceAndThief.Jewel02.transform.position = new(21.5f, -2, 1f);
                            PoliceAndThief.Jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.Jewel03.transform.SetParent(null);
                            PoliceAndThief.Jewel03.transform.position = new(5.9f, -8.25f, 1f);
                            PoliceAndThief.Jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.Jewel04.transform.SetParent(null);
                            PoliceAndThief.Jewel04.transform.position = new(-4.5f, -7.5f, 1f);
                            PoliceAndThief.Jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.Jewel05.transform.SetParent(null);
                            PoliceAndThief.Jewel05.transform.position = new(-7.85f, -14.45f, 1f);
                            PoliceAndThief.Jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.Jewel06.transform.SetParent(null);
                            PoliceAndThief.Jewel06.transform.position = new(-6.65f, -4.8f, 1f);
                            PoliceAndThief.Jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.Jewel07.transform.SetParent(null);
                            PoliceAndThief.Jewel07.transform.position = new(-10.5f, 2.15f, 1f);
                            PoliceAndThief.Jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.Jewel08.transform.SetParent(null);
                            PoliceAndThief.Jewel08.transform.position = new(5.5f, 3.5f, 1f);
                            PoliceAndThief.Jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.Jewel09.transform.SetParent(null);
                            PoliceAndThief.Jewel09.transform.position = new(19, -1.2f, 1f);
                            PoliceAndThief.Jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.Jewel10.transform.SetParent(null);
                            PoliceAndThief.Jewel10.transform.position = new(21.5f, -8.35f, 1f);
                            PoliceAndThief.Jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.Jewel11.transform.SetParent(null);
                            PoliceAndThief.Jewel11.transform.position = new(12.5f, -3.75f, 1f);
                            PoliceAndThief.Jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.Jewel12.transform.SetParent(null);
                            PoliceAndThief.Jewel12.transform.position = new(5.9f, -5.25f, 1f);
                            PoliceAndThief.Jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.Jewel13.transform.SetParent(null);
                            PoliceAndThief.Jewel13.transform.position = new(-2.65f, -16.5f, 1f);
                            PoliceAndThief.Jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.Jewel14.transform.SetParent(null);
                            PoliceAndThief.Jewel14.transform.position = new(-16.75f, -4.75f, 1f);
                            PoliceAndThief.Jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.Jewel15.transform.SetParent(null);
                            PoliceAndThief.Jewel15.transform.position = new(-3.8f, 3.5f, 1f);
                            PoliceAndThief.Jewel15BeingStealed = null;
                            break;
                    }
                }
                else
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.Jewel01.transform.SetParent(null);
                            PoliceAndThief.Jewel01.transform.position = new(-18.65f, -9.9f, 1f);
                            PoliceAndThief.Jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.Jewel02.transform.SetParent(null);
                            PoliceAndThief.Jewel02.transform.position = new(-21.5f, -2, 1f);
                            PoliceAndThief.Jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.Jewel03.transform.SetParent(null);
                            PoliceAndThief.Jewel03.transform.position = new(-5.9f, -8.25f, 1f);
                            PoliceAndThief.Jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.Jewel04.transform.SetParent(null);
                            PoliceAndThief.Jewel04.transform.position = new(4.5f, -7.5f, 1f);
                            PoliceAndThief.Jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.Jewel05.transform.SetParent(null);
                            PoliceAndThief.Jewel05.transform.position = new(7.85f, -14.45f, 1f);
                            PoliceAndThief.Jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.Jewel06.transform.SetParent(null);
                            PoliceAndThief.Jewel06.transform.position = new(6.65f, -4.8f, 1f);
                            PoliceAndThief.Jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.Jewel07.transform.SetParent(null);
                            PoliceAndThief.Jewel07.transform.position = new(10.5f, 2.15f, 1f);
                            PoliceAndThief.Jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.Jewel08.transform.SetParent(null);
                            PoliceAndThief.Jewel08.transform.position = new(-5.5f, 3.5f, 1f);
                            PoliceAndThief.Jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.Jewel09.transform.SetParent(null);
                            PoliceAndThief.Jewel09.transform.position = new(-19, -1.2f, 1f);
                            PoliceAndThief.Jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.Jewel10.transform.SetParent(null);
                            PoliceAndThief.Jewel10.transform.position = new(-21.5f, -8.35f, 1f);
                            PoliceAndThief.Jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.Jewel11.transform.SetParent(null);
                            PoliceAndThief.Jewel11.transform.position = new(-12.5f, -3.75f, 1f);
                            PoliceAndThief.Jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.Jewel12.transform.SetParent(null);
                            PoliceAndThief.Jewel12.transform.position = new(-5.9f, -5.25f, 1f);
                            PoliceAndThief.Jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.Jewel13.transform.SetParent(null);
                            PoliceAndThief.Jewel13.transform.position = new(2.65f, -16.5f, 1f);
                            PoliceAndThief.Jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.Jewel14.transform.SetParent(null);
                            PoliceAndThief.Jewel14.transform.position = new(16.75f, -4.75f, 1f);
                            PoliceAndThief.Jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.Jewel15.transform.SetParent(null);
                            PoliceAndThief.Jewel15.transform.position = new(3.8f, 3.5f, 1f);
                            PoliceAndThief.Jewel15BeingStealed = null;
                            break;
                    }
                }

                break;
            // MiraHQ
            case 1:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(-4.5f, 2.5f, 1f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(6.25f, 14f, 1f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(9.15f, 4.75f, 1f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(14.75f, 20.5f, 1f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(19.5f, 17.5f, 1f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(21, 24.1f, 1f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(19.5f, 4.75f, 1f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(28.25f, 0, 1f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(2.45f, 11.25f, 1f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(4.4f, 1.75f, 1f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(9.25f, 13f, 1f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(13.75f, 23.5f, 1f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(16, 4, 1f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(15.35f, -0.9f, 1f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(19.5f, -1.75f, 1f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
            // Polus
            case 2:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(16.7f, -2.65f, 0.75f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(25.35f, -7.35f, 0.75f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(34.9f, -9.75f, 0.75f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(36.5f, -21.75f, 0.75f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(17.25f, -17.5f, 0.75f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(10.9f, -20.5f, -0.75f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(1.5f, -20.25f, 0.75f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 08:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(3f, -12f, 0.75f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 09:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(30f, -7.35f, 0.75f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(40.25f, -8f, 0.75f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(26f, -17.15f, 0.75f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(22f, -25.25f, 0.75f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(20.65f, -12f, 0.75f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(9.75f, -12.25f, 0.75f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(2.25f, -24f, 0.75f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
            // Dleks
            case 3:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(18.65f, -9.9f, 1f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(21.5f, -2, 1f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(5.9f, -8.25f, 1f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(-4.5f, -7.5f, 1f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(-7.85f, -14.45f, 1f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(-6.65f, -4.8f, 1f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(-10.5f, 2.15f, 1f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(5.5f, 3.5f, 1f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(19, -1.2f, 1f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(21.5f, -8.35f, 1f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(12.5f, -3.75f, 1f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(5.9f, -5.25f, 1f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(-2.65f, -16.5f, 1f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(-16.75f, -4.75f, 1f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(-3.8f, 3.5f, 1f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
            // Airship
            case 4:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(-23.5f, -1.5f, 1f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(-14.15f, -4.85f, 1f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(-13.9f, -16.25f, 1f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(-0.85f, -2.5f, 1f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(-5, 8.5f, 1f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(19.3f, -4.15f, 1f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(19.85f, 8, 1f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(28.85f, -1.75f, 1f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(-14.5f, -8.5f, 1f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(6.3f, -2.75f, 1f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(20.75f, 2.5f, 1f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(29.25f, 7, 1f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(37.5f, -3.5f, 1f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(25.2f, -8.75f, 1f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(16.3f, -11, 1f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
            // Fungle
            case 5:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(-18.25f, 5f, 1f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(-22.65f, -7.15f, 1f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(2, 4.35f, 1f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(-3.15f, -10.5f, 0.9f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(23.7f, -7.8f, 1f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(-4.75f, -1.75f, 1f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(8f, -10f, 1f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(7f, 1.75f, 1f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(13.25f, 10, 1f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(22.3f, 3.3f, 1f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(20.5f, 7.35f, 1f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(24.15f, 14.45f, 1f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(-16.12f, 0.7f, 1f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(1.65f, -1.5f, 1f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(10.5f, -12, 1f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
            // Submerged
            case 6:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.transform.SetParent(null);
                        PoliceAndThief.Jewel01.transform.position = new(-15f, 17.5f, -1f);
                        PoliceAndThief.Jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.transform.SetParent(null);
                        PoliceAndThief.Jewel02.transform.position = new(8f, 32f, -1f);
                        PoliceAndThief.Jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.transform.SetParent(null);
                        PoliceAndThief.Jewel03.transform.position = new(-6.75f, 10f, -1f);
                        PoliceAndThief.Jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.transform.SetParent(null);
                        PoliceAndThief.Jewel04.transform.position = new(5.15f, 8f, -1f);
                        PoliceAndThief.Jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.transform.SetParent(null);
                        PoliceAndThief.Jewel05.transform.position = new(5f, -33.5f, -1f);
                        PoliceAndThief.Jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.transform.SetParent(null);
                        PoliceAndThief.Jewel06.transform.position = new(-4.15f, -33.5f, -1f);
                        PoliceAndThief.Jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.transform.SetParent(null);
                        PoliceAndThief.Jewel07.transform.position = new(-14f, -27.75f, -1f);
                        PoliceAndThief.Jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.transform.SetParent(null);
                        PoliceAndThief.Jewel08.transform.position = new(7.8f, -23.75f, -1f);
                        PoliceAndThief.Jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.transform.SetParent(null);
                        PoliceAndThief.Jewel09.transform.position = new(-6.75f, -42.75f, -1f);
                        PoliceAndThief.Jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.transform.SetParent(null);
                        PoliceAndThief.Jewel10.transform.position = new(13f, -25.25f, -1f);
                        PoliceAndThief.Jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.transform.SetParent(null);
                        PoliceAndThief.Jewel11.transform.position = new(-14f, -34.25f, -1f);
                        PoliceAndThief.Jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.transform.SetParent(null);
                        PoliceAndThief.Jewel12.transform.position = new(0f, -33.5f, -1f);
                        PoliceAndThief.Jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.transform.SetParent(null);
                        PoliceAndThief.Jewel13.transform.position = new(-6.5f, 14f, -1f);
                        PoliceAndThief.Jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.transform.SetParent(null);
                        PoliceAndThief.Jewel14.transform.position = new(14.25f, 24.5f, -1f);
                        PoliceAndThief.Jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.transform.SetParent(null);
                        PoliceAndThief.Jewel15.transform.position = new(-12.25f, 31f, -1f);
                        PoliceAndThief.Jewel15BeingStealed = null;
                        break;
                }

                break;
        }

        // if police can't see jewels, hide it after jailing a player
        if (PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer01
            || PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer02
            || PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer03
            || PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer04
            || PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer05
            || PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer06)
        {
            if (!PoliceAndThief.PoliceCanSeeJewels)
            {
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.Jewel01.SetActive(false);
                        break;
                    case 2:
                        PoliceAndThief.Jewel02.SetActive(false);
                        break;
                    case 3:
                        PoliceAndThief.Jewel03.SetActive(false);
                        break;
                    case 4:
                        PoliceAndThief.Jewel04.SetActive(false);
                        break;
                    case 5:
                        PoliceAndThief.Jewel05.SetActive(false);
                        break;
                    case 6:
                        PoliceAndThief.Jewel06.SetActive(false);
                        break;
                    case 7:
                        PoliceAndThief.Jewel07.SetActive(false);
                        break;
                    case 8:
                        PoliceAndThief.Jewel08.SetActive(false);
                        break;
                    case 9:
                        PoliceAndThief.Jewel09.SetActive(false);
                        break;
                    case 10:
                        PoliceAndThief.Jewel10.SetActive(false);
                        break;
                    case 11:
                        PoliceAndThief.Jewel11.SetActive(false);
                        break;
                    case 12:
                        PoliceAndThief.Jewel12.SetActive(false);
                        break;
                    case 13:
                        PoliceAndThief.Jewel13.SetActive(false);
                        break;
                    case 14:
                        PoliceAndThief.Jewel14.SetActive(false);
                        break;
                    case 15:
                        PoliceAndThief.Jewel15.SetActive(false);
                        break;
                }
            }
        }
    }

    public static void PoliceandThiefsTased(byte targetId)
    {
        var tased = Helpers.PlayerById(targetId);

        if (PlayerControl.LocalPlayer == tased)
        {
            SoundManager.Instance.PlaySound(AssetLoader.PoliceTaser, false, 100f);
            if (MapBehaviour.Instance) MapBehaviour.Instance.Close();
        }

        new Tased(PoliceAndThief.PoliceTaseDuration, tased);
        if (PoliceAndThief.Thiefplayer01 != null && targetId == PoliceAndThief.Thiefplayer01.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer01IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer01JewelId);
        }
        else if (PoliceAndThief.Thiefplayer02 != null && targetId == PoliceAndThief.Thiefplayer02.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer02IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer02JewelId);
        }
        else if (PoliceAndThief.Thiefplayer03 != null && targetId == PoliceAndThief.Thiefplayer03.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer03IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer03JewelId);
        }
        else if (PoliceAndThief.Thiefplayer04 != null && targetId == PoliceAndThief.Thiefplayer04.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer04IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer04JewelId);
        }
        else if (PoliceAndThief.Thiefplayer05 != null && targetId == PoliceAndThief.Thiefplayer05.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer05IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer05JewelId);
        }
        else if (PoliceAndThief.Thiefplayer06 != null && targetId == PoliceAndThief.Thiefplayer06.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer06IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer06JewelId);
        }
        else if (PoliceAndThief.Thiefplayer07 != null && targetId == PoliceAndThief.Thiefplayer07.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer07IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer07JewelId);
        }
        else if (PoliceAndThief.Thiefplayer08 != null && targetId == PoliceAndThief.Thiefplayer08.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer08IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer08JewelId);
        }
        else if (PoliceAndThief.Thiefplayer09 != null && targetId == PoliceAndThief.Thiefplayer09.PlayerId)
        {
            if (PoliceAndThief.Thiefplayer09IsStealing)
                PoliceandThiefRevertedJewelPosition(targetId, PoliceAndThief.Thiefplayer09JewelId);
        }
    }

    public static void HotPotatoTransfer(byte targetId)
    {
        var player = Helpers.PlayerById(targetId);

        if (HotPotato.HotPotatoPlayer != null)
        {
            if (!HotPotato.FirstPotatoTransfered) HotPotato.FirstPotatoTransfered = true;

            if (HotPotato.ResetTimeForTransfer)
                HotPotato.TimeforTransfer = HotPotato.SavedtimeforTransfer + 3f;
            else
                HotPotato.TimeforTransfer = HotPotato.TimeforTransfer + HotPotato.IncreaseTimeIfNoReset + 3f;

            OldHotPotato = HotPotato.HotPotatoPlayer;
            OldHotPotato.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);

            HotPotato.NOT_POTATO_TEAM.Add(OldHotPotato);

            // Switch role
            if (HotPotato.NotPotato01 != null && HotPotato.NotPotato01 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato01);
                HotPotato.NotPotato01 = OldHotPotato;
            }
            else if (HotPotato.NotPotato02 != null && HotPotato.NotPotato02 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato02);
                HotPotato.NotPotato02 = OldHotPotato;
            }
            else if (HotPotato.NotPotato03 != null && HotPotato.NotPotato03 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato03);
                HotPotato.NotPotato03 = OldHotPotato;
            }
            else if (HotPotato.NotPotato04 != null && HotPotato.NotPotato04 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato04);
                HotPotato.NotPotato04 = OldHotPotato;
            }
            else if (HotPotato.NotPotato05 != null && HotPotato.NotPotato05 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato05);
                HotPotato.NotPotato05 = OldHotPotato;
            }
            else if (HotPotato.NotPotato06 != null && HotPotato.NotPotato06 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato06);
                HotPotato.NotPotato06 = OldHotPotato;
            }
            else if (HotPotato.NotPotato07 != null && HotPotato.NotPotato07 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato07);
                HotPotato.NotPotato07 = OldHotPotato;
            }
            else if (HotPotato.NotPotato08 != null && HotPotato.NotPotato08 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato08);
                HotPotato.NotPotato08 = OldHotPotato;
            }
            else if (HotPotato.NotPotato09 != null && HotPotato.NotPotato09 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato09);
                HotPotato.NotPotato09 = OldHotPotato;
            }
            else if (HotPotato.NotPotato10 != null && HotPotato.NotPotato10 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato10);
                HotPotato.NotPotato10 = OldHotPotato;
            }
            else if (HotPotato.NotPotato11 != null && HotPotato.NotPotato11 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato11);
                HotPotato.NotPotato11 = OldHotPotato;
            }
            else if (HotPotato.NotPotato12 != null && HotPotato.NotPotato12 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato12);
                HotPotato.NotPotato12 = OldHotPotato;
            }
            else if (HotPotato.NotPotato13 != null && HotPotato.NotPotato13 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato13);
                HotPotato.NotPotato13 = OldHotPotato;
            }
            else if (HotPotato.NotPotato14 != null && HotPotato.NotPotato14 == player)
            {
                HotPotato.NOT_POTATO_TEAM.Remove(HotPotato.NotPotato14);
                HotPotato.NotPotato14 = OldHotPotato;
            }

            HotPotato.HotPotatoPlayer = player;
            HotPotato.HotPotatoPlayer.NetTransform.Halt();
            HotPotato.HotPotatoPlayer.moveable = false;
            HotPotato.HotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            HotPotato.HotPotatoObject.transform.position = HotPotato.HotPotatoPlayer.transform.position
                                                           + new Vector3(0, 0.5f, -0.25f);
            HotPotato.HotPotatoObject.transform.parent = HotPotato.HotPotatoPlayer.transform;

            HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>(p =>
            {
                // Delayed action
                if (p == 1f) HotPotato.HotPotatoPlayer.moveable = true;
            })));

            var notPotatosAlives = 0;
            HotPotato.NOT_POTATO_TEAM_ALIVE.Clear();
            foreach (var notPotato in HotPotato.NOT_POTATO_TEAM)
            {
                if (!notPotato.Data.IsDead)
                {
                    notPotatosAlives += 1;
                    HotPotato.NOT_POTATO_TEAM_ALIVE.Add(notPotato);
                }
            }

            Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(HotPotato.HotPotatoPlayer.PlayerId));

            HotPotato.HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus))
                                              .Append("<color=#808080FF>")
                                              .Append(HotPotato.HotPotatoPlayer.name)
                                              .Append("</color> | ")
                                              .Append(Tr.Get(TrKey.ColdPotatoes))
                                              .Append("<color=#00F7FFFF>")
                                              .Append(HotPotato.NOT_POTATO_TEAM.Count)
                                              .Append("</color>")
                                              .ToString();

            // Set custom cooldown to the hotpotato button
            HotPotato.HotPotatoButton.Timer = HotPotato.TransferCooldown;
            if (PlayerControl.LocalPlayer == HotPotato.HotPotatoPlayer || PlayerControl.LocalPlayer == OldHotPotato)
                SoundManager.Instance.PlaySound(AssetLoader.StealRoleSound, false, 100f);
        }
    }

    public static void HotPotatoExploded()
    {
        HotPotato.HotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
        HotPotato.HotPotatoPlayer.MurderPlayer(HotPotato.HotPotatoPlayer,
                                               MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
        HudManager.Instance.DangerMeter.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead);
    }

    public static void BattleRoyaleShowShoots(byte playerId, int color, float angle)
    {
        new BattleRoyaleShoot(Helpers.PlayerById(playerId), color, angle);
    }

    public static void BattleRoyaleCheckWin(int whichTeamCheck)
    {
        if (BattleRoyale.MatchType == 0)
        {
            var soloPlayersAlives = 0;

            foreach (var soloPlayer in BattleRoyale.SoloPlayerTeam)
            {
                if (!soloPlayer.Data.IsDead)
                    soloPlayersAlives += 1;
            }

            BattleRoyale.BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters))
                                                    .Append("<color=#009F57FF>")
                                                    .Append(soloPlayersAlives)
                                                    .Append("</color>")
                                                    .ToString();

            if (soloPlayersAlives <= 1)
            {
                BattleRoyale.TriggerSoloWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin, false);
            }
        }
        else
        {
            var limePlayersAlive = 0;

            foreach (var limePlayer in BattleRoyale.LimeTeam)
            {
                if (!limePlayer.Data.IsDead)
                    limePlayersAlive += 1;
            }

            var pinkPlayersAlive = 0;

            foreach (var pinkPlayer in BattleRoyale.PinkTeam)
            {
                if (!pinkPlayer.Data.IsDead)
                    pinkPlayersAlive += 1;
            }

            if (whichTeamCheck == 3)
            {
                if (limePlayersAlive <= 0)
                {
                    BattleRoyale.TriggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin,
                                                    false);
                }
                else if (pinkPlayersAlive <= 0)
                {
                    BattleRoyale.TriggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin,
                                                    false);
                }
            }

            if (BattleRoyale.SerialKiller != null)
            {
                var serialKillerAlive = 0;

                foreach (var serialKiller in BattleRoyale.SerialKillerTeam)
                {
                    if (!serialKiller.Data.IsDead)
                        serialKillerAlive += 1;
                }

                BattleRoyale.BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                                        .Append("<color=#39FF14FF>")
                                                        .Append(limePlayersAlive)
                                                        .Append("</color> | ")
                                                        .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                                        .Append(" <color=#F2BEFFFF>")
                                                        .Append(pinkPlayersAlive)
                                                        .Append("</color> | ")
                                                        .Append(Tr.Get(TrKey.BattleRoyaleSerialKiller))
                                                        .Append("<color=#808080FF>")
                                                        .Append(serialKillerAlive)
                                                        .Append("</color>")
                                                        .ToString();
                if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.SerialKiller.Data.IsDead)
                {
                    BattleRoyale.TriggerSerialKillerWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin,
                                                    false);
                }
                else if (pinkPlayersAlive <= 0 && BattleRoyale.SerialKiller.Data.IsDead)
                {
                    BattleRoyale.TriggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin,
                                                    false);
                }
                else if (limePlayersAlive <= 0 && BattleRoyale.SerialKiller.Data.IsDead)
                {
                    BattleRoyale.TriggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin,
                                                    false);
                }
            }
            else
            {
                BattleRoyale.BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                                        .Append("<color=#39FF14FF>")
                                                        .Append(limePlayersAlive)
                                                        .Append("</color> | ")
                                                        .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                                        .Append(" <color=#F2BEFFFF>")
                                                        .Append(pinkPlayersAlive)
                                                        .Append("</color>")
                                                        .ToString();
                if (pinkPlayersAlive <= 0)
                {
                    BattleRoyale.TriggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin,
                                                    false);
                }
                else if (limePlayersAlive <= 0)
                {
                    BattleRoyale.TriggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin,
                                                    false);
                }
            }
        }
    }

    public static void BattleRoyaleScoreCheck(int whichTeamCheck, int multiplier)
    {
        switch (whichTeamCheck)
        {
            case 1:
                BattleRoyale.LimePoints += 10 * multiplier;
                break;
            case 2:
                BattleRoyale.PinkPoints += 10 * multiplier;
                break;
            case 3:
                BattleRoyale.SerialKillerPoints += 10 * multiplier;
                break;
        }

        if (BattleRoyale.SerialKiller != null)
            BattleRoyale.BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal))
                                                    .Append(BattleRoyale.RequiredScore)
                                                    .Append(" | <color=#39FF14FF>")
                                                    .Append(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                                    .Append(BattleRoyale.LimePoints)
                                                    .Append("</color> | <color=#F2BEFFFF>")
                                                    .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                                    .Append(BattleRoyale.PinkPoints)
                                                    .Append("</color> | <color=#808080FF>")
                                                    .Append(Tr.Get(TrKey.BattleRoyaleSerialKillerPoints))
                                                    .Append(BattleRoyale.SerialKillerPoints)
                                                    .Append("</color>")
                                                    .ToString();
        else
            BattleRoyale.BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal))
                                                    .Append(BattleRoyale.RequiredScore)
                                                    .Append(" | <color=#39FF14FF>")
                                                    .Append(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                                    .Append(BattleRoyale.LimePoints)
                                                    .Append("</color> | <color=#F2BEFFFF>")
                                                    .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                                    .Append(BattleRoyale.PinkPoints)
                                                    .Append("</color>")
                                                    .ToString();

        if (BattleRoyale.LimePoints >= BattleRoyale.RequiredScore)
        {
            BattleRoyale.TriggerLimeTeamWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
        }
        else if (BattleRoyale.PinkPoints >= BattleRoyale.RequiredScore)
        {
            BattleRoyale.TriggerPinkTeamWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
        }
        else if (BattleRoyale.SerialKillerPoints >= BattleRoyale.RequiredScore)
        {
            BattleRoyale.TriggerSerialKillerWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
        }
    }
}
