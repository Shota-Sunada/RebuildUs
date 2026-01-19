using System.Collections;
using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace RebuildUs.Modules;

public static class Meeting
{
    static bool[] Selections;
    static SpriteRenderer[] Renderers;
    private static NetworkedPlayerInfo Target = null;
    private const float Scale = 0.65f;
    private static TextMeshPro MeetingExtraButtonText;
    private static PassiveButton[] SwapperButtonList;
    private static TextMeshPro MeetingExtraButtonLabel;
    private static PlayerVoteArea Swapped1 = null;
    private static PlayerVoteArea Swapped2 = null;

    // GMH
    public static bool AnimateSwap = false;
    static TextMeshPro MeetingInfoText;

    public static void Update(MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Animating)
        {
            return;
        }

        // Deactivate skip Button if skipping on emergency meetings is disabled
        if (ModMapOptions.BlockSkippingInEmergencyMeetings)
        {
            __instance.SkipVoteButton?.gameObject?.SetActive(false);
        }

        UpdateMeetingText(__instance);

        // This fixes a bug with the original game where pressing the button and a kill happens simultaneously
        // results in bodies sometimes being created *after* the meeting starts, marking them as dead and
        // removing the corpses so there's no random corpses leftover afterwards
        foreach (var deadBody in UnityEngine.Object.FindObjectsOfType<DeadBody>())
        {
            if (deadBody == null) continue;

            foreach (var pva in __instance.playerStates)
            {
                if (pva == null) continue;

                if (pva.TargetPlayerId == deadBody?.ParentId && !pva.AmDead)
                {
                    pva?.SetDead(pva.DidReport, true);
                    pva?.Overlay?.gameObject?.SetActive(true);
                }
            }
        }
    }

    private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
    {
        var dictionary = new Dictionary<byte, int>();
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];

            if (playerVoteArea.VotedFor is not 252 and not 255 and not 254)
            {
                var player = Helpers.PlayerById(playerVoteArea.TargetPlayerId);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

                var additionalVotes = (Mayor.Exists && Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.Mayor)) ? Mayor.NumVotes : 1; // Mayor vote
                dictionary[playerVoteArea.VotedFor] = dictionary.TryGetValue(playerVoteArea.VotedFor, out int currentVotes) ? currentVotes + additionalVotes : additionalVotes;
            }
        }

        // Swapper swap votes
        if (Swapper.Exists && Swapper.LivingPlayers.Count != 0)
        {
            Swapped1 = null;
            Swapped2 = null;
            foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1) Swapped1 = playerVoteArea;
                if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2) Swapped2 = playerVoteArea;
            }

            if (Swapped1 != null && Swapped2 != null)
            {
                if (!dictionary.ContainsKey(Swapped1.TargetPlayerId)) dictionary[Swapped1.TargetPlayerId] = 0;
                if (!dictionary.ContainsKey(Swapped2.TargetPlayerId)) dictionary[Swapped2.TargetPlayerId] = 0;
                (dictionary[Swapped2.TargetPlayerId], dictionary[Swapped1.TargetPlayerId]) = (dictionary[Swapped1.TargetPlayerId], dictionary[Swapped2.TargetPlayerId]);
            }
        }

        return dictionary;
    }

    public static bool CheckForEndVoting(MeetingHud __instance)
    {
        if (__instance.playerStates.All(ps => ps.AmDead || ps.DidVote))
        {
            if (Target == null && ModMapOptions.BlockSkippingInEmergencyMeetings && ModMapOptions.NoVoteIsSelfVote)
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
            var max = self.MaxPair(out bool tie);
            var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key && !v.IsDead);

            var array = new MeetingHud.VoterState[__instance.playerStates.Length];
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                array[i] = new MeetingHud.VoterState
                {
                    VoterId = playerVoteArea.TargetPlayerId,
                    VotedForId = playerVoteArea.VotedFor
                };
            }

            __instance.RpcVotingComplete(array, exiled, tie);
        }

        return false;
    }

    public static bool BloopAVoteIcon(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        var spriteRenderer = UnityEngine.Object.Instantiate(__instance.PlayerVotePrefab);
        var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() ||
                            (PlayerControl.LocalPlayer.Data.IsDead && ModMapOptions.GhostsSeeVotes) ||
                            (PlayerControl.LocalPlayer.IsRole(RoleType.Mayor) && Mayor.MayorCanSeeVoteColors && TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data).Item1 >= Mayor.MayorTasksNeededToSeeVoteColors);
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

    public static bool PopulateVotes(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        // Swapper swap
        Swapped1 = null;
        Swapped2 = null;
        foreach (var playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1) Swapped1 = playerVoteArea;
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2) Swapped2 = playerVoteArea;
        }
        bool doSwap = Swapped1 != null && Swapped2 != null && Swapper.Exists && Swapper.LivingPlayers.Count != 0;
        if (doSwap)
        {
            __instance.StartCoroutine(Effects.Slide3D(Swapped1.transform, Swapped1.transform.localPosition, Swapped2.transform.localPosition, 1.5f));
            __instance.StartCoroutine(Effects.Slide3D(Swapped2.transform, Swapped2.transform.localPosition, Swapped1.transform.localPosition, 1.5f));
        }

        __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
        var num = 0;
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            var targetPlayerId = playerVoteArea.TargetPlayerId;
            // Swapper change playerVoteArea that gets the votes
            if (doSwap && playerVoteArea.TargetPlayerId == Swapped1.TargetPlayerId) playerVoteArea = Swapped2;
            else if (doSwap && playerVoteArea.TargetPlayerId == Swapped2.TargetPlayerId) playerVoteArea = Swapped1;

            playerVoteArea.ClearForResults();
            var num2 = 0;
            Dictionary<int, int> votesApplied = [];
            for (int j = 0; j < states.Length; j++)
            {
                var voterState = states[j];
                var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                var voter = Helpers.PlayerById(voterState.VoterId);
                if (voter == null) continue;

                if (playerById == null)
                {
                    Debug.LogError(string.Format("Couldn't find player info for voter: {0}", voterState.VoterId));
                }
                else if (i == 0 && voterState.SkippedVote && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                    num++;
                }
                else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead)
                {
                    __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                    num2++;
                }

                if (!votesApplied.ContainsKey(voter.PlayerId))
                {
                    votesApplied[voter.PlayerId] = 0;
                }
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

    public static void VotingComplete(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states, NetworkedPlayerInfo exiled, bool tie)
    {
        // Reset swapper values
        Swapper.PlayerId1 = byte.MaxValue;
        Swapper.PlayerId2 = byte.MaxValue;

        MeetingInfoText?.gameObject.SetActive(false);

        foreach (var deadBody in UnityEngine.Object.FindObjectsOfType<DeadBody>())
        {
            UnityEngine.Object.Destroy(deadBody.gameObject);
        }

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

    public static bool Select(ref bool __result, MeetingHud __instance, [HarmonyArgument(0)] int suspectStateIdx)
    {
        __result = false;
        // if (GM.gm != null && GM.gm.PlayerId == suspectStateIdx) return false;
        if (ModMapOptions.NoVoteIsSelfVote && PlayerControl.LocalPlayer.PlayerId == suspectStateIdx) return false;
        if (ModMapOptions.BlockSkippingInEmergencyMeetings && suspectStateIdx == -1) return false;

        return true;
    }

    static void SwapperOnClick(int i, MeetingHud __instance)
    {
        if (Swapper.NumSwaps <= 0) return;
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (__instance.playerStates[i].AmDead) return;

        int selectedCount = Selections.Count(b => b);
        var renderer = Renderers[i];

        if (selectedCount == 0)
        {
            renderer.color = Color.green;
            Selections[i] = true;
        }
        else if (selectedCount == 1)
        {
            if (Selections[i])
            {
                renderer.color = Color.red;
                Selections[i] = false;
            }
            else
            {
                Selections[i] = true;
                renderer.color = Color.green;
                MeetingExtraButtonLabel.text = Helpers.Cs(Color.green, "Confirm Swap");
            }
        }
        else if (selectedCount == 2)
        {
            if (Selections[i])
            {
                renderer.color = Color.red;
                Selections[i] = false;
                MeetingExtraButtonLabel.text = Helpers.Cs(Color.red, "Confirm Swap");
            }
        }
    }

    static void SwapperConfirm(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel();  // This will stop the underlying buttons of the template from showing up
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (Selections.Count(b => b) != 2) return;
        if (Swapper.NumSwaps <= 0 || Swapper.PlayerId1 != byte.MaxValue) return;

        PlayerVoteArea firstPlayer = null;
        PlayerVoteArea secondPlayer = null;
        for (int a = 0; a < Selections.Length; a++)
        {
            if (Selections[a])
            {
                if (firstPlayer == null)
                {
                    firstPlayer = __instance.playerStates[a];
                }
                else
                {
                    secondPlayer = __instance.playerStates[a];
                }
                Renderers[a].color = Color.green;
            }
            else
            {
                Renderers[a]?.color = Color.gray;
            }
            SwapperButtonList[a]?.OnClick.RemoveAllListeners();  // Swap buttons can't be clicked / changed anymore
        }
        if (firstPlayer != null && secondPlayer != null)
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SwapperSwap);
                sender.Write(firstPlayer.TargetPlayerId);
                sender.Write(secondPlayer.TargetPlayerId);
                RPCProcedure.SwapperSwap(firstPlayer.TargetPlayerId, secondPlayer.TargetPlayerId);
            }

            MeetingExtraButtonLabel.text = Helpers.Cs(Color.green, "Swapping!");
            Swapper.RemainSwaps--;
            MeetingExtraButtonText.text = $"Swaps: {Swapper.RemainSwaps}";
        }
    }

    public static void SwapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or died in the meeting, check if this affects the swapper.
        if (!Swapper.Exists || __instance.state == MeetingHud.VoteStates.Results) return;

        // reset swap.
        bool reset = false;
        if (dyingPlayerId == Swapper.PlayerId1 || dyingPlayerId == Swapper.PlayerId2)
        {
            reset = true;
            Swapper.PlayerId1 = Swapper.PlayerId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Swapper)) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (int i = 0; i < __instance.playerStates.Count; i++)
        {
            reset = reset || Selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId;
            if (reset) break;
        }

        if (!reset) return;

        for (int i = 0; i < Selections.Length; i++)
        {
            Selections[i] = false;
            PlayerVoteArea playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || (Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.Swapper) && Swapper.CanOnlySwapOthers)) continue;
            Renderers[i].color = Color.red;
            Swapper.RemainSwaps++;
            int copyI = i;
            SwapperButtonList[i].OnClick.RemoveAllListeners();
            SwapperButtonList[i].OnClick.AddListener((Action)(() => SwapperOnClick(copyI, __instance)));
        }
        MeetingExtraButtonText.text = $"Swaps: {Swapper.RemainSwaps}";
        MeetingExtraButtonLabel.text = Helpers.Cs(Color.red, "Confirm Swap");
    }

    public static GameObject GuesserUI;
    public static PassiveButton GuesserUIExitButton;
    public static byte GuesserCurrentTarget;
    static void GuesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (GuesserUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

        var phoneUI = UnityEngine.Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
        var container = UnityEngine.Object.Instantiate(phoneUI, __instance.transform);
        container.transform.localPosition = new Vector3(0, 0, -5f);
        GuesserUI = container.gameObject;

        int i = 0;
        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        var textTemplate = __instance.playerStates[0].NameText;

        GuesserCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

        var exitButtonParent = new GameObject().transform;
        exitButtonParent.SetParent(container);
        var exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
        var exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
        exitButtonParent.transform.localScale = new Vector3(0.217f, 0.9f, 1);
        exitButtonParent.transform.SetAsFirstSibling();
        GuesserUIExitButton = exitButton.GetComponent<PassiveButton>();
        GuesserUIExitButton.OnClick.RemoveAllListeners();
        GuesserUIExitButton.OnClick.AddListener((Action)(() =>
        {
            __instance.playerStates.ToList().ForEach(x =>
            {
                x.gameObject.SetActive(true);
                if (PlayerControl.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
            });
            UnityEngine.Object.Destroy(container.gameObject);
        }));

        var buttons = new List<Transform>();
        Transform selectedButton = null;

        foreach (var roleInfo in RoleInfo.AllRoleInfos)
        {
            RoleType guesserRole = PlayerControl.LocalPlayer.IsRole(RoleType.NiceGuesser)
                ? RoleType.NiceGuesser
                : PlayerControl.LocalPlayer.IsRole(RoleType.EvilGuesser) || PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor)
                    ? RoleType.EvilGuesser
                    : RoleType.NiceGuesser;

            if (roleInfo == null ||
                roleInfo.RoleType == guesserRole ||
                (Guesser.OnlyAvailableRoles && !roleInfo.Enabled) ||
                (!Guesser.EvilCanKillSpy && guesserRole == RoleType.EvilGuesser && roleInfo.RoleType == RoleType.Spy))
            {
                continue; // Not guessable roles & modifier
            }

            var buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            var button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
            var buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
            var label = UnityEngine.Object.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            buttons.Add(button);
            int row = i / 5, col = i % 5;
            buttonParent.localPosition = new Vector3(-3.47f + 1.75f * col, 1.5f - 0.45f * row, -5);
            buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            label.text = Helpers.Cs(roleInfo.Color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;
            int copiedIndex = i;

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.IsAlive() && !Helpers.PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId).Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
            {
                if (selectedButton != button)
                {
                    selectedButton = button;
                    buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                }
                else
                {
                    var focusedTarget = Helpers.PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                    if (!(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted) || focusedTarget == null || Guesser.RemainingShots(PlayerControl.LocalPlayer) <= 0) return;
                    if (Guesser.RemainingShots(PlayerControl.LocalPlayer) <= 0) return;

                    if (!Guesser.KillsThroughShield)
                    {
                        if (Medic.Shielded == focusedTarget)
                        {
                            // Depending on the options, shooting the shielded player will not allow the guess, notify everyone about the kill attempt and close the window
                            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                            UnityEngine.Object.Destroy(container.gameObject);

                            {
                                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShieldedMurderAttempt);
                                RPCProcedure.ShieldedMurderAttempt();
                            }
                            return;
                        }
                    }

                    var mainRoleInfo = RoleInfo.GetRoleInfoForPlayer(focusedTarget, false).FirstOrDefault();
                    if (mainRoleInfo == null) return;

                    var dyingTarget = (mainRoleInfo == roleInfo) ? focusedTarget : PlayerControl.LocalPlayer;

                    // Reset the GUI
                    __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                    UnityEngine.Object.Destroy(container.gameObject);
                    if (Guesser.HasMultipleShotsPerMeeting && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 1 && dyingTarget != PlayerControl.LocalPlayer)
                    {
                        __instance.playerStates.ToList().ForEach(x =>
                        {
                            if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                            {
                                UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            }
                        });
                    }
                    else
                    {
                        __instance.playerStates.ToList().ForEach(x =>
                        {
                            if (x.transform.FindChild("ShootButton") != null)
                            {
                                UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                            }
                        });
                    }

                    // Shoot player and send chat info if activated
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.GuesserShoot);
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

    public static void PopulateButtonsPostfix(MeetingHud __instance)
    {
        // Add Swapper Buttons
        bool addSwapperButtons = Swapper.Exists && PlayerControl.LocalPlayer.IsRole(RoleType.Swapper) && Swapper.LivingPlayers.Count != 0;
        if (addSwapperButtons)
        {
            Selections = new bool[__instance.playerStates.Length];
            Renderers = new SpriteRenderer[__instance.playerStates.Length];
            SwapperButtonList = new PassiveButton[__instance.playerStates.Length];

            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.Swapper) && Swapper.CanOnlySwapOthers)) continue;

                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject checkbox = UnityEngine.Object.Instantiate(template);
                checkbox.transform.SetParent(playerVoteArea.transform);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
                renderer.sprite = AssetLoader.SwapperCheck;
                renderer.color = Color.red;

                if (Swapper.RemainSwaps <= 0) renderer.color = Color.gray;

                PassiveButton button = checkbox.GetComponent<PassiveButton>();
                SwapperButtonList[i] = button;
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((Action)(() => SwapperOnClick(copiedIndex, __instance)));

                Selections[i] = false;
                Renderers[i] = renderer;
            }

            Transform meetingUI = UnityEngine.Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
            Transform meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            Transform meetingExtraButton = UnityEngine.Object.Instantiate(buttonTemplate, meetingExtraButtonParent);

            Transform infoTransform = __instance.playerStates[0].NameText.transform.parent.FindChild("Info");
            TextMeshPro meetingInfo = infoTransform?.GetComponent<TextMeshPro>();
            MeetingExtraButtonText = UnityEngine.Object.Instantiate(__instance.playerStates[0].NameText, meetingExtraButtonParent);
            MeetingExtraButtonText.text = addSwapperButtons ? $"Swaps: {Swapper.RemainSwaps}" : "";
            MeetingExtraButtonText.enableWordWrapping = false;
            MeetingExtraButtonText.transform.localScale = Vector3.one * 1.7f;
            MeetingExtraButtonText.transform.localPosition = new Vector3(-2.5f, 0f, 0f);

            Transform meetingExtraButtonMask = UnityEngine.Object.Instantiate(maskTemplate, meetingExtraButtonParent);
            MeetingExtraButtonLabel = UnityEngine.Object.Instantiate(textTemplate, meetingExtraButton);
            meetingExtraButton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
            meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            MeetingExtraButtonLabel.alignment = TMPro.TextAlignmentOptions.Center;
            MeetingExtraButtonLabel.transform.localPosition = new Vector3(0, 0, MeetingExtraButtonLabel.transform.localPosition.z);
            MeetingExtraButtonLabel.transform.localScale *= 1.7f;
            MeetingExtraButtonLabel.text = Helpers.Cs(Color.red, "Confirm Swap");

            PassiveButton passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                passiveButton.OnClick.AddListener((Action)(() => SwapperConfirm(__instance)));
            }
            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>((p) =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if (p == 1f)
                {
                    meetingExtraButton.parent.gameObject.SetActive(true);
                }
            })));
        }

        // Add overlay for spelled players
        if (Witch.Exists && Witch.FutureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (Witch.FutureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
                {
                    SpriteRenderer rend = new GameObject().AddComponent<SpriteRenderer>();
                    rend.transform.SetParent(pva.transform);
                    rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                    rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                    if (PlayerControl.LocalPlayer.IsRole(RoleType.Swapper) && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId)) rend.transform.localPosition = new Vector3(-0.725f, -0.15f, -1f);
                    rend.sprite = AssetLoader.SpellButtonMeeting;
                }
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
                GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "EvilTrackerButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = AssetLoader.Arrow;
                renderer.color = Palette.CrewmateBlue;
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((Action)(() =>
                {
                    PlayerControl focusedTarget = Helpers.PlayerById((byte)__instance.playerStates[copiedIndex].TargetPlayerId);
                    EvilTracker.Target = focusedTarget;
                    // Reset the GUI
                    __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("EvilTrackerButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("EvilTrackerButton").gameObject); });
                    GameObject targetMark = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetMark.name = "EvilTrackerMark";
                    PassiveButton button = targetMark.GetComponent<PassiveButton>();
                    targetMark.transform.localPosition = new Vector3(1.1f, 0.03f, -20f);
                    GameObject.Destroy(button);
                    SpriteRenderer renderer = targetMark.GetComponent<SpriteRenderer>();
                    renderer.sprite = AssetLoader.Arrow;
                    renderer.color = Palette.CrewmateBlue;

                    bool isGuesserButton = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
                    bool isLastImpostorButton = PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) && PlayerControl.LocalPlayer.IsAlive() && LastImpostor.CanGuess();
                    if (isGuesserButton || isLastImpostorButton)
                    {
                        CreateGuesserButton(__instance);
                    }
                }));
            }
        }

        // Add Guesser Buttons
        bool isGuesser = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
        bool isLastImpostorButton = !isTrackerButton && PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) && PlayerControl.LocalPlayer.IsAlive() && LastImpostor.CanGuess();
        if (isGuesser || isLastImpostorButton)
        {
            CreateGuesserButton(__instance);
        }
    }

    public static void CreateGuesserButton(MeetingHud __instance)
    {
        for (int i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
            targetBox.name = "ShootButton";
            targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.TargetIcon;
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            int copiedIndex = i;
            button.OnClick.AddListener((Action)(() => GuesserOnClick(copiedIndex, __instance)));
        }
    }

    public static void Deserialize(MeetingHud __instance, MessageReader reader, bool initialState)
    {
        // Add swapper buttons
        if (initialState)
        {
            PopulateButtonsPostfix(__instance);
        }
    }

    public static bool VoteAreaSelect()
    {
        return !(PlayerControl.LocalPlayer != null && Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && GuesserUI != null);
    }

    public static void UpdateMeetingText(MeetingHud __instance)
    {
        // Uses remaining text for guesser/swapper
        if (MeetingInfoText == null)
        {
            MeetingInfoText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, __instance.transform);
            MeetingInfoText.alignment = TextAlignmentOptions.BottomLeft;
            MeetingInfoText.transform.position = Vector3.zero;
            MeetingInfoText.transform.localPosition = new Vector3(-3.07f, 3.33f, -20f);
            MeetingInfoText.transform.localScale *= 1.1f;
            MeetingInfoText.color = Palette.White;
            MeetingInfoText.gameObject.SetActive(false);
        }

        MeetingInfoText.text = "";
        MeetingInfoText.gameObject.SetActive(false);

        if (MeetingHud.Instance.state is not MeetingHud.VoteStates.Voted and not MeetingHud.VoteStates.NotVoted and not MeetingHud.VoteStates.Discussion)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Swapper) && Swapper.RemainSwaps > 0 && Swapper.LivingPlayers.Count != 0)
        {
            MeetingInfoText.text = string.Format(Tr.Get("Hud.SwapperSwapsLeft"), Swapper.RemainSwaps);
            MeetingInfoText.gameObject.SetActive(true);
        }

        int numGuesses = Guesser.RemainingShots(PlayerControl.LocalPlayer);
        if ((Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) || PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor)) && PlayerControl.LocalPlayer.IsAlive() && numGuesses > 0)
        {
            MeetingInfoText.text = string.Format(Tr.Get("Hud.GuesserGuessesLeft"), numGuesses);
            MeetingInfoText.gameObject.SetActive(true);
        }

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Shifter) && Shifter.FutureShift != null)
        {
            MeetingInfoText.text = string.Format(Tr.Get("Hud.ShifterTargetInfo"), Shifter.FutureShift.Data.PlayerName);
            MeetingInfoText.gameObject.SetActive(true);
        }
    }

    public static void StartMeetingClear()
    {
        AnimateSwap = false;
        CustomOverlays.ShowBlackBG();
        CustomOverlays.HideInfoOverlay();
        CustomOverlays.HideRoleOverlay();
        RebuildUs.OnMeetingStart();
        Map.ShareRealTasks();
    }

    public static bool StartMeetingPrefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
    {
        {
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            byte roomId = byte.MinValue;
            if (roomTracker != null && roomTracker.LastRoom != null)
            {
                roomId = (byte)roomTracker.LastRoom?.RoomId;
            }

            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (PlayerControl.LocalPlayer.MyPhysics.enabled && (PlayerControl.LocalPlayer.moveable || PlayerControl.LocalPlayer.inVent
                || Hacker.HackerVitalsButton.IsEffectActive || Hacker.HackerAdminTableButton.IsEffectActive || SecurityGuard.SecurityGuardCamButton.IsEffectActive))
            {
                if (!PlayerControl.LocalPlayer.inMovingPlat)
                {
                    AntiTeleport.Position = PlayerControl.LocalPlayer.transform.position;
                }
            }

            // Reset vampire bitten
            Vampire.Bitten = null;
            // Count meetings
            if (meetingTarget == null) ModMapOptions.MeetingsCount++;
            // Save the meeting target
            Target = meetingTarget;
            Medium.MeetingStartTime = DateTime.UtcNow;

            StartMeetingClear();
        }

        {
            bool isEmergency = Target == null;
            DestroyableSingleton<UnityTelemetry>.Instance.WriteMeetingStarted(isEmergency);
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.MeetingStarted(__instance.Data, Target == null);
            ShipStatus.Instance.StartCoroutine(CoStartMeeting(__instance, Target).WrapToIl2Cpp());
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

    private static float Delay { get { return CustomOptionHolder.DelayBeforeMeeting.GetFloat(); } }

    private static IEnumerator CoStartMeeting(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        // 既存処理の移植
        {
            while (!MeetingHud.Instance)
            {
                yield return null;
            }
            MeetingRoomManager.Instance.RemoveSelf();
            for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
            {
                PlayerControl playerControl = PlayerControl.AllPlayerControls[i];
                playerControl?.ResetForMeeting();
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
        yield break;
    }

    private static IEnumerator CoStartMeeting2(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        // Modで追加する遅延処理
        {
            // ボタンと同時に通報が入った場合のバグ対応、他のクライアントからキルイベントが飛んでくるのを待つ
            // 見えては行けないものが見えるので暗転させる
            MeetingHud.Instance.state = MeetingHud.VoteStates.Animating; // ゲッサーのキル用meetingUpdateが呼ばれないようにするおまじない（呼ばれるとバグる）
            HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
            var blackScreen = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
            var greyScreen = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
            blackScreen.color = Palette.Black;
            blackScreen.transform.position = Vector3.zero;
            blackScreen.transform.localPosition = new Vector3(0f, 0f, -910f);
            blackScreen.transform.localScale = new Vector3(10f, 10f, 1f);
            blackScreen.gameObject.SetActive(true);
            blackScreen.enabled = true;
            greyScreen.color = Palette.Black;
            greyScreen.transform.position = Vector3.zero;
            greyScreen.transform.localPosition = new Vector3(0f, 0f, -920f);
            greyScreen.transform.localScale = new Vector3(10f, 10f, 1f);
            greyScreen.gameObject.SetActive(true);
            greyScreen.enabled = true;
            TMP_Text text;
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
            gameObject.transform.localPosition = new Vector3(0, 0, -930f);
            gameObject.transform.localScale = Vector3.one * 5f;
            text = gameObject.GetComponent<TMP_Text>();
            yield return Effects.Lerp(Delay, new Action<float>((p) =>
            {
                // Delayed action
                greyScreen.color = new Color(1.0f, 1.0f, 1.0f, 0.5f - p / 2);
                string message = (Delay - (p * Delay)).ToString("0.00");
                if (message == "0") return;
                string prefix = "<color=#FFFFFFFF>";
                text.text = prefix + message + "</color>";
                text?.color = Color.white;
            }));
            // yield return new WaitForSeconds(2f);
            UnityEngine.Object.Destroy(text.gameObject);
            UnityEngine.Object.Destroy(blackScreen);
            UnityEngine.Object.Destroy(greyScreen);

            // ミーティング画面の並び替えを直す
            PopulateButtons(MeetingHud.Instance, reporter.Data.PlayerId);
            PopulateButtonsPostfix(MeetingHud.Instance);
        }

        // 既存処理の移植
        {
            var array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            var deadBodies = (from b in array select GameData.Instance.GetPlayerById(b.ParentId)).ToArray();
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j] != null && array[j].gameObject != null)
                {
                    UnityEngine.Object.Destroy(array[j].gameObject);
                }
                else
                {
                    Debug.LogError("Encountered a null Dead Body while destroying.");
                }
            }
            var array2 = UnityEngine.Object.FindObjectsOfType<ShapeshifterEvidence>();
            for (int k = 0; k < array2.Length; k++)
            {
                if (array2[k] != null && array2[k].gameObject != null)
                {
                    UnityEngine.Object.Destroy(array2[k].gameObject);
                }
                else
                {
                    Debug.LogError("Encountered a null Evidence while destroying.");
                }
            }
            MeetingHud.Instance.StartCoroutine(MeetingHud.Instance.CoIntro(reporter.Data, target, deadBodies));
        }
        yield break;
    }

    public static void PopulateButtons(MeetingHud __instance, byte reporter)
    {
        // 投票画面に人形遣いのダミーを表示させない
        // 会議に参加しないPlayerControlを持つRoleが増えたらこのListに追加
        // 特殊なplayerInfo.Role.Roleを設定することで自動的に無視できないか？もしくはフラグをplayerInfoのどこかに追加
        var playerControlsToBeIgnored = new List<PlayerControl>() { };
        playerControlsToBeIgnored.RemoveAll(x => x == null);
        var playerIdsToBeIgnored = playerControlsToBeIgnored.Select(x => x.PlayerId);
        // Generate PlayerVoteAreas
        __instance.playerStates = new PlayerVoteArea[GameData.Instance.PlayerCount - playerIdsToBeIgnored.Count()];
        int playerStatesCounter = 0;
        for (int i = 0; i < __instance.playerStates.Length + playerIdsToBeIgnored.Count(); i++)
        {
            if (playerIdsToBeIgnored.Contains(GameData.Instance.AllPlayers[i].PlayerId))
            {
                continue;
            }
            var playerInfo = GameData.Instance.AllPlayers[i];
            var playerVoteArea = __instance.playerStates[playerStatesCounter] = __instance.CreateButton(playerInfo);
            playerVoteArea.Parent = __instance;
            playerVoteArea.SetTargetPlayerId(playerInfo.PlayerId);
            playerVoteArea.SetDead(reporter == playerInfo.PlayerId, playerInfo.Disconnected || playerInfo.IsDead, playerInfo.Role.Role == RoleTypes.GuardianAngel);
            playerVoteArea.UpdateOverlay();
            playerStatesCounter++;
        }
        foreach (var playerVoteArea2 in __instance.playerStates)
        {
            ControllerManager.Instance.AddSelectableUiElement(playerVoteArea2.PlayerButton, false);
        }
        __instance.SortButtons();
    }
}