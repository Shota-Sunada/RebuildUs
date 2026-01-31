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
        ModMapOptions.GameMode = (CustomGamemodes)gameMode;
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
        // Lovers.addCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
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
        SwitchSystem switchSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
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
                string msg = string.Format(Tr.Get("Hud.GuesserGuessChat"), roleInfo.Name, guessedTarget.Data.PlayerName);
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
            new CustomMessage("Hud.TricksterLightsOutText", Trickster.LightsOutDuration);
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
        ModMapOptions.RestrictAdminTime -= time;
    }

    public static void UseCameraTime(float time)
    {
        ModMapOptions.RestrictCamerasTime -= time;
    }

    public static void UseVitalsTime(float time)
    {
        ModMapOptions.RestrictVitalsTime -= time;
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
        ModMapOptions.CamerasToAdd.Add(camera);
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

        ModMapOptions.VentsToSeal.Add(vent);
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
}