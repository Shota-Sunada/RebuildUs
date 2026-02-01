namespace RebuildUs.Modules.CustomOptions;

public enum PanePage
{
    VanillaGeneral = StringNames.OverviewCategory,
    VanillaRoles = StringNames.RolesCategory, // disabled in mod
    General = 2,
    Overview = 3,
    Impostor = 4,
    Crewmate = 5,
    Neutral = 6,
    Modifier = 7
}

public partial class CustomOption
{
    public static GameObject GeneralPaneButton;
    public static GameObject OverviewPaneButton;
    public static GameObject ImpostorPaneButton;
    public static GameObject CrewmatePaneButton;
    public static GameObject NeutralPaneButton;
    public static GameObject ModifierPaneButton;

    public static void SetTab(LobbyViewSettingsPane __instance, PanePage id)
    {
        var generalPaneButton = GeneralPaneButton.GetComponent<PassiveButton>();
        var overviewPaneButton = OverviewPaneButton.GetComponent<PassiveButton>();
        var impostorPaneButton = ImpostorPaneButton.GetComponent<PassiveButton>();
        var crewmatePaneButton = CrewmatePaneButton.GetComponent<PassiveButton>();
        var neutralPaneButton = NeutralPaneButton.GetComponent<PassiveButton>();
        var modifierPaneButton = ModifierPaneButton.GetComponent<PassiveButton>();

        generalPaneButton.SelectButton(false);
        overviewPaneButton.SelectButton(false);
        impostorPaneButton.SelectButton(false);
        crewmatePaneButton.SelectButton(false);
        neutralPaneButton.SelectButton(false);
        modifierPaneButton.SelectButton(false);
        __instance.taskTabButton.SelectButton(false);
        __instance.rolesTabButton.SelectButton(false);

        switch (id)
        {
            case PanePage.VanillaGeneral:
                __instance.taskTabButton.SelectButton(true);
                __instance.DrawNormalTab();
                break;
            case PanePage.VanillaRoles:
                __instance.rolesTabButton.SelectButton(true);
                __instance.DrawRolesTab();
                break;
            case PanePage.General:
                generalPaneButton.SelectButton(true);
                DrawTab(__instance, CustomOptionType.General);
                break;
            case PanePage.Overview:
                overviewPaneButton.SelectButton(true);
                DrawOverviewTab(__instance);
                break;
            case PanePage.Impostor:
                impostorPaneButton.SelectButton(true);
                DrawTab(__instance, CustomOptionType.Impostor);
                break;
            case PanePage.Crewmate:
                crewmatePaneButton.SelectButton(true);
                DrawTab(__instance, CustomOptionType.Crewmate);
                break;
            case PanePage.Neutral:
                neutralPaneButton.SelectButton(true);
                DrawTab(__instance, CustomOptionType.Neutral);
                break;
            case PanePage.Modifier:
                modifierPaneButton.SelectButton(true);
                DrawTab(__instance, CustomOptionType.Modifier);
                break;
            default:
                Logger.LogWarn($"Invalid Pane Page ID in SettingsPaneChangeTab: {id}");
                break;
        }
    }

    public static void SettingsPaneChangeTab(LobbyViewSettingsPane __instance, PanePage id)
    {
        __instance.currentTab = (StringNames)id;
        for (int i = 0; i < __instance.settingsInfo.Count; ++i)
        {
            __instance.settingsInfo[i].gameObject.Destroy();
        }
        __instance.settingsInfo.Clear();
        SetTab(__instance, id);
        __instance.scrollBar.ScrollToTop();
    }

    private static void DrawTab(LobbyViewSettingsPane __instance, CustomOptionType type)
    {
        if (!OptionsByType.TryGetValue(type, out var options)) return;

        float num = 1.44f;
        int i = 0;
        int singles = 1;
        int headers = 0;
        int lines = 0;
        int numBonus = 0;

        for (int j = 0; j < options.Count; j++)
        {
            var option = options[j];
            if (option.IsHeader)
            {
                if (i != 0)
                {
                    num -= 0.85f;
                    numBonus++;
                }
                if (i % 2 != 0) singles++;
                headers++; // for header

                var categoryHeaderMasked = UnityEngine.Object.Instantiate(__instance.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                categoryHeaderMasked.Title.text = Helpers.Cs(option.Color, Tr.Get(option.HeaderText != "" ? option.HeaderText : option.NameKey));
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                i = 0;
            }
            else if (option.Parent != null && (option.Parent.Selection == 0 || option.Parent.Parent != null && option.Parent.Parent.Selection == 0))
            {
                // Hides options, for which the parent is disabled!
                continue;
            }

            if (option == CustomOptionHolder.CrewmateRolesCountMax ||
                option == CustomOptionHolder.NeutralRolesCountMax ||
                option == CustomOptionHolder.ImpostorRolesCountMax ||
                option == CustomOptionHolder.ModifiersCountMax
            )
            {
                continue;
            }

            var viewSettingsInfoPanel = UnityEngine.Object.Instantiate(__instance.infoPanelOrigin);
            viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
            viewSettingsInfoPanel.transform.localScale = Vector3.one;
            float num2;
            if (i % 2 == 0)
            {
                lines++;
                num2 = -8.95f;
                if (i > 0)
                {
                    num -= 0.85f;
                }
            }
            else
            {
                num2 = -3f;
            }
            viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
            int value = option.GetSelection();
            var (viewName, viewValue) = HandleSpecialOptionsView(option, option.NameKey, option.Selections[value].ToString());
            viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, viewValue, 61);
            viewSettingsInfoPanel.titleText.text = Tr.Get(viewName);

            if (option.IsHeader &&
                (option.Type is CustomOptionType.Neutral or CustomOptionType.Crewmate or CustomOptionType.Impostor or CustomOptionType.Modifier)
            )
            {
                viewSettingsInfoPanel.titleText.text = Tr.Get("SpawnChance");
            }

            __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

            i++;
        }
        float actual_spacing = (headers * 1.05f + lines * 0.85f) / (headers + lines) * 1.01f;
        __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + singles * 2 + headers, 2f, 5f, actual_spacing);
    }

    private static void DrawOverviewTab(LobbyViewSettingsPane __instance)
    {
        var options = new List<CustomOption>();
        var targetTypes = new[] { CustomOptionType.Impostor, CustomOptionType.Crewmate, CustomOptionType.Neutral, CustomOptionType.Modifier };
        foreach (var type in targetTypes)
        {
            foreach (var option in AllOptions)
            {
                if (option.IsHeader && option.Type == type)
                {
                    options.Add(option);
                }
            }
        }

        float num = 1.44f;
        int i = 0;
        int singles = 1;
        int headers = 0;
        int lines = 0;
        var currentType = (CustomOptionType)(-1);
        int numBonus = 0;

        foreach (var option in options)
        {
            if (currentType != option.Type)
            {
                currentType = option.Type;
                if (i != 0)
                {
                    num -= 0.85f;
                    numBonus++;
                }
                if (i % 2 != 0) singles++;
                headers++; // for header
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(__instance.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                categoryHeaderMasked.Title.text = currentType switch
                {
                    CustomOptionType.Impostor => Tr.Get("ImpostorRoles"),
                    CustomOptionType.Neutral => Tr.Get("NeutralRoles"),
                    CustomOptionType.Crewmate => Tr.Get("CrewmateRoles"),
                    CustomOptionType.Modifier => Tr.Get("Modifiers"),
                    _ => Tr.Get("Others")
                };
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                i = 0;
            }

            var viewSettingsInfoPanel = UnityEngine.Object.Instantiate(__instance.infoPanelOrigin);
            viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
            viewSettingsInfoPanel.transform.localScale = Vector3.one;
            float num2;
            if (i % 2 == 0)
            {
                lines++;
                num2 = -8.95f;
                if (i > 0)
                {
                    num -= 0.85f;
                }
            }
            else
            {
                num2 = -3f;
            }
            viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
            int value = option.GetSelection();
            var (optName, optValue) = HandleSpecialOptionsView(option, option.NameKey, option.Selections[value].ToString());
            viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, optValue, 61);
            viewSettingsInfoPanel.titleText.text = Helpers.Cs(option.Color, Tr.Get(optName));
            viewSettingsInfoPanel.titleText.outlineColor = Color.white;
            viewSettingsInfoPanel.titleText.outlineWidth = 0.1f;
            if (option.Type == CustomOptionType.Modifier)
            {
                viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text + BuildModifierExtras(option);
            }
            __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

            i++;
        }
        float actual_spacing = (headers * 1.05f + lines * 0.85f) / (headers + lines) * 1.01f;
        __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + singles * 2 + headers, 2f, 5f, actual_spacing);
    }

    public static void SettingsPaneAwake(LobbyViewSettingsPane __instance)
    {
        __instance.rolesTabButton.gameObject.SetActive(false);
        __instance.gameModeText.text = Tr.Get("GameModeText");

        var overview = __instance.taskTabButton.gameObject;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { __instance.taskTabButton.gameObject.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "Among Us"; })));
        overview.transform.localScale = new Vector3(0.5f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
        overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);
        overview.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.35f, 1f, 1f);
        overview.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-0.6f, -0.1f, 0f);

        GeneralPaneButton = CreateCustomButton(__instance, PanePage.General, "RUSettings", Tr.Get("TabGeneral"), CustomOptionType.General);
        OverviewPaneButton = CreateCustomButton(__instance, PanePage.Overview, "RoleOverview", Tr.Get("TabRolesOverview"), (CustomOptionType)99);
        ImpostorPaneButton = CreateCustomButton(__instance, PanePage.Impostor, "ImpostorSettings", Tr.Get("TabImpostor"), CustomOptionType.Impostor);
        CrewmatePaneButton = CreateCustomButton(__instance, PanePage.Crewmate, "CrewmateSettings", Tr.Get("TabCrewmate"), CustomOptionType.Crewmate);
        NeutralPaneButton = CreateCustomButton(__instance, PanePage.Neutral, "NeutralSettings", Tr.Get("TabNeutral"), CustomOptionType.Neutral);
        ModifierPaneButton = CreateCustomButton(__instance, PanePage.Modifier, "ModifierSettings", Tr.Get("TabModifiers"), CustomOptionType.Modifier);
    }

    public static GameObject CreateCustomButton(LobbyViewSettingsPane __instance, PanePage id, string buttonName, string buttonText, CustomOptionType optionType)
    {
        var template = __instance.taskTabButton.gameObject;
        var buttonObj = UnityEngine.Object.Instantiate(template, template.transform.parent);
        buttonObj.transform.localPosition += Vector3.right * 1.75f * (int)(id - 1);
        buttonObj.name = buttonName;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { buttonObj.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
        var buttonPB = buttonObj.GetComponent<PassiveButton>();
        buttonPB.OnClick.RemoveAllListeners();
        buttonPB.OnClick.AddListener((Action)(() =>
        {
            __instance.ChangeTab((StringNames)id);
        }));
        buttonPB.OnMouseOut.RemoveAllListeners();
        buttonPB.OnMouseOver.RemoveAllListeners();
        buttonPB.SelectButton(false);

        return buttonObj;
    }

    private static (string name, string value) HandleSpecialOptionsView(CustomOption option, string defaultString, string defaultVal)
    {
        string name = defaultString;
        string val = defaultVal;
        if (option == CustomOptionHolder.CrewmateRolesCountMin)
        {
            val = "";
            name = "CrewmateRoles";
            var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
            var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
            if (min > max) min = max;
            val += (min == max) ? $"{max}" : $"{min} - {max}";
        }
        if (option == CustomOptionHolder.NeutralRolesCountMin)
        {
            name = "NeutralRoles";
            var min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
            var max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
            if (min > max) min = max;
            val = (min == max) ? $"{max}" : $"{min} - {max}";
        }
        if (option == CustomOptionHolder.ImpostorRolesCountMin)
        {
            name = "ImpostorRoles";
            var min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
            var max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
            if (max > Helpers.GetOption(Int32OptionNames.NumImpostors)) max = Helpers.GetOption(Int32OptionNames.NumImpostors);
            if (min > max) min = max;
            val = (min == max) ? $"{max}" : $"{min} - {max}";
        }
        if (option == CustomOptionHolder.ModifiersCountMin)
        {
            name = "Modifiers";
            var min = CustomOptionHolder.ModifiersCountMin.GetSelection();
            var max = CustomOptionHolder.ModifiersCountMax.GetSelection();
            if (min > max) min = max;
            val = (min == max) ? $"{max}" : $"{min} - {max}";
        }

        return new(name, val);
    }
}
