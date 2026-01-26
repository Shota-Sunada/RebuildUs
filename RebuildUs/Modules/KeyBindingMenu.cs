using UnityEngine.UI;

namespace RebuildUs.Modules;

public static class KeyBindingMenu
{
    private static GameObject PopUp;
    private static TextMeshPro TitleText;
    private static GameObject WaitingPopUp;
    private static TextMeshPro WaitingText;
    private static readonly List<ToggleButtonBehaviour> ModButtons = [];
    private static ToggleButtonBehaviour ButtonPrefab;

    private static KeyBindingManager.RebuildUsInput activeInput = null;
    private static readonly KeyCode[] AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

    public static void Start(OptionsMenuBehaviour __instance)
    {
        if (__instance.name == "KeyBindingMenu") return;
        if (!__instance.CensorChatButton) return;

        if (!PopUp)
        {
            CreateCustom(__instance);
        }

        if (!ButtonPrefab)
        {
            ButtonPrefab = UnityEngine.Object.Instantiate(__instance.CensorChatButton);
            UnityEngine.Object.DontDestroyOnLoad(ButtonPrefab);
            ButtonPrefab.name = "KeyBindPrefab";

            var toggle = ButtonPrefab.GetComponent<ToggleButtonBehaviour>();
            if (toggle) toggle.enabled = false;

            var collider = ButtonPrefab.GetComponent<BoxCollider2D>();
            if (collider) collider.size = new Vector2(2.5f, 0.7f);

            ButtonPrefab.transform.localScale = new Vector3(1f, 2f, 1f);
            ButtonPrefab.Text.transform.localScale = new Vector3(1f, 0.5f, 1f);

            ButtonPrefab.Text.fontSizeMax = ButtonPrefab.Text.fontSizeMin = 1.6f;
            ButtonPrefab.Text.alignment = TextAlignmentOptions.Center;

            ButtonPrefab.gameObject.SetActive(false);
        }

        InitializeMenuButton(__instance);
    }

    private static void CreateCustom(OptionsMenuBehaviour prefab)
    {
        PopUp = UnityEngine.Object.Instantiate(prefab.gameObject);
        PopUp.name = "KeyBindingMenu";
        UnityEngine.Object.DontDestroyOnLoad(PopUp);
        var transform = PopUp.transform;

        var omb = PopUp.GetComponent<OptionsMenuBehaviour>();
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
        var go = new GameObject("TitleTextKeyBinds");
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = Tr.Get("Hud.KeyBindings");
        tmp.transform.SetParent(PopUp.transform);
        tmp.transform.localPosition = new Vector3(0, 2.2f, -1f);
        TitleText = tmp;

        // Setup Close Button
        var closeButton = PopUp.transform.Find("CloseButton")?.GetComponent<PassiveButton>();
        if (closeButton)
        {
            closeButton.OnClick = new Button.ButtonClickedEvent();
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
        WaitingPopUp = new GameObject("KeyBindingWaitingPopUp");
        UnityEngine.Object.DontDestroyOnLoad(WaitingPopUp);

        var bgObj = PopUp.transform.Find("Background")?.gameObject;
        if (bgObj)
        {
            var bg = UnityEngine.Object.Instantiate(bgObj, WaitingPopUp.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(0.7f, 0.4f, 1f);
        }

        var wgo = new GameObject("WaitingText");
        WaitingText = wgo.AddComponent<TextMeshPro>();
        WaitingText.fontSize = 2.5f;
        WaitingText.alignment = TextAlignmentOptions.Center;
        WaitingText.text = Tr.Get("Hud.PressAnyKey");
        WaitingText.transform.SetParent(WaitingPopUp.transform);
        WaitingText.transform.localPosition = new Vector3(0, 0, -1f);

        WaitingPopUp.SetActive(false);

        // Remove unnecessary components
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var obj = transform.GetChild(i).gameObject;
            if (obj.name is not "Background" and not "CloseButton")
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        PopUp.SetActive(false);
    }

    public static void Close()
    {
        if (!PopUp) return;

        activeInput = null;
        PopUp.SetActive(false);
        PopUp.transform.SetParent(null);
        UnityEngine.Object.DontDestroyOnLoad(PopUp);

        if (WaitingPopUp)
        {
            WaitingPopUp.SetActive(false);
            WaitingPopUp.transform.SetParent(null);
            UnityEngine.Object.DontDestroyOnLoad(WaitingPopUp);
        }

        if (ControllerManager.Instance)
        {
            ControllerManager.Instance.CloseOverlayMenu("OptionsMenu");
        }
    }

    private static void InitializeMenuButton(OptionsMenuBehaviour __instance)
    {
        // Add a button to open this menu in the main options menu
        if (__instance.CensorChatButton.transform.parent.Find("KeyBindingMenuButton")) return;

        var openButton = UnityEngine.Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
        openButton.name = "KeyBindingMenuButton";

        // Position it symmetrical to the Client Options button (which is at down * 1.0)
        var pos = __instance.CensorChatButton.transform.localPosition;
        openButton.transform.localPosition = new Vector3(-pos.x - 0.5f, pos.y - 0.5f, pos.z);

        // Adjust size to fit in the space
        var collider = openButton.GetComponent<BoxCollider2D>();
        if (collider) collider.size = new Vector2(1.5f, 0.7f);

        openButton.gameObject.SetActive(true);
        openButton.Text.text = "Keys";
        openButton.Text.fontSizeMax = openButton.Text.fontSizeMin = 2.0f;

        var passiveButton = openButton.GetComponent<PassiveButton>();
        passiveButton.OnClick = new Button.ButtonClickedEvent();
        passiveButton.OnClick.AddListener((Action)(() =>
        {
            if (!PopUp)
            {
                CreateCustom(__instance);
            }

            PopUp.transform.SetParent(__instance.transform.parent);
            PopUp.transform.localPosition = new Vector3(0, 0, -900f);

            SetUpOptions();
            PopUp.SetActive(true);

            // Re-grab controller buttons for navigation
            var omb = PopUp.GetComponent<OptionsMenuBehaviour>();
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

    private static bool waitingForKey = false;

    public static void Update()
    {
        if (activeInput != null)
        {
            if (WaitingPopUp)
            {
                if (!WaitingPopUp.activeSelf) WaitingPopUp.SetActive(true);
                WaitingPopUp.transform.SetParent(PopUp.transform.parent);
                WaitingPopUp.transform.localPosition = new Vector3(0, 0, -950f);

                if (WaitingText && activeInput != null)
                {
                    WaitingText.text = new StringBuilder(Tr.Get($"Hud.{activeInput.Identifier}"))
                        .Append("\n")
                        .Append(Tr.Get("Hud.PressAnyKey")).ToString();
                }
            }

            if (!waitingForKey)
            {
                if (!Input.anyKey) waitingForKey = true;
                return;
            }

            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in AllKeyCodes)
                {
                    if (Input.GetKeyDown(key))
                    {
                        if (key != KeyCode.Escape)
                        {
                            activeInput.ChangeKey(key);
                        }
                        activeInput = null;
                        waitingForKey = false;
                        SetUpOptions();
                        break;
                    }
                }
            }
            return;
        }

        if (WaitingPopUp && WaitingPopUp.activeSelf) WaitingPopUp.SetActive(false);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Close();
        }
    }

    private static void SetUpOptions()
    {
        if (!PopUp) return;

        var omb = PopUp.GetComponent<OptionsMenuBehaviour>();

        if (ModButtons.Count > 0 && ModButtons.Count == KeyBindingManager.AllInputs.Count)
        {
            for (int i = 0; i < KeyBindingManager.AllInputs.Count; i++)
            {
                var input = KeyBindingManager.AllInputs[i];
                var button = ModButtons[i];

                string keyText = activeInput == input ? Tr.Get("Hud.PressAnyKey") : input.Key.ToString();
                button.Text.text = new StringBuilder(Tr.Get($"Hud.{input.Identifier}")).Append(": ").Append(keyText).ToString();
            }
            return;
        }

        if (omb)
        {
            if (omb.ControllerSelectable == null) omb.ControllerSelectable = new();
            omb.ControllerSelectable.Clear();
        }

        foreach (var button in ModButtons)
        {
            if (button != null) UnityEngine.Object.Destroy(button.gameObject);
        }
        ModButtons.Clear();

        float startY = 1.4f;
        float spacing = 1.0f;
        float xOffset = 1.4f;

        for (int i = 0; i < KeyBindingManager.AllInputs.Count; i++)
        {
            var input = KeyBindingManager.AllInputs[i];
            var button = UnityEngine.Object.Instantiate(ButtonPrefab, PopUp.transform);

            float x = (i % 2 == 0) ? -xOffset : xOffset;
            float y = startY - ((i / 2) * spacing);

            button.transform.localPosition = new Vector3(x, y, -1f);
            button.gameObject.SetActive(true);

            string keyText = activeInput == input ? Tr.Get("Hud.PressAnyKey") : input.Key.ToString();
            button.Text.text = new StringBuilder(Tr.Get($"Hud.{input.Identifier}")).Append(": ").Append(keyText).ToString();

            var passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                activeInput = input;
                SetUpOptions();
            }));

            if (omb)
            {
                var uiElement = button.GetComponent<UiElement>();
                if (uiElement) omb.ControllerSelectable.Add(uiElement);
                if (i == 0) omb.DefaultButtonSelected = uiElement;
            }

            ModButtons.Add(button);
        }
    }
}
