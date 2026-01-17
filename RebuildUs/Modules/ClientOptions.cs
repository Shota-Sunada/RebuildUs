using UnityEngine.UI;

namespace RebuildUs.Modules;

public static class ClientOptions
{
    private static SelectionBehaviour[] AllOptions = [
            new SelectionBehaviour("ghostsSeeTasksButton", () => MapOptions.ghostsSeeTasks = RebuildUs.GhostsSeeTasks.Value = !RebuildUs.GhostsSeeTasks.Value, RebuildUs.GhostsSeeTasks.Value),
            new SelectionBehaviour("ghostsSeeVotesButton", () => MapOptions.ghostsSeeVotes = RebuildUs.GhostsSeeVotes.Value = !RebuildUs.GhostsSeeVotes.Value, RebuildUs.GhostsSeeVotes.Value),
            new SelectionBehaviour("ghostsSeeRolesButton", () => MapOptions.ghostsSeeRoles = RebuildUs.GhostsSeeRoles.Value = !RebuildUs.GhostsSeeRoles.Value, RebuildUs.GhostsSeeRoles.Value),
            new SelectionBehaviour("showRoleSummaryButton", () => MapOptions.showRoleSummary = RebuildUs.ShowRoleSummary.Value = !RebuildUs.ShowRoleSummary.Value, RebuildUs.ShowRoleSummary.Value),
            new SelectionBehaviour("hideNameplates", () => {
                MapOptions.HideNameplates = RebuildUs.HideNameplates.Value = !RebuildUs.HideNameplates.Value;
                MeetingHudPatch.nameplatesChanged = true;
                return MapOptions.hideNameplates;
            }, RebuildUs.HideNameplates.Value),
            new SelectionBehaviour("showLighterDarker", () => MapOptions.showLighterDarker = RebuildUs.ShowLighterDarker.Value = !RebuildUs.ShowLighterDarker.Value, RebuildUs.ShowLighterDarker.Value),
            new SelectionBehaviour("hideTaskArrows", () => MapOptions.hideTaskArrows = RebuildUs.HideTaskArrows.Value = !RebuildUs.HideTaskArrows.Value, RebuildUs.HideTaskArrows.Value),
            new SelectionBehaviour("offlineHats", () => MapOptions.offlineHats = RebuildUs.OfflineHats.Value = !RebuildUs.OfflineHats.Value, RebuildUs.OfflineHats.Value),
            new SelectionBehaviour("hideFakeTasks", () => MapOptions.hideFakeTasks = RebuildUs.HideFakeTasks.Value = !RebuildUs.HideFakeTasks.Value, RebuildUs.HideFakeTasks.Value),
            new SelectionBehaviour("betterSabotageMap", () => MapOptions.betterSabotageMap = RebuildUs.BetterSabotageMap.Value = !RebuildUs.BetterSabotageMap.Value, RebuildUs.BetterSabotageMap.Value),
            new SelectionBehaviour("forceNormalSabotageMap", () => MapOptions.forceNormalSabotageMap = RebuildUs.ForceNormalSabotageMap.Value = !RebuildUs.ForceNormalSabotageMap.Value, RebuildUs.ForceNormalSabotageMap.Value),
            new SelectionBehaviour("transparentMap", () => MapOptions.transparentMap = RebuildUs.TransparentMap.Value = !RebuildUs.TransparentMap.Value, RebuildUs.TransparentMap.Value),
    ];

    private static GameObject popUp;
    private static TextMeshPro titleText;

    private static ToggleButtonBehaviour moreOptions;
    private static List<ToggleButtonBehaviour> modButtons = [];
    private static TextMeshPro titleTextTitle;

    private static ToggleButtonBehaviour buttonPrefab;
    private static int page = 1;

    public static void Start(MainMenuManager __instance)
    {
        // Prefab for the title
        var go = new GameObject("TitleTextRU");
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.transform.localPosition += Vector3.left * 0.2f;
        titleText = UnityEngine.Object.Instantiate(tmp);
        titleText.gameObject.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(titleText);
    }

    public static void Start(OptionsMenuBehaviour __instance)
    {
        if (!__instance.CensorChatButton) return;

        if (!popUp)
        {
            CreateCustom(__instance);
        }

        if (!buttonPrefab)
        {
            buttonPrefab = UnityEngine.Object.Instantiate(__instance.CensorChatButton);
            UnityEngine.Object.DontDestroyOnLoad(buttonPrefab);
            buttonPrefab.name = "CensorChatPrefab";
            buttonPrefab.gameObject.SetActive(false);
        }

        SetUpOptions();
        InitializeMoreButton(__instance);
    }

    private static void CreateCustom(OptionsMenuBehaviour prefab)
    {
        popUp = UnityEngine.Object.Instantiate(prefab.gameObject);
        UnityEngine.Object.DontDestroyOnLoad(popUp);
        var transform = popUp.transform;
        var pos = transform.localPosition;
        pos.z = -810f;
        transform.localPosition = pos;

        UnityEngine.Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());
        foreach (var obj in popUp.gameObject.GetAllChildren())
        {
            if (obj.name is not "Background" and not "CloseButton")
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        popUp.SetActive(false);
    }

    private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
    {
        __instance.BackButton.transform.localPosition += Vector3.right * 1.8f;
        moreOptions = UnityEngine.Object.Instantiate(buttonPrefab, __instance.CensorChatButton.transform.parent);
        moreOptions.transform.localPosition = __instance.CensorChatButton.transform.localPosition + Vector3.down * 1.0f;

        moreOptions.gameObject.SetActive(true);
        moreOptions.Text.text = Tr.Get("modOptionsText");
        var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
        moreOptionsButton.OnClick = new Button.ButtonClickedEvent();
        moreOptionsButton.OnClick.AddListener((Action)(() =>
        {
            if (!popUp) return;

            if (__instance.transform.parent && __instance.transform.parent == FastDestroyableSingleton<HudManager>.Instance.transform)
            {
                popUp.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                popUp.transform.localPosition = new Vector3(0, 0, -800f);
            }
            else
            {
                popUp.transform.SetParent(null);
                UnityEngine.Object.DontDestroyOnLoad(popUp);
            }

            CheckSetTitle();
            RefreshOpen();
        }));

        var leaveGameButton = GameObject.Find("LeaveGameButton");
        leaveGameButton?.transform.localPosition += Vector3.right * 1.3f;
    }

    private static void RefreshOpen()
    {
        popUp.gameObject.SetActive(false);
        popUp.gameObject.SetActive(true);
        SetUpOptions();
    }

    private static void CheckSetTitle()
    {
        if (!popUp || popUp.GetComponentInChildren<TextMeshPro>() || !titleText) return;

        var title = titleTextTitle = UnityEngine.Object.Instantiate(titleText, popUp.transform);
        title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
        title.gameObject.SetActive(true);
        title.text = Tr.Get("moreOptionsText");
        title.name = "TitleText";
    }

    private static void SetUpOptions()
    {
        // if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

        foreach (var button in modButtons)
        {
            if (button != null) UnityEngine.Object.Destroy(button.gameObject);
        }

        modButtons = [];
        int length = (page * 10) < AllOptions.Length ? page * 10 : AllOptions.Length;

        for (var i = 0; i + ((page - 1) * 10) < length; i++)
        {
            var info = AllOptions[i + ((page - 1) * 10)];

            var button = UnityEngine.Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(i % 2 == 0 ? -1.17f : 1.17f, 1.3f - i / 2 * 0.8f, -.5f);

            var transform = button.transform;
            transform.localPosition = pos;

            button.onState = info.DefaultValue;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = Tr.Get(info.Title);
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(titleText.font);
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

            modButtons.Add(button);
        }
        // ページ移動ボタンを追加
        if (page * 10 < AllOptions.Length)
        {
            var button = UnityEngine.Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(1.2f, -2.5f, -0.5f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get("next");
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(titleText.font);
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
                page += 1;
                SetUpOptions();
            }));
            modButtons.Add(button);
        }
        if (page > 1)
        {
            var button = UnityEngine.Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(-1.2f, -2.5f, -0.5f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Text.text = Tr.Get("previous");
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
            button.Text.font = UnityEngine.Object.Instantiate(titleText.font);
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
                page -= 1;
                SetUpOptions();
            }));
            modButtons.Add(button);
        }
    }

    private static IEnumerable<GameObject> GetAllChildren(this GameObject Go)
    {
        for (var i = 0; i < Go.transform.childCount; i++)
        {
            yield return Go.transform.GetChild(i).gameObject;
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