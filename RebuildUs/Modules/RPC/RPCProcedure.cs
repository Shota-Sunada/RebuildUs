using Assets.CoreScripts;
using PowerTools;
using Action = Il2CppSystem.Action;

namespace RebuildUs.Modules.RPC;

internal static partial class RPCProcedure
{
    internal static void ResetVariables()
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
        DeathPopup.Reset();
        CustomOverlays.ResetOverlays();
        SpecimenVital.ClearAndReload();
        AdditionalVents.ClearAndReload();
        SpawnIn.Reset();
        Map.ResetRealTasks();

        KillAnimationPatch.HideNextAnimation = false;
        KillAnimationPatch.AvoidNextKillMovement = false;

        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishResetVariables);
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
            if (CustomOption.AllOptionsById.TryGetValue((int)id, out CustomOption option))
            {
                option.UpdateSelection((int)selection, option.GetOptionIcon());
            }
        }
    }

    private static void WorkaroundSetRoles(byte numberOfRoles, MessageReader reader)
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

    internal static void SetRole(byte roleId, byte playerId)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }
        Logger.LogInfo($"{player.Data.PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), roleId)}", "setRole");
        player.SetRole((RoleType)roleId);
    }

    internal static void AddModifier(byte modId, byte playerId)
    {
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(x => x.PlayerId == playerId, x => x.AddModifier((ModifierType)modId));
    }

    internal static void VersionHandshake(int major, int minor, int build, int revision, int clientId, Guid guid)
    {
        GameStart.PlayerVersions[clientId] = new(major, minor, build, revision, guid);
    }

    internal static void FinishSetRole()
    {
        RoleAssignment.IsAssigned = true;
    }

    internal static void UpdateMeeting(byte targetId, bool dead = true)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }
        foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
        {
            if (pva.TargetPlayerId == targetId)
            {
                pva.SetDead(pva.DidReport, dead);
                pva.Overlay.gameObject.SetActive(dead);
            }

            // Give players back their vote if target is shot dead
            if (!Helpers.RefundVotes || !dead)
            {
                continue;
            }
            if (pva.VotedFor != targetId)
            {
                continue;
            }
            pva.UnsetVote();
            PlayerControl voteAreaPlayer = Helpers.PlayerById(pva.TargetPlayerId);
            if (!voteAreaPlayer.AmOwner)
            {
                continue;
            }
            MeetingHud.Instance.ClearVote();
        }

        if (AmongUsClient.Instance.AmHost)
        {
            MeetingHud.Instance.CheckForEndVoting();
        }
    }

    internal static void UncheckedCmdReportDeadBody(byte sourceId, byte targetId)
    {
        PlayerControl source = Helpers.PlayerById(sourceId);
        NetworkedPlayerInfo target = targetId == byte.MaxValue ? null : Helpers.PlayerById(targetId).Data;
        source?.ReportDeadBody(target);
    }

    internal static void UseUncheckedVent(int ventId, byte playerId, byte isEnter)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the coroutines cannot be accessed
        MessageReader reader = new();
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

    internal static void UncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
        {
            return;
        }
        PlayerControl source = Helpers.PlayerById(sourceId);
        PlayerControl target = Helpers.PlayerById(targetId);
        if (source == null || target == null)
        {
            return;
        }
        if (showAnimation == 0)
        {
            KillAnimationPatch.HideNextAnimation = true;
        }
        source.MurderPlayer(target);
    }

    internal static void UncheckedExilePlayer(byte targetId)
    {
        PlayerControl target = Helpers.PlayerById(targetId);
        target?.Exiled();
    }

    internal static void DynamicMapOption(byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.SetByte(ByteOptionNames.MapId, mapId);
    }

    internal static void ShareGamemode(byte gameMode)
    {
        MapSettings.GameMode = (CustomGamemode)gameMode;
        GameStart.SendGamemode = false;
    }

    private static void SetGameStarting()
    {
        GameStart.StartingTimer = 5f;
    }

    private static void StopStart()
    {
        StopStartSound();
        if (AmongUsClient.Instance.AmHost)
        {
            FastDestroyableSingleton<GameStartManager>.Instance.ResetStartState();
        }
    }

    private static void StopStartSound()
    {
        SoundManager.Instance.StopSound(FastDestroyableSingleton<GameStartManager>.Instance.gameStartSound);
    }

    private static void FinishResetVariables(byte playerId)
    {
        Dictionary<byte, bool> checkList = RoleAssignment.CheckList;
        if (checkList == null)
        {
            return;
        }
        if (checkList.ContainsKey(playerId))
        {
            checkList[playerId] = true;
        }
    }

    internal static void SetLovers(byte playerId1, byte playerId2)
    {
        Lovers.AddCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
    }

    private static void OverrideNativeRole(byte playerId, byte roleType)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        player.roleAssigned = false;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
    }

    internal static void UncheckedEndGame(byte reason, bool isO2Win = false)
    {
        EndGameMain.IsO2Win = isO2Win;
        AmongUsClient.Instance.GameState = InnerNetClient.GameStates.Ended;
        Il2CppSystem.Collections.Generic.List<ClientData> obj2 = AmongUsClient.Instance.allClients;
        lock (obj2) AmongUsClient.Instance.allClients.Clear();

        Il2CppSystem.Collections.Generic.List<Action> obj = AmongUsClient.Instance.Dispatcher;
        lock (obj)
        {
            AmongUsClient.Instance.Dispatcher.Add(new System.Action(() =>
            {
                GameManager.Instance.enabled = false;
                GameManager.Instance.ShouldCheckForGameEnd = false;
                AmongUsClient.Instance.OnGameEnd(new((GameOverReason)reason, false));

                if (AmongUsClient.Instance.AmHost)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)reason, false);
                }
            }));
        }
    }

    internal static void UncheckedSetTasks(byte playerId, byte[] taskTypeIds)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        player.ClearAllTasks();

        player.Data.SetTasks(taskTypeIds);
    }

    internal static void FinishShipStatusBegin()
    {
        PlayerControl.LocalPlayer.OnFinishShipStatusBegin();
    }

    internal static void EngineerFixLights()
    {
        SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    internal static void EngineerFixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    internal static void EngineerUsedRepair(byte engineerId)
    {
        PlayerControl engineerPlayer = Helpers.PlayerById(engineerId);
        if (engineerPlayer == null)
        {
            return;
        }
        Engineer engineer = Engineer.GetRole(engineerPlayer);
        if (engineer != null)
        {
            engineer.RemainingFixes--;
        }
    }

    internal static void ArsonistDouse(byte playerId)
    {
        Arsonist arsonist = Arsonist.Instance;
        if (arsonist == null)
        {
            return;
        }
        PlayerControl target = Helpers.PlayerById(playerId);
        if (target != null)
        {
            arsonist.DousedPlayers.Add(target);
        }
    }

    internal static void ArsonistWin()
    {
        Arsonist.TriggerArsonistWin = true;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null || !p.IsAlive() || p.IsRole(RoleType.Arsonist))
            {
                continue;
            }
            p.Exiled();
            GameHistory.FinalStatuses[p.PlayerId] = FinalStatus.Torched;
        }
    }

    internal static void CleanBody(byte playerId)
    {
        DeadBody[] array = UnityObject.FindObjectsOfType<DeadBody>();
        foreach (DeadBody t in array)
        {
            NetworkedPlayerInfo info = GameData.Instance.GetPlayerById(t.ParentId);
            if (info != null && info.PlayerId == playerId)
            {
                UnityObject.Destroy(t.gameObject);
            }
        }
    }

    internal static void VultureEat(byte playerId)
    {
        CleanBody(playerId);
        Vulture vulture = Vulture.Instance;
        if (vulture != null)
        {
            vulture.EatenBodies++;
        }
    }

    internal static void VultureWin()
    {
        Vulture.TriggerVultureWin = true;
    }

    internal static void ErasePlayerRoles(byte playerId, bool ignoreLovers = false, bool clearNeutralTasks = true)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }

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

    internal static void JackalCreatesSidekick(byte targetId)
    {
        PlayerControl target = Helpers.PlayerById(targetId);
        Jackal jackal = Jackal.Instance;
        if (target == null)
        {
            return;
        }

        if (!Jackal.CanCreateSidekickFromImpostor && target.Data.Role.IsImpostor)
        {
            jackal?.FakeSidekick = target;
        }
        else
        {
            bool wasSpy = target.IsRole(RoleType.Spy);
            bool wasImpostor = target.IsTeamImpostor(); // This can only be reached if impostors can be sidekicked.
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
            ErasePlayerRoles(target.PlayerId, true);
            if (target.SetRole(RoleType.Sidekick))
            {
                Sidekick sidekick = Sidekick.Instance;
                if (sidekick != null)
                {
                    if (wasSpy || wasImpostor)
                    {
                        sidekick.WasTeamRed = true;
                    }
                    sidekick.WasSpy = wasSpy;
                    sidekick.WasImpostor = wasImpostor;
                }
            }

            if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                PlayerControl.LocalPlayer.moveable = true;
            }
        }

        if (jackal != null)
        {
            jackal.CanSidekick = false;
            jackal.MySidekick = target;
        }
    }

    internal static void SidekickPromotes()
    {
        Sidekick sidekick = Sidekick.Instance;
        if (sidekick == null)
        {
            return;
        }

        bool wasTeamRed = sidekick.WasTeamRed;
        bool wasImpostor = sidekick.WasImpostor;
        bool wasSpy = sidekick.WasSpy;
        Jackal.RemoveCurrentJackal();
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(sidekick.Player, RoleTypes.Crewmate);
        ErasePlayerRoles(sidekick.Player.PlayerId, true);
        if (sidekick.Player.SetRole(RoleType.Jackal))
        {
            Jackal newJackal = Jackal.Instance;
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

    internal static void MedicSetShielded(byte shieldedId)
    {
        Medic.UsedShield = true;
        Medic.Shielded = Helpers.PlayerById(shieldedId);
        Medic.FutureShielded = null;
    }

    internal static void ShieldedMurderAttempt()
    {
        if (!Medic.Exists || Medic.Shielded == null)
        {
            return;
        }

        bool isShieldedAndShow = Medic.Shielded == PlayerControl.LocalPlayer && Medic.ShowAttemptToShielded;
        bool isMedicAndShow = PlayerControl.LocalPlayer.IsRole(RoleType.Medic) && Medic.ShowAttemptToMedic;

        if (!isShieldedAndShow && !isMedicAndShow || FastDestroyableSingleton<HudManager>.Instance?.FullScreen == null)
        {
            return;
        }
        Color c = Palette.ImpostorRed;
        Helpers.ShowFlash(new(c.r, c.g, c.b));
    }

    internal static void SetFutureShielded(byte playerId)
    {
        Medic.FutureShielded = Helpers.PlayerById(playerId);
        Medic.UsedShield = true;
    }

    internal static void TimeMasterRewindTime()
    {
        TimeMaster.ShieldActive = false; // Shield is no longer active when rewinding
        if (PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster))
        {
            TimeMaster.ResetTimeMasterButton();
        }

        HudManager hm = FastDestroyableSingleton<HudManager>.Instance;
        hm.FullScreen.color = new(0f, 0.5f, 0.8f, 0.3f);
        hm.FullScreen.enabled = true;
        hm.FullScreen.gameObject.SetActive(true);
        hm.StartCoroutine(Effects.Lerp(TimeMaster.RewindTime / 2,
            new Action<float>(p =>
            {
                if (Mathf.Approximately(p, 1f))
                {
                    hm.FullScreen.enabled = false;
                }
            })));

        if (!TimeMaster.Exists || PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster))
        {
            return; // Time Master himself does not rewind
        }
        if (PlayerControl.LocalPlayer.IsGm())
        {
            return; // GM does not rewind
        }

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

    internal static void TimeMasterShield()
    {
        TimeMaster.ShieldActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.ShieldDuration,
            new Action<float>(p =>
            {
                if (Mathf.Approximately(p, 1f))
                {
                    TimeMaster.ShieldActive = false;
                }
            })));
    }

    internal static void GuesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleType)
    {
        PlayerControl killer = Helpers.PlayerById(killerId);
        PlayerControl dyingTarget = Helpers.PlayerById(dyingTargetId);
        if (dyingTarget == null)
        {
            return;
        }
        dyingTarget.Exiled();
        PlayerControl dyingLoverPartner = Lovers.BothDie ? dyingTarget.GetPartner() : null; // Lover check

        if (killer != null)
        {
            Guesser.RemainingShots(killer, true);
        }

        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
        }

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

        PlayerControl guessedTarget = Helpers.PlayerById(guessedTargetId);
        if (Guesser.ShowInfoInGhostChat && PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null)
        {
            RoleInfo roleInfo = null;
            foreach (RoleInfo r in RoleInfo.AllRoleInfos)
            {
                if ((byte)r.RoleType == guessedRoleType)
                {
                    roleInfo = r;
                    break;
                }
            }

            if (roleInfo == null)
            {
                return;
            }
            string msg = string.Format(Tr.Get(TrKey.GuesserGuessChat), roleInfo.Name, guessedTarget.Data.PlayerName);
            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(killer, msg);
            }

            if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
            {
                FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }
        }
    }

    internal static void PlaceJackInTheBox(byte[] buff)
    {
        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        _ = new JackInTheBox(position);
    }

    internal static void LightsOut()
    {
        Trickster.LightsOutTimer = Trickster.LightsOutDuration;
        // If the local player is impostor indicate lights out
        if (Helpers.HasImpostorVision(PlayerControl.LocalPlayer))
        {
            _ = new CustomMessage("TricksterLightsOutText", Trickster.LightsOutDuration);
        }
    }

    internal static void EvilHackerCreatesMadmate(byte targetId, byte evilHackerId)
    {
        PlayerControl targetPlayer = Helpers.PlayerById(targetId);
        PlayerControl evilHackerPlayer = Helpers.PlayerById(evilHackerId);
        if (targetPlayer == null || evilHackerPlayer == null)
        {
            return;
        }
        EvilHacker evilHacker = EvilHacker.GetRole(evilHackerPlayer);
        if (evilHacker == null)
        {
            return;
        }
        if (!EvilHacker.CanCreateMadmateFromJackal && targetPlayer.IsRole(RoleType.Jackal))
        {
            evilHacker.FakeMadmate = targetPlayer;
        }
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

    internal static void UseAdminTime(float time)
    {
        MapSettings.RestrictAdminTime -= time;
    }

    internal static void UseCameraTime(float time)
    {
        MapSettings.RestrictCamerasTime -= time;
    }

    internal static void UseVitalsTime(float time)
    {
        MapSettings.RestrictVitalsTime -= time;
    }

    internal static void TrackerUsedTracker(byte targetId, byte trackerId)
    {
        PlayerControl trackerPlayer = Helpers.PlayerById(trackerId);
        if (trackerPlayer == null)
        {
            return;
        }
        Tracker tracker = Tracker.GetRole(trackerPlayer);
        if (tracker == null)
        {
            return;
        }

        tracker.UsedTracker = true;
        tracker.Tracked = Helpers.PlayerById(targetId);
    }

    internal static void SetFutureErased(byte playerId)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        Eraser.FutureErased ??= [];
        if (player != null)
        {
            Eraser.FutureErased.Add(player);
        }
    }

    internal static void VampireSetBitten(byte targetId, byte performReset)
    {
        if (performReset != 0)
        {
            Vampire.Bitten = null;
            return;
        }

        if (!Vampire.Exists)
        {
            return;
        }
        PlayerControl player = Helpers.PlayerById(targetId);
        if (player != null && !player.Data.IsDead)
        {
            Vampire.Bitten = player;
        }
    }

    private static void ShareRealTasks(MessageReader reader)
    {
        byte count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            byte playerId = reader.ReadByte();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Vector2 pos = new(x, y);

            if (!Map.RealTasks.TryGetValue(playerId, out Il2CppSystem.Collections.Generic.List<Vector2> list))
            {
                list = new();
                Map.RealTasks[playerId] = list;
            }

            list.Add(pos);
        }
    }

    internal static void PolusRandomSpawn(byte playerId, byte locId)
    {
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f,
            new Action<float>(p =>
            {
                // Delayed action
                if (!Mathf.Approximately(p, 1f))
                {
                    return;
                }
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
                PlayerControl player = Helpers.PlayerById(playerId);
                player?.transform.position = loc;
            })));
    }

    internal static void Synchronize(byte playerId, int tag)
    {
        SpawnIn.SynchronizeData.Synchronize((SynchronizeTag)tag, playerId);
    }

    internal static void PlaceCamera(float x, float y, byte roomId)
    {
        SecurityGuard sg = SecurityGuard.Instance;

        SurvCamera referenceCamera = UnityObject.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null)
        {
            return; // Mira HQ
        }

        sg.RemainingScrews -= SecurityGuard.CamPrice;
        sg.PlacedCameras++;

        Vector3 position = new(x, y);

        SystemTypes roomType = (SystemTypes)roomId;

        SurvCamera camera = UnityObject.Instantiate(referenceCamera);
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
        if (ByteOptionNames.MapId.Get() is 2 or 4)
        {
            camera.transform.localRotation = new(0, 0, 1, 1); // Polus and Airship
        }

        if (PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard))
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }

        MapSettings.CamerasToAdd.Add(camera);
    }

    internal static void SealVent(int ventId)
    {
        SecurityGuard sg = SecurityGuard.Instance;

        Vent vent = null;
        Il2CppReferenceArray<Vent> allVents = MapUtilities.CachedShipStatus.AllVents;
        foreach (Vent v in allVents)
        {
            if (v == null || v.Id != ventId)
            {
                continue;
            }
            vent = v;
            break;
        }

        if (vent == null)
        {
            return;
        }

        sg.RemainingScrews -= SecurityGuard.VentPrice;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard))
        {
            SpriteAnim animator = vent.GetComponent<SpriteAnim>();
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            vent.myRend.sprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0)
            {
                vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            }
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14)
            {
                vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            }
            vent.myRend.color = new(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        MapSettings.VentsToSeal.Add(vent);
    }

    internal static void MorphingMorph(byte playerId, byte morphId)
    {
        PlayerControl morphPlayer = Helpers.PlayerById(morphId);
        PlayerControl target = Helpers.PlayerById(playerId);
        if (morphPlayer == null || target == null)
        {
            return;
        }
        Morphing.GetRole(morphPlayer).StartMorph(target);
    }

    internal static void CamouflagerCamouflage()
    {
        if (!Camouflager.Exists)
        {
            return;
        }
        Camouflager.StartCamouflage();
    }

    internal static void SwapperSwap(byte playerId1, byte playerId2)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }
        Swapper.PlayerId1 = playerId1;
        Swapper.PlayerId2 = playerId2;
    }

    private static void SwapperAnimate() { }

    internal static void SetFutureSpelled(byte playerId)
    {
        PlayerControl player = Helpers.PlayerById(playerId);
        Witch.FutureSpelled ??= [];
        if (player != null)
        {
            Witch.FutureSpelled.Add(player);
        }
    }

    internal static void WitchSpellCast(byte playerId)
    {
        UncheckedExilePlayer(playerId);
        GameHistory.FinalStatuses[playerId] = FinalStatus.Spelled;
    }

    internal static void PlaceGarlic(float x, float y)
    {
        _ = new Garlic(new Vector3(x, y));
    }

    internal static void ImpostorPromotesToLastImpostor(byte targetId)
    {
        PlayerControl player = Helpers.PlayerById(targetId);
        player.AddModifier(ModifierType.LastImpostor);
    }

    internal static void ShifterShift(byte targetId)
    {
        if (Shifter.Players.Count == 0)
        {
            return;
        }
        Shifter oldShifter = Shifter.Players[0];
        PlayerControl player = Helpers.PlayerById(targetId);
        if (player == null || oldShifter == null)
        {
            return;
        }

        PlayerControl oldShifterPlayer = oldShifter.Player;
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

    internal static void SetFutureShifted(byte playerId)
    {
        if (Shifter.IsNeutral && !Shifter.ShiftPastShifters && Shifter.PastShifters.Contains(playerId))
        {
            return;
        }
        Shifter.FutureShift = Helpers.PlayerById(playerId);
    }

    internal static void SetShifterType(bool isNeutral)
    {
        Shifter.IsNeutral = isNeutral;
    }

    internal static void FortuneTellerUsedDivine(byte killerId, byte targetId)
    {
        LastImpostor.NumUsed += 1;
    }

    private static void SheriffKillRequest(byte sheriffId, byte targetId)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }
        PlayerControl sheriff = Helpers.PlayerById(sheriffId);
        PlayerControl target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        bool misfire = Sheriff.CheckKill(target);

        using RPCSender killSender = new(sheriff.NetId, CustomRPC.SheriffKill);
        killSender.Write(sheriffId);
        killSender.Write(targetId);
        killSender.Write(misfire);

        SheriffKill(sheriffId, targetId, misfire);
    }

    internal static void SheriffKill(byte sheriffId, byte targetId, bool misfire)
    {
        PlayerControl sheriff = Helpers.PlayerById(sheriffId);
        PlayerControl target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        Sheriff role = Sheriff.GetRole(sheriff);
        if (role != null)
        {
            role.NumShots--;
        }

        if (misfire)
        {
            sheriff.MurderPlayer(sheriff);
            GameHistory.FinalStatuses[sheriffId] = FinalStatus.Misfire;

            if (!Sheriff.MisfireKillsTarget)
            {
                return;
            }
            GameHistory.FinalStatuses[targetId] = FinalStatus.Misfire;
        }

        sheriff.MurderPlayer(target);
    }
}