using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RebuildUs.Modules;

public static class RoleAssignment
{
    public static bool IsAssigned = false;

    public static IEnumerator CoStartGameHost(AmongUsClient __instance)
    {
        if (LobbyBehaviour.Instance)
        {
            LobbyBehaviour.Instance.Despawn();
        }

        if (!ShipStatus.Instance)
        {
            var index = Mathf.Clamp(Helpers.GetOption(ByteOptionNames.MapId), 0, Constants.MapNames.Length - 1);
            try
            {
                if (index == 0 && AprilFoolsMode.ShouldFlipSkeld())
                {
                    index = 3;
                }
                else if (index == 3)
                {
                    if (!AprilFoolsMode.ShouldFlipSkeld())
                    {
                        index = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("AmongUsClient::CoStartGame: Exception:");
                var client2 = __instance;
                Logger.LogError(ex);
                Logger.LogError(client2);
            }
            __instance.ShipLoadingAsyncHandle = __instance.ShipPrefabs[index].InstantiateAsync(null, false);
            yield return __instance.ShipLoadingAsyncHandle;
            var result = __instance.ShipLoadingAsyncHandle.Result;
            __instance.ShipLoadingAsyncHandle = new AsyncOperationHandle<GameObject>();
            ShipStatus.Instance = result.GetComponent<ShipStatus>();
            __instance.Spawn(ShipStatus.Instance);
        }

        var start = DateTime.Now;
        while (true)
        {
            var flag = true;
            var num = 10;
            var totalSeconds = (float)(DateTime.Now - start).TotalSeconds;
            if (Helpers.GetOption(ByteOptionNames.MapId) is 4 or 5)
            {
                num = 15;
            }
            lock (__instance.allClients)
            {
                for (int index = 0; index < __instance.allClients.Count; ++index)
                {
                    var allClient = __instance.allClients[index];
                    if (allClient.Id != __instance.ClientId && !allClient.IsReady)
                    {
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
            }
            if (totalSeconds > 1.0 && totalSeconds < num)
            {
                DestroyableSingleton<LoadingBarManager>.Instance.ToggleLoadingBar(true);
                DestroyableSingleton<LoadingBarManager>.Instance.SetLoadingPercent((float)((double)totalSeconds / num * 100.0), StringNames.LoadingBarGameStartWaitingPlayers);
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
        DestroyableSingleton<LoadingBarManager>.Instance.ToggleLoadingBar(false);
        DestroyableSingleton<RoleManager>.Instance.SelectRoles();

        // 独自処理開始
        CreateCheckList();
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ResetVariables);
            RPCProcedure.ResetVariables();
        }
        yield return WaitResetVariables().WrapToIl2Cpp();

        if (!DestroyableSingleton<TutorialManager>.InstanceExists && CustomOptionHolder.ActivateRoles.GetBool()) // Don't assign Roles in Tutorial or if deactivated
        {
            AssignRoles();
            {
                using var sender2 = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishSetRole);
                RPCProcedure.FinishSetRole();
            }
        }
        // 独自処理終了

        MapUtilities.CachedShipStatus.Begin();
        __instance.SendClientReady();

        yield break;
    }
    public static Dictionary<byte, bool> CheckList;
    private static void CreateCheckList()
    {
        CheckList = [];
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.Data == null || player.Data.Disconnected) continue;
            CheckList.Add(player.PlayerId, false);
        }
    }

    public static IEnumerator WaitResetVariables()
    {
        Logger.LogInfo("waitResetVariables");
        bool check = false;
        int timeout = 10000;
        DateTime startTime = DateTime.UtcNow;
        while (!check)
        {
            check = true;
            foreach (var playerId in CheckList.Keys)
            {
                if (!CheckList[playerId])
                {
                    check = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1);
            if ((DateTime.UtcNow - startTime).TotalMilliseconds > timeout)
            {
                Logger.LogInfo($"{(DateTime.UtcNow - startTime).TotalMilliseconds}");
                Logger.LogError($"Timeout({timeout}ms) ResetVariables");
                break;
            }
        }
        Logger.LogInfo("waitResetVariables done.");
        yield break;
    }

    private static List<byte> BlockLovers = [];
    public static int BlockedAssignments = 0;
    public static int MaxBlocks = 10;
    private static readonly List<Tuple<byte, byte>> PlayerRoleMap = [];

    private static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        var rnd = RebuildUs.Instance.Rnd;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public static void AssignRoles()
    {
        RebuildUs.Instance.RefreshRnd((int)DateTime.Now.Ticks);
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

        BlockLovers = [(byte)RoleType.Bait];

        if (!Lovers.HasTasks)
        {
            BlockLovers.Add((byte)RoleType.Snitch);
        }

        if (!CustomOptionHolder.ArsonistCanBeLovers.GetBool())
        {
            BlockLovers.Add((byte)RoleType.Arsonist);
        }

        switch (MapSettings.GameMode)
        {
            case CustomGameMode.Roles:
                var data = GetRoleAssignmentData();
                AssignSpecialRoles(data); // Assign special roles like mafia and lovers first as they assign a role to multiple players and the chances are independent of the ticket system
                SelectFactionForFactionIndependentRoles(data);
                AssignEnsuredRoles(data); // Assign roles that should always be in the game next
                AssignChanceRoles(data); // Assign roles that may or may not be in the game last
                AssignRoleTargets(data);
                AssignRoleModifiers(data);
                SetRolesAgain();
                break;
            case CustomGameMode.CaptureTheFlag:
                AssignCaptureTheFlagRoles();
                break;
            case CustomGameMode.PoliceAndThieves:
                AssignPoliceAndThievesRoles();
                break;
            case CustomGameMode.HotPotato:
                AssignHotPotatoRoles();
                break;
            case CustomGameMode.BattleRoyale:
                AssignBattleRoyaleRoles();
                break;
        }
    }

    public static RoleAssignmentData GetRoleAssignmentData()
    {
        // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
        List<PlayerControl> crewmates = [];
        List<PlayerControl> impostors = [];

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data == null || p.Data.Disconnected) continue;
            if (p.Data.Role.IsImpostor) impostors.Add(p);
            else crewmates.Add(p);
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
        if (crewmateMin > crewmateMax) crewmateMin = crewmateMax;
        if (neutralMin > neutralMax) neutralMin = neutralMax;
        if (impostorMin > impostorMax) impostorMin = impostorMax;

        // Get the maximum allowed count of each role type based on the minimum and maximum option
        int crewCountSettings = RebuildUs.Instance.Rnd.Next(crewmateMin, crewmateMax + 1);
        int neutralCountSettings = RebuildUs.Instance.Rnd.Next(neutralMin, neutralMax + 1);
        int impCountSettings = RebuildUs.Instance.Rnd.Next(impostorMin, impostorMax + 1);

        // Potentially lower the actual maximum to the assignable players
        int maxCrewmateRoles = Mathf.Min(crewmates.Count, crewCountSettings);
        int maxNeutralRoles = Mathf.Min(crewmates.Count, neutralCountSettings);
        int maxImpostorRoles = Mathf.Min(impostors.Count, impCountSettings);

        // Fill in the lists with the roles that should be assigned to players. Note that the special roles (like Mafia or Lovers) are NOT included in these lists
        Dictionary<byte, (int rate, int count)> impSettings = [];
        Dictionary<byte, (int rate, int count)> neutralSettings = [];
        Dictionary<byte, (int rate, int count)> crewSettings = [];

        Logger.LogMessage("Initializing Role Data");
        foreach (var role in RoleData.Roles)
        {
            if (role.getOption != null && role.getOption() is CustomRoleOption roleOption)
            {
                // ここで例外的な役職を個別に弾く
                if (role.roleType is
                    RoleType.Godfather or RoleType.Mafioso or RoleType.Janitor or
                    RoleType.NiceGuesser or RoleType.EvilGuesser or
                    RoleType.NiceSwapper or RoleType.EvilSwapper or
                    RoleType.Shifter or
                    RoleType.Lovers or RoleType.Sidekick) continue;

                // Spyはインポスターが1人以下の時は出現しない
                if (role.roleType == RoleType.Spy && impostors.Count <= 1) continue;

                if (role.roleTeam == RoleTeam.Crewmate)
                {
                    crewSettings.TryAdd((byte)role.roleType, roleOption.Data);
                }
                else if (role.roleTeam == RoleTeam.Impostor)
                {
                    impSettings.TryAdd((byte)role.roleType, roleOption.Data);
                }
                else if (role.roleTeam == RoleTeam.Neutral)
                {
                    neutralSettings.TryAdd((byte)role.roleType, roleOption.Data);
                }
            }
        }

        return new RoleAssignmentData
        {
            Crewmates = crewmates,
            Impostors = impostors,
            CrewSettings = crewSettings,
            NeutralSettings = neutralSettings,
            ImpSettings = impSettings,
            MaxCrewmateRoles = maxCrewmateRoles,
            MaxNeutralRoles = maxNeutralRoles,
            MaxImpostorRoles = maxImpostorRoles
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
        //         gmID = SetRoleToRandomPlayer((byte)ERoleType.GM, data.crewmates);
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
            for (int i = 0; i < CustomOptionHolder.LoversNumCouples.GetFloat(); i++)
            {
                var singleCrew = data.Crewmates.FindAll(x => !x.IsLovers());
                var singleImps = data.Impostors.FindAll(x => !x.IsLovers());

                bool isOnlyRole = !CustomOptionHolder.LoversCanHaveAnotherRole.GetBool();
                if (RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.LoversSpawnRate.GetSelection() * 10)
                {
                    int lover1 = -1;
                    int lover2 = -1;
                    int lover1Index = -1;
                    int lover2Index = -1;
                    if (singleImps.Count > 0 && singleCrew.Count > 0 && (!isOnlyRole || (data.MaxCrewmateRoles > 0 && data.MaxImpostorRoles > 0)) && RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.LoversImpLoverRate.GetSelection() * 10)
                    {
                        lover1Index = RebuildUs.Instance.Rnd.Next(0, singleImps.Count);
                        lover1 = singleImps[lover1Index].PlayerId;

                        lover2Index = RebuildUs.Instance.Rnd.Next(0, singleCrew.Count);
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
                        lover1Index = RebuildUs.Instance.Rnd.Next(0, singleCrew.Count);
                        while (lover2Index == lover1Index || lover2Index < 0) lover2Index = RebuildUs.Instance.Rnd.Next(0, singleCrew.Count);

                        lover1 = singleCrew[lover1Index].PlayerId;
                        lover2 = singleCrew[lover2Index].PlayerId;

                        if (isOnlyRole)
                        {
                            data.MaxCrewmateRoles -= 2;
                            data.Crewmates.RemoveAll(x => x.PlayerId == lover1);
                            data.Crewmates.RemoveAll(x => x.PlayerId == lover2);
                        }
                    }

                    if (lover1 >= 0 && lover2 >= 0)
                    {
                        using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetLovers))
                        {
                            sender.Write((byte)lover1);
                            sender.Write((byte)lover2);
                        }
                        RPCProcedure.SetLovers((byte)lover1, (byte)lover2);
                    }
                }
            }
        }

        // Assign Mafia
        if (data.Impostors.Count >= 3 && data.MaxImpostorRoles >= 3 && (RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.MafiaSpawnRate.GetSelection() * 10))
        {
            SetRoleToRandomPlayer((byte)RoleType.Godfather, data.Impostors);
            SetRoleToRandomPlayer((byte)RoleType.Janitor, data.Impostors);
            SetRoleToRandomPlayer((byte)RoleType.Mafioso, data.Impostors);
            data.MaxImpostorRoles -= 3;
        }
    }

    private static void SelectFactionForFactionIndependentRoles(RoleAssignmentData data)
    {
        // Assign Guesser (chance to be impostor based on setting)
        bool isEvilGuesser = RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.GuesserIsImpGuesserRate.GetSelection() * 10;
        if (CustomOptionHolder.GuesserSpawnBothRate.GetSelection() > 0)
        {
            if (RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.GuesserSpawnRate.GetSelection() * 10)
            {
                if (isEvilGuesser)
                {
                    if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0)
                    {
                        byte evilGuesser = SetRoleToRandomPlayer((byte)RoleType.EvilGuesser, data.Impostors);
                        data.Impostors.RemoveAll(x => x.PlayerId == evilGuesser);
                        data.MaxImpostorRoles--;
                        data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.GuesserSpawnBothRate.GetSelection(), 1));
                    }
                }
                else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
                {
                    byte niceGuesser = SetRoleToRandomPlayer((byte)RoleType.NiceGuesser, data.Crewmates);
                    data.Crewmates.RemoveAll(x => x.PlayerId == niceGuesser);
                    data.MaxCrewmateRoles--;
                    data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.GuesserSpawnBothRate.GetSelection(), 1));
                }
            }
        }
        else
        {
            if (isEvilGuesser) data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.GuesserSpawnRate.GetSelection(), 1));
            else data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.GuesserSpawnRate.GetSelection(), 1));
        }

        // Assign Swapper (chance to be impostor based on setting)
        if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.SwapperIsImpRate.GetSelection() * 10)
        {
            data.ImpSettings.Add((byte)RoleType.EvilSwapper, (CustomOptionHolder.SwapperSpawnRate.GetSelection(), 1));
        }
        else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
        {
            data.CrewSettings.Add((byte)RoleType.NiceSwapper, (CustomOptionHolder.SwapperSpawnRate.GetSelection(), 1));
        }

        // Assign Shifter (chance to be neutral based on setting)
        bool shifterIsNeutral = false;
        if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.ShifterIsNeutralRate.GetSelection() * 10)
        {
            data.NeutralSettings.Add((byte)RoleType.Shifter, (CustomOptionHolder.ShifterSpawnRate.GetSelection(), 1));
            shifterIsNeutral = true;
        }
        else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
        {
            data.CrewSettings.Add((byte)RoleType.Shifter, (CustomOptionHolder.ShifterSpawnRate.GetSelection(), 1));
            shifterIsNeutral = false;
        }

        using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetShifterType))
        {
            sender.Write(shifterIsNeutral);
        }
        RPCProcedure.SetShifterType(shifterIsNeutral);
    }

    private static void AssignEnsuredRoles(RoleAssignmentData data)
    {
        BlockedAssignments = 0;

        // Get all roles where the chance to occur is set to 100%
        List<byte> ensuredCrewmateRoles = [];
        foreach (var kvp in data.CrewSettings)
        {
            if (kvp.Value.rate == 10)
            {
                for (int i = 0; i < kvp.Value.count; i++)
                    ensuredCrewmateRoles.Add(kvp.Key);
            }
        }

        List<byte> ensuredNeutralRoles = [];
        foreach (var kvp in data.NeutralSettings)
        {
            if (kvp.Value.rate == 10)
            {
                for (int i = 0; i < kvp.Value.count; i++)
                    ensuredNeutralRoles.Add(kvp.Key);
            }
        }

        List<byte> ensuredImpostorRoles = [];
        foreach (var kvp in data.ImpSettings)
        {
            if (kvp.Value.rate == 10)
            {
                for (int i = 0; i < kvp.Value.count; i++)
                    ensuredImpostorRoles.Add(kvp.Key);
            }
        }

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (
            (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) ||
            (data.Crewmates.Count > 0 && (
                (data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) ||
                (data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
            )))
        {
            List<TeamType> availableTeams = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) availableTeams.Add(TeamType.Crewmate);
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0) availableTeams.Add(TeamType.Neutral);
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) availableTeams.Add(TeamType.Impostor);

            var roleType = availableTeams[RebuildUs.Instance.Rnd.Next(0, availableTeams.Count)];

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

            var index = RebuildUs.Instance.Rnd.Next(0, selectedRoles.Count);
            var roleId = selectedRoles[index];
            var player = SetRoleToRandomPlayer(roleId, players);

            if (player == byte.MaxValue && BlockedAssignments < MaxBlocks)
            {
                BlockedAssignments++;
                continue;
            }
            BlockedAssignments = 0;

            selectedRoles.RemoveAt(index);

            if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out var blockedRoles))
            {
                foreach (var blockedRoleId in blockedRoles)
                {
                    // Set chance for the blocked roles to 0 for chances less than 100%
                    if (data.ImpSettings.ContainsKey(blockedRoleId)) data.ImpSettings[blockedRoleId] = (0, 0);
                    if (data.NeutralSettings.ContainsKey(blockedRoleId)) data.NeutralSettings[blockedRoleId] = (0, 0);
                    if (data.CrewSettings.ContainsKey(blockedRoleId)) data.CrewSettings[blockedRoleId] = (0, 0);

                    // Remove blocked roles even if the chance was 100%
                    ensuredCrewmateRoles.RemoveAll(x => x == blockedRoleId);
                    ensuredNeutralRoles.RemoveAll(x => x == blockedRoleId);
                    ensuredImpostorRoles.RemoveAll(x => x == blockedRoleId);
                }
            }

            // Adjust the role limit
            switch (roleType)
            {
                case TeamType.Crewmate: data.MaxCrewmateRoles--; break;
                case TeamType.Neutral: data.MaxNeutralRoles--; break;
                case TeamType.Impostor: data.MaxImpostorRoles--; break;
            }
        }
    }

    private static void AssignChanceRoles(RoleAssignmentData data)
    {
        BlockedAssignments = 0;

        // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
        List<byte> crewmateTickets = [];
        foreach (var kvp in data.CrewSettings)
        {
            if (kvp.Value.rate is > 0 and < 10)
            {
                for (int i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
                    crewmateTickets.Add(kvp.Key);
            }
        }

        List<byte> neutralTickets = [];
        foreach (var kvp in data.NeutralSettings)
        {
            if (kvp.Value.rate is > 0 and < 10)
            {
                for (int i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
                    neutralTickets.Add(kvp.Key);
            }
        }

        List<byte> impostorTickets = [];
        foreach (var kvp in data.ImpSettings)
        {
            if (kvp.Value.rate is > 0 and < 10)
            {
                for (int i = 0; i < kvp.Value.rate * kvp.Value.count; i++)
                    impostorTickets.Add(kvp.Key);
            }
        }

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (
            (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0) ||
            (data.Crewmates.Count > 0 && (
                (data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0) ||
                (data.MaxNeutralRoles > 0 && neutralTickets.Count > 0)
            )))
        {
            List<TeamType> availableTeams = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0) availableTeams.Add(TeamType.Crewmate);
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && neutralTickets.Count > 0) availableTeams.Add(TeamType.Neutral);
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0) availableTeams.Add(TeamType.Impostor);

            var roleType = availableTeams[RebuildUs.Instance.Rnd.Next(0, availableTeams.Count)];

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

            var index = RebuildUs.Instance.Rnd.Next(0, selectedTickets.Count);
            var roleId = selectedTickets[index];
            var player = SetRoleToRandomPlayer(roleId, players);

            if (player == byte.MaxValue && BlockedAssignments < MaxBlocks)
            {
                BlockedAssignments++;
                continue;
            }
            BlockedAssignments = 0;

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
                case TeamType.Crewmate: data.MaxCrewmateRoles--; break;
                case TeamType.Neutral: data.MaxNeutralRoles--; break;
                case TeamType.Impostor: data.MaxImpostorRoles--; break;
            }
        }
    }

    private static byte SetRoleToHost(byte roleId, PlayerControl host)
    {
        byte playerId = host.PlayerId;

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetRole);
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

    private static byte SetRoleToRandomPlayer(byte roleId, List<PlayerControl> playerList, bool removePlayer = true)
    {
        var index = RebuildUs.Instance.Rnd.Next(0, playerList.Count);
        byte playerId = playerList[index].PlayerId;
        if (Helpers.RolesEnabled && (CustomOptionHolder.LoversSpawnRate == null || CustomOptionHolder.LoversSpawnRate.Enabled) &&
            Helpers.PlayerById(playerId)?.IsLovers() == true &&
            BlockLovers.Contains(roleId))
        {
            return byte.MaxValue;
        }

        if (removePlayer) playerList.RemoveAt(index);
        PlayerRoleMap.Add(new Tuple<byte, byte>(playerId, roleId));

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SetRole);
        sender.Write(roleId);
        sender.Write(playerId);
        RPCProcedure.SetRole(roleId, playerId);

        return playerId;
    }

    private static byte SetModifierToRandomPlayer(byte modId, List<PlayerControl> playerList)
    {
        if (playerList.Count <= 0)
        {
            return byte.MaxValue;
        }

        var index = RebuildUs.Instance.Rnd.Next(0, playerList.Count);
        byte playerId = playerList[index].PlayerId;

        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.AddModifier);
        sender.Write(modId);
        sender.Write(playerId);
        RPCProcedure.AddModifier(modId, playerId);

        return playerId;
    }

    private static List<int> myGamemodeList = new List<int>();

    private static void AssignCaptureTheFlagRoles()
    {
        var players = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            players.Add(p);
        }

        myGamemodeList.Clear();
        bool oddNumber = false;
        int playerNumber = 1;

        if (Mathf.Ceil(PlayerControl.AllPlayerControls.Count) % 2 != 0)
        {
            oddNumber = true;
            SetRoleToRandomPlayer((byte)RoleType.StealerPlayer, players);
        }
        while (myGamemodeList.Count < Mathf.Round(PlayerControl.AllPlayerControls.Count / 2))
        {
            switch (playerNumber)
            {
                case 1:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer01, players);
                    break;
                case 2:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer02, players);
                    break;
                case 3:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer03, players);
                    break;
                case 4:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer04, players);
                    break;
                case 5:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer05, players);
                    break;
                case 6:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer06, players);
                    break;
                case 7:
                    SetRoleToRandomPlayer((byte)RoleType.RedPlayer07, players);
                    break;
            }
            myGamemodeList.Add(playerNumber);
            playerNumber += 1;
        }
        playerNumber = 9;
        while (!oddNumber && myGamemodeList.Count < PlayerControl.AllPlayerControls.Count || oddNumber && myGamemodeList.Count < PlayerControl.AllPlayerControls.Count - 1)
        {
            switch (playerNumber)
            {
                case 9:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer01, players);
                    break;
                case 10:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer02, players);
                    break;
                case 11:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer03, players);
                    break;
                case 12:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer04, players);
                    break;
                case 13:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer05, players);
                    break;
                case 14:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer06, players);
                    break;
                case 15:
                    SetRoleToRandomPlayer((byte)RoleType.BluePlayer07, players);
                    break;
            }
            myGamemodeList.Add(playerNumber);
            playerNumber += 1;
        }
    }

    private static void AssignPoliceAndThievesRoles()
    {
        var players = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            players.Add(p);
        }

        myGamemodeList.Clear();
        int playerNumber = 1;

        while (myGamemodeList.Count < Mathf.Round(PlayerControl.AllPlayerControls.Count / 2.39f))
        {
            switch (playerNumber)
            {
                case 1:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer01, players);
                    break;
                case 2:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer03, players);
                    break;
                case 3:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer02, players);
                    break;
                case 4:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer05, players);
                    break;
                case 5:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer04, players);
                    break;
                case 6:
                    SetRoleToRandomPlayer((byte)RoleType.PolicePlayer06, players);
                    break;
            }
            myGamemodeList.Add(playerNumber);
            playerNumber += 1;
        }
        playerNumber = 7;
        while (myGamemodeList.Count < PlayerControl.AllPlayerControls.Count)
        {
            switch (playerNumber)
            {
                case 7:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer01, players);
                    break;
                case 8:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer02, players);
                    break;
                case 9:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer03, players);
                    break;
                case 10:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer04, players);
                    break;
                case 11:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer05, players);
                    break;
                case 12:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer06, players);
                    break;
                case 13:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer07, players);
                    break;
                case 14:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer08, players);
                    break;
                case 15:
                    SetRoleToRandomPlayer((byte)RoleType.ThiefPlayer09, players);
                    break;
            }
            myGamemodeList.Add(playerNumber);
            playerNumber += 1;
        }
    }

    private static void AssignHotPotatoRoles()
    {
        var players = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            players.Add(p);
        }

        myGamemodeList.Clear();
        int playerNumber = 1;

        while (myGamemodeList.Count < PlayerControl.AllPlayerControls.Count)
        {
            switch (playerNumber)
            {
                case 1:
                    SetRoleToRandomPlayer((byte)RoleType.HotPotato, players);
                    break;
                case 2:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato01, players);
                    break;
                case 3:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato02, players);
                    break;
                case 4:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato03, players);
                    break;
                case 5:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato04, players);
                    break;
                case 6:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato05, players);
                    break;
                case 7:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato06, players);
                    break;
                case 8:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato07, players);
                    break;
                case 9:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato08, players);
                    break;
                case 10:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato09, players);
                    break;
                case 11:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato10, players);
                    break;
                case 12:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato11, players);
                    break;
                case 13:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato12, players);
                    break;
                case 14:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato13, players);
                    break;
                case 15:
                    SetRoleToRandomPlayer((byte)RoleType.NotPotato14, players);
                    break;
            }
            myGamemodeList.Add(playerNumber);
            playerNumber += 1;
        }
    }

    private static void AssignBattleRoyaleRoles()
    {
        var players = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            players.Add(p);
        }

        myGamemodeList.Clear();
        bool oddNumber = false;
        int playerNumber = 1;

        if (BattleRoyale.matchType == 0)
        {
            while (myGamemodeList.Count < PlayerControl.AllPlayerControls.Count)
            {
                switch (playerNumber)
                {
                    case 1:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer01, players);
                        break;
                    case 2:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer02, players);
                        break;
                    case 3:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer03, players);
                        break;
                    case 4:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer04, players);
                        break;
                    case 5:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer05, players);
                        break;
                    case 6:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer06, players);
                        break;
                    case 7:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer07, players);
                        break;
                    case 8:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer08, players);
                        break;
                    case 9:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer09, players);
                        break;
                    case 10:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer10, players);
                        break;
                    case 11:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer11, players);
                        break;
                    case 12:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer12, players);
                        break;
                    case 13:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer13, players);
                        break;
                    case 14:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer14, players);
                        break;
                    case 15:
                        SetRoleToRandomPlayer((byte)RoleType.SoloPlayer15, players);
                        break;
                }
                myGamemodeList.Add(playerNumber);
                playerNumber += 1;
            }
        }
        else
        {
            // Battle Royale Teams
            if (Mathf.Ceil(PlayerControl.AllPlayerControls.Count) % 2 != 0)
            {
                oddNumber = true;
                SetRoleToRandomPlayer((byte)RoleType.SerialKiller, players);
            }
            while (myGamemodeList.Count < (Mathf.Round(PlayerControl.AllPlayerControls.Count / 2)))
            {
                switch (playerNumber)
                {
                    case 1:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer01, players);
                        break;
                    case 2:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer02, players);
                        break;
                    case 3:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer03, players);
                        break;
                    case 4:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer04, players);
                        break;
                    case 5:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer05, players);
                        break;
                    case 6:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer06, players);
                        break;
                    case 7:
                        SetRoleToRandomPlayer((byte)RoleType.LimePlayer07, players);
                        break;
                }
                myGamemodeList.Add(playerNumber);
                playerNumber += 1;
            }
            playerNumber = 9;
            while (!oddNumber && myGamemodeList.Count < PlayerControl.AllPlayerControls.Count || oddNumber && myGamemodeList.Count < PlayerControl.AllPlayerControls.Count - 1)
            {
                switch (playerNumber)
                {
                    case 9:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer01, players);
                        break;
                    case 10:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer02, players);
                        break;
                    case 11:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer03, players);
                        break;
                    case 12:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer04, players);
                        break;
                    case 13:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer05, players);
                        break;
                    case 14:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer06, players);
                        break;
                    case 15:
                        SetRoleToRandomPlayer((byte)RoleType.PinkPlayer07, players);
                        break;
                }
                myGamemodeList.Add(playerNumber);
                playerNumber += 1;
            }
        }
    }

    private static void SetRolesAgain()
    {
        while (PlayerRoleMap.Count > 0)
        {
            byte amount = (byte)Math.Min(PlayerRoleMap.Count, 20);
            var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WorkaroundSetRoles, SendOption.Reliable, -1);
            writer.Write(amount);
            for (int i = 0; i < amount; i++)
            {
                var option = PlayerRoleMap[0];
                PlayerRoleMap.RemoveAt(0);
                writer.WritePacked((uint)option.Item1);
                writer.WritePacked((uint)option.Item2);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    public class RoleAssignmentData
    {
        public List<PlayerControl> Crewmates { get; set; }
        public List<PlayerControl> Impostors { get; set; }
        public Dictionary<byte, (int rate, int count)> ImpSettings = [];
        public Dictionary<byte, (int rate, int count)> NeutralSettings = [];
        public Dictionary<byte, (int rate, int count)> CrewSettings = [];
        public int MaxCrewmateRoles { get; set; }
        public int MaxNeutralRoles { get; set; }
        public int MaxImpostorRoles { get; set; }
    }

    private enum TeamType
    {
        Crewmate = 0,
        Neutral = 1,
        Impostor = 2
    }
}