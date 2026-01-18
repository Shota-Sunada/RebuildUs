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
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ResetVariables);
            RPCProcedure.ResetVariables();
        }
        yield return WaitResetVariables().WrapToIl2Cpp();

        if (!DestroyableSingleton<TutorialManager>.InstanceExists && CustomOptionHolder.ActivateRoles.GetBool()) // Don't assign Roles in Tutorial or if deactivated
        {
            AssignRoles();
            {
                using var sender2 = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.FinishSetRole);
                RPCProcedure.FinishSetRole();
            }
        }
        // 独自処理終了

        ShipStatus.Instance.Begin();
        __instance.SendClientReady();

        yield break;
    }
    public static Dictionary<byte, bool> CheckList;
    private static void CreateCheckList()
    {
        CheckList = [];
        foreach (var player in PlayerControl.AllPlayerControls)
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

    public static void AssignRoles()
    {
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
        //                 newImpId = RebuildUs.Instance.rnd.Next(0, PlayerControl.AllPlayerControls.Count);
        //                 newImp = PlayerControl.AllPlayerControls[newImpId];
        //                 if (newImp == host || newImp.Data.Role.IsImpostor)
        //                 {
        //                     continue;
        //                 }
        //                 break;
        //             }

        //             writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.OverrideNativeRole, Hazel.SendOption.Reliable, -1);
        //             writer.Write(host.PlayerId);
        //             writer.Write((byte)RoleTypes.Crewmate);
        //             AmongUsClient.Instance.FinishRpcImmediately(writer);
        //             RPCProcedure.overrideNativeRole(host.PlayerId, (byte)RoleTypes.Crewmate);

        //             writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.OverrideNativeRole, Hazel.SendOption.Reliable, -1);
        //             writer.Write(newImp.PlayerId);
        //             writer.Write((byte)RoleTypes.Impostor);
        //             AmongUsClient.Instance.FinishRpcImmediately(writer);
        //             RPCProcedure.overrideNativeRole(newImp.PlayerId, (byte)RoleTypes.Impostor);
        //         }
        //     }
        // }

        BlockLovers = [(byte)RoleType.Bait,];

        if (!Lovers.hasTasks)
        {
            BlockLovers.Add((byte)RoleType.Snitch);
        }

        if (!CustomOptionHolder.ArsonistCanBeLovers.GetBool())
        {
            BlockLovers.Add((byte)RoleType.Arsonist);
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

    public static RoleAssignmentData GetRoleAssignmentData()
    {
        // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
        List<PlayerControl> crewmates = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().OrderBy(x => Guid.NewGuid())];
        crewmates.RemoveAll(x => x.Data.Role.IsImpostor);
        List<PlayerControl> impostors = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().OrderBy(x => Guid.NewGuid())];
        impostors.RemoveAll(x => !x.Data.Role.IsImpostor);

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

        // impSettings.Add((byte)ERoleType.Morphling, CustomOptionHolder.morphlingSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Camouflager, CustomOptionHolder.camouflagerSpawnRate.data);
        // impSettings.Add((byte)ERoleType.EvilHacker, CustomOptionHolder.evilHackerSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Vampire, CustomOptionHolder.vampireSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Eraser, CustomOptionHolder.eraserSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Trickster, CustomOptionHolder.tricksterSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Cleaner, CustomOptionHolder.cleanerSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Warlock, CustomOptionHolder.warlockSpawnRate.data);
        // impSettings.Add((byte)ERoleType.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Witch, CustomOptionHolder.witchSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Assassin, CustomOptionHolder.assassinSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Ninja, CustomOptionHolder.ninjaSpawnRate.data);
        // impSettings.Add((byte)ERoleType.NekoKabocha, CustomOptionHolder.nekoKabochaSpawnRate.data);
        // impSettings.Add((byte)ERoleType.SerialKiller, CustomOptionHolder.serialKillerSpawnRate.data);
        // impSettings.Add((byte)ERoleType.Trapper, CustomOptionHolder.trapperSpawnRate.data);
        // impSettings.Add((byte)ERoleType.EvilTracker, CustomOptionHolder.evilTrackerSpawnRate.data);

        neutralSettings.Add((byte)RoleType.Jester, CustomOptionHolder.JesterSpawnRate.Data);
        // neutralSettings.Add((byte)ERoleType.Arsonist, CustomOptionHolder.arsonistSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Jackal, CustomOptionHolder.jackalSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Opportunist, CustomOptionHolder.opportunistSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Vulture, CustomOptionHolder.vultureSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Lawyer, CustomOptionHolder.lawyerSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.PlagueDoctor, CustomOptionHolder.plagueDoctorSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Fox, CustomOptionHolder.foxSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.SchrodingersCat, CustomOptionHolder.schrodingersCatSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Puppeteer, CustomOptionHolder.puppeteerSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.JekyllAndHyde, CustomOptionHolder.jekyllAndHydeSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Akujo, CustomOptionHolder.akujoSpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Moriarty, CustomOptionHolder.moriartySpawnRate.data);
        // neutralSettings.Add((byte)ERoleType.Cupid, CustomOptionHolder.cupidSpawnRate.data);

        // crewSettings.Add((byte)ERoleType.FortuneTeller, CustomOptionHolder.fortuneTellerSpawnRate.data);
        crewSettings.Add((byte)RoleType.Mayor, CustomOptionHolder.MayorSpawnRate.Data);
        // crewSettings.Add((byte)ERoleType.Engineer, CustomOptionHolder.engineerSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Sheriff, CustomOptionHolder.sheriffSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Lighter, CustomOptionHolder.lighterSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Detective, CustomOptionHolder.detectiveSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.TimeMaster, CustomOptionHolder.timeMasterSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Medic, CustomOptionHolder.medicSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Seer, CustomOptionHolder.seerSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Hacker, CustomOptionHolder.hackerSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Tracker, CustomOptionHolder.trackerSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Snitch, CustomOptionHolder.snitchSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Bait, CustomOptionHolder.baitSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Medium, CustomOptionHolder.mediumSpawnRate.data);
        // crewSettings.Add((byte)ERoleType.Sherlock, CustomOptionHolder.sherlockSpawnRate.data);
        // if (impostors.Count > 1)
        // {
        //     // Only add Spy if more than 1 impostor as the spy role is otherwise useless
        //     crewSettings.Add((byte)ERoleType.Spy, CustomOptionHolder.spySpawnRate.data);
        // }

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
        //         gmID = setRoleToRandomPlayer((byte)ERoleType.GM, data.crewmates);
        //     }

        //     PlayerControl p = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().ToList().Find(x => x.PlayerId == gmID);

        //     if (p != null && CustomOptionHolder.gmDiesAtStart.getBool())
        //     {
        //         p.Exiled();
        //     }
        // }

        // // Assign Lovers
        // if (CustomOptionHolder.loversSpawnRate.enabled)
        // {
        //     for (int i = 0; i < CustomOptionHolder.loversNumCouples.getFloat(); i++)
        //     {
        //         var singleCrew = data.crewmates.FindAll(x => !x.isLovers());
        //         var singleImps = data.impostors.FindAll(x => !x.isLovers());

        //         bool isOnlyRole = !CustomOptionHolder.loversCanHaveAnotherRole.getBool();
        //         if (rnd.Next(1, 101) <= CustomOptionHolder.loversSpawnRate.getSelection() * 10)
        //         {
        //             int lover1 = -1;
        //             int lover2 = -1;
        //             int lover1Index = -1;
        //             int lover2Index = -1;
        //             if (singleImps.Count > 0 && singleCrew.Count > 0 && (!isOnlyRole || (data.maxCrewmateRoles > 0 && data.maxImpostorRoles > 0)) && rnd.Next(1, 101) <= CustomOptionHolder.loversImpLoverRate.getSelection() * 10)
        //             {
        //                 lover1Index = rnd.Next(0, singleImps.Count);
        //                 lover1 = singleImps[lover1Index].PlayerId;

        //                 lover2Index = rnd.Next(0, singleCrew.Count);
        //                 lover2 = singleCrew[lover2Index].PlayerId;

        //                 if (isOnlyRole)
        //                 {
        //                     data.maxImpostorRoles--;
        //                     data.maxCrewmateRoles--;

        //                     data.impostors.RemoveAll(x => x.PlayerId == lover1);
        //                     data.crewmates.RemoveAll(x => x.PlayerId == lover2);
        //                 }
        //             }

        //             else if (singleCrew.Count >= 2 && (isOnlyRole || data.maxCrewmateRoles >= 2))
        //             {
        //                 lover1Index = rnd.Next(0, singleCrew.Count);
        //                 while (lover2Index == lover1Index || lover2Index < 0) lover2Index = rnd.Next(0, singleCrew.Count);

        //                 lover1 = singleCrew[lover1Index].PlayerId;
        //                 lover2 = singleCrew[lover2Index].PlayerId;

        //                 if (isOnlyRole)
        //                 {
        //                     data.maxCrewmateRoles -= 2;
        //                     data.crewmates.RemoveAll(x => x.PlayerId == lover1);
        //                     data.crewmates.RemoveAll(x => x.PlayerId == lover2);
        //                 }
        //             }

        //             if (lover1 >= 0 && lover2 >= 0)
        //             {
        //                 writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetLovers, Hazel.SendOption.Reliable, -1);
        //                 writer.Write((byte)lover1);
        //                 writer.Write((byte)lover2);
        //                 AmongUsClient.Instance.FinishRpcImmediately(writer);
        //                 RPCProcedure.setLovers((byte)lover1, (byte)lover2);
        //             }
        //         }
        //     }
        // }

        // Assign Mafia
        if (data.Impostors.Count >= 3 && data.MaxImpostorRoles >= 3 && (RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.mafiaSpawnRate.GetSelection() * 10))
        {
            SetRoleToRandomPlayer((byte)RoleType.Godfather, data.Impostors);
            SetRoleToRandomPlayer((byte)RoleType.Janitor, data.Impostors);
            SetRoleToRandomPlayer((byte)RoleType.Mafioso, data.Impostors);
            data.MaxImpostorRoles -= 3;
        }

        // // Assign Bomber
        // if (data.impostors.Count >= 2 && data.maxImpostorRoles >= 2 && (rnd.Next(1, 101) <= CustomOptionHolder.bomberSpawnRate.getSelection() * 10))
        // {
        //     setRoleToRandomPlayer((byte)ERoleType.BomberA, data.impostors);
        //     setRoleToRandomPlayer((byte)ERoleType.BomberB, data.impostors);
        //     data.maxImpostorRoles -= 2;
        // }

        // // Assign Mimic
        // if (data.impostors.Count >= 2 && data.maxImpostorRoles >= 2 && (rnd.Next(1, 101) <= CustomOptionHolder.mimicSpawnRate.getSelection() * 10))
        // {
        //     setRoleToRandomPlayer((byte)ERoleType.MimicK, data.impostors);
        //     setRoleToRandomPlayer((byte)ERoleType.MimicA, data.impostors);
        //     data.maxImpostorRoles -= 2;
        // }
    }

    private static void SelectFactionForFactionIndependentRoles(RoleAssignmentData data)
    {
        // Assign Guesser (chance to be impostor based on setting)
        bool isEvilGuesser = RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.guesserIsImpGuesserRate.GetSelection() * 10;
        if (CustomOptionHolder.guesserSpawnBothRate.GetSelection() > 0)
        {
            if (RebuildUs.Instance.Rnd.Next(1, 101) <= CustomOptionHolder.guesserSpawnRate.GetSelection() * 10)
            {
                if (isEvilGuesser)
                {
                    if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0)
                    {
                        byte evilGuesser = SetRoleToRandomPlayer((byte)RoleType.EvilGuesser, data.Impostors);
                        data.Impostors.ToList().RemoveAll(x => x.PlayerId == evilGuesser);
                        data.MaxImpostorRoles--;
                        data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.guesserSpawnBothRate.GetSelection(), 1));
                    }
                }
                else if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0)
                {
                    byte niceGuesser = SetRoleToRandomPlayer((byte)RoleType.NiceGuesser, data.Crewmates);
                    data.Crewmates.ToList().RemoveAll(x => x.PlayerId == niceGuesser);
                    data.MaxCrewmateRoles--;
                    data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.guesserSpawnBothRate.GetSelection(), 1));
                }
            }
        }
        else
        {
            if (isEvilGuesser) data.ImpSettings.Add((byte)RoleType.EvilGuesser, (CustomOptionHolder.guesserSpawnRate.GetSelection(), 1));
            else data.CrewSettings.Add((byte)RoleType.NiceGuesser, (CustomOptionHolder.guesserSpawnRate.GetSelection(), 1));
        }

        // // Assign Swapper (chance to be impostor based on setting)
        // if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && rnd.Next(1, 101) <= CustomOptionHolder.swapperIsImpRate.getSelection() * 10)
        // {
        //     data.impSettings.Add((byte)ERoleType.Swapper, (CustomOptionHolder.swapperSpawnRate.getSelection(), 1));
        // }
        // else if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0)
        // {
        //     data.crewSettings.Add((byte)ERoleType.Swapper, (CustomOptionHolder.swapperSpawnRate.getSelection(), 1));
        // }

        // // Assign Shifter (chance to be neutral based on setting)
        // bool shifterIsNeutral = false;
        // if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && rnd.Next(1, 101) <= CustomOptionHolder.shifterIsNeutralRate.getSelection() * 10)
        // {
        //     data.neutralSettings.Add((byte)ERoleType.Shifter, (CustomOptionHolder.shifterSpawnRate.getSelection(), 1));
        //     shifterIsNeutral = true;
        // }
        // else if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0)
        // {
        //     data.crewSettings.Add((byte)ERoleType.Shifter, (CustomOptionHolder.shifterSpawnRate.getSelection(), 1));
        //     shifterIsNeutral = false;
        // }

        // // Assign any dual role types
        // foreach (var option in CustomDualRoleOption.dualRoles)
        // {
        //     if (option.count <= 0 || !option.roleEnabled) continue;

        //     int niceCount = 0;
        //     int evilCount = 0;
        //     while (niceCount + evilCount < option.count)
        //     {
        //         if (option.assignEqually)
        //         {
        //             niceCount++;
        //             evilCount++;
        //         }
        //         else
        //         {
        //             bool isEvil = rnd.Next(1, 101) <= option.impChance * 10;
        //             if (isEvil) evilCount++;
        //             else niceCount++;
        //         }
        //     }

        //     if (niceCount > 0)
        //         data.crewSettings.Add((byte)option.roleType, (option.rate, niceCount));

        //     if (evilCount > 0)
        //         data.impSettings.Add((byte)option.roleType, (option.rate, evilCount));
        // }

        //  writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetShifterType, Hazel.SendOption.Reliable, -1);
        // writer.Write(shifterIsNeutral);
        // AmongUsClient.Instance.FinishRpcImmediately(writer);
        // RPCProcedure.setShifterType(shifterIsNeutral);
    }

    private static void AssignEnsuredRoles(RoleAssignmentData data)
    {
        BlockedAssignments = 0;

        // Get all roles where the chance to occur is set to 100%
        var ensuredCrewmateRoles = data.CrewSettings.Where(x => x.Value.rate == 10).Select(x => Enumerable.Repeat(x.Key, x.Value.count)).SelectMany(x => x).ToList();
        var ensuredNeutralRoles = data.NeutralSettings.Where(x => x.Value.rate == 10).Select(x => Enumerable.Repeat(x.Key, x.Value.count)).SelectMany(x => x).ToList();
        var ensuredImpostorRoles = data.ImpSettings.Where(x => x.Value.rate == 10).Select(x => Enumerable.Repeat(x.Key, x.Value.count)).SelectMany(x => x).ToList();

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (
            (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) ||
            (data.Crewmates.Count > 0 && (
                (data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) ||
                (data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
            )))
        {

            Dictionary<TeamType, List<byte>> rolesToAssign = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) rolesToAssign.Add(TeamType.Crewmate, ensuredCrewmateRoles);
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0) rolesToAssign.Add(TeamType.Neutral, ensuredNeutralRoles);
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) rolesToAssign.Add(TeamType.Impostor, ensuredImpostorRoles);

            // Randomly select a pool of roles to assign a role from next (Crewmate role, Neutral role or Impostor role)
            // then select one of the roles from the selected pool to a player
            // and remove the role (and any potentially blocked role pairings) from the pool(s)
            var roleType = rolesToAssign.Keys.ElementAt(RebuildUs.Instance.Rnd.Next(0, rolesToAssign.Keys.Count()));
            var players = roleType is TeamType.Crewmate or TeamType.Neutral ? data.Crewmates : data.Impostors;
            var index = RebuildUs.Instance.Rnd.Next(0, rolesToAssign[roleType].Count);
            var roleId = rolesToAssign[roleType][index];
            var player = SetRoleToRandomPlayer(rolesToAssign[roleType][index], players);
            if (player == byte.MaxValue && BlockedAssignments < MaxBlocks)
            {
                BlockedAssignments++;
                continue;
            }
            BlockedAssignments = 0;

            rolesToAssign[roleType].RemoveAt(index);

            if (CustomOptionHolder.BlockedRolePairings.ContainsKey(roleId))
            {
                foreach (var blockedRoleId in CustomOptionHolder.BlockedRolePairings[roleId])
                {
                    // Set chance for the blocked roles to 0 for chances less than 100%
                    if (data.ImpSettings.ContainsKey(blockedRoleId)) data.ImpSettings[blockedRoleId] = (0, 0);
                    if (data.NeutralSettings.ContainsKey(blockedRoleId)) data.NeutralSettings[blockedRoleId] = (0, 0);
                    if (data.CrewSettings.ContainsKey(blockedRoleId)) data.CrewSettings[blockedRoleId] = (0, 0);
                    // Remove blocked roles even if the chance was 100%
                    foreach (var ensuredRolesList in rolesToAssign.Values)
                    {
                        ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                    }
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
        List<byte> crewmateTickets = [.. data.CrewSettings.Where(x => x.Value.rate is > 0 and < 10).Select(x => Enumerable.Repeat(x.Key, x.Value.rate * x.Value.count)).SelectMany(x => x)];
        List<byte> neutralTickets = [.. data.NeutralSettings.Where(x => x.Value.rate is > 0 and < 10).Select(x => Enumerable.Repeat(x.Key, x.Value.rate * x.Value.count)).SelectMany(x => x)];
        List<byte> impostorTickets = [.. data.ImpSettings.Where(x => x.Value.rate is > 0 and < 10).Select(x => Enumerable.Repeat(x.Key, x.Value.rate * x.Value.count)).SelectMany(x => x)];

        // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
        while (
            (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0) ||
            (data.Crewmates.Count > 0 && (
                (data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0) ||
                (data.MaxNeutralRoles > 0 && neutralTickets.Count > 0)
            )))
        {

            Dictionary<TeamType, List<byte>> rolesToAssign = [];
            if (data.Crewmates.Count > 0 && data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0) rolesToAssign.Add(TeamType.Crewmate, crewmateTickets);
            if (data.Crewmates.Count > 0 && data.MaxNeutralRoles > 0 && neutralTickets.Count > 0) rolesToAssign.Add(TeamType.Neutral, neutralTickets);
            if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0) rolesToAssign.Add(TeamType.Impostor, impostorTickets);

            // Randomly select a pool of role tickets to assign a role from next (Crewmate role, Neutral role or Impostor role)
            // then select one of the roles from the selected pool to a player
            // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
            var roleType = rolesToAssign.Keys.ElementAt(RebuildUs.Instance.Rnd.Next(0, rolesToAssign.Keys.Count()));
            var players = roleType is TeamType.Crewmate or TeamType.Neutral ? data.Crewmates : data.Impostors;
            var index = RebuildUs.Instance.Rnd.Next(0, rolesToAssign[roleType].Count);
            var roleId = rolesToAssign[roleType][index];
            var player = SetRoleToRandomPlayer(rolesToAssign[roleType][index], players);
            if (player == byte.MaxValue && BlockedAssignments < MaxBlocks)
            {
                BlockedAssignments++;
                continue;
            }
            BlockedAssignments = 0;

            rolesToAssign[roleType].RemoveAll(x => x == roleId);

            if (CustomOptionHolder.BlockedRolePairings.ContainsKey(roleId))
            {
                foreach (var blockedRoleId in CustomOptionHolder.BlockedRolePairings[roleId])
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

        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetRole);
        sender.Write(roleId);
        sender.Write(playerId);
        RPCProcedure.SetRole(roleId, playerId);

        return playerId;
    }

    private static void AssignRoleTargets(RoleAssignmentData data)
    {
        // // Set Lawyer Target
        // if (Lawyer.lawyer != null)
        // {
        //     var possibleTargets = new List<PlayerControl>();
        //     foreach (PlayerControl p in CachedPlayer.AllPlayers)
        //     {
        //         if (!p.Data.IsDead && !p.Data.Disconnected && !p.isLovers() && (p.Data.Role.IsImpostor || p == Jackal.jackal))
        //             possibleTargets.Add(p);
        //     }
        //     if (possibleTargets.Count == 0)
        //     {
        //         w = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
        //         AmongUsClient.Instance.FinishRpcImmediately(w);
        //         RPCProcedure.lawyerPromotesToPursuer();
        //     }
        //     else
        //     {
        //         var target = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
        //         writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerSetTarget, Hazel.SendOption.Reliable, -1);
        //         writer.Write(target.PlayerId);
        //         AmongUsClient.Instance.FinishRpcImmediately(writer);
        //         RPCProcedure.lawyerSetTarget(target.PlayerId);
        //     }
        // }
    }

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
        // if (RoleInfo.lovers.enabled &&
        //     Helpers.PlayerById(playerId)?.IsLovers() == true &&
        //     blockLovers.Contains(roleId))
        // {
        //     return byte.MaxValue;
        // }

        if (removePlayer) playerList.RemoveAt(index);
        PlayerRoleMap.Add(new Tuple<byte, byte>(playerId, roleId));

        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SetRole);
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

        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.AddModifier);
        sender.Write(modId);
        sender.Write(playerId);
        RPCProcedure.AddModifier(modId, playerId);

        return playerId;
    }

    private static void SetRolesAgain()
    {

        while (PlayerRoleMap.Any())
        {
            byte amount = (byte)Math.Min(PlayerRoleMap.Count, 20);
            var writer = AmongUsClient.Instance!.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.WorkaroundSetRoles, SendOption.Reliable, -1);
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