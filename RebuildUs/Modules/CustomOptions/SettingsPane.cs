using Object = UnityEngine.Object;

namespace RebuildUs.Modules.CustomOptions;

internal enum PanePage
{
    VanillaGeneral = StringNames.OverviewCategory,
    VanillaRoles = StringNames.RolesCategory, // disabled in mod
    General = 2,
    Overview = 3,
    Impostor = 4,
    Crewmate = 5,
    Neutral = 6,
    Modifier = 7,
}

internal partial class CustomOption
{
    private static GameObject _generalPaneButton;
    private static GameObject _overviewPaneButton;
    private static GameObject _impostorPaneButton;
    private static GameObject _crewmatePaneButton;
    private static GameObject _neutralPaneButton;
    private static GameObject _modifierPaneButton;

    internal static void SetTab(LobbyViewSettingsPane __instance, PanePage id)
    {
        PassiveButton generalPaneButton = _generalPaneButton.GetComponent<PassiveButton>();
        PassiveButton overviewPaneButton = _overviewPaneButton.GetComponent<PassiveButton>();
        PassiveButton impostorPaneButton = _impostorPaneButton.GetComponent<PassiveButton>();
        PassiveButton crewmatePaneButton = _crewmatePaneButton.GetComponent<PassiveButton>();
        PassiveButton neutralPaneButton = _neutralPaneButton.GetComponent<PassiveButton>();
        PassiveButton modifierPaneButton = _modifierPaneButton.GetComponent<PassiveButton>();

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

    internal static void SettingsPaneChangeTab(LobbyViewSettingsPane __instance, PanePage id)
    {
        __instance.currentTab = (StringNames)id;
        foreach (GameObject info in __instance.settingsInfo.GetFastEnumerator()) info.gameObject.Destroy();

        __instance.settingsInfo.Clear();
        SetTab(__instance, id);
        __instance.scrollBar.ScrollToTop();
    }

    private static void DrawTab(LobbyViewSettingsPane __instance, CustomOptionType type)
    {
        if (!OptionsByType.TryGetValue(type, out List<CustomOption> options)) return;

        float num = 1.44f;
        int i = 0;
        int singles = 1;
        int headers = 0;
        int lines = 0;

        foreach (CustomOption option in options)
        {
            if (option.IsHeader)
            {
                if (i != 0) num -= 0.85f;

                if (i % 2 != 0) singles++;
                headers++; // for header

                CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(__instance.categoryHeaderOrigin, __instance.settingsContainer, true);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                categoryHeaderMasked.Title.text = Helpers.Cs(option.Color, option.HeaderKey != TrKey.None ? Tr.Get(option.HeaderKey) : Tr.Get(option.NameKey));
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new(-9.77f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                i = 0;
            }
            else if (option.Parent != null && (option.Parent.Selection == 0 || (option.Parent.Parent != null && option.Parent.Parent.Selection == 0)))
            {
                // Hides options, for which the parent is disabled!
                continue;
            }

            if (option == CustomOptionHolder.CrewmateRolesCountMax || option == CustomOptionHolder.NeutralRolesCountMax || option == CustomOptionHolder.ImpostorRolesCountMax || option == CustomOptionHolder.ModifiersCountMax) continue;

            ViewSettingsInfoPanel viewSettingsInfoPanel = Object.Instantiate(__instance.infoPanelOrigin, __instance.settingsContainer, true);
            viewSettingsInfoPanel.transform.localScale = Vector3.one;
            float num2;
            if (i % 2 == 0)
            {
                lines++;
                num2 = -8.95f;
                if (i > 0) num -= 0.85f;
            }
            else
                num2 = -3f;

            viewSettingsInfoPanel.transform.localPosition = new(num2, num, -2f);
            int value = option.GetSelection();
            (TrKey viewName, string viewValue) = HandleSpecialOptionsView(option, option.NameKey, option.Selections[value].ToString());
            viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, viewValue, 61);
            viewSettingsInfoPanel.titleText.text = Tr.Get(viewName);

            if (option.IsHeader && option.Type is CustomOptionType.Neutral or CustomOptionType.Crewmate or CustomOptionType.Impostor or CustomOptionType.Modifier) viewSettingsInfoPanel.titleText.text = Tr.Get(TrKey.SpawnChance);

            __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

            i++;
        }

        float actualSpacing = (((headers * 1.05f) + (lines * 0.85f)) / (headers + lines)) * 1.01f;
        __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + (singles * 2) + headers, 2f, 5f, actualSpacing);
    }

    private static void DrawOverviewTab(LobbyViewSettingsPane __instance)
    {
        List<CustomOption> options = new();
        CustomOptionType[] targetTypes = new[] { CustomOptionType.Impostor, CustomOptionType.Crewmate, CustomOptionType.Neutral, CustomOptionType.Modifier };
        foreach (CustomOptionType type in targetTypes)
        {
            foreach (CustomOption option in AllOptions)
                if (option.IsHeader && option.Type == type)
                    options.Add(option);
        }

        float num = 1.44f;
        int i = 0;
        int singles = 1;
        int headers = 0;
        int lines = 0;
        CustomOptionType currentType = (CustomOptionType)(-1);

        foreach (CustomOption option in options)
        {
            if (currentType != option.Type)
            {
                currentType = option.Type;
                if (i != 0) num -= 0.85f;

                if (i % 2 != 0) singles++;
                headers++; // for header
                CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(__instance.categoryHeaderOrigin, __instance.settingsContainer, true);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                categoryHeaderMasked.Title.text = currentType switch
                {
                    CustomOptionType.Impostor => Tr.Get(TrKey.ImpostorRoles),
                    CustomOptionType.Neutral => Tr.Get(TrKey.NeutralRoles),
                    CustomOptionType.Crewmate => Tr.Get(TrKey.CrewmateRoles),
                    CustomOptionType.Modifier => Tr.Get(TrKey.Modifiers),
                    _ => Tr.Get(TrKey.Others),
                };
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new(-9.77f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                i = 0;
            }

            ViewSettingsInfoPanel viewSettingsInfoPanel = Object.Instantiate(__instance.infoPanelOrigin, __instance.settingsContainer, true);
            viewSettingsInfoPanel.transform.localScale = Vector3.one;
            float num2;
            if (i % 2 == 0)
            {
                lines++;
                num2 = -8.95f;
                if (i > 0) num -= 0.85f;
            }
            else
                num2 = -3f;

            viewSettingsInfoPanel.transform.localPosition = new(num2, num, -2f);
            int value = option.GetSelection();
            (TrKey optName, string optValue) = HandleSpecialOptionsView(option, option.NameKey, option.Selections[value].ToString());
            viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, optValue, 61);
            viewSettingsInfoPanel.titleText.text = Helpers.Cs(option.Color, Tr.Get(optName));
            viewSettingsInfoPanel.titleText.outlineColor = Color.white;
            viewSettingsInfoPanel.titleText.outlineWidth = 0.1f;
            if (option.Type == CustomOptionType.Modifier) viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text;

            __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

            i++;
        }

        float actualSpacing = (((headers * 1.05f) + (lines * 0.85f)) / (headers + lines)) * 1.01f;
        __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + (singles * 2) + headers, 2f, 5f, actualSpacing);
    }

    internal static void SettingsPaneAwake(LobbyViewSettingsPane __instance)
    {
        __instance.rolesTabButton.gameObject.SetActive(false);
        __instance.gameModeText.text = Tr.Get(TrKey.GameModeText);

        GameObject overview = __instance.taskTabButton.gameObject;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(_ => { __instance.taskTabButton.gameObject.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "Among Us"; })));
        overview.transform.localScale = new(0.5f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
        overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);
        overview.transform.FindChild("FontPlacer").transform.localScale = new(1.35f, 1f, 1f);
        overview.transform.FindChild("FontPlacer").transform.localPosition = new(-0.6f, -0.1f, 0f);

        _generalPaneButton = CreateCustomButton(__instance, PanePage.General, "RUSettings", Tr.Get(TrKey.TabGeneral), CustomOptionType.General);
        _overviewPaneButton = CreateCustomButton(__instance, PanePage.Overview, "RoleOverview", Tr.Get(TrKey.TabRolesOverview), (CustomOptionType)99);
        _impostorPaneButton = CreateCustomButton(__instance, PanePage.Impostor, "ImpostorSettings", Tr.Get(TrKey.TabImpostor), CustomOptionType.Impostor);
        _crewmatePaneButton = CreateCustomButton(__instance, PanePage.Crewmate, "CrewmateSettings", Tr.Get(TrKey.TabCrewmate), CustomOptionType.Crewmate);
        _neutralPaneButton = CreateCustomButton(__instance, PanePage.Neutral, "NeutralSettings", Tr.Get(TrKey.TabNeutral), CustomOptionType.Neutral);
        _modifierPaneButton = CreateCustomButton(__instance, PanePage.Modifier, "ModifierSettings", Tr.Get(TrKey.TabModifiers), CustomOptionType.Modifier);
    }

    private static GameObject CreateCustomButton(LobbyViewSettingsPane __instance, PanePage id, string buttonName, string buttonText, CustomOptionType optionType)
    {
        GameObject template = __instance.taskTabButton.gameObject;
        GameObject buttonObj = Object.Instantiate(template, template.transform.parent);
        buttonObj.transform.localPosition += Vector3.right * 1.75f * (int)(id - 1);
        buttonObj.name = buttonName;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(_ => { buttonObj.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
        PassiveButton buttonPb = buttonObj.GetComponent<PassiveButton>();
        buttonPb.OnClick.RemoveAllListeners();
        buttonPb.OnClick.AddListener((Action)(() =>
        {
            __instance.ChangeTab((StringNames)id);
        }));
        buttonPb.OnMouseOut.RemoveAllListeners();
        buttonPb.OnMouseOver.RemoveAllListeners();
        buttonPb.SelectButton(false);

        return buttonObj;
    }

    private static (TrKey name, string value) HandleSpecialOptionsView(CustomOption option, TrKey defKey, string defaultVal)
    {
        TrKey name = defKey;
        string val = defaultVal;
        if (option == CustomOptionHolder.CrewmateRolesCountMin)
        {
            val = "";
            name = TrKey.CrewmateRoles;
            int min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
            int max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
            if (min > max) min = max;
            val += min == max ? $"{max}" : $"{min} - {max}";
        }

        if (option == CustomOptionHolder.NeutralRolesCountMin)
        {
            name = TrKey.NeutralRoles;
            int min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
            int max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
            if (min > max) min = max;
            val = min == max ? $"{max}" : $"{min} - {max}";
        }

        if (option == CustomOptionHolder.ImpostorRolesCountMin)
        {
            name = TrKey.ImpostorRoles;
            int min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
            int max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
            if (max > Helpers.GetOption(Int32OptionNames.NumImpostors)) max = Helpers.GetOption(Int32OptionNames.NumImpostors);
            if (min > max) min = max;
            val = min == max ? $"{max}" : $"{min} - {max}";
        }

        if (option == CustomOptionHolder.ModifiersCountMin)
        {
            name = TrKey.Modifiers;
            int min = CustomOptionHolder.ModifiersCountMin.GetSelection();
            int max = CustomOptionHolder.ModifiersCountMax.GetSelection();
            if (min > max) min = max;
            val = min == max ? $"{max}" : $"{min} - {max}";
        }

        return new(name, val);
    }
}