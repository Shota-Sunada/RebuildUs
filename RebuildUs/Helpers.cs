using System.Reflection;
using System.Text.RegularExpressions;
using InnerNet;

namespace RebuildUs;

public enum MurderAttemptResult
{
    PerformKill,
    SuppressKill,
    BlankKill,
    DelayVampireKill
}

public static class Helpers
{
    public static bool ShowButtons => !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance;
    public static bool ShowMeetingText => MeetingHud.Instance != null && (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion);
    public static bool GameStarted => AmongUsClient.Instance?.GameState == InnerNet.InnerNetClient.GameStates.Started;
    public static bool RolesEnabled => CustomOptionHolder.ActivateRoles.GetBool();
    public static bool RefundVotes => CustomOptionHolder.RefundVotesOnDeath.GetBool();

    public static bool IsSkeld => GetOption(ByteOptionNames.MapId) == 0;
    public static bool IsMiraHQ => GetOption(ByteOptionNames.MapId) == 1;
    public static bool IsPolus => GetOption(ByteOptionNames.MapId) == 2;
    public static bool IsAirship => GetOption(ByteOptionNames.MapId) == 4;
    public static bool IsFungle => GetOption(ByteOptionNames.MapId) == 5;

    public static void DestroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : UnityEngine.Object
    {
        if (items == null) return;
        foreach (var item in items) UnityEngine.Object.Destroy(item);
    }

    public static void DestroyList<T>(IEnumerable<T> items) where T : UnityEngine.Object
    {
        if (items == null) return;
        foreach (var item in items) UnityEngine.Object.Destroy(item);
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

    private static readonly StringBuilder ColorStringBuilder = new();

    public static string Cs(Color c, string s)
    {
        ColorStringBuilder.Clear();
        ColorStringBuilder.Append("<color=#")
            .Append(ToByte(c.r).ToString("X2"))
            .Append(ToByte(c.g).ToString("X2"))
            .Append(ToByte(c.b).ToString("X2"))
            .Append(ToByte(c.a).ToString("X2"))
            .Append('>')
            .Append(s)
            .Append("</color>");
        return ColorStringBuilder.ToString();
    }

    private static byte ToByte(float f) => (byte)(Mathf.Clamp01(f) * 255);

    public static int LineCount(string text) => text.Count(c => c == '\n');

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
            if (Input.GetKeyDown(key))
            {
                anyJustPressed = true;
            }
        }
        return allHeld && anyJustPressed;
    }

    private static readonly StringBuilder InfoStringBuilder = new();

    public static bool HasFakeTasks(this PlayerControl player)
    {
        return (player.IsNeutral() && !player.NeutralHasTasks())
            || (player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks)
            || (player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks)
            || (player.IsRole(RoleType.Madmate) && !MadmateRole.CanKnowImpostorAfterFinishTasks)
            || (player.IsRole(RoleType.Suicider) && !Suicider.CanKnowImpostorAfterFinishTasks)
            || (player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount);
    }

    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == id) return player;
        }
        return null;
    }

    private static readonly Dictionary<byte, PlayerControl> PlayerByIdCache = [];
    private static int LastCacheFrame = -1;

    public static Dictionary<byte, PlayerControl> AllPlayersById()
    {
        if (UnityEngine.Time.frameCount == LastCacheFrame) return PlayerByIdCache;

        LastCacheFrame = UnityEngine.Time.frameCount;
        PlayerByIdCache.Clear();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null) PlayerByIdCache[p.PlayerId] = p;
        }
        return PlayerByIdCache;
    }

    public static void HandleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        var killer = Vampire.AllPlayers.FirstOrDefault();
        if (killer != null && Vampire.Bitten != null)
        {
            CheckMurderAttemptAndKill(killer, Vampire.Bitten, true, false);
        }

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
            UnityEngine.Object.Destroy(t.gameObject);
        }

        foreach (var roleInfo in infos)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
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
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            InfoStringBuilder.Clear();
            InfoStringBuilder.Append(Madmate.FullName).Append(": ").Append(Tr.Get(TrKey.MadmateShortDesc));
            task.Text = Cs(Madmate.NameColor, InfoStringBuilder.ToString());
            player.myTasks.Insert(0, task);
        }
    }

    public static bool IsLighterColor(int colorId) => CustomColors.LighterColors.Contains(colorId);

    public static bool MushroomSabotageActive()
    {
        foreach (var task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.TaskType == TaskTypes.MushroomMixupSabotage) return true;
        }
        return false;
    }

    public static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        var alpha = value ? 0.25f : 1f;
        foreach (var r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = r.color;
            r.color = new Color(c.r, c.g, c.b, alpha);
        }
        var nameTxt = player.cosmetics.nameText;
        nameTxt.color = new Color(nameTxt.color.r, nameTxt.color.g, nameTxt.color.b, alpha);
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
            UnityEngine.Object.Destroy(t.gameObject);
        }
        player.myTasks.Clear();
        player.Data?.Tasks?.Clear();
    }

    public static bool IsCrewmateAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamCrewmate() && !p.HasModifier(ModifierType.Madmate) && !p.IsRole(RoleType.Madmate) && !p.IsRole(RoleType.Suicider) && p.IsAlive()) return true;
        }
        return false;
    }

    public static bool HasImpostorVision(PlayerControl player)
    {
        if (player.IsTeamImpostor()) return true;

        var isFormerJackal = false;
        foreach (var p in Jackal.FormerJackals)
        {
            if (p.PlayerId == player.PlayerId) { isFormerJackal = true; break; }
        }

        return ((player.IsRole(RoleType.Jackal) || isFormerJackal) && Jackal.HasImpostorVision)
            || (player.IsRole(RoleType.Sidekick) && Sidekick.HasImpostorVision)
            || (player.IsRole(RoleType.Spy) && Spy.HasImpostorVision)
            || (player.IsRole(RoleType.Madmate) && MadmateRole.HasImpostorVision)
            || (player.IsRole(RoleType.Suicider) && Suicider.HasImpostorVision)
            || (player.IsRole(RoleType.Jester) && Jester.HasImpostorVision);
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
            else if (pair.Value == maxValue)
            {
                tie = true;
            }
        }
        return new KeyValuePair<byte, int>(maxKey, maxValue);
    }

    public static MurderAttemptResult CheckMurderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false)
    {
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill;
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill;
        if (IsHideNSeekMode) return MurderAttemptResult.PerformKill;

        if (Medic.Exists && !ignoreMedic && Medic.Shielded != null && Medic.Shielded == target)
        {
            using (new RPCSender(killer.NetId, CustomRPC.ShieldedMurderAttempt))
            {
                RPCProcedure.ShieldedMurderAttempt();
            }
            return MurderAttemptResult.SuppressKill;
        }

        if (Mini.Exists && target.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(target)) return MurderAttemptResult.SuppressKill;

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
        {
            MurderPlayer(killer, target, showAnimation);
        }
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
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

    public static bool SabotageActive() => MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;

    public static float SabotageTimer() => MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;

    public static bool CanUseSabotage()
    {
        var sabSystem = MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (MapUtilities.Systems.TryGetValue(SystemTypes.Doors, out var systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }
        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, int killDistanceIdx = -1)
    {
        if (!MapUtilities.CachedShipStatus) return null;

        targetingPlayer ??= PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent || targetingPlayer.IsGM()) return null;

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
                    _ => false
                };
            }

            var mat = target.cosmetics.currentBodySprite.BodySprite.material;
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

    private static readonly Vector3 ColorBlindMeetingPos = new(0.3384f, 0.23334f, -0.11f);
    private static readonly Vector3 ColorBlindMeetingScale = new(0.72f, 0.8f, 0.8f);
    private static readonly Dictionary<byte, PlayerVoteArea> _voteAreaStates = [];

    public static void UpdatePlayerInfo()
    {
        if (MapSettings.GameMode is not CustomGameMode.Roles) return;

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer?.Data == null) return;

        var meeting = MeetingHud.Instance;
        var hasMeeting = meeting?.playerStates != null;
        if (hasMeeting)
        {
            _voteAreaStates.Clear();
            foreach (var s in meeting.playerStates) if (s != null) _voteAreaStates[s.TargetPlayerId] = s;
        }

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p?.Data == null || p.cosmetics == null) continue;

            PlayerVoteArea pva = null;
            if (hasMeeting) _voteAreaStates.TryGetValue(p.PlayerId, out pva);

            // Colorblind Text Handling
            if (pva?.ColorBlindName != null && pva.ColorBlindName.gameObject.active)
            {
                pva.ColorBlindName.transform.localPosition = ColorBlindMeetingPos;
                pva.ColorBlindName.transform.localScale = ColorBlindMeetingScale;
            }

            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active)
            {
                p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -0.25f, 0f);
            }

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
                    {
                        pva.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                    }
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
                                {
                                    dousedSurvivors++;
                                }
                            }
                            var totalSurvivors = 0;
                            foreach (var targetPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                            {
                                if (targetPlayer?.Data != null &&
                                    !targetPlayer.Data.IsDead &&
                                    !targetPlayer.Data.Disconnected &&
                                    !targetPlayer.IsRole(RoleType.Arsonist) &&
                                    !targetPlayer.IsGM()
                                )
                                {
                                    totalSurvivors++;
                                }
                            }
                            statusText = Cs(Arsonist.NameColor, $" ({dousedSurvivors}/{totalSurvivors})");
                        }
                    }
                    else if (p.IsRole(RoleType.Vulture))
                    {
                        var role = Vulture.GetRole(p);
                        if (role != null) statusText = Cs(Vulture.NameColor, $" ({role.EatenBodies}/{Vulture.NumberToWin})");
                    }
                }

                var taskText = "";
                if (total > 0)
                {
                    InfoStringBuilder.Clear();

                    var commsActive = false;
                    if (MapUtilities.CachedShipStatus != null && MapUtilities.Systems.TryGetValue(SystemTypes.Comms, out var comms))
                    {
                        var activatable = comms.TryCast<IActivatable>();
                        if (activatable != null) commsActive = activatable.IsActive;
                    }

                    if (commsActive)
                    {
                        InfoStringBuilder.Append("<color=#808080FF>(?/?)</color>");
                    }
                    else
                    {
                        var color = (completed == total) ? "#00FF00FF" : "#FAD934FF";
                        InfoStringBuilder.Append("<color=").Append(color).Append(">(").Append(completed).Append('/').Append(total).Append(")</color>");
                    }
                    taskText = InfoStringBuilder.ToString();
                }

                var pInfo = "";
                var mInfo = "";
                if (p == localPlayer)
                {
                    var roles = (p.Data.IsDead ? roleGhost : roleBase) + statusText;
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
                        var tabTextObj = HudManager.Instance.TaskPanel.tab.transform.Find("TabText_TMP");
                        if (tabTextObj != null)
                        {
                            var tabText = tabTextObj.GetComponent<TextMeshPro>();
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
                meetingLabel?.text = (meeting != null && meeting.state == MeetingHud.VoteStates.Results) ? "" : mInfo;
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
            label = UnityEngine.Object.Instantiate(source, source.transform.parent);
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
        {
            p.cosmetics.SetPetVisible((localDead && !p.Data.IsDead) || !localDead);
        }
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
        {
            RPCProcedure.VersionHandshake(ver.Major, ver.Minor, ver.Build, ver.Revision, AmongUsClient.Instance.ClientId, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId);
        }
    }

    public static string PadRightV2(this object text, int num)
    {
        var bc = 0;
        var t = text.ToString();
        foreach (var c in t) bc += Encoding.UTF8.GetByteCount(c.ToString()) == 1 ? 1 : 2;
        return t?.PadRight(num - (bc - t.Length));
    }

    public static string RemoveHtml(this string text) => Regex.Replace(text, "<[^>]*?>", "");

    public static bool HidePlayerName(PlayerControl target) => HidePlayerName(PlayerControl.LocalPlayer, target);

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
            {
                Exile.WrapUpPostfix(controller.initData.networkedPlayer?.Object);
            }
        }
    }

    public static void ShowFlash(Color color, float duration = 1f)
    {
        var hud = FastDestroyableSingleton<HudManager>.Instance;
        if (hud?.FullScreen == null) return;

        hud.FullScreen.gameObject.SetActive(true);
        hud.FullScreen.enabled = true;
        hud.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
        {
            var renderer = hud.FullScreen;
            if (renderer == null) return;

            var alpha = (p < 0.5f) ? (p * 2f * 0.75f) : ((1f - p) * 2f * 0.75f);
            renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));

            if (p == 1f)
            {
                var reactorActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType == TaskTypes.StopCharles) { reactorActive = true; break; }
                }
                if (!reactorActive && IsAirship) renderer.color = Color.black;
                renderer.gameObject.SetActive(false);
            }
        })));
    }

    public static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> iList) => [.. iList];

    public static T Random<T>(this IList<T> self) => self.Count > 0 ? self[RebuildUs.Instance.Rnd.Next(0, self.Count)] : default;

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
        var filter = new ContactFilter2D { layerMask = Constants.PlayersOnlyMask, useLayerMask = true, useTriggers = false };
        var rooms = MapUtilities.CachedShipStatus?.AllRooms;
        if (rooms == null) return null;

        foreach (var room in rooms)
        {
            if (room.roomArea == null) continue;
            var hits = room.roomArea.OverlapCollider(filter, buffer);
            for (var i = 0; i < hits; i++)
            {
                if (buffer[i]?.gameObject == p.gameObject) return room;
            }
        }
        return null;
    }

    public static bool IsOnElecTask() => Camera.main.gameObject.GetComponentInChildren<SwitchMinigame>() != null;

    public static bool IsHideNSeekMode => GameManager.Instance.IsHideAndSeek();
    public static bool IsNormal => GameManager.Instance.IsNormal();

    public static bool IsCountdown => GameStartManager.InstanceExists && GameStartManager.Instance.startState is GameStartManager.StartingStates.Countdown;

    public static int GetOption(Int32OptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetInt(opt);
    public static int[] GetOption(Int32ArrayOptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetIntArray(opt);
    public static float GetOption(FloatOptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetFloat(opt);
    public static float[] GetOption(FloatArrayOptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetFloatArray(opt);
    public static bool GetOption(BoolOptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetBool(opt);
    public static byte GetOption(ByteOptionNames opt) => GameOptionsManager.Instance.CurrentGameOptions.GetByte(opt);

    public static void SetOption(Int32OptionNames opt, int value) => GameOptionsManager.Instance.CurrentGameOptions.SetInt(opt, value);
    public static void SetOption(FloatOptionNames opt, float value) => GameOptionsManager.Instance.CurrentGameOptions.SetFloat(opt, value);
    public static void SetOption(BoolOptionNames opt, bool value) => GameOptionsManager.Instance.CurrentGameOptions.SetBool(opt, value);
    public static void SetOption(ByteOptionNames opt, byte value) => GameOptionsManager.Instance.CurrentGameOptions.SetByte(opt, value);

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var bytes = Il2CppSystem.IO.File.ReadAllBytes(path);
                ImageConversion.LoadImage(texture, bytes, false);
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
            _ => RoleTypes.Crewmate
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
        {
            DiscordAutoMuteManager.SetMute(did, true, true);
        }
    }

    public static void Unmute(this PlayerControl player)
    {
        if (player == null || player.Data == null || string.IsNullOrEmpty(player.FriendCode)) return;
        if (DiscordModManager.TryGetDiscordId(player.FriendCode, out var did))
        {
            DiscordAutoMuteManager.SetMute(did, false, false);
        }
    }

    public static bool IsDead(this PlayerControl player)
    {
        if (player == null) return true;
        var data = player.Data;
        if (data == null || data.IsDead || data.Disconnected) return true;

        return GameHistory.FinalStatuses != null && GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out var status) && status != FinalStatus.Alive;
    }

    public static bool IsAlive(this PlayerControl player)
    {
        return !IsDead(player);
    }

    public static bool IsNeutral(this PlayerControl player)
    {
        return player != null
            && (player.IsRole(RoleType.Jackal) ||
                player.IsRole(RoleType.Sidekick) ||
                Jackal.FormerJackals.Contains(player) ||
                player.IsRole(RoleType.Arsonist) ||
                player.IsRole(RoleType.Jester) ||
                // player.IsRole(RoleType.Opportunist) ||
                player.IsRole(RoleType.Vulture) ||
                (player.IsRole(RoleType.Shifter) && Shifter.IsNeutral));
    }

    public static bool IsTeamCrewmate(this PlayerControl player)
    {
        return player != null && !player.IsTeamImpostor() && !player.IsNeutral() && !player.IsGM();
    }

    public static bool IsTeamImpostor(this PlayerControl player)
    {
        return player?.Data?.Role != null && player.Data.Role.IsImpostor;
    }

    public static bool NeutralHasTasks(this PlayerControl player)
    {
        return player.IsNeutral() && player.IsRole(RoleType.Shifter);
    }

    public static bool IsGM(this PlayerControl player)
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
                break;
            case CustomGameMode.CaptureTheFlag:
                if (PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag
                 && (PlayerControl.LocalPlayer == CaptureTheFlag.redplayer01 && !CaptureTheFlag.redplayer01IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer02 && !CaptureTheFlag.redplayer02IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer03 && !CaptureTheFlag.redplayer03IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer04 && !CaptureTheFlag.redplayer04IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer05 && !CaptureTheFlag.redplayer05IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer06 && !CaptureTheFlag.redplayer06IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.redplayer07 && !CaptureTheFlag.redplayer07IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer01 && !CaptureTheFlag.blueplayer01IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer02 && !CaptureTheFlag.blueplayer02IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer03 && !CaptureTheFlag.blueplayer03IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer04 && !CaptureTheFlag.blueplayer04IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer05 && !CaptureTheFlag.blueplayer05IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer06 && !CaptureTheFlag.blueplayer06IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.blueplayer07 && !CaptureTheFlag.blueplayer07IsReviving
                 || PlayerControl.LocalPlayer == CaptureTheFlag.stealerPlayer && !CaptureTheFlag.stealerPlayerIsReviving))
                {
                    roleCouldUse = true;
                }
                else
                {
                    roleCouldUse = false;
                }
                break;
            case CustomGameMode.PoliceAndThieves:
                if (PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer01 && !PoliceAndThief.thiefplayer01IsStealing && !PoliceAndThief.thiefplayer01IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer02 && !PoliceAndThief.thiefplayer02IsStealing && !PoliceAndThief.thiefplayer02IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer03 && !PoliceAndThief.thiefplayer03IsStealing && !PoliceAndThief.thiefplayer03IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer04 && !PoliceAndThief.thiefplayer04IsStealing && !PoliceAndThief.thiefplayer04IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer05 && !PoliceAndThief.thiefplayer05IsStealing && !PoliceAndThief.thiefplayer05IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer06 && !PoliceAndThief.thiefplayer06IsStealing && !PoliceAndThief.thiefplayer06IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer07 && !PoliceAndThief.thiefplayer07IsStealing && !PoliceAndThief.thiefplayer07IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer08 && !PoliceAndThief.thiefplayer08IsStealing && !PoliceAndThief.thiefplayer08IsReviving
                 || PlayerControl.LocalPlayer == PoliceAndThief.thiefplayer09 && !PoliceAndThief.thiefplayer09IsStealing && !PoliceAndThief.thiefplayer09IsReviving)
                {
                    roleCouldUse = true;
                }
                else
                {
                    roleCouldUse = false;
                }
                break;
            case CustomGameMode.HotPotato:
                // HP:
                if (PlayerControl.LocalPlayer == HotPotato.hotPotatoPlayer)
                {
                    roleCouldUse = true;
                }
                else
                {
                    roleCouldUse = false;
                }
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

    private static readonly StringBuilder RoleStringBuilder = new();
    public static string GetRoleName(this PlayerControl player) => RoleInfo.GetRolesString(player, false, joinSeparator: " + ");

    public static string GetNameWithRole(this PlayerControl player)
    {
        if (player == null || player.Data == null) return "";
        RoleStringBuilder.Clear();
        var name = player.Data.PlayerName;
        if (Camouflager.CamouflageTimer > 0f)
        {
            if (string.IsNullOrEmpty(name))
                name = player.Data.DefaultOutfit.PlayerName;
        }
        else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
        {
            name = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
        }
        RoleStringBuilder.Append(name)
                         .Append(" (")
                         .Append(player.GetRoleName())
                         .Append(')');
        return RoleStringBuilder.ToString();
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
            {
                return role.IntroSound;
            }
        }

        return null;
    }

    internal static void alphaPlayer(bool invisible, byte playerId)
    {
        var player = PlayerById(playerId);

        if (invisible)
        {
            player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, 0.5f);
            player.cosmetics.colorBlindText.color = new Color(player.cosmetics.colorBlindText.color.r, player.cosmetics.colorBlindText.color.g, player.cosmetics.colorBlindText.color.b, 0.5f);
            if (!player.cosmetics.currentPet.data.IsEmpty)
            {
                if (player.cosmetics.currentPet.renderers[0] != null && player.cosmetics.currentPet.shadows[0] != null)
                {
                    player.cosmetics.currentPet.renderers[0].color = new Color(player.cosmetics.currentPet.renderers[0].color.r, player.cosmetics.currentPet.renderers[0].color.g, player.cosmetics.currentPet.renderers[0].color.b, 0.5f);
                    player.cosmetics.currentPet.shadows[0].color = new Color(player.cosmetics.currentPet.shadows[0].color.r, player.cosmetics.currentPet.shadows[0].color.g, player.cosmetics.currentPet.shadows[0].color.b, 0.5f);
                }
            }
            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.Parent.color = new Color(player.cosmetics.hat.Parent.color.r, player.cosmetics.hat.Parent.color.g, player.cosmetics.hat.Parent.color.b, 0.5f);
                player.cosmetics.hat.BackLayer.color = new Color(player.cosmetics.hat.BackLayer.color.r, player.cosmetics.hat.BackLayer.color.g, player.cosmetics.hat.BackLayer.color.b, 0.5f);
                player.cosmetics.hat.FrontLayer.color = new Color(player.cosmetics.hat.FrontLayer.color.r, player.cosmetics.hat.FrontLayer.color.g, player.cosmetics.hat.FrontLayer.color.b, 0.5f);
            }
            if (player.cosmetics.visor != null)
            {
                player.cosmetics.visor.Image.color = new Color(player.cosmetics.visor.Image.color.r, player.cosmetics.visor.Image.color.g, player.cosmetics.visor.Image.color.b, 0.5f);
            }
            player.MyPhysics.myPlayer.cosmetics.skin.layer.color = new Color(player.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 0.5f);
        }
        else
        {
            player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, 1f);
            player.cosmetics.colorBlindText.color = new Color(player.cosmetics.colorBlindText.color.r, player.cosmetics.colorBlindText.color.g, player.cosmetics.colorBlindText.color.b, 1);
            if (!player.cosmetics.currentPet.data.IsEmpty)
            {
                if (player.cosmetics.currentPet.renderers[0] != null && player.cosmetics.currentPet.shadows[0] != null)
                {
                    player.cosmetics.currentPet.renderers[0].color = new Color(player.cosmetics.currentPet.renderers[0].color.r, player.cosmetics.currentPet.renderers[0].color.g, player.cosmetics.currentPet.renderers[0].color.b, 1f);
                    player.cosmetics.currentPet.shadows[0].color = new Color(player.cosmetics.currentPet.shadows[0].color.r, player.cosmetics.currentPet.shadows[0].color.g, player.cosmetics.currentPet.shadows[0].color.b, 1f);
                }
            }
            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.Parent.color = new Color(player.cosmetics.hat.Parent.color.r, player.cosmetics.hat.Parent.color.g, player.cosmetics.hat.Parent.color.b, 1f);
                player.cosmetics.hat.BackLayer.color = new Color(player.cosmetics.hat.BackLayer.color.r, player.cosmetics.hat.BackLayer.color.g, player.cosmetics.hat.BackLayer.color.b, 1f);
                player.cosmetics.hat.FrontLayer.color = new Color(player.cosmetics.hat.FrontLayer.color.r, player.cosmetics.hat.FrontLayer.color.g, player.cosmetics.hat.FrontLayer.color.b, 1f);
            }
            if (player.cosmetics.visor != null)
            {
                player.cosmetics.visor.Image.color = new Color(player.cosmetics.visor.Image.color.r, player.cosmetics.visor.Image.color.g, player.cosmetics.visor.Image.color.b, 1f);
            }
            player.MyPhysics.myPlayer.cosmetics.skin.layer.color = new Color(player.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, player.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 1f);
        }
    }

    public static void activateDleksMap()
    {
        if (RebuildUs.activatedDleks == false && CustomOptionHolder.CustomSkeldMap.GetSelection() == 1 && GetOption(ByteOptionNames.MapId) == 0)
        {
            var dleksMap = GameObject.Find("SkeldShip(Clone)");
            dleksMap.transform.localScale = new Vector3(dleksMap.transform.localScale.x * -1, dleksMap.transform.localScale.y, dleksMap.transform.localScale.z);
            RebuildUs.activatedDleks = true;
        }
    }

    public static void activateSenseiMap()
    {
        var activeSensei = CustomOptionHolder.CustomSkeldMap.GetSelection() == 2;

        if (GameOptionsManager.Instance.currentGameOptions.MapId == 0 && RebuildUs.activatedSensei == false)
        {
            if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal && activeSensei)
            {
                // Spawn map + assign shadow and materials layers
                GameObject senseiMap = GameObject.Instantiate(AssetLoader.customMap, PlayerControl.LocalPlayer.transform.parent);
                senseiMap.name = "HalconUI";
                senseiMap.transform.position = new Vector3(-1.5f, -1.4f, 15.05f);
                senseiMap.transform.GetChild(0).gameObject.layer = 9; // Ship Layer for HalconColisions
                senseiMap.transform.GetChild(0).transform.GetChild(0).gameObject.layer = 11; // Object Layer for HalconShadows
                senseiMap.transform.GetChild(0).transform.GetChild(1).gameObject.layer = 9; // Ship Layer for HalconAboveItems
                Material shadowShader = null;
                var background = GameObject.Find("SkeldShip(Clone)/AdminHallway");
                {
                    var sp = background.GetComponent<SpriteRenderer>();
                    if (sp != null)
                    {
                        shadowShader = sp.material;
                    }
                }
                {
                    var sp = senseiMap.GetComponent<SpriteRenderer>();
                    if (sp != null && shadowShader != null)
                    {
                        sp.material = shadowShader;
                        senseiMap.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material = shadowShader;
                    }
                }

                // Assign colliders objects, find halconCollisions to be the main parent
                var halconCollisions = senseiMap.transform.GetChild(0).transform.gameObject;

                // Area colliders rebuilded for showing map names
                var colliderAdmin = GameObject.Find("SkeldShip(Clone)/Admin/Room");
                colliderAdmin.transform.SetParent(halconCollisions.transform);
                colliderAdmin.name = "RoomAdmin";
                foreach (var c in colliderAdmin.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderAdmin.transform.position = new Vector3(0, 0, 0);
                Vector2[] myAdminpoints = { new Vector2(10.09f, -3.65f), new Vector2(1.96f, -3.65f), new Vector2(0.28f, -6.09f), new Vector2(3.97f, -10.45f), new Vector2(7.12f, -10.43f) };
                colliderAdmin.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myAdminpoints;

                var colliderCafeteria = GameObject.Find("SkeldShip(Clone)/Cafeteria/Room");
                colliderCafeteria.transform.SetParent(halconCollisions.transform);
                colliderCafeteria.name = "RoomCafeteria";
                foreach (var c in colliderCafeteria.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderCafeteria.transform.position = new Vector3(0, 0, 0);
                Vector2[] myCafeteriapoints = { new Vector2(4f, 3.35f), new Vector2(-2f, 3.35f), new Vector2(-2f, 4f), new Vector2(-4.5f, 6f), new Vector2(-4.5f, 0.55f), new Vector2(-2.8f, 0f), new Vector2(-2.8f, -2.64f), new Vector2(4, -2.64f) };
                colliderCafeteria.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myCafeteriapoints;

                var colliderCockpit = GameObject.Find("SkeldShip(Clone)/Cockpit/Room");
                colliderCockpit.transform.SetParent(halconCollisions.transform);
                colliderCockpit.name = "RoomCookpit";
                foreach (var c in colliderCockpit.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderCockpit.transform.position = new Vector3(0, 0, 0);
                Vector2[] myCockpitpoints = { new Vector2(5f, -10f), new Vector2(5f, -13f), new Vector2(8.5f, -13f), new Vector2(8.5f, -10f) };
                colliderCockpit.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myCockpitpoints;

                var colliderWeapons = GameObject.Find("SkeldShip(Clone)/Weapons/Room");
                colliderWeapons.transform.SetParent(halconCollisions.transform);
                colliderWeapons.name = "RoomWeapons";
                foreach (var c in colliderWeapons.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderWeapons.transform.position = new Vector3(0, 0, 0);
                Vector2[] myWeaponspoints = { new Vector2(12.5f, 0.5f), new Vector2(8.5f, 1.35f), new Vector2(8.5f, -3.5f), new Vector2(12.5f, -3.5f) };
                colliderWeapons.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myWeaponspoints;

                var colliderLifeSupport = GameObject.Find("SkeldShip(Clone)/LifeSupport/Room");
                colliderLifeSupport.transform.SetParent(halconCollisions.transform);
                colliderLifeSupport.name = "RoomLifeSupport";
                foreach (var c in colliderLifeSupport.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderLifeSupport.transform.position = new Vector3(0, 0, 0);
                Vector2[] myLifeSupportpoints = { new Vector2(-6.66f, 1.8f), new Vector2(-8.56f, 0.75f), new Vector2(-9.1f, 0.5f), new Vector2(-9.1f, -0.6f), new Vector2(-6.3f, -0.6f), new Vector2(-6.3f, 1.8f) };
                colliderLifeSupport.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myLifeSupportpoints;

                var colliderShields = GameObject.Find("SkeldShip(Clone)/Shields/Room");
                colliderShields.transform.SetParent(halconCollisions.transform);
                colliderShields.name = "RoomShields";
                foreach (var c in colliderShields.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderShields.transform.position = new Vector3(0, 0, 0);
                Vector2[] myShieldspoints = { new Vector2(4.3f, 0.3f), new Vector2(4.3f, -3.1f), new Vector2(8f, -3.1f), new Vector2(8f, 0.3f) };
                colliderShields.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myShieldspoints;

                var colliderElectrical = GameObject.Find("SkeldShip(Clone)/Electrical/Room");
                colliderElectrical.transform.SetParent(halconCollisions.transform);
                colliderElectrical.name = "RoomElectrical";
                foreach (var c in colliderElectrical.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderElectrical.transform.position = new Vector3(0, 0, 0);
                Vector2[] myElectricalpoints = { new Vector2(-3.9f, -9.54f), new Vector2(-3.9f, -6.69f), new Vector2(-6.7f, -6.69f), new Vector2(-6.7f, -9.54f), new Vector2(-7.3f, -9.54f), new Vector2(-7.3f, -12.9f), new Vector2(-3.39f, -12.9f), new Vector2(-3.39f, -9.54f) };
                colliderElectrical.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myElectricalpoints;


                var colliderReactor = GameObject.Find("SkeldShip(Clone)/Reactor/Room");
                colliderReactor.transform.SetParent(halconCollisions.transform);
                colliderReactor.name = "RoomReactor";
                foreach (var c in colliderReactor.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderReactor.transform.position = new Vector3(0, 0, 0);
                Vector2[] myReactorpoints = { new Vector2(-21, 2f), new Vector2(-21.5f, 0f), new Vector2(-21f, -4.2f), new Vector2(-12.6f, -2.79f), new Vector2(-12.85f, -1.25f), new Vector2(-12.6f, -0.1f) };
                colliderReactor.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myReactorpoints;

                var colliderStorage = GameObject.Find("SkeldShip(Clone)/Storage/Room");
                colliderStorage.transform.SetParent(halconCollisions.transform);
                colliderStorage.name = "RoomStorage";
                foreach (var c in colliderStorage.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderStorage.transform.position = new Vector3(0, 0, 0);
                Vector2[] myStoragepoints = { new Vector2(-11.2f, -5.7f), new Vector2(-17.4f, -9f), new Vector2(-14.91f, -11.23f), new Vector2(-15.19f, -11.61f), new Vector2(-12.46f, -13.07f), new Vector2(-9.13f, -14.07f), new Vector2(-8.78f, -13.24f), new Vector2(-7.38f, -13.24f), new Vector2(-7.4f, -9.52f), new Vector2(-7.2f, -9.52f), new Vector2(-7.2f, -7.2f) };
                colliderStorage.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myStoragepoints;

                var colliderRightEngine = GameObject.Find("SkeldShip(Clone)/RightEngine/Room");
                colliderRightEngine.transform.SetParent(halconCollisions.transform);
                colliderRightEngine.name = "RoomRightEngine";
                foreach (var c in colliderRightEngine.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderRightEngine.transform.position = new Vector3(0, 0, 0);
                Vector2[] myRightEnginepoints = { new Vector2(-20f, -4.5f), new Vector2(-19.15f, -6.95f), new Vector2(-16.8f, -8.9f), new Vector2(-11f, -5.1f), new Vector2(-11.75f, -4.75f), new Vector2(-12.65f, -3.25f) };
                colliderRightEngine.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myRightEnginepoints;

                var colliderLeftEngine = GameObject.Find("SkeldShip(Clone)/LeftEngine/Room");
                colliderLeftEngine.transform.SetParent(halconCollisions.transform);
                colliderLeftEngine.name = "RoomLeftEngine";
                foreach (var c in colliderLeftEngine.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderLeftEngine.transform.position = new Vector3(0, 0, 0);
                Vector2[] myLeftEnginepoints = { new Vector2(-16.68f, 7.17f), new Vector2(-18.86f, 4.95f), new Vector2(-20.28f, 2.03f), new Vector2(-12.84f, 0.3f), new Vector2(-11.93f, 1.85f), new Vector2(-10.87f, 2.85f) };
                colliderLeftEngine.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myLeftEnginepoints;

                var colliderComms = GameObject.Find("SkeldShip(Clone)/Comms/Room");
                colliderComms.transform.SetParent(halconCollisions.transform);
                colliderComms.name = "RoomComms";
                foreach (var c in colliderComms.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderComms.transform.position = new Vector3(0, 0, 0);
                Vector2[] myCommspoints = { new Vector2(4.3f, 4.5f), new Vector2(4.3f, 0.7f), new Vector2(8f, 0.7f), new Vector2(8f, 4.5f) };
                colliderComms.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myCommspoints;

                var colliderSecurity = GameObject.Find("SkeldShip(Clone)/Security/Room");
                colliderSecurity.transform.SetParent(halconCollisions.transform);
                colliderSecurity.name = "RoomSecurity";
                foreach (var c in colliderSecurity.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderSecurity.transform.position = new Vector3(0, 0, 0);
                Vector2[] mySecuritypoints = { new Vector2(-7.9f, 10.3f), new Vector2(-7.9f, 8.25f), new Vector2(-3.75f, 8.25f), new Vector2(-3.75f, 10.3f) };
                colliderSecurity.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = mySecuritypoints;

                var colliderMedical = GameObject.Find("SkeldShip(Clone)/Medical/Room");
                colliderMedical.transform.SetParent(halconCollisions.transform);
                colliderMedical.name = "RoomMedical";
                foreach (var c in colliderMedical.GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
                colliderMedical.transform.position = new Vector3(0, 0, 0);
                Vector2[] myMedicalpoints = { new Vector2(-4.8f, 1.3f), new Vector2(-5.99f, 1.3f), new Vector2(-5.99f, -1.75f), new Vector2(-8.31f, -2.5f), new Vector2(-7.5f, -2.5f), new Vector2(-7.5f, -3.9f), new Vector2(-3.23f, -3.9f), new Vector2(-3.23f, -1.8f), new Vector2(-3.23f, -0.18f), new Vector2(-4.8f, -0.18f) };
                colliderMedical.transform.GetChild(0).GetComponent<PolygonCollider2D>().points = myMedicalpoints;

                // HullItems objects
                var halconHullItems = senseiMap.transform.GetChild(1).transform.gameObject; // find halconHullItems to the parent
                var skeldhatch0001 = GameObject.Find("hatch0001");
                skeldhatch0001.transform.SetParent(halconHullItems.transform);
                skeldhatch0001.transform.position = new Vector3(-10.33f, -14.025f, skeldhatch0001.transform.position.z);
                var skeldshieldborder_off = GameObject.Find("shieldborder_off");
                skeldshieldborder_off.transform.SetParent(halconHullItems.transform);
                skeldshieldborder_off.transform.position = new Vector3(10.85f, -6.2f, skeldshieldborder_off.transform.position.z);
                var skeldthruster0001lowestone = GameObject.Find("thruster0001 (1)");
                skeldthruster0001lowestone.transform.SetParent(halconHullItems.transform);
                skeldthruster0001lowestone.transform.position = new Vector3(-24.4f, -9.25f, skeldthruster0001lowestone.transform.position.z);
                var skeldthruster0001lowerone = GameObject.Find("thruster0001 (2)");
                skeldthruster0001lowerone.transform.SetParent(halconHullItems.transform);
                skeldthruster0001lowerone.transform.position = new Vector3(-25.75f, -6, skeldthruster0001lowerone.transform.position.z);
                var skeldthruster0001upperone = GameObject.Find("thruster0001");
                skeldthruster0001upperone.transform.SetParent(halconHullItems.transform);
                skeldthruster0001upperone.transform.position = new Vector3(-25.75f, 3.275f, skeldthruster0001upperone.transform.position.z);
                var skeldthruster0001higherone = GameObject.Find("thruster0001 (3)");
                skeldthruster0001higherone.transform.SetParent(halconHullItems.transform);
                skeldthruster0001higherone.transform.position = new Vector3(-24.4f, 5.9f, skeldthruster0001higherone.transform.position.z);
                var skeldthruster0001middleone = GameObject.Find("thrusterbig0001");
                skeldthruster0001middleone.transform.SetParent(halconHullItems.transform);
                skeldthruster0001middleone.transform.position = new Vector3(-28.15f, -2, skeldthruster0001middleone.transform.position.z);
                var skeldweapongun = GameObject.Find("WeaponGun");
                skeldweapongun.transform.SetParent(halconHullItems.transform);
                skeldweapongun.transform.position = new Vector3(16.5f, -1.865f, skeldweapongun.transform.position.z);
                var skeldlowershield = GameObject.Find("shield_off");
                skeldlowershield.transform.SetParent(halconHullItems.transform);
                skeldlowershield.transform.position = new Vector3(10.9f, -6.65f, skeldlowershield.transform.position.z);
                var skelduppershield = GameObject.Find("shield_off (1)");
                skelduppershield.transform.SetParent(halconHullItems.transform);
                skelduppershield.transform.position = new Vector3(10.8f, -5.85f, skelduppershield.transform.position.z);
                var skeldstarfield = GameObject.Find("starfield");
                skeldstarfield.transform.SetParent(halconHullItems.transform);
                skeldstarfield.transform.position = new Vector3(3, -4.5f, skeldstarfield.transform.position.z);

                // Admin objects
                var halconAdmin = senseiMap.transform.GetChild(2).transform.gameObject; // find halconAdmin to be the parent
                var skeldAdminVent = GameObject.Find("AdminVent");
                skeldAdminVent.transform.SetParent(halconAdmin.transform);
                skeldAdminVent.transform.position = new Vector3(4.17f, -10.5f, skeldAdminVent.transform.position.z);
                var skeldadmintable = GameObject.Find("admin_bridge");
                skeldadmintable.transform.SetParent(halconAdmin.transform);
                skeldadmintable.transform.position = new Vector3(5.01f, -6.675f, skeldadmintable.transform.position.z);
                var skeldSwipeCardConsole = GameObject.Find("SwipeCardConsole");
                skeldSwipeCardConsole.transform.SetParent(halconAdmin.transform);
                skeldSwipeCardConsole.transform.position = new Vector3(6.07f, -6.575f, skeldSwipeCardConsole.transform.position.z);
                var skeldMapRoomConsole = GameObject.Find("MapRoomConsole");
                skeldMapRoomConsole.transform.SetParent(halconAdmin.transform);
                skeldMapRoomConsole.transform.position = new Vector3(3.95f, -6.575f, skeldMapRoomConsole.transform.position.z);
                var skeldLeftScreen = GameObject.Find("LeftScreen");
                skeldLeftScreen.transform.SetParent(halconAdmin.transform);
                skeldLeftScreen.transform.position = new Vector3(3.56f, -3.85f, skeldLeftScreen.transform.position.z);
                var skeldRightScreen = GameObject.Find("RightScreen");
                skeldRightScreen.transform.SetParent(halconAdmin.transform);
                skeldRightScreen.transform.position = new Vector3(5.55f, -3.85f, skeldRightScreen.transform.position.z);
                var skeldAdminUploadDataConsole = GameObject.Find("SkeldShip(Clone)/Admin/Ground/admin_walls/UploadDataConsole");
                skeldAdminUploadDataConsole.transform.SetParent(halconAdmin.transform);
                skeldAdminUploadDataConsole.transform.position = new Vector3(8.975f, -3.86f, skeldAdminUploadDataConsole.transform.position.z);
                var skeldAdminNoOxyConsole = GameObject.Find("SkeldShip(Clone)/Admin/Ground/admin_walls/NoOxyConsole");
                skeldAdminNoOxyConsole.transform.SetParent(halconAdmin.transform);
                skeldAdminNoOxyConsole.transform.position = new Vector3(2.65f, -4f, skeldAdminNoOxyConsole.transform.position.z);
                var skeldAdminFixWiringConsole = GameObject.Find("SkeldShip(Clone)/Admin/Ground/admin_walls/FixWiringConsole");
                skeldAdminFixWiringConsole.transform.SetParent(halconAdmin.transform);
                skeldAdminFixWiringConsole.transform.position = new Vector3(6.47f, -3.87f, skeldAdminFixWiringConsole.transform.position.z);
                var skeldmapComsChairs = GameObject.Find("map_ComsChairs");
                skeldmapComsChairs.transform.SetParent(halconAdmin.transform);
                skeldmapComsChairs.transform.position = new Vector3(4.585f, -4.38f, skeldmapComsChairs.transform.position.z);
                skeldadmintable.transform.GetChild(0).gameObject.SetActive(false); // Deactivate map animation

                // Cafeteria objects
                var halconCafeteria = senseiMap.transform.GetChild(3).transform.gameObject; // find halconCafeteria to be the parent
                var skeldCafeVent = GameObject.Find("CafeVent");
                skeldCafeVent.transform.SetParent(halconCafeteria.transform);
                skeldCafeVent.transform.position = new Vector3(-4.7f, 4, skeldCafeVent.transform.position.z);
                var skeldCafeGarbageConsole = GameObject.Find("SkeldShip(Clone)/Cafeteria/Ground/GarbageConsole");
                skeldCafeGarbageConsole.transform.SetParent(halconCafeteria.transform);
                skeldCafeGarbageConsole.transform.position = new Vector3(4.69f, 4, skeldCafeGarbageConsole.transform.position.z);
                var skeldCafeFixWiringConsole = GameObject.Find("SkeldShip(Clone)/Cafeteria/Ground/FixWiringConsole");
                skeldCafeFixWiringConsole.transform.SetParent(halconCafeteria.transform);
                skeldCafeFixWiringConsole.transform.position = new Vector3(-4.15f, 2.62f, skeldCafeFixWiringConsole.transform.position.z);
                var skeldCafeDataConsole = GameObject.Find("SkeldShip(Clone)/Cafeteria/Ground/DataConsole");
                skeldCafeDataConsole.transform.SetParent(halconCafeteria.transform);
                skeldCafeDataConsole.transform.position = new Vector3(-3.75f, 6.05f, skeldCafeDataConsole.transform.position.z);
                var skeldCafeEmergencyConsole = GameObject.Find("EmergencyConsole");
                skeldCafeEmergencyConsole.transform.SetParent(halconCafeteria.transform);
                skeldCafeEmergencyConsole.transform.position = new Vector3(-0.65f, 1, skeldCafeEmergencyConsole.transform.position.z);

                // nav objects
                var halconCockpit = senseiMap.transform.GetChild(4).transform.gameObject; // find halconCockpit to be the parent
                var skeldNavVentNorth = GameObject.Find("NavVentNorth");
                skeldNavVentNorth.transform.SetParent(halconCockpit.transform);
                skeldNavVentNorth.transform.position = new Vector3(6.5f, -13.15f, skeldNavVentNorth.transform.position.z);
                var skeldNavVentSouth = GameObject.Find("NavVentSouth");
                skeldNavVentSouth.transform.SetParent(halconCockpit.transform);
                skeldNavVentSouth.transform.position = new Vector3(6.5f, -15.05f, skeldNavVentSouth.transform.position.z);
                var skeldNavDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Cockpit/DivertPowerConsole");
                skeldNavDivertPowerConsole.transform.SetParent(halconCockpit.transform);
                skeldNavDivertPowerConsole.transform.position = new Vector3(6.07f, -12.55f, skeldNavDivertPowerConsole.transform.position.z);
                var skeldNavStabilizeSteeringConsole = GameObject.Find("StabilizeSteeringConsole");
                skeldNavStabilizeSteeringConsole.transform.SetParent(halconCockpit.transform);
                skeldNavStabilizeSteeringConsole.transform.position = new Vector3(9.21f, -14.17f, skeldNavStabilizeSteeringConsole.transform.position.z);
                var skeldNavChartCourseConsole = GameObject.Find("ChartCourseConsole");
                skeldNavChartCourseConsole.transform.SetParent(halconCockpit.transform);
                skeldNavChartCourseConsole.transform.position = new Vector3(8.01f, -13.1f, skeldNavChartCourseConsole.transform.position.z);
                var skeldNavUploadDataConsole = GameObject.Find("SkeldShip(Clone)/Cockpit/Ground/UploadDataConsole");
                skeldNavUploadDataConsole.transform.SetParent(halconCockpit.transform);
                skeldNavUploadDataConsole.transform.position = new Vector3(6.59f, -12.55f, skeldNavUploadDataConsole.transform.position.z);
                var skeldNavnav_chairmid = GameObject.Find("nav_chairmid");
                skeldNavnav_chairmid.transform.SetParent(halconCockpit.transform);
                skeldNavnav_chairmid.transform.position = new Vector3(8.5f, -14.1f, skeldNavnav_chairmid.transform.position.z);
                var skeldNavnav_chairback = GameObject.Find("nav_chairback");
                skeldNavnav_chairback.transform.SetParent(halconCockpit.transform);
                skeldNavnav_chairback.transform.position = new Vector3(7.7f, -13.4f, skeldNavnav_chairback.transform.position.z);

                // Weapons objects
                var halconWeapons = senseiMap.transform.GetChild(5).transform.gameObject; // find halconWeapons to be the parent
                var skeldWeaponsVent = GameObject.Find("WeaponsVent");
                skeldWeaponsVent.transform.SetParent(halconWeapons.transform);
                skeldWeaponsVent.transform.position = new Vector3(12.25f, -2.85f, skeldWeaponsVent.transform.position.z);
                var skeldWeaponsUploadDataConsole = GameObject.Find("SkeldShip(Clone)/Weapons/Ground/UploadDataConsole");
                skeldWeaponsUploadDataConsole.transform.SetParent(halconWeapons.transform);
                skeldWeaponsUploadDataConsole.transform.position = new Vector3(11.33f, 0.3f, skeldWeaponsUploadDataConsole.transform.position.z);
                var skeldWeaponsDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Weapons/Ground/weap_wall/DivertPowerConsole");
                skeldWeaponsDivertPowerConsole.transform.SetParent(halconWeapons.transform);
                skeldWeaponsDivertPowerConsole.transform.position = new Vector3(14.24f, 0.075f, skeldWeaponsDivertPowerConsole.transform.position.z);
                var skeldWeaponsHeadAnim = GameObject.Find("bullettop-capglo0001");
                skeldWeaponsHeadAnim.transform.SetParent(halconWeapons.transform);
                skeldWeaponsHeadAnim.transform.position = new Vector3(10.14f, 0.525f, skeldWeaponsHeadAnim.transform.position.z);
                var skeldWeaponsConsole = GameObject.Find("WeaponConsole");
                skeldWeaponsConsole.transform.SetParent(halconWeapons.transform);
                skeldWeaponsConsole.transform.position = new Vector3(11.84f, -1.25f, skeldWeaponsConsole.transform.position.z);

                // LifeSupport objects
                var halconLifeSupport = senseiMap.transform.GetChild(6).transform.gameObject; // find halconLifeSupport to be the parent
                var skeldLifeSupportGarbageConsole = GameObject.Find("SkeldShip(Clone)/LifeSupport/Ground/GarbageConsole");
                skeldLifeSupportGarbageConsole.transform.SetParent(halconLifeSupport.transform);
                skeldLifeSupportGarbageConsole.transform.position = new Vector3(-10.665f, 0.37f, skeldLifeSupportGarbageConsole.transform.position.z);
                var skeldLifeSupportDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/LifeSupport/Ground/DivertPowerConsole");
                skeldLifeSupportDivertPowerConsole.transform.SetParent(halconLifeSupport.transform);
                skeldLifeSupportDivertPowerConsole.transform.position = new Vector3(-7.808f, 2.07f, skeldLifeSupportDivertPowerConsole.transform.position.z);
                var skeldLifeSupportCleanFilterConsole = GameObject.Find("SkeldShip(Clone)/LifeSupport/Ground/CleanFilterConsole");
                skeldLifeSupportCleanFilterConsole.transform.SetParent(halconLifeSupport.transform);
                skeldLifeSupportCleanFilterConsole.transform.position = new Vector3(-9.8f, 0.82f, skeldLifeSupportCleanFilterConsole.transform.position.z);
                var skeldLifeSupportLifeSuppTank = GameObject.Find("SkeldShip(Clone)/LifeSupport/Ground/LifeSuppTank");
                skeldLifeSupportLifeSuppTank.transform.SetParent(halconLifeSupport.transform);
                skeldLifeSupportLifeSuppTank.transform.position = new Vector3(-8.45f, 0.6f, skeldLifeSupportLifeSuppTank.transform.position.z);
                var skeldBigYVent = GameObject.Find("BigYVent");
                skeldBigYVent.transform.SetParent(halconLifeSupport.transform);
                skeldBigYVent.transform.position = new Vector3(-9.65f, -0.4f, skeldBigYVent.transform.position.z);

                // Shields objects
                var halconShields = senseiMap.transform.GetChild(7).transform.gameObject; // find halconShields to be the parent
                var skeldShieldsVent = GameObject.Find("ShieldsVent");
                skeldShieldsVent.transform.SetParent(halconShields.transform);
                skeldShieldsVent.transform.position = new Vector3(5.575f, -1f, skeldShieldsVent.transform.position.z);
                var skeldShieldsDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Shields/Ground/shields_floor/shields_wallside/DivertPowerConsole");
                skeldShieldsDivertPowerConsole.transform.SetParent(halconShields.transform);
                skeldShieldsDivertPowerConsole.transform.position = new Vector3(8.962f, 0.7f, skeldShieldsDivertPowerConsole.transform.position.z);
                var skeldShieldLowerLeft = GameObject.Find("ShieldLowerLeft");
                skeldShieldLowerLeft.transform.SetParent(halconShields.transform);
                skeldShieldLowerLeft.transform.position = new Vector3(5.99f, -2.98f, skeldShieldLowerLeft.transform.position.z);
                var skeldShieldsbulb = GameObject.Find("bulb");
                skeldShieldsbulb.transform.SetParent(halconShields.transform);
                skeldShieldsbulb.transform.position = new Vector3(9.55f, -1.05f, skeldShieldsbulb.transform.position.z);
                var skeldShieldsbulbone = GameObject.Find("bulb (1)");
                skeldShieldsbulbone.transform.SetParent(halconShields.transform);
                skeldShieldsbulbone.transform.position = new Vector3(9.55f, -0.7f, skeldShieldsbulbone.transform.position.z);
                var skeldShieldsbulbtwo = GameObject.Find("bulb (2)");
                skeldShieldsbulbtwo.transform.SetParent(halconShields.transform);
                skeldShieldsbulbtwo.transform.position = new Vector3(9.55f, -0.35f, skeldShieldsbulbtwo.transform.position.z);
                var skeldShieldsbulbthree = GameObject.Find("bulb (3)");
                skeldShieldsbulbthree.transform.SetParent(halconShields.transform);
                skeldShieldsbulbthree.transform.position = new Vector3(5.45f, 0.15f, skeldShieldsbulbthree.transform.position.z);
                var skeldShieldsbulbfour = GameObject.Find("bulb (4)");
                skeldShieldsbulbfour.transform.SetParent(halconShields.transform);
                skeldShieldsbulbfour.transform.position = new Vector3(5.75f, 0.3f, skeldShieldsbulbfour.transform.position.z);
                var skeldShieldsbulbfive = GameObject.Find("bulb (5)");
                skeldShieldsbulbfive.transform.SetParent(halconShields.transform);
                skeldShieldsbulbfive.transform.position = new Vector3(6.05f, 0.45f, skeldShieldsbulbfive.transform.position.z);
                var skeldShieldsbulbsix = GameObject.Find("bulb (6)");
                skeldShieldsbulbsix.transform.SetParent(halconShields.transform);
                skeldShieldsbulbsix.transform.position = new Vector3(6.35f, 0.6f, skeldShieldsbulbsix.transform.position.z);

                // Hallway objects
                var halconHallway = senseiMap.transform.GetChild(8).transform.gameObject; // find halconBigHallway to be the parent
                var skeldCrossHallwayFixWiringConsole = GameObject.Find("SkeldShip(Clone)/CrossHallway/FixWiringConsole");
                skeldCrossHallwayFixWiringConsole.transform.SetParent(halconHallway.transform);
                skeldCrossHallwayFixWiringConsole.transform.position = new Vector3(-8.9F, 4.93F, skeldCrossHallwayFixWiringConsole.transform.position.z);
                var skeldBigYHallwayFixWiringConsole = GameObject.Find("SkeldShip(Clone)/BigYHallway/FixWiringConsole");
                skeldBigYHallwayFixWiringConsole.transform.SetParent(halconHallway.transform);
                skeldBigYHallwayFixWiringConsole.transform.position = new Vector3(4.685f, -12.53f, skeldBigYHallwayFixWiringConsole.transform.position.z);
                var skeldAdminSurvCamera = GameObject.Find("SkeldShip(Clone)/AdminHallway/SurvCamera");
                skeldAdminSurvCamera.transform.SetParent(halconHallway.transform);
                skeldAdminSurvCamera.transform.position = new Vector3(5.345f, -12.45f, skeldAdminSurvCamera.transform.position.z);
                var skeldBigHallwaySurvCamera = GameObject.Find("SkeldShip(Clone)/BigYHallway/SurvCamera");
                skeldBigHallwaySurvCamera.transform.SetParent(halconHallway.transform);
                skeldBigHallwaySurvCamera.transform.position = new Vector3(9.33f, 0.8f, skeldBigHallwaySurvCamera.transform.position.z);
                var skeldNorthHallwaySurvCamera = GameObject.Find("SkeldShip(Clone)/NorthHallway/SurvCamera");
                skeldNorthHallwaySurvCamera.transform.SetParent(halconHallway.transform);
                skeldNorthHallwaySurvCamera.transform.position = new Vector3(-14.53f, -4.5f, skeldNorthHallwaySurvCamera.transform.position.z);
                var skeldCrossHallwaySurvCamera = GameObject.Find("SkeldShip(Clone)/CrossHallway/SurvCamera");
                skeldCrossHallwaySurvCamera.transform.SetParent(halconHallway.transform);
                skeldCrossHallwaySurvCamera.transform.position = new Vector3(-9.85f, 4.75f, skeldCrossHallwaySurvCamera.transform.position.z);

                // Electrical objects
                var halconElectrical = senseiMap.transform.GetChild(9).transform.gameObject; // find halconElectrical to be the parent
                var skeldElecVent = GameObject.Find("ElecVent");
                skeldElecVent.transform.SetParent(halconElectrical.transform);
                skeldElecVent.transform.position = new Vector3(-5.22f, -13.95f, skeldElecVent.transform.position.z);
                var skeldElecCalibrateConsole = GameObject.Find("CalibrateConsole");
                skeldElecCalibrateConsole.transform.SetParent(halconElectrical.transform);
                skeldElecCalibrateConsole.transform.position = new Vector3(-5.48f, -11.55f, skeldElecCalibrateConsole.transform.position.z);
                var skeldelectric_frontset = GameObject.Find("electric_frontset");
                skeldelectric_frontset.transform.SetParent(halconElectrical.transform);
                skeldelectric_frontset.transform.position = new Vector3(-7.6f, -12.75f, skeldelectric_frontset.transform.position.z);
                var skeldElecUploadDataConsole = GameObject.Find("SkeldShip(Clone)/Electrical/Ground/UploadDataConsole");
                skeldElecUploadDataConsole.transform.SetParent(halconElectrical.transform);
                skeldElecUploadDataConsole.transform.position = new Vector3(-7.75f, -8.25f, skeldElecUploadDataConsole.transform.position.z);
                var skeldElecFixWiringConsole = GameObject.Find("SkeldShip(Clone)/Electrical/Ground/FixWiringConsole");
                skeldElecFixWiringConsole.transform.SetParent(halconElectrical.transform);
                skeldElecFixWiringConsole.transform.position = new Vector3(-6.37f, -8.725f, skeldElecFixWiringConsole.transform.position.z);
                var skeldElectDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Electrical/Ground/DivertPowerConsole");
                skeldElectDivertPowerConsole.transform.SetParent(halconElectrical.transform);
                skeldElectDivertPowerConsole.transform.position = new Vector3(-8.55f, -11.25f, skeldElectDivertPowerConsole.transform.position.z);

                // Reactor objects
                var halconReactor = senseiMap.transform.GetChild(10).transform.gameObject; // find halconReactor to be the parent
                var skeldReactorVent = GameObject.Find("ReactorVent");
                skeldReactorVent.transform.SetParent(halconReactor.transform);
                skeldReactorVent.transform.position = new Vector3(-19.75f, -3.1f, skeldReactorVent.transform.position.z);
                var skeldUpperReactorVent = GameObject.Find("UpperReactorVent");
                skeldUpperReactorVent.transform.SetParent(halconReactor.transform);
                skeldUpperReactorVent.transform.position = new Vector3(-19.75f, 0f, skeldUpperReactorVent.transform.position.z);
                var skeldDivertPowerFalsePanel = GameObject.Find("DivertPowerFalsePanel");
                skeldDivertPowerFalsePanel.transform.SetParent(halconReactor.transform);
                skeldDivertPowerFalsePanel.transform.position = new Vector3(-18.6f, 1, skeldDivertPowerFalsePanel.transform.position.z);
                var skeldreactor_toppipe = GameObject.Find("reactor_toppipe");
                skeldreactor_toppipe.transform.SetParent(halconReactor.transform);
                skeldreactor_toppipe.transform.position = new Vector3(-22.08f, 0.8f, skeldreactor_toppipe.transform.position.z);
                var skeldreactor_base = GameObject.Find("reactor_base");
                skeldreactor_base.transform.SetParent(halconReactor.transform);
                skeldreactor_base.transform.position = new Vector3(-22.12f, -2.6f, skeldreactor_base.transform.position.z);
                var skeldreactor_wireTop = GameObject.Find("reactor_wireTop");
                skeldreactor_wireTop.transform.SetParent(halconReactor.transform);
                skeldreactor_wireTop.transform.position = new Vector3(-21.21f, 0.175f, 6.7f);
                var skeldreactor_wireBot = GameObject.Find("reactor_wireBot");
                skeldreactor_wireBot.transform.SetParent(halconReactor.transform);
                skeldreactor_wireBot.transform.position = new Vector3(-21.21f, -2.7f, 6.9f);
                skeldreactor_wireBot.transform.rotation = Quaternion.Euler(0f, 0f, 12.5f);

                // Storage objects
                var halconStorage = senseiMap.transform.GetChild(11).transform.gameObject; // find halconStorage to be the parent
                var skeldAirlockConsole = GameObject.Find("AirlockConsole");
                skeldAirlockConsole.transform.SetParent(halconStorage.transform);
                skeldAirlockConsole.transform.position = new Vector3(-9.725f, -12.6f, skeldAirlockConsole.transform.position.z);
                var skeldstorage_Boxes = GameObject.Find("storage_Boxes");
                skeldstorage_Boxes.transform.SetParent(halconStorage.transform);
                skeldstorage_Boxes.transform.position = new Vector3(-13.55f, -10.4f, skeldstorage_Boxes.transform.position.z);
                var skeldStorageFixWiringConsole = GameObject.Find("SkeldShip(Clone)/Storage/Ground/FixWiringConsole");
                skeldStorageFixWiringConsole.transform.SetParent(halconStorage.transform);
                skeldStorageFixWiringConsole.transform.position = new Vector3(-17.77f, -9.74f, skeldStorageFixWiringConsole.transform.position.z);

                // RightEngine objects
                var halconRightEngine = senseiMap.transform.GetChild(12).transform.gameObject; // find halconRightEngine to be the parent
                var skeldREngineVent = GameObject.Find("REngineVent");
                skeldREngineVent.transform.SetParent(halconRightEngine.transform);
                skeldREngineVent.transform.position = new Vector3(-18.9f, -8.7f, skeldREngineVent.transform.position.z);
                var skeldRchain01 = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/chain01");
                skeldRchain01.transform.SetParent(halconRightEngine.transform);
                skeldRchain01.transform.position = new Vector3(-17.75f, -3.65f, skeldRchain01.transform.position.z);
                var skeldRchain02 = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/chain02");
                skeldRchain02.transform.SetParent(halconRightEngine.transform);
                skeldRchain02.transform.position = new Vector3(-18.025f, -3.7f, skeldRchain02.transform.position.z);
                var skeldRchain011 = GameObject.Find("chain01 (1)");
                skeldRchain011.transform.SetParent(halconRightEngine.transform);
                skeldRchain011.transform.position = new Vector3(-18.765f, -3.85f, skeldRchain011.transform.position.z);
                var skeldREngineDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/DivertPowerConsole");
                skeldREngineDivertPowerConsole.transform.SetParent(halconRightEngine.transform);
                skeldREngineDivertPowerConsole.transform.position = new Vector3(-16.875f, -3.7f, skeldREngineDivertPowerConsole.transform.position.z);
                var skeldREngineFuelEngineConsole = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/FuelEngineConsole");
                skeldREngineFuelEngineConsole.transform.SetParent(halconRightEngine.transform);
                skeldREngineFuelEngineConsole.transform.position = new Vector3(-19.65f, -7.12f, skeldREngineFuelEngineConsole.transform.position.z);
                var skeldREngineAlignEngineConsole = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/AlignEngineConsole");
                skeldREngineAlignEngineConsole.transform.SetParent(halconRightEngine.transform);
                skeldREngineAlignEngineConsole.transform.position = new Vector3(-20.475f, -7.12f, skeldREngineAlignEngineConsole.transform.position.z);
                var skeldREngineElectric = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/Electric");
                skeldREngineElectric.transform.SetParent(halconRightEngine.transform);
                skeldREngineElectric.transform.position = new Vector3(-19.2f, -5.475f, skeldREngineElectric.transform.position.z);
                var skeldREngineSteam = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/Steam");
                skeldREngineSteam.transform.SetParent(halconRightEngine.transform);
                skeldREngineSteam.transform.position = new Vector3(-17.6f, -4.4f, skeldREngineSteam.transform.position.z);
                var skeldREngineSteam1 = GameObject.Find("SkeldShip(Clone)/RightEngine/Ground/engineRight/Steam (1)");
                skeldREngineSteam1.transform.SetParent(halconRightEngine.transform);
                skeldREngineSteam1.transform.position = new Vector3(-17.6f, -7.4f, skeldREngineSteam1.transform.position.z);
                var skeldengineRight = GameObject.Find("engineRight");
                skeldengineRight.transform.SetParent(halconRightEngine.transform);
                skeldengineRight.transform.position = new Vector3(-19.02f, -5.982f, skeldengineRight.transform.position.z);

                // LeftEngine objects
                var halconLeftEngine = senseiMap.transform.GetChild(13).transform.gameObject; // find halconLeftEngine to be the parent
                var skeldLEngineVent = GameObject.Find("LEngineVent");
                skeldLEngineVent.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineVent.transform.position = new Vector3(-18.92f, 5.8f, skeldLEngineVent.transform.position.z);
                var skeldLchain01 = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/chain01");
                skeldLchain01.transform.SetParent(halconLeftEngine.transform);
                skeldLchain01.transform.position = new Vector3(-17.1f, 6.1f, skeldLchain01.transform.position.z);
                var skeldLchain02 = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/chain02");
                skeldLchain02.transform.SetParent(halconLeftEngine.transform);
                skeldLchain02.transform.position = new Vector3(-16.9f, 5.95f, skeldLchain02.transform.position.z);
                var skeldLEngineDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/DivertPowerConsole");
                skeldLEngineDivertPowerConsole.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineDivertPowerConsole.transform.position = new Vector3(-18.92f, 6.95f, skeldLEngineDivertPowerConsole.transform.position.z);
                var skeldLEngineFuelEngineConsole = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/engineLeft/FuelEngineConsole");
                skeldLEngineFuelEngineConsole.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineFuelEngineConsole.transform.position = new Vector3(-19.65f, 2.48f, skeldLEngineFuelEngineConsole.transform.position.z);
                var skeldLEngineAlignEngineConsole = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/engineLeft/AlignEngineConsole");
                skeldLEngineAlignEngineConsole.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineAlignEngineConsole.transform.position = new Vector3(-20.375f, 2.56f, skeldLEngineAlignEngineConsole.transform.position.z);
                var skeldLEngineElectric = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/engineLeft/Electric");
                skeldLEngineElectric.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineElectric.transform.position = new Vector3(-19.2f, 4.15f, skeldLEngineElectric.transform.position.z);
                var skeldLEngineSteam = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/engineLeft/Steam");
                skeldLEngineSteam.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineSteam.transform.position = new Vector3(-17.6f, 5.1f, skeldLEngineSteam.transform.position.z);
                var skeldLEngineSteam1 = GameObject.Find("SkeldShip(Clone)/LeftEngine/Ground/engineLeft/Steam (1)");
                skeldLEngineSteam1.transform.SetParent(halconLeftEngine.transform);
                skeldLEngineSteam1.transform.position = new Vector3(-17.7f, 3.8f, skeldLEngineSteam1.transform.position.z);
                var skeldengineLeft = GameObject.Find("engineLeft");
                skeldengineLeft.transform.SetParent(halconLeftEngine.transform);
                skeldengineLeft.transform.position = new Vector3(-19.02f, 3.63f, skeldengineLeft.transform.position.z);

                // Comms objects
                var halconComms = senseiMap.transform.GetChild(14).transform.gameObject; // find halconComms to be the parent
                var skeldFixCommsConsole = GameObject.Find("FixCommsConsole");
                skeldFixCommsConsole.transform.SetParent(halconComms.transform);
                skeldFixCommsConsole.transform.position = new Vector3(7.555f, 3.34f, skeldFixCommsConsole.transform.position.z);
                skeldFixCommsConsole.GetComponent<SpriteRenderer>().sprite = AssetLoader.customComms.GetComponent<SpriteRenderer>().sprite;
                var skeldcommsDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Comms/Ground/comms_wallstuff/DivertPowerConsole");
                skeldcommsDivertPowerConsole.transform.SetParent(halconComms.transform);
                skeldcommsDivertPowerConsole.transform.position = new Vector3(6.95f, 5.775f, skeldcommsDivertPowerConsole.transform.position.z);
                var skeldcommsUploadDataConsole = GameObject.Find("SkeldShip(Clone)/Comms/Ground/comms_wallstuff/UploadDataConsole");
                skeldcommsUploadDataConsole.transform.SetParent(halconComms.transform);
                skeldcommsUploadDataConsole.transform.position = new Vector3(8.85f, 1.87f, skeldcommsUploadDataConsole.transform.position.z);
                var skeldtapescomms_tapes0001 = GameObject.Find("tapes-comms_tapes0001");
                skeldtapescomms_tapes0001.transform.SetParent(halconComms.transform);
                skeldtapescomms_tapes0001.transform.position = new Vector3(6.047f, 5.8f, skeldtapescomms_tapes0001.transform.position.z);

                // Security objects
                var halconSecurity = senseiMap.transform.GetChild(15).transform.gameObject; // find halconSecurity to be the parent
                var skeldSecurityVent = GameObject.Find("SecurityVent");
                skeldSecurityVent.transform.SetParent(halconSecurity.transform);
                skeldSecurityVent.transform.position = new Vector3(-8.25f, 10.7f, skeldSecurityVent.transform.position.z);
                var skeldmap_surveillance = GameObject.Find("map_surveillance");
                skeldmap_surveillance.transform.SetParent(halconSecurity.transform);
                skeldmap_surveillance.transform.position = new Vector3(-6.8f, 12.26f, skeldmap_surveillance.transform.position.z);
                var skeldServers = GameObject.Find("Servers");
                skeldServers.transform.SetParent(halconSecurity.transform);
                skeldServers.transform.position = new Vector3(-8.5f, 11.72f, skeldServers.transform.position.z);
                var skeldsecurityDivertPowerConsole = GameObject.Find("SkeldShip(Clone)/Security/Ground/DivertPowerConsole");
                skeldsecurityDivertPowerConsole.transform.SetParent(halconSecurity.transform);
                skeldsecurityDivertPowerConsole.transform.position = new Vector3(-5.3f, 12.025f, skeldsecurityDivertPowerConsole.transform.position.z);

                // Medical objects
                var halconMedical = senseiMap.transform.GetChild(16).transform.gameObject; // find halconMedical to be the parent
                var skeldMedVent = GameObject.Find("MedVent");
                skeldMedVent.transform.SetParent(halconMedical.transform);
                skeldMedVent.transform.position = new Vector3(-4.35f, -1.8f, skeldMedVent.transform.position.z);
                var skeldMedScanner = GameObject.Find("MedScanner");
                skeldMedScanner.transform.SetParent(halconMedical.transform);
                skeldMedScanner.transform.position = new Vector3(-8.4f, -2.915f, skeldMedScanner.transform.position.z);
                var skeldMedBayConsole = GameObject.Find("MedBayConsole");
                skeldMedBayConsole.transform.SetParent(halconMedical.transform);
                skeldMedBayConsole.transform.position = new Vector3(-4.315f, -0.595f, skeldMedBayConsole.transform.position.z);

                var objList = GameObject.FindObjectsOfType<Console>().ToList();
                foreach (var obj in objList)
                {
                    if (obj.name != "AlignEngineConsole")
                    {
                        obj.checkWalls = true;
                    }
                }

                // Change original skeld map parent and hide the innecesary vanilla objects (don't destroy them, the game won't work otherwise)
                var skeldship = GameObject.Find("SkeldShip(Clone)");
                Transform[] allChildren = skeldship.transform.GetComponentsInChildren<Transform>(true);
                for (var i = 1; i < allChildren.Length - 1; i++)
                {
                    allChildren[i].gameObject.SetActive(false);
                }
                skeldship.transform.SetParent(halconCollisions.transform);
                RebuildUs.activatedSensei = true;
            }
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
                var DoorLog = GameObject.Find("SurvLogConsole");
                DoorLog.GetComponent<BoxCollider2D>().enabled = false;
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
                ramp.transform.position = new Vector3(ramp.transform.position.x, ramp.transform.position.y, 0.75f);
                break;
            case 4:
                // Remove camera use, admin table, vitals, electrical doors on Airship
                var cameras = GameObject.Find("task_cams");
                cameras.GetComponent<BoxCollider2D>().enabled = false;
                var airshipadmin = GameObject.Find("panel_cockpit_map");
                airshipadmin.GetComponent<BoxCollider2D>().enabled = false;
                var airshipvitals = GameObject.Find("panel_vitals");
                airshipvitals.GetComponent<CircleCollider2D>().enabled = false;

                Helpers.GetStaticDoor("TopLeftVert").SetOpen(true);
                Helpers.GetStaticDoor("TopLeftHort").SetOpen(true);
                Helpers.GetStaticDoor("BottomHort").SetOpen(true);
                Helpers.GetStaticDoor("TopCenterHort").SetOpen(true);
                Helpers.GetStaticDoor("LeftVert").SetOpen(true);
                Helpers.GetStaticDoor("RightVert").SetOpen(true);
                Helpers.GetStaticDoor("TopRightVert").SetOpen(true);
                Helpers.GetStaticDoor("TopRightHort").SetOpen(true);
                Helpers.GetStaticDoor("BottomRightHort").SetOpen(true);
                Helpers.GetStaticDoor("BottomRightVert").SetOpen(true);
                Helpers.GetStaticDoor("LeftDoorTop").SetOpen(true);
                Helpers.GetStaticDoor("LeftDoorBottom").SetOpen(true);

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

    public static void showGamemodesPopUp(int flag, PlayerControl player)
    {
        var popup = GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab;

        var newPopUp = UnityEngine.Object.Instantiate(popup, HudManager.Instance.transform.parent);
        if (flag != 0)
        {
            newPopUp.gameObject.transform.GetChild(0).GetComponent<TextTranslatorTMP>().enabled = false;
        }
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.CaptureTheFlag:
                // CTF:
                switch (flag)
                {
                    case 0: // kill flag player
                        if (player == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                        {
                            newPopUp.gameObject.transform.position += new Vector3(-3, -0.25f, 0);
                        }
                        else if (player == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                        {
                            newPopUp.gameObject.transform.position += new Vector3(3, -0.25f, 0);
                        }
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
                switch (BattleRoyale.matchType)
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
        foreach (var doors in UnityEngine.Object.FindObjectsOfType(Il2CppType.Of<StaticDoor>()))
        {
            if (doors.name != name) continue;

            return doors.CastFast<StaticDoor>();
        }
        return null;
    }
}