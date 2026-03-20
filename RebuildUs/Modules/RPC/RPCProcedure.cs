using Assets.CoreScripts;
using Action = Il2CppSystem.Action;

namespace RebuildUs.Modules.RPC;

internal static partial class RPCProcedure
{
    [MethodRpc((uint)CustomRPC.ResetVariables)]
    internal static void ResetVariables(PlayerControl sender)
    {
        ResetVariablesLocal();
    }

    internal static void ResetVariablesLocal()
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

        FinishResetVariables(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.PlayerId);
    }

    private static void ShareOptions(MessageReader reader)
    {
        var amount = reader.ReadByte();
        for (var i = 0; i < amount; i++)
        {
            var id = reader.ReadPackedUInt32();
            var selection = reader.ReadPackedUInt32();
            if (CustomOption.AllOptionsById.TryGetValue((int)id, out var option))
            {
                option.UpdateSelection((int)selection, option.GetOptionIcon());
            }
        }
    }

    private static void WorkaroundSetRoles(byte numberOfRoles, MessageReader reader)
    {
        for (var i = 0; i < numberOfRoles; i++)
        {
            var playerId = (byte)reader.ReadPackedUInt32();
            var roleId = (byte)reader.ReadPackedUInt32();
            try
            {
                SetRoleLocal(roleId, playerId);
            }
            catch (Exception e)
            {
                Logger.LogError("Error while deserializing roles: " + e.Message);
            }
        }
    }

    [MethodRpc((uint)CustomRPC.SetRole)]
    internal static void SetRole(PlayerControl sender, byte roleId, byte playerId)
    {
        SetRoleLocal(roleId, playerId);
    }

    internal static void SetRoleLocal(byte roleId, byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }
        Logger.LogInfo($"{player.Data.PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), roleId)}", "setRole");
        player.SetRole((RoleType)roleId);
    }

    [MethodRpc((uint)CustomRPC.AddModifier)]
    internal static void AddModifier(PlayerControl sender, byte modId, byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }
        Logger.LogInfo($"{player.Data.PlayerName}({playerId}): {Enum.GetName(typeof(RoleType), modId)}", nameof(AddModifier));
        player.AddModifier((ModifierType)modId);
    }

    internal static void VersionHandshake(int major, int minor, int build, int revision, int clientId, Guid guid)
    {
        GameStart.PlayerVersions[clientId] = new(major, minor, build, revision, guid);
    }

    [MethodRpc((uint)CustomRPC.FinishSetRole)]
    internal static void FinishSetRole(PlayerControl sender)
    {
        RoleAssignment.IsAssigned = true;
    }

    internal static void UpdateMeeting(byte targetId, bool dead = true)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }
        foreach (var pva in MeetingHud.Instance.playerStates)
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
            var voteAreaPlayer = Helpers.PlayerById(pva.TargetPlayerId);
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

    [MethodRpc((uint)CustomRPC.UncheckedCmdReportDeadBody)]
    internal static void UncheckedCmdReportDeadBody(PlayerControl sender, byte sourceId, byte targetId)
    {
        var source = Helpers.PlayerById(sourceId);
        var target = targetId == byte.MaxValue ? null : Helpers.PlayerById(targetId).Data;
        source?.ReportDeadBody(target);
    }

    [MethodRpc((uint)CustomRPC.UseUncheckedVent)]
    internal static void UseUncheckedVent(PlayerControl sender, int ventId, byte playerId, byte isEnter)
    {
        var player = Helpers.PlayerById(playerId);
        if (player == null)
        {
            return;
        }
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the coroutines cannot be accessed
        MessageReader reader = new();
        var bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.StartAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    [MethodRpc((uint)CustomRPC.UncheckedMurderPlayer)]
    internal static void UncheckedMurderPlayer(PlayerControl sender, byte sourceId, byte targetId, byte showAnimation)
    {
        if (!Helpers.IsGameStarted)
        {
            return;
        }
        var source = Helpers.PlayerById(sourceId);
        var target = Helpers.PlayerById(targetId);
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

    [MethodRpc((uint)CustomRPC.UncheckedExilePlayer)]
    internal static void UncheckedExilePlayer(PlayerControl sender, byte targetId)
    {
        ExilePlayerLocal(targetId);
    }

    internal static void ExilePlayerLocal(byte targetId)
    {
        var target = Helpers.PlayerById(targetId);
        target?.Exiled();
    }

    [MethodRpc((uint)CustomRPC.DynamicMapOption)]
    internal static void DynamicMapOption(PlayerControl sender, byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.SetByte(ByteOptionNames.MapId, mapId);
    }

    [MethodRpc((uint)CustomRPC.ShareGamemode)]
    internal static void ShareGamemode(PlayerControl sender, byte gameMode)
    {
        MapSettings.GameMode = (CustomGamemode)gameMode;
        GameStart.SendGamemode = false;
    }

    [MethodRpc((uint)CustomRPC.SetGameStarting)]
    internal static void SetGameStarting(PlayerControl sender)
    {
        GameStart.StartingTimer = 5f;
    }

    [MethodRpc((uint)CustomRPC.StopStart)]
    internal static void StopStart(PlayerControl sender)
    {
        StopStartSoundLocal();
        if (AmongUsClient.Instance.AmHost)
        {
            FastDestroyableSingleton<GameStartManager>.Instance.ResetStartState();
        }
    }

    [MethodRpc((uint)CustomRPC.StopStartSound)]
    internal static void StopStartSound(PlayerControl sender)
    {
        StopStartSoundLocal();
    }

    private static void StopStartSoundLocal()
    {
        SoundManager.Instance.StopSound(FastDestroyableSingleton<GameStartManager>.Instance.gameStartSound);
    }

    [MethodRpc((uint)CustomRPC.FinishResetVariables)]
    internal static void FinishResetVariables(PlayerControl sender, byte playerId)
    {
        var checkList = RoleAssignment.CheckList;
        if (checkList == null)
        {
            return;
        }
        if (checkList.ContainsKey(playerId))
        {
            checkList[playerId] = true;
        }
    }

    [MethodRpc((uint)CustomRPC.SetLovers)]
    internal static void SetLovers(PlayerControl sender, byte playerId1, byte playerId2)
    {
        Lovers.AddCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
    }

    [MethodRpc((uint)CustomRPC.OverrideNativeRole)]
    internal static void OverrideNativeRole(PlayerControl sender, byte playerId, byte roleType)
    {
        var player = Helpers.PlayerById(playerId);
        player.roleAssigned = false;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
    }

    [MethodRpc((uint)CustomRPC.UncheckedEndGame)]
    internal static void UncheckedEndGame(PlayerControl sender, byte reason, bool isO2Win)
    {
        EndGameMain.IsO2Win = isO2Win;
        AmongUsClient.Instance.GameState = InnerNetClient.GameStates.Ended;
        var obj2 = AmongUsClient.Instance.allClients;
        lock (obj2) AmongUsClient.Instance.allClients.Clear();

        var obj = AmongUsClient.Instance.Dispatcher;
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

    [MethodRpc((uint)CustomRPC.UncheckedSetTasks)]
    internal static void UncheckedSetTasks(PlayerControl sender, byte playerId, byte[] taskTypeIds)
    {
        var player = Helpers.PlayerById(playerId);
        player.ClearAllTasks();

        player.Data.SetTasks(taskTypeIds);
    }

    [MethodRpc((uint)CustomRPC.FinishShipStatusBegin)]
    internal static void FinishShipStatusBegin(PlayerControl sender)
    {
        PlayerControl.LocalPlayer.OnFinishShipStatusBegin();
    }

    private static void ShareRealTasks(MessageReader reader)
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

    [MethodRpc((uint)CustomRPC.PolusRandomSpawn)]
    internal static void PolusRandomSpawn(PlayerControl sender, byte playerId, byte locId)
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
            })));
    }

    [MethodRpc((uint)CustomRPC.Synchronize)]
    internal static void Synchronize(PlayerControl sender, byte playerId, int tag)
    {
        SpawnIn.SynchronizeData.Synchronize((SynchronizeTag)tag, playerId);
    }
<<<<<<< HEAD
}
=======

    internal static void PlaceCamera(float x, float y, byte roomId)
    {
        var sg = SecurityGuard.Instance;

        var referenceCamera = UnityObject.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null)
        {
            return; // Mira HQ
        }

        sg.RemainingScrews -= SecurityGuard.CamPrice;
        sg.PlacedCameras++;

        Vector3 position = new(x, y);

        var roomType = (SystemTypes)roomId;

        var camera = UnityObject.Instantiate(referenceCamera);
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
        var sg = SecurityGuard.Instance;

        Vent vent = null;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        foreach (var v in allVents)
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
            var animator = vent.GetComponent<SpriteAnim>();
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
        var morphPlayer = Helpers.PlayerById(morphId);
        var target = Helpers.PlayerById(playerId);
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
        var player = Helpers.PlayerById(playerId);
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
        var player = Helpers.PlayerById(targetId);
        player.AddModifier(ModifierType.LastImpostor);
    }

    internal static void ShifterShift(byte targetId)
    {
        if (Shifter.Players.Count == 0)
        {
            return;
        }
        var oldShifter = Shifter.Players[0];
        var player = Helpers.PlayerById(targetId);
        if (player == null || oldShifter == null)
        {
            return;
        }

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
        var sheriff = Helpers.PlayerById(sheriffId);
        var target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        var misfire = Sheriff.CheckKill(target);

        using RPCSender killSender = new(sheriff.NetId, CustomRPC.SheriffKill);
        killSender.Write(sheriffId);
        killSender.Write(targetId);
        killSender.Write(misfire);

        SheriffKill(sheriffId, targetId, misfire);
    }

    internal static void SheriffKill(byte sheriffId, byte targetId, bool misfire)
    {
        var sheriff = Helpers.PlayerById(sheriffId);
        var target = Helpers.PlayerById(targetId);
        if (sheriff == null || target == null)
        {
            return;
        }

        var role = Sheriff.GetRole(sheriff);
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
>>>>>>> 167f591 (リバートによる調整)
