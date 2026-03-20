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
}
