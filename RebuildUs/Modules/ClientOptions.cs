namespace RebuildUs.Modules;

internal static class ClientOptions
{
    private static readonly SelectionBehaviour[] AllOptions =
    [
        new("GhostsSeeTasks", () => MapSettings.GhostsSeeInformation = RebuildUs.GhostsSeeInformation.Value = !RebuildUs.GhostsSeeInformation.Value, RebuildUs.GhostsSeeInformation.Value),
        new("GhostsSeeVotes", () => MapSettings.GhostsSeeVotes = RebuildUs.GhostsSeeVotes.Value = !RebuildUs.GhostsSeeVotes.Value, RebuildUs.GhostsSeeVotes.Value),
        new("GhostsSeeRoles", () => MapSettings.GhostsSeeRoles = RebuildUs.GhostsSeeRoles.Value = !RebuildUs.GhostsSeeRoles.Value, RebuildUs.GhostsSeeRoles.Value),
        new("ShowRoleSummary", () => MapSettings.ShowRoleSummary = RebuildUs.ShowRoleSummary.Value = !RebuildUs.ShowRoleSummary.Value, RebuildUs.ShowRoleSummary.Value),
        new("ShowLighterDarker", () => MapSettings.ShowLighterDarker = RebuildUs.ShowLighterDarker.Value = !RebuildUs.ShowLighterDarker.Value, RebuildUs.ShowLighterDarker.Value),
        new("BetterSabotageMap", () => MapSettings.BetterSabotageMap = RebuildUs.BetterSabotageMap.Value = !RebuildUs.BetterSabotageMap.Value, RebuildUs.BetterSabotageMap.Value),
        new("ForceNormalSabotageMap", () => MapSettings.ForceNormalSabotageMap = RebuildUs.ForceNormalSabotageMap.Value = !RebuildUs.ForceNormalSabotageMap.Value, RebuildUs.ForceNormalSabotageMap.Value),
        new("TransparentMap", () => MapSettings.TransparentMap = RebuildUs.TransparentMap.Value = !RebuildUs.TransparentMap.Value, RebuildUs.TransparentMap.Value),
        new("HideFakeTasks", () => MapSettings.HideFakeTasks = RebuildUs.HideFakeTasks.Value = !RebuildUs.HideFakeTasks.Value, RebuildUs.HideFakeTasks.Value),
    ];

    private static GameObject _popUp;
    private static TextMeshPro _titleText;

    private static List<ToggleButtonBehaviour> _modButtons = [];

    private static ToggleButtonBehaviour _buttonPrefab;
    private static int _page = 1;

    internal static void Start(MainMenuManager __instance)
    {
        // Prefab for the title
        GameObject go = new("TitleTextRU");
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.transform.localPosition += Vector3.left * 0.2f;
        _titleText = UnityObject.Instantiate(tmp);
        _titleText.gameObject.SetActive(false);
        UnityObject.DontDestroyOnLoad(_titleText);
    }

    internal static void Start(OptionsMenuBehaviour __instance)
    {
        if (!__instance.CensorChatButton) return;

        if (!_popUp) CreateCustom(__instance);

        if (!_buttonPrefab)
        {
            _buttonPrefab = UnityObject.Instantiate(__instance.CensorChatButton);
            UnityObject.DontDestroyOnLoad(_buttonPrefab);
            _buttonPrefab.name = "CensorChatPrefab";
            _buttonPrefab.gameObject.SetActive(false);
        }

        SetUpOptions();
        InitializeMoreButton(__instance);
    }

    private static void CreateCustom(Component prefab)
    {
        _popUp = UnityObject.Instantiate(prefab.gameObject);
        UnityObject.DontDestroyOnLoad(_popUp);
        Transform transform = _popUp.transform;
        Vector3 pos = transform.localPosition;
        pos.z = -810f;
        transform.localPosition = pos;

        UnityObject.Destroy(_popUp.GetComponent<OptionsMenuBehaviour>());
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            if (obj.name is not "Background" and not "CloseButton") UnityObject.Destroy(obj);
        }

        _popUp.SetActive(false);
    }

    private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
    {
        ToggleButtonBehaviour moreOptions = UnityObject.Instantiate(_buttonPrefab, __instance.CensorChatButton.transform.parent);
        moreOptions.transform.localPosition = __instance.CensorChatButton.transform.localPosition + (Vector3.down * 1.0f);
        moreOptions.gameObject.SetActive(true);
        moreOptions.Text.text = Tr.Get(TrKey.ModOptionsText);
        PassiveButton moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
        moreOptionsButton.OnClick = new();
        moreOptionsButton.OnClick.AddListener((Action)(() =>
        {
            bool closeUnderlying = false;
            if (!_popUp) return;

            if (__instance.transform.parent && __instance.transform.parent == FastDestroyableSingleton<HudManager>.Instance.transform)
            {
                _popUp.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                _popUp.transform.localPosition = new(0, 0, -800f);
                closeUnderlying = true;
            }
            else
            {
                _popUp.transform.SetParent(null);
                UnityObject.DontDestroyOnLoad(_popUp);
            }

            CheckSetTitle();
            RefreshOpen();

            if (closeUnderlying) __instance.Close();
        }));

        GameObject returnToGameButton = GameObject.Find("ReturnToGameButton");
        GameObject leaveGameButton = GameObject.Find("LeaveGameButton");

        if (!returnToGameButton || !leaveGameButton) return;
        returnToGameButton.transform.localPosition += Vector3.left * 1.3f;
        leaveGameButton.transform.localPosition += Vector3.right * 1.3f;
        leaveGameButton.transform.localPosition = new(leaveGameButton.transform.localPosition.x, returnToGameButton.transform.localPosition.y, leaveGameButton.transform.localPosition.z);
    }

    private static void RefreshOpen()
    {
        _popUp.gameObject.SetActive(false);
        _popUp.gameObject.SetActive(true);
        SetUpOptions();
    }

    private static void CheckSetTitle()
    {
        if (!_popUp || _popUp.GetComponentInChildren<TextMeshPro>() || !_titleText) return;

        TextMeshPro title = UnityObject.Instantiate(_titleText, _popUp.transform);
        title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
        title.gameObject.SetActive(true);
        title.text = Tr.Get(TrKey.MoreOptionsText);
        title.name = "TitleText";
    }

    private static void SetUpOptions()
    {
        if (_popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

        foreach (ToggleButtonBehaviour button in _modButtons)
        {
            if (button != null)
                UnityObject.Destroy(button.gameObject);
        }

        _modButtons = [];
        int length = _page * 10 < AllOptions.Length ? _page * 10 : AllOptions.Length;

        for (int i = 0; i + ((_page - 1) * 10) < length; i++)
        {
            SelectionBehaviour info = AllOptions[i + ((_page - 1) * 10)];

            ToggleButtonBehaviour button = UnityObject.Instantiate(_buttonPrefab, _popUp.transform);
            Vector3 pos = new(i % 2 == 0 ? -1.17f : 1.17f, 1.3f - ((i / 2) * 0.8f), -.5f);

            Transform transform = button.transform;
            transform.localPosition = pos;

            button.onState = info.DefaultValue;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = Tr.GetDynamic(info.Title);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityObject.Instantiate(_titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);

            button.name = info.Title.Replace(" ", "") + "Toggle";
            button.gameObject.SetActive(true);

            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            BoxCollider2D colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new(2.2f, .7f);

            passiveButton.OnClick = new();
            passiveButton.OnMouseOut = new();
            passiveButton.OnMouseOver = new();

            passiveButton.OnClick.AddListener((Action)(() =>
            {
                button.onState = info.OnClick();
                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
            }));

            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));

            foreach (SpriteRenderer spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new(2.2f, .7f);

            _modButtons.Add(button);
        }

        // ページ移動ボタンを追加
        if (_page * 10 < AllOptions.Length)
        {
            ToggleButtonBehaviour button = UnityObject.Instantiate(_buttonPrefab, _popUp.transform);
            Vector3 pos = new(1.2f, -2.5f, -0.5f);
            Transform transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get(TrKey.Next);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityObject.Instantiate(_titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);
            button.gameObject.SetActive(true);
            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            BoxCollider2D colliderButton = button.GetComponent<BoxCollider2D>();
            colliderButton.size = new(2.2f, .7f);
            passiveButton.OnClick = new();
            passiveButton.OnMouseOut = new();
            passiveButton.OnMouseOver = new();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                _page += 1;
                SetUpOptions();
            }));
            _modButtons.Add(button);
        }

        if (_page > 1)
        {
            ToggleButtonBehaviour button = UnityObject.Instantiate(_buttonPrefab, _popUp.transform);
            Vector3 pos = new(-1.2f, -2.5f, -0.5f);
            Transform transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get(TrKey.Previous);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityObject.Instantiate(_titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);
            button.gameObject.SetActive(true);
            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            BoxCollider2D colliderButton = button.GetComponent<BoxCollider2D>();
            colliderButton.size = new(2.2f, .7f);
            passiveButton.OnClick = new();
            passiveButton.OnMouseOut = new();
            passiveButton.OnMouseOver = new();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                _page -= 1;
                SetUpOptions();
            }));
            _modButtons.Add(button);
        }
    }

    private sealed class SelectionBehaviour
    {
        internal readonly bool DefaultValue;
        internal readonly Func<bool> OnClick;
        internal readonly string Title;

        internal SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
        {
            Title = title;
            OnClick = onClick;
            DefaultValue = defaultValue;
        }
    }
}