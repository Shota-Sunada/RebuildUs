using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace RebuildUs.Modules;

internal static class RoleAssignment
{
    private const int MAX_BLOCKS = 10;
    internal static bool IsAssigned = false;

    internal static Dictionary<byte, bool> CheckList;

    private static List<byte> _blockLovers = [];
    private static int _blockedAssignments;
    private static readonly List<Tuple<byte, byte>> PlayerRoleMap = [];

    public static IEnumerator CoStartGameHost(AmongUsClient __instance)
    {
        if (__instance == null)
        {
            Logger.LogError("CoStartGameHost aborted: AmongUsClient instance is null.");
            yield break;
        }

        if (LobbyBehaviour.Instance)
        {
            LobbyBehaviour.Instance.Despawn();
        }

        if (!ShipStatus.Instance)
        {
            var index = Mathf.Clamp(ByteOptionNames.MapId.Get(), 0, Constants.MapNames.Length - 1);
            try
            {
                switch (index)
                {
                    case 0 when AprilFoolsMode.ShouldFlipSkeld():
                        index = 3;
                        break;
                    case 3:
                        {
                            if (!AprilFoolsMode.ShouldFlipSkeld())
                            {
                                index = 0;
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("AmongUsClient::CoStartGame: Exception:");
                Logger.LogError(ex);
                Logger.LogError(__instance);
            }

            if (__instance.ShipPrefabs == null || __instance.ShipPrefabs.Count <= index || __instance.ShipPrefabs[index] == null)
            {
                Logger.LogError($"CoStartGameHost aborted: invalid ship prefab index {index}.");
                yield break;
            }

            __instance.ShipLoadingAsyncHandle = __instance.ShipPrefabs[index].InstantiateAsync();
            yield return __instance.ShipLoadingAsyncHandle;
            var result = __instance.ShipLoadingAsyncHandle.Result;
            if (result == null)
            {
                Logger.LogError("CoStartGameHost aborted: ship loading result is null.");
                yield break;
            }

            __instance.ShipLoadingAsyncHandle = new();
            ShipStatus.Instance = result.GetComponent<ShipStatus>();
            if (ShipStatus.Instance == null)
            {
                Logger.LogError("CoStartGameHost aborted: ShipStatus component was not found.");
                yield break;
            }

            __instance.Spawn(ShipStatus.Instance);
        }

        var start = DateTime.Now;
        while (true)
        {
            var flag = true;
            var num = 10;
            var totalSeconds = (float)(DateTime.Now - start).TotalSeconds;
            if (ByteOptionNames.MapId.Get() is 4 or 5)
            {
                num = 15;
            }

            lock (__instance.allClients)
            {
                foreach (var allClient in __instance.allClients)
                {
                    if (allClient.Id == __instance.ClientId || allClient.IsReady)
                    {
                        continue;
                    }
                    if (totalSeconds < num)
                    {
                        flag = false;
                    }
                    else
                    {
                        __instance.SendLateRejection(allClient.Id, DisconnectReasons.ClientTimeout);
                        allClient.IsReady = true;
                        __instance.OnPlayerLeft(allClient, DisconnectReasons.ClientTimeout);
                    }
                }
            }

            if (totalSeconds > 1.0 && totalSeconds < num)
            {
                var loadingBar = FastDestroyableSingleton<LoadingBarManager>.Instance;
                if (loadingBar != null)
                {
                    loadingBar.ToggleLoadingBar(true);
                    loadingBar.SetLoadingPercent((float)((double)totalSeconds / num * 100.0), StringNames.LoadingBarGameStartWaitingPlayers);
                }
            }

            if (!flag)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                break;
            }
        }

        FastDestroyableSingleton<LoadingBarManager>.Instance?.ToggleLoadingBar(false);
        FastDestroyableSingleton<RoleManager>.Instance?.SelectRoles();

        // 独自処理開始
        yield return WaitForLocalPlayer().WrapToIl2Cpp();
        if (PlayerControl.LocalPlayer == null)
        {
            Logger.LogError("Skip custom role assignment because PlayerControl.LocalPlayer is null.");
            BeginShipAndSendClientReady(__instance);
            yield break;
        }

        CreateCheckList();
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ResetVariables);
            RPCProcedure.ResetVariables();
        }
        yield return WaitResetVariables().WrapToIl2Cpp();

        if (!FastDestroyableSingleton<TutorialManager>.InstanceExists
            && CustomOptionHolder.ActivateRoles.GetBool()) // Don't assign Roles in Tutorial or if deactivated
        {
            try
            {
                AssignRoles();
            }
            catch (Exception ex)
            {
                Logger.LogError("AssignRoles failed in CoStartGameHost.");
                Logger.LogError(ex);
            }

            {
                using RPCSender sender2 = new(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishSetRole);
                RPCProcedure.FinishSetRole();
            }
        }
        // 独自処理終了

        BeginShipAndSendClientReady(__instance);
    }

    private static void BeginShipAndSendClientReady(AmongUsClient client)
    {
        var ship = MapUtilities.CachedShipStatus ?? ShipStatus.Instance;
        if (ship == null)
        {
            Logger.LogError("Cannot begin ship: both MapUtilities.CachedShipStatus and ShipStatus.Instance are null.");
        }
        else
        {
            ship.Begin();
        }

        client.SendClientReady();
    }

    private static IEnumerator WaitForLocalPlayer()
    {
        const int timeoutMs = 5000;
        var start = DateTime.UtcNow;
        while (PlayerControl.LocalPlayer == null)
        {
            if ((DateTime.UtcNow - start).TotalMilliseconds > timeoutMs)
            {
                Logger.LogError($"Timeout({timeoutMs}ms): PlayerControl.LocalPlayer is null in CoStartGameHost");
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private static void CreateCheckList()
    {
        CheckList = [];
        if (PlayerControl.AllPlayerControls == null)
        {
            return;
        }

        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.Data == null || player.Data.Disconnected)
            {
                continue;
            }
            CheckList.Add(player.PlayerId, false);
        }
    }

    private static IEnumerator WaitResetVariables()
    {
        Logger.LogInfo("waitResetVariables");
        var check = false;
        const int timeout = 10000;
        var startTime = DateTime.UtcNow;
        while (!check)
        {
            if (CheckList == null)
            {
                Logger.LogError("CheckList is null in WaitResetVariables");
                yield break;
            }

            check = true;
            foreach (var playerId in CheckList.Keys)
            {
                if (CheckList[playerId])
                {
                    continue;
                }
                check = false;
                break;
            }

            yield return new WaitForSeconds(1);
            if (!((DateTime.UtcNow - startTime).TotalMilliseconds > timeout))
            {
                continue;
            }
            Logger.LogInfo($"{(DateTime.UtcNow - startTime).TotalMilliseconds}");
            Logger.LogError($"Timeout({timeout}ms) ResetVariables");
            break;
        }

        Logger.LogInfo("waitResetVariables done.");
    }

    private static void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;
        var rnd = RebuildUs.Rnd;
        while (n > 1)
        {
            n--;
            var k = rnd.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private static void AssignRoles()
    {
        RebuildUs.RefreshRnd((int)DateTime.Now.Ticks);
        // if (CustomOptionHolder.gmEnabled.getBool() && CustomOptionHolder.gmIsHost.getBool())
        // {
        //     var host = AmongUsClient.Instance?.GetHost().Character;
        //     if (host.Data.Role.IsImpostor)
        //     {
        //         var hostIsImpostor = host.Data.Role.IsImpostor;
        //         if (host.Data.Role.IsImpostor)
        //         {
        //             int newImpId = 0;
        //             PlayerControl newImp;
        //             while (true)
        //             {
        //                 newImpId = RebuildUs.Instance.rnd.Next(0, PlayerControl.AllPlayerControls.GetFastEnumerator().Count);
        //                 newImp = PlayerControl.AllPlayerControls.GetFastEnumerator()[newImpId];
        //                 if (newImp == host || newImp.Data.Role.IsImpostor)
        //                 {
        //                     continue;
        //                 }
        //                 break;
        //             }

        //             writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.OverrideNativeRole, Hazel.SendOption.Reliable, -1);
        //             writer.Write(host.PlayerId);
        //             writer.Write((byte)RoleTypes.Crewmate);
        //             AmongUsClient.Instance.FinishRpcImmediately(writer);
        //             RPCProcedure.overrideNativeRole(host.PlayerId, (byte)RoleTypes.Crewmate);

        //             writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.OverrideNativeRole, Hazel.SendOption.Reliable, -1);
        //             writer.Write(newImp.PlayerId);
        //             writer.Write((byte)RoleTypes.Impostor);
        //             AmongUsClient.Instance.FinishRpcImmediately(writer);
        //             RPCProcedure.overrideNativeRole(newImp.PlayerId, (byte)RoleTypes.Impostor);
        //         }
        //     }
        // }

        _blockLovers = [(byte)RoleType.Bait];

        if (!Lovers.HasTasks)
        {
            _blockLovers.Add((byte)RoleType.Snitch);
        }

        if (!CustomOptionHolder.ArsonistCanBeLovers.GetBool())
        {
            _blockLovers.Add((byte)RoleType.Arsonist);
        }

        var data = GetRoleAssignmentData();
        AssignSpecialRoles(data); // Assign special roles like mafia and lovers first as they assign a role to multiple players and the chances are independent of the ticket system
        SelectFactionForFactionIndependentRoles(data);
        AssignEnsuredRoles(data); // Assign roles that should always be in the game next
        AssignChanceRoles(data); // Assign roles that may or may not be in the game last
        AssignRoleTargets(data);
        AssignRoleModifiers(data);
        SetRolesAgain();
    }

    private static RoleAssignmentData GetRoleAssignmentData()
    {
        // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
        List<PlayerControl> crewmates = [];
        List<PlayerControl> impostors = [];

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data == null || p.Data.Disconnected)
            {
                continue;
            }
            if (p.Data.Role.IsImpostor)
            {
                impostors.Add(p);
            }
            else
            {
                crewmates.Add(p);
            }
        }

        Shuffle(crewmates);
        Shuffle(impostors);

        var crewmateMin = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
        var crewmateMax = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
        var neutralMin = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
        var neutralMax = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
        var impostorMin = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
        var impostorMax = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();

        // Make sure min is less or equal to max
        if (crewmateMin > crewmateMax)
        {
            crewmateMin = crewmateMax;
        }
        if (neutralMin > neutralMax)
        {
            neutralMin = neutralMax;
        }
        if (impostorMin > impostorMax)
        {
            impostorMin = impostorMax;
        }

        // Get the maximum allowed count of each role type based on the minimum and maximum option
        var crewCountSettings = RebuildUs.Rnd.Next(crewmateMin, crewmateMax + 1);
        var neutralCountSettings = RebuildUs.Rnd.Next(neutralMin, neutralMax + 1);
        var impCountSettings = RebuildUs.Rnd.Next(impostorMin, impostorMax + 1);

        // Potentially lower the actual maximum to the assignable players
        var maxCrewmateRoles = Mathf.Min(crewmates.Count, crewCountSettings);
        var maxNeutralRoles = Mathf.Min(crewmates.Count, neutralCountSettings);
        var maxImpostorRoles = Mathf.Min(impostors.Count, impCountSettings);

        // Fill in the lists with the roles that should be assigned to players. Note that the special roles (like Mafia or Lovers) are NOT included in these lists
        Dictionary<byte, (int rate, int count)> impSettings = [];
        Dictionary<byte, (int rate, int count)> neutralSettings = [];
        Dictionary<byte, (int rate, int count)> crewSettings = [];

        Logger.LogMessage("Initializing Role Data");
        foreach (var role in RoleData.Roles)
        {
            if (role.GetOption == null || role.GetOption() is not CustomRoleOption roleOption)
            {
                continue;
            }
            switch (role.RoleType)
            {
                // ここで例外的な役職を個別に弾く
                case RoleType.Godfather
                     or RoleType.Mafioso
                     or RoleType.Janitor
                     or RoleType.NiceGuesser
                     or RoleType.EvilGuesser
                     or RoleType.NiceSwapper
                     or RoleType.EvilSwapper
                     or RoleType.Shifter
                     or RoleType.Lovers
                     or RoleType.Sidekick:
                // Spyはインポスターが1人以下の時は出現しない
                case RoleType.Spy when impostors.Count <= 1:
                    continue;
            }

            switch (role.RoleTeam)
            {
                case RoleTeam.Crewmate:
                    crewSettings.TryAdd((byte)role.RoleType, roleOption.Data);
                    break;
                case RoleTeam.Impostor:
                    impSettings.TryAdd((byte)role.RoleType, roleOption.Data);
                    break;
                case RoleTeam.Neutral:
                    neutralSettings.TryAdd((byte)role.RoleType, roleOption.Data);
                    break;
            }
        }

        return new()
        {
            Crewmates = crewmates,
            Impostors = impostors,
            CrewSettings = crewSettings,
            NeutralSettings = neutralSettings,
            ImpSettings = impSettings,
            MaxCrewmateRoles = maxCrewmateRoles,
            MaxNeutralRoles = maxNeutralRoles,
            MaxImpostorRoles = maxImpostorRoles,
        };
    }

    private static void AssignSpecialRoles(RoleAssignmentData data)
    {
        // Assign GM
        // if (CustomOptionHolder.gmEnabled.getBool() == true)
        // {
        //     byte gmID = 0;

        //     if (CustomOptionHolder.gmIsHost.getBool() == true)
        //     {
        //         PlayerControl host = AmongUsClient.Instance?.GetHost().Character;
        //         gmID = setRoleToHost((byte)ERoleType.GM, host);

        //         // First, remove the GM from role selection.
        //         data.crewmates.RemoveAll(x => x.PlayerId == host.PlayerId);
        //         data.impostors.RemoveAll(x => x.PlayerId == host.PlayerId);

        //     }
        //     else
        //     {
        //         gmID = setRoleToRandomPlayer((byte)ERoleType.GM, data.crewmates);
        //     }

        //     PlayerControl p = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().Find(x => x.PlayerId == gmID);

        //     if (p != null && CustomOptionHolder.gmDiesAtStart.getBool())
        //     {
        //         p.Exiled();
        //     }
        // }

        // Assign Lovers
        if (CustomOptionHolder.LoversSpawnRate.Enabled)
        {
            for (var i = 0; i < CustomOptionHolder.LoversNumCouples.GetFloat(); i++)
            {
                var singleCrew = data.Crewmates.FindAll(x => !x.IsLovers());
                var singleImps = data.Impostors.FindAll(x => !x.IsLovers());

                var isOnlyRole = !CustomOptionHolder.LoversCanHaveAnotherRole.GetBool();
                if (RebuildUs.Rnd.Next(1, 101) > CustomOptionHolder.LoversSpawnRate.GetSelection() * 10)
                {
                    continue;
                }

                var lover1 = -1;
                var lover2 = -1;
                var lover1Index = -1;
                var lover2Index = -1;
                if (singleImps.Count > 0
                    && singleCrew.Count > 0
                    && (!isOnlyRole || data.MaxCrewmateRoles > 0 && data.MaxImpostorRoles > 0)
                    && RebuildUs.Rnd.Next(1, 101) <= CustomOptionHolder.LoversImpLoverRate.GetSelection() * 10)
                {
                    lover1Index = RebuildUs.Rnd.Next(0, singleImps.Count);
                    lover1 = singleImps[lover1Index].PlayerId;

                    lover2Index = RebuildUs.Rnd.Next(0, singleCrew.Count);
                    lover2 = singleCrew[lover2Index].PlayerId;

                    if (isOnlyRole)
                    {
                        data.MaxImpostorRoles--;
                        data.MaxCrewmateRoles--;

                        data.Impostors.RemoveAll(x => x.PlayerId == lover1);
                        data.Crewmates.RemoveAll(x => x.PlayerId == lover2);
                    }
                }
                else if (singleCrew.Count >= 2 && (isOnlyRole || data.MaxCrewmateRoles >= 2))
                {
                    lover1Index = RebuildUs.Rnd.Next(0, singleCrew.Count);
                    while (lover2Index == lover1Index || lover2Index < 0)
                    {
                        lover2Index = RebuildUs.Rnd.Next(0, singleCrew.Count);
                    }

                    lover1 = singleCrew[lover1Index].PlayerId;
                    lover2 = singleCrew[lover2Index].PlayerId;

                    if (isOnlyRole)
                    {
                        data.MaxCrewmateRoles -= 2;
                        data.Crewmates.RemoveAll(x => x.PlayerId == lover1);
                        data.Crewmates.RemoveAll(x => x.PlayerId == lover2);
                    }
                }

                if (lover1 < 0 || lover2 < 0)
                {
                    continue;
                }
                using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetLovers))
                {
                    sender.Write((byte)lover1);
                    sender.Write((byte)lover2);
                }

                RPCProcedure.SetLovers((byte)lover1, (byte)lover2);
            }
        }

        // Assign Mafia
        if (data.Impostors.Count < 3
            || data.MaxImpostorRoles < 3
            || RebuildUs.Rnd.Next(1, 101) > CustomOptionHolder.MafiaSpawnRate.GetSelection() * 10)
        {
            return;
        }

        SetRoleToRandomPlayer((byte)RoleType.Godfather, data.Impostors);
        SetRoleToRandomPlayer((byte)RoleType.Janitor, data.Impostors);
        SetRoleToRandomPlayer((byte)RoleType.Mafioso, data.Impostors);
        data.MaxImpostorRoles -= 3;
    }

    private static void SelectFactionForFactionIndependentRoles(RoleAssignmentData data)
    {
        // Assign Guesser (chance to be impostor based on setting)
        var isEvilGuesser = RebuildUs.Rnd.Next(1, 101) <= CustomOptionHolder.GuesserIsImpGuesserRate.GetSelection() * 10;
        if (CustomOptionHolder.GuesserSpawnBothRate.GetSelection() > 0)
        {
            if (RebuildUs.Rnd.Next(1, 101) <= CustomOptionHolder.GuesserSpawnRate.GetSelection() * 10)
            {
                if (isEvilGuesser)
                {
                    if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0)
                    {
                        var evilGuesser = SetRoleToRandomPlayer((byte)RoleType.EvilGuesser, data.Impostors);
                        data.Impostors.RemoveAll(x => x.PlayerId == evilGuesser);
                        data.MaxImpostorRoles--;
                        data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.GuesserSpawnBothRate.GetSelection(), 1));
                    }
                }
                else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
                {
                    var niceGuesser = SetRoleToRandomPlayer((byte)RoleType.NiceGuesser, data.Crewmates);
                    data.Crewmates.RemoveAll(x => x.PlayerId == niceGuesser);
                    data.MaxCrewmateRoles--;
                    data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.GuesserSpawnBothRate.GetSelection(), 1));
                }
            }
        }
        else
        {
            if (isEvilGuesser)
            {
                data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.GuesserSpawnRate.GetSelection(), 1));
            }
            else
            {
                data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.GuesserSpawnRate.GetSelection(), 1));
            }
        }

        // Assign Swapper (chance to be impostor based on setting)
        if (data.Impostors.Count > 0
            && data.MaxImpostorRoles > 0
            && RebuildUs.Rnd.Next(1, 101) <= CustomOptionHolder.SwapperIsImpRate.GetSelection() * 10)
        {
            data.ImpSettings.Add((byte)RoleType.EvilSwapper, (CustomOptionHolder.SwapperSpawnRate.GetSelection(), 1));
        }
        else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
        {
            data.CrewSettings.Add((byte)RoleType.NiceSwapper, (CustomOptionHolder.SwapperSpawnRate.GetSelection(), 1));
        }

        // Assign Shifter (chance to be neutral based on setting)
        var shifterIsNeutral = false;
        switch (data.Crewmates.Count)
        {
            case > 0 when data.MaxNeutralRoles > 0 && RebuildUs.Rnd.Next(1, 101) <= CustomOptionHolder.ShifterIsNeutralRate.GetSelection() * 10:
                data.NeutralSettings.Add((byte)RoleType.Shifter, (CustomOptionHolder.ShifterSpawnRate.GetSelection(), 1));
                shifterIsNeutral = true;
                break;
            case > 0 when data.MaxCrewmateRoles > 0:
                data.CrewSettings.Add((byte)RoleType.Shifter, (CustomOptionHolder.ShifterSpawnRate.GetSelection(), 1));
                break;
        }

        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetShifterType)) sender.Write(shifterIsNeutral);

        RPCProcedure.SetShifterType(shifterIsNeutral);
    }

    private static void AssignEnsuredRoles(RoleAssignmentData data)
    {
        _blockedAssignments = 0;

        // Get all roles where the chance to occur is set to 100%
        List<byte> ensuredCrewmateRoles = [];
        foreach (var kvp in data.CrewSettings)
        {
            if (kvp.Value.rate != 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.count; i++)
            {
                ensuredCrewmateRoles.Add(kvp.Key);
            }
        }

        List<byte> ensuredNeutralRoles = [];
        foreach (var kvp in data.NeutralSettings)
        {
            if (kvp.Value.rate != 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.count; i++)
            {
                ensuredNeutralRoles.Add(kvp.Key);
            }
        }

        List<byte> ensuredImpostorRoles = [];
        foreach (var kvp in data.ImpSettings)
        {
            if (kvp.Value.rate != 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.count; i++)
            {
                ensuredImpostorRoles.Add(kvp.Key);
            }
        }

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0
               || data.Crewmates.Count > 0
               && (data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0 || data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0))
        {
            List<TeamType> availableTeams = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0)
            {
                availableTeams.Add(TeamType.Crewmate);
            }
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
            {
                availableTeams.Add(TeamType.Neutral);
            }
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0)
            {
                availableTeams.Add(TeamType.Impostor);
            }

            var roleType = availableTeams[RebuildUs.Rnd.Next(0, availableTeams.Count)];

            List<byte> selectedRoles;
            List<PlayerControl> players;
            switch (roleType)
            {
                case TeamType.Crewmate:
                    selectedRoles = ensuredCrewmateRoles;
                    players = data.Crewmates;
                    break;
                case TeamType.Neutral:
                    selectedRoles = ensuredNeutralRoles;
                    players = data.Crewmates;
                    break;
                default:
                    selectedRoles = ensuredImpostorRoles;
                    players = data.Impostors;
                    break;
            }

            var index = RebuildUs.Rnd.Next(0, selectedRoles.Count);
            var roleId = selectedRoles[index];
            var player = SetRoleToRandomPlayer(roleId, players);

            if (player == byte.MaxValue && _blockedAssignments < MAX_BLOCKS)
            {
                _blockedAssignments++;
                continue;
            }

            _blockedAssignments = 0;

            selectedRoles.RemoveAt(index);

            if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out var blockedRoles))
            {
                foreach (var blockedRoleId in blockedRoles)
                {
                    // Set chance for the blocked roles to 0 for chances less than 100%
                    if (data.ImpSettings.ContainsKey(blockedRoleId))
                    {
                        data.ImpSettings[blockedRoleId] = (0, 0);
                    }
                    if (data.NeutralSettings.ContainsKey(blockedRoleId))
                    {
                        data.NeutralSettings[blockedRoleId] = (0, 0);
                    }
                    if (data.CrewSettings.ContainsKey(blockedRoleId))
                    {
                        data.CrewSettings[blockedRoleId] = (0, 0);
                    }

                    // Remove blocked roles even if the chance was 100%
                    ensuredCrewmateRoles.RemoveAll(x => x == blockedRoleId);
                    ensuredNeutralRoles.RemoveAll(x => x == blockedRoleId);
                    ensuredImpostorRoles.RemoveAll(x => x == blockedRoleId);
                }
            }

            // Adjust the role limit
            switch (roleType)
            {
                case TeamType.Crewmate:
                    data.MaxCrewmateRoles--;
                    break;
                case TeamType.Neutral:
                    data.MaxNeutralRoles--;
                    break;
                case TeamType.Impostor:
                    data.MaxImpostorRoles--;
                    break;
            }
        }
    }

    private static void AssignChanceRoles(RoleAssignmentData data)
    {
        _blockedAssignments = 0;

        // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
        List<byte> crewmateTickets = [];
        foreach (var kvp in data.CrewSettings)
        {
            if (kvp.Value.rate is <= 0 or >= 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
            {
                crewmateTickets.Add(kvp.Key);
            }
        }

        List<byte> neutralTickets = [];
        foreach (var kvp in data.NeutralSettings)
        {
            if (kvp.Value.rate is <= 0 or >= 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
            {
                neutralTickets.Add(kvp.Key);
            }
        }

        List<byte> impostorTickets = [];
        foreach (var kvp in data.ImpSettings)
        {
            if (kvp.Value.rate is <= 0 or >= 10)
            {
                continue;
            }
            for (var i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
            {
                impostorTickets.Add(kvp.Key);
            }
        }

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0
               || data.Crewmates.Count > 0
               && (data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0 || data.MaxNeutralRoles > 0 && neutralTickets.Count > 0))
        {
            List<TeamType> availableTeams = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0)
            {
                availableTeams.Add(TeamType.Crewmate);
            }
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && neutralTickets.Count > 0)
            {
                availableTeams.Add(TeamType.Neutral);
            }
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0)
            {
                availableTeams.Add(TeamType.Impostor);
            }

            var roleType = availableTeams[RebuildUs.Rnd.Next(0, availableTeams.Count)];

            List<byte> selectedTickets;
            List<PlayerControl> players;
            switch (roleType)
            {
                case TeamType.Crewmate:
                    selectedTickets = crewmateTickets;
                    players = data.Crewmates;
                    break;
                case TeamType.Neutral:
                    selectedTickets = neutralTickets;
                    players = data.Crewmates;
                    break;
                default:
                    selectedTickets = impostorTickets;
                    players = data.Impostors;
                    break;
            }

            var index = RebuildUs.Rnd.Next(0, selectedTickets.Count);
            var roleId = selectedTickets[index];
            var player = SetRoleToRandomPlayer(roleId, players);

            if (player == byte.MaxValue && _blockedAssignments < MAX_BLOCKS)
            {
                _blockedAssignments++;
                continue;
            }

            _blockedAssignments = 0;

            selectedTickets.RemoveAll(x => x == roleId);

            if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out var blockedRoles))
            {
                foreach (var blockedRoleId in blockedRoles)
                {
                    // Remove tickets of blocked roles from all pools
                    crewmateTickets.RemoveAll(x => x == blockedRoleId);
                    neutralTickets.RemoveAll(x => x == blockedRoleId);
                    impostorTickets.RemoveAll(x => x == blockedRoleId);
                }
            }

            // Adjust the role limit
            switch (roleType)
            {
                case TeamType.Crewmate:
                    data.MaxCrewmateRoles--;
                    break;
                case TeamType.Neutral:
                    data.MaxNeutralRoles--;
                    break;
                case TeamType.Impostor:
                    data.MaxImpostorRoles--;
                    break;
            }
        }
    }

    private static byte SetRoleToHost(byte roleId, PlayerControl host)
    {
        var playerId = host.PlayerId;

        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetRole);
        sender.Write(roleId);
        sender.Write(playerId);
        RPCProcedure.SetRole(roleId, playerId);

        return playerId;
    }

    private static void AssignRoleTargets(RoleAssignmentData data) { }

    private static void AssignRoleModifiers(RoleAssignmentData data)
    {
        // // Madmate
        // for (int i = 0; i < CustomOptionHolder.madmateSpawnRate.count; i++)
        // {
        //     if (rnd.Next(1, 100) <= CustomOptionHolder.madmateSpawnRate.rate * 10)
        //     {
        //         var candidates = Madmate.candidates;
        //         if (candidates.Count <= 0)
        //         {
        //             break;
        //         }

        //         if (Madmate.madmateType == Madmate.MadmateType.Simple)
        //         {
        //             if (data.maxCrewmateRoles <= 0) break;
        //             setModifierToRandomPlayer((byte)ModifierType.Madmate, Madmate.candidates);
        //             data.maxCrewmateRoles--;
        //         }
        //         else
        //         {
        //             setModifierToRandomPlayer((byte)ModifierType.Madmate, Madmate.candidates);
        //         }
        //     }
        // }
        // // Munou
        // for (int i = 0; i < CustomOptionHolder.munouSpawnRate.count; i++)
        // {
        //     if (rnd.Next(1, 100) <= CustomOptionHolder.munouSpawnRate.rate * 10)
        //     {
        //         var candidates = Munou.candidates;
        //         if (candidates.Count <= 0)
        //         {
        //             break;
        //         }

        //         if (Munou.munouType == Munou.MunouType.Simple)
        //         {
        //             if (data.maxCrewmateRoles <= 0) break;
        //             setModifierToRandomPlayer((byte)ModifierType.Munou, Munou.candidates);
        //             data.maxCrewmateRoles--;
        //         }
        //         else
        //         {
        //             setModifierToRandomPlayer((byte)ModifierType.Munou, Munou.candidates);
        //         }
        //     }
        // }
        // // AntiTeleport
        // for (int i = 0; i < CustomOptionHolder.antiTeleportSpawnRate.count; i++)
        // {
        //     if (rnd.Next(1, 100) <= CustomOptionHolder.antiTeleportSpawnRate.rate * 10)
        //     {
        //         var candidates = AntiTeleport.candidates;
        //         if (candidates.Count <= 0)
        //         {
        //             break;
        //         }
        //         setModifierToRandomPlayer((byte)ModifierType.AntiTeleport, AntiTeleport.candidates);
        //     }
        // }
        // // Mini
        // for (int i = 0; i < CustomOptionHolder.miniSpawnRate.count; i++)
        // {
        //     if (rnd.Next(1, 100) <= CustomOptionHolder.miniSpawnRate.rate * 10)
        //     {
        //         var candidates = Mini.candidates;
        //         if (candidates.Count <= 0)
        //         {
        //             break;
        //         }
        //         setModifierToRandomPlayer((byte)ModifierType.Mini, Mini.candidates);
        //     }
        // }
    }

    private static byte SetRoleToRandomPlayer(byte roleId, IList<PlayerControl> playerList, bool removePlayer = true)
    {
        var index = RebuildUs.Rnd.Next(0, playerList.Count);
        var playerId = playerList[index].PlayerId;
        if (Helpers.RolesEnabled
            && (CustomOptionHolder.LoversSpawnRate == null || CustomOptionHolder.LoversSpawnRate.Enabled)
            && Helpers.PlayerById(playerId)?.IsLovers() == true
            && _blockLovers.Contains(roleId))
        {
            return byte.MaxValue;
        }

        if (removePlayer)
        {
            playerList.RemoveAt(index);
        }
        PlayerRoleMap.Add(new(playerId, roleId));

        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SetRole);
        sender.Write(roleId);
        sender.Write(playerId);
        RPCProcedure.SetRole(roleId, playerId);

        return playerId;
    }

    private static byte SetModifierToRandomPlayer(byte modId, IReadOnlyList<PlayerControl> playerList)
    {
        if (playerList.Count <= 0)
        {
            return byte.MaxValue;
        }

        var index = RebuildUs.Rnd.Next(0, playerList.Count);
        var playerId = playerList[index].PlayerId;

        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.AddModifier);
        sender.Write(modId);
        sender.Write(playerId);
        RPCProcedure.AddModifier(modId, playerId);

        return playerId;
    }

    private static void SetRolesAgain()
    {
        while (PlayerRoleMap.Count > 0)
        {
            var amount = (byte)Math.Min(PlayerRoleMap.Count, 20);
            var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.WorkaroundSetRoles,
                SendOption.Reliable);
            writer.Write(amount);
            for (var i = 0; i < amount; i++)
            {
                var option = PlayerRoleMap[0];
                PlayerRoleMap.RemoveAt(0);
                writer.WritePacked((uint)option.Item1);
                writer.WritePacked((uint)option.Item2);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    private sealed class RoleAssignmentData
    {
        internal Dictionary<byte, (int rate, int count)> CrewSettings = [];
        internal Dictionary<byte, (int rate, int count)> ImpSettings = [];
        internal Dictionary<byte, (int rate, int count)> NeutralSettings = [];
        internal List<PlayerControl> Crewmates { get; set; }
        internal List<PlayerControl> Impostors { get; set; }
        internal int MaxCrewmateRoles { get; set; }
        internal int MaxNeutralRoles { get; set; }
        internal int MaxImpostorRoles { get; set; }
    }

    private enum TeamType
    {
        Crewmate = 0,
        Neutral = 1,
        Impostor = 2,
    }
}