using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
    public static bool ShowButtons { get { return !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance; } }
    public static bool ShowMeetingText { get { return MeetingHud.Instance != null && (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion); } }
    public static bool GameStarted { get { return AmongUsClient.Instance?.GameState == InnerNet.InnerNetClient.GameStates.Started; } }
    public static bool RolesEnabled { get { return CustomOptionHolder.ActivateRoles.GetBool(); } }
    public static bool RefundVotes { get { return CustomOptionHolder.RefundVotesOnDeath.GetBool(); } }

    public static bool IsSkeld { get { return GetOption(ByteOptionNames.MapId) == 0; } }
    public static bool IsMiraHQ { get { return GetOption(ByteOptionNames.MapId) == 1; } }
    public static bool IsPolus { get { return GetOption(ByteOptionNames.MapId) == 2; } }
    public static bool IsAirship { get { return GetOption(ByteOptionNames.MapId) == 4; } }
    public static bool IsFungle { get { return GetOption(ByteOptionNames.MapId) == 5; } }

    public static void DestroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : UnityEngine.Object
    {
        if (items == null) return;
        foreach (T item in items)
        {
            UnityEngine.Object.Destroy(item);
        }
    }

    public static void destroyList<T>(List<T> items) where T : UnityEngine.Object
    {
        if (items == null) return;
        foreach (T item in items)
        {
            UnityEngine.Object.Destroy(item);
        }
    }

    public static void GenerateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
    {
        if (player == null) return;

        var taskTypeIds = GenerateTasks(numCommon, numShort, numLong);
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedSetTasks);
            sender.Write(player.PlayerId);
            sender.WriteBytesAndSize(taskTypeIds.ToArray());
            RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
        }
    }

    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    public static string Cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static int LineCount(string text)
    {
        return text.Count(c => c == '\n');
    }
    private static readonly StringBuilder InfoStringBuilder = new();
    public static string PreviousEndGameSummary = "";

    public static bool HasFakeTasks(this PlayerControl player)
    {
        return player.IsRole(RoleType.Jester)
            || player.IsRole(RoleType.Jackal)
            || player.IsRole(RoleType.Sidekick)
            || player.IsRole(RoleType.Arsonist)
            || player.IsRole(RoleType.Vulture)
            || Jackal.FormerJackals.Any(x => x == player)
        ;
    }

    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.PlayerId == id)
            {
                return player;
            }
        }
        return null;
    }

    public static Dictionary<byte, PlayerControl> AllPlayersById()
    {
        Dictionary<byte, PlayerControl> res = [];
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            res.Add(player.PlayerId, player);
        }
        return res;
    }

    public static void HandleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        CheckMurderAttemptAndKill(Vampire.AllPlayers.FirstOrDefault(), Vampire.Bitten, true, false);
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
            sender.Write(byte.MaxValue);
            sender.Write(byte.MaxValue);
            RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
        }
    }

    public static void RefreshRoleDescription(PlayerControl player)
    {
        if (player == null) return;

        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(player);

        var toRemove = new List<PlayerTask>();
        foreach (PlayerTask t in player.myTasks)
        {
            var textTask = t.gameObject.GetComponent<ImportantTextTask>();
            if (textTask != null)
            {
                var info = infos.FirstOrDefault(x => textTask.Text.StartsWith(x.Name));
                if (info != null)
                    infos.Remove(info); // TextTask for this RoleInfo does not have to be added, as it already exists
                else
                    toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
            }
        }

        foreach (PlayerTask t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            UnityEngine.Object.Destroy(t.gameObject);
        }

        // Add TextTask for remaining RoleInfos
        foreach (RoleInfo roleInfo in infos)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);

            if (roleInfo.RoleType == RoleType.Jackal)
            {
                if (Jackal.CanCreateSidekick)
                {
                    task.Text = Cs(roleInfo.Color, $"{roleInfo.Name}: " + Tr.Get("Option.JackalWithSidekick"));
                }
                else
                {
                    task.Text = Cs(roleInfo.Color, $"{roleInfo.Name}: " + Tr.Get("ShortDesc.Jackal"));
                }
            }
            else
            {
                task.Text = Cs(roleInfo.Color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
            }

            player.myTasks.Insert(0, task);
        }

        if (player.HasModifier(ModifierType.Madmate) || player.HasModifier(ModifierType.CreatedMadmate))
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = Cs(Madmate.NameColor, $"{Madmate.FullName}: " + Tr.Get("ShortDesc.Madmate"));
            player.myTasks.Insert(0, task);
        }
    }
    public static bool IsLighterColor(int colorId)
    {
        return CustomColors.LighterColors.Contains(colorId);
    }
    public static bool MushroomSabotageActive()
    {
        return PlayerControl.LocalPlayer.myTasks.ToArray().Any((x) => x.TaskType == TaskTypes.MushroomMixupSabotage);
    }

    public static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        var alpha = value ? 0.25f : 1f;
        foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
        }
        player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
    }

    public static List<byte> GenerateTasks(int numCommon, int numShort, int numLong)
    {
        if (numCommon + numShort + numLong <= 0)
        {
            numShort = 1;
        }

        var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
        var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

        var commonTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.CommonTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) commonTasks.Add(task);

        var shortTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.ShortTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) shortTasks.Add(task);

        var longTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.LongTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) longTasks.Add(task);

        int start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasks);

        start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasks);

        start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasks);

        return tasks.ToArray().ToList();
    }

    public static void ClearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks)
        {
            playerTask.OnRemove();
            UnityEngine.Object.Destroy(playerTask.gameObject);
        }
        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
        {
            player.Data.Tasks.Clear();
        }
    }

    public static bool IsCrewmateAlive()
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.IsTeamCrewmate() && !player.HasModifier(ModifierType.Madmate) && player.IsAlive()) return true;
        }
        return false;
    }

    public static bool HasImpostorVision(PlayerControl player)
    {
        return player.IsTeamImpostor()
        || ((player.IsRole(RoleType.Jackal) || Jackal.FormerJackals.Any(x => x.PlayerId == player.PlayerId)) && Jackal.HasImpostorVision)
        || (player.IsRole(RoleType.Sidekick) && Sidekick.HasImpostorVision)
        || (player.IsRole(RoleType.Spy) && Spy.HasImpostorVision)
        || (player.IsRole(RoleType.Jester) && Jester.HasImpostorVision);
    }

    public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        KeyValuePair<byte, int> result = new(byte.MaxValue, int.MinValue);
        foreach (KeyValuePair<byte, int> keyValuePair in self)
        {
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (keyValuePair.Value == result.Value)
            {
                tie = true;
            }
        }
        return result;
    }

    public static MurderAttemptResult CheckMurderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false)
    {
        // Modified vanilla checks
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code
        if (IsHideNSeekMode) return MurderAttemptResult.PerformKill;

        // Block impostor shielded kill
        if (Medic.Exists)
        {
            if (!ignoreMedic && Medic.Shielded != null && Medic.Shielded == target)
            {
                {
                    using var sender = new RPCSender(killer.NetId, CustomRPC.ShieldedMurderAttempt);
                    RPCProcedure.ShieldedMurderAttempt();
                }
                return MurderAttemptResult.SuppressKill;
            }
        }

        // Block impostor not fully grown mini kill
        else if (Mini.Exists && target.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(target))
        {
            return MurderAttemptResult.SuppressKill;
        }

        // Block Time Master with time shield kill
        else if (TimeMaster.Exists)
        {
            if (TimeMaster.ShieldActive && target.IsRole(RoleType.TimeMaster))
            {
                if (!blockRewind)
                {
                    // Only rewind the attempt was not called because a meeting started
                    using var sender = new RPCSender(killer.NetId, CustomRPC.TimeMasterRewindTime);
                    RPCProcedure.TimeMasterRewindTime();
                }
                return MurderAttemptResult.SuppressKill;
            }
        }

        // if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.vampire)
        // {
        //     return MurderAttemptResult.DelayVampireKill;
        // }
        // else if (TransportationToolPatches.isUsingTransportation(target))
        // {
        //     return MurderAttemptResult.SuppressKill;
        // }

        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
            sender.Write(killer.PlayerId);
            sender.Write(target.PlayerId);
            sender.Write(showAnimation ? byte.MaxValue : 0);
        }
        RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
        // The kill attempt will be shared using a custom RPC, hence combining modded and un modded versions is impossible
        var murder = CheckMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);

        Logger.LogMessage(Enum.GetName(murder));

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
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                    sender.Write(byte.MaxValue);
                    sender.Write(byte.MaxValue);
                    RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
                    MurderPlayer(killer, target, showAnimation);
                }
            })));
        }
        return murder;
    }

    public static bool SabotageActive()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.AnyActive;
    }

    public static float SabotageTimer()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.Timer;
    }
    public static bool CanUseSabotage()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        ISystemType systemType;
        IActivatable doors = null;
        if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }
        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, int killDistance = 3)
    {
        PlayerControl result = null;
        float num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(Helpers.GetOption(Int32OptionNames.KillDistance), 0, 2)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent) return result;
        if (targetingPlayer.IsGM()) return result;

        untargetablePlayers ??= [];

        // GM is untargetable by anything
        // if (GM.gm != null)
        // {
        //     untargetablePlayers.Add(GM.gm);
        // }

        var truePosition = targetingPlayer.GetTruePosition();
        foreach (var playerInfo in GameData.Instance.AllPlayers)
        {
            if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor))
            {
                var @object = playerInfo.Object;
                if (untargetablePlayers.Any(x => x == @object))
                {
                    // if that player is not targetable: skip check
                    continue;
                }

                if (@object && (!@object.inVent || targetPlayersInVents))
                {
                    Vector2 vector = @object.GetTruePosition() - truePosition;
                    float magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = @object;
                        num = magnitude;
                    }
                }
            }
        }
        return result;
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", color);
    }

    // Update functions

    public static void SetBasePlayerOutlines()
    {
        foreach (PlayerControl target in PlayerControl.AllPlayerControls)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            bool isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
            bool hasVisibleShield = false;
            if (Camouflager.CamouflageTimer <= 0f && Medic.Shielded != null && ((target == Medic.Shielded && !isMorphedMorphing) || (isMorphedMorphing && Morphing.MorphTarget == Medic.Shielded)))
            {
                hasVisibleShield = Medic.ShowShielded == 0 // Everyone
                || (Medic.ShowShielded == 1 && (PlayerControl.LocalPlayer == Medic.Shielded || PlayerControl.LocalPlayer.IsRole(RoleType.Medic))) // Shielded + Medic
                || (Medic.ShowShielded == 2 && PlayerControl.LocalPlayer.IsRole(RoleType.Medic)); // Medic only
            }

            if (hasVisibleShield)
            {
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", Medic.ShieldedColor);
            }
            else
            {
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
    }

    public static void UpdatePlayerInfo()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || localPlayer.Data == null) return;

        var meeting = MeetingHud.Instance;
        Dictionary<byte, PlayerVoteArea> playerStatesCache = null;
        if (meeting != null && meeting.playerStates != null)
        {
            playerStatesCache = [];
            foreach (var state in meeting.playerStates)
            {
                if (state != null) playerStatesCache[state.TargetPlayerId] = state;
            }
        }

        var colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
        var colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);

        InfoStringBuilder.Clear();

        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p == null || p.Data == null || p.cosmetics == null) continue;

            // Colorblind Text in Meeting
            PlayerVoteArea playerVoteArea = null;
            playerStatesCache?.TryGetValue(p.PlayerId, out playerVoteArea);

            if (playerVoteArea != null && playerVoteArea.ColorBlindName != null && playerVoteArea.ColorBlindName.gameObject != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = colorBlindTextMeetingInitialLocalPos + new Vector3(0f, 0.4f, 0f);
                playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 0.8f;
            }

            // Colorblind Text During the round
            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject != null && p.cosmetics.colorBlindText.gameObject.active)
            {
                p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -0.25f, 0f);
            }

            if (p.cosmetics.nameText != null && p.cosmetics.nameText.transform.parent != null)
            {
                p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);
            }

            if (p == localPlayer || localPlayer.Data.IsDead)
            {
                var playerInfoLabel = GetOrCreateLabel(p.cosmetics.nameText, "Info", 0.225f, 0.75f);
                if (playerInfoLabel == null) continue;

                TextMeshPro meetingInfoLabel = null;
                if (playerVoteArea != null)
                {
                    meetingInfoLabel = GetOrCreateLabel(playerVoteArea.NameText, "Info", -0.2f, 0.60f);
                    if (meetingInfoLabel != null && playerVoteArea.NameText != null)
                    {
                        var playerName = playerVoteArea.NameText;
                        playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                    }
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(p.Data);
                string roleNames = RoleInfo.GetRolesString(p, true, false);
                string roleText = RoleInfo.GetRolesString(p, true, ModMapOptions.GhostsSeeModifier);

                string taskInfo = "";
                if (tasksTotal > 0)
                {
                    InfoStringBuilder.Append("<color=#FAD934FF>(").Append(tasksCompleted).Append('/').Append(tasksTotal).Append(")</color>");
                    taskInfo = InfoStringBuilder.ToString();
                    InfoStringBuilder.Clear();
                }

                string playerInfoText = "";
                string meetingInfoText = "";
                if (p == localPlayer)
                {
                    if (p.Data.IsDead) roleNames = roleText;

                    if (p.IsRole(RoleType.Swapper))
                    {
                        InfoStringBuilder.Append(roleNames).Append(Cs(Swapper.NameColor, " (")).Append(Swapper.RemainSwaps).Append(")");
                        playerInfoText = InfoStringBuilder.ToString();
                        InfoStringBuilder.Clear();
                    }
                    else
                    {
                        playerInfoText = roleNames;
                    }

                    if (HudManager.Instance != null && HudManager.Instance.TaskPanel != null && HudManager.Instance.TaskPanel.tab != null)
                    {
                        var tabTextTransform = HudManager.Instance.TaskPanel.tab.transform.Find("TabText_TMP");
                        if (tabTextTransform != null)
                        {
                            var tabText = tabTextTransform.GetComponent<TextMeshPro>();
                            if (tabText != null)
                            {
                                InfoStringBuilder.Append("Tasks ").Append(taskInfo);
                                tabText.SetText(InfoStringBuilder.ToString());
                                InfoStringBuilder.Clear();
                            }
                        }
                    }
                    InfoStringBuilder.Append(roleNames).Append(" ").Append(taskInfo);
                    meetingInfoText = InfoStringBuilder.ToString().Trim();
                    InfoStringBuilder.Clear();
                }
                else if (ModMapOptions.GhostsSeeRoles && ModMapOptions.GhostsSeeInformation)
                {
                    InfoStringBuilder.Append(roleText).Append(" ").Append(taskInfo);
                    playerInfoText = InfoStringBuilder.ToString().Trim();
                    meetingInfoText = playerInfoText;
                    InfoStringBuilder.Clear();
                }
                else if (ModMapOptions.GhostsSeeInformation)
                {
                    playerInfoText = taskInfo.Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (ModMapOptions.GhostsSeeRoles)
                {
                    playerInfoText = roleText;
                    meetingInfoText = playerInfoText;
                }

                playerInfoLabel.text = playerInfoText;
                playerInfoLabel.gameObject.SetActive(p.Visible);
                meetingInfoLabel?.text = meeting != null && meeting.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
            }
        }
    }

    private static TextMeshPro GetOrCreateLabel(TextMeshPro source, string name, float yOffset, float fontScale)
    {
        if (source == null || source.transform == null || source.transform.parent == null) return null;

        var labelTransform = source.transform.parent.Find(name);
        TextMeshPro label = labelTransform?.GetComponent<TextMeshPro>();

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
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var playerAlive = !player.Data.IsDead;
            player.cosmetics.SetPetVisible((localDead && playerAlive) || !localDead);
        }
    }

    public static void ShareGameVersion(byte hostId)
    {
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VersionHandshake, hostId);
            sender.Write((byte)RebuildUs.Instance.Version.Major);
            sender.Write((byte)RebuildUs.Instance.Version.Minor);
            sender.Write((byte)RebuildUs.Instance.Version.Build);
            sender.WritePacked(AmongUsClient.Instance.ClientId);
            sender.Write((byte)(RebuildUs.Instance.Version.Revision < 0 ? 0xFF : RebuildUs.Instance.Version.Revision));
            sender.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        }
        RPCProcedure.VersionHandshake(RebuildUs.Instance.Version.Major, RebuildUs.Instance.Version.Minor, RebuildUs.Instance.Version.Build, RebuildUs.Instance.Version.Revision, AmongUsClient.Instance.ClientId);
    }

    public static string PadRightV2(this object text, int num)
    {
        int bc = 0;
        var t = text.ToString();
        foreach (char c in t) bc += Encoding.GetEncoding("UTF-8").GetByteCount(c.ToString()) == 1 ? 1 : 2;
        return t?.PadRight(num - (bc - t.Length));
    }

    public static string RemoveHtml(this string text) => Regex.Replace(text, "<[^>]*?>", "");

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
        if (Camouflager.CamouflageTimer > 0f) return true; // No names are visible
        if (ModMapOptions.HideOutOfSightNametags && GameStarted && MapUtilities.CachedShipStatus != null && source.transform != null && target.transform != null)
        {
            float distMod = 1.025f;
            float distance = Vector3.Distance(source.transform.position, target.transform.position);
            bool anythingBetween = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

            if (distance > MapUtilities.CachedShipStatus.CalculateLightRadius(source.Data) * distMod || anythingBetween) return true;
        }
        if (!ModMapOptions.HidePlayerNames) return false; // All names are visible
        if (source.IsTeamImpostor() && (target.IsTeamImpostor() || target.IsRole(RoleType.Spy) || (target.IsRole(RoleType.Sidekick) && Sidekick.GetRole(target).WasTeamRed) || (target.IsRole(RoleType.Jackal) && Jackal.GetRole(target).WasTeamRed))) return false; // Members of team Impostors see the names of Impostors/Spies
        if (source.GetPartner() == target) return false; // Members of team Lovers see the names of each other
        if ((source.IsRole(RoleType.Jackal) || source.IsRole(RoleType.Sidekick)) && (target.IsRole(RoleType.Jackal) || target.IsRole(RoleType.Sidekick) || target == Jackal.GetRole(target).FakeSidekick)) return false; // Members of team Jackal see the names of each other
        return true;
    }

    public static void ShowFlash(Color color, float duration = 1f)
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null || FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
        {
            var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;

            if (p < 0.5)
            {
                renderer?.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
            }
            else
            {
                renderer?.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
            }
            if (p == 1f && renderer != null)
            {
                bool reactorActive = false;
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                {
                    if (task.TaskType == TaskTypes.StopCharles)
                    {
                        reactorActive = true;
                    }
                }
                if (!reactorActive && IsAirship) renderer.color = Color.black;
                renderer.gameObject.SetActive(false);
            }
        })));
    }

    public static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> iList)
    {
        List<T> systemList = [.. iList];
        return systemList;
    }

    public static T Random<T>(this IList<T> self)
    {
        return self.Count > 0 ? self[UnityEngine.Random.Range(0, self.Count)] : default;
    }

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
            T value = self[i];
            int index = UnityEngine.Random.Range(i, self.Count);
            self[i] = self[index];
            self[index] = value;
        }
    }

    public static PlainShipRoom GetPlainShipRoom(PlayerControl p)
    {
        var buffer = new Collider2D[10];
        var filter = default(ContactFilter2D);
        filter.layerMask = Constants.PlayersOnlyMask;
        filter.useLayerMask = true;
        filter.useTriggers = false;
        var array = MapUtilities.CachedShipStatus?.AllRooms;
        if (array == null) return null;
        foreach (var plainShipRoom in array)
        {
            if (plainShipRoom.roomArea)
            {
                int hitCount = plainShipRoom.roomArea.OverlapCollider(filter, buffer);
                if (hitCount == 0) continue;
                for (int i = 0; i < hitCount; i++)
                {
                    if (buffer[i]?.gameObject == p.gameObject)
                    {
                        return plainShipRoom;
                    }
                }
            }
        }
        return null;
    }

    public static bool IsOnElecTask()
    {
        return Camera.main.gameObject.GetComponentInChildren<SwitchMinigame>() != null;
    }
    public static bool IsHideNSeekMode
    {
        get
        {
            return GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
        }
    }

    public static bool IsNormalMode
    {
        get
        {
            return GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
        }
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
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            Logger.LogError("Error loading texture from disk: " + path);
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
}