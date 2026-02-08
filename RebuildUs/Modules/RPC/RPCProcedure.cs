namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
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
        byte amount = reader.ReadByte();
        for (int i = 0; i < amount; i++)
        {
            uint id = reader.ReadPackedUInt32();
            uint selection = reader.ReadPackedUInt32();
            if (CustomOption.AllOptionsById.TryGetValue((int)id, out var option))
            {
                option.UpdateSelection((int)selection, option.GetOptionIcon());
            }
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
        var player = Helpers.PlayerById(playerId);
        if (player == null) return;
        Logger.LogInfo($"{player.Data.PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), roleId)}", "setRole");
        player.SetRole((RoleType)roleId);
    }

    public static void AddModifier(byte modId, byte playerId)
    {
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
            x => x.PlayerId == playerId,
            x => x.AddModifier((ModifierType)modId)
        );
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, int clientId, Guid guid)
    {
        GameStart.PlayerVersions[clientId] = new GameStart.PlayerVersion(major, minor, build, revision, guid);
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
        if (AmongUsClient.Instance.AmHost)
        {
            GameStartManager.Instance.ResetStartState();
        }
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
            {
                checkList[playerId] = true;
            }
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
        SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
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
        if (engineer != null)
        {
            engineer.RemainingFixes--;
        }
    }

    public static void ArsonistDouse(byte playerId, byte arsonistId)
    {
        var arsonistPlayer = Helpers.PlayerById(arsonistId);
        if (arsonistPlayer == null) return;
        var arsonist = Arsonist.GetRole(arsonistPlayer);
        if (arsonist == null) return;
        var target = Helpers.PlayerById(playerId);
        if (target != null)
        {
            arsonist.DousedPlayers.Add(target);
        }
    }

    public static void ArsonistWin(byte arsonistId)
    {
        Arsonist.TriggerArsonistWin = true;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null && p.IsAlive() && !p.IsRole(RoleType.Arsonist))
            {
                p.Exiled();
                GameHistory.FinalStatuses[p.PlayerId] = FinalStatus.Torched;
            }
        }
    }

    public static void CleanBody(byte playerId)
    {
        DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
        for (int i = 0; i < array.Length; i++)
        {
            var info = GameData.Instance.GetPlayerById(array[i].ParentId);
            if (info != null && info.PlayerId == playerId)
            {
                UnityEngine.Object.Destroy(array[i].gameObject);
            }
        }
    }

    public static void VultureEat(byte playerId, byte vultureId)
    {
        CleanBody(playerId);
        var vulturePlayer = Helpers.PlayerById(vultureId);
        if (vulturePlayer == null) return;
        var vulture = Vulture.GetRole(vulturePlayer);
        if (vulture != null)
        {
            vulture.EatenBodies++;
        }
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
        {
            jackal?.FakeSidekick = target;
        }
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

        bool isShieldedAndShow = Medic.Shielded == PlayerControl.LocalPlayer && Medic.ShowAttemptToShielded;
        bool isMedicAndShow = PlayerControl.LocalPlayer.IsRole(RoleType.Medic) && Medic.ShowAttemptToMedic;

        if ((isShieldedAndShow || isMedicAndShow) && FastDestroyableSingleton<HudManager>.Instance?.FullScreen != null)
        {
            var c = Palette.ImpostorRed;
            Helpers.ShowFlash(new Color(c.r, c.g, c.b));
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
        if (PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster))
        {
            TimeMaster.ResetTimeMasterButton();
        }
        var hm = FastDestroyableSingleton<HudManager>.Instance;
        hm.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
        hm.FullScreen.enabled = true;
        hm.FullScreen.gameObject.SetActive(true);
        hm.StartCoroutine(Effects.Lerp(TimeMaster.RewindTime / 2, new Action<float>((p) =>
        {
            if (p == 1f) hm.FullScreen.enabled = false;
        })));

        if (!TimeMaster.Exists || PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster)) return; // Time Master himself does not rewind
        if (PlayerControl.LocalPlayer.IsGM()) return; // GM does not rewind

        TimeMaster.IsRewinding = true;

        if (MapBehaviour.Instance)
        {
            MapBehaviour.Instance.Close();
        }
        if (Minigame.Instance)
        {
            Minigame.Instance.ForceClose();
        }
        PlayerControl.LocalPlayer.moveable = false;
    }

    public static void TimeMasterShield()
    {
        TimeMaster.ShieldActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.ShieldDuration, new Action<float>((p) =>
        {
            if (p == 1f) TimeMaster.ShieldActive = false;
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
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, dyingTarget.Data);
            }
            else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);
            }
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
                string msg = string.Format(Tr.Get(TrKey.GuesserGuessChat), roleInfo.Name, guessedTarget.Data.PlayerName);
                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                {
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(killer, msg);
                }
                if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                {
                    FastDestroyableSingleton<Assets.CoreScripts.UnityTelemetry>.Instance.SendWho();
                }
            }
        }
    }

    public static void PlaceJackInTheBox(byte[] buff)
    {
        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new JackInTheBox(position);
    }

    public static void LightsOut()
    {
        Trickster.LightsOutTimer = Trickster.LightsOutDuration;
        // If the local player is impostor indicate lights out
        if (Helpers.HasImpostorVision(PlayerControl.LocalPlayer))
        {
            new CustomMessage("TricksterLightsOutText", Trickster.LightsOutDuration, new(0f, 1.8f), MessageType.Normal);
        }
    }

    public static void EvilHackerCreatesMadmate(byte targetId, byte evilHackerId)
    {
        var targetPlayer = Helpers.PlayerById(targetId);
        var evilHackerPlayer = Helpers.PlayerById(evilHackerId);
        if (targetPlayer == null || evilHackerPlayer == null) return;
        var evilHacker = EvilHacker.GetRole(evilHackerPlayer);
        if (evilHacker == null) return;
        if (!EvilHacker.CanCreateMadmateFromJackal && targetPlayer.IsRole(RoleType.Jackal))
        {
            evilHacker.FakeMadmate = targetPlayer;
        }
        else
        {
            // Jackalバグ対応
            List<PlayerControl> tmpFormerJackals = [.. Jackal.FormerJackals];

            // タスクがないプレイヤーがMadmateになった場合はショートタスクを必要数割り当てる
            if (Helpers.HasFakeTasks(targetPlayer))
            {
                if (CreatedMadmate.HasTasks)
                {
                    Helpers.ClearAllTasks(targetPlayer);
                    Helpers.GenerateAndAssignTasks(targetPlayer, 0, CreatedMadmate.NumTasks, 0);
                }
            }

            FastDestroyableSingleton<RoleManager>.Instance.SetRole(targetPlayer, RoleTypes.Crewmate);
            ErasePlayerRoles(targetPlayer.PlayerId, true, false);

            // Jackalバグ対応
            Jackal.FormerJackals = tmpFormerJackals;

            targetPlayer.AddModifier(ModifierType.CreatedMadmate);
        }
        evilHacker.CanCreateMadmate = false;
        return;
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
        if (player != null)
        {
            Eraser.FutureErased.Add(player);
        }
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
        if (player != null && !player.Data.IsDead)
        {
            Vampire.Bitten = player;
        }
    }

    public static void ShareRealTasks(MessageReader reader)
    {
        byte count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            byte playerId = reader.ReadByte();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            Vector2 pos = new(x, y);

            if (!Map.RealTasks.TryGetValue(playerId, out var list))
            {
                list = new Il2CppSystem.Collections.Generic.List<Vector2>();
                Map.RealTasks[playerId] = list;
            }
            list.Add(pos);
        }
    }
    public static void PolusRandomSpawn(byte playerId, byte locId)
    {
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
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
                Vector2 loc = locId switch
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

        var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        sg.RemainingScrews -= SecurityGuard.CamPrice;
        sg.PlacedCameras++;

        Vector3 position = new(x, y);

        SystemTypes roomType = (SystemTypes)roomId;

        var camera = UnityEngine.Object.Instantiate(referenceCamera);
        camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {sg.PlacedCameras}";
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

        if (PlayerControl.LocalPlayer.PlayerId == sgId)
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }
        MapSettings.CamerasToAdd.Add(camera);
    }

    public static void SealVent(int ventId, byte sgId)
    {
        var player = Helpers.PlayerById(sgId);
        var sg = SecurityGuard.GetRole(player);

        Vent vent = null;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        for (int i = 0; i < allVents.Count; i++)
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
            PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            vent.myRend.sprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            vent.myRend.color = new Color(1f, 1f, 1f, 0.5f);
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
        if (player != null)
        {
            Witch.FutureSpelled.Add(player);
        }
    }

    public static void WitchSpellCast(byte playerId)
    {
        UncheckedExilePlayer(playerId);
        GameHistory.FinalStatuses[playerId] = FinalStatus.Spelled;
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
        PlayerControl player = Helpers.PlayerById(targetId);
        if (player == null || oldShifter == null) return;

        var oldShifterPlayer = oldShifter.Player;
        Shifter.FutureShift = null;

        // Suicide (exile) when impostor or impostor variants
        if (!Shifter.IsNeutral &&
                (player.Data.Role.IsImpostor
                || player.IsNeutral()
                || player.HasModifier(ModifierType.Madmate)
                || player.IsRole(RoleType.Madmate)
                || player.IsRole(RoleType.Suicider)
                || player.HasModifier(ModifierType.CreatedMadmate)))
        {
            oldShifterPlayer.Exiled();
            GameHistory.FinalStatuses[oldShifterPlayer.PlayerId] = FinalStatus.Suicide;
            return;
        }

        if (Shifter.ShiftsModifiers)
        {
            // Switch shield
            if (Medic.Shielded != null && Medic.Shielded == player)
            {
                Medic.Shielded = oldShifterPlayer;
            }
            else if (Medic.Shielded != null && Medic.Shielded == oldShifterPlayer)
            {
                Medic.Shielded = player;
            }

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
        {
            CustomButton.ResetAllCooldowns();
        }
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
            PlayerControl sheriff = Helpers.PlayerById(sheriffId);
            PlayerControl target = Helpers.PlayerById(targetId);
            if (sheriff == null || target == null) return;

            bool misfire = Sheriff.CheckKill(target);

            using var killSender = new RPCSender(sheriff.NetId, CustomRPC.SheriffKill);
            killSender.Write(sheriffId);
            killSender.Write(targetId);
            killSender.Write(misfire);

            SheriffKill(sheriffId, targetId, misfire);
        }
    }

    public static void SheriffKill(byte sheriffId, byte targetId, bool misfire)
    {
        PlayerControl sheriff = Helpers.PlayerById(sheriffId);
        PlayerControl target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null) return;

        Sheriff role = Sheriff.GetRole(sheriff);
        if (role != null)
        {
            role.NumShots--;
        }

        if (misfire)
        {
            sheriff.MurderPlayer(sheriff);
            GameHistory.FinalStatuses[sheriffId] = FinalStatus.Misfire;

            if (!Sheriff.MisfireKillsTarget) return;
            GameHistory.FinalStatuses[targetId] = FinalStatus.Misfire;
        }

        sheriff.MurderPlayer(target);
    }

    public static void gamemodeKills(byte targetId, byte sourceId)
    {
        PlayerControl killer = Helpers.PlayerById(sourceId);
        PlayerControl murdered = Helpers.PlayerById(targetId);

        switch (MapSettings.GameMode)
        {
            // CTF
            case CustomGameMode.CaptureTheFlag:
                if (CaptureTheFlag.stealerPlayer != null && sourceId == CaptureTheFlag.stealerPlayer.PlayerId)
                {
                    if (CaptureTheFlag.redPlayerWhoHasBlueFlag != null && murdered.PlayerId == CaptureTheFlag.redPlayerWhoHasBlueFlag.PlayerId)
                    {
                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(CaptureTheFlag.stealerPlayer.PlayerId));
                        if (CaptureTheFlag.redplayer01 != null && CaptureTheFlag.redplayer01 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer01);
                            CaptureTheFlag.redplayer01 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer01.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer01.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer02 != null && CaptureTheFlag.redplayer02 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer02);
                            CaptureTheFlag.redplayer02 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer02.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer02.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer03 != null && CaptureTheFlag.redplayer03 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer03);
                            CaptureTheFlag.redplayer03 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer03.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer03.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer04 != null && CaptureTheFlag.redplayer04 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer04);
                            CaptureTheFlag.redplayer04 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer04.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer04.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer05 != null && CaptureTheFlag.redplayer05 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer05);
                            CaptureTheFlag.redplayer05 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer05.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer05.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer06 != null && CaptureTheFlag.redplayer06 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer06);
                            CaptureTheFlag.redplayer06 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer06.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer06.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.redplayer07 != null && CaptureTheFlag.redplayer07 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            CaptureTheFlag.redteamFlag.Remove(CaptureTheFlag.redplayer07);
                            CaptureTheFlag.redplayer07 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.redteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.redplayer07.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.redplayer07.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                    }
                    else if (CaptureTheFlag.bluePlayerWhoHasRedFlag != null && murdered.PlayerId == CaptureTheFlag.bluePlayerWhoHasRedFlag.PlayerId)
                    {
                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(CaptureTheFlag.stealerPlayer.PlayerId));
                        if (CaptureTheFlag.blueplayer01 != null && CaptureTheFlag.blueplayer01 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer01);
                            CaptureTheFlag.blueplayer01 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer01.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer01.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer02 != null && CaptureTheFlag.blueplayer02 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer02);
                            CaptureTheFlag.blueplayer02 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer02.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer02.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer03 != null && CaptureTheFlag.blueplayer03 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer03);
                            CaptureTheFlag.blueplayer03 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer03.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer03.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer04 != null && CaptureTheFlag.blueplayer04 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer04);
                            CaptureTheFlag.blueplayer04 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer04.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer04.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer05 != null && CaptureTheFlag.blueplayer05 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer05);
                            CaptureTheFlag.blueplayer05 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer05.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer05.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer06 != null && CaptureTheFlag.blueplayer06 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer06);
                            CaptureTheFlag.blueplayer06 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer06.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer06.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                        else if (CaptureTheFlag.blueplayer07 != null && CaptureTheFlag.blueplayer07 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            CaptureTheFlag.blueteamFlag.Remove(CaptureTheFlag.blueplayer07);
                            CaptureTheFlag.blueplayer07 = CaptureTheFlag.stealerPlayer;
                            CaptureTheFlag.blueteamFlag.Add(CaptureTheFlag.stealerPlayer);
                            CaptureTheFlag.stealerPlayer = murdered;
                            CaptureTheFlag.blueplayer07.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                            CaptureTheFlag.blueplayer07.MurderPlayer(CaptureTheFlag.stealerPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        }
                    }
                    else
                    {
                        CaptureTheFlag.stealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                        killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                        CaptureTheFlag.stealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    }
                }
                else
                {
                    killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                }
                break;
            // PAT
            case CustomGameMode.PoliceAndThieves:
                killer.MurderPlayer(murdered, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
                break;
            // BR
            case CustomGameMode.BattleRoyale:
                // Hit sound
                if (murdered == PlayerControl.LocalPlayer)
                {
                    SoundManager.Instance.PlaySound(AssetLoader.royaleGetHit, false, 100f);
                }

                if (BattleRoyale.matchType == 0)
                {
                    new BattleRoyaleFootprint(murdered, 0);

                    // Remove 1 life and check remaining lifes
                    if (BattleRoyale.soloPlayer01 != null && murdered == BattleRoyale.soloPlayer01)
                    {
                        BattleRoyale.soloPlayer01Lifes -= 1;
                        if (BattleRoyale.soloPlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer02 != null && murdered == BattleRoyale.soloPlayer02)
                    {
                        BattleRoyale.soloPlayer02Lifes -= 1;
                        if (BattleRoyale.soloPlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer03 != null && murdered == BattleRoyale.soloPlayer03)
                    {
                        BattleRoyale.soloPlayer03Lifes -= 1;
                        if (BattleRoyale.soloPlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer04 != null && murdered == BattleRoyale.soloPlayer04)
                    {
                        BattleRoyale.soloPlayer04Lifes -= 1;
                        if (BattleRoyale.soloPlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer05 != null && murdered == BattleRoyale.soloPlayer05)
                    {
                        BattleRoyale.soloPlayer05Lifes -= 1;
                        if (BattleRoyale.soloPlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer06 != null && murdered == BattleRoyale.soloPlayer06)
                    {
                        BattleRoyale.soloPlayer06Lifes -= 1;
                        if (BattleRoyale.soloPlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer07 != null && murdered == BattleRoyale.soloPlayer07)
                    {
                        BattleRoyale.soloPlayer07Lifes -= 1;
                        if (BattleRoyale.soloPlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer08 != null && murdered == BattleRoyale.soloPlayer08)
                    {
                        BattleRoyale.soloPlayer08Lifes -= 1;
                        if (BattleRoyale.soloPlayer08Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer09 != null && murdered == BattleRoyale.soloPlayer09)
                    {
                        BattleRoyale.soloPlayer09Lifes -= 1;
                        if (BattleRoyale.soloPlayer09Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer10 != null && murdered == BattleRoyale.soloPlayer10)
                    {
                        BattleRoyale.soloPlayer10Lifes -= 1;
                        if (BattleRoyale.soloPlayer10Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer11 != null && murdered == BattleRoyale.soloPlayer11)
                    {
                        BattleRoyale.soloPlayer11Lifes -= 1;
                        if (BattleRoyale.soloPlayer11Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer12 != null && murdered == BattleRoyale.soloPlayer12)
                    {
                        BattleRoyale.soloPlayer12Lifes -= 1;
                        if (BattleRoyale.soloPlayer12Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer13 != null && murdered == BattleRoyale.soloPlayer13)
                    {
                        BattleRoyale.soloPlayer13Lifes -= 1;
                        if (BattleRoyale.soloPlayer13Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer14 != null && murdered == BattleRoyale.soloPlayer14)
                    {
                        BattleRoyale.soloPlayer14Lifes -= 1;
                        if (BattleRoyale.soloPlayer14Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                    else if (BattleRoyale.soloPlayer15 != null && murdered == BattleRoyale.soloPlayer15)
                    {
                        BattleRoyale.soloPlayer15Lifes -= 1;
                        if (BattleRoyale.soloPlayer15Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            battleRoyaleCheckWin(0);
                            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(targetId));
                        }
                    }
                }
                else
                {
                    if (BattleRoyale.serialKiller != null && killer == BattleRoyale.serialKiller)
                    {
                        BattleRoyale.serialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                    }
                    // Remove 1 life and check remaining lifes
                    if (BattleRoyale.limePlayer01 != null && murdered == BattleRoyale.limePlayer01)
                    {
                        BattleRoyale.limePlayer01Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer02 != null && murdered == BattleRoyale.limePlayer02)
                    {
                        BattleRoyale.limePlayer02Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer03 != null && murdered == BattleRoyale.limePlayer03)
                    {
                        BattleRoyale.limePlayer03Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer04 != null && murdered == BattleRoyale.limePlayer04)
                    {
                        BattleRoyale.limePlayer04Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer05 != null && murdered == BattleRoyale.limePlayer05)
                    {
                        BattleRoyale.limePlayer05Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer06 != null && murdered == BattleRoyale.limePlayer06)
                    {
                        BattleRoyale.limePlayer06Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.limePlayer07 != null && murdered == BattleRoyale.limePlayer07)
                    {
                        BattleRoyale.limePlayer07Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 1);
                        if (BattleRoyale.limePlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(1);
                                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 1);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer01 != null && murdered == BattleRoyale.pinkPlayer01)
                    {
                        BattleRoyale.pinkPlayer01Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer01Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer02 != null && murdered == BattleRoyale.pinkPlayer02)
                    {
                        BattleRoyale.pinkPlayer02Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer02Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer03 != null && murdered == BattleRoyale.pinkPlayer03)
                    {
                        BattleRoyale.pinkPlayer03Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer03Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer04 != null && murdered == BattleRoyale.pinkPlayer04)
                    {
                        BattleRoyale.pinkPlayer04Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer04Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer05 != null && murdered == BattleRoyale.pinkPlayer05)
                    {
                        BattleRoyale.pinkPlayer05Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer05Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer06 != null && murdered == BattleRoyale.pinkPlayer06)
                    {
                        BattleRoyale.pinkPlayer06Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer06Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.pinkPlayer07 != null && murdered == BattleRoyale.pinkPlayer07)
                    {
                        BattleRoyale.pinkPlayer07Lifes -= 1;
                        new BattleRoyaleFootprint(murdered, 2);
                        if (BattleRoyale.pinkPlayer07Lifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(2);
                                    Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.serialKiller != null && sourceId == BattleRoyale.serialKiller.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(3, 1);
                                        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(1, 1);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    else if (BattleRoyale.serialKiller != null && murdered == BattleRoyale.serialKiller)
                    {
                        BattleRoyale.serialKillerLifes -= 1;
                        new BattleRoyaleFootprint(murdered, 3);
                        if (BattleRoyale.serialKillerLifes <= 0)
                        {
                            UncheckedMurderPlayer(sourceId, targetId, 0);
                            switch (BattleRoyale.matchType)
                            {
                                case 1:
                                    battleRoyaleCheckWin(3);
                                    Helpers.showGamemodesPopUp(3, Helpers.PlayerById(targetId));
                                    break;
                                case 2:
                                    if (BattleRoyale.limePlayer01 != null && sourceId == BattleRoyale.limePlayer01.PlayerId || BattleRoyale.limePlayer02 != null && sourceId == BattleRoyale.limePlayer02.PlayerId || BattleRoyale.limePlayer03 != null && sourceId == BattleRoyale.limePlayer03.PlayerId || BattleRoyale.limePlayer04 != null && sourceId == BattleRoyale.limePlayer04.PlayerId || BattleRoyale.limePlayer05 != null && sourceId == BattleRoyale.limePlayer05.PlayerId || BattleRoyale.limePlayer06 != null && sourceId == BattleRoyale.limePlayer06.PlayerId || BattleRoyale.limePlayer07 != null && sourceId == BattleRoyale.limePlayer07.PlayerId)
                                    {
                                        battleRoyaleScoreCheck(1, 3);
                                        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(targetId));
                                    }
                                    else
                                    {
                                        battleRoyaleScoreCheck(2, 3);
                                        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(targetId));
                                    }
                                    break;
                            }
                        }
                    }
                    if (BattleRoyale.serialKiller != null && killer == BattleRoyale.serialKiller)
                    {
                        BattleRoyale.serialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    }
                }
                break;
        }
    }
    public static void captureTheFlagWhoTookTheFlag(byte playerWhoStoleTheFlag, int redorblue)
    {
        PlayerControl playerWhoGotTheFlag = Helpers.PlayerById(playerWhoStoleTheFlag);

        // Red team steal blue flag
        if (redorblue == 1)
        {
            CaptureTheFlag.blueflagtaken = true;
            CaptureTheFlag.redPlayerWhoHasBlueFlag = playerWhoGotTheFlag;
            CaptureTheFlag.blueflag.transform.parent = playerWhoGotTheFlag.transform;
            CaptureTheFlag.blueflag.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            Helpers.showGamemodesPopUp(3, Helpers.PlayerById(CaptureTheFlag.redPlayerWhoHasBlueFlag.PlayerId));
        }

        // Blue team steal red flag
        if (redorblue == 2)
        {
            CaptureTheFlag.redflagtaken = true;
            CaptureTheFlag.bluePlayerWhoHasRedFlag = playerWhoGotTheFlag;
            CaptureTheFlag.redflag.transform.parent = playerWhoGotTheFlag.transform;
            CaptureTheFlag.redflag.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            Helpers.showGamemodesPopUp(4, Helpers.PlayerById(CaptureTheFlag.bluePlayerWhoHasRedFlag.PlayerId));
        }
    }

    public static void captureTheFlagWhichTeamScored(int whichteam)
    {
        // Red team
        if (whichteam == 1)
        {
            Helpers.showGamemodesPopUp(5, Helpers.PlayerById(CaptureTheFlag.redPlayerWhoHasBlueFlag.PlayerId));
            CaptureTheFlag.blueflagtaken = false;
            CaptureTheFlag.redPlayerWhoHasBlueFlag = null;
            CaptureTheFlag.blueflag.transform.parent = CaptureTheFlag.blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(7.7f, -1.15f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(16.5f, -4.65f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(12.5f, -31.45f, -0.011f);
                    break;
            }
            CaptureTheFlag.currentRedTeamPoints += 1;
            CaptureTheFlag.flagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(CaptureTheFlag.currentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(CaptureTheFlag.currentBlueTeamPoints).Append("</color>").ToString();
            if (CaptureTheFlag.currentRedTeamPoints >= CaptureTheFlag.requiredFlags)
            {
                CaptureTheFlag.triggerRedTeamWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RedTeamFlagWin, false);
            }
        }

        // Blue team
        if (whichteam == 2)
        {
            Helpers.showGamemodesPopUp(6, Helpers.PlayerById(CaptureTheFlag.bluePlayerWhoHasRedFlag.PlayerId));
            CaptureTheFlag.redflagtaken = false;
            CaptureTheFlag.bluePlayerWhoHasRedFlag = null;
            CaptureTheFlag.redflag.transform.parent = CaptureTheFlag.redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.35f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-20.5f, -5.35f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.redflag.transform.position = new Vector3(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.redflag.transform.position = new Vector3(36.4f, -21.7f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-8.35f, 28.05f, -0.011f);
                    break;
            }
            CaptureTheFlag.currentBlueTeamPoints += 1;
            CaptureTheFlag.flagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(CaptureTheFlag.currentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(CaptureTheFlag.currentBlueTeamPoints).Append("</color>").ToString(); ;
            if (CaptureTheFlag.currentBlueTeamPoints >= CaptureTheFlag.requiredFlags)
            {
                CaptureTheFlag.triggerBlueTeamWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BlueTeamFlagWin, false);
            }
        }
    }

    public static void policeandThiefJail(byte thiefId)
    {
        PlayerControl capturedThief = Helpers.PlayerById(thiefId);

        if (capturedThief.inVent)
        {
            foreach (Vent vent in ShipStatus.Instance.AllVents)
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
        {
            SoundManager.Instance.PlaySound(AssetLoader.JailSound, false, 100f);
        }
        PoliceAndThief.thiefArrested.Add(capturedThief);
        if (PoliceAndThief.thiefplayer01 != null && thiefId == PoliceAndThief.thiefplayer01.PlayerId)
        {
            if (PoliceAndThief.thiefplayer01IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer01JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer02 != null && thiefId == PoliceAndThief.thiefplayer02.PlayerId)
        {
            if (PoliceAndThief.thiefplayer02IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer02JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer03 != null && thiefId == PoliceAndThief.thiefplayer03.PlayerId)
        {
            if (PoliceAndThief.thiefplayer03IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer03JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer04 != null && thiefId == PoliceAndThief.thiefplayer04.PlayerId)
        {
            if (PoliceAndThief.thiefplayer04IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer04JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer05 != null && thiefId == PoliceAndThief.thiefplayer05.PlayerId)
        {
            if (PoliceAndThief.thiefplayer05IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer05JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer06 != null && thiefId == PoliceAndThief.thiefplayer06.PlayerId)
        {
            if (PoliceAndThief.thiefplayer06IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer06JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer07 != null && thiefId == PoliceAndThief.thiefplayer07.PlayerId)
        {
            if (PoliceAndThief.thiefplayer07IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer07JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer08 != null && thiefId == PoliceAndThief.thiefplayer08.PlayerId)
        {
            if (PoliceAndThief.thiefplayer08IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer08JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer09 != null && thiefId == PoliceAndThief.thiefplayer09.PlayerId)
        {
            if (PoliceAndThief.thiefplayer09IsStealing)
            {
                policeandThiefRevertedJewelPosition(thiefId, PoliceAndThief.thiefplayer09JewelId);
            }
        }
        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    capturedThief.transform.position = new Vector3(-12f, 7.15f, capturedThief.transform.position.z);
                }
                else if (RebuildUs.activatedDleks)
                {
                    capturedThief.transform.position = new Vector3(10.2f, 3.6f, capturedThief.transform.position.z);
                }
                else
                {
                    capturedThief.transform.position = new Vector3(-10.2f, 3.6f, capturedThief.transform.position.z);
                }
                break;
            // MiraHQ
            case 1:
                capturedThief.transform.position = new Vector3(1.8f, 1.25f, capturedThief.transform.position.z);
                break;
            // Polus
            case 2:
                capturedThief.transform.position = new Vector3(8.25f, -5f, capturedThief.transform.position.z);
                break;
            // Dleks
            case 3:
                capturedThief.transform.position = new Vector3(10.2f, 3.6f, capturedThief.transform.position.z);
                break;
            // Airship
            case 4:
                capturedThief.transform.position = new Vector3(-18.5f, 3.5f, capturedThief.transform.position.z);
                break;
            // Fungle
            case 5:
                capturedThief.transform.position = new Vector3(-26.75f, -0.5f, capturedThief.transform.position.z);
                break;
            // Submerged
            case 6:
                if (capturedThief.transform.position.y > 0)
                {
                    capturedThief.transform.position = new Vector3(-6f, 32f, capturedThief.transform.position.z);
                }
                else
                {
                    capturedThief.transform.position = new Vector3(-14.1f, -39f, capturedThief.transform.position.z);
                }
                break;
        }
        PoliceAndThief.currentThiefsCaptured += 1;
        Helpers.showGamemodesPopUp(1, Helpers.PlayerById(capturedThief.PlayerId));
        PoliceAndThief.thiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>").Append(PoliceAndThief.currentJewelsStoled).Append(" / ").Append(PoliceAndThief.requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(PoliceAndThief.currentThiefsCaptured).Append(" / ").Append(PoliceAndThief.thiefTeam.Count).Append("</color>").ToString();
        if (PoliceAndThief.currentThiefsCaptured == PoliceAndThief.thiefTeam.Count)
        {
            PoliceAndThief.triggerPoliceWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
        }
    }

    public static void policeandThiefFreeThief()
    {
        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    PoliceAndThief.thiefArrested[0].transform.position = new Vector3(13.75f, -0.2f, PoliceAndThief.thiefArrested[0].transform.position.z);
                }
                else if (RebuildUs.activatedDleks)
                {
                    PoliceAndThief.thiefArrested[0].transform.position = new Vector3(1.31f, -16.25f, PoliceAndThief.thiefArrested[0].transform.position.z);
                }
                else
                {
                    PoliceAndThief.thiefArrested[0].transform.position = new Vector3(-1.31f, -16.25f, PoliceAndThief.thiefArrested[0].transform.position.z);
                }
                break;
            // MiraHQ
            case 1:
                PoliceAndThief.thiefArrested[0].transform.position = new Vector3(17.75f, 11.5f, PoliceAndThief.thiefArrested[0].transform.position.z);
                break;
            // Polus
            case 2:
                PoliceAndThief.thiefArrested[0].transform.position = new Vector3(30f, -15.75f, PoliceAndThief.thiefArrested[0].transform.position.z);
                break;
            // Dleks
            case 3:
                PoliceAndThief.thiefArrested[0].transform.position = new Vector3(1.31f, -16.25f, PoliceAndThief.thiefArrested[0].transform.position.z);
                break;
            // Airship
            case 4:
                PoliceAndThief.thiefArrested[0].transform.position = new Vector3(7.15f, -14.5f, PoliceAndThief.thiefArrested[0].transform.position.z);
                break;
            // Fungle
            case 5:
                PoliceAndThief.thiefArrested[0].transform.position = new Vector3(20f, 11f, PoliceAndThief.thiefArrested[0].transform.position.z);
                break;
            // Submerged
            case 6:
                if (PoliceAndThief.thiefArrested[0].transform.position.y > 0)
                {
                    PoliceAndThief.thiefArrested[0].transform.position = new Vector3(1f, 10f, PoliceAndThief.thiefArrested[0].transform.position.z);
                }
                else
                {
                    PoliceAndThief.thiefArrested[0].transform.position = new Vector3(12.5f, -31.75f, PoliceAndThief.thiefArrested[0].transform.position.z);
                }
                break;
        }
        Helpers.showGamemodesPopUp(2, Helpers.PlayerById(PoliceAndThief.thiefArrested[0].PlayerId));
        PoliceAndThief.thiefArrested.RemoveAt(0);
        PoliceAndThief.currentThiefsCaptured = PoliceAndThief.currentThiefsCaptured - 1;
        PoliceAndThief.thiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>").Append(PoliceAndThief.currentJewelsStoled).Append(" / ").Append(PoliceAndThief.requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(PoliceAndThief.currentThiefsCaptured).Append(" / ").Append(PoliceAndThief.thiefTeam.Count).Append("</color>").ToString(); ;
    }

    public static void policeandThiefTakeJewel(byte thiefWhoTookATreasure, byte jewelId)
    {
        PlayerControl thiefTookJewel = Helpers.PlayerById(thiefWhoTookATreasure);

        if (PoliceAndThief.thiefplayer01 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer01.PlayerId)
        {
            PoliceAndThief.thiefplayer01IsStealing = true;
            PoliceAndThief.thiefplayer01JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer02 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer02.PlayerId)
        {
            PoliceAndThief.thiefplayer02IsStealing = true;
            PoliceAndThief.thiefplayer02JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer03 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer03.PlayerId)
        {
            PoliceAndThief.thiefplayer03IsStealing = true;
            PoliceAndThief.thiefplayer03JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer04 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer04.PlayerId)
        {
            PoliceAndThief.thiefplayer04IsStealing = true;
            PoliceAndThief.thiefplayer04JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer05 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer05.PlayerId)
        {
            PoliceAndThief.thiefplayer05IsStealing = true;
            PoliceAndThief.thiefplayer05JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer06 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer06.PlayerId)
        {
            PoliceAndThief.thiefplayer06IsStealing = true;
            PoliceAndThief.thiefplayer06JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer07 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer07.PlayerId)
        {
            PoliceAndThief.thiefplayer07IsStealing = true;
            PoliceAndThief.thiefplayer07JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer08 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer08.PlayerId)
        {
            PoliceAndThief.thiefplayer08IsStealing = true;
            PoliceAndThief.thiefplayer08JewelId = jewelId;
        }
        else if (PoliceAndThief.thiefplayer09 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer09.PlayerId)
        {
            PoliceAndThief.thiefplayer09IsStealing = true;
            PoliceAndThief.thiefplayer09JewelId = jewelId;
        }
        switch (jewelId)
        {
            case 1:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel01);
                PoliceAndThief.jewel01BeingStealed = thiefTookJewel;
                break;
            case 2:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel02);
                PoliceAndThief.jewel02BeingStealed = thiefTookJewel;
                break;
            case 3:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel03);
                PoliceAndThief.jewel03BeingStealed = thiefTookJewel;
                break;
            case 4:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel04);
                PoliceAndThief.jewel04BeingStealed = thiefTookJewel;
                break;
            case 5:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel05);
                PoliceAndThief.jewel05BeingStealed = thiefTookJewel;
                break;
            case 6:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel06);
                PoliceAndThief.jewel06BeingStealed = thiefTookJewel;
                break;
            case 7:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel07);
                PoliceAndThief.jewel07BeingStealed = thiefTookJewel;
                break;
            case 8:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel08);
                PoliceAndThief.jewel08BeingStealed = thiefTookJewel;
                break;
            case 9:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel09);
                PoliceAndThief.jewel09BeingStealed = thiefTookJewel;
                break;
            case 10:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel10);
                PoliceAndThief.jewel10BeingStealed = thiefTookJewel;
                break;
            case 11:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel11);
                PoliceAndThief.jewel11BeingStealed = thiefTookJewel;
                break;
            case 12:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel12);
                PoliceAndThief.jewel12BeingStealed = thiefTookJewel;
                break;
            case 13:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel13);
                PoliceAndThief.jewel13BeingStealed = thiefTookJewel;
                break;
            case 14:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel14);
                PoliceAndThief.jewel14BeingStealed = thiefTookJewel;
                break;
            case 15:
                thiefCarryJewel(thiefTookJewel, PoliceAndThief.jewel15);
                PoliceAndThief.jewel15BeingStealed = thiefTookJewel;
                break;
        }
    }

    public static void thiefCarryJewel(PlayerControl thief, GameObject jewel)
    {
        jewel.SetActive(true);
        jewel.transform.parent = thief.transform;
        jewel.transform.localPosition = new Vector3(0f, 0.7f, -0.1f);
    }

    public static void policeandThiefDeliverJewel(byte thiefWhoTookATreasure, byte jewelId)
    {
        PlayerControl thiefDeliverJewel = Helpers.PlayerById(thiefWhoTookATreasure);

        // Thief player steal a jewel
        if (PoliceAndThief.thiefplayer01 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer01.PlayerId)
        {
            PoliceAndThief.thiefplayer01IsStealing = false;
            PoliceAndThief.thiefplayer01JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer02 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer02.PlayerId)
        {
            PoliceAndThief.thiefplayer02IsStealing = false;
            PoliceAndThief.thiefplayer02JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer03 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer03.PlayerId)
        {
            PoliceAndThief.thiefplayer03IsStealing = false;
            PoliceAndThief.thiefplayer03JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer04 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer04.PlayerId)
        {
            PoliceAndThief.thiefplayer04IsStealing = false;
            PoliceAndThief.thiefplayer04JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer05 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer05.PlayerId)
        {
            PoliceAndThief.thiefplayer05IsStealing = false;
            PoliceAndThief.thiefplayer05JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer06 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer06.PlayerId)
        {
            PoliceAndThief.thiefplayer06IsStealing = false;
            PoliceAndThief.thiefplayer06JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer07 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer07.PlayerId)
        {
            PoliceAndThief.thiefplayer07IsStealing = false;
            PoliceAndThief.thiefplayer07JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer08 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer08.PlayerId)
        {
            PoliceAndThief.thiefplayer08IsStealing = false;
            PoliceAndThief.thiefplayer08JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer09 != null && thiefWhoTookATreasure == PoliceAndThief.thiefplayer09.PlayerId)
        {
            PoliceAndThief.thiefplayer09IsStealing = false;
            PoliceAndThief.thiefplayer09JewelId = 0;
        }
        GameObject myJewel = null;
        bool isDiamond = true;
        switch (jewelId)
        {
            case 1:
                myJewel = PoliceAndThief.jewel01;
                PoliceAndThief.jewel01BeingStealed = null;
                break;
            case 2:
                myJewel = PoliceAndThief.jewel02;
                PoliceAndThief.jewel02BeingStealed = null;
                break;
            case 3:
                myJewel = PoliceAndThief.jewel03;
                PoliceAndThief.jewel03BeingStealed = null;
                break;
            case 4:
                myJewel = PoliceAndThief.jewel04;
                PoliceAndThief.jewel04BeingStealed = null;
                break;
            case 5:
                myJewel = PoliceAndThief.jewel05;
                PoliceAndThief.jewel05BeingStealed = null;
                break;
            case 6:
                myJewel = PoliceAndThief.jewel06;
                PoliceAndThief.jewel06BeingStealed = null;
                break;
            case 7:
                myJewel = PoliceAndThief.jewel07;
                PoliceAndThief.jewel07BeingStealed = null;
                break;
            case 8:
                myJewel = PoliceAndThief.jewel08;
                PoliceAndThief.jewel08BeingStealed = null;
                break;
            case 9:
                myJewel = PoliceAndThief.jewel09;
                PoliceAndThief.jewel09BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 10:
                myJewel = PoliceAndThief.jewel10;
                PoliceAndThief.jewel10BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 11:
                myJewel = PoliceAndThief.jewel11;
                PoliceAndThief.jewel11BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 12:
                myJewel = PoliceAndThief.jewel12;
                PoliceAndThief.jewel12BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 13:
                myJewel = PoliceAndThief.jewel13;
                PoliceAndThief.jewel13BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 14:
                myJewel = PoliceAndThief.jewel14;
                PoliceAndThief.jewel14BeingStealed = null;
                isDiamond = !isDiamond;
                break;
            case 15:
                myJewel = PoliceAndThief.jewel15;
                PoliceAndThief.jewel15BeingStealed = null;
                isDiamond = !isDiamond;
                break;
        }
        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    if (isDiamond)
                    {
                        myJewel.transform.position = new Vector3(15.25f, -0.33f, thiefDeliverJewel.transform.position.z);
                    }
                    else
                    {
                        myJewel.transform.position = new Vector3(15.7f, -0.33f, thiefDeliverJewel.transform.position.z);
                    }
                }
                else if (RebuildUs.activatedDleks)
                {
                    if (isDiamond)
                    {
                        myJewel.transform.position = new Vector3(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                    }
                    else
                    {
                        myJewel.transform.position = new Vector3(-0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                    }
                }
                else
                {
                    if (isDiamond)
                    {
                        myJewel.transform.position = new Vector3(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                    }
                    else
                    {
                        myJewel.transform.position = new Vector3(0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                    }
                }
                break;
            // MiraHQ
            case 1:
                if (isDiamond)
                {
                    myJewel.transform.position = new Vector3(19.65f, 13.9f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    myJewel.transform.position = new Vector3(20.075f, 13.9f, thiefDeliverJewel.transform.position.z);
                }
                break;
            // Polus
            case 2:
                if (isDiamond)
                {
                    myJewel.transform.position = new Vector3(33.6f, 13.9f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    myJewel.transform.position = new Vector3(34.05f, -15.9f, thiefDeliverJewel.transform.position.z);
                }
                break;
            // Dleks
            case 3:
                if (isDiamond)
                {
                    myJewel.transform.position = new Vector3(0f, -19.4f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    myJewel.transform.position = new Vector3(-0.4f, -19.4f, thiefDeliverJewel.transform.position.z);
                }
                break;
            // Airship
            case 4:
                if (isDiamond)
                {
                    myJewel.transform.position = new Vector3(11.75f, -16.35f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    myJewel.transform.position = new Vector3(12.2f, -16.35f, thiefDeliverJewel.transform.position.z);
                }
                break;
            // Fungle
            case 5:
                if (isDiamond)
                {
                    myJewel.transform.position = new Vector3(17.25f, 8.95f, thiefDeliverJewel.transform.position.z);
                }
                else
                {
                    myJewel.transform.position = new Vector3(17.7f, 8.95f, thiefDeliverJewel.transform.position.z);
                }
                break;
            // Submerged
            case 6:
                if (isDiamond)
                {
                    if (myJewel.transform.position.y > 0)
                    {
                        myJewel.transform.position = new Vector3(-1.4f, 8.65f, thiefDeliverJewel.transform.position.z);
                    }
                    else
                    {
                        myJewel.transform.position = new Vector3(12.7f, -35.35f, thiefDeliverJewel.transform.position.z);
                    }
                }
                else
                {
                    if (myJewel.transform.position.y > 0)
                    {
                        myJewel.transform.position = new Vector3(-1f, 8.65f, thiefDeliverJewel.transform.position.z);
                    }
                    else
                    {
                        myJewel.transform.position = new Vector3(13.1f, -35.35f, thiefDeliverJewel.transform.position.z);
                    }
                }
                break;
        }
        myJewel.transform.SetParent(null);
        PoliceAndThief.currentJewelsStoled += 1;
        Helpers.showGamemodesPopUp(3, Helpers.PlayerById(thiefDeliverJewel.PlayerId));
        PoliceAndThief.thiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>").Append(PoliceAndThief.currentJewelsStoled).Append(" / ").Append(PoliceAndThief.requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(PoliceAndThief.currentThiefsCaptured).Append(" / ").Append(PoliceAndThief.thiefTeam.Count).Append("</color>").ToString();
        if (PoliceAndThief.currentJewelsStoled >= PoliceAndThief.requiredJewels)
        {
            PoliceAndThief.triggerThiefWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModeThiefWin, false);
        }
    }

    public static void policeandThiefRevertedJewelPosition(byte thiefWhoLostJewel, byte jewelRevertedId)
    {

        if (PoliceAndThief.thiefplayer01 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer01.PlayerId)
        {
            PoliceAndThief.thiefplayer01IsStealing = false;
            PoliceAndThief.thiefplayer01JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer02 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer02.PlayerId)
        {
            PoliceAndThief.thiefplayer02IsStealing = false;
            PoliceAndThief.thiefplayer02JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer03 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer03.PlayerId)
        {
            PoliceAndThief.thiefplayer03IsStealing = false;
            PoliceAndThief.thiefplayer03JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer04 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer04.PlayerId)
        {
            PoliceAndThief.thiefplayer04IsStealing = false;
            PoliceAndThief.thiefplayer04JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer05 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer05.PlayerId)
        {
            PoliceAndThief.thiefplayer05IsStealing = false;
            PoliceAndThief.thiefplayer05JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer06 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer06.PlayerId)
        {
            PoliceAndThief.thiefplayer06IsStealing = false;
            PoliceAndThief.thiefplayer06JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer07 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer07.PlayerId)
        {
            PoliceAndThief.thiefplayer07IsStealing = false;
            PoliceAndThief.thiefplayer07JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer08 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer08.PlayerId)
        {
            PoliceAndThief.thiefplayer08IsStealing = false;
            PoliceAndThief.thiefplayer08JewelId = 0;
        }
        else if (PoliceAndThief.thiefplayer09 != null && thiefWhoLostJewel == PoliceAndThief.thiefplayer09.PlayerId)
        {
            PoliceAndThief.thiefplayer09IsStealing = false;
            PoliceAndThief.thiefplayer09JewelId = 0;
        }
        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.jewel01.transform.SetParent(null);
                            PoliceAndThief.jewel01.transform.position = new Vector3(6.95f, 4.95f, 1f);
                            PoliceAndThief.jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.jewel02.transform.SetParent(null);
                            PoliceAndThief.jewel02.transform.position = new Vector3(-3.75f, 5.35f, 1f);
                            PoliceAndThief.jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.jewel03.transform.SetParent(null);
                            PoliceAndThief.jewel03.transform.position = new Vector3(-7.7f, 11.3f, 1f);
                            PoliceAndThief.jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.jewel04.transform.SetParent(null);
                            PoliceAndThief.jewel04.transform.position = new Vector3(-19.65f, 5.3f, 1f);
                            PoliceAndThief.jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.jewel05.transform.SetParent(null);
                            PoliceAndThief.jewel05.transform.position = new Vector3(-19.65f, -8, 1f);
                            PoliceAndThief.jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.jewel06.transform.SetParent(null);
                            PoliceAndThief.jewel06.transform.position = new Vector3(-5.45f, -13f, 1f);
                            PoliceAndThief.jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.jewel07.transform.SetParent(null);
                            PoliceAndThief.jewel07.transform.position = new Vector3(-7.65f, -4.2f, 1f);
                            PoliceAndThief.jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.jewel08.transform.SetParent(null);
                            PoliceAndThief.jewel08.transform.position = new Vector3(2f, -6.75f, 1f);
                            PoliceAndThief.jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.jewel09.transform.SetParent(null);
                            PoliceAndThief.jewel09.transform.position = new Vector3(8.9f, 1.45f, 1f);
                            PoliceAndThief.jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.jewel10.transform.SetParent(null);
                            PoliceAndThief.jewel10.transform.position = new Vector3(4.6f, -2.25f, 1f);
                            PoliceAndThief.jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.jewel11.transform.SetParent(null);
                            PoliceAndThief.jewel11.transform.position = new Vector3(-5.05f, -0.88f, 1f);
                            PoliceAndThief.jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.jewel12.transform.SetParent(null);
                            PoliceAndThief.jewel12.transform.position = new Vector3(-8.25f, -0.45f, 1f);
                            PoliceAndThief.jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.jewel13.transform.SetParent(null);
                            PoliceAndThief.jewel13.transform.position = new Vector3(-19.75f, -1.55f, 1f);
                            PoliceAndThief.jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.jewel14.transform.SetParent(null);
                            PoliceAndThief.jewel14.transform.position = new Vector3(-12.1f, -13.15f, 1f);
                            PoliceAndThief.jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.jewel15.transform.SetParent(null);
                            PoliceAndThief.jewel15.transform.position = new Vector3(7.15f, -14.45f, 1f);
                            PoliceAndThief.jewel15BeingStealed = null;
                            break;
                    }
                }
                else if (RebuildUs.activatedDleks)
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.jewel01.transform.SetParent(null);
                            PoliceAndThief.jewel01.transform.position = new Vector3(18.65f, -9.9f, 1f);
                            PoliceAndThief.jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.jewel02.transform.SetParent(null);
                            PoliceAndThief.jewel02.transform.position = new Vector3(21.5f, -2, 1f);
                            PoliceAndThief.jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.jewel03.transform.SetParent(null);
                            PoliceAndThief.jewel03.transform.position = new Vector3(5.9f, -8.25f, 1f);
                            PoliceAndThief.jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.jewel04.transform.SetParent(null);
                            PoliceAndThief.jewel04.transform.position = new Vector3(-4.5f, -7.5f, 1f);
                            PoliceAndThief.jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.jewel05.transform.SetParent(null);
                            PoliceAndThief.jewel05.transform.position = new Vector3(-7.85f, -14.45f, 1f);
                            PoliceAndThief.jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.jewel06.transform.SetParent(null);
                            PoliceAndThief.jewel06.transform.position = new Vector3(-6.65f, -4.8f, 1f);
                            PoliceAndThief.jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.jewel07.transform.SetParent(null);
                            PoliceAndThief.jewel07.transform.position = new Vector3(-10.5f, 2.15f, 1f);
                            PoliceAndThief.jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.jewel08.transform.SetParent(null);
                            PoliceAndThief.jewel08.transform.position = new Vector3(5.5f, 3.5f, 1f);
                            PoliceAndThief.jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.jewel09.transform.SetParent(null);
                            PoliceAndThief.jewel09.transform.position = new Vector3(19, -1.2f, 1f);
                            PoliceAndThief.jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.jewel10.transform.SetParent(null);
                            PoliceAndThief.jewel10.transform.position = new Vector3(21.5f, -8.35f, 1f);
                            PoliceAndThief.jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.jewel11.transform.SetParent(null);
                            PoliceAndThief.jewel11.transform.position = new Vector3(12.5f, -3.75f, 1f);
                            PoliceAndThief.jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.jewel12.transform.SetParent(null);
                            PoliceAndThief.jewel12.transform.position = new Vector3(5.9f, -5.25f, 1f);
                            PoliceAndThief.jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.jewel13.transform.SetParent(null);
                            PoliceAndThief.jewel13.transform.position = new Vector3(-2.65f, -16.5f, 1f);
                            PoliceAndThief.jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.jewel14.transform.SetParent(null);
                            PoliceAndThief.jewel14.transform.position = new Vector3(-16.75f, -4.75f, 1f);
                            PoliceAndThief.jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.jewel15.transform.SetParent(null);
                            PoliceAndThief.jewel15.transform.position = new Vector3(-3.8f, 3.5f, 1f);
                            PoliceAndThief.jewel15BeingStealed = null;
                            break;
                    }
                }
                else
                {
                    switch (jewelRevertedId)
                    {
                        case 1:
                            PoliceAndThief.jewel01.transform.SetParent(null);
                            PoliceAndThief.jewel01.transform.position = new Vector3(-18.65f, -9.9f, 1f);
                            PoliceAndThief.jewel01BeingStealed = null;
                            break;
                        case 2:
                            PoliceAndThief.jewel02.transform.SetParent(null);
                            PoliceAndThief.jewel02.transform.position = new Vector3(-21.5f, -2, 1f);
                            PoliceAndThief.jewel02BeingStealed = null;
                            break;
                        case 3:
                            PoliceAndThief.jewel03.transform.SetParent(null);
                            PoliceAndThief.jewel03.transform.position = new Vector3(-5.9f, -8.25f, 1f);
                            PoliceAndThief.jewel03BeingStealed = null;
                            break;
                        case 4:
                            PoliceAndThief.jewel04.transform.SetParent(null);
                            PoliceAndThief.jewel04.transform.position = new Vector3(4.5f, -7.5f, 1f);
                            PoliceAndThief.jewel04BeingStealed = null;
                            break;
                        case 5:
                            PoliceAndThief.jewel05.transform.SetParent(null);
                            PoliceAndThief.jewel05.transform.position = new Vector3(7.85f, -14.45f, 1f);
                            PoliceAndThief.jewel05BeingStealed = null;
                            break;
                        case 6:
                            PoliceAndThief.jewel06.transform.SetParent(null);
                            PoliceAndThief.jewel06.transform.position = new Vector3(6.65f, -4.8f, 1f);
                            PoliceAndThief.jewel06BeingStealed = null;
                            break;
                        case 7:
                            PoliceAndThief.jewel07.transform.SetParent(null);
                            PoliceAndThief.jewel07.transform.position = new Vector3(10.5f, 2.15f, 1f);
                            PoliceAndThief.jewel07BeingStealed = null;
                            break;
                        case 8:
                            PoliceAndThief.jewel08.transform.SetParent(null);
                            PoliceAndThief.jewel08.transform.position = new Vector3(-5.5f, 3.5f, 1f);
                            PoliceAndThief.jewel08BeingStealed = null;
                            break;
                        case 9:
                            PoliceAndThief.jewel09.transform.SetParent(null);
                            PoliceAndThief.jewel09.transform.position = new Vector3(-19, -1.2f, 1f);
                            PoliceAndThief.jewel09BeingStealed = null;
                            break;
                        case 10:
                            PoliceAndThief.jewel10.transform.SetParent(null);
                            PoliceAndThief.jewel10.transform.position = new Vector3(-21.5f, -8.35f, 1f);
                            PoliceAndThief.jewel10BeingStealed = null;
                            break;
                        case 11:
                            PoliceAndThief.jewel11.transform.SetParent(null);
                            PoliceAndThief.jewel11.transform.position = new Vector3(-12.5f, -3.75f, 1f);
                            PoliceAndThief.jewel11BeingStealed = null;
                            break;
                        case 12:
                            PoliceAndThief.jewel12.transform.SetParent(null);
                            PoliceAndThief.jewel12.transform.position = new Vector3(-5.9f, -5.25f, 1f);
                            PoliceAndThief.jewel12BeingStealed = null;
                            break;
                        case 13:
                            PoliceAndThief.jewel13.transform.SetParent(null);
                            PoliceAndThief.jewel13.transform.position = new Vector3(2.65f, -16.5f, 1f);
                            PoliceAndThief.jewel13BeingStealed = null;
                            break;
                        case 14:
                            PoliceAndThief.jewel14.transform.SetParent(null);
                            PoliceAndThief.jewel14.transform.position = new Vector3(16.75f, -4.75f, 1f);
                            PoliceAndThief.jewel14BeingStealed = null;
                            break;
                        case 15:
                            PoliceAndThief.jewel15.transform.SetParent(null);
                            PoliceAndThief.jewel15.transform.position = new Vector3(3.8f, 3.5f, 1f);
                            PoliceAndThief.jewel15BeingStealed = null;
                            break;
                    }
                }
                break;
            // MiraHQ
            case 1:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(-4.5f, 2.5f, 1f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(6.25f, 14f, 1f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(9.15f, 4.75f, 1f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(14.75f, 20.5f, 1f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(19.5f, 17.5f, 1f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(21, 24.1f, 1f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(19.5f, 4.75f, 1f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(28.25f, 0, 1f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(2.45f, 11.25f, 1f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(4.4f, 1.75f, 1f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(9.25f, 13f, 1f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(13.75f, 23.5f, 1f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(16, 4, 1f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(15.35f, -0.9f, 1f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(19.5f, -1.75f, 1f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
            // Polus
            case 2:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(16.7f, -2.65f, 0.75f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(25.35f, -7.35f, 0.75f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(34.9f, -9.75f, 0.75f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(36.5f, -21.75f, 0.75f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(17.25f, -17.5f, 0.75f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(10.9f, -20.5f, -0.75f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(1.5f, -20.25f, 0.75f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 08:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(3f, -12f, 0.75f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 09:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(30f, -7.35f, 0.75f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(40.25f, -8f, 0.75f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(26f, -17.15f, 0.75f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(22f, -25.25f, 0.75f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(20.65f, -12f, 0.75f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(9.75f, -12.25f, 0.75f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(2.25f, -24f, 0.75f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
            // Dleks
            case 3:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(18.65f, -9.9f, 1f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(21.5f, -2, 1f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(5.9f, -8.25f, 1f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(-4.5f, -7.5f, 1f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(-7.85f, -14.45f, 1f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(-6.65f, -4.8f, 1f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(-10.5f, 2.15f, 1f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(5.5f, 3.5f, 1f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(19, -1.2f, 1f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(21.5f, -8.35f, 1f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(12.5f, -3.75f, 1f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(5.9f, -5.25f, 1f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(-2.65f, -16.5f, 1f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(-16.75f, -4.75f, 1f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(-3.8f, 3.5f, 1f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
            // Airship
            case 4:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(-23.5f, -1.5f, 1f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(-14.15f, -4.85f, 1f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(-13.9f, -16.25f, 1f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(-0.85f, -2.5f, 1f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(-5, 8.5f, 1f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(19.3f, -4.15f, 1f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(19.85f, 8, 1f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(28.85f, -1.75f, 1f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(-14.5f, -8.5f, 1f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(6.3f, -2.75f, 1f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(20.75f, 2.5f, 1f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(29.25f, 7, 1f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(37.5f, -3.5f, 1f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(25.2f, -8.75f, 1f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(16.3f, -11, 1f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
            // Fungle
            case 5:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(-18.25f, 5f, 1f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(-22.65f, -7.15f, 1f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(2, 4.35f, 1f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(-3.15f, -10.5f, 0.9f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(23.7f, -7.8f, 1f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(-4.75f, -1.75f, 1f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(8f, -10f, 1f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(7f, 1.75f, 1f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(13.25f, 10, 1f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(22.3f, 3.3f, 1f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(20.5f, 7.35f, 1f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(24.15f, 14.45f, 1f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(-16.12f, 0.7f, 1f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(1.65f, -1.5f, 1f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(10.5f, -12, 1f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
            // Submerged
            case 6:
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.transform.SetParent(null);
                        PoliceAndThief.jewel01.transform.position = new Vector3(-15f, 17.5f, -1f);
                        PoliceAndThief.jewel01BeingStealed = null;
                        break;
                    case 2:
                        PoliceAndThief.jewel02.transform.SetParent(null);
                        PoliceAndThief.jewel02.transform.position = new Vector3(8f, 32f, -1f);
                        PoliceAndThief.jewel02BeingStealed = null;
                        break;
                    case 3:
                        PoliceAndThief.jewel03.transform.SetParent(null);
                        PoliceAndThief.jewel03.transform.position = new Vector3(-6.75f, 10f, -1f);
                        PoliceAndThief.jewel03BeingStealed = null;
                        break;
                    case 4:
                        PoliceAndThief.jewel04.transform.SetParent(null);
                        PoliceAndThief.jewel04.transform.position = new Vector3(5.15f, 8f, -1f);
                        PoliceAndThief.jewel04BeingStealed = null;
                        break;
                    case 5:
                        PoliceAndThief.jewel05.transform.SetParent(null);
                        PoliceAndThief.jewel05.transform.position = new Vector3(5f, -33.5f, -1f);
                        PoliceAndThief.jewel05BeingStealed = null;
                        break;
                    case 6:
                        PoliceAndThief.jewel06.transform.SetParent(null);
                        PoliceAndThief.jewel06.transform.position = new Vector3(-4.15f, -33.5f, -1f);
                        PoliceAndThief.jewel06BeingStealed = null;
                        break;
                    case 7:
                        PoliceAndThief.jewel07.transform.SetParent(null);
                        PoliceAndThief.jewel07.transform.position = new Vector3(-14f, -27.75f, -1f);
                        PoliceAndThief.jewel07BeingStealed = null;
                        break;
                    case 8:
                        PoliceAndThief.jewel08.transform.SetParent(null);
                        PoliceAndThief.jewel08.transform.position = new Vector3(7.8f, -23.75f, -1f);
                        PoliceAndThief.jewel08BeingStealed = null;
                        break;
                    case 9:
                        PoliceAndThief.jewel09.transform.SetParent(null);
                        PoliceAndThief.jewel09.transform.position = new Vector3(-6.75f, -42.75f, -1f);
                        PoliceAndThief.jewel09BeingStealed = null;
                        break;
                    case 10:
                        PoliceAndThief.jewel10.transform.SetParent(null);
                        PoliceAndThief.jewel10.transform.position = new Vector3(13f, -25.25f, -1f);
                        PoliceAndThief.jewel10BeingStealed = null;
                        break;
                    case 11:
                        PoliceAndThief.jewel11.transform.SetParent(null);
                        PoliceAndThief.jewel11.transform.position = new Vector3(-14f, -34.25f, -1f);
                        PoliceAndThief.jewel11BeingStealed = null;
                        break;
                    case 12:
                        PoliceAndThief.jewel12.transform.SetParent(null);
                        PoliceAndThief.jewel12.transform.position = new Vector3(0f, -33.5f, -1f);
                        PoliceAndThief.jewel12BeingStealed = null;
                        break;
                    case 13:
                        PoliceAndThief.jewel13.transform.SetParent(null);
                        PoliceAndThief.jewel13.transform.position = new Vector3(-6.5f, 14f, -1f);
                        PoliceAndThief.jewel13BeingStealed = null;
                        break;
                    case 14:
                        PoliceAndThief.jewel14.transform.SetParent(null);
                        PoliceAndThief.jewel14.transform.position = new Vector3(14.25f, 24.5f, -1f);
                        PoliceAndThief.jewel14BeingStealed = null;
                        break;
                    case 15:
                        PoliceAndThief.jewel15.transform.SetParent(null);
                        PoliceAndThief.jewel15.transform.position = new Vector3(-12.25f, 31f, -1f);
                        PoliceAndThief.jewel15BeingStealed = null;
                        break;
                }
                break;
        }

        // if police can't see jewels, hide it after jailing a player
        if (PlayerControl.LocalPlayer == PoliceAndThief.policeplayer01 || PlayerControl.LocalPlayer == PoliceAndThief.policeplayer02 || PlayerControl.LocalPlayer == PoliceAndThief.policeplayer03 || PlayerControl.LocalPlayer == PoliceAndThief.policeplayer04 || PlayerControl.LocalPlayer == PoliceAndThief.policeplayer05 || PlayerControl.LocalPlayer == PoliceAndThief.policeplayer06)
        {
            if (!PoliceAndThief.policeCanSeeJewels)
            {
                switch (jewelRevertedId)
                {
                    case 1:
                        PoliceAndThief.jewel01.SetActive(false);
                        break;
                    case 2:
                        PoliceAndThief.jewel02.SetActive(false);
                        break;
                    case 3:
                        PoliceAndThief.jewel03.SetActive(false);
                        break;
                    case 4:
                        PoliceAndThief.jewel04.SetActive(false);
                        break;
                    case 5:
                        PoliceAndThief.jewel05.SetActive(false);
                        break;
                    case 6:
                        PoliceAndThief.jewel06.SetActive(false);
                        break;
                    case 7:
                        PoliceAndThief.jewel07.SetActive(false);
                        break;
                    case 8:
                        PoliceAndThief.jewel08.SetActive(false);
                        break;
                    case 9:
                        PoliceAndThief.jewel09.SetActive(false);
                        break;
                    case 10:
                        PoliceAndThief.jewel10.SetActive(false);
                        break;
                    case 11:
                        PoliceAndThief.jewel11.SetActive(false);
                        break;
                    case 12:
                        PoliceAndThief.jewel12.SetActive(false);
                        break;
                    case 13:
                        PoliceAndThief.jewel13.SetActive(false);
                        break;
                    case 14:
                        PoliceAndThief.jewel14.SetActive(false);
                        break;
                    case 15:
                        PoliceAndThief.jewel15.SetActive(false);
                        break;
                }
            }
        }
    }

    public static void policeandThiefsTased(byte targetId)
    {
        PlayerControl tased = Helpers.PlayerById(targetId);

        if (PlayerControl.LocalPlayer == tased)
        {
            SoundManager.Instance.PlaySound(AssetLoader.policeTaser, false, 100f);
            if (MapBehaviour.Instance)
            {
                MapBehaviour.Instance.Close();
            }
        }
        new Tased(PoliceAndThief.policeTaseDuration, tased);
        if (PoliceAndThief.thiefplayer01 != null && targetId == PoliceAndThief.thiefplayer01.PlayerId)
        {
            if (PoliceAndThief.thiefplayer01IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer01JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer02 != null && targetId == PoliceAndThief.thiefplayer02.PlayerId)
        {
            if (PoliceAndThief.thiefplayer02IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer02JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer03 != null && targetId == PoliceAndThief.thiefplayer03.PlayerId)
        {
            if (PoliceAndThief.thiefplayer03IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer03JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer04 != null && targetId == PoliceAndThief.thiefplayer04.PlayerId)
        {
            if (PoliceAndThief.thiefplayer04IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer04JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer05 != null && targetId == PoliceAndThief.thiefplayer05.PlayerId)
        {
            if (PoliceAndThief.thiefplayer05IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer05JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer06 != null && targetId == PoliceAndThief.thiefplayer06.PlayerId)
        {
            if (PoliceAndThief.thiefplayer06IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer06JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer07 != null && targetId == PoliceAndThief.thiefplayer07.PlayerId)
        {
            if (PoliceAndThief.thiefplayer07IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer07JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer08 != null && targetId == PoliceAndThief.thiefplayer08.PlayerId)
        {
            if (PoliceAndThief.thiefplayer08IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer08JewelId);
            }
        }
        else if (PoliceAndThief.thiefplayer09 != null && targetId == PoliceAndThief.thiefplayer09.PlayerId)
        {
            if (PoliceAndThief.thiefplayer09IsStealing)
            {
                policeandThiefRevertedJewelPosition(targetId, PoliceAndThief.thiefplayer09JewelId);
            }
        }
        return;
    }

    public static PlayerControl oldHotPotato = null;

    public static void hotPotatoTransfer(byte targetId)
    {
        PlayerControl player = Helpers.PlayerById(targetId);

        if (HotPotato.hotPotatoPlayer != null)
        {

            if (!HotPotato.firstPotatoTransfered)
            {
                HotPotato.firstPotatoTransfered = true;
            }

            if (HotPotato.resetTimeForTransfer)
            {
                HotPotato.timeforTransfer = HotPotato.savedtimeforTransfer + 3f;
            }
            else
            {
                HotPotato.timeforTransfer = (HotPotato.timeforTransfer + HotPotato.increaseTimeIfNoReset + 3f);
            }

            oldHotPotato = HotPotato.hotPotatoPlayer;
            oldHotPotato.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);

            HotPotato.notPotatoTeam.Add(oldHotPotato);

            // Switch role
            if (HotPotato.notPotato01 != null && HotPotato.notPotato01 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato01);
                HotPotato.notPotato01 = oldHotPotato;
            }
            else if (HotPotato.notPotato02 != null && HotPotato.notPotato02 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato02);
                HotPotato.notPotato02 = oldHotPotato;
            }
            else if (HotPotato.notPotato03 != null && HotPotato.notPotato03 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato03);
                HotPotato.notPotato03 = oldHotPotato;
            }
            else if (HotPotato.notPotato04 != null && HotPotato.notPotato04 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato04);
                HotPotato.notPotato04 = oldHotPotato;
            }
            else if (HotPotato.notPotato05 != null && HotPotato.notPotato05 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato05);
                HotPotato.notPotato05 = oldHotPotato;
            }
            else if (HotPotato.notPotato06 != null && HotPotato.notPotato06 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato06);
                HotPotato.notPotato06 = oldHotPotato;
            }
            else if (HotPotato.notPotato07 != null && HotPotato.notPotato07 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato07);
                HotPotato.notPotato07 = oldHotPotato;
            }
            else if (HotPotato.notPotato08 != null && HotPotato.notPotato08 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato08);
                HotPotato.notPotato08 = oldHotPotato;
            }
            else if (HotPotato.notPotato09 != null && HotPotato.notPotato09 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato09);
                HotPotato.notPotato09 = oldHotPotato;
            }
            else if (HotPotato.notPotato10 != null && HotPotato.notPotato10 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato10);
                HotPotato.notPotato10 = oldHotPotato;
            }
            else if (HotPotato.notPotato11 != null && HotPotato.notPotato11 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato11);
                HotPotato.notPotato11 = oldHotPotato;
            }
            else if (HotPotato.notPotato12 != null && HotPotato.notPotato12 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato12);
                HotPotato.notPotato12 = oldHotPotato;
            }
            else if (HotPotato.notPotato13 != null && HotPotato.notPotato13 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato13);
                HotPotato.notPotato13 = oldHotPotato;
            }
            else if (HotPotato.notPotato14 != null && HotPotato.notPotato14 == player)
            {
                HotPotato.notPotatoTeam.Remove(HotPotato.notPotato14);
                HotPotato.notPotato14 = oldHotPotato;
            }

            HotPotato.hotPotatoPlayer = player;
            HotPotato.hotPotatoPlayer.NetTransform.Halt();
            HotPotato.hotPotatoPlayer.moveable = false;
            HotPotato.hotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            HotPotato.hotPotato.transform.position = HotPotato.hotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
            HotPotato.hotPotato.transform.parent = HotPotato.hotPotatoPlayer.transform;

            HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>((p) =>
            { // Delayed action
                if (p == 1f)
                {
                    HotPotato.hotPotatoPlayer.moveable = true;
                }
            })));

            int notPotatosAlives = 0;
            HotPotato.notPotatoTeamAlive.Clear();
            foreach (PlayerControl notPotato in HotPotato.notPotatoTeam)
            {
                if (!notPotato.Data.IsDead)
                {
                    notPotatosAlives += 1;
                    HotPotato.notPotatoTeamAlive.Add(notPotato);
                }
            }

            Helpers.showGamemodesPopUp(1, Helpers.PlayerById(HotPotato.hotPotatoPlayer.PlayerId));

            HotPotato.hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>").Append(HotPotato.hotPotatoPlayer.name).Append("</color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#00F7FFFF>").Append(HotPotato.notPotatoTeam.Count).Append("</color>").ToString();

            // Set custom cooldown to the hotpotato button
            HotPotato.hotPotatoButton.Timer = HotPotato.transferCooldown;
            if (PlayerControl.LocalPlayer == HotPotato.hotPotatoPlayer || PlayerControl.LocalPlayer == oldHotPotato)
                SoundManager.Instance.PlaySound(AssetLoader.StealRoleSound, false, 100f);
        }
    }

    public static void hotPotatoExploded()
    {
        HotPotato.hotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
        HotPotato.hotPotatoPlayer.MurderPlayer(HotPotato.hotPotatoPlayer, MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost);
        HudManager.Instance.DangerMeter.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead);
    }

    public static void battleRoyaleShowShoots(byte playerId, int color, float angle)
    {
        new BattleRoyaleShoot(Helpers.PlayerById(playerId), color, angle);
    }

    public static void battleRoyaleCheckWin(int whichTeamCheck)
    {
        if (BattleRoyale.matchType == 0)
        {
            int soloPlayersAlives = 0;

            foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam)
            {

                if (!soloPlayer.Data.IsDead)
                {
                    soloPlayersAlives += 1;
                }
            }

            BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters)).Append("<color=#009F57FF>").Append(soloPlayersAlives).Append("</color>").ToString();

            if (soloPlayersAlives <= 1)
            {
                BattleRoyale.triggerSoloWin = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin, false);
            }
        }
        else
        {

            int limePlayersAlive = 0;

            foreach (PlayerControl limePlayer in BattleRoyale.limeTeam)
            {

                if (!limePlayer.Data.IsDead)
                {
                    limePlayersAlive += 1;
                }

            }

            int pinkPlayersAlive = 0;

            foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam)
            {

                if (!pinkPlayer.Data.IsDead)
                {
                    pinkPlayersAlive += 1;
                }

            }

            if (whichTeamCheck == 3)
            {
                if (limePlayersAlive <= 0)
                {
                    BattleRoyale.triggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                }
                else if (pinkPlayersAlive <= 0)
                {
                    BattleRoyale.triggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                }
            }

            if (BattleRoyale.serialKiller != null)
            {

                int serialKillerAlive = 0;

                foreach (PlayerControl serialKiller in BattleRoyale.serialKillerTeam)
                {

                    if (!serialKiller.Data.IsDead)
                    {
                        serialKillerAlive += 1;
                    }

                }
                BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(" <color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyaleSerialKiller)).Append("<color=#808080FF>").Append(serialKillerAlive).Append("</color>").ToString();
                if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.serialKiller.Data.IsDead)
                {
                    BattleRoyale.triggerSerialKillerWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                }
                else if (pinkPlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                {
                    BattleRoyale.triggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                }
                else if (limePlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                {
                    BattleRoyale.triggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                }
            }
            else
            {
                BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(" <color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color>").ToString();
                if (pinkPlayersAlive <= 0)
                {
                    BattleRoyale.triggerLimeTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                }
                else if (limePlayersAlive <= 0)
                {
                    BattleRoyale.triggerPinkTeamWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                }
            }
        }
    }

    public static void battleRoyaleScoreCheck(int whichTeamCheck, int multiplier)
    {
        switch (whichTeamCheck)
        {
            case 1:
                BattleRoyale.limePoints += 10 * multiplier;
                break;
            case 2:
                BattleRoyale.pinkPoints += 10 * multiplier;
                break;
            case 3:
                BattleRoyale.serialKillerPoints += 10 * multiplier;
                break;
        }

        if (BattleRoyale.serialKiller != null)
        {
            BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal)).Append(BattleRoyale.requiredScore).Append(" | <color=#39FF14FF>").Append(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append(BattleRoyale.limePoints).Append("</color> | <color=#F2BEFFFF>").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(BattleRoyale.pinkPoints).Append("</color> | <color=#808080FF>").Append(Tr.Get(TrKey.BattleRoyaleSerialKillerPoints)).Append(BattleRoyale.serialKillerPoints).Append("</color>").ToString();
        }
        else
        {
            BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal)).Append(BattleRoyale.requiredScore).Append(" | <color=#39FF14FF>").Append(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append(BattleRoyale.limePoints).Append("</color> | <color=#F2BEFFFF>").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(BattleRoyale.pinkPoints).Append("</color>").ToString();
        }

        if (BattleRoyale.limePoints >= BattleRoyale.requiredScore)
        {
            BattleRoyale.triggerLimeTeamWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
        }
        else if (BattleRoyale.pinkPoints >= BattleRoyale.requiredScore)
        {
            BattleRoyale.triggerPinkTeamWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
        }
        else if (BattleRoyale.serialKillerPoints >= BattleRoyale.requiredScore)
        {
            BattleRoyale.triggerSerialKillerWin = true;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
        }
    }
}