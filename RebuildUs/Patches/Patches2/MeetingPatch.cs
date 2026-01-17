// namespace TheOtherRoles.Patches
// {
//     [HarmonyPatch]
//     class MeetingHudPatch
//     {
//         static bool[] selections;
//         static SpriteRenderer[] renderers;
//         private const float scale = 0.65f;
//         private static Sprite blankNameplate = null;
//         public static bool nameplatesChanged = true;
//         public static bool animateSwap = false;

//         static TMPro.TextMeshPro meetingInfoText;

//         static void gmKillOnClick(int i, MeetingHud __instance)
//         {
//             if (__instance.state == MeetingHud.VoteStates.Results) return;
//             SpriteRenderer renderer = renderers[i];
//             var target = __instance.playerStates[i];

//             if (target != null)
//             {
//                 if (target.AmDead)
//                 {
//                     MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GMRevive, Hazel.SendOption.Reliable, -1);
//                     writer.Write((byte)target.TargetPlayerId);
//                     AmongUsClient.Instance.FinishRpcImmediately(writer);
//                     RPCProcedure.GMRevive(target.TargetPlayerId);

//                     renderer.sprite = Guesser.getTargetSprite();
//                     renderer.color = Color.red;
//                 }
//                 else
//                 {
//                     MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GMKill, Hazel.SendOption.Reliable, -1);
//                     writer.Write((byte)target.TargetPlayerId);
//                     AmongUsClient.Instance.FinishRpcImmediately(writer);
//                     RPCProcedure.GMKill(target.TargetPlayerId);

//                     renderer.sprite = Swapper.getCheckSprite();
//                     renderer.color = Color.green;
//                 }
//             }
//         }

//         static void swapperOnClick(int i, MeetingHud __instance)
//         {
//             if (Swapper.numSwaps <= 0) return;
//             if (__instance.state == MeetingHud.VoteStates.Results) return;
//             if (__instance.playerStates[i].AmDead) return;

//             int selectedCount = selections.Where(b => b).Count();
//             SpriteRenderer renderer = renderers[i];

//             if (selectedCount == 0)
//             {
//                 renderer.color = Color.green;
//                 selections[i] = true;
//             }
//             else if (selectedCount == 1)
//             {
//                 if (selections[i])
//                 {
//                     renderer.color = Color.red;
//                     selections[i] = false;
//                 }
//                 else
//                 {
//                     selections[i] = true;
//                     renderer.color = Color.green;

//                     PlayerVoteArea firstPlayer = null;
//                     PlayerVoteArea secondPlayer = null;
//                     for (int A = 0; A < selections.Length; A++)
//                     {
//                         if (selections[A])
//                         {
//                             if (firstPlayer != null)
//                             {
//                                 secondPlayer = __instance.playerStates[A];
//                                 break;
//                             }
//                             else
//                             {
//                                 firstPlayer = __instance.playerStates[A];
//                             }
//                         }
//                     }

//                     if (firstPlayer != null && secondPlayer != null)
//                     {
//                         MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SwapperSwap, Hazel.SendOption.Reliable, -1);
//                         writer.Write((byte)firstPlayer.TargetPlayerId);
//                         writer.Write((byte)secondPlayer.TargetPlayerId);
//                         AmongUsClient.Instance.FinishRpcImmediately(writer);

//                         RPCProcedure.swapperSwap((byte)firstPlayer.TargetPlayerId, (byte)secondPlayer.TargetPlayerId);
//                     }
//                 }
//             }
//         }

//         private static GameObject guesserUI;

//         static void populateButtonsPostfix(MeetingHud __instance)
//         {
//             nameplatesChanged = true;

//             if (CachedPlayer.LocalPlayer.PlayerControl.isRole(RoleType.GM) && GM.canKill)
//             {
//                 renderers = new SpriteRenderer[__instance.playerStates.Length];

//                 for (int i = 0; i < __instance.playerStates.Length; i++)
//                 {
//                     PlayerVoteArea playerVoteArea = __instance.playerStates[i];

//                     GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
//                     GameObject checkbox = UnityEngine.Object.Instantiate(template);
//                     checkbox.transform.SetParent(playerVoteArea.transform);
//                     checkbox.transform.position = template.transform.position;
//                     checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -20f);
//                     SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
//                     renderer.sprite = playerVoteArea.AmDead ? Swapper.getCheckSprite() : Guesser.getTargetSprite();
//                     renderer.color = playerVoteArea.AmDead ? Color.green : Color.red;

//                     PassiveButton button = checkbox.GetComponent<PassiveButton>();
//                     button.OnClick.RemoveAllListeners();
//                     int copiedIndex = i;
//                     button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => gmKillOnClick(copiedIndex, __instance)));

//                     renderers[i] = renderer;
//                 }
//             }

//             // Add Swapper Buttons
//             if (CachedPlayer.LocalPlayer.PlayerControl.isRole(RoleType.Swapper) && Swapper.numSwaps > 0 && !Swapper.swapper.Data.IsDead)
//             {
//                 selections = new bool[__instance.playerStates.Length];
//                 renderers = new SpriteRenderer[__instance.playerStates.Length];

//                 for (int i = 0; i < __instance.playerStates.Length; i++)
//                 {
//                     PlayerVoteArea pddddddddddddddddddlayerVoteArea = __instance.playerStates[i];
//                     if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers) || (playerVoteArea.TargetPlayerId == GM.gm?.PlayerId)) continue;

//                     GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
//                     GameObject checkbox = UnityEngine.Object.Instantiate(template);
//                     checkbox.transform.SetParent(playerVoteArea.transform);
//                     checkbox.transform.position = template.transform.position;
//                     checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -20f);
//                     SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
//                     renderer.sprite = Swapper.getCheckSprite();
//                     renderer.color = Color.red;

//                     PassiveButton button = checkbox.GetComponent<PassiveButton>();
//                     button.OnClick.RemoveAllListeners();
//                     int copiedIndex = i;
//                     button.OnClick.AddListener((System.Action)(() => swapperOnClick(copiedIndex, __instance)));

//                     selections[i] = false;
//                     renderers[i] = renderer;
//                 }

//             }

//             // Add overlay for spelled players
//             if (Witch.witch != null && Witch.futureSpelled != null)
//             {
//                 foreach (PlayerVoteArea pva in __instance.playerStates)
//                 {
//                     if (Witch.futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
//                     {
//                         SpriteRenderer rend = new GameObject().AddComponent<SpriteRenderer>();
//                         rend.transform.SetParent(pva.transform);
//                         rend.gameObject.layer = pva.Megaphone.gameObject.layer;
//                         rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
//                         rend.sprite = Witch.getSpelledOverlaySprite();
//                     }
//                 }
//             }

//             // トラックボタン
//             bool isTrackerButton = EvilTracker.canSetTargetOnMeeting && EvilTracker.target == null && CachedPlayer.LocalPlayer.PlayerControl.isRole(RoleType.EvilTracker) && CachedPlayer.LocalPlayer.PlayerControl.isAlive();
//             if (isTrackerButton)
//             {
//                 for (int i = 0; i < __instance.playerStates.Length; i++)
//                 {
//                     PlayerVoteArea playerVoteArea = __instance.playerStates[i];
//                     if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) continue;
//                     GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
//                     GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
//                     targetBox.name = "EvilTrackerButton";
//                     targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
//                     SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
//                     renderer.sprite = EvilTracker.getArrowSprite();
//                     renderer.color = Palette.CrewmateBlue;
//                     PassiveButton button = targetBox.GetComponent<PassiveButton>();
//                     button.OnClick.RemoveAllListeners();
//                     int copiedIndex = i;
//                     button.OnClick.AddListener((System.Action)(() =>
//                     {
//                         PlayerControl focusedTarget = Helpers.playerById((byte)__instance.playerStates[copiedIndex].TargetPlayerId);
//                         EvilTracker.target = focusedTarget;
//                         // Reset the GUI
//                         __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("EvilTrackerButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("EvilTrackerButton").gameObject); });
//                         GameObject targetMark = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
//                         targetMark.name = "EvilTrackerMark";
//                         PassiveButton button = targetMark.GetComponent<PassiveButton>();
//                         targetMark.transform.localPosition = new Vector3(1.1f, 0.03f, -20f);
//                         GameObject.Destroy(button);
//                         SpriteRenderer renderer = targetMark.GetComponent<SpriteRenderer>();
//                         renderer.sprite = EvilTracker.getArrowSprite();
//                         renderer.color = Palette.CrewmateBlue;

//                         bool isGuesserButton = Guesser.isGuesser(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) && CachedPlayer.LocalPlayer.PlayerControl.isAlive() && Guesser.remainingShots(CachedPlayer.LocalPlayer.PlayerControl) > 0;
//                         bool isLastImpostorButton = CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.LastImpostor) && CachedPlayer.LocalPlayer.PlayerControl.isAlive() && LastImpostor.canGuess();
//                         if (isGuesserButton || isLastImpostorButton)
//                         {
//                             createGuesserButton(__instance);
//                         }
//                     }));
//                 }
//             }

//             // Add Guesser Buttons
//             bool isGuesserButton = !isTrackerButton && Guesser.isGuesser(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) && CachedPlayer.LocalPlayer.PlayerControl.isAlive() && Guesser.remainingShots(CachedPlayer.LocalPlayer.PlayerControl) > 0;
//             bool isLastImpostorButton = !isTrackerButton && CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.LastImpostor) && CachedPlayer.LocalPlayer.PlayerControl.isAlive() && LastImpostor.canGuess();
//             if (isGuesserButton || isLastImpostorButton)
//             {
//                 createGuesserButton(__instance);
//             }
//         }

//         public static void createGuesserButton(MeetingHud __instance)
//         {
//             for (int i = 0; i < __instance.playerStates.Length; i++)
//             {
//                 PlayerVoteArea playerVoteArea = __instance.playerStates[i];
//                 if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId || playerVoteArea.TargetPlayerId == GM.gm?.PlayerId) continue;

//                 GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
//                 GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
//                 targetBox.name = "ShootButton";
//                 targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
//                 SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
//                 renderer.sprite = Guesser.getTargetSprite();
//                 PassiveButton button = targetBox.GetComponent<PassiveButton>();
//                 button.OnClick.RemoveAllListeners();
//                 int copiedIndex = i;
//                 button.OnClick.AddListener((System.Action)(() => guesserOnClick(copiedIndex, __instance)));
//             }
//         }
//     }
// }
