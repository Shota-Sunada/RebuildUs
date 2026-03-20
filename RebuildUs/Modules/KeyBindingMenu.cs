namespace RebuildUs.Modules;

internal static class KeyBindingMenu
{
    private static GameObject _popUp;
    private static GameObject _waitingPopUp;
    private static TextMeshPro _waitingText;
    private static readonly List<ToggleButtonBehaviour> ModButtons = [];
    private static ToggleButtonBehaviour _buttonPrefab;

    private static KeyBindingManager.RebuildUsInput _activeInput;
    private static readonly KeyCode[] AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

    private static bool _waitingForKey;

    internal static void Start(OptionsMenuBehaviour __instance)
    {
        if (__instance.name == "KeyBindingMenu")
        {
            return;
        }
        if (!__instance.CensorChatButton)
        {
            return;
        }

        if (!_popUp)
        {
            CreateCustom(__instance);
        }

        if (!_buttonPrefab)
        {
            _buttonPrefab = UnityObject.Instantiate(__instance.CensorChatButton);
            UnityObject.DontDestroyOnLoad(_buttonPrefab);
            _buttonPrefab.name = "KeyBindPrefab";

            var toggle = _buttonPrefab.GetComponent<ToggleButtonBehaviour>();
            if (toggle)
            {
                toggle.enabled = false;
            }

            var collider = _buttonPrefab.GetComponent<BoxCollider2D>();
            if (collider)
            {
                collider.size = new(2.5f, 0.7f);
            }

            _buttonPrefab.transform.localScale = new(1f, 2f, 1f);
            _buttonPrefab.Text.transform.localScale = new(1f, 0.5f, 1f);

            _buttonPrefab.Text.fontSizeMax = _buttonPrefab.Text.fontSizeMin = 1.6f;
            _buttonPrefab.Text.alignment = TextAlignmentOptions.Center;

            _buttonPrefab.gameObject.SetActive(false);
        }

        InitializeMenuButton(__instance);
    }

    private static void CreateCustom(Component prefab)
    {
        ModButtons.Clear();
        _popUp = UnityObject.Instantiate(prefab.gameObject);
        _popUp.name = "KeyBindingMenu";
        UnityObject.DontDestroyOnLoad(_popUp);
        var transform = _popUp.transform;

        var omb = _popUp.GetComponent<OptionsMenuBehaviour>();
        if (omb)
        {
            omb.Tabs = Array.Empty<TabGroup>();
            omb.ControllerSelectable ??= new();
            omb.ControllerSelectable.Clear();
            omb.IgnoreControllerSelection ??= new();
            omb.IgnoreControllerSelection.Clear();
        }

        // Ensure it's in front of everything
        var pos = transform.localPosition;
        pos.z = -900f;
        transform.localPosition = pos;

        // Setup Title
        GameObject go = new("TitleTextKeyBinds");
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = Tr.Get(TrKey.KeyBindings);
        tmp.transform.SetParent(_popUp.transform);
        tmp.transform.localPosition = new(0, 2.2f, -1f);

        // Setup Close Button
        var closeButton = _popUp.transform.Find("CloseButton")?.GetComponent<PassiveButton>();
        if (closeButton)
        {
            closeButton.OnClick = new();
            closeButton.OnClick.AddListener((Action)(() =>
            {
                Close();
            }));

            if (omb)
            {
                omb.BackButton = closeButton.GetComponent<UiElement>();
            }
        }

        // Setup Waiting PopUp
        _waitingPopUp = new("KeyBindingWaitingPopUp");
        UnityObject.DontDestroyOnLoad(_waitingPopUp);

        var bgObj = _popUp.transform.Find("Background")?.gameObject;
        if (bgObj)
        {
            var bg = UnityObject.Instantiate(bgObj, _waitingPopUp.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new(0.7f, 0.4f, 1f);
        }

        GameObject wgo = new("WaitingText");
        _waitingText = wgo.AddComponent<TextMeshPro>();
        _waitingText.fontSize = 2.5f;
        _waitingText.alignment = TextAlignmentOptions.Center;
        _waitingText.text = Tr.Get(TrKey.PressAnyKey);
        _waitingText.transform.SetParent(_waitingPopUp.transform);
        _waitingText.transform.localPosition = new(0, 0, -1f);

        _waitingPopUp.SetActive(false);

        // Remove unnecessary components
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var obj = transform.GetChild(i).gameObject;
            if (obj.name is not "Background" and not "CloseButton")
            {
                UnityObject.Destroy(obj);
            }
        }

        _popUp.SetActive(false);
    }

    private static void Close()
    {
        if (!_popUp)
        {
            return;
        }

        _activeInput = null;
        _popUp.SetActive(false);
        _popUp.transform.SetParent(null);
        UnityObject.DontDestroyOnLoad(_popUp);

        if (_waitingPopUp)
        {
            _waitingPopUp.SetActive(false);
            _waitingPopUp.transform.SetParent(null);
            UnityObject.DontDestroyOnLoad(_waitingPopUp);
        }

        if (ControllerManager.Instance)
        {
            ControllerManager.Instance.CloseOverlayMenu("OptionsMenu");
        }
    }

    private static void InitializeMenuButton(OptionsMenuBehaviour __instance)
    {
        // Add a button to open this menu in the main options menu
        if (__instance.CensorChatButton.transform.parent.Find("KeyBindingMenuButton"))
        {
            return;
        }

        var openButton = UnityObject.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
        openButton.name = "KeyBindingMenuButton";

        // Position it symmetrical to the Client Options button (which is at down * 1.0)
        var pos = __instance.CensorChatButton.transform.localPosition;
        openButton.transform.localPosition = new(1.3f, pos.y - 1.0f, pos.z);

        // Adjust size to fit in the space
        var collider = openButton.GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.size = new(1.5f, 0.7f);
        }

        openButton.gameObject.SetActive(true);
        openButton.Text.text = "Keys";
        openButton.Text.fontSizeMax = openButton.Text.fontSizeMin = 2.0f;

        var passiveButton = openButton.GetComponent<PassiveButton>();
        passiveButton.OnClick = new();
        passiveButton.OnClick.AddListener((Action)(() =>
        {
            if (!_popUp)
            {
                CreateCustom(__instance);
            }

            _popUp.transform.SetParent(__instance.transform.parent);
            _popUp.transform.localPosition = new(0, 0, -900f);

            SetUpOptions();
            _popUp.SetActive(true);

            // Re-grab controller buttons for navigation
            var omb = _popUp.GetComponent<OptionsMenuBehaviour>();
            if (omb)
            {
                ControllerManager.Instance.OpenOverlayMenu("OptionsMenu", omb.BackButton, omb.DefaultButtonSelected, omb.ControllerSelectable);
            }

            if (__instance.transform.parent && __instance.transform.parent == FastDestroyableSingleton<HudManager>.Instance?.transform)
            {
                __instance.Close();
            }
        }));
    }

    internal static void Update()
    {
        if (_activeInput != null)
        {
            if (_waitingPopUp)
            {
                if (!_waitingPopUp.activeSelf)
                {
                    _waitingPopUp.SetActive(true);
                }
                _waitingPopUp.transform.SetParent(_popUp.transform.parent);
                _waitingPopUp.transform.localPosition = new(0, 0, -950f);

                if (_waitingText && _activeInput != null)
                {
                    _waitingText.text = new StringBuilder(Tr.GetDynamic($"{_activeInput.Identifier}"))
                                        .Append("\n")
                                        .Append(Tr.Get(TrKey.PressAnyKey))
                                        .ToString();
                }
            }

            if (!_waitingForKey)
            {
                if (!Input.anyKey)
                {
                    _waitingForKey = true;
                }
                return;
            }

            if (!Input.anyKeyDown)
            {
                return;
            }
            foreach (var key in AllKeyCodes)
            {
                if (!Input.GetKeyDown(key))
                {
                    continue;
                }
                if (key != KeyCode.Escape)
                {
                    _activeInput.ChangeKey(key);
                }

                _activeInput = null;
                _waitingForKey = false;
                SetUpOptions();
                break;
            }

            return;
        }

        if (_waitingPopUp && _waitingPopUp.activeSelf)
        {
            _waitingPopUp.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Close();
        }
    }

    private static void SetUpOptions()
    {
        if (!_popUp)
        {
            return;
        }

        var omb = _popUp.GetComponent<OptionsMenuBehaviour>();

        if (ModButtons.Count > 0 && ModButtons.Count == KeyBindingManager.AllInputs.Count && ModButtons[0] != null)
        {
            for (var i = 0; i < KeyBindingManager.AllInputs.Count; i++)
            {
                var input = KeyBindingManager.AllInputs[i];
                var button = ModButtons[i];

                var keyText = _activeInput == input ? Tr.Get(TrKey.PressAnyKey) : input.Key.ToString();
                button.Text.text = new StringBuilder(Tr.GetDynamic($"{input.Identifier}")).Append(": ").Append(keyText).ToString();
            }

            return;
        }

        if (omb)
        {
            omb.ControllerSelectable ??= new();
            omb.ControllerSelectable.Clear();
        }

        foreach (var button in ModButtons)
        {
            if (button != null)
            {
                UnityObject.Destroy(button.gameObject);
            }
        }

        ModButtons.Clear();

        const float startY = 1.4f;
        const float spacing = 1.0f;
        const float xOffset = 1.4f;

        for (var i = 0; i < KeyBindingManager.AllInputs.Count; i++)
        {
            var input = KeyBindingManager.AllInputs[i];
            var button = UnityObject.Instantiate(_buttonPrefab, _popUp.transform);

            var x = i % 2 == 0 ? -xOffset : xOffset;
            var y = startY - i / 2 * spacing;

            button.transform.localPosition = new(x, y, -1f);
            button.gameObject.SetActive(true);

            var keyText = _activeInput == input ? Tr.Get(TrKey.PressAnyKey) : input.Key.ToString();
            button.Text.text = new StringBuilder(Tr.GetDynamic($"{input.Identifier}")).Append(": ").Append(keyText).ToString();

            var passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                _activeInput = input;
                SetUpOptions();
            }));

            if (omb)
            {
                var uiElement = button.GetComponent<UiElement>();
                if (uiElement)
                {
                    omb.ControllerSelectable.Add(uiElement);
                }
                if (i == 0)
                {
                    omb.DefaultButtonSelected = uiElement;
                }
            }

            ModButtons.Add(button);
        }
    }
}