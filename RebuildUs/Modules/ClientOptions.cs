using UnityEngine.UI;

namespace RebuildUs.Modules;

public static class ClientOptions
{
    private static readonly SelectionBehaviour[] AllOptions = [
        new SelectionBehaviour("GhostsSeeTasks", () => ModMapOptions.GhostsSeeInformation = RebuildUs.GhostsSeeInformation.Value = !RebuildUs.GhostsSeeInformation.Value, RebuildUs.GhostsSeeInformation.Value),
        new SelectionBehaviour("GhostsSeeVotes", () => ModMapOptions.GhostsSeeVotes = RebuildUs.GhostsSeeVotes.Value = !RebuildUs.GhostsSeeVotes.Value, RebuildUs.GhostsSeeVotes.Value),
        new SelectionBehaviour("GhostsSeeRoles", () => ModMapOptions.GhostsSeeRoles = RebuildUs.GhostsSeeRoles.Value = !RebuildUs.GhostsSeeRoles.Value, RebuildUs.GhostsSeeRoles.Value),
        new SelectionBehaviour("ShowRoleSummary", () => ModMapOptions.ShowRoleSummary = RebuildUs.ShowRoleSummary.Value = !RebuildUs.ShowRoleSummary.Value, RebuildUs.ShowRoleSummary.Value),
        new SelectionBehaviour("ShowLighterDarker", () => ModMapOptions.ShowLighterDarker = RebuildUs.ShowLighterDarker.Value = !RebuildUs.ShowLighterDarker.Value, RebuildUs.ShowLighterDarker.Value),
        new SelectionBehaviour("BetterSabotageMap", () => ModMapOptions.BetterSabotageMap = RebuildUs.BetterSabotageMap.Value = !RebuildUs.BetterSabotageMap.Value, RebuildUs.BetterSabotageMap.Value),
        new SelectionBehaviour("ForceNormalSabotageMap", () => ModMapOptions.ForceNormalSabotageMap = RebuildUs.ForceNormalSabotageMap.Value = !RebuildUs.ForceNormalSabotageMap.Value, RebuildUs.ForceNormalSabotageMap.Value),
        new SelectionBehaviour("TransparentMap", () => ModMapOptions.TransparentMap = RebuildUs.TransparentMap.Value = !RebuildUs.TransparentMap.Value, RebuildUs.TransparentMap.Value),
        new SelectionBehaviour("HideFakeTasks", () => ModMapOptions.HideFakeTasks = RebuildUs.HideFakeTasks.Value = !RebuildUs.HideFakeTasks.Value, RebuildUs.HideFakeTasks.Value),
    ];

    private static GameObject PopUp;
    private static TextMeshPro TitleText;

    private static List<ToggleButtonBehaviour> ModButtons = [];

    private static ToggleButtonBehaviour ButtonPrefab;
    private static int Page = 1;

    public static void Start(MainMenuManager __instance)
    {
        // Prefab for the title
        var go = new GameObject("TitleTextRU");
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.transform.localPosition += Vector3.left * 0.2f;
        TitleText = UnityEngine.Object.Instantiate(tmp);
        TitleText.gameObject.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(TitleText);
    }

    public static void Start(OptionsMenuBehaviour __instance)
    {
        if (!__instance.CensorChatButton) return;

        if (!PopUp)
        {
            CreateCustom(__instance);
        }

        if (!ButtonPrefab)
        {
            ButtonPrefab = UnityEngine.Object.Instantiate(__instance.CensorChatButton);
            UnityEngine.Object.DontDestroyOnLoad(ButtonPrefab);
            ButtonPrefab.name = "CensorChatPrefab";
            ButtonPrefab.gameObject.SetActive(false);
        }

        SetUpOptions();
        InitializeMoreButton(__instance);
    }

    private static void CreateCustom(OptionsMenuBehaviour prefab)
    {
        PopUp = UnityEngine.Object.Instantiate(prefab.gameObject);
        UnityEngine.Object.DontDestroyOnLoad(PopUp);
        var transform = PopUp.transform;
        var pos = transform.localPosition;
        pos.z = -810f;
        transform.localPosition = pos;

        UnityEngine.Object.Destroy(PopUp.GetComponent<OptionsMenuBehaviour>());
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

    private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
    {
        var moreOptions = UnityEngine.Object.Instantiate(ButtonPrefab, __instance.CensorChatButton.transform.parent);
        moreOptions.transform.localPosition = __instance.CensorChatButton.transform.localPosition + Vector3.down * 1.0f;
        moreOptions.gameObject.SetActive(true);
        moreOptions.Text.text = Tr.Get(TranslateKey.ModOptionsText);
        var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
        moreOptionsButton.OnClick = new Button.ButtonClickedEvent();
        moreOptionsButton.OnClick.AddListener((Action)(() =>
        {
            bool closeUnderlying = false;
            if (!PopUp) return;

            if (__instance.transform.parent && __instance.transform.parent == FastDestroyableSingleton<HudManager>.Instance.transform)
            {
                PopUp.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                PopUp.transform.localPosition = new Vector3(0, 0, -800f);
                closeUnderlying = true;
            }
            else
            {
                PopUp.transform.SetParent(null);
                UnityEngine.Object.DontDestroyOnLoad(PopUp);
            }

            CheckSetTitle();
            RefreshOpen();

            if (closeUnderlying) __instance.Close();
        }));

        var returnToGameButton = GameObject.Find("ReturnToGameButton");
        var leaveGameButton = GameObject.Find("LeaveGameButton");

        if (returnToGameButton && leaveGameButton)
        {
            returnToGameButton.transform.localPosition += Vector3.left * 1.3f;
            leaveGameButton.transform.localPosition += Vector3.right * 1.3f;
            leaveGameButton.transform.localPosition = new(leaveGameButton.transform.localPosition.x, returnToGameButton.transform.localPosition.y, leaveGameButton.transform.localPosition.z);
        }
    }

    private static void RefreshOpen()
    {
        PopUp.gameObject.SetActive(false);
        PopUp.gameObject.SetActive(true);
        SetUpOptions();
    }

    private static void CheckSetTitle()
    {
        if (!PopUp || PopUp.GetComponentInChildren<TextMeshPro>() || !TitleText) return;

        var title = UnityEngine.Object.Instantiate(TitleText, PopUp.transform);
        title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
        title.gameObject.SetActive(true);
        title.text = Tr.Get(TranslateKey.MoreOptionsText);
        title.name = "TitleText";
    }

    private static void SetUpOptions()
    {
        if (PopUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

        foreach (var button in ModButtons)
        {
            if (button != null) UnityEngine.Object.Destroy(button.gameObject);
        }

        ModButtons = [];
        int length = (Page * 10) < AllOptions.Length ? Page * 10 : AllOptions.Length;

        for (var i = 0; i + ((Page - 1) * 10) < length; i++)
        {
            var info = AllOptions[i + ((Page - 1) * 10)];

            var button = UnityEngine.Object.Instantiate(ButtonPrefab, PopUp.transform);
            var pos = new Vector3(i % 2 == 0 ? -1.17f : 1.17f, 1.3f - i / 2 * 0.8f, -.5f);

            var transform = button.transform;
            transform.localPosition = pos;

            button.onState = info.DefaultValue;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = Tr.GetDynamic(info.Title);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(TitleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = info.Title.Replace(" ", "") + "Toggle";
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(2.2f, .7f);

            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEngine.Events.UnityEvent();
            passiveButton.OnMouseOver = new UnityEngine.Events.UnityEvent();

            passiveButton.OnClick.AddListener((Action)(() =>
            {
                button.onState = info.OnClick();
                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
            }));

            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));

            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(2.2f, .7f);

            ModButtons.Add(button);
        }
        // ページ移動ボタンを追加
        if (Page * 10 < AllOptions.Length)
        {
            var button = UnityEngine.Object.Instantiate(ButtonPrefab, PopUp.transform);
            var pos = new Vector3(1.2f, -2.5f, -0.5f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get(TranslateKey.Next);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(TitleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
            button.gameObject.SetActive(true);
            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();
            colliderButton.size = new Vector2(2.2f, .7f);
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEngine.Events.UnityEvent();
            passiveButton.OnMouseOver = new UnityEngine.Events.UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                Page += 1;
                SetUpOptions();
            }));
            ModButtons.Add(button);
        }
        if (Page > 1)
        {
            var button = UnityEngine.Object.Instantiate(ButtonPrefab, PopUp.transform);
            var pos = new Vector3(-1.2f, -2.5f, -0.5f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get(TranslateKey.Previous);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(TitleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);
            button.gameObject.SetActive(true);
            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();
            colliderButton.size = new Vector2(2.2f, .7f);
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEngine.Events.UnityEvent();
            passiveButton.OnMouseOver = new UnityEngine.Events.UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                Page -= 1;
                SetUpOptions();
            }));
            ModButtons.Add(button);
        }
    }

    private class SelectionBehaviour
    {
        public string Title;
        public Func<bool> OnClick;
        public bool DefaultValue;

        public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
        {
            Title = title;
            OnClick = onClick;
            DefaultValue = defaultValue;
        }
    }
}