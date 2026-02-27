using Object = UnityEngine.Object;

namespace RebuildUs;

internal enum MurderAttemptResult
{
    PerformKill,
    SuppressKill,
    BlankKill,
    DelayVampireKill,
}

internal static class Helpers
{
    private static readonly StringBuilder ColorStringBuilder = new();

    private static readonly StringBuilder InfoStringBuilder = new();

    private static readonly Dictionary<byte, PlayerControl> PlayerByIdCache = [];
    private static int _lastCacheFrame = -1;

    private static readonly Vector3 ColorBlindMeetingPos = new(0.3384f, 0.23334f, -0.11f);
    private static readonly Vector3 ColorBlindMeetingScale = new(0.72f, 0.8f, 0.8f);
    private static readonly Dictionary<byte, PlayerVoteArea> VoteAreaStates = [];

    private static readonly StringBuilder RoleStringBuilder = new();

    internal static bool ShowButtons
    {
        get => !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance;
    }

    internal static bool ShowMeetingText
    {
        get => MeetingHud.Instance != null
               && MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion;
    }

    internal static bool GameStarted
    {
        get => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started;
    }

    internal static bool RolesEnabled
    {
        get => CustomOptionHolder.ActivateRoles.GetBool();
    }

    internal static bool RefundVotes
    {
        get => CustomOptionHolder.RefundVotesOnDeath.GetBool();
    }

    internal static bool IsSkeld
    {
        get => GetOption(ByteOptionNames.MapId) == 0;
    }

    internal static bool IsMiraHq
    {
        get => GetOption(ByteOptionNames.MapId) == 1;
    }

    internal static bool IsPolus
    {
        get => GetOption(ByteOptionNames.MapId) == 2;
    }

    internal static bool IsAirship
    {
        get => GetOption(ByteOptionNames.MapId) == 4;
    }

    internal static bool IsFungle
    {
        get => GetOption(ByteOptionNames.MapId) == 5;
    }

    internal static bool IsHideNSeekMode
    {
        get => GameManager.Instance.IsHideAndSeek();
    }

    internal static bool IsNormal
    {
        get => GameManager.Instance.IsNormal();
    }

    internal static bool IsCountdown
    {
        get => GameStartManager.InstanceExists && GameStartManager.Instance.startState is GameStartManager.StartingStates.Countdown;
    }

    internal static void DestroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : Object
    {
        if (items == null)
        {
            return;
        }
        foreach (T item in items)
        {
            Object.Destroy(item);
        }
    }

    internal static void DestroyList<T>(IEnumerable<T> items) where T : Object
    {
        if (items == null)
        {
            return;
        }
        foreach (T item in items)
        {
            Object.Destroy(item);
        }
    }

    internal static void GenerateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
    {
        if (player == null)
        {
            return;
        }

        List<byte> taskTypeIds = GenerateTasks(numCommon, numShort, numLong);
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedSetTasks);
        sender.Write(player.PlayerId);
        sender.WriteBytesAndSize(taskTypeIds.ToArray());
        RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
    }

    internal static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    internal static string Cs(Color c, string s)
    {
        ColorStringBuilder.Clear();
        ColorStringBuilder
            .Append("<color=#")
            .Append(ToByte(c.r).ToString("X2"))
            .Append(ToByte(c.g).ToString("X2"))
            .Append(ToByte(c.b).ToString("X2"))
            .Append(ToByte(c.a).ToString("X2"))
            .Append('>')
            .Append(s)
            .Append("</color>");
        return ColorStringBuilder.ToString();
    }

    private static byte ToByte(float f)
    {
        return (byte)(Mathf.Clamp01(f) * 255);
    }

    internal static int LineCount(string text)
    {
        return text.Count(c => c == '\n');
    }

    internal static bool GetKeysDown(params KeyCode[] keys)
    {
        if (keys.Length == 0)
        {
            return false;
        }
        bool allHeld = true;
        bool anyJustPressed = false;
        foreach (KeyCode key in keys)
        {
            if (!Input.GetKey(key))
            {
                allHeld = false;
                break;
            }

            if (Input.GetKeyDown(key))
            {
                anyJustPressed = true;
            }
        }

        return allHeld && anyJustPressed;
    }

    internal static bool HasFakeTasks(this PlayerControl player)
    {
        return player.IsNeutral() && !player.NeutralHasTasks()
               || player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks
               || player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks
               || player.IsRole(RoleType.Madmate) && !MadmateRole.CanKnowImpostorAfterFinishTasks
               || player.IsRole(RoleType.Suicider) && !Suicider.CanKnowImpostorAfterFinishTasks
               || player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount;
    }

    internal static PlayerControl PlayerById(byte id)
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == id)
            {
                return player;
            }
        }

        return null;
    }

    internal static Dictionary<byte, PlayerControl> AllPlayersById()
    {
        if (Time.frameCount == _lastCacheFrame)
        {
            return PlayerByIdCache;
        }

        _lastCacheFrame = Time.frameCount;
        PlayerByIdCache.Clear();
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null)
            {
                PlayerByIdCache[p.PlayerId] = p;
            }
        }

        return PlayerByIdCache;
    }

    internal static void HandleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        PlayerControl killer = Vampire.AllPlayers.FirstOrDefault();
        if (killer != null && Vampire.Bitten != null)
        {
            CheckMurderAttemptAndKill(killer, Vampire.Bitten, true, false);
        }

        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
        {
            sender.Write(byte.MaxValue);
            sender.Write(byte.MaxValue);
        }

        RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
    }

    internal static void RefreshRoleDescription(PlayerControl player)
    {
        if (player == null)
        {
            return;
        }

        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(player);
        List<PlayerTask> toRemove = [];

        foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
        {
            ImportantTextTask textTask = t.TryCast<ImportantTextTask>();
            if (textTask != null)
            {
                bool found = false;
                for (int i = 0; i < infos.Count; i++)
                {
                    if (textTask.Text.StartsWith(infos[i].Name))
                    {
                        infos.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    toRemove.Add(t);
                }
            }
        }

        foreach (PlayerTask t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            Object.Destroy(t.gameObject);
        }

        foreach (RoleInfo roleInfo in infos)
        {
            ImportantTextTask task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            InfoStringBuilder.Clear();
            InfoStringBuilder.Append(roleInfo.Name).Append(": ");
            if (roleInfo.RoleType == RoleType.Jackal)
            {
                InfoStringBuilder.Append(Jackal.CanCreateSidekick ? Tr.Get(TrKey.JackalWithSidekick) : Tr.Get(TrKey.JackalShortDesc));
            }
            else
            {
                InfoStringBuilder.Append(roleInfo.ShortDescription);
            }

            task.Text = Cs(roleInfo.Color, InfoStringBuilder.ToString());
            player.myTasks.Insert(0, task);
        }

        if (player.HasModifier(ModifierType.Madmate) || player.HasModifier(ModifierType.CreatedMadmate))
        {
            ImportantTextTask task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            InfoStringBuilder.Clear();
            InfoStringBuilder.Append(Madmate.FullName).Append(": ").Append(Tr.Get(TrKey.MadmateShortDesc));
            task.Text = Cs(Madmate.NameColor, InfoStringBuilder.ToString());
            player.myTasks.Insert(0, task);
        }
    }

    internal static bool IsLighterColor(int colorId)
    {
        return CustomColors.LighterColors.Contains(colorId);
    }

    internal static bool MushroomSabotageActive()
    {
        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.TaskType == TaskTypes.MushroomMixupSabotage)
            {
                return true;
            }
        }

        return false;
    }

    internal static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        float alpha = value ? 0.25f : 1f;
        foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = r.color;
            r.color = new(c.r, c.g, c.b, alpha);
        }

        TextMeshPro nameTxt = player.cosmetics.nameText;
        nameTxt.color = new(nameTxt.color.r, nameTxt.color.g, nameTxt.color.b, alpha);
    }

    internal static List<byte> GenerateTasks(int numCommon, int numShort, int numLong)
    {
        if (numCommon + numShort + numLong <= 0)
        {
            numShort = 1;
        }

        Il2CppSystem.Collections.Generic.List<byte> tasks = new();
        Il2CppSystem.Collections.Generic.HashSet<TaskTypes> hashSet = new();

        List<NormalPlayerTask> commonTasks = [.. MapUtilities.CachedShipStatus.CommonTasks];
        commonTasks.Shuffle();

        List<NormalPlayerTask> shortTasks = [.. MapUtilities.CachedShipStatus.ShortTasks];
        shortTasks.Shuffle();

        List<NormalPlayerTask> longTasks = [.. MapUtilities.CachedShipStatus.LongTasks];
        longTasks.Shuffle();

        int start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> commonTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in commonTasks)
        {
            commonTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasksIl2Cpp);

        start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> shortTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in shortTasks)
        {
            shortTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasksIl2Cpp);

        start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> longTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in longTasks)
        {
            longTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasksIl2Cpp);

        return [.. tasks];
    }

    internal static void ClearAllTasks(this PlayerControl player)
    {
        if (player == null)
        {
            return;
        }
        foreach (PlayerTask t in player.myTasks)
        {
            t.OnRemove();
            Object.Destroy(t.gameObject);
        }

        player.myTasks.Clear();
        player.Data?.Tasks?.Clear();
    }

    internal static bool IsCrewmateAlive()
    {
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamCrewmate()
                && !p.HasModifier(ModifierType.Madmate)
                && !p.IsRole(RoleType.Madmate)
                && !p.IsRole(RoleType.Suicider)
                && p.IsAlive())
            {
                return true;
            }
        }

        return false;
    }

    internal static bool HasImpostorVision(PlayerControl player)
    {
        if (player.IsTeamImpostor())
        {
            return true;
        }

        bool isFormerJackal = false;
        foreach (PlayerControl p in Jackal.FormerJackals)
        {
            if (p.PlayerId == player.PlayerId)
            {
                isFormerJackal = true;
                break;
            }
        }

        return (player.IsRole(RoleType.Jackal) || isFormerJackal) && Jackal.HasImpostorVision
               || player.IsRole(RoleType.Sidekick) && Sidekick.HasImpostorVision
               || player.IsRole(RoleType.Spy) && Spy.HasImpostorVision
               || player.IsRole(RoleType.Madmate) && MadmateRole.HasImpostorVision
               || player.IsRole(RoleType.Suicider) && Suicider.HasImpostorVision
               || player.IsRole(RoleType.Jester) && Jester.HasImpostorVision;
    }

    internal static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        byte maxKey = byte.MaxValue;
        int maxValue = int.MinValue;

        foreach (KeyValuePair<byte, int> pair in self)
        {
            if (pair.Value > maxValue)
            {
                maxValue = pair.Value;
                maxKey = pair.Key;
                tie = false;
            }
            else if (pair.Value == maxValue)
            {
                tie = true;
            }
        }

        return new(maxKey, maxValue);
    }

    internal static MurderAttemptResult CheckMurderAttempt(PlayerControl killer,
                                                           PlayerControl target,
                                                           bool blockRewind = false,
                                                           bool ignoreBlank = false,
                                                           bool ignoreIfKillerIsDead = false,
                                                           bool ignoreMedic = false)
    {
        if (AmongUsClient.Instance.IsGameOver)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (killer == null || killer.Data == null || killer.Data.IsDead && !ignoreIfKillerIsDead || killer.Data.Disconnected)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (IsHideNSeekMode)
        {
            return MurderAttemptResult.PerformKill;
        }

        if (Medic.Exists && !ignoreMedic && Medic.Shielded != null && Medic.Shielded == target)
        {
            using (new RPCSender(killer.NetId, CustomRPC.ShieldedMurderAttempt)) RPCProcedure.ShieldedMurderAttempt();

            return MurderAttemptResult.SuppressKill;
        }

        if (Mini.Exists && target.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(target))
        {
            return MurderAttemptResult.SuppressKill;
        }

        if (TimeMaster.Exists && TimeMaster.ShieldActive && target.IsRole(RoleType.TimeMaster))
        {
            if (!blockRewind)
            {
                using (new RPCSender(killer.NetId, CustomRPC.TimeMasterRewindTime)) RPCProcedure.TimeMasterRewindTime();
            }

            return MurderAttemptResult.SuppressKill;
        }

        return MurderAttemptResult.PerformKill;
    }

    internal static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer))
        {
            sender.Write(killer.PlayerId);
            sender.Write(target.PlayerId);
            sender.Write(showAnimation ? byte.MaxValue : (byte)0);
        }

        RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    internal static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer,
                                                                  PlayerControl target,
                                                                  bool isMeetingStart = false,
                                                                  bool showAnimation = true,
                                                                  bool ignoreBlank = false,
                                                                  bool ignoreIfKillerIsDead = false)
    {
        MurderAttemptResult murder = CheckMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);
        Logger.LogMessage(Enum.GetName(typeof(MurderAttemptResult), murder));

        if (murder == MurderAttemptResult.PerformKill)
        {
            MurderPlayer(killer, target, showAnimation);
        }
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f,
                new Action<float>(p =>
                {
                    if (!TransportationToolPatches.IsUsingTransportation(target) && Vampire.Bitten != null)
                    {
                        using (new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
                            RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);

                        MurderPlayer(killer, target, showAnimation);
                    }
                })));
        }

        return murder;
    }

    internal static bool SabotageActive()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;
    }

    internal static float SabotageTimer()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;
    }

    internal static bool CanUseSabotage()
    {
        SabotageSystemType sabSystem = MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (MapUtilities.Systems.TryGetValue(SystemTypes.Doors, out Object systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }

        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    internal static PlayerControl SetTarget(bool onlyCrewmates = false,
                                            bool targetPlayersInVents = false,
                                            List<PlayerControl> untargetablePlayers = null,
                                            PlayerControl targetingPlayer = null,
                                            int killDistanceIdx = -1)
    {
        if (!MapUtilities.CachedShipStatus)
        {
            return null;
        }

        targetingPlayer ??= PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent || targetingPlayer.IsGm())
        {
            return null;
        }

        float num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(killDistanceIdx == -1 ? GetOption(Int32OptionNames.KillDistance) : killDistanceIdx,
            0,
            2)];
        untargetablePlayers ??= [];

        Vector2 truePosition = targetingPlayer.GetTruePosition();
        PlayerControl result = null;

        foreach (NetworkedPlayerInfo playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (playerInfo.Disconnected || playerInfo.PlayerId == targetingPlayer.PlayerId || playerInfo.IsDead)
            {
                continue;
            }
            if (onlyCrewmates && playerInfo.Role.IsImpostor)
            {
                continue;
            }

            PlayerControl obj = playerInfo.Object;
            if (obj == null || obj.inVent && !targetPlayersInVents)
            {
                continue;
            }

            bool untargetable = false;
            foreach (PlayerControl utp in untargetablePlayers)
            {
                if (utp == obj)
                {
                    untargetable = true;
                    break;
                }
            }

            if (untargetable)
            {
                continue;
            }

            Vector2 vector = obj.GetTruePosition() - truePosition;
            float magnitude = vector.magnitude;
            if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
            {
                result = obj;
                num = magnitude;
            }
        }

        return result;
    }

    internal static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
        {
            return;
        }

        Material mat = target.cosmetics.currentBodySprite.BodySprite.material;
        mat.SetFloat("_Outline", 1f);
        mat.SetColor("_OutlineColor", color);
    }

    internal static void SetBasePlayerOutlines()
    {
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
            {
                continue;
            }

            bool isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
            bool hasVisibleShield = false;
            if (Camouflager.CamouflageTimer <= 0f
                && Medic.Shielded != null
                && (target == Medic.Shielded && !isMorphedMorphing || isMorphedMorphing && Morphing.MorphTarget == Medic.Shielded))
            {
                hasVisibleShield = Medic.ShowShielded switch
                {
                    0 => true, // Everyone
                    1 => localPlayer == Medic.Shielded || localPlayer.IsRole(RoleType.Medic), // Shielded + Medic
                    2 => localPlayer.IsRole(RoleType.Medic), // Medic only
                    _ => false,
                };
            }

            Material mat = target.cosmetics.currentBodySprite.BodySprite.material;
            if (hasVisibleShield)
            {
                mat.SetFloat("_Outline", 1f);
                mat.SetColor("_OutlineColor", Medic.ShieldedColor);
            }
            else
            {
                mat.SetFloat("_Outline", 0f);
            }
        }
    }

    internal static void UpdatePlayerInfo()
    {
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer?.Data == null)
        {
            return;
        }

        MeetingHud meeting = MeetingHud.Instance;
        bool hasMeeting = meeting?.playerStates != null;
        if (hasMeeting)
        {
            VoteAreaStates.Clear();
            foreach (PlayerVoteArea s in meeting.playerStates)
            {
                if (s != null)
                {
                    VoteAreaStates[s.TargetPlayerId] = s;
                }
            }
        }

        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p?.Data == null || p.cosmetics == null)
            {
                continue;
            }

            PlayerVoteArea pva = null;
            if (hasMeeting)
            {
                VoteAreaStates.TryGetValue(p.PlayerId, out pva);
            }

            // Colorblind Text Handling
            if (pva?.ColorBlindName != null && pva.ColorBlindName.gameObject.active)
            {
                pva.ColorBlindName.transform.localPosition = ColorBlindMeetingPos;
                pva.ColorBlindName.transform.localScale = ColorBlindMeetingScale;
            }

            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active)
            {
                p.cosmetics.colorBlindText.transform.localPosition = new(0, -0.25f, 0f);
            }

            p.cosmetics.nameText?.transform.parent?.SetLocalZ(-0.0001f);

            if (p == localPlayer || localPlayer.Data.IsDead)
            {
                TextMeshPro label = GetOrCreateLabel(p.cosmetics.nameText, "Info", 0.225f, 0.75f);
                if (label == null)
                {
                    continue;
                }

                TextMeshPro meetingLabel = null;
                if (pva != null)
                {
                    meetingLabel = GetOrCreateLabel(pva.NameText, "Info", -0.2f, 0.60f);
                    if (meetingLabel != null && pva.NameText != null)
                    {
                        pva.NameText.transform.localPosition = new(0.3384f, 0.0311f, -0.1f);
                    }
                }

                (int completed, int total) = TasksHandler.TaskInfo(p.Data);
                string roleBase = RoleInfo.GetRolesString(p, true, false);
                string roleGhost = RoleInfo.GetRolesString(p, true, MapSettings.GhostsSeeModifier);

                string statusText = "";
                if (p == localPlayer || localPlayer.Data.IsDead && MapSettings.GhostsSeeInformation)
                {
                    if (p.IsRole(RoleType.Arsonist))
                    {
                        Arsonist role = Arsonist.Instance;
                        if (role != null)
                        {
                            int dousedSurvivors = 0;
                            foreach (PlayerControl dousedPlayer in role.DousedPlayers)
                            {
                                if (dousedPlayer?.Data != null && !dousedPlayer.Data.IsDead && !dousedPlayer.Data.Disconnected)
                                {
                                    dousedSurvivors++;
                                }
                            }

                            int totalSurvivors = 0;
                            foreach (PlayerControl targetPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                            {
                                if (targetPlayer?.Data != null
                                    && !targetPlayer.Data.IsDead
                                    && !targetPlayer.Data.Disconnected
                                    && !targetPlayer.IsRole(RoleType.Arsonist)
                                    && !targetPlayer.IsGm())
                                {
                                    totalSurvivors++;
                                }
                            }

                            statusText = Cs(Arsonist.NameColor, $" ({dousedSurvivors}/{totalSurvivors})");
                        }
                    }
                    else if (p.IsRole(RoleType.Vulture))
                    {
                        Vulture role = Vulture.Instance;
                        if (role != null)
                        {
                            statusText = Cs(Vulture.NameColor, $" ({role.EatenBodies}/{Vulture.NumberToWin})");
                        }
                    }
                }

                string taskText = "";
                if (total > 0)
                {
                    InfoStringBuilder.Clear();

                    bool commsActive = false;
                    if (MapUtilities.CachedShipStatus != null && MapUtilities.Systems.TryGetValue(SystemTypes.Comms, out UnityObject comms))
                    {
                        IActivatable activatable = comms.TryCast<IActivatable>();
                        if (activatable != null)
                        {
                            commsActive = activatable.IsActive;
                        }
                    }

                    if (commsActive)
                    {
                        InfoStringBuilder.Append("<color=#808080FF>(?/?)</color>");
                    }
                    else
                    {
                        string color = completed == total ? "#00FF00FF" : "#FAD934FF";
                        InfoStringBuilder
                            .Append("<color=")
                            .Append(color)
                            .Append(">(")
                            .Append(completed)
                            .Append('/')
                            .Append(total)
                            .Append(")</color>");
                    }

                    taskText = InfoStringBuilder.ToString();
                }

                string pInfo = "";
                string mInfo = "";
                if (p == localPlayer)
                {
                    string roles = (p.Data.IsDead ? roleGhost : roleBase) + statusText;
                    if (p.IsRole(RoleType.NiceSwapper))
                    {
                        InfoStringBuilder.Clear();
                        InfoStringBuilder.Append(roles).Append("<color=#FAD934FF> (").Append(Swapper.RemainSwaps).Append(")</color>");
                        pInfo = InfoStringBuilder.ToString();
                    }
                    else
                    {
                        if (p.IsTeamCrewmate() || p.IsRole(RoleType.Arsonist) || p.IsRole(RoleType.Vulture))
                        {
                            InfoStringBuilder.Clear();
                            InfoStringBuilder.Append(roles).Append(' ').Append(taskText);
                            pInfo = InfoStringBuilder.ToString();
                        }
                        else
                        {
                            pInfo = roles;
                        }
                    }

                    if (HudManager.Instance?.TaskPanel?.tab != null)
                    {
                        Transform tabTextObj = HudManager.Instance.TaskPanel.tab.transform.Find("TabText_TMP");
                        if (tabTextObj != null)
                        {
                            TextMeshPro tabText = tabTextObj.GetComponent<TextMeshPro>();
                            if (tabText != null)
                            {
                                InfoStringBuilder.Clear();
                                InfoStringBuilder.Append(TranslationController.Instance.GetString(StringNames.Tasks)).Append(' ').Append(taskText);
                                tabText.SetText(InfoStringBuilder.ToString());
                            }
                        }
                    }

                    InfoStringBuilder.Clear();
                    InfoStringBuilder.Append(roles).Append(' ').Append(taskText);
                    mInfo = InfoStringBuilder.ToString().Trim();
                }
                else if (MapSettings.GhostsSeeRoles && MapSettings.GhostsSeeInformation)
                {
                    InfoStringBuilder.Clear();
                    InfoStringBuilder.Append(roleGhost).Append(statusText).Append(' ').Append(taskText);
                    pInfo = InfoStringBuilder.ToString().Trim();
                    mInfo = pInfo;
                }
                else if (MapSettings.GhostsSeeInformation)
                {
                    InfoStringBuilder.Clear();
                    InfoStringBuilder.Append(taskText).Append(statusText);
                    pInfo = InfoStringBuilder.ToString().Trim();
                    mInfo = pInfo;
                }
                else if (MapSettings.GhostsSeeRoles)
                {
                    pInfo = roleGhost;
                    mInfo = pInfo;
                }

                label.text = pInfo;
                label.gameObject.SetActive(p.Visible);
                meetingLabel?.text = meeting != null && meeting.state == MeetingHud.VoteStates.Results ? "" : mInfo;
            }
        }
    }

    private static TextMeshPro GetOrCreateLabel(TextMeshPro source, string name, float yOffset, float fontScale)
    {
        if (source?.transform?.parent == null)
        {
            return null;
        }

        Transform labelTransform = source.transform.parent.Find(name);
        TextMeshPro label = labelTransform?.GetComponent<TextMeshPro>();

        if (label == null)
        {
            label = Object.Instantiate(source, source.transform.parent);
            if (label == null)
            {
                return null;
            }
            label.transform.localPosition += Vector3.up * yOffset;
            label.fontSize *= fontScale;
            label.gameObject.name = name;
            label.color = label.color.SetAlpha(1f);
        }

        return label;
    }

    internal static void SetPetVisibility()
    {
        bool localDead = PlayerControl.LocalPlayer.Data.IsDead;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            p.cosmetics.SetPetVisible(localDead && !p.Data.IsDead || !localDead);
        }
    }

    internal static void ShareGameVersion(int targetId = -1)
    {
        if (AmongUsClient.Instance.ClientId < 0)
        {
            return;
        }

        Version ver = RebuildUs.Instance.Version;
        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VersionHandshake, targetId))
        {
            sender.Write((byte)ver.Major);
            sender.Write((byte)ver.Minor);
            sender.Write((byte)ver.Build);
            sender.WritePacked(AmongUsClient.Instance.ClientId);
            sender.Write((byte)(ver.Revision < 0 ? 0xFF : ver.Revision));
            sender.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        }

        if (targetId == -1 || targetId == AmongUsClient.Instance.ClientId)
        {
            RPCProcedure.VersionHandshake(ver.Major,
                ver.Minor,
                ver.Build,
                ver.Revision,
                AmongUsClient.Instance.ClientId,
                Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId);
        }
    }

    internal static string PadRightV2(this object text, int num)
    {
        int bc = 0;
        string t = text.ToString();
        foreach (char c in t)
        {
            bc += Encoding.UTF8.GetByteCount(c.ToString()) == 1 ? 1 : 2;
        }
        return t?.PadRight(num - (bc - t.Length));
    }

    internal static string RemoveHtml(this string text)
    {
        return Regex.Replace(text, "<[^>]*?>", "");
    }

    internal static bool HidePlayerName(PlayerControl target)
    {
        return HidePlayerName(PlayerControl.LocalPlayer, target);
    }

    internal static bool HidePlayerName(PlayerControl source, PlayerControl target)
    {
        if (source == target)
        {
            return false;
        }
        if (source == null || target == null)
        {
            return true;
        }
        if (source.IsDead())
        {
            return false;
        }
        if (target.IsDead())
        {
            return true;
        }
        if (Camouflager.CamouflageTimer > 0f)
        {
            return true;
        }

        if (MapSettings.HideOutOfSightNametags && GameStarted && MapUtilities.CachedShipStatus != null)
        {
            float distMod = 1.025f;
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            bool blocked = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

            if (distance > MapUtilities.CachedShipStatus.CalculateLightRadius(source.Data) * distMod || blocked)
            {
                return true;
            }
        }

        if (!MapSettings.HidePlayerNames)
        {
            return false;
        }
        if (source.IsTeamImpostor()
            && (target.IsTeamImpostor()
                || target.IsRole(RoleType.Spy)
                || target.IsRole(RoleType.Sidekick) && Sidekick.Instance.WasTeamRed
                || target.IsRole(RoleType.Jackal) && Jackal.Instance.WasTeamRed))
        {
            return false;
        }
        if (source.GetPartner() == target)
        {
            return false;
        }
        if ((source.IsRole(RoleType.Jackal) || source.IsRole(RoleType.Sidekick))
            && (target.IsRole(RoleType.Jackal) || target.IsRole(RoleType.Sidekick) || target == Jackal.Instance.FakeSidekick))
        {
            return false;
        }

        return true;
    }

    internal static void OnObjectDestroy(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        string name = obj.name;
        if (name == null)
        {
            return;
        }

        // night vision
        if (name.Contains("FungleSecurity"))
        {
            // SurveillanceMinigamePatch.resetNightVision();
            return;
        }

        // submerged
        if (!SubmergedCompatibility.IsSubmerged)
        {
            return;
        }

        if (!name.Contains("ExileCutscene"))
        {
            return;
        }
        ExileController controller = obj.GetComponent<ExileController>();
        if (controller != null && controller.initData != null)
        {
            ExileControllerPatch.WrapUpPostfix(controller.initData.networkedPlayer?.Object);
        }
    }

    internal static void ShowFlash(Color color, float duration = 1f)
    {
        HudManager hud = FastDestroyableSingleton<HudManager>.Instance;
        if (hud?.FullScreen == null)
        {
            return;
        }

        hud.FullScreen.gameObject.SetActive(true);
        hud.FullScreen.enabled = true;
        hud.StartCoroutine(Effects.Lerp(duration,
            new Action<float>(p =>
            {
                SpriteRenderer renderer = hud.FullScreen;
                if (renderer == null)
                {
                    return;
                }

                float alpha = p < 0.5f ? p * 2f * 0.75f : (1f - p) * 2f * 0.75f;
                renderer.color = new(color.r, color.g, color.b, Mathf.Clamp01(alpha));

                if (p == 1f)
                {
                    bool reactorActive = false;
                    foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    {
                        if (task.TaskType == TaskTypes.StopCharles)
                        {
                            reactorActive = true;
                            break;
                        }
                    }

                    if (!reactorActive && IsAirship)
                    {
                        renderer.color = Color.black;
                    }
                    renderer.gameObject.SetActive(false);
                }
            })));
    }

    internal static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> iList)
    {
        return [.. iList];
    }

    internal static T Random<T>(this IList<T> self)
    {
        return self.Count > 0 ? self[RebuildUs.Rnd.Next(0, self.Count)] : default;
    }

    internal static void SetKillTimerUnchecked(this PlayerControl player, float time, float max = float.NegativeInfinity)
    {
        if (max == float.NegativeInfinity)
        {
            max = time;
        }
        player.killTimer = time;
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
    }

    internal static void Shuffle<T>(this IList<T> self, int startAt = 0)
    {
        for (int i = startAt; i < self.Count - 1; i++)
        {
            int index = RebuildUs.Rnd.Next(i, self.Count);
            (self[index], self[i]) = (self[i], self[index]);
        }
    }

    internal static PlainShipRoom GetPlainShipRoom(PlayerControl p)
    {
        Collider2D[] buffer = new Collider2D[10];
        ContactFilter2D filter = new()
        {
            layerMask = Constants.PlayersOnlyMask,
            useLayerMask = true,
            useTriggers = false,
        };
        Il2CppReferenceArray<PlainShipRoom> rooms = MapUtilities.CachedShipStatus?.AllRooms;
        if (rooms == null)
        {
            return null;
        }

        foreach (PlainShipRoom room in rooms)
        {
            if (room.roomArea == null)
            {
                continue;
            }
            int hits = room.roomArea.OverlapCollider(filter, buffer);
            for (int i = 0; i < hits; i++)
            {
                if (buffer[i]?.gameObject == p.gameObject)
                {
                    return room;
                }
            }
        }

        return null;
    }

    internal static bool IsOnElecTask()
    {
        return Camera.main.gameObject.GetComponentInChildren<SwitchMinigame>() != null;
    }

    internal static int GetOption(Int32OptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetInt(opt);
    }

    internal static int[] GetOption(Int32ArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetIntArray(opt);
    }

    internal static float GetOption(FloatOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloat(opt);
    }

    internal static float[] GetOption(FloatArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloatArray(opt);
    }

    internal static bool GetOption(BoolOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetBool(opt);
    }

    internal static byte GetOption(ByteOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetByte(opt);
    }

    internal static void SetOption(Int32OptionNames opt, int value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(opt, value);
    }

    internal static void SetOption(FloatOptionNames opt, float value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetFloat(opt, value);
    }

    internal static void SetOption(BoolOptionNames opt, bool value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(opt, value);
    }

    internal static void SetOption(ByteOptionNames opt, byte value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetByte(opt, value);
    }

    internal static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                Il2CppStructArray<byte> bytes = Il2CppSystem.IO.File.ReadAllBytes(path);
                texture.LoadImage(bytes, false);
                return texture;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Error loading texture from disk (" + path + "): " + e.Message);
        }

        return null;
    }

    internal static RoleTypes GetOptionIcon(this CustomOption option)
    {
        return option.Type switch
        {
            CustomOptionType.General => RoleTypes.Crewmate,
            CustomOptionType.Crewmate => RoleTypes.Scientist,
            CustomOptionType.Impostor => RoleTypes.Shapeshifter,
            CustomOptionType.Neutral => RoleTypes.Noisemaker,
            CustomOptionType.Modifier => RoleTypes.GuardianAngel,
            _ => RoleTypes.Crewmate,
        };
    }

    internal static bool HasAliveKillingLover(this PlayerControl player)
    {
        return Lovers.ExistingAndAlive(player) && Lovers.ExistingWithKiller(player) && player != null && player.IsLovers();
    }

    internal static bool IsDead(this PlayerControl player)
    {
        if (player == null)
        {
            return true;
        }
        NetworkedPlayerInfo data = player.Data;
        if (data == null || data.IsDead || data.Disconnected)
        {
            return true;
        }

        return GameHistory.FinalStatuses != null
               && GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out FinalStatus status)
               && status != FinalStatus.Alive;
    }

    internal static bool IsAlive(this PlayerControl player)
    {
        return !player.IsDead();
    }

    internal static bool IsNeutral(this PlayerControl player)
    {
        return player != null
               && (player.IsRole(RoleType.Jackal)
                   || player.IsRole(RoleType.Sidekick)
                   || Jackal.FormerJackals.Contains(player)
                   || player.IsRole(RoleType.Arsonist)
                   || player.IsRole(RoleType.Jester)
                   ||
                   // player.IsRole(RoleType.Opportunist) ||
                   player.IsRole(RoleType.Vulture)
                   || player.IsRole(RoleType.Shifter) && Shifter.IsNeutral);
    }

    internal static bool IsTeamCrewmate(this PlayerControl player)
    {
        return player != null && !player.IsTeamImpostor() && !player.IsNeutral() && !player.IsGm();
    }

    internal static bool IsTeamImpostor(this PlayerControl player)
    {
        return player?.Data?.Role != null && player.Data.Role.IsImpostor;
    }

    internal static bool NeutralHasTasks(this PlayerControl player)
    {
        return player.IsNeutral() && player.IsRole(RoleType.Shifter);
    }

    internal static bool IsGm(this PlayerControl player)
    {
        return false;
        // return GM.gm != null && player == GM.gm;
    }

    internal static bool IsLovers(this PlayerControl player)
    {
        return player != null && Lovers.IsLovers(player);
    }

    internal static PlayerControl GetPartner(this PlayerControl player)
    {
        return Lovers.GetPartner(player);
    }

    internal static bool CanBeErased(this PlayerControl player)
    {
        return !player.IsRole(RoleType.Jackal) && !player.IsRole(RoleType.Sidekick) && !Jackal.FormerJackals.Contains(player);
    }

    internal static bool CanUseVents(this PlayerControl player)
    {
        bool roleCouldUse = false;
        if (player.IsRole(RoleType.Engineer))
        {
            roleCouldUse = true;
        }
        else if (Jackal.CanUseVents && player.IsRole(RoleType.Jackal))
        {
            roleCouldUse = true;
        }
        else if (Sidekick.CanUseVents && player.IsRole(RoleType.Sidekick))
        {
            roleCouldUse = true;
        }
        else if (Spy.CanEnterVents && player.IsRole(RoleType.Spy))
        {
            roleCouldUse = true;
        }
        else if (Madmate.CanEnterVents && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (MadmateRole.CanEnterVents && player.IsRole(RoleType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (Suicider.CanEnterVents && player.IsRole(RoleType.Suicider))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.CanEnterVents && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Vulture.CanUseVents && player.IsRole(RoleType.Vulture))
        {
            roleCouldUse = true;
        }
        else if (player.Data?.Role != null && player.Data.Role.CanVent)
        {
            if (!Mafia.Janitor.CanVent && player.IsRole(RoleType.Janitor))
            {
                roleCouldUse = false;
            }
            else if (!Mafia.Mafioso.CanVent && player.IsRole(RoleType.Mafioso))
            {
                roleCouldUse = false;
            }
            else
            {
                roleCouldUse = true;
            }
        }

        return roleCouldUse;
    }

    internal static bool CanSabotage(this PlayerControl player)
    {
        bool roleCouldUse = false;
        if (Madmate.CanSabotage && player.HasModifier(ModifierType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (MadmateRole.CanSabotage && player.IsRole(RoleType.Madmate))
        {
            roleCouldUse = true;
        }
        else if (CreatedMadmate.CanSabotage && player.HasModifier(ModifierType.CreatedMadmate))
        {
            roleCouldUse = true;
        }
        else if (Jester.CanSabotage && player.IsRole(RoleType.Jester))
        {
            roleCouldUse = true;
        }
        else if (!Mafia.Mafioso.CanSabotage && player.IsRole(RoleType.Mafioso))
        {
            roleCouldUse = false;
        }
        else if (!Mafia.Janitor.CanSabotage && player.IsRole(RoleType.Janitor))
        {
            roleCouldUse = false;
        }
        else if (player.Data?.Role != null && player.Data.Role.IsImpostor)
        {
            roleCouldUse = true;
        }

        return roleCouldUse;
    }

    internal static ClientData GetClient(this PlayerControl player)
    {
        if (player == null)
        {
            return null;
        }
        Il2CppSystem.Collections.Generic.List<ClientData> allClients = AmongUsClient.Instance.allClients;
        for (int i = 0; i < allClients.Count; i++)
        {
            ClientData cd = allClients[i];
            if (cd?.Character != null && cd.Character.PlayerId == player.PlayerId)
            {
                return cd;
            }
        }

        return null;
    }

    internal static string GetPlatform(this PlayerControl player)
    {
        ClientData client = player.GetClient();
        return client != null ? client.PlatformData.Platform.ToString() : "Unknown";
    }

    internal static string GetRoleName(this PlayerControl player)
    {
        return RoleInfo.GetRolesString(player, false, joinSeparator: " + ");
    }

    internal static string GetNameWithRole(this PlayerControl player)
    {
        if (player == null || player.Data == null)
        {
            return "";
        }
        RoleStringBuilder.Clear();
        string name = player.Data.PlayerName;
        if (Camouflager.CamouflageTimer > 0f)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = player.Data.DefaultOutfit.PlayerName;
            }
        }
        else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
        {
            name = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
        }

        RoleStringBuilder.Append(name).Append(" (").Append(player.GetRoleName()).Append(')');
        return RoleStringBuilder.ToString();
    }

    internal static void MurderPlayer(this PlayerControl player, PlayerControl target)
    {
        player.MurderPlayer(target, MurderResultFlags.Succeeded);
    }

    internal static bool IsBlocked(PlayerTask task, PlayerControl pc)
    {
        if (task == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        TaskTypes taskType = task.TaskType;
        bool isLights = taskType == TaskTypes.FixLights;
        bool isComms = taskType == TaskTypes.FixComms;
        bool isReactor = taskType is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor;
        bool isO2 = taskType == TaskTypes.RestoreOxy;

        if (pc.IsRole(RoleType.NiceSwapper) && (isLights || isComms))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.Madmate) && (isLights || isComms && !Madmate.CanFixComm))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Madmate) && (isLights || isComms && !MadmateRole.CanFixComm))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Suicider) && (isLights || isComms && !Suicider.CanFixComm))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.CreatedMadmate) && (isLights || isComms && !CreatedMadmate.CanFixComm))
        {
            return true;
        }

        if (pc.IsGm() && (isLights || isComms || isReactor || isO2))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanRepair && (isLights || isComms))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Janitor) && !Mafia.Janitor.CanRepair && (isLights || isComms))
        {
            return true;
        }

        return false;
    }

    internal static bool IsBlocked(Console console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        PlayerTask task = console.FindTask(pc);
        return IsBlocked(task, pc);
    }

    internal static bool IsBlocked(SystemConsole console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        string name = console.name;
        bool isSecurity = name is "task_cams" or "Surv_Panel" or "SurvLogConsole" or "SurvConsole";
        bool isVitals = name == "panel_vitals";

        return isSecurity && !MapSettings.CanUseCameras || isVitals && !MapSettings.CanUseVitals;
    }

    internal static bool IsBlocked(IUsable target, PlayerControl pc)
    {
        if (target == null)
        {
            return false;
        }

        Console targetConsole = target.TryCast<Console>();
        if (targetConsole != null)
        {
            return IsBlocked(targetConsole, pc);
        }

        SystemConsole targetSysConsole = target.TryCast<SystemConsole>();
        if (targetSysConsole != null)
        {
            return IsBlocked(targetSysConsole, pc);
        }

        MapConsole targetMapConsole = target.TryCast<MapConsole>();
        if (targetMapConsole != null)
        {
            return !MapSettings.CanUseAdmin;
        }

        return false;
    }
}