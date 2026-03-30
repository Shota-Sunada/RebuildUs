namespace RebuildUs.Features;

internal static class GuesserModule
{
    private static GameObject _guesserUI;
    private static PassiveButton _guesserUIExitButton;

    public static void Reset()
    {
        _guesserUI = null;
        _guesserUIExitButton = null;
    }

    public static bool IsUIOpen()
    {
        return _guesserUI != null;
    }

    public static void GuesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (_guesserUI != null || !(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)) return;
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0) return;
        if (buttonTarget < 0 || buttonTarget >= __instance.playerStates.Length) return;

        foreach (var x in __instance.playerStates) x.gameObject.SetActive(false);

        Transform phoneUI = null;
        var transforms = UnityObject.FindObjectsOfType<Transform>();
        for (var i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == "PhoneUI")
            {
                phoneUI = transforms[i];
                break;
            }
        }
        var container = UnityObject.Instantiate(phoneUI, __instance.transform);
        container.transform.localPosition = new(0, 0, -5f);
        _guesserUI = container.gameObject;

        var buttonIndex = 0;
        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        var textTemplate = __instance.playerStates[0].NameText;

        var exitButtonParent = new GameObject().transform;
        exitButtonParent.SetParent(container);
        var exitButton = UnityObject.Instantiate(buttonTemplate.transform, exitButtonParent);
        _ = UnityObject.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        exitButtonParent.transform.localPosition = new(2.725f, 2.1f, -5);
        exitButtonParent.transform.localScale = new(0.217f, 0.9f, 1);
        exitButtonParent.transform.SetAsFirstSibling();
        _guesserUIExitButton = exitButton.GetComponent<PassiveButton>();
        _guesserUIExitButton.OnClick.RemoveAllListeners();
        _guesserUIExitButton.OnClick.AddListener((Action)(() =>
        {
            foreach (var x in __instance.playerStates)
            {
                x.gameObject.SetActive(true);
                if (PlayerControl.LocalPlayer.IsDead())
                {
                    var shootButton = x.transform.FindChild("ShootButton");
                    if (shootButton != null) UnityObject.Destroy(shootButton.gameObject);
                }
            }
            UnityObject.Destroy(container.gameObject);
        }));

        List<Transform> buttons = [];
        Transform selectedButton = null;

        var roleInfos = RoleInfo.AllRoleInfos;
        for (var i = 0; i < roleInfos.Count; i++)
        {
            var roleInfo = roleInfos[i];
            var guesserRole = PlayerControl.LocalPlayer.IsRole(RoleType.NiceGuesser) ? RoleType.NiceGuesser : PlayerControl.LocalPlayer.IsRole(RoleType.EvilGuesser) ? RoleType.EvilGuesser : RoleType.NiceGuesser;

            if (roleInfo == null || roleInfo.RoleType == guesserRole || (Guesser.OnlyAvailableRoles && !roleInfo.Enabled) || (!Guesser.EvilCanKillSpy && guesserRole == RoleType.EvilGuesser && roleInfo.RoleType == RoleType.Spy)) continue;

            var buttonParent = new GameObject().transform;
            buttonParent.SetParent(container);
            var button = UnityObject.Instantiate(buttonTemplate, buttonParent);
            _ = UnityObject.Instantiate(maskTemplate, buttonParent);
            var label = UnityObject.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = MapUtilities.CachedShipStatus.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            buttons.Add(button);
            int row = buttonIndex / 5, col = buttonIndex % 5;
            buttonParent.localPosition = new(-3.47f + 1.75f * col, 1.5f - 0.45f * row, -5);
            buttonParent.localScale = new(0.55f, 0.55f, 1f);
            label.text = Helpers.Cs(roleInfo.Color, roleInfo.Name);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
            {
                if (selectedButton != button)
                {
                    selectedButton = button;
                    foreach (var x in buttons) x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white;
                }
                else
                {
                    var focusedTarget = Helpers.PlayerById(__instance.playerStates[buttonTarget].TargetPlayerId);
                    if (!(__instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)
                    || focusedTarget == null
                    || Guesser.RemainingShots(PlayerControl.LocalPlayer) <= 0)
                    {
                        return;
                    }

                    if (!Guesser.KillsThroughShield && Medic.Shielded == focusedTarget)
                    {
                        foreach (var x in __instance.playerStates) x.gameObject.SetActive(true);
                        UnityObject.Destroy(container.gameObject);
                        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShieldedMurderAttempt))
                        {
                            RPCProcedure.ShieldedMurderAttempt();
                        }
                        return;
                    }

                    RoleInfo mainRoleInfo = null;
                    var targetRoles = RoleInfo.GetRoleInfoForPlayer(focusedTarget, false);
                    if (targetRoles.Count > 0) mainRoleInfo = targetRoles[0];
                    if (mainRoleInfo == null) return;

                    var dyingTarget = mainRoleInfo == roleInfo ? focusedTarget : PlayerControl.LocalPlayer;

                    foreach (var x in __instance.playerStates) x.gameObject.SetActive(true);
                    UnityObject.Destroy(container.gameObject);

                    if (Guesser.HasMultipleShotsPerMeeting && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 1 && dyingTarget != PlayerControl.LocalPlayer)
                    {
                        foreach (var x in __instance.playerStates)
                        {
                            if (x.TargetPlayerId == dyingTarget.PlayerId)
                            {
                                var shootButton = x.transform.FindChild("ShootButton");
                                if (shootButton != null) UnityObject.Destroy(shootButton.gameObject);
                            }
                        }
                    }
                    else
                    {
                        foreach (var x in __instance.playerStates)
                        {
                            var shootButton = x.transform.FindChild("ShootButton");
                            if (shootButton != null) UnityObject.Destroy(shootButton.gameObject);
                        }
                    }

                    using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.GuesserShoot))
                    {
                        sender.Write(PlayerControl.LocalPlayer.PlayerId);
                        sender.Write(dyingTarget.PlayerId);
                        sender.Write(focusedTarget.PlayerId);
                        sender.Write((byte)roleInfo.RoleType);
                        RPCProcedure.GuesserShoot(PlayerControl.LocalPlayer.PlayerId, dyingTarget.PlayerId, focusedTarget.PlayerId, (byte)roleInfo.RoleType);
                    }
                }
            }));
            buttonIndex++;
        }
        container.transform.localScale *= 0.75f;
    }

    public static void CreateGuesserButton(MeetingHud __instance)
    {
        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = UnityObject.Instantiate(template, playerVoteArea.transform);
            targetBox.name = "ShootButton";
            targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.TargetIcon;
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            var copiedIndex = i;
            button.OnClick.AddListener((Action)(() => GuesserOnClick(copiedIndex, __instance)));
        }
    }
}