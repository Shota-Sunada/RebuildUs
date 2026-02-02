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
        foreach (T item in items) UnityEngine.Object.Destroy(item);
    }

    public static void DestroyList<T>(IEnumerable<T> items) where T : UnityEngine.Object
    {
        if (items == null) return;
        foreach (T item in items) UnityEngine.Object.Destroy(item);
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
        bool allHeld = true;
        bool anyJustPressed = false;
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
        foreach (var player in PlayerControl.AllPlayerControls)
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
        foreach (var p in PlayerControl.AllPlayerControls)
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

        foreach (var t in player.myTasks)
        {
            var textTask = t.TryCast<ImportantTextTask>();
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
                InfoStringBuilder.Append(Jackal.CanCreateSidekick ? Tr.Get(TranslateKey.JackalWithSidekick) : Tr.Get(TranslateKey.JackalShortDesc));
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
            InfoStringBuilder.Append(Madmate.FullName).Append(": ").Append(Tr.Get(TranslateKey.MadmateShortDesc));
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

        int start = 0;
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
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.IsTeamCrewmate() && !p.HasModifier(ModifierType.Madmate) && !p.IsRole(RoleType.Madmate) && !p.IsRole(RoleType.Suicider) && p.IsAlive()) return true;
        }
        return false;
    }

    public static bool HasImpostorVision(PlayerControl player)
    {
        if (player.IsTeamImpostor()) return true;

        bool isFormerJackal = false;
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
        byte maxKey = byte.MaxValue;
        int maxValue = int.MinValue;

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

    public static bool SabotageActive() => MapUtilities.CachedShipStatus.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;

    public static float SabotageTimer() => MapUtilities.CachedShipStatus.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;

    public static bool CanUseSabotage()
    {
        var sabSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (MapUtilities.CachedShipStatus.Systems.TryGetValue(SystemTypes.Doors, out var systemType))
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

        float num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(killDistanceIdx == -1 ? GetOption(Int32OptionNames.KillDistance) : killDistanceIdx, 0, 2)];
        untargetablePlayers ??= [];

        var truePosition = targetingPlayer.GetTruePosition();
        PlayerControl result = null;

        foreach (var playerInfo in GameData.Instance.AllPlayers)
        {
            if (playerInfo.Disconnected || playerInfo.PlayerId == targetingPlayer.PlayerId || playerInfo.IsDead) continue;
            if (onlyCrewmates && playerInfo.Role.IsImpostor) continue;

            var obj = playerInfo.Object;
            if (obj == null || (obj.inVent && !targetPlayersInVents)) continue;

            bool untargetable = false;
            foreach (var utp in untargetablePlayers)
            {
                if (utp == obj)
                {
                    untargetable = true;
                    break;
                }
            }
            if (untargetable) continue;

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
        foreach (var target in PlayerControl.AllPlayerControls)
        {
            if (target?.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            bool isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
            bool hasVisibleShield = false;
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
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer?.Data == null) return;

        var meeting = MeetingHud.Instance;
        bool hasMeeting = meeting?.playerStates != null;
        if (hasMeeting)
        {
            _voteAreaStates.Clear();
            foreach (var s in meeting.playerStates) if (s != null) _voteAreaStates[s.TargetPlayerId] = s;
        }

        foreach (var p in PlayerControl.AllPlayerControls)
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
                string roleBase = RoleInfo.GetRolesString(p, true, false);
                string roleGhost = RoleInfo.GetRolesString(p, true, ModMapOptions.GhostsSeeModifier);

                string statusText = "";
                if (p == localPlayer || (localPlayer.Data.IsDead && ModMapOptions.GhostsSeeInformation))
                {
                    if (p.IsRole(RoleType.Arsonist))
                    {
                        var role = Arsonist.GetRole(p);
                        if (role != null)
                        {
                            int dousedSurvivors = 0;
                            foreach (var dousedPlayer in role.DousedPlayers)
                            {
                                if (dousedPlayer?.Data != null && !dousedPlayer.Data.IsDead && !dousedPlayer.Data.Disconnected)
                                {
                                    dousedSurvivors++;
                                }
                            }
                            int totalSurvivors = 0;
                            foreach (var targetPlayer in PlayerControl.AllPlayerControls)
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

                string taskText = "";
                if (total > 0)
                {
                    InfoStringBuilder.Clear();

                    bool commsActive = false;
                    if (MapUtilities.CachedShipStatus != null && MapUtilities.CachedShipStatus.Systems.TryGetValue(SystemTypes.Comms, out var comms))
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
                        string color = (completed == total) ? "#00FF00FF" : "#FAD934FF";
                        InfoStringBuilder.Append("<color=").Append(color).Append(">(").Append(completed).Append('/').Append(total).Append(")</color>");
                    }
                    taskText = InfoStringBuilder.ToString();
                }

                string pInfo = "";
                string mInfo = "";
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
                else if (ModMapOptions.GhostsSeeRoles && ModMapOptions.GhostsSeeInformation)
                {
                    InfoStringBuilder.Clear();
                    InfoStringBuilder.Append(roleGhost).Append(statusText).Append(' ').Append(taskText);
                    pInfo = InfoStringBuilder.ToString().Trim();
                    mInfo = pInfo;
                }
                else if (ModMapOptions.GhostsSeeInformation)
                {
                    InfoStringBuilder.Clear();
                    InfoStringBuilder.Append(taskText).Append(statusText);
                    pInfo = InfoStringBuilder.ToString().Trim();
                    mInfo = pInfo;
                }
                else if (ModMapOptions.GhostsSeeRoles)
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
        foreach (var p in PlayerControl.AllPlayerControls)
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
        int bc = 0;
        var t = text.ToString();
        foreach (char c in t) bc += Encoding.UTF8.GetByteCount(c.ToString()) == 1 ? 1 : 2;
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

        if (ModMapOptions.HideOutOfSightNametags && GameStarted && MapUtilities.CachedShipStatus != null)
        {
            float distMod = 1.025f;
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            bool blocked = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

            if (distance > MapUtilities.CachedShipStatus.CalculateLightRadius(source.Data) * distMod || blocked) return true;
        }

        if (!ModMapOptions.HidePlayerNames) return false;
        if (source.IsTeamImpostor() && (target.IsTeamImpostor() || target.IsRole(RoleType.Spy) || (target.IsRole(RoleType.Sidekick) && Sidekick.GetRole(target).WasTeamRed) || (target.IsRole(RoleType.Jackal) && Jackal.GetRole(target).WasTeamRed))) return false;
        if (source.GetPartner() == target) return false;
        if ((source.IsRole(RoleType.Jackal) || source.IsRole(RoleType.Sidekick)) && (target.IsRole(RoleType.Jackal) || target.IsRole(RoleType.Sidekick) || target == Jackal.GetRole(target).FakeSidekick)) return false;

        return true;
    }

    public static void OnObjectDestroy(GameObject obj)
    {
        if (obj == null) return;
        string name = obj.name;
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

            float alpha = (p < 0.5f) ? (p * 2f * 0.75f) : ((1f - p) * 2f * 0.75f);
            renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));

            if (p == 1f)
            {
                bool reactorActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
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
        for (int i = startAt; i < self.Count - 1; i++)
        {
            int index = RebuildUs.Instance.Rnd.Next(i, self.Count);
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
            int hits = room.roomArea.OverlapCollider(filter, buffer);
            for (int i = 0; i < hits; i++)
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

        if (GameHistory.FinalStatuses != null && GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out var status))
        {
            return status != FinalStatus.Alive;
        }

        return false;
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

    public static bool CanSabotage(this PlayerControl player)
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

    public static ClientData GetClient(this PlayerControl player)
    {
        if (player == null) return null;
        var allClients = AmongUsClient.Instance.allClients;
        for (int i = 0; i < allClients.Count; i++)
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
        string name = player.Data.PlayerName;
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
}