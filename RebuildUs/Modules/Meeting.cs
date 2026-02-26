using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace RebuildUs.Modules;

internal static class Meeting
{
    private static bool[] _selections;
    private static SpriteRenderer[] _renderers;
    private static NetworkedPlayerInfo _target;
    private static TextMeshPro _meetingExtraButtonText;
    private static PassiveButton[] _swapperButtonList;
    private static TextMeshPro _meetingExtraButtonLabel;
    private static PlayerVoteArea _swapped1;
    private static PlayerVoteArea _swapped2;

    // GMH
    private static TextMeshPro _meetingInfoText;

    private static GameObject _guesserUI;
    private static PassiveButton _guesserUIExitButton;

    private static readonly StringBuilder InfoStringBuilder = new();

    private static float Delay { get => CustomOptionHolder.DelayBeforeMeeting.GetFloat(); }

    internal static void Update(MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Animating) return;

        // Deactivate skip Button if skipping on emergency meetings is disabled
        if (MapSettings.BlockSkippingInEmergencyMeetings) __instance.SkipVoteButton?.gameObject?.SetActive(false);

        UpdateMeetingText(__instance);

        // This fixes a bug with the original game where pressing the button and a kill happens simultaneously
        // results in bodies sometimes being created *after* the meeting starts, marking them as dead and
        // removing the corpses so there's no random corpses leftover afterwards
        foreach (DeadBody deadBody in UnityObject.FindObjectsOfType<DeadBody>())
        {
            if (deadBody == null) continue;

            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (pva == null) continue;

                if (pva.TargetPlayerId != deadBody?.ParentId || pva.AmDead) continue;
                pva?.SetDead(pva.DidReport, true);
                pva?.Overlay?.gameObject?.SetActive(true);
            }
        }
    }

    private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
    {
        Dictionary<byte, int> dictionary = [];
        foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.VotedFor is 252 or 255 or 254) continue;
            PlayerControl player = Helpers.PlayerById(playerVoteArea.TargetPlayerId);
            if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

            int additionalVotes = Mayor.Exists && player.IsRole(RoleType.Mayor) ? Mayor.NumVotes : 1; // Mayor vote
            dictionary[playerVoteArea.VotedFor] = dictionary.TryGetValue(playerVoteArea.VotedFor, out int currentVotes) ? currentVotes + additionalVotes : additionalVotes;
        }

        // Swapper swap votes
        if (!Swapper.Exists || Swapper.PlayerControl.IsDead()) return dictionary;
        {
            _swapped1 = null;
            _swapped2 = null;
            foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1) _swapped1 = playerVoteArea;
                if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2) _swapped2 = playerVoteArea;
            }

            if (_swapped1 == null || _swapped2 == null) return dictionary;
            dictionary.TryAdd(_swapped1.TargetPlayerId, 0);
            dictionary.TryAdd(_swapped2.TargetPlayerId, 0);
            (dictionary[_swapped2.TargetPlayerId], dictionary[_swapped1.TargetPlayerId]) = (dictionary[_swapped1.TargetPlayerId], dictionary[_swapped2.TargetPlayerId]);
        }

        return dictionary;
    }

    internal static bool CheckForEndVoting(MeetingHud __instance)
    {
        foreach (PlayerVoteArea ps in __instance.playerStates)
        {
            if (!ps.AmDead && !ps.DidVote)
                return false;
        }

        if (_target == null && MapSettings.BlockSkippingInEmergencyMeetings && MapSettings.NoVoteIsSelfVote)
        {
            foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.VotedFor == byte.MaxValue - 1)
                    playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId;
            }
        }

        Dictionary<byte, int> self = CalculateVotes(__instance);
        KeyValuePair<byte, int> max = self.MaxPair(out bool tie);
        PlayerControl exiledPlayer = null;
        if (!tie)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId != max.Key || p.Data.IsDead) continue;
                exiledPlayer = p;
                break;
            }
        }

        NetworkedPlayerInfo exiled = exiledPlayer?.Data;
        MeetingHud.VoterState[] states = new MeetingHud.VoterState[__instance.playerStates.Length];
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            PlayerVoteArea playerVoteArea = __instance.playerStates[i];
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
        SpriteRenderer spriteRenderer = UnityObject.Instantiate(__instance.PlayerVotePrefab);
        bool showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes()
                              || (PlayerControl.LocalPlayer.IsDead()
                                  && MapSettings.GhostsSeeVotes
                              )
                              || (PlayerControl.LocalPlayer.IsRole(RoleType.Mayor)
                                  && Mayor.MayorCanSeeVoteColors
                                  && TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data).Completed >= Mayor.MayorTasksNeededToSeeVoteColors
                              );
        if (showVoteColors)
            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
        else
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);

        Transform transform = spriteRenderer.transform;
        transform.SetParent(parent);
        transform.localScale = Vector3.zero;
        PlayerVoteArea component = parent.GetComponent<PlayerVoteArea>();
        if (component != null) spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);

        __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }

    internal static bool PopulateResults(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        // Swapper swap
        _swapped1 = null;
        _swapped2 = null;
        foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1) _swapped1 = playerVoteArea;
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2) _swapped2 = playerVoteArea;
        }

        bool doSwap = _swapped1 != null && _swapped2 != null && Swapper.Exists && Swapper.PlayerControl.IsAlive();
        if (doSwap)
        {
            __instance.StartCoroutine(Effects.Slide3D(_swapped1.transform, _swapped1.transform.localPosition, _swapped2.transform.localPosition, 1.5f));
            __instance.StartCoroutine(Effects.Slide3D(_swapped2.transform, _swapped2.transform.localPosition, _swapped1.transform.localPosition, 1.5f));
        }

        __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

        Dictionary<byte, PlayerControl> playersById = Helpers.AllPlayersById();
        int num = 0;
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            PlayerVoteArea playerVoteArea = __instance.playerStates[i];
            byte targetPlayerId = playerVoteArea.TargetPlayerId;
            playerVoteArea = doSwap switch
            {
                // Swapper change playerVoteArea that gets the votes
                true when playerVoteArea.TargetPlayerId == _swapped1.TargetPlayerId => _swapped2,
                true when playerVoteArea.TargetPlayerId == _swapped2.TargetPlayerId => _swapped1,
                _ => playerVoteArea,
            };

            playerVoteArea.ClearForResults();
            int num2 = 0;
            Dictionary<int, int> votesApplied = [];
            for (int j = 0; j < states.Length; j++)
            {
                MeetingHud.VoterState state = states[j];
                NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(state.VoterId);
                PlayerControl voter = playersById.GetValueOrDefault(state.VoterId);
                if (voter == null) continue;

                if (playerById == null)
                    Logger.LogError(new StringBuilder("Couldn't find player info for voter: ").Append(state.VoterId).ToString());
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
                if (voter.IsRole(RoleType.Mayor) && votesApplied[voter.PlayerId] < Mayor.NumVotes) j--;
            }
        }

        return false;
    }

    internal static void VotingComplete(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states, NetworkedPlayerInfo exiled, bool tie)
    {
        // Reset swapper values
        Swapper.PlayerId1 = byte.MaxValue;
        Swapper.PlayerId2 = byte.MaxValue;

        _meetingInfoText?.gameObject.SetActive(false);

        foreach (DeadBody deadBody in UnityObject.FindObjectsOfType<DeadBody>()) UnityObject.Destroy(deadBody.gameObject);

        // // Lovers, Lawyer & Pursuer save next to be exiled, because RPC of ending game comes before RPC of exiled
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
        if (MapSettings.NoVoteIsSelfVote && PlayerControl.LocalPlayer.PlayerId == suspectStateIdx) return false;
        return !MapSettings.BlockSkippingInEmergencyMeetings || suspectStateIdx != -1;
    }

    private static int GetSelectedCount()
    {
        if (_selections == null) return 0;
        int count = 0;
        foreach (bool t in _selections)
        {
            if (t)
                count++;
        }

        return count;
    }

    private static void SwapperOnClick(int i, MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || i < 0 || i >= __instance.playerStates.Length) return;
        if (_selections == null || i >= _selections.Length) return;
        if (Swapper.NumSwaps <= 0) return;
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (__instance.playerStates[i].AmDead) return;

        int selectedCount = GetSelectedCount();
        SpriteRenderer renderer = _renderers[i];

        switch (selectedCount)
        {
            case 0:
                renderer.color = Color.green;
                _selections[i] = true;
                break;
            case 1 when _selections[i]:
                renderer.color = Color.red;
                _selections[i] = false;
                break;
            case 1:
                _selections[i] = true;
                renderer.color = Color.green;
                _meetingExtraButtonLabel.text = Helpers.Cs(Color.green, Tr.Get(TrKey.SwapperConfirm));
                break;
            case 2:
                {
                    if (_selections[i])
                    {
                        renderer.color = Color.red;
                        _selections[i] = false;
                        _meetingExtraButtonLabel.text = Helpers.Cs(Color.red, Tr.Get(TrKey.SwapperConfirm));
                    }

                    break;
                }
        }
    }

    private static void SwapperConfirm(MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0) return;
        __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (GetSelectedCount() != 2) return;
        if (Swapper.NumSwaps <= 0 || Swapper.PlayerId1 != byte.MaxValue) return;

        PlayerVoteArea firstPlayer = null;
        PlayerVoteArea secondPlayer = null;
        for (int a = 0; a < _selections.Length && a < __instance.playerStates.Length; a++)
        {
            if (_selections[a])
            {
                if (firstPlayer == null)
                    firstPlayer = __instance.playerStates[a];
                else
                    secondPlayer = __instance.playerStates[a];

                if (_renderers != null && a < _renderers.Length) _renderers[a].color = Color.green;
            }
            else
                _renderers?[a]?.color = Color.gray;

            _swapperButtonList[a]?.OnClick.RemoveAllListeners(); // Swap buttons can't be clicked / changed anymore
        }

        if (firstPlayer == null || secondPlayer == null) return;
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SwapperSwap);
            sender.Write(firstPlayer.TargetPlayerId);
            sender.Write(secondPlayer.TargetPlayerId);
            RPCProcedure.SwapperSwap(firstPlayer.TargetPlayerId, secondPlayer.TargetPlayerId);
        }

        _meetingExtraButtonLabel.text = Helpers.Cs(Color.green, Tr.Get(TrKey.SwapperSwapping));
        Swapper.RemainSwaps--;
        _meetingExtraButtonText.text = Tr.Get(TrKey.SwapperSwapsLeft, Swapper.RemainSwaps);
    }

    internal static void SwapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or died in the meeting, check if this affects the swapper.
        if (__instance == null || __instance.playerStates == null || !Swapper.Exists || __instance.state == MeetingHud.VoteStates.Results) return;

        // reset swap.
        bool reset = false;
        if (dyingPlayerId == Swapper.PlayerId1 || dyingPlayerId == Swapper.PlayerId2)
        {
            reset = true;
            Swapper.PlayerId1 = Swapper.PlayerId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) || _selections == null) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (int i = 0; i < __instance.playerStates.Length && i < _selections.Length; i++)
        {
            reset = reset || (_selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId);
            if (reset) break;
        }

        if (!reset) return;

        int stateCount = __instance.playerStates.Length; // Using Length as it seems standard in this file
        for (int i = 0; i < _selections.Length; i++)
        {
            _selections[i] = false;
            if (i >= stateCount) continue;
            PlayerVoteArea playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || (Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.NiceSwapper) && Swapper.CanOnlySwapOthers)) continue;
            _renderers[i].color = Color.red;
            Swapper.RemainSwaps++;
            int copyI = i;
            _swapperButtonList[i].OnClick.RemoveAllListeners();
            _swapperButtonList[i].OnClick.AddListener((Action)(() => SwapperOnClick(copyI, __instance)));
        }

        _meetingExtraButtonText.text = Tr.Get(TrKey.SwapperSwapsLeft, Swapper.RemainSwaps);
        _meetingExtraButtonLabel.text = Helpers.Cs(Color.red, Tr.Get(TrKey.SwapperConfirm));
    }

    private static void GuesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (_guesserUI != null || !(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)) return;
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0) return;
        if (buttonTarget < 0 || buttonTarget >= __instance.playerStates.Length) return;

        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

        Transform phoneUI = UnityObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
        Transform container = UnityObject.Instantiate(phoneUI, __instance.transform);
        container.transform.localPosition = new(0, 0, -5f);
        _guesserUI = container.gameObject;

        int i = 0;
        Transform buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        Transform maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        Transform smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        TextMeshPro textTemplate = __instance.playerStates[0].NameText;

        Transform exitButtonParent = new GameObject().transform;
        exitButtonParent.SetParent(container);
        Transform exitButton = UnityObject.Instantiate(buttonTemplate.transform, exitButtonParent);
        _ = UnityObject.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        exitButtonParent.transform.localPosition = new(2.725f, 2.1f, -5);
        exitButtonParent.transform.localScale = new(0.217f, 0.9f, 1);
        exitButtonParent.transform.SetAsFirstSibling();
        _guesserUIExitButton = exitButton.GetComponent<PassiveButton>();
        _guesserUIExitButton.OnClick.RemoveAllListeners();
        _guesserUIExitButton.OnClick.AddListener((Action)(() =>
        {
            __instance.playerStates
                      .ToList()
                      .ForEach(x =>
                      {
                          x.gameObject.SetActive(true);
                          if (PlayerControl.LocalPlayer.IsDead() && x.transform.FindChild("ShootButton") != null) UnityObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                      });
            UnityObject.Destroy(container.gameObject);
        }));

        List<Transform> buttons = [];
        Transform selectedButton = null;

        foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfos)
        {
            RoleType guesserRole = PlayerControl.LocalPlayer.IsRole(RoleType.NiceGuesser) ? RoleType.NiceGuesser : PlayerControl.LocalPlayer.IsRole(RoleType.EvilGuesser) || PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) ? RoleType.EvilGuesser : RoleType.NiceGuesser;

            if (roleInfo == null || roleInfo.RoleType == guesserRole || (Guesser.OnlyAvailableRoles && !roleInfo.Enabled) || (!Guesser.EvilCanKillSpy && guesserRole == RoleType.EvilGuesser && roleInfo.RoleType == RoleType.Spy)) continue; // Not guessable roles & modifier

            Transform buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            Transform button = UnityObject.Instantiate(buttonTemplate, buttonParent);
            _ = UnityObject.Instantiate(maskTemplate, buttonParent);
            TextMeshPro label = UnityObject.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = MapUtilities.CachedShipStatus.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            buttons.Add(button);
            int row = i / 5, col = i % 5;
            buttonParent.localPosition = new(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -5);
            buttonParent.localScale = new(0.55f, 0.55f, 1f);
            label.text = Helpers.Cs(roleInfo.Color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            button.GetComponent<PassiveButton>()
                  .OnClick
                  .AddListener((Action)(() =>
                  {
                      if (selectedButton != button)
                      {
                          selectedButton = button;
                          buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                      }
                      else
                      {
                          PlayerControl focusedTarget = Helpers.PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                          if (!(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted) || focusedTarget == null || Guesser.RemainingShots(PlayerControl.LocalPlayer) <= 0) return;
                          if (Guesser.RemainingShots(PlayerControl.LocalPlayer) <= 0) return;

                          if (!Guesser.KillsThroughShield)
                          {
                              if (Medic.Shielded == focusedTarget)
                              {
                                  // Depending on the options, shooting the shielded player will not allow the guess, notify everyone about the kill attempt and close the window
                                  __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                                  UnityObject.Destroy(container.gameObject);

                                  {
                                      using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShieldedMurderAttempt);
                                      RPCProcedure.ShieldedMurderAttempt();
                                  }
                                  return;
                              }
                          }

                          RoleInfo mainRoleInfo = RoleInfo.GetRoleInfoForPlayer(focusedTarget, false).FirstOrDefault();
                          if (mainRoleInfo == null) return;

                          PlayerControl dyingTarget = mainRoleInfo == roleInfo ? focusedTarget : PlayerControl.LocalPlayer;

                          // Reset the GUI
                          __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                          UnityObject.Destroy(container.gameObject);
                          if (Guesser.HasMultipleShotsPerMeeting && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 1 && dyingTarget != PlayerControl.LocalPlayer)
                          {
                              __instance.playerStates
                                        .ToList()
                                        .ForEach(x =>
                                        {
                                            if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                                        });
                          }
                          else
                          {
                              __instance.playerStates
                                        .ToList()
                                        .ForEach(x =>
                                        {
                                            if (x.transform.FindChild("ShootButton") != null) UnityObject.Destroy(x.transform.FindChild("ShootButton").gameObject);
                                        });
                          }

                          // Shoot player and send chat info if activated
                          {
                              using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.GuesserShoot);
                              sender.Write(PlayerControl.LocalPlayer.PlayerId);
                              sender.Write(dyingTarget.PlayerId);
                              sender.Write(focusedTarget.PlayerId);
                              sender.Write((byte)roleInfo.RoleType);
                              RPCProcedure.GuesserShoot(PlayerControl.LocalPlayer.PlayerId, dyingTarget.PlayerId, focusedTarget.PlayerId, (byte)roleInfo.RoleType);
                          }
                      }
                  }));

            i++;
        }

        container.transform.localScale *= 0.75f;
    }

    internal static void PopulateButtonsPostfix(MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0) return;

        // Add Swapper Buttons
        bool addSwapperButtons = Swapper.Exists && PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) && Swapper.PlayerControl.IsAlive();
        if (addSwapperButtons)
        {
            _selections = new bool[__instance.playerStates.Length];
            _renderers = new SpriteRenderer[__instance.playerStates.Length];
            _swapperButtonList = new PassiveButton[__instance.playerStates.Length];

            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.NiceSwapper) && Swapper.CanOnlySwapOthers)) continue;

                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject checkbox = UnityObject.Instantiate(template, playerVoteArea.transform, true);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
                SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
                renderer.sprite = AssetLoader.SwapperCheck;
                renderer.color = Color.red;

                if (Swapper.RemainSwaps <= 0) renderer.color = Color.gray;

                PassiveButton button = checkbox.GetComponent<PassiveButton>();
                _swapperButtonList[i] = button;
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((Action)(() => SwapperOnClick(copiedIndex, __instance)));

                _selections[i] = false;
                _renderers[i] = renderer;
            }

            Transform meetingUI = UnityObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            Transform buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            Transform maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            TextMeshPro textTemplate = __instance.playerStates[0].NameText;
            Transform meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            Transform meetingExtraButton = UnityObject.Instantiate(buttonTemplate, meetingExtraButtonParent);

            Transform infoTransform = __instance.playerStates[0].NameText.transform.parent.FindChild("Info");
            _meetingExtraButtonText = UnityObject.Instantiate(__instance.playerStates[0].NameText, meetingExtraButtonParent);
            _meetingExtraButtonText.text = Tr.Get(TrKey.SwapperSwapsLeft, Swapper.RemainSwaps);
            _meetingExtraButtonText.alignment = TextAlignmentOptions.Right;
            _meetingExtraButtonText.enableWordWrapping = false;
            _meetingExtraButtonText.transform.localScale = Vector3.one * 1.7f;
            _meetingExtraButtonText.transform.localPosition = new(-3.3f, 0f, 0f);

            _ = UnityObject.Instantiate(maskTemplate, meetingExtraButtonParent);
            _meetingExtraButtonLabel = UnityObject.Instantiate(textTemplate, meetingExtraButton);
            meetingExtraButton.GetComponent<SpriteRenderer>().sprite = MapUtilities.CachedShipStatus.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            meetingExtraButtonParent.localPosition = new(0, -2.225f, -5);
            meetingExtraButtonParent.localScale = new(0.55f, 0.55f, 1f);
            _meetingExtraButtonLabel.alignment = TextAlignmentOptions.Center;
            _meetingExtraButtonLabel.transform.localPosition = new(0, 0, _meetingExtraButtonLabel.transform.localPosition.z);
            _meetingExtraButtonLabel.transform.localScale *= 1.7f;
            _meetingExtraButtonLabel.text = Helpers.Cs(Color.red, Tr.Get(TrKey.SwapperConfirm));

            PassiveButton passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (PlayerControl.LocalPlayer.IsAlive()) passiveButton.OnClick.AddListener((Action)(() => SwapperConfirm(__instance)));

            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if (Mathf.Approximately(p, 1f)) meetingExtraButton.parent.gameObject.SetActive(true);
            })));
        }

        // Add overlay for spelled players
        if (Witch.Exists && Witch.FutureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (!Witch.FutureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId)) continue;
                SpriteRenderer rend = new GameObject().AddComponent<SpriteRenderer>();
                rend.transform.SetParent(pva.transform);
                rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                rend.transform.localPosition = new(-0.5f, -0.03f, -1f);
                if (PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId)) rend.transform.localPosition = new(-0.725f, -0.15f, -1f);
                rend.sprite = AssetLoader.SpellButtonMeeting;
            }
        }

        // トラックボタン
        bool isTrackerButton = EvilTracker.CanSetTargetOnMeeting && EvilTracker.Target == null && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && PlayerControl.LocalPlayer.IsAlive();
        if (isTrackerButton)
        {
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject targetBox = UnityObject.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "EvilTrackerButton";
                targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = AssetLoader.Arrow;
                renderer.color = Palette.CrewmateBlue;
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((Action)(() =>
                {
                    if (__instance == null || __instance.playerStates == null || copiedIndex < 0 || copiedIndex >= __instance.playerStates.Length) return;
                    PlayerControl focusedTarget = Helpers.PlayerById(__instance.playerStates[copiedIndex].TargetPlayerId);
                    EvilTracker.Target = focusedTarget;
                    // Reset the GUI
                    __instance.playerStates
                              .ToList()
                              .ForEach(x =>
                              {
                                  if (x.transform.FindChild("EvilTrackerButton") != null) UnityObject.Destroy(x.transform.FindChild("EvilTrackerButton").gameObject);
                              });
                    GameObject targetMark = UnityObject.Instantiate(template, playerVoteArea.transform);
                    targetMark.name = "EvilTrackerMark";
                    PassiveButton passiveButton = targetMark.GetComponent<PassiveButton>();
                    targetMark.transform.localPosition = new(1.1f, 0.03f, -20f);
                    UnityObject.Destroy(passiveButton);
                    SpriteRenderer spriteRenderer = targetMark.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = AssetLoader.Arrow;
                    spriteRenderer.color = Palette.CrewmateBlue;

                    bool isGuesserButton = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
                    bool isLastImpostorButton = PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) && PlayerControl.LocalPlayer.IsAlive() && LastImpostor.CanGuess();
                    if (isGuesserButton || isLastImpostorButton) CreateGuesserButton(__instance);
                }));
            }
        }

        // Add Guesser Buttons
        bool isGuesser = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
        bool isLastImpostorButton = !isTrackerButton && PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) && PlayerControl.LocalPlayer.IsAlive() && LastImpostor.CanGuess();
        if (isGuesser || isLastImpostorButton) CreateGuesserButton(__instance);
    }

    private static void CreateGuesserButton(MeetingHud __instance)
    {
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            PlayerVoteArea playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
            GameObject targetBox = UnityObject.Instantiate(template, playerVoteArea.transform);
            targetBox.name = "ShootButton";
            targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.TargetIcon;
            PassiveButton button = targetBox.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            int copiedIndex = i;
            button.OnClick.AddListener((Action)(() => GuesserOnClick(copiedIndex, __instance)));
        }
    }

    internal static void Deserialize(MeetingHud __instance, MessageReader reader, bool initialState)
    {
        // Add swapper buttons
        if (initialState) PopulateButtonsPostfix(__instance);
    }

    internal static bool VoteAreaSelect()
    {
        return !(PlayerControl.LocalPlayer != null && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && _guesserUI != null);
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
            PlayerControl lp = PlayerControl.LocalPlayer;
            if (lp != null)
            {
                int numGuesses = Guesser.RemainingShots(lp);
                if ((Guesser.IsGuesser(lp.PlayerId) || lp.HasModifier(ModifierType.LastImpostor)) && lp.IsAlive() && numGuesses > 0)
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

        string text = InfoStringBuilder.ToString().Trim();
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
        {
            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (PlayerControl.LocalPlayer.MyPhysics.enabled && (PlayerControl.LocalPlayer.moveable || PlayerControl.LocalPlayer.inVent || Hacker.HackerVitalsButton.IsEffectActive || Hacker.HackerAdminTableButton.IsEffectActive || SecurityGuard.SecurityGuardCamButton.IsEffectActive))
            {
                if (!PlayerControl.LocalPlayer.inMovingPlat)
                    AntiTeleport.Position = PlayerControl.LocalPlayer.transform.position;
            }

            // Reset vampire bitten
            Vampire.Bitten = null;
            // Count meetings
            if (meetingTarget == null) MapSettings.MeetingsCount++;
            // Save the meeting target
            _target = meetingTarget;
            Medium.MeetingStartTime = DateTime.UtcNow;

            StartMeetingClear();
        }

        {
            bool isEmergency = _target == null;
            DestroyableSingleton<UnityTelemetry>.Instance.WriteMeetingStarted(isEmergency);
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.MeetingStarted(__instance.Data, _target == null);
            MapUtilities.CachedShipStatus.StartCoroutine(CoStartMeeting(__instance, _target).WrapToIl2Cpp());
            if (!__instance.AmOwner) return false;

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
            while (!MeetingHud.Instance) yield return null;

            MeetingRoomManager.Instance.RemoveSelf();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator()) player?.ResetForMeeting();

            if (MapBehaviour.Instance) MapBehaviour.Instance.Close();

            if (Minigame.Instance) Minigame.Instance.ForceClose();

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
            HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
            SpriteRenderer blackScreen = UnityObject.Instantiate(hudManager.FullScreen, hudManager.transform);
            SpriteRenderer greyScreen = UnityObject.Instantiate(hudManager.FullScreen, hudManager.transform);
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
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            if (roomTracker != null)
            {
                GameObject gameObject = UnityObject.Instantiate(roomTracker.gameObject, FastDestroyableSingleton<HudManager>.Instance.transform, true);
                UnityObject.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                gameObject.transform.localPosition = new(0, 0, -930f);
                gameObject.transform.localScale = Vector3.one * 5f;
                text = gameObject.GetComponent<TMP_Text>();
            }

            yield return Effects.Lerp(Delay, new Action<float>(p =>
            {
                // Delayed action
                greyScreen.color = new(1.0f, 1.0f, 1.0f, 0.5f - (p / 2));
                string message = (Delay - (p * Delay)).ToString("0.00");
                if (message == "0") return;
                const string prefix = "<color=#FFFFFFFF>";
                if (text == null) return;
                text.text = prefix + message + "</color>";
                text?.color = Color.white;
            }));
            // yield return new WaitForSeconds(2f);
            if (text != null) UnityObject.Destroy(text.gameObject);
            UnityObject.Destroy(blackScreen);
            UnityObject.Destroy(greyScreen);

            // ミーティング画面の並び替えを直す
            PopulateButtons(MeetingHud.Instance, reporter.Data.PlayerId);
            PopulateButtonsPostfix(MeetingHud.Instance);

            HandleReportDeadBody(reporter, target);
        }

        // 既存処理の移植
        {
            Il2CppArrayBase<DeadBody> array = UnityObject.FindObjectsOfType<DeadBody>();
            NetworkedPlayerInfo[] deadBodies = [.. (from b in array select GameData.Instance.GetPlayerById(b.ParentId))];
            foreach (DeadBody t in array)
            {
                if (t != null && t.gameObject != null)
                    UnityObject.Destroy(t.gameObject);
                else
                    Logger.LogError("Encountered a null Dead Body while destroying.");
            }

            Il2CppArrayBase<ShapeshifterEvidence> array2 = UnityObject.FindObjectsOfType<ShapeshifterEvidence>();
            foreach (ShapeshifterEvidence t in array2)
            {
                if (t != null && t.gameObject != null)
                    UnityObject.Destroy(t.gameObject);
                else
                    Logger.LogError("Encountered a null Evidence while destroying.");
            }

            MeetingHud.Instance.StartCoroutine(MeetingHud.Instance.CoIntro(reporter.Data, target, deadBodies));
        }
    }

    private static void PopulateButtons(MeetingHud __instance, byte reporter)
    {
        // 投票画面に人形遣いのダミーを表示させない
        // 会議に参加しないPlayerControlを持つRoleが増えたらこのListに追加
        // 特殊なplayerInfo.Role.Roleを設定することで自動的に無視できないか？もしくはフラグをplayerInfoのどこかに追加
        List<PlayerControl> playerControlsToBeIgnored = [];
        playerControlsToBeIgnored.RemoveAll(x => x == null);
        IEnumerable<byte> playerIdsToBeIgnored = playerControlsToBeIgnored.Select(x => x.PlayerId);
        // Generate PlayerVoteAreas
        IEnumerable<byte> idsToBeIgnored = playerIdsToBeIgnored as byte[] ?? [.. playerIdsToBeIgnored];
        __instance.playerStates = new PlayerVoteArea[GameData.Instance.PlayerCount - idsToBeIgnored.Count()];
        int playerStatesCounter = 0;
        for (int i = 0; i < __instance.playerStates.Length + idsToBeIgnored.Count(); i++)
        {
            if (idsToBeIgnored.Contains(GameData.Instance.AllPlayers[i].PlayerId)) continue;

            NetworkedPlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
            PlayerVoteArea playerVoteArea = __instance.playerStates[playerStatesCounter] = __instance.CreateButton(playerInfo);
            playerVoteArea.Parent = __instance;
            playerVoteArea.SetTargetPlayerId(playerInfo.PlayerId);
            playerVoteArea.SetDead(reporter == playerInfo.PlayerId, playerInfo.Disconnected || playerInfo.IsDead, playerInfo.Role.Role == RoleTypes.GuardianAngel);
            playerVoteArea.UpdateOverlay();
            playerStatesCounter++;
        }

        foreach (PlayerVoteArea playerVoteArea2 in __instance.playerStates) ControllerManager.Instance.AddSelectableUiElement(playerVoteArea2.PlayerButton);

        __instance.SortButtons();
    }

    private static void HandleReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        if (target == null) return;
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null) return;

        bool isLocalReporter = reporter.PlayerId == localPlayer.PlayerId;
        bool isMedicReport = Medic.Exists && localPlayer.IsRole(RoleType.Medic) && isLocalReporter;
        bool isDetectiveReport = Detective.Exists && localPlayer.IsRole(RoleType.Detective) && isLocalReporter;

        if (!isMedicReport && !isDetectiveReport) return;
        DeadPlayer deadPlayer = GameHistory.GetDeadPlayer(target.PlayerId);

        if (deadPlayer == null || deadPlayer.KillerIfExisting == null) return;
        float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;

        StringBuilder sb = new();
        if (isMedicReport)
            sb.AppendFormat(Tr.Get(TrKey.MedicReport), (int)Math.Round(timeSinceDeath / 1000));
        else
        {
            if (timeSinceDeath < Detective.ReportNameDuration * 1000)
                sb.AppendFormat(Tr.Get(TrKey.DetectiveReportName), deadPlayer.KillerIfExisting.Data.PlayerName);
            else if (timeSinceDeath < Detective.ReportColorDuration * 1000)
            {
                string typeOfColor = Helpers.IsLighterColor(deadPlayer.KillerIfExisting.Data.DefaultOutfit.ColorId) ? Tr.Get(TrKey.DetectiveColorLight) : Tr.Get(TrKey.DetectiveColorDark);
                sb.AppendFormat(Tr.Get(TrKey.DetectiveReportColor), typeOfColor);
            }
            else
                sb.Append(Tr.Get(TrKey.DetectiveReportNone));
        }

        string msg = sb.ToString();
        if (string.IsNullOrWhiteSpace(msg)) return;
        if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance) FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(CoAddDelayedChat(reporter, msg).WrapToIl2Cpp());

        if (msg.Contains("who", StringComparison.OrdinalIgnoreCase)) FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
    }

    private static IEnumerator CoAddDelayedChat(PlayerControl source, string msg)
    {
        // 会議画面が準備できるまで待機
        float timer = 0f;
        while ((!MeetingHud.Instance || MeetingHud.Instance.playerStates == null) && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // さらに少し待って、チャットがDidVoteエラーを吐かないようにする
        yield return new WaitForSeconds(1.0f);

        if (MeetingHud.Instance == null || MeetingHud.Instance.playerStates == null) yield break;
        // 安全確認: 会議ボタンの一覧にプレイヤーが存在するかチェック
        bool found = false;
        foreach (PlayerVoteArea state in MeetingHud.Instance.playerStates)
        {
            if (state == null || state.TargetPlayerId != source.PlayerId) continue;
            found = true;
            break;
        }

        if (found)
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(source, msg);
        else
            Logger.LogWarn($"Player {source.Data.PlayerName} not found in MeetingHud. Message suppressed to prevent crash.");
    }
}