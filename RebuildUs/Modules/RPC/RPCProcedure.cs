using RebuildUs.Modules;
using AmongUs.GameOptions;
using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;
using RebuildUs.Objects;

namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
    public static void ResetVariables()
    {
        RoleAssignment.IsAssigned = false;
        Garlic.ClearGarlics();
        JackInTheBox.ClearJackInTheBoxes();
        MapOptions.ClearAndReloadMapOptions();
        RebuildUs.ClearAndReloadRoles();
        GameHistory.ClearGameHistory();
        // setCustomButtonCooldowns();
        // AdminPatch.ResetData();
        // CameraPatch.ResetData();
        // VitalsPatch.ResetData();
        // MapBehaviorPatch.reset();
        // CustomOverlays.resetOverlays();
        // SpecimenVital.clearAndReload();
        // AdditionalVents.clearAndReload();
        // BombEffect.clearBombEffects();
        // Trap.clearAllTraps();
        // AssassinTrace.clearTraces();
        // SpawnInMinigamePatch.reset();
        // MapBehaviorPatch.resetRealTasks();
        // CustomNormalPlayerTask.reset();

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
        Logger.LogInfo($"{GameData.Instance.GetPlayerById(playerId).PlayerName}({playerId}): {Enum.GetName(typeof(ERoleType), roleId)}", "setRole");
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
            x => x.PlayerId == playerId,
            x => x.SetRole((ERoleType)roleId)
        );
    }

    public static void AddModifier(byte modId, byte playerId)
    {
        // PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
        //     x => x.PlayerId == playerId,
        //     x => x.addModifier((ModifierType)modId)
        // );
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        var ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStart.PlayerVersions[clientId] = new(ver, guid);
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

    public static void ShareGamemode(byte gm)
    {
        MapOptions.GameMode = (CustomGamemodes)gm;
        CustomOption.SettingsPaneCurrentButtons?.ForEach(x => x.gameObject?.Destroy());
        CustomOption.SettingsPaneCurrentButtons?.Clear();
        CustomOption.SettingsPaneCurrentButtonTypes?.Clear();
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
        var livingPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(p => !p.IsRole(ERoleType.Arsonist) && p.IsAlive());
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
            var wasSpy = target.IsRole(ERoleType.Spy);
            var wasImpostor = target.IsTeamImpostor(); // This can only be reached if impostors can be sidekicked.
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
            ErasePlayerRoles(target.PlayerId, true);
            target.SetRole(ERoleType.Sidekick);
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
        sidekickPlayer.SetRole(ERoleType.Jackal);
        var newJackal = Jackal.GetRole(sidekickPlayer);
        newJackal.CanSidekick = Jackal.JackalPromotedFromSidekickCanCreateSidekick;
        newJackal.WasTeamRed = wasTeamRed;
        newJackal.WasImpostor = wasImpostor;
        newJackal.WasSpy = wasSpy;
        Sidekick.Clear();
        return;
    }
}