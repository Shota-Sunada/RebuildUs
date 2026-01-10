using RebuildUs.Modules;
using AmongUs.GameOptions;

namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
    public static void resetVariables()
    {
        Garlic.clearGarlics();
        JackInTheBox.clearJackInTheBoxes();
        MapOptions.clearAndReloadMapOptions();
        TheOtherRoles.clearAndReloadRoles();
        GameHistory.clearGameHistory();
        setCustomButtonCooldowns();
        AdminPatch.ResetData();
        CameraPatch.ResetData();
        VitalsPatch.ResetData();
        MapBehaviorPatch.reset();
        CustomOverlays.resetOverlays();
        SpecimenVital.clearAndReload();
        AdditionalVents.clearAndReload();
        BombEffect.clearBombEffects();
        Trap.clearAllTraps();
        AssassinTrace.clearTraces();
        SpawnInMinigamePatch.reset();
        MapBehaviorPatch.resetRealTasks();
        CustomNormalPlayerTask.reset();
        Shrine.reset();

        KillAnimationCoPerformKillPatch.hideNextAnimation = false;

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishResetVariables);
        sender.Write(PlayerControl.LocalPlayer.PlayerId);
        finishResetVariables(PlayerControl.LocalPlayer.PlayerId);
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

    public static void workaroundSetRoles(byte numberOfRoles, MessageReader reader)
    {
        for (int i = 0; i < numberOfRoles; i++)
        {
            byte playerId = (byte)reader.ReadPackedUInt32();
            byte roleId = (byte)reader.ReadPackedUInt32();
            try
            {
                setRole(roleId, playerId);
            }
            catch (Exception e)
            {
                Logger.LogError("Error while deserializing roles: " + e.Message);
            }
        }
    }

    public static void setRole(byte roleId, byte playerId)
    {
        Logger.LogInfo($"{GameData.Instance.GetPlayerById(playerId).PlayerName}({playerId}): {Enum.GetName(typeof(ERoleType), roleId)}", "setRole");
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
            x => x.PlayerId == playerId,
            x => x.SetRole((ERoleType)roleId)
        );
    }

    public static void addModifier(byte modId, byte playerId)
    {
        PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
            x => x.PlayerId == playerId,
            x => x.addModifier((ModifierType)modId)
        );
    }

    public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        Version ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
    }

    public static void finishSetRole()
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

    public static void uncheckedCmdReportDeadBody(byte sourceId, byte targetId)
    {
        var source = Helpers.PlayerById(sourceId);
        var target = targetId == byte.MaxValue ? null : Helpers.PlayerById(targetId).Data;
        source?.ReportDeadBody(target);
    }

    public static void useUncheckedVent(int ventId, byte playerId, byte isEnter)
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

        JackInTheBox.startAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    public static void uncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
        var source = Helpers.PlayerById(sourceId);
        var target = Helpers.PlayerById(targetId);
        if (source != null && target != null)
        {
            if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
            source.MurderPlayer(target);
        }
    }

    public static void uncheckedExilePlayer(byte targetId)
    {
        var target = Helpers.PlayerById(targetId);
        target?.Exiled();
    }

    public static void dynamicMapOption(byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.SetByte(ByteOptionNames.MapId, mapId);
    }

    public static void setGameStarting()
    {
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
    }

    public static void shareGamemode(byte gm)
    {
        MapOptions.GameMode = (CustomGamemodes)gm;
        LobbyViewSettingsPatch.currentButtons?.ForEach(x => x.gameObject?.Destroy());
        LobbyViewSettingsPatch.currentButtons?.Clear();
        LobbyViewSettingsPatch.currentButtonTypes?.Clear();
    }

    public static void finishResetVariables(byte playerId)
    {
        var checkList = RoleAssignment.checkList;
        if (checkList != null)
        {
            if (checkList.ContainsKey(playerId))
            {
                checkList[playerId] = true;
            }
        }
    }

    public static void setLovers(byte playerId1, byte playerId2)
    {
        Lovers.addCouple(Helpers.PlayerById(playerId1), Helpers.PlayerById(playerId2));
    }

    public static void overrideNativeRole(byte playerId, byte roleType)
    {
        var player = Helpers.PlayerById(playerId);
        player.roleAssigned = false;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, (RoleTypes)roleType);
    }

    public static void uncheckedEndGame(byte reason)
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

    public static void finishShipStatusBegin()
    {
        PlayerControl.LocalPlayer.OnFinishShipStatusBegin();
    }

    public static void engineerFixLights()
    {
        SwitchSystem switchSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    public static void engineerFixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void engineerUsedRepair()
    {
        Engineer.remainingFixes--;
    }
}