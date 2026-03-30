namespace RebuildUs.Features;

internal static class SwapperModule
{
    private static bool[] _selections;
    private static SpriteRenderer[] _renderers;
    private static PassiveButton[] _swapperButtonList;
    private static TextMeshPro _meetingExtraButtonText;
    private static TextMeshPro _meetingExtraButtonLabel;
    private static PlayerVoteArea _swapped1;
    private static PlayerVoteArea _swapped2;

    public static void Reset()
    {
        _selections = null;
        _renderers = null;
        _swapperButtonList = null;
        _meetingExtraButtonText = null;
        _meetingExtraButtonLabel = null;
        _swapped1 = null;
        _swapped2 = null;
    }

    public static void HandleCalculateVotes(MeetingHud __instance, Dictionary<byte, int> dictionary)
    {
        if (!Swapper.Exists || Swapper.PlayerControl.IsDead())
        {
            return;
        }

        _swapped1 = null;
        _swapped2 = null;
        foreach (var playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1)
            {
                _swapped1 = playerVoteArea;
            }
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2)
            {
                _swapped2 = playerVoteArea;
            }
        }

        if (_swapped1 == null || _swapped2 == null)
        {
            return;
        }

        dictionary.TryAdd(_swapped1.TargetPlayerId, 0);
        dictionary.TryAdd(_swapped2.TargetPlayerId, 0);
        (dictionary[_swapped2.TargetPlayerId], dictionary[_swapped1.TargetPlayerId]) = (
            dictionary[_swapped1.TargetPlayerId], dictionary[_swapped2.TargetPlayerId]);
    }

    public static PlayerVoteArea GetRedirectedVoteArea(PlayerVoteArea original, bool doSwap)
    {
        if (!doSwap) return original;
        if (original.TargetPlayerId == _swapped1?.TargetPlayerId) return _swapped2;
        if (original.TargetPlayerId == _swapped2?.TargetPlayerId) return _swapped1;
        return original;
    }

    public static bool ShouldDoSwap(MeetingHud __instance)
    {
        _swapped1 = null;
        _swapped2 = null;
        foreach (var playerVoteArea in __instance.playerStates)
        {
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId1)
            {
                _swapped1 = playerVoteArea;
            }
            if (playerVoteArea.TargetPlayerId == Swapper.PlayerId2)
            {
                _swapped2 = playerVoteArea;
            }
        }

        return _swapped1 != null && _swapped2 != null && Swapper.Exists && Swapper.PlayerControl.IsAlive();
    }

    public static void PerformSwapAnimation(MeetingHud __instance)
    {
        if (_swapped1 == null || _swapped2 == null) return;

        __instance.StartCoroutine(
            Effects.Slide3D(_swapped1.transform, _swapped1.transform.localPosition, _swapped2.transform.localPosition, 1.5f));
        __instance.StartCoroutine(
            Effects.Slide3D(_swapped2.transform, _swapped2.transform.localPosition, _swapped1.transform.localPosition, 1.5f));
    }

    public static void OnVotingComplete()
    {
        Swapper.PlayerId1 = byte.MaxValue;
        Swapper.PlayerId2 = byte.MaxValue;
    }

    private static int GetSelectedCount()
    {
        if (_selections == null)
        {
            return 0;
        }
        var count = 0;
        foreach (var t in _selections)
        {
            if (t) count++;
        }
        return count;
    }

    public static void SwapperOnClick(int i, MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || i < 0 || i >= __instance.playerStates.Length) return;
        if (_selections == null || i >= _selections.Length) return;
        if (Swapper.NumSwaps <= 0) return;
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (__instance.playerStates[i].AmDead) return;

        var selectedCount = GetSelectedCount();
        var renderer = _renderers[i];

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
                if (_selections[i])
                {
                    renderer.color = Color.red;
                    _selections[i] = false;
                    _meetingExtraButtonLabel.text = Helpers.Cs(Color.red, Tr.Get(TrKey.SwapperConfirm));
                }
                break;
        }
    }

    public static void SwapperConfirm(MeetingHud __instance)
    {
        if (__instance == null || __instance.playerStates == null || __instance.playerStates.Length == 0) return;
        __instance.playerStates[0].Cancel();
        if (__instance.state == MeetingHud.VoteStates.Results) return;
        if (GetSelectedCount() != 2) return;
        if (Swapper.NumSwaps <= 0 || Swapper.PlayerId1 != byte.MaxValue) return;

        PlayerVoteArea firstPlayer = null;
        PlayerVoteArea secondPlayer = null;
        for (var a = 0; a < _selections.Length && a < __instance.playerStates.Length; a++)
        {
            if (_selections[a])
            {
                if (firstPlayer == null) firstPlayer = __instance.playerStates[a];
                else secondPlayer = __instance.playerStates[a];

                if (_renderers != null && a < _renderers.Length) _renderers[a].color = Color.green;
            }
            else
            {
                _renderers?[a]?.color = Color.gray;
            }
            _swapperButtonList[a]?.OnClick.RemoveAllListeners();
        }

        if (firstPlayer == null || secondPlayer == null) return;

        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SwapperSwap))
        {
            sender.Write(firstPlayer.TargetPlayerId);
            sender.Write(secondPlayer.TargetPlayerId);
            RPCProcedure.SwapperSwap(firstPlayer.TargetPlayerId, secondPlayer.TargetPlayerId);
        }

        _meetingExtraButtonLabel.text = Helpers.Cs(Color.green, Tr.Get(TrKey.SwapperSwapping));
        Swapper.RemainSwaps--;
        _meetingExtraButtonText.text = Tr.Get(TrKey.SwapperSwapsLeft, Swapper.RemainSwaps);
    }

    public static void CheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        if (__instance == null || __instance.playerStates == null || !Swapper.Exists || __instance.state == MeetingHud.VoteStates.Results) return;

        var reset = false;
        if (dyingPlayerId == Swapper.PlayerId1 || dyingPlayerId == Swapper.PlayerId2)
        {
            reset = true;
            Swapper.PlayerId1 = Swapper.PlayerId2 = byte.MaxValue;
        }

        if (!PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) || _selections == null) return;

        for (var i = 0; i < __instance.playerStates.Length && i < _selections.Length; i++)
        {
            reset = reset || _selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId;
            if (reset) break;
        }

        if (!reset) return;

        var stateCount = __instance.playerStates.Length;
        for (var i = 0; i < _selections.Length; i++)
        {
            _selections[i] = false;
            if (i >= stateCount) continue;
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.NiceSwapper) && Swapper.CanOnlySwapOthers) continue;
            _renderers[i].color = Color.red;
            Swapper.RemainSwaps++;
            var copyI = i;
            _swapperButtonList[i].OnClick.RemoveAllListeners();
            _swapperButtonList[i].OnClick.AddListener((Action)(() => SwapperOnClick(copyI, __instance)));
        }

        _meetingExtraButtonText.text = Tr.Get(TrKey.SwapperSwapsLeft, Swapper.RemainSwaps);
        _meetingExtraButtonLabel.text = Helpers.Cs(Color.red, Tr.Get(TrKey.SwapperConfirm));
    }

    public static void SetupButtons(MeetingHud __instance)
    {
        var addSwapperButtons = Swapper.Exists && PlayerControl.LocalPlayer.IsRole(RoleType.NiceSwapper) && Swapper.PlayerControl.IsAlive();
        if (!addSwapperButtons) return;

        _selections = new bool[__instance.playerStates.Length];
        _renderers = new SpriteRenderer[__instance.playerStates.Length];
        _swapperButtonList = new PassiveButton[__instance.playerStates.Length];

        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || Helpers.PlayerById(playerVoteArea.TargetPlayerId).IsRole(RoleType.NiceSwapper) && Swapper.CanOnlySwapOthers) continue;

            var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
            var checkbox = UnityObject.Instantiate(template, playerVoteArea.transform, true);
            checkbox.transform.position = template.transform.position;
            checkbox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            var renderer = checkbox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.SwapperCheck;
            renderer.color = Color.red;

            if (Swapper.RemainSwaps <= 0)
            {
                renderer.color = Color.gray;
            }

            var button = checkbox.GetComponent<PassiveButton>();
            _swapperButtonList[i] = button;
            button.OnClick.RemoveAllListeners();
            var copiedIndex = i;
            button.OnClick.AddListener((Action)(() => SwapperOnClick(copiedIndex, __instance)));

            _selections[i] = false;
            _renderers[i] = renderer;
        }

        Transform meetingUI = null;
        var transforms = UnityObject.FindObjectsOfType<Transform>();
        for (var i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == "PhoneUI")
            {
                meetingUI = transforms[i];
                break;
            }
        }

        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var textTemplate = __instance.playerStates[0].NameText;
        var meetingExtraButtonParent = new GameObject().transform;
        meetingExtraButtonParent.SetParent(meetingUI);
        var meetingExtraButton = UnityObject.Instantiate(buttonTemplate, meetingExtraButtonParent);

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

        var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
        passiveButton.OnClick.RemoveAllListeners();
        if (PlayerControl.LocalPlayer.IsAlive())
        {
            passiveButton.OnClick.AddListener((Action)(() => SwapperConfirm(__instance)));
        }

        meetingExtraButton.parent.gameObject.SetActive(false);
        __instance.StartCoroutine(Effects.Lerp(7.27f,
            new Action<float>(p =>
            {
                if (Mathf.Approximately(p, 1f))
                {
                    meetingExtraButton.parent.gameObject.SetActive(true);
                }
            })));
    }
}