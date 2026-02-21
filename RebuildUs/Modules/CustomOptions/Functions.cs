// TODO: 設定画面開いたときに説明文章が更新されない問題があります

using Object = UnityEngine.Object;

namespace RebuildUs.Modules.CustomOptions;

internal enum OptionPage
{
    Presets = 0, // Disabled in mod
    GameSettings = 1,
    VanillaRoleSettings = 2, // Disabled in mod
    GeneralSettings = 3,
    ImpostorSettings = 4,
    CrewmateSettings = 5,
    NeutralSettings = 6,
    ModifierSettings = 7,
}

internal partial class CustomOption
{
    private static GameObject _generalButton;
    private static GameObject _impostorButton;
    private static GameObject _crewmateButton;
    private static GameObject _neutralButton;
    private static GameObject _modifierButton;
    private static GameObject _generalTab;
    private static GameObject _impostorTab;
    private static GameObject _crewmateTab;
    private static GameObject _neutralTab;
    private static GameObject _modifierTab;
    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
    private static readonly int Stencil = Shader.PropertyToID("_Stencil");

    internal static bool ChangeTabPrefix(GameSettingMenu __instance, OptionPage tabNum, bool previewOnly)
    {
        if (_generalTab == null) return true;

        if ((previewOnly && Controller.currentTouchType == Controller.TouchType.Joystick) || !previewOnly)
        {
            __instance.PresetsTab.gameObject.SetActive(false);
            __instance.GameSettingsTab.gameObject.SetActive(false);
            __instance.RoleSettingsTab.gameObject.SetActive(false);
            _generalTab.SetActive(false);
            _impostorTab.SetActive(false);
            _crewmateTab.SetActive(false);
            _neutralTab.SetActive(false);
            _modifierTab.SetActive(false);
            __instance.GamePresetsButton.SelectButton(false);
            __instance.GameSettingsButton.SelectButton(false);
            __instance.RoleSettingsButton.SelectButton(false);
            _generalButton.GetComponent<PassiveButton>().SelectButton(false);
            _impostorButton.GetComponent<PassiveButton>().SelectButton(false);
            _crewmateButton.GetComponent<PassiveButton>().SelectButton(false);
            _neutralButton.GetComponent<PassiveButton>().SelectButton(false);
            _modifierButton.GetComponent<PassiveButton>().SelectButton(false);
            switch (tabNum)
            {
                case OptionPage.Presets:
                    __instance.PresetsTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GamePresetsDescription);
                    break;
                case OptionPage.GameSettings:
                    __instance.GameSettingsTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameSettingsDescription);
                    break;
                case OptionPage.VanillaRoleSettings:
                    __instance.RoleSettingsTab.gameObject.SetActive(true);
                    __instance.RoleSettingsTab.OpenMenu(false);
                    __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoleSettingsDescription);
                    break;
                case OptionPage.GeneralSettings:
                    _generalTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = Tr.Get(TrKey.GeneralSettings);
                    break;
                case OptionPage.ImpostorSettings:
                    _impostorTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = Tr.Get(TrKey.ImpostorSettings);
                    break;
                case OptionPage.CrewmateSettings:
                    _crewmateTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = Tr.Get(TrKey.CrewmateSettings);
                    break;
                case OptionPage.NeutralSettings:
                    _neutralTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = Tr.Get(TrKey.NeutralSettings);
                    break;
                case OptionPage.ModifierSettings:
                    _modifierTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = Tr.Get(TrKey.ModifierSettings);
                    break;
                default:
                    Logger.LogWarn($"Invalid Option Page ID in ChangeTabPrefix: {tabNum}");
                    break;
            }
        }

        if (!previewOnly)
        {
            __instance.ToggleLeftSideDarkener(true);
            __instance.ToggleRightSideDarkener(false);

            switch (tabNum)
            {
                case OptionPage.Presets:
                    __instance.PresetsTab.OpenMenu();
                    __instance.GamePresetsButton.SelectButton(true);
                    break;
                case OptionPage.GameSettings:
                    __instance.GameSettingsTab.OpenMenu();
                    __instance.GameSettingsButton.SelectButton(true);
                    break;
                case OptionPage.VanillaRoleSettings:
                    __instance.RoleSettingsTab.OpenMenu();
                    __instance.RoleSettingsButton.SelectButton(true);
                    break;
                case OptionPage.GeneralSettings:
                    _generalTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    _generalButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.ImpostorSettings:
                    _impostorTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    _impostorButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.CrewmateSettings:
                    _crewmateTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    _crewmateButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.NeutralSettings:
                    _neutralTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    _neutralButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.ModifierSettings:
                    _modifierTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    _modifierButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                default:
                    Logger.LogWarn($"Invalid Option Page ID in ChangeTabPrefix: {tabNum}");
                    break;
            }
        }
        else
        {
            __instance.ToggleLeftSideDarkener(false);
            __instance.ToggleRightSideDarkener(true);
        }

        return false;
    }

    internal static void AdaptTaskCount(GameOptionsMenu __instance)
    {
        if (__instance.gameObject.name != "GAME SETTINGS TAB") return;
        // Adapt task count for main options
        NumberOption commonTasksOption = null;
        NumberOption shortTasksOption = null;
        NumberOption longTasksOption = null;

        foreach (OptionBehaviour child in __instance.Children)
        {
            NumberOption numOpt = child.TryCast<NumberOption>();
            if (numOpt == null) continue;

            switch (numOpt.intOptionName)
            {
                case Int32OptionNames.NumCommonTasks:
                    commonTasksOption = numOpt;
                    break;
                case Int32OptionNames.NumShortTasks:
                    shortTasksOption = numOpt;
                    break;
                case Int32OptionNames.NumLongTasks:
                    longTasksOption = numOpt;
                    break;
            }
        }

        commonTasksOption?.ValidRange = new(0f, 4f);
        shortTasksOption?.ValidRange = new(0f, 23f);
        longTasksOption?.ValidRange = new(0f, 15f);
    }

    internal static void OnEnablePrefix(GameSettingMenu __instance)
    {
        ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.GameSettingsButton, __instance.ControllerSelectable);
        if (Controller.currentTouchType != Controller.TouchType.Joystick) __instance.ChangeTab((int)OptionPage.GameSettings, Controller.currentTouchType == Controller.TouchType.Joystick);

        __instance.StartCoroutine(__instance.CoSelectDefault());
    }

    internal static void SettingMenuStart(GameSettingMenu __instance)
    {
        if (Helpers.IsHideNSeekMode) return;

        // Initialize vanilla tabs
        __instance.ChangeTab((int)OptionPage.GameSettings, false);

        // disable vanilla buttons
        __instance.GamePresetsButton.gameObject.SetActive(false);
        __instance.RoleSettingsButton.gameObject.SetActive(false);

        PassiveButton gameSettingsButton = __instance.GameSettingsButton;
        Transform leftPanel = gameSettingsButton.gameObject.transform.parent.parent.FindEx("LeftPanel");
        Transform gameSettingsLabel = leftPanel.parent.gameObject.transform.FindEx("GameSettingsLabel");
        Transform whatIsThis = leftPanel.parent.gameObject.transform.FindEx("What Is This?");

        gameSettingsLabel.gameObject.SetActive(false);
        whatIsThis.transform.localPosition = new(whatIsThis.transform.localPosition.x - 0.4f, whatIsThis.transform.localPosition.y + 0.9f, whatIsThis.transform.localPosition.z);
        whatIsThis.transform.localScale *= Vector2.one * 0.9f;

        // Game Settings
        gameSettingsButton.transform.localPosition = new(gameSettingsButton.transform.localPosition.x - 0.2f, gameSettingsButton.transform.localPosition.y + 1.65f, gameSettingsButton.transform.localPosition.z);
        gameSettingsButton.transform.localScale *= Vector2.one * 0.75f;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { gameSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = Tr.Get(TrKey.AmongUsSettings); })));
        gameSettingsButton.OnMouseOut.RemoveAllListeners();
        gameSettingsButton.OnMouseOver.RemoveAllListeners();
        gameSettingsButton.SelectButton(false);

        // Mod General Settings
        _generalButton = CreateSettingButton(__instance, "RUGeneralSettingsButton", Tr.Get(TrKey.GeneralSettingsButton), OptionPage.GeneralSettings);
        _generalTab = CreateSettingTab(__instance, "RUGeneralSettingsTab", CustomOptionType.General);
        _impostorButton = CreateSettingButton(__instance, "RUImpostorSettingsButton", Tr.Get(TrKey.ImpostorSettingsButton), OptionPage.ImpostorSettings);
        _impostorTab = CreateSettingTab(__instance, "RUGeneralImpostorTab", CustomOptionType.Impostor);
        _crewmateButton = CreateSettingButton(__instance, "RUCrewmateSettingsButton", Tr.Get(TrKey.CrewmateSettingsButton), OptionPage.CrewmateSettings);
        _crewmateTab = CreateSettingTab(__instance, "RUCrewmateSettingsTab", CustomOptionType.Crewmate);
        _neutralButton = CreateSettingButton(__instance, "RUNeutralSettingsButton", Tr.Get(TrKey.NeutralSettingsButton), OptionPage.NeutralSettings);
        _neutralTab = CreateSettingTab(__instance, "RUNeutralSettingsTab", CustomOptionType.Neutral);
        _modifierButton = CreateSettingButton(__instance, "RUModifierSettingsButton", Tr.Get(TrKey.ModifierSettingsButton), OptionPage.ModifierSettings);
        _modifierTab = CreateSettingTab(__instance, "RUModifierSettingsTab", CustomOptionType.Modifier);

        __instance.GameSettingsButton.SelectButton(true);
        __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameSettingsDescription);
    }

    private static GameObject CreateSettingButton(GameSettingMenu __instance, string name, string buttonText, OptionPage id)
    {
        GameObject template = __instance.GameSettingsButton.gameObject;
        GameObject buttonObj = Object.Instantiate(template, template.transform.parent);
        buttonObj.transform.localPosition += Vector3.down * 0.5f * ((int)id - 2); // 場所調整
        buttonObj.name = name;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { buttonObj.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
        PassiveButton buttonPb = buttonObj.GetComponent<PassiveButton>();
        buttonPb.OnClick.RemoveAllListeners();
        buttonPb.OnClick.AddListener((Action)(() =>
        {
            __instance.ChangeTab((int)id, false);
        }));
        buttonPb.OnMouseOut.RemoveAllListeners();
        buttonPb.OnMouseOver.RemoveAllListeners();
        buttonPb.SelectButton(false);

        return buttonObj;
    }

    private static GameObject CreateSettingTab(GameSettingMenu __instance, string name, CustomOptionType optionType)
    {
        GameObject template = __instance.GameSettingsTab.gameObject;
        GameObject tabObj = Object.Instantiate(template, template.transform.parent);
        tabObj.name = name;

        GameOptionsMenu gameOptionsMenu = tabObj.GetComponent<GameOptionsMenu>();
        gameOptionsMenu.Children ??= new();
        foreach (OptionBehaviour child in gameOptionsMenu.Children.GetFastEnumerator()) child.Destroy();

        gameOptionsMenu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        gameOptionsMenu.Children.Clear();

        if (OptionsByType.TryGetValue(optionType, out List<CustomOption> relevantOptions)) CreateSettingsNew(gameOptionsMenu, relevantOptions);

        tabObj.SetActive(false);
        return tabObj;
    }

    private static void CreateSettingsNew(GameOptionsMenu menu, List<CustomOption> options)
    {
        float num = 1.5f;
        foreach (CustomOption option in options)
        {
            if (option.IsHeader)
            {
                CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                categoryHeaderMasked.Title.text = Helpers.Cs(option.Color, option.HeaderKey != TrKey.None ? Tr.Get(option.HeaderKey) : Tr.Get(option.NameKey));
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new(-0.903f, num, -2f);
                num -= 0.63f;
            }
            else if (option.Parent != null && ((option.Parent.GetSelectionIndex() == 0 && !option.HideIfParentEnabled) || (option.Parent.Parent != null && option.Parent.Parent.GetSelectionIndex() == 0 && !option.Parent.HideIfParentEnabled)))
                continue; // Hides options, for which the parent is disabled!
            else if (option.Parent != null && option.Parent.GetSelectionIndex() != 0 && option.HideIfParentEnabled) continue;

            StringOption ob = Object.Instantiate(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
            ob.transform.localPosition = new(0.952f, num, -2f);
            ob.SetClickMask(menu.ButtonClickMask);

            // "SetUpFromData"
            Il2CppArrayBase<SpriteRenderer> renderer = ob.GetComponentsInChildren<SpriteRenderer>(true);
            for (int j = 0; j < renderer.Length; j++) renderer[j].material.SetInt(PlayerMaterial.MaskLayer, 20);

            foreach (TextMeshPro tmp in ob.GetComponentsInChildren<TextMeshPro>(true))
            {
                tmp.fontMaterial.SetFloat(StencilComp, 3f);
                tmp.fontMaterial.SetFloat(Stencil, 20);
            }

            StringOption so = ob;
            so.OnValueChanged = new Action<OptionBehaviour>(o => { });
            so.TitleText.text = Helpers.Cs(option.Color, Tr.Get(option.NameKey));
            if (option.IsHeader && option.Type is CustomOptionType.Neutral or CustomOptionType.Crewmate or CustomOptionType.Impostor or CustomOptionType.Modifier) so.TitleText.text = Tr.Get(TrKey.SpawnChance);

            if (so.TitleText.text.Length > 25) so.TitleText.fontSize = 2.2f;

            if (so.TitleText.text.Length > 40) so.TitleText.fontSize = 2f;

            so.Value = so.oldValue = option.GetSelectionIndex();
            so.ValueText.text = option.GetValue()?.ToString() ?? string.Empty;
            option._optionBehavior = so;
            OptionsByBehaviour[so] = option;

            menu.Children.Add(ob);
            num -= 0.45f;
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }

        foreach (OptionBehaviour ob in menu.Children)
        {
            if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                ob.SetAsPlayer();
        }
    }

    private static void UpdateGameOptionsMenu(CustomOptionType optionType, GameOptionsMenu menu)
    {
        Il2CppSystem.Collections.Generic.List<OptionBehaviour> children = menu.Children;
        foreach (OptionBehaviour t in children) t.Destroy();

        menu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        children.Clear();

        if (OptionsByType.TryGetValue(optionType, out List<CustomOption> options)) CreateSettingsNew(menu, options);
    }

    internal static bool StringOptionInitialize(StringOption __instance)
    {
        CustomOption option = null;
        foreach (CustomOption t in AllOptions)
        {
            if (t._optionBehavior != __instance) continue;
            option = t;
            break;
        }

        if (option == null) return true;

        __instance.OnValueChanged = new Action<OptionBehaviour>(o => { });
        // __instance.TitleText.text = option.name;
        __instance.Value = __instance.oldValue = option.GetSelectionIndex();
        __instance.ValueText.text = option.GetValue()?.ToString() ?? string.Empty;

        return false;
    }

    internal static bool StringOptionIncrease(StringOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out CustomOption option)) return true;
        option.UpdateSelection(option.GetSelectionIndex() + 1, option.GetOptionIcon());
        return false;
    }

    internal static bool StringOptionDecrease(StringOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out CustomOption option)) return true;
        option.UpdateSelection(option.GetSelectionIndex() - 1, option.GetOptionIcon());
        return false;
    }

    internal static void CoSpawnSyncSettings()
    {
        if (PlayerControl.LocalPlayer == null || !AmongUsClient.Instance.AmHost) return;
        // Save all custom option selections to config at game start
        foreach (CustomOption option in AllOptions) option.Entry?.Value = option.GetSelectionIndex();

        GameManager.Instance.LogicOptions.SyncOptions();
        ShareOptionSelections();
    }

    internal static bool LgoAreInvalid(LegacyGameOptions __instance, ref int maxExpectedPlayers)
    {
        //making the kill distances bound check higher since extra short is added
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1 || __instance.NumImpostors > 3 || __instance.KillDistance < 0 || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f;
    }

    internal static bool Ngo10AreInvalid(NormalGameOptionsV10 __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1 || __instance.NumImpostors > 3 || __instance.KillDistance < 0 || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f;
    }

    internal static void StringOptionInitializePrefix(StringOption __instance) { }

    internal static void StringOptionInitializePostfix(StringOption __instance) { }

    internal static void AppendItem(ref StringNames stringName, ref string value) { }

    internal static bool AdjustStringForViewPanel(StringGameSetting __instance, float value, ref string __result)
    {
        return true;
    }

    internal static string OptionToString(CustomOption option)
    {
        return option == null ? "" : $"{option.GetName()}: {option.GetString()}";
    }

    internal static string OptionsToString(CustomOption option, bool skipFirst = false)
    {
        if (option == null) return "";

        List<string> options = [];
        if (!skipFirst) options.Add(OptionToString(option));
        if (!option.Enabled) return string.Join("\n", options);
        foreach (CustomOption op in option.Children)
        {
            string str = OptionsToString(op);
            if (str != "") options.Add(str);
        }

        return string.Join("\n", options);
    }
}