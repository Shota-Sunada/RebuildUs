using System.Reflection;
using System.Text.RegularExpressions;
using InnerNet;
using Object = UnityEngine.Object;

namespace RebuildUs;

public enum MurderAttemptResult
{
    PerformKill,
    SuppressKill,
    BlankKill,
    DelayVampireKill,
}

internal static class Helpers
{
    private static readonly StringBuilder COLOR_STRING_BUILDER = new();

    private static readonly StringBuilder INFO_STRING_BUILDER = new();

    private static readonly Dictionary<byte, PlayerControl> PLAYER_BY_ID_CACHE = [];
    private static int _lastCacheFrame = -1;

    private static readonly Vector3 COLOR_BLIND_MEETING_POS = new(0.3384f, 0.23334f, -0.11f);
    private static readonly Vector3 COLOR_BLIND_MEETING_SCALE = new(0.72f, 0.8f, 0.8f);
    private static readonly Dictionary<byte, PlayerVoteArea> VOTE_AREA_STATES = [];

    private static readonly StringBuilder ROLE_STRING_BUILDER = new();

    public static bool ShowButtons
    {
        get =>
            !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance;
    }

    public static bool ShowMeetingText
    {
        get =>
            MeetingHud.Instance != null && MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion;
    }

    public static bool GameStarted
    {
        get => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started;
    }

    public static bool RolesEnabled
    {
        get => CustomOptionHolder.ActivateRoles.GetBool();
    }

    public static bool RefundVotes
    {
        get => CustomOptionHolder.RefundVotesOnDeath.GetBool();
    }

    public static bool IsSkeld
    {
        get => GetOption(ByteOptionNames.MapId) == 0;
    }

    public static bool IsMiraHq
    {
        get => GetOption(ByteOptionNames.MapId) == 1;
    }

    public static bool IsPolus
    {
        get => GetOption(ByteOptionNames.MapId) == 2;
    }

    public static bool IsAirship
    {
        get => GetOption(ByteOptionNames.MapId) == 4;
    }

    public static bool IsFungle
    {
        get => GetOption(ByteOptionNames.MapId) == 5;
    }

    public static bool IsHideNSeekMode
    {
        get => GameManager.Instance.IsHideAndSeek();
    }

    public static bool IsNormal
    {
        get => GameManager.Instance.IsNormal();
    }

    public static bool IsCountdown
    {
        get =>
            GameStartManager.InstanceExists && GameStartManager.Instance.startState is GameStartManager.StartingStates.Countdown;
    }

    public static void DestroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : Object
    {
        if (items == null) return;
        foreach (var item in items) Object.Destroy(item);
    }

    public static void DestroyList<T>(IEnumerable<T> items) where T : Object
    {
        if (items == null) return;
        foreach (var item in items) Object.Destroy(item);
    }

    public static void GenerateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
    {
        if (player == null) return;

        var taskTypeIds = GenerateTasks(numCommon, numShort, numLong);
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedSetTasks);
        sender.Write(player.PlayerId);
        sender.WriteBytesAndSize(taskTypeIds.ToArray());
        RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
    }

    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    public static string Cs(Color c, string s)
    {
        COLOR_STRING_BUILDER.Clear();
        COLOR_STRING_BUILDER.Append("<color=#").Append(ToByte(c.r).ToString("X2")).Append(ToByte(c.g).ToString("X2")).Append(ToByte(c.b).ToString("X2")).Append(ToByte(c.a).ToString("X2")).Append('>').Append(s).Append("</color>");
        return COLOR_STRING_BUILDER.ToString();
    }

    private static byte ToByte(float f)
    {
        return (byte)(Mathf.Clamp01(f) * 255);
    }

    public static int LineCount(string text)
    {
        return text.Count(c => c == '\n');
    }

    public static bool GetKeysDown(params KeyCode[] keys)
    {
        if (keys.Length == 0) return false;
        var allHeld = true;
        var anyJustPressed = false;
        foreach (var key in keys)
        {
            if (!Input.GetKey(key))
            {
                allHeld = false;
                break;
            }

            if (Input.GetKeyDown(key)) anyJustPressed = true;
        }

        return allHeld && anyJustPressed;
    }

    public static bool HasFakeTasks(this PlayerControl player)
    {
        return (player.IsNeutral() && !player.NeutralHasTasks()) || (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks) || (player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks) || (player.IsRole(RoleType.Madmate) && !MadmateRole.CanKnowImpostorAfterFinishTasks) || (player.IsRole(RoleType.Suicider) && !Suicider.CanKnowImpostorAfterFinishTasks) || (player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount);
    }

    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == id)
                return player;
        }

        return null;
    }

    public static Dictionary<byte, PlayerControl> AllPlayersById()
    {
        if (Time.frameCount == _lastCacheFrame) return PLAYER_BY_ID_CACHE;

        _lastCacheFrame = Time.frameCount;
        PLAYER_BY_ID_CACHE.Clear();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null)
                PLAYER_BY_ID_CACHE[p.PlayerId] = p;
        }

        return PLAYER_BY_ID_CACHE;
    }

    public static void HandleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        var killer = Vampire.AllPlayers.FirstOrDefault();
        if (killer != null && Vampire.Bitten != null) CheckMurderAttemptAndKill(killer, Vampire.Bitten, true, false);

        using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
        {
            sender.Write(byte.MaxValue);
            sender.Write(byte.MaxValue);
        }

        RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
    }

    public static void RefreshRoleDescription(PlayerControl player)
    {
        if (player == null) return;

        var infos = RoleInfo.GetRoleInfoForPlayer(player);
        var toRemove = new List<PlayerTask>();

        foreach (var t in player.myTasks.GetFastEnumerator())
        {
            var textTask = t.TryCast<ImportantTextTask>();
            if (textTask != null)
            {
                var found = false;
                for (var i = 0; i < infos.Count; i++)
                {
                    if (textTask.Text.StartsWith(infos[i].Name))
                    {
                        infos.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found) toRemove.Add(t);
            }
        }

        foreach (var t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            Object.Destroy(t.gameObject);
        }

        foreach (var roleInfo in infos)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            INFO_STRING_BUILDER.Clear();
            INFO_STRING_BUILDER.Append(roleInfo.Name).Append(": ");
            if (roleInfo.RoleType == RoleType.Jackal)
                INFO_STRING_BUILDER.Append(Jackal.CanCreateSidekick ? Tr.Get(TrKey.JackalWithSidekick) : Tr.Get(TrKey.JackalShortDesc));
            else
                INFO_STRING_BUILDER.Append(roleInfo.ShortDescription);

            task.Text = Cs(roleInfo.Color, INFO_STRING_BUILDER.ToString());
            player.myTasks.Insert(0, task);
        }

        if (player.HasModifier(ModifierType.Madmate) || player.HasModifier(ModifierType.CreatedMadmate))
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            INFO_STRING_BUILDER.Clear();
            INFO_STRING_BUILDER.Append(Madmate.FullName).Append(": ").Append(Tr.Get(TrKey.MadmateShortDesc));
            task.Text = Cs(Madmate.NameColor, INFO_STRING_BUILDER.ToString());
            player.myTasks.Insert(0, task);
        }
    }

    public static bool IsLighterColor(int colorId)
    {
        return CustomColors.LighterColors.Contains(colorId);
    }

    public static bool MushroomSabotageActive()
    {
        foreach (var task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.TaskType == TaskTypes.MushroomMixupSabotage)
                return true;
        }

        return false;
    }

    public static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        var alpha = value ? 0.25f : 1f;
        foreach (var r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = r.color;
            r.color = new(c.r, c.g, c.b, alpha);
        }

        var nameTxt = player.cosmetics.nameText;
        nameTxt.color = new(nameTxt.color.r, nameTxt.color.g, nameTxt.color.b, alpha);
    }

    public static List<byte> GenerateTasks(int numCommon, int numShort, int numLong)
    {
        if (numCommon + numShort + numLong <= 0) numShort = 1;

        var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
        var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

        var commonTasks = new List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.CommonTasks) commonTasks.Add(task);
        commonTasks.Shuffle();

        var shortTasks = new List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.ShortTasks) shortTasks.Add(task);
        shortTasks.Shuffle();

        var longTasks = new List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.LongTasks) longTasks.Add(task);
        longTasks.Shuffle();

        var start = 0;
        var commonTasksIl2Cpp = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var t in commonTasks) commonTasksIl2Cpp.Add(t);
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasksIl2Cpp);

        start = 0;
        var shortTasksIl2Cpp = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var t in shortTasks) shortTasksIl2Cpp.Add(t);
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasksIl2Cpp);

        start = 0;
        var longTasksIl2Cpp = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var t in longTasks) longTasksIl2Cpp.Add(t);
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasksIl2Cpp);

        return [.. tasks];
    }

    public static void ClearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var t in player.myTasks)
        {
            t.OnRemove();
            Object.Destroy(t.gameObject);
        }

        player.myTasks.Clear();
        player.Data?.Tasks?.Clear();
    }

    public static bool IsCrewmateAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamCrewmate() && !p.HasModifier(ModifierType.Madmate) && !p.IsRole(RoleType.Madmate) && !p.IsRole(RoleType.Suicider) && p.IsAlive())
                return true;
        }

        return false;
    }

    public static bool HasImpostorVision(PlayerControl player)
    {
        if (player.IsTeamImpostor()) return true;

        var isFormerJackal = false;
        foreach (var p in Jackal.FormerJackals)
        {
            if (p.PlayerId == player.PlayerId)
            {
                isFormerJackal = true;
                break;
            }
        }

        return ((player.IsRole(RoleType.Jackal) || isFormerJackal) && Jackal.HasImpostorVision) || (player.IsRole(RoleType.Sidekick) && Sidekick.HasImpostorVision) || (player.IsRole(RoleType.Spy) && Spy.HasImpostorVision) || (player.IsRole(RoleType.Madmate) && MadmateRole.HasImpostorVision) || (player.IsRole(RoleType.Suicider) && Suicider.HasImpostorVision) || (player.IsRole(RoleType.Jester) && Jester.HasImpostorVision);
    }

    public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        var maxKey = byte.MaxValue;
        var maxValue = int.MinValue;

        foreach (var pair in self)
        {
            if (pair.Value > maxValue)
            {
                maxValue = pair.Value;
                maxKey = pair.Key;
                tie = false;
            }
            else if (pair.Value == maxValue) tie = true;
        }

        return new(maxKey, maxValue);
    }

    public static MurderAttemptResult CheckMurderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false)
    {
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill;
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected)
            return MurderAttemptResult.SuppressKill;
        if (IsHideNSeekMode) return MurderAttemptResult.PerformKill;

        if (Medic.Exists && !ignoreMedic && Medic.Shielded != null && Medic.Shielded == target)
        {
            using (new RPCSender(killer.NetId, CustomRPC.ShieldedMurderAttempt))
            {
                RPCProcedure.ShieldedMurderAttempt();
            }

            return MurderAttemptResult.SuppressKill;
        }

        if (Mini.Exists && target.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(target))
            return MurderAttemptResult.SuppressKill;

        if (TimeMaster.Exists && TimeMaster.ShieldActive && target.IsRole(RoleType.TimeMaster))
        {
            if (!blockRewind)
            {
                using (new RPCSender(killer.NetId, CustomRPC.TimeMasterRewindTime))
                {
                    RPCProcedure.TimeMasterRewindTime();
                }
            }

            return MurderAttemptResult.SuppressKill;
        }

        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer))
        {
            sender.Write(killer.PlayerId);
            sender.Write(target.PlayerId);
            sender.Write(showAnimation ? byte.MaxValue : (byte)0);
        }

        RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        var murder = CheckMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);
        Logger.LogMessage(Enum.GetName(typeof(MurderAttemptResult), murder));

        if (murder == MurderAttemptResult.PerformKill)
            MurderPlayer(killer, target, showAnimation);
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>(p =>
            {
                if (!TransportationToolPatches.IsUsingTransportation(target) && Vampire.Bitten != null)
                {
                    using (new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
                    {
                        RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
                    }

                    MurderPlayer(killer, target, showAnimation);
                }
            })));
        }

        return murder;
    }

    public static bool SabotageActive()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;
    }

    public static float SabotageTimer()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;
    }

    public static bool CanUseSabotage()
    {
        var sabSystem = MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (MapUtilities.Systems.TryGetValue(SystemTypes.Doors, out var systemType))
            doors = systemType.CastFast<IActivatable>();
        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, int killDistanceIdx = -1)
    {
        if (!MapUtilities.CachedShipStatus) return null;

        targetingPlayer ??= PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent || targetingPlayer.IsGm()) return null;

        var num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(killDistanceIdx == -1 ? GetOption(Int32OptionNames.KillDistance) : killDistanceIdx, 0, 2)];
        untargetablePlayers ??= [];

        var truePosition = targetingPlayer.GetTruePosition();
        PlayerControl result = null;

        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (playerInfo.Disconnected || playerInfo.PlayerId == targetingPlayer.PlayerId || playerInfo.IsDead) continue;
            if (onlyCrewmates && playerInfo.Role.IsImpostor) continue;

            var obj = playerInfo.Object;
            if (obj == null || (obj.inVent && !targetPlayersInVents)) continue;

            var untargetable = false;
            foreach (var utp in untargetablePlayers)
            {
                if (utp == obj)
                {
                    untargetable = true;
                    break;
                }
            }

            if (untargetable) continue;

            var vector = obj.GetTruePosition() - truePosition;
            var magnitude = vector.magnitude;
            if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
            {
                result = obj;
                num = magnitude;
            }
        }

        return result;
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target?.cosmetics?.currentBodySprite?.BodySprite == null) return;

        var mat = target.cosmetics.currentBodySprite.BodySprite.material;
        mat.SetFloat("_Outline", 1f);
        mat.SetColor("_OutlineColor", color);
    }

    public static void SetBasePlayerOutlines()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        foreach (var target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target?.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            var isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
            var hasVisibleShield = false;
            if (Camouflager.CamouflageTimer <= 0f && Medic.Shielded != null && ((target == Medic.Shielded && !isMorphedMorphing) || (isMorphedMorphing && Morphing.MorphTarget == Medic.Shielded)))
            {
                hasVisibleShield = Medic.ShowShielded switch
                {
                    0 => true, // Everyone
                    1 => localPlayer == Medic.Shielded || localPlayer.IsRole(RoleType.Medic), // Shielded + Medic
                    2 => localPlayer.IsRole(RoleType.Medic), // Medic only
                    _ => false,
                };
            }

            var mat = target.cosmetics.currentBodySprite.BodySprite.material;
            if (hasVisibleShield)
            {
                mat.SetFloat("_Outline", 1f);
                mat.SetColor("_OutlineColor", Medic.ShieldedColor);
            }
            else
                mat.SetFloat("_Outline", 0f);
        }
    }

    public static void UpdatePlayerInfo()
    {
        if (MapSettings.GameMode is not CustomGameMode.Roles) return;

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer?.Data == null) return;

        var meeting = MeetingHud.Instance;
        var hasMeeting = meeting?.playerStates != null;
        if (hasMeeting)
        {
            VOTE_AREA_STATES.Clear();
            foreach (var s in meeting.playerStates)
            {
                if (s != null)
                    VOTE_AREA_STATES[s.TargetPlayerId] = s;
            }
        }

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p?.Data == null || p.cosmetics == null) continue;

            PlayerVoteArea pva = null;
            if (hasMeeting) VOTE_AREA_STATES.TryGetValue(p.PlayerId, out pva);

            // Colorblind Text Handling
            if (pva?.ColorBlindName != null && pva.ColorBlindName.gameObject.active)
            {
                pva.ColorBlindName.transform.localPosition = COLOR_BLIND_MEETING_POS;
                pva.ColorBlindName.transform.localScale = COLOR_BLIND_MEETING_SCALE;
            }

            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active)
                p.cosmetics.colorBlindText.transform.localPosition = new(0, -0.25f, 0f);

            p.cosmetics.nameText?.transform.parent?.SetLocalZ(-0.0001f);

            if (p == localPlayer || localPlayer.Data.IsDead)
            {
                var label = GetOrCreateLabel(p.cosmetics.nameText, "Info", 0.225f, 0.75f);
                if (label == null) continue;

                TextMeshPro meetingLabel = null;
                if (pva != null)
                {
                    meetingLabel = GetOrCreateLabel(pva.NameText, "Info", -0.2f, 0.60f);
                    if (meetingLabel != null && pva.NameText != null)
                        pva.NameText.transform.localPosition = new(0.3384f, 0.0311f, -0.1f);
                }

                var (completed, total) = TasksHandler.TaskInfo(p.Data);
                var roleBase = RoleInfo.GetRolesString(p, true, false);
                var roleGhost = RoleInfo.GetRolesString(p, true, MapSettings.GhostsSeeModifier);

                var statusText = "";
                if (p == localPlayer || (localPlayer.Data.IsDead && MapSettings.GhostsSeeInformation))
                {
                    if (p.IsRole(RoleType.Arsonist))
                    {
                        var role = Arsonist.GetRole(p);
                        if (role != null)
                        {
                            var dousedSurvivors = 0;
                            foreach (var dousedPlayer in role.DousedPlayers)
                            {
                                if (dousedPlayer?.Data != null && !dousedPlayer.Data.IsDead && !dousedPlayer.Data.Disconnected)
                                    dousedSurvivors++;
                            }

                            var totalSurvivors = 0;
                            foreach (var targetPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                            {
                                if (targetPlayer?.Data != null && !targetPlayer.Data.IsDead && !targetPlayer.Data.Disconnected && !targetPlayer.IsRole(RoleType.Arsonist) && !targetPlayer.IsGm())
                                    totalSurvivors++;
                            }

                            statusText = Cs(Arsonist.NameColor, $" ({dousedSurvivors}/{totalSurvivors})");
                        }
                    }
                    else if (p.IsRole(RoleType.Vulture))
                    {
                        var role = Vulture.GetRole(p);
                        if (role != null)
                            statusText = Cs(Vulture.NameColor, $" ({role.EatenBodies}/{Vulture.NumberToWin})");
                    }
                }

                var taskText = "";
                if (total > 0)
                {
                    INFO_STRING_BUILDER.Clear();

                    var commsActive = false;
                    if (MapUtilities.CachedShipStatus != null && MapUtilities.Systems.TryGetValue(SystemTypes.Comms, out var comms))
                    {
                        var activatable = comms.TryCast<IActivatable>();
                        if (activatable != null) commsActive = activatable.IsActive;
                    }

                    if (commsActive)
                        INFO_STRING_BUILDER.Append("<color=#808080FF>(?/?)</color>");
                    else
                    {
                        var color = completed == total ? "#00FF00FF" : "#FAD934FF";
                        INFO_STRING_BUILDER.Append("<color=").Append(color).Append(">(").Append(completed).Append('/').Append(total).Append(")</color>");
                    }

                    taskText = INFO_STRING_BUILDER.ToString();
                }

                var pInfo = "";
                var mInfo = "";
                if (p == localPlayer)
                {
                    var roles = (p.Data.IsDead ? roleGhost : roleBase) + statusText;
                    if (p.IsRole(RoleType.NiceSwapper))
                    {
                        INFO_STRING_BUILDER.Clear();
                        INFO_STRING_BUILDER.Append(roles).Append("<color=#FAD934FF> (").Append(Swapper.RemainSwaps).Append(")</color>");
                        pInfo = INFO_STRING_BUILDER.ToString();
                    }
                    else
                    {
                        if (p.IsTeamCrewmate() || p.IsRole(RoleType.Arsonist) || p.IsRole(RoleType.Vulture))
                        {
                            INFO_STRING_BUILDER.Clear();
                            INFO_STRING_BUILDER.Append(roles).Append(' ').Append(taskText);
                            pInfo = INFO_STRING_BUILDER.ToString();
                        }
                        else
                            pInfo = roles;
                    }

                    if (HudManager.Instance?.TaskPanel?.tab != null)
                    {
                        var tabTextObj = HudManager.Instance.TaskPanel.tab.transform.Find("TabText_TMP");
                        if (tabTextObj != null)
                        {
                            var tabText = tabTextObj.GetComponent<TextMeshPro>();
                            if (tabText != null)
                            {
                                INFO_STRING_BUILDER.Clear();
                                INFO_STRING_BUILDER.Append(TranslationController.Instance.GetString(StringNames.Tasks)).Append(' ').Append(taskText);
                                tabText.SetText(INFO_STRING_BUILDER.ToString());
                            }
                        }
                    }

                    INFO_STRING_BUILDER.Clear();
                    INFO_STRING_BUILDER.Append(roles).Append(' ').Append(taskText);
                    mInfo = INFO_STRING_BUILDER.ToString().Trim();
                }
                else if (MapSettings.GhostsSeeRoles && MapSettings.GhostsSeeInformation)
                {
                    INFO_STRING_BUILDER.Clear();
                    INFO_STRING_BUILDER.Append(roleGhost).Append(statusText).Append(' ').Append(taskText);
                    pInfo = INFO_STRING_BUILDER.ToString().Trim();
                    mInfo = pInfo;
                }
                else if (MapSettings.GhostsSeeInformation)
                {
                    INFO_STRING_BUILDER.Clear();
                    INFO_STRING_BUILDER.Append(taskText).Append(statusText);
                    pInfo = INFO_STRING_BUILDER.ToString().Trim();
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
        if (source?.transform?.parent == null) return null;

        var labelTransform = source.transform.parent.Find(name);
        var label = labelTransform?.GetComponent<TextMeshPro>();

        if (label == null)
        {
            label = Object.Instantiate(source, source.transform.parent);
            if (label == null) return null;
            label.transform.localPosition += Vector3.up * yOffset;
            label.fontSize *= fontScale;
            label.gameObject.name = name;
            label.color = label.color.SetAlpha(1f);
        }

        return label;
    }

    public static void SetPetVisibility()
    {
        var localDead = PlayerControl.LocalPlayer.Data.IsDead;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            p.cosmetics.SetPetVisible((localDead && !p.Data.IsDead) || !localDead);
    }

    public static void ShareGameVersion(int targetId = -1)
    {
        if (AmongUsClient.Instance.ClientId < 0) return;

        var ver = RebuildUs.Instance.Version;
        using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VersionHandshake, targetId))
        {
            sender.Write((byte)ver.Major);
            sender.Write((byte)ver.Minor);
            sender.Write((byte)ver.Build);
            sender.WritePacked(AmongUsClient.Instance.ClientId);
            sender.Write((byte)(ver.Revision < 0 ? 0xFF : ver.Revision));
            sender.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        }

        if (targetId == -1 || targetId == AmongUsClient.Instance.ClientId)
            RPCProcedure.VersionHandshake(ver.Major, ver.Minor, ver.Build, ver.Revision, AmongUsClient.Instance.ClientId, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId);
    }

    public static string PadRightV2(this object text, int num)
    {
        var bc = 0;
        var t = text.ToString();
        foreach (var c in t) bc += Encoding.UTF8.GetByteCount(c.ToString()) == 1 ? 1 : 2;
        return t?.PadRight(num - (bc - t.Length));
    }

    public static string RemoveHtml(this string text)
    {
        return Regex.Replace(text, "<[^>]*?>", "");
    }

    public static bool HidePlayerName(PlayerControl target)
    {
        return HidePlayerName(PlayerControl.LocalPlayer, target);
    }

    public static bool HidePlayerName(PlayerControl source, PlayerControl target)
    {
        if (source == target) return false;
        if (source == null || target == null) return true;
        if (source.IsDead()) return false;
        if (target.IsDead()) return true;
        if (Camouflager.CamouflageTimer > 0f) return true;

        if (MapSettings.HideOutOfSightNametags && GameStarted && MapUtilities.CachedShipStatus != null)
        {
            var distMod = 1.025f;
            var distance = Vector3.Distance(source.transform.position, target.transform.position);
            var blocked = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

            if (distance > MapUtilities.CachedShipStatus.CalculateLightRadius(source.Data) * distMod || blocked) return true;
        }

        if (!MapSettings.HidePlayerNames) return false;
        if (source.IsTeamImpostor() && (target.IsTeamImpostor() || target.IsRole(RoleType.Spy) || (target.IsRole(RoleType.Sidekick) && Sidekick.GetRole(target).WasTeamRed) || (target.IsRole(RoleType.Jackal) && Jackal.GetRole(target).WasTeamRed))) return false;
        if (source.GetPartner() == target) return false;
        if ((source.IsRole(RoleType.Jackal) || source.IsRole(RoleType.Sidekick)) && (target.IsRole(RoleType.Jackal) || target.IsRole(RoleType.Sidekick) || target == Jackal.GetRole(target).FakeSidekick)) return false;

        return true;
    }

    public static void OnObjectDestroy(GameObject obj)
    {
        if (obj == null) return;
        var name = obj.name;
        if (name == null) return;

        // night vision
        if (name.Contains("FungleSecurity"))
        {
            // SurveillanceMinigamePatch.resetNightVision();
            return;
        }

        // submerged
        if (!SubmergedCompatibility.IsSubmerged) return;

        if (name.Contains("ExileCutscene"))
        {
            var controller = obj.GetComponent<ExileController>();
            if (controller != null && controller.initData != null)
                Exile.WrapUpPostfix(controller.initData.networkedPlayer?.Object);
        }
    }

    public static void ShowFlash(Color color, float duration = 1f)
    {
        var hud = FastDestroyableSingleton<HudManager>.Instance;
        if (hud?.FullScreen == null) return;

        hud.FullScreen.gameObject.SetActive(true);
        hud.FullScreen.enabled = true;
        hud.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var renderer = hud.FullScreen;
            if (renderer == null) return;

            var alpha = p < 0.5f ? p * 2f * 0.75f : (1f - p) * 2f * 0.75f;
            renderer.color = new(color.r, color.g, color.b, Mathf.Clamp01(alpha));

            if (p == 1f)
            {
                var reactorActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType == TaskTypes.StopCharles)
                    {
                        reactorActive = true;
                        break;
                    }
                }

                if (!reactorActive && IsAirship) renderer.color = Color.black;
                renderer.gameObject.SetActive(false);
            }
        })));
    }

    public static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> iList)
    {
        return [.. iList];
    }

    public static T Random<T>(this IList<T> self)
    {
        return self.Count > 0 ? self[RebuildUs.Instance.Rnd.Next(0, self.Count)] : default;
    }

    public static void SetKillTimerUnchecked(this PlayerControl player, float time, float max = float.NegativeInfinity)
    {
        if (max == float.NegativeInfinity) max = time;
        player.killTimer = time;
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
    }

    public static void Shuffle<T>(this IList<T> self, int startAt = 0)
    {
        for (var i = startAt; i < self.Count - 1; i++)
        {
            var index = RebuildUs.Instance.Rnd.Next(i, self.Count);
            (self[index], self[i]) = (self[i], self[index]);
        }
    }

    public static PlainShipRoom GetPlainShipRoom(PlayerControl p)
    {
        var buffer = new Collider2D[10];
        var filter = new ContactFilter2D
        {
            layerMask = Constants.PlayersOnlyMask,
            useLayerMask = true,
            useTriggers = false
        };
        var rooms = MapUtilities.CachedShipStatus?.AllRooms;
        if (rooms == null) return null;

        foreach (var room in rooms)
        {
            if (room.roomArea == null) continue;
            var hits = room.roomArea.OverlapCollider(filter, buffer);
            for (var i = 0; i < hits; i++)
            {
                if (buffer[i]?.gameObject == p.gameObject)
                    return room;
            }
        }

        return null;
    }

    public static bool IsOnElecTask()
    {
        return Camera.main.gameObject.GetComponentInChildren<SwitchMinigame>() != null;
    }

    public static int GetOption(Int32OptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetInt(opt);
    }

    public static int[] GetOption(Int32ArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetIntArray(opt);
    }

    public static float GetOption(FloatOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloat(opt);
    }

    public static float[] GetOption(FloatArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloatArray(opt);
    }

    public static bool GetOption(BoolOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetBool(opt);
    }

    public static byte GetOption(ByteOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetByte(opt);
    }

    public static void SetOption(Int32OptionNames opt, int value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(opt, value);
    }

    public static void SetOption(FloatOptionNames opt, float value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetFloat(opt, value);
    }

    public static void SetOption(BoolOptionNames opt, bool value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(opt, value);
    }

    public static void SetOption(ByteOptionNames opt, byte value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetByte(opt, value);
    }

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var bytes = Il2CppSystem.IO.File.ReadAllBytes(path);
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

    public static RoleTypes GetOptionIcon(this CustomOption option)
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

    public static bool HasAliveKillingLover(this PlayerControl player)
    {
        return Lovers.ExistingAndAlive(player) && Lovers.ExistingWithKiller(player) && player != null && player.IsLovers();
    }

    public static void UpdateMute(this PlayerControl player)
    {
        DiscordAutoMuteManager.UpdatePlayerMute(player);
    }

    public static void Mute(this PlayerControl player)
    {
        if (player == null || player.Data == null || string.IsNullOrEmpty(player.FriendCode)) return;
        if (DiscordModManager.TryGetDiscordId(player.FriendCode, out var did))
            DiscordAutoMuteManager.SetMute(did, true, true);
    }

    public static void Unmute(this PlayerControl player)
    {
        if (player == null || player.Data == null || string.IsNullOrEmpty(player.FriendCode)) return;
        if (DiscordModManager.TryGetDiscordId(player.FriendCode, out var did))
            DiscordAutoMuteManager.SetMute(did, false, false);
    }

    public static bool IsDead(this PlayerControl player)
    {
        if (player == null) return true;
        var data = player.Data;
        if (data == null || data.IsDead || data.Disconnected) return true;

        return GameHistory.FINAL_STATUSES != null && GameHistory.FINAL_STATUSES.TryGetValue(player.PlayerId, out var status) && status != FinalStatus.Alive;
    }

    public static bool IsAlive(this PlayerControl player)
    {
        return !player.IsDead();
    }

    public static bool IsNeutral(this PlayerControl player)
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
                   || (player.IsRole(RoleType.Shifter) && Shifter.IsNeutral));
    }

    public static bool IsTeamCrewmate(this PlayerControl player)
    {
        return player != null && !player.IsTeamImpostor() && !player.IsNeutral() && !player.IsGm();
    }

    public static bool IsTeamImpostor(this PlayerControl player)
    {
        return player?.Data?.Role != null && player.Data.Role.IsImpostor;
    }

    public static bool NeutralHasTasks(this PlayerControl player)
    {
        return player.IsNeutral() && player.IsRole(RoleType.Shifter);
    }

    public static bool IsGm(this PlayerControl player)
    {
        return false;
        // return GM.gm != null && player == GM.gm;
    }

    public static bool IsLovers(this PlayerControl player)
    {
        return player != null && Lovers.IsLovers(player);
    }

    public static PlayerControl GetPartner(this PlayerControl player)
    {
        return Lovers.GetPartner(player);
    }

    public static bool CanBeErased(this PlayerControl player)
    {
        return !player.IsRole(RoleType.Jackal) && !player.IsRole(RoleType.Sidekick) && !Jackal.FormerJackals.Contains(player);
    }

    public static bool CanUseVents(this PlayerControl player)
    {
        var roleCouldUse = false;

        switch (MapSettings.GameMode)
        {
            case CustomGameMode.Roles:
                if (player.IsRole(RoleType.Engineer))
                    roleCouldUse = true;
                else if (Jackal.CanUseVents && player.IsRole(RoleType.Jackal))
                    roleCouldUse = true;
                else if (Sidekick.CanUseVents && player.IsRole(RoleType.Sidekick))
                    roleCouldUse = true;
                else if (Spy.CanEnterVents && player.IsRole(RoleType.Spy))
                    roleCouldUse = true;
                else if (Madmate.CanEnterVents && player.HasModifier(ModifierType.Madmate))
                    roleCouldUse = true;
                else if (MadmateRole.CanEnterVents && player.IsRole(RoleType.Madmate))
                    roleCouldUse = true;
                else if (Suicider.CanEnterVents && player.IsRole(RoleType.Suicider))
                    roleCouldUse = true;
                else if (CreatedMadmate.CanEnterVents && player.HasModifier(ModifierType.CreatedMadmate))
                    roleCouldUse = true;
                else if (Vulture.CanUseVents && player.IsRole(RoleType.Vulture))
                    roleCouldUse = true;
                else if (player.Data?.Role != null && player.Data.Role.CanVent)
                {
                    if (!Mafia.Janitor.CanVent && player.IsRole(RoleType.Janitor))
                        roleCouldUse = false;
                    else if (!Mafia.Mafioso.CanVent && player.IsRole(RoleType.Mafioso))
                        roleCouldUse = false;
                    else
                        roleCouldUse = true;
                }

                break;
            case CustomGameMode.CaptureTheFlag:
                if (PlayerControl.LocalPlayer != CaptureTheFlag.BluePlayerWhoHasRedFlag
                    && PlayerControl.LocalPlayer != CaptureTheFlag.RedPlayerWhoHasBlueFlag
                    && ((PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer01 && !CaptureTheFlag.Redplayer01IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer02 && !CaptureTheFlag.Redplayer02IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer03 && !CaptureTheFlag.Redplayer03IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer04 && !CaptureTheFlag.Redplayer04IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer05 && !CaptureTheFlag.Redplayer05IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer06 && !CaptureTheFlag.Redplayer06IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Redplayer07 && !CaptureTheFlag.Redplayer07IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer01 && !CaptureTheFlag.Blueplayer01IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer02 && !CaptureTheFlag.Blueplayer02IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer03 && !CaptureTheFlag.Blueplayer03IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer04 && !CaptureTheFlag.Blueplayer04IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer05 && !CaptureTheFlag.Blueplayer05IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer06 && !CaptureTheFlag.Blueplayer06IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.Blueplayer07 && !CaptureTheFlag.Blueplayer07IsReviving)
                        || (PlayerControl.LocalPlayer == CaptureTheFlag.StealerPlayer && !CaptureTheFlag.StealerPlayerIsReviving)))
                    roleCouldUse = true;
                else
                    roleCouldUse = false;
                break;
            case CustomGameMode.PoliceAndThieves:
                if ((PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer01 && !PoliceAndThief.Thiefplayer01IsStealing && !PoliceAndThief.Thiefplayer01IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer02 && !PoliceAndThief.Thiefplayer02IsStealing && !PoliceAndThief.Thiefplayer02IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer03 && !PoliceAndThief.Thiefplayer03IsStealing && !PoliceAndThief.Thiefplayer03IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer04 && !PoliceAndThief.Thiefplayer04IsStealing && !PoliceAndThief.Thiefplayer04IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer05 && !PoliceAndThief.Thiefplayer05IsStealing && !PoliceAndThief.Thiefplayer05IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer06 && !PoliceAndThief.Thiefplayer06IsStealing && !PoliceAndThief.Thiefplayer06IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer07 && !PoliceAndThief.Thiefplayer07IsStealing && !PoliceAndThief.Thiefplayer07IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer08 && !PoliceAndThief.Thiefplayer08IsStealing && !PoliceAndThief.Thiefplayer08IsReviving)
                    || (PlayerControl.LocalPlayer == PoliceAndThief.Thiefplayer09 && !PoliceAndThief.Thiefplayer09IsStealing && !PoliceAndThief.Thiefplayer09IsReviving))
                    roleCouldUse = true;
                else
                    roleCouldUse = false;
                break;
            case CustomGameMode.HotPotato:
                // HP:
                if (PlayerControl.LocalPlayer == HotPotato.HotPotatoPlayer)
                    roleCouldUse = true;
                else
                    roleCouldUse = false;
                break;
            case CustomGameMode.BattleRoyale:
                // BR:
                roleCouldUse = false;
                break;
        }

        return roleCouldUse;
    }

    public static bool CanSabotage(this PlayerControl player)
    {
        var roleCouldUse = false;
        if (Madmate.CanSabotage && player.HasModifier(ModifierType.Madmate))
            roleCouldUse = true;
        else if (MadmateRole.CanSabotage && player.IsRole(RoleType.Madmate))
            roleCouldUse = true;
        else if (CreatedMadmate.CanSabotage && player.HasModifier(ModifierType.CreatedMadmate))
            roleCouldUse = true;
        else if (Jester.CanSabotage && player.IsRole(RoleType.Jester))
            roleCouldUse = true;
        else if (!Mafia.Mafioso.CanSabotage && player.IsRole(RoleType.Mafioso))
            roleCouldUse = false;
        else if (!Mafia.Janitor.CanSabotage && player.IsRole(RoleType.Janitor))
            roleCouldUse = false;
        else if (player.Data?.Role != null && player.Data.Role.IsImpostor) roleCouldUse = true;

        return roleCouldUse;
    }

    public static ClientData GetClient(this PlayerControl player)
    {
        if (player == null) return null;
        var allClients = AmongUsClient.Instance.allClients;
        for (var i = 0; i < allClients.Count; i++)
        {
            var cd = allClients[i];
            if (cd?.Character != null && cd.Character.PlayerId == player.PlayerId) return cd;
        }

        return null;
    }

    public static string GetPlatform(this PlayerControl player)
    {
        var client = player.GetClient();
        return client != null ? client.PlatformData.Platform.ToString() : "Unknown";
    }

    public static string GetRoleName(this PlayerControl player)
    {
        return RoleInfo.GetRolesString(player, false, joinSeparator: " + ");
    }

    public static string GetNameWithRole(this PlayerControl player)
    {
        if (player == null || player.Data == null) return "";
        ROLE_STRING_BUILDER.Clear();
        var name = player.Data.PlayerName;
        if (Camouflager.CamouflageTimer > 0f)
        {
            if (string.IsNullOrEmpty(name))
                name = player.Data.DefaultOutfit.PlayerName;
        }
        else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
            name = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";

        ROLE_STRING_BUILDER.Append(name).Append(" (").Append(player.GetRoleName()).Append(')');
        return ROLE_STRING_BUILDER.ToString();
    }

    public static void MurderPlayer(this PlayerControl player, PlayerControl target)
    {
        player.MurderPlayer(target, MurderResultFlags.Succeeded);
    }

    public static AudioClip GetIntroSound(RoleTypes roleType)
    {
        foreach (var role in RoleManager.Instance.AllRoles.GetFastEnumerator())
        {
            if (role.Role == roleType)
                return role.IntroSound;
        }

        return null;
    }

    internal static void AlphaPlayer(bool invisible, byte playerId)
    {
        var player = PlayerById(playerId);

        if (invisible)
        {
            player.cosmetics.nameText.color = new(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, 0.5f);
            player.cosmetics.colorBlindText.color = new(player.cosmetics.colorBlindText.color.r, player.cosmetics.colorBlindText.color.g, player.cosmetics.colorBlindText.color.b, 0.5f);
            if (!player.cosmetics.currentPet.data.IsEmpty)
            {
                if (player.cosmetics.currentPet.renderers[0] != null && player.cosmetics.currentPet.shadows[0] != null)
                {
                    player.cosmetics.currentPet.renderers[0].color = new(player.cosmetics.currentPet.renderers[0].color.r, player.cosmetics.currentPet.renderers[0].color.g, player.cosmetics.currentPet.renderers[0].color.b, 0.5f);
                    player.cosmetics.currentPet.shadows[0].color = new(player.cosmetics.currentPet.shadows[0].color.r, player.cosmetics.currentPet.shadows[0].color.g, player.cosmetics.currentPet.shadows[0].color.b, 0.5f);
                }
            }

            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.Parent.color = new(player.cosmetics.hat.Parent.color.r, player.cosmetics.hat.Parent.color.g, player.cosmetics.hat.Parent.color.b, 0.5f);
                player.cosmetics.hat.BackLayer.color = new(player.cosmetics.hat.BackLayer.color.r, player.cosmetics.hat.BackLayer.color.g, player.cosmetics.hat.BackLayer.color.b, 0.5f);
                player.cosmetics.hat.FrontLayer.color = new(player.cosmetics.hat.FrontLayer.color.r, player.cosmetics.hat.FrontLayer.color.g, player.cosmetics.hat.FrontLayer.color.b, 0.5f);
            }

            if (player.cosmetics.visor != null)
                player.cosmetics.visor.Image.color = new(player.cosmetics.visor.Image.color.r, player.cosmetics.visor.Image.color.g, player.cosmetics.visor.Image.color.b, 0.5f);
            player.MyPhysics.myPlayer.cosmetics.skin.layer.color = new(player.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 0.5f);
        }
        else
        {
            player.cosmetics.nameText.color = new(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, 1f);
            player.cosmetics.colorBlindText.color = new(player.cosmetics.colorBlindText.color.r, player.cosmetics.colorBlindText.color.g, player.cosmetics.colorBlindText.color.b, 1);
            if (!player.cosmetics.currentPet.data.IsEmpty)
            {
                if (player.cosmetics.currentPet.renderers[0] != null && player.cosmetics.currentPet.shadows[0] != null)
                {
                    player.cosmetics.currentPet.renderers[0].color = new(player.cosmetics.currentPet.renderers[0].color.r, player.cosmetics.currentPet.renderers[0].color.g, player.cosmetics.currentPet.renderers[0].color.b, 1f);
                    player.cosmetics.currentPet.shadows[0].color = new(player.cosmetics.currentPet.shadows[0].color.r, player.cosmetics.currentPet.shadows[0].color.g, player.cosmetics.currentPet.shadows[0].color.b, 1f);
                }
            }

            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.Parent.color = new(player.cosmetics.hat.Parent.color.r, player.cosmetics.hat.Parent.color.g, player.cosmetics.hat.Parent.color.b, 1f);
                player.cosmetics.hat.BackLayer.color = new(player.cosmetics.hat.BackLayer.color.r, player.cosmetics.hat.BackLayer.color.g, player.cosmetics.hat.BackLayer.color.b, 1f);
                player.cosmetics.hat.FrontLayer.color = new(player.cosmetics.hat.FrontLayer.color.r, player.cosmetics.hat.FrontLayer.color.g, player.cosmetics.hat.FrontLayer.color.b, 1f);
            }

            if (player.cosmetics.visor != null)
                player.cosmetics.visor.Image.color = new(player.cosmetics.visor.Image.color.r, player.cosmetics.visor.Image.color.g, player.cosmetics.visor.Image.color.b, 1f);
            player.MyPhysics.myPlayer.cosmetics.skin.layer.color = new(player.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 1f);
        }
    }

    public static void ActivateDleksMap()
    {
        if (!RebuildUs.ActivatedDleks && CustomOptionHolder.CustomSkeldMap.GetSelection() == 1 && GetOption(ByteOptionNames.MapId) == 0)
        {
            var dleksMap = GameObject.Find("SkeldShip(Clone)");
            dleksMap.transform.localScale = new(dleksMap.transform.localScale.x * -1, dleksMap.transform.localScale.y, dleksMap.transform.localScale.z);
            RebuildUs.ActivatedDleks = true;
        }
    }

    public static void RemoveObjectsOnGamemodes(int mapId)
    {
        switch (mapId)
        {
            case 0:
            case 3:
                // Remove camera use and admin table on Skeld / Custom Skeld / Dleks
                var cameraStand = GameObject.Find("SurvConsole");
                cameraStand.GetComponent<PolygonCollider2D>().enabled = false;
                var admin = GameObject.Find("MapRoomConsole");
                admin.GetComponent<CircleCollider2D>().enabled = false;
                break;
            case 1:
                // Remove Doorlog use, Decontamination doors and admin table on MiraHQ
                var doorLog = GameObject.Find("SurvLogConsole");
                doorLog.GetComponent<BoxCollider2D>().enabled = false;
                var deconUpperDoor = GameObject.Find("UpperDoor");
                deconUpperDoor.SetActive(false);
                var deconLowerDoor = GameObject.Find("LowerDoor");
                deconLowerDoor.SetActive(false);
                var deconUpperDoorPanelTop = GameObject.Find("DeconDoorPanel-Top");
                deconUpperDoorPanelTop.SetActive(false);
                var deconUpperDoorPanelHigh = GameObject.Find("DeconDoorPanel-High");
                deconUpperDoorPanelHigh.SetActive(false);
                var deconUpperDoorPanelBottom = GameObject.Find("DeconDoorPanel-Bottom");
                deconUpperDoorPanelBottom.SetActive(false);
                var deconUpperDoorPanelLow = GameObject.Find("DeconDoorPanel-Low");
                deconUpperDoorPanelLow.SetActive(false);
                var miraAdmin = GameObject.Find("AdminMapConsole");
                miraAdmin.GetComponent<CircleCollider2D>().enabled = false;
                break;
            case 2:
                // Remove Decon doors, camera use, vitals, admin tables on Polus
                var lowerdecon = GameObject.Find("LowerDecon");
                lowerdecon.SetActive(false);
                var upperdecon = GameObject.Find("UpperDecon");
                upperdecon.SetActive(false);
                var survCameras = GameObject.Find("Surv_Panel");
                survCameras.GetComponent<BoxCollider2D>().enabled = false;
                var vitals = GameObject.Find("panel_vitals");
                vitals.GetComponent<BoxCollider2D>().enabled = false;
                var adminone = GameObject.Find("panel_map");
                adminone.GetComponent<BoxCollider2D>().enabled = false;
                var admintwo = GameObject.Find("panel_map (1)");
                admintwo.GetComponent<BoxCollider2D>().enabled = false;
                var ramp = GameObject.Find("ramp");
                ramp.transform.position = new(ramp.transform.position.x, ramp.transform.position.y, 0.75f);
                break;
            case 4:
                // Remove camera use, admin table, vitals, electrical doors on Airship
                var cameras = GameObject.Find("task_cams");
                cameras.GetComponent<BoxCollider2D>().enabled = false;
                var airshipadmin = GameObject.Find("panel_cockpit_map");
                airshipadmin.GetComponent<BoxCollider2D>().enabled = false;
                var airshipvitals = GameObject.Find("panel_vitals");
                airshipvitals.GetComponent<CircleCollider2D>().enabled = false;

                GetStaticDoor("TopLeftVert").SetOpen(true);
                GetStaticDoor("TopLeftHort").SetOpen(true);
                GetStaticDoor("BottomHort").SetOpen(true);
                GetStaticDoor("TopCenterHort").SetOpen(true);
                GetStaticDoor("LeftVert").SetOpen(true);
                GetStaticDoor("RightVert").SetOpen(true);
                GetStaticDoor("TopRightVert").SetOpen(true);
                GetStaticDoor("TopRightHort").SetOpen(true);
                GetStaticDoor("BottomRightHort").SetOpen(true);
                GetStaticDoor("BottomRightVert").SetOpen(true);
                GetStaticDoor("LeftDoorTop").SetOpen(true);
                GetStaticDoor("LeftDoorBottom").SetOpen(true);

                var laddermeeting = GameObject.Find("ladder_meeting");
                laddermeeting.SetActive(false);
                var platform = GameObject.Find("Platform");
                platform.SetActive(false);
                var platformleft = GameObject.Find("PlatformLeft");
                platformleft.SetActive(false);
                var platformright = GameObject.Find("PlatformRight");
                platformright.SetActive(false);
                var recordsadmin = GameObject.Find("records_admin_map");
                recordsadmin.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case 5:
                // Remove Decon doors, camera use, vitals, admin tables on Fungle
                var binoculars = GameObject.Find("BinocularsSecurityConsole");
                binoculars.GetComponent<PolygonCollider2D>().enabled = false;
                var mushrooms = GameObject.Find("FungleShip(Clone)/Outside/OutsideJungle/Mushrooms");
                mushrooms.SetActive(false);
                var labvitals = GameObject.Find("FungleShip(Clone)/Rooms/Laboratory/VitalsConsole");
                labvitals.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case 6:
                // Remove camera use, admin table, vitals, on Submerged
                var upperCentralVent = GameObject.Find("UpperCentralVent");
                upperCentralVent.GetComponent<CircleCollider2D>().enabled = false;
                upperCentralVent.GetComponent<PolygonCollider2D>().enabled = false;
                var lowerCentralVent = GameObject.Find("LowerCentralVent");
                lowerCentralVent.GetComponent<BoxCollider2D>().enabled = false;
                var securityCams = GameObject.Find("SecurityConsole");
                securityCams.GetComponent<PolygonCollider2D>().enabled = false;
                var submergedvitals = GameObject.Find("panel_vitals(Clone)");
                submergedvitals.GetComponent<CircleCollider2D>().enabled = false;
                var submergedadminone = GameObject.Find("console-adm-admintable");
                submergedadminone.GetComponent<CircleCollider2D>().enabled = false;
                var submergedadmintwo = GameObject.Find("console-adm-admintable (1)");
                submergedadmintwo.GetComponent<CircleCollider2D>().enabled = false;
                var deconVLower = GameObject.Find("DeconDoorVLower");
                deconVLower.SetActive(false);
                var deconVUpper = GameObject.Find("DeconDoorVUpper");
                deconVUpper.SetActive(false);
                var deconHLower = GameObject.Find("DeconDoorHLower");
                deconHLower.SetActive(false);
                var deconHUpper = GameObject.Find("DeconDoorHUpper");
                deconHUpper.SetActive(false);
                var camsone = GameObject.Find("Submerged(Clone)/Cameras/LowerDeck/Electrical/FixConsole");
                camsone.GetComponent<PolygonCollider2D>().enabled = false;
                var camstwo = GameObject.Find("Submerged(Clone)/Cameras/LowerDeck/Lobby/FixConsole");
                camstwo.GetComponent<BoxCollider2D>().enabled = false;
                camstwo.GetComponent<CircleCollider2D>().enabled = false;
                var camsthree = GameObject.Find("Submerged(Clone)/Cameras/UpperDeck/Comms/FixConsole");
                camsthree.GetComponent<PolygonCollider2D>().enabled = false;
                var camsfour = GameObject.Find("Submerged(Clone)/Cameras/UpperDeck/Lobby/FixConsole");
                camsfour.GetComponent<BoxCollider2D>().enabled = false;
                camsfour.GetComponent<CircleCollider2D>().enabled = false;
                var camsfive = GameObject.Find("Submerged(Clone)/Cameras/UpperDeck/WestHallway/FixConsole");
                camsfive.GetComponent<BoxCollider2D>().enabled = false;
                camsfive.GetComponent<CircleCollider2D>().enabled = false;
                var camssix = GameObject.Find("Submerged(Clone)/Cameras/UpperDeck/YHallway/FixConsole");
                camssix.GetComponent<BoxCollider2D>().enabled = false;
                camssix.GetComponent<CircleCollider2D>().enabled = false;
                var camsseven = GameObject.Find("Submerged(Clone)/Cameras/LowerDeck/WestHallway/FixConsole");
                camsseven.GetComponent<BoxCollider2D>().enabled = false;
                break;
        }
    }

    public static void ShowGamemodesPopUp(int flag, PlayerControl player)
    {
        var popup = GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab;

        var newPopUp = Object.Instantiate(popup, HudManager.Instance.transform.parent);
        if (flag != 0) newPopUp.gameObject.transform.GetChild(0).GetComponent<TextTranslatorTMP>().enabled = false;
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.CaptureTheFlag:
                // CTF:
                switch (flag)
                {
                    case 0: // kill flag player
                        if (player == CaptureTheFlag.RedPlayerWhoHasBlueFlag)
                            newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                        else if (player == CaptureTheFlag.BluePlayerWhoHasRedFlag)
                            newPopUp.gameObject.transform.position += new Vector3(3, -0.25f, 0);
                        break;
                    case 1: // new red player
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.JoinedRedTeam);
                        newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                        break;
                    case 2: // new blue player
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.JoinedBlueTeam);
                        newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                        break;
                    case 3: // steal blue flag
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.BlueFlagStolen);
                        newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                        break;
                    case 4: // steal red flag
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.RedFlagStolen);
                        newPopUp.gameObject.transform.position += new Vector3(3, -0.25f, 0);
                        break;
                    case 5: // score red
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.RedTeamScored);
                        newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                        break;
                    case 6: // score blue
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.BlueTeamScored);
                        newPopUp.gameObject.transform.position += new Vector3(3, -0.25f, 0);
                        break;
                }

                break;
            case CustomGameMode.PoliceAndThieves:
                // PAT:
                switch (flag)
                {
                    case 1: // captured thief
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.ThiefCaptured);
                        newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                        break;
                    case 2: // release thief
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.ThiefReleased);
                        newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                        break;
                    case 3: // deliver jewel
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.JewelDelivered);
                        newPopUp.gameObject.transform.position += new Vector3(3, -0.25f, 0);
                        break;
                }

                break;
            case CustomGameMode.HotPotato:
                // HP:
                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.NewHotPotato);
                newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                break;
            case CustomGameMode.BattleRoyale:
                // BR:
                switch (BattleRoyale.MatchType)
                {
                    case 0:
                        newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.FighterDown);
                        break;
                    case 1:
                        switch (flag)
                        {
                            case 1:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.LimeFighterDown);
                                newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                                break;
                            case 2:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.PinkFighterDown);
                                newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                                break;
                            case 3:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.SerialKillerDown);
                                newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                                break;
                        }

                        break;
                    case 2:
                        switch (flag)
                        {
                            case 1:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.PointsLimeTeam);
                                newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                                break;
                            case 2:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.PointsPinkTeam);
                                newPopUp.gameObject.transform.position += new Vector3(0, -0.25f, 0);
                                break;
                            case 3:
                                newPopUp.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = Tr.Get(TrKey.PointsSerialKiller);
                                newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                                break;
                        }

                        break;
                }

                break;
        }

        newPopUp.Show(player, 0);
    }

    private static StaticDoor GetStaticDoor(string name)
    {
        foreach (var doors in Object.FindObjectsOfType(Il2CppType.Of<StaticDoor>()))
        {
            if (doors.name != name) continue;

            return doors.CastFast<StaticDoor>();
        }

        return null;
    }
}
