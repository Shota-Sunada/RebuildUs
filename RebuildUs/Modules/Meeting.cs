using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace RebuildUs.Modules;

internal static class Meeting
{
    private static NetworkedPlayerInfo _target;

    // GMH
    private static TextMeshPro _meetingInfoText;

    private static readonly StringBuilder InfoStringBuilder = new();

    private static float Delay { get => CustomOptionHolder.DelayBeforeMeeting.GetFloat(); }

    internal static void Update(MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Animating)
        {
            return;
        }

        // Deactivate skip Button if skipping on emergency meetings is disabled
        if (MapSettings.BlockSkippingInEmergencyMeetings)
        {
            __instance.SkipVoteButton?.gameObject?.SetActive(false);
        }

        UpdateMeetingText(__instance);

        // This fixes a bug with the original game where pressing the button and a kill happens simultaneously
        // results in bodies sometimes being created *after* the meeting starts, marking them as dead and
        // removing the corpses so there's no random corpses leftover afterwards
        foreach (var deadBody in UnityObject.FindObjectsOfType<DeadBody>())
        {
            if (deadBody == null)
            {
                continue;
            }

            foreach (var pva in __instance.playerStates)
            {
                if (pva == null)
                {
                    continue;
                }

                if (pva.TargetPlayerId != deadBody?.ParentId || pva.AmDead)
                {
                    continue;
                }
                pva?.SetDead(pva.DidReport, true);
                pva?.Overlay?.gameObject?.SetActive(true);
            }
        }
    }

    private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
    {
        Dictionary<byte, int> dictionary = [];
        foreach (var playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.VotedFor is 252 or 255 or 254)
            {
                continue;
            }
            var player = Helpers.PlayerById(playerVoteArea.TargetPlayerId);
            if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
            {
                continue;
            }

            var additionalVotes = Mayor.Exists && player.IsRole(RoleType.Mayor) ? Mayor.NumVotes : 1; // Mayor vote
            dictionary[playerVoteArea.VotedFor] = dictionary.TryGetValue(playerVoteArea.VotedFor, out var currentVotes)
                ? currentVotes + additionalVotes
                : additionalVotes;
        }

        // Swapper swap votes
        SwapperModule.HandleCalculateVotes(__instance, dictionary);

        return dictionary;
    }

    internal static bool CheckForEndVoting(MeetingHud __instance)
    {
        foreach (var ps in __instance.playerStates)
        {
            if (!ps.AmDead && !ps.DidVote)
            {
                return false;
            }
        }

        if (_target == null && MapSettings.BlockSkippingInEmergencyMeetings && MapSettings.NoVoteIsSelfVote)
        {
            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.VotedFor == byte.MaxValue - 1)
                {
                    playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId;
                }
            }
        }

        var self = CalculateVotes(__instance);
        var max = self.MaxPair(out var tie);
        PlayerControl exiledPlayer = null;
        if (!tie)
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId != max.Key || p.Data.IsDead)
                {
                    continue;
                }
                exiledPlayer = p;
                break;
            }
        }

        var exiled = exiledPlayer?.Data;
        var states = new MeetingHud.VoterState[__instance.playerStates.Length];
        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            states[i] = new()
            {
                VoterId = playerVoteArea.TargetPlayerId,
                VotedForId = playerVoteArea.VotedFor,
            };
        }

        __instance.RpcVotingComplete(states, exiled, tie);

        return false;
    }

    internal static bool BloopAVoteIcon(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        var spriteRenderer = UnityObject.Instantiate(__instance.PlayerVotePrefab);
        var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes()
                            || PlayerControl.LocalPlayer.IsDead() && MapSettings.GhostsSeeVotes
                            || PlayerControl.LocalPlayer.IsRole(RoleType.Mayor) && Mayor.MayorCanSeeVoteColors && TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data).Completed >= Mayor.MayorTasksNeededToSeeVoteColors;
        if (showVoteColors)
        {
            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
        }
        else
        {
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        }

        var transform = spriteRenderer.transform;
        transform.SetParent(parent);
        transform.localScale = Vector3.zero;
        var component = parent.GetComponent<PlayerVoteArea>();
        if (component != null)
        {
            spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
        }

        __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }

    internal static bool PopulateResults(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        // Swapper swap
        var doSwap = SwapperModule.ShouldDoSwap(__instance);
        if (doSwap)
        {
            SwapperModule.PerformSwapAnimation(__instance);
        }

        __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<CppObject>(0));

        var playersById = Helpers.AllPlayersById();
        var num = 0;
        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            var targetPlayerId = playerVoteArea.TargetPlayerId;
            playerVoteArea = SwapperModule.GetRedirectedVoteArea(playerVoteArea, doSwap);

            playerVoteArea.ClearForResults();
            var num2 = 0;
            Dictionary<int, int> votesApplied = [];
            for (var j = 0; j < states.Length; j++)
            {
                var state = states[j];
                var playerById = GameData.Instance.GetPlayerById(state.VoterId);
                var voter = playersById.GetValueOrDefault(state.VoterId);
                if (voter == null)
                {
                    continue;
                }

                if (playerById == null)
                {
                    Logger.LogError("[PopulateResults] Couldn't find player info for voter: {0}", state.VoterId);
                }
                else if (i == 0 && state.SkippedVote && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                    num++;
                }
                else if (state.VotedForId == targetPlayerId && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                    num2++;
                }

                votesApplied.TryAdd(voter.PlayerId, 0);

                votesApplied[voter.PlayerId]++;

                // Major vote, redo this iteration to place a second vote
                if (voter.IsRole(RoleType.Mayor) && votesApplied[voter.PlayerId] < Mayor.NumVotes)
                {
                    j--;
                }
            }
        }

        return false;
    }

    internal static void VotingComplete(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states, NetworkedPlayerInfo exiled, bool tie)
    {
        // Reset swapper values
        SwapperModule.OnVotingComplete();
        SwapperModule.Reset();
        GuesserModule.Reset();

        _meetingInfoText?.gameObject.SetActive(false);

        foreach (var deadBody in UnityObject.FindObjectsOfType<DeadBody>())
        {
            UnityObject.Destroy(deadBody.gameObject);
        }

        // Lovers, Lawyer & Pursuer save next to be exiled, because RPC of ending game comes before RPC of exiled
        // Lovers.notAckedExiledIsLover = false;
        if (exiled != null)
        {
            // Lovers.notAckedExiledIsLover = ((Lovers.lover1 != null && Lovers.lover1.PlayerId == exiled.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == exiled.PlayerId));
        }

        // // Mini
        // if (!Mini.isGrowingUpInMeeting) Mini.timeOfGrowthStart = Mini.timeOfGrowthStart.Add(DateTime.UtcNow.Subtract(Mini.timeOfMeetingStart)).AddSeconds(10);

        // // Snitch
        // if (Snitch.snitch != null && !Snitch.needsUpdate && Snitch.snitch.Data.IsDead && Snitch.text != null)
        // {
        //     UnityEngine.Object.Destroy(Snitch.text);
        // }
    }

    internal static bool Select(ref bool __result, MeetingHud __instance, [HarmonyArgument(0)] int suspectStateIdx)
    {
        __result = false;
        // if (GM.gm != null && GM.gm.PlayerId == suspectStateIdx) return false;
        if (MapSettings.NoVoteIsSelfVote && PlayerControl.LocalPlayer.PlayerId == suspectStateIdx)
        {
            return false;
        }
        return !MapSettings.BlockSkippingInEmergencyMeetings || suspectStateIdx != -1;
    }

    internal static void PopulateButtonsPostfix(MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0)
        {
            return;
        }

        // Add Swapper Buttons
        SwapperModule.SetupButtons(__instance);

        // Add overlay for spelled players
        if (Witch.Exists && Witch.FutureSpelled != null)
        {
            foreach (var pva in __instance.playerStates)
            {
                var isSpelled = false;
                foreach (var spelled in Witch.FutureSpelled)
                {
                    if (spelled.PlayerId == pva.TargetPlayerId)
                    {
                        isSpelled = true;
                        break;
                    }
                }
                if (!isSpelled)
                {
                    continue;
                }
                var rend = new GameObject().AddComponent<SpriteRenderer>();
                rend.transform.SetParent(pva.transform);
                rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                rend.transform.localPosition = new(-0.5f, -0.03f, -1f);
                if (PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId))
                {
                    rend.transform.localPosition = new(-0.725f, -0.15f, -1f);
                }
                rend.sprite = AssetLoader.SpellButtonMeeting;
            }
        }

        // トラックボタン
        TrackerModule.SetupButtons(__instance);

        // Add Guesser Buttons
        var isGuesser = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId)
                        && PlayerControl.LocalPlayer.IsAlive()
                        && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
        if (isGuesser)
        {
            GuesserModule.CreateGuesserButton(__instance);
        }
    }

    internal static void Deserialize(MeetingHud __instance, MessageReader reader, bool initialState)
    {
        // Add swapper buttons
        if (initialState)
        {
            PopulateButtonsPostfix(__instance);
        }
    }

    internal static bool VoteAreaSelect()
    {
        return !(PlayerControl.LocalPlayer != null && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && GuesserModule.IsUIOpen());
    }

    private static void UpdateMeetingText(MeetingHud __instance)
    {
        // Uses remaining text for guesser/swapper
        if (_meetingInfoText == null)
        {
            _meetingInfoText = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
            _meetingInfoText.alignment = TextAlignmentOptions.BottomLeft;
            _meetingInfoText.transform.position = Vector3.zero;
            _meetingInfoText.transform.localPosition = new(3.0f, 3.33f, -20f);
            _meetingInfoText.transform.localScale *= 1.1f;
            _meetingInfoText.color = Palette.White;
            _meetingInfoText.gameObject.SetActive(false);
        }

        InfoStringBuilder.Clear();

        if (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion)
        {
            var lp = PlayerControl.LocalPlayer;
            if (lp != null)
            {
                var numGuesses = Guesser.RemainingShots(lp);
                if (Guesser.IsGuesser(lp.PlayerId) && lp.IsAlive() && numGuesses > 0)
                {
                    InfoStringBuilder.AppendFormat(Tr.Get(TrKey.GuesserGuessesLeft), numGuesses);
                    InfoStringBuilder.AppendLine();
                }

                if (Shifter.Exists && lp.IsRole(RoleType.Shifter) && Shifter.FutureShift != null)
                {
                    InfoStringBuilder.AppendFormat(Tr.Get(TrKey.ShifterTargetInfo), Shifter.FutureShift.Data.PlayerName);
                    InfoStringBuilder.AppendLine();
                }
            }
        }

        var text = InfoStringBuilder.ToString().Trim();
        _meetingInfoText.text = text;
        _meetingInfoText.gameObject.SetActive(text.Length > 0);
    }

    internal static void StartMeetingClear()
    {
        CustomOverlays.ShowBlackBg();
        CustomOverlays.HideInfoOverlay();
        RebuildUs.OnMeetingStart();
        Map.ShareRealTasks();
    }

    internal static bool StartMeetingPrefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
    {
        if (GameModeManager.CurrentGameMode != CustomGamemode.Normal && !GameModeManager.CurrentGameModeInstance.CanCallMeeting)
        {
            return false;
        }

        {
            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (PlayerControl.LocalPlayer.MyPhysics.enabled
                && (PlayerControl.LocalPlayer.moveable
                    || PlayerControl.LocalPlayer.inVent
                    || Hacker.HackerVitalsButton.IsEffectActive
                    || Hacker.HackerAdminTableButton.IsEffectActive
                    || SecurityGuard.SecurityGuardCamButton.IsEffectActive))
            {
                if (!PlayerControl.LocalPlayer.inMovingPlat)
                {
                    AntiTeleport.Position = PlayerControl.LocalPlayer.transform.position;
                }
            }

            // Reset vampire bitten
            Vampire.Bitten = null;
            // Count meetings
            if (meetingTarget == null)
            {
                MapSettings.MeetingsCount++;
            }
            // Save the meeting target
            _target = meetingTarget;
            Medium.MeetingStartTime = DateTime.UtcNow;

            StartMeetingClear();
        }

        {
            var isEmergency = _target == null;
            FastDestroyableSingleton<UnityTelemetry>.Instance.WriteMeetingStarted(isEmergency);
            FastDestroyableSingleton<DebugAnalytics>.Instance.Analytics.MeetingStarted(__instance.Data, _target == null);
            MapUtilities.CachedShipStatus.StartCoroutine(CoStartMeeting(__instance, _target).WrapToIl2Cpp());
            if (!__instance.AmOwner)
            {
                return false;
            }

            if (isEmergency)
            {
                __instance.RemainingEmergencies--;
                DataManager.Player.Stats.IncrementStat(StatID.EmergenciesCalled);
            }
            else
            {
                DataManager.Player.Stats.IncrementStat(StatID.BodiesReported);
            }
        }

        return false;
    }

    private static IEnumerator CoStartMeeting(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        // 既存処理の移植
        {
            while (!MeetingHud.Instance)
            {
                yield return null;
            }

            MeetingRoomManager.Instance.RemoveSelf();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player?.ResetForMeeting();
            }

            if (MapBehaviour.Instance)
            {
                MapBehaviour.Instance.Close();
            }

            if (Minigame.Instance)
            {
                Minigame.Instance.ForceClose();
            }

            MapUtilities.CachedShipStatus.OnMeetingCalled();
            KillAnimation.SetMovement(reporter, true);
        }

        // 遅延処理追加そのままyield returnで待ちを入れるとロックしたのでHudManagerのコルーチンとして実行させる
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(CoStartMeeting2(reporter, target).WrapToIl2Cpp());
    }

    private static IEnumerator CoStartMeeting2(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        // Modで追加する遅延処理
        {
            // ボタンと同時に通報が入った場合のバグ対応、他のクライアントからキルイベントが飛んでくるのを待つ
            // 見えては行けないものが見えるので暗転させる
            MeetingHud.Instance.state = MeetingHud.VoteStates.Animating; // ゲッサーのキル用meetingUpdateが呼ばれないようにするおまじない（呼ばれるとバグる）
            var hudManager = FastDestroyableSingleton<HudManager>.Instance;
            var blackScreen = UnityObject.Instantiate(hudManager.FullScreen, hudManager.transform);
            var greyScreen = UnityObject.Instantiate(hudManager.FullScreen, hudManager.transform);
            blackScreen.color = Palette.Black;
            blackScreen.transform.position = Vector3.zero;
            blackScreen.transform.localPosition = new(0f, 0f, -910f);
            blackScreen.transform.localScale = new(10f, 10f, 1f);
            blackScreen.gameObject.SetActive(true);
            blackScreen.enabled = true;
            greyScreen.color = Palette.Black;
            greyScreen.transform.position = Vector3.zero;
            greyScreen.transform.localPosition = new(0f, 0f, -920f);
            greyScreen.transform.localScale = new(10f, 10f, 1f);
            greyScreen.gameObject.SetActive(true);
            greyScreen.enabled = true;
            TMP_Text text = null;
            var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            if (roomTracker != null)
            {
                var gameObject = UnityObject.Instantiate(roomTracker.gameObject, FastDestroyableSingleton<HudManager>.Instance.transform, true);
                UnityObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                gameObject.transform.localPosition = new(0, 0, -930f);
                gameObject.transform.localScale = Vector3.one * 5f;
                text = gameObject.GetComponent<TMP_Text>();
            }

            yield return Effects.Lerp(Delay,
                new Action<float>(p =>
                {
                    // Delayed action
                    greyScreen.color = new(1.0f, 1.0f, 1.0f, 0.5f - p / 2);
                    var message = (Delay - p * Delay).ToString("0.00");
                    if (message == "0")
                    {
                        return;
                    }
                    const string prefix = "<color=#FFFFFFFF>";
                    if (text == null)
                    {
                        return;
                    }
                    text.text = string.Format("{0}{1}</color>", prefix, message);
                    text?.color = Color.white;
                }));
            // yield return new WaitForSeconds(2f);
            if (text != null)
            {
                UnityObject.Destroy(text.gameObject);
            }
            UnityObject.Destroy(blackScreen);
            UnityObject.Destroy(greyScreen);

            // ミーティング画面の並び替えを直す
            PopulateButtons(MeetingHud.Instance, reporter.Data.PlayerId);
            PopulateButtonsPostfix(MeetingHud.Instance);

            HandleReportDeadBody(reporter, target);
        }

        // 既存処理の移植
        {
            var array = UnityObject.FindObjectsOfType<DeadBody>();
            var deadBodies = new NetworkedPlayerInfo[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                deadBodies[i] = GameData.Instance.GetPlayerById(array[i].ParentId);
            }

            foreach (var t in array)
            {
                if (t != null && t.gameObject != null)
                {
                    UnityObject.Destroy(t.gameObject);
                }
                else
                {
                    Logger.LogError("[CoStartMeeting2] Encountered a null Dead Body while destroying.");
                }
            }

            var array2 = UnityObject.FindObjectsOfType<ShapeshifterEvidence>();
            foreach (var t in array2)
            {
                if (t != null && t.gameObject != null)
                {
                    UnityObject.Destroy(t.gameObject);
                }
                else
                {
                    Logger.LogError("[CoStartMeeting2] Encountered a null Evidence while destroying.");
                }
            }

            MeetingHud.Instance.StartCoroutine(MeetingHud.Instance.CoIntro(reporter.Data, target, deadBodies));
        }
    }

    private static void PopulateButtons(MeetingHud __instance, byte reporter)
    {
        // 会議に参加しないPlayerControlを持つRoleが増えたらこのListに追加
        List<PlayerControl> playerControlsToBeIgnored = [];
        playerControlsToBeIgnored.RemoveAll(x => x == null);

        // Generate PlayerVoteAreas
        var ignoredCount = playerControlsToBeIgnored.Count;
        __instance.playerStates = new PlayerVoteArea[GameData.Instance.PlayerCount - ignoredCount];
        var playerStatesCounter = 0;

        for (var i = 0; i < GameData.Instance.AllPlayers.Count; i++)
        {
            var playerInfo = GameData.Instance.AllPlayers[i];

            var isIgnored = false;
            for (var j = 0; j < ignoredCount; j++)
            {
                if (playerControlsToBeIgnored[j].PlayerId == playerInfo.PlayerId)
                {
                    isIgnored = true;
                    break;
                }
            }

            if (isIgnored)
            {
                continue;
            }

            var playerVoteArea = __instance.playerStates[playerStatesCounter] = __instance.CreateButton(playerInfo);
            playerVoteArea.Parent = __instance;
            playerVoteArea.SetTargetPlayerId(playerInfo.PlayerId);
            playerVoteArea.SetDead(reporter == playerInfo.PlayerId,
                playerInfo.Disconnected || playerInfo.IsDead,
                playerInfo.Role.Role == RoleTypes.GuardianAngel);
            playerVoteArea.UpdateOverlay();
            playerStatesCounter++;
        }

        foreach (var playerVoteArea2 in __instance.playerStates)
        {
            ControllerManager.Instance.AddSelectableUiElement(playerVoteArea2.PlayerButton);
        }

        __instance.SortButtons();
    }

    private static void HandleReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        if (target == null)
        {
            return;
        }
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            return;
        }

        var isLocalReporter = reporter.PlayerId == localPlayer.PlayerId;
        var isMedicReport = Medic.Exists && localPlayer.IsRole(RoleType.Medic) && isLocalReporter;
        var isDetectiveReport = Detective.Exists && localPlayer.IsRole(RoleType.Detective) && isLocalReporter;

        if (!isMedicReport && !isDetectiveReport)
        {
            return;
        }

        var deadPlayer = GameHistory.GetDeadPlayer(target.PlayerId);
        if (deadPlayer == null || deadPlayer.KillerIfExisting == null)
        {
            return;
        }
        var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;

        StringBuilder sb = new();
        if (isMedicReport)
        {
            sb.AppendFormat(Tr.Get(TrKey.MedicReport), (int)Math.Round(timeSinceDeath / 1000));
        }
        else
        {
            if (timeSinceDeath < Detective.ReportNameDuration * 1000)
            {
                sb.AppendFormat(Tr.Get(TrKey.DetectiveReportName), deadPlayer.KillerIfExisting.Data.PlayerName);
            }
            else if (timeSinceDeath < Detective.ReportColorDuration * 1000)
            {
                var typeOfColor = Helpers.IsLighterColor(deadPlayer.KillerIfExisting.Data.DefaultOutfit.ColorId)
                    ? Tr.Get(TrKey.DetectiveColorLight)
                    : Tr.Get(TrKey.DetectiveColorDark);
                sb.AppendFormat(Tr.Get(TrKey.DetectiveReportColor), typeOfColor);
            }
            else
            {
                sb.Append(Tr.Get(TrKey.DetectiveReportNone));
            }
        }

        var msg = sb.ToString();
        if (string.IsNullOrWhiteSpace(msg))
        {
            return;
        }
        if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(CoAddDelayedChat(reporter, msg).WrapToIl2Cpp());
        }

        if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
        {
            FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
        }
    }

    private static IEnumerator CoAddDelayedChat(PlayerControl source, string msg)
    {
        // 会議画面が準備できるまで待機
        var timer = 0f;
        while ((!MeetingHud.Instance || MeetingHud.Instance.playerStates == null) && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // さらに少し待って、チャットがDidVoteエラーを吐かないようにする
        yield return new WaitForSeconds(1.0f);

        if (MeetingHud.Instance == null || MeetingHud.Instance.playerStates == null)
        {
            yield break;
        }
        // 安全確認: 会議ボタンの一覧にプレイヤーが存在するかチェック
        var found = false;
        foreach (var state in MeetingHud.Instance.playerStates)
        {
            if (state == null || state.TargetPlayerId != source.PlayerId)
            {
                continue;
            }
            found = true;
            break;
        }

        if (found)
        {
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(source, msg);
        }
        else
        {
            Logger.LogWarn("[CoAddDelayedChat] Player {0} not found in MeetingHud. Message suppressed to prevent crash.", source.Data.PlayerName);
        }
    }
}