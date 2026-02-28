using static RebuildUs.RebuildUs;

namespace RebuildUs.Modules.CustomOptions;

internal enum CustomOptionType
{
    General,
    Impostor,
    Neutral,
    Crewmate,
    Modifier,
}

internal enum OptionPage
{
    Presets = 0,
    GameSettings = 1,
    VanillaRoleSettings = 2,
    GeneralSettings = 3,
    ImpostorSettings = 4,
    CrewmateSettings = 5,
    NeutralSettings = 6,
    ModifierSettings = 7,
}

internal partial class CustomOption
{
    internal const int CUSTOM_OPTION_PRE_ID = 60000;

    internal static readonly List<CustomOption> AllOptions = [];
    internal static readonly Dictionary<int, CustomOption> AllOptionsById = [];
    private static readonly Dictionary<CustomOptionType, List<CustomOption>> OptionsByType = [];
    private static readonly Dictionary<OptionBehaviour, CustomOption> OptionsByBehaviour = [];
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
    protected static int Preset;
    internal readonly List<CustomOption> Children = [];
    internal readonly bool HideIfParentEnabled;

    internal readonly int Id;
    internal readonly TrKey NameKey;
    internal readonly CustomOptionHeader Header;
    internal readonly CustomOption Parent;
    internal readonly CustomOptionType Type;

    protected OptionBehaviour _optionBehavior;
    internal Color Color;
    internal int DefaultSelection;
    internal ConfigEntry<int> Entry;
    internal string Format;
    internal bool UseSpawnChanceLabel;

    protected CustomOption(int id, CustomOptionType type, TrKey nameKey, CustomOption parent, bool hideIfParentEnabled, string format, Color color, CustomOptionHeader header = null)
    {
        Id = id;
        Type = type;
        NameKey = nameKey;
        Header = header ?? parent?.Header;
        Parent = parent;
        HideIfParentEnabled = hideIfParentEnabled;
        Format = format;
        Color = color;

        Header?.AddOption(this);

        parent?.Children.Add(this);

        if (id != 0)
        {
            AllOptionsById[id] = this;
        }

        if (!OptionsByType.TryGetValue(type, out var list))
        {
            list = [];
            OptionsByType[type] = list;
        }

        list.Add(this);

        AllOptions.Add(this);
    }

    protected CustomOption() { }

    internal virtual bool Enabled
    {
        get => Helpers.RolesEnabled && GetBool();
    }

    protected virtual object GetValue()
    {
        return null;
    }

    protected virtual int GetSelectionIndex()
    {
        return 0;
    }

    protected virtual void SetSelectionIndex(int index) { }

    protected virtual object[] GetSelections()
    {
        return [];
    }

    internal virtual OptionBehaviour CreateOptionBehaviour(GameOptionsMenu menu)
    {
        return UnityObject.Instantiate(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
    }

    internal virtual void InitializeOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        if (optionBehaviour is not StringOption stringOption)
        {
            return;
        }

        stringOption.OnValueChanged = new Action<OptionBehaviour>(_ => { });
        ApplyTitleText(stringOption.TitleText);
        stringOption.Value = stringOption.oldValue = GetSelectionIndex();
        stringOption.ValueText.text = GetValue()?.ToString() ?? string.Empty;
    }

    internal virtual void SyncOptionBehaviourValue()
    {
        if (_optionBehavior is not StringOption stringOption)
        {
            return;
        }

        stringOption.oldValue = stringOption.Value = GetSelectionIndex();
        stringOption.ValueText.text = GetValue()?.ToString() ?? string.Empty;
    }

    internal virtual void IncreaseSelection(RoleTypes icon)
    {
        UpdateSelection(GetSelectionIndex() + 1, icon);
    }

    internal virtual void DecreaseSelection(RoleTypes icon)
    {
        UpdateSelection(GetSelectionIndex() - 1, icon);
    }

    internal virtual void ToggleSelection(RoleTypes icon)
    {
        UpdateSelection(GetBool() ? 0 : 1, icon);
    }

    protected void ApplyTitleText(TextMeshPro titleText)
    {
        if (titleText == null)
        {
            return;
        }

        titleText.text = Helpers.Cs(Color, Tr.Get(NameKey));
        if (UseSpawnChanceLabel)
        {
            titleText.text = Tr.Get(TrKey.SpawnChance);
        }

        var titleLength = titleText.text.Length;
        if (titleLength > 40)
        {
            titleText.fontSize = 2f;
        }
        else if (titleLength > 25)
        {
            titleText.fontSize = 2.2f;
        }
    }

    private static Color ResolveColor(Color? color)
    {
        return color ?? Color.white;
    }

    private static float[] CreateNumberSelections(float min, float max, float step)
    {
        List<float> selections = [];
        for (var selection = min; selection <= max; selection += step)
        {
            selections.Add(selection);
        }

        return [.. selections];
    }

    // Factory methods
    internal static CustomToggleOption Normal(int id, CustomOptionType type, TrKey nameKey, bool defaultValue, CustomOption parent = null, CustomOptionHeader header = null, Color? color = null, bool hideIfParentEnabled = false, string format = "")
    {
        return new CustomToggleOption(id, type, nameKey, defaultValue, parent, hideIfParentEnabled, format, ResolveColor(color), header);
    }

    internal static CustomNumberOption Normal(int id, CustomOptionType type, TrKey nameKey, float defaultValue, float min, float max, float step, CustomOption parent = null, CustomOptionHeader header = null, Color? color = null, bool hideIfParentEnabled = false, string format = "")
    {
        return new CustomNumberOption(id, type, nameKey, CreateNumberSelections(min, max, step), defaultValue, parent, hideIfParentEnabled, format, ResolveColor(color), header);
    }

    internal static CustomGeneralOption<T> Normal<T>(int id, CustomOptionType type, TrKey nameKey, T[] selections, T defaultValue, CustomOption parent = null, CustomOptionHeader header = null, Color? color = null, bool hideIfParentEnabled = false, string format = "")
    {
        return new CustomGeneralOption<T>(id, type, nameKey, selections, defaultValue, parent, hideIfParentEnabled, format, ResolveColor(color), header);
    }

    internal static CustomGeneralOption<string> Normal(int id, CustomOptionType type, TrKey nameKey, string[] selections, int defaultSelection, CustomOption parent = null, CustomOptionHeader header = null, Color? color = null, bool hideIfParentEnabled = false, string format = "")
    {
        return new CustomGeneralOption<string>(id, type, nameKey, selections, defaultSelection, parent, hideIfParentEnabled, format, ResolveColor(color), header);
    }

    internal static CustomPlayerOption Player(int id, CustomOptionType type, TrKey nameKey, int[] playerIds, int defaultSelection, CustomOption parent = null, CustomOptionHeader header = null, Color? color = null, bool hideIfParentEnabled = false, string format = "")
    {
        return new CustomPlayerOption(id, type, nameKey, playerIds, defaultSelection, parent, hideIfParentEnabled, format, ResolveColor(color), header);
    }

    // Static behavior
    private static void SwitchPreset(int newPreset)
    {
        Preset = newPreset;
        foreach (var option in AllOptions)
        {
            if (option.Id == 0)
            {
                continue;
            }

            option.Entry = Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
            option.SetSelectionIndex(Mathf.Clamp(option.Entry.Value, 0, option.GetSelections().Length - 1));
            option.SyncOptionBehaviourValue();
        }

        // make sure to reload all tabs, even the ones in the background, because they might have changed when the preset was switched!
        if (AmongUsClient.Instance?.AmHost != true)
        {
            return;
        }
        UpdateGameOptionsMenu(CustomOptionType.General, _generalTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Impostor, _impostorTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Crewmate, _crewmateTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Neutral, _neutralTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Modifier, _modifierTab.GetComponent<GameOptionsMenu>());
    }

    private static void ShareOptionChange(uint optionId)
    {
        if (!AllOptionsById.TryGetValue((int)optionId, out var option))
        {
            return;
        }
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
        sender.Write((byte)1);
        sender.WritePacked((uint)option.Id);
        sender.WritePacked(Convert.ToUInt32(option.GetSelectionIndex()));
    }

    private static void ShareOptionSelections()
    {
        if (PlayerControl.AllPlayerControls.Count <= 1 || !AmongUsClient.Instance!.AmHost && PlayerControl.LocalPlayer == null)
        {
            return;
        }

        var totalOptions = AllOptions.Count;
        var currentIndex = 0;

        while (currentIndex < totalOptions)
        {
            var amount = (byte)Math.Min(totalOptions - currentIndex, 200);
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
            sender.Write(amount);
            for (var i = 0; i < amount; i++)
            {
                var option = AllOptions[currentIndex++];
                sender.WritePacked((uint)option.Id);
                sender.WritePacked(Convert.ToUInt32(option.GetSelectionIndex()));
            }
        }
    }

    // Getter
    internal int GetSelection()
    {
        return GetSelectionIndex();
    }

    internal bool GetBool()
    {
        return GetSelectionIndex() > 0;
    }

    internal float GetFloat()
    {
        var val = GetValue();
        return val switch
        {
            float f => f,
            int i => i,
            byte b => b,
            _ => 0f,
        };
    }

    internal int GetQuantity()
    {
        return GetSelectionIndex() + 1;
    }

    internal string GetString()
    {
        var sel = GetValue()?.ToString() ?? "";

        return sel switch
        {
            "On" => "<color=#FFFF00FF>" + Tr.Get(TrKey.On) + "</color>",
            "Off" => "<color=#CCCCCCFF>" + Tr.Get(TrKey.Off) + "</color>",
            _ => sel,
        };
    }

    private string GetName()
    {
        return Helpers.Cs(Color, Tr.Get(NameKey));
    }

    internal void UpdateSelection(int newSelection, RoleTypes icon, bool notifyUsers = true)
    {
        var selections = GetSelections();
        var currentIndex = GetSelectionIndex();
        newSelection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
        if (AmongUsClient.Instance?.AmClient == true && notifyUsers && currentIndex != newSelection)
        {
            DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(Id + CUSTOM_OPTION_PRE_ID),
                selections[newSelection].ToString(),
                false,
                icon);
            try
            {
                SetSelectionIndex(newSelection);
                if (FastDestroyableSingleton<GameStartManager>.Instance != null
                    && FastDestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane != null
                    && FastDestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane.LobbyViewSettingsPane != null
                    && FastDestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                {
                    SettingsPaneChangeTab(FastDestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane.LobbyViewSettingsPane,
                        (PanePage)FastDestroyableSingleton<GameStartManager>.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                }
            }
            catch
            {
                // ignored
            }
        }

        SetSelectionIndex(newSelection);
        SyncOptionBehaviourValue();

        if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
        {
            Entry?.Value = GetSelectionIndex();

            if (Id == 0)
            {
                if (GetSelectionIndex() != Preset)
                {
                    SwitchPreset(GetSelectionIndex());
                    ShareOptionSelections();
                }
            }
            else
            {
                ShareOptionChange((uint)Id);
            }
        }

        if (AmongUsClient.Instance?.AmHost != true)
        {
            return;
        }
        if (_generalTab.active)
        {
            var tab = _generalTab.GetComponent<GameOptionsMenu>();
            if (tab != null)
            {
                UpdateGameOptionsMenu(CustomOptionType.General, tab);
            }
        }
        else if (_impostorTab.active)
        {
            var tab = _impostorTab.GetComponent<GameOptionsMenu>();
            if (tab != null)
            {
                UpdateGameOptionsMenu(CustomOptionType.Impostor, tab);
            }
        }
        else if (_crewmateTab.active)
        {
            var tab = _crewmateTab.GetComponent<GameOptionsMenu>();
            if (tab != null)
            {
                UpdateGameOptionsMenu(CustomOptionType.Crewmate, tab);
            }
        }
        else if (_neutralTab.active)
        {
            var tab = _neutralTab.GetComponent<GameOptionsMenu>();
            if (tab != null)
            {
                UpdateGameOptionsMenu(CustomOptionType.Neutral, tab);
            }
        }
        else if (_modifierTab.active)
        {
            var tab = _modifierTab.GetComponent<GameOptionsMenu>();
            if (tab != null)
            {
                UpdateGameOptionsMenu(CustomOptionType.Modifier, tab);
            }
        }
    }

    internal static bool ChangeTabPrefix(GameSettingMenu __instance, OptionPage tabNum, bool previewOnly)
    {
        if (_generalTab == null)
        {
            return true;
        }

        if (previewOnly && Controller.currentTouchType == Controller.TouchType.Joystick || !previewOnly)
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
                    __instance.MenuDescriptionText.text =
                        DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GamePresetsDescription);
                    break;
                case OptionPage.GameSettings:
                    __instance.GameSettingsTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text =
                        DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameSettingsDescription);
                    break;
                case OptionPage.VanillaRoleSettings:
                    __instance.RoleSettingsTab.gameObject.SetActive(true);
                    __instance.RoleSettingsTab.OpenMenu(false);
                    __instance.MenuDescriptionText.text =
                        DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoleSettingsDescription);
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
        if (__instance.gameObject.name != "GAME SETTINGS TAB")
        {
            return;
        }

        NumberOption commonTasksOption = null;
        NumberOption shortTasksOption = null;
        NumberOption longTasksOption = null;

        foreach (var child in __instance.Children)
        {
            var numOpt = child.TryCast<NumberOption>();
            if (numOpt == null)
            {
                continue;
            }

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
        if (Controller.currentTouchType != Controller.TouchType.Joystick)
        {
            __instance.ChangeTab((int)OptionPage.GameSettings, Controller.currentTouchType == Controller.TouchType.Joystick);
        }

        __instance.StartCoroutine(__instance.CoSelectDefault());
    }

    internal static void SettingMenuStart(GameSettingMenu __instance)
    {
        if (Helpers.IsHideNSeekMode)
        {
            return;
        }

        __instance.ChangeTab((int)OptionPage.GameSettings, false);
        __instance.GamePresetsButton.gameObject.SetActive(false);
        __instance.RoleSettingsButton.gameObject.SetActive(false);

        var gameSettingsButton = __instance.GameSettingsButton;
        var leftPanel = gameSettingsButton.gameObject.transform.parent.parent.FindEx("LeftPanel");
        var gameSettingsLabel = leftPanel.parent.gameObject.transform.FindEx("GameSettingsLabel");
        var whatIsThis = leftPanel.parent.gameObject.transform.FindEx("What Is This?");

        gameSettingsLabel.gameObject.SetActive(false);
        whatIsThis.transform.localPosition = new(whatIsThis.transform.localPosition.x - 0.4f, whatIsThis.transform.localPosition.y + 0.9f, whatIsThis.transform.localPosition.z);
        whatIsThis.transform.localScale *= Vector2.one * 0.9f;

        gameSettingsButton.transform.localPosition = new(gameSettingsButton.transform.localPosition.x - 0.2f, gameSettingsButton.transform.localPosition.y + 1.65f, gameSettingsButton.transform.localPosition.z);
        gameSettingsButton.transform.localScale *= Vector2.one * 0.75f;
        __instance.StartCoroutine(Effects.Lerp(2f,
            new Action<float>(_ =>
            {
                gameSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = Tr.Get(TrKey.AmongUsSettings);
            })));
        gameSettingsButton.OnMouseOut.RemoveAllListeners();
        gameSettingsButton.OnMouseOver.RemoveAllListeners();
        gameSettingsButton.SelectButton(false);

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
        var template = __instance.GameSettingsButton.gameObject;
        var buttonObj = UnityObject.Instantiate(template, template.transform.parent);
        buttonObj.transform.localPosition += Vector3.down * 0.5f * ((int)id - 2);
        buttonObj.name = name;
        __instance.StartCoroutine(Effects.Lerp(2f,
            new Action<float>(_ =>
            {
                buttonObj.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText;
            })));
        var buttonPb = buttonObj.GetComponent<PassiveButton>();
        buttonPb.OnClick.RemoveAllListeners();
        buttonPb.OnClick.AddListener((Action)(() => { __instance.ChangeTab((int)id, false); }));
        buttonPb.OnMouseOut.RemoveAllListeners();
        buttonPb.OnMouseOver.RemoveAllListeners();
        buttonPb.SelectButton(false);

        return buttonObj;
    }

    private static GameObject CreateSettingTab(GameSettingMenu __instance, string name, CustomOptionType optionType)
    {
        var template = __instance.GameSettingsTab.gameObject;
        var tabObj = UnityObject.Instantiate(template, template.transform.parent);
        tabObj.name = name;

        var gameOptionsMenu = tabObj.GetComponent<GameOptionsMenu>();
        gameOptionsMenu.Children ??= new();
        foreach (var child in gameOptionsMenu.Children.GetFastEnumerator())
        {
            child.Destroy();
        }

        gameOptionsMenu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        gameOptionsMenu.Children.Clear();

        if (OptionsByType.TryGetValue(optionType, out var relevantOptions))
        {
            CreateSettingsNew(gameOptionsMenu, relevantOptions);
        }

        tabObj.SetActive(false);
        return tabObj;
    }

    private static void CreateSettingsNew(GameOptionsMenu menu, List<CustomOption> options)
    {
        var num = 1.5f;
        CustomOptionHeader currentHeader = null;
        foreach (var option in options)
        {
            if (option.Header != currentHeader)
            {
                currentHeader = option.Header;
                if (currentHeader == null)
                {
                    continue;
                }

                var categoryHeaderMasked = UnityObject.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                categoryHeaderMasked.Title.text = currentHeader.GetTitleText();
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.1f;
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new(-0.903f, num, -2f);
                num -= 0.63f;
            }
            else if (option.Parent != null && (option.Parent.GetSelectionIndex() == 0 && !option.HideIfParentEnabled
                                                || option.Parent.Parent != null && option.Parent.Parent.GetSelectionIndex() == 0 && !option.Parent.HideIfParentEnabled))
            {
                continue;
            }
            else if (option.Parent != null && option.Parent.GetSelectionIndex() != 0 && option.HideIfParentEnabled)
            {
                continue;
            }

            var ob = option.CreateOptionBehaviour(menu);

            ob.transform.localPosition = new(0.952f, num, -2f);
            ob.SetClickMask(menu.ButtonClickMask);

            var renderer = ob.GetComponentsInChildren<SpriteRenderer>(true);
            for (var j = 0; j < renderer.Length; j++)
            {
                renderer[j].material.SetInt(PlayerMaterial.MaskLayer, 20);
            }

            foreach (var tmp in ob.GetComponentsInChildren<TextMeshPro>(true))
            {
                tmp.fontMaterial.SetFloat(StencilComp, 3f);
                tmp.fontMaterial.SetFloat(Stencil, 20);
            }

            option.InitializeOptionBehaviour(ob);

            option._optionBehavior = ob;
            OptionsByBehaviour[ob] = option;

            menu.Children.Add(ob);
            num -= 0.45f;
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }

        foreach (var c in menu.Children)
        {
            if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
            {
                c.SetAsPlayer();
            }
        }
    }

    private static void UpdateGameOptionsMenu(CustomOptionType optionType, GameOptionsMenu menu)
    {
        var children = menu.Children;
        foreach (var t in children)
        {
            t.Destroy();
        }

        menu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        children.Clear();

        if (OptionsByType.TryGetValue(optionType, out var options))
        {
            CreateSettingsNew(menu, options);
        }
    }

    internal static bool StringOptionInitialize(StringOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }

        option.InitializeOptionBehaviour(__instance);

        return false;
    }

    internal static bool StringOptionIncrease(StringOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.IncreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool StringOptionDecrease(StringOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.DecreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool ToggleOptionInitialize(ToggleOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }

        option.InitializeOptionBehaviour(__instance);

        return false;
    }

    internal static void ToggleOptionToggle(ToggleOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return;
        }

        option.ToggleSelection(option.GetOptionIcon());
    }

    internal static bool NumberOptionInitialize(NumberOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }

        option.InitializeOptionBehaviour(__instance);

        return false;
    }

    internal static bool NumberOptionIncrease(NumberOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.IncreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool NumberOptionDecrease(NumberOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.DecreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool PlayerOptionInitialize(PlayerOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }

        option.InitializeOptionBehaviour(__instance);

        return false;
    }

    internal static bool PlayerOptionIncrease(PlayerOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.IncreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool PlayerOptionDecrease(PlayerOption __instance)
    {
        if (!OptionsByBehaviour.TryGetValue(__instance, out var option))
        {
            return true;
        }
        option.DecreaseSelection(option.GetOptionIcon());
        return false;
    }

    internal static bool IsCustomOption(OptionBehaviour ob)
    {
        return OptionsByBehaviour.ContainsKey(ob);
    }

    internal static void CoSpawnSyncSettings()
    {
        if (PlayerControl.LocalPlayer == null || !AmongUsClient.Instance.AmHost)
        {
            return;
        }

        foreach (var option in AllOptions)
        {
            option.Entry?.Value = option.GetSelectionIndex();
        }

        GameManager.Instance.LogicOptions.SyncOptions();
        ShareOptionSelections();
    }

    internal static bool LgoAreInvalid(LegacyGameOptions __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers
            || __instance.NumImpostors < 1
            || __instance.NumImpostors > 3
            || __instance.KillDistance < 0
            || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count
            || __instance.PlayerSpeedMod <= 0f
            || __instance.PlayerSpeedMod > 3f;
    }

    internal static bool Ngo10AreInvalid(NormalGameOptionsV10 __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers
            || __instance.NumImpostors < 1
            || __instance.NumImpostors > 3
            || __instance.KillDistance < 0
            || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count
            || __instance.PlayerSpeedMod <= 0f
            || __instance.PlayerSpeedMod > 3f;
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
        if (option == null)
        {
            return "";
        }

        List<string> options = [];
        if (!skipFirst)
        {
            options.Add(OptionToString(option));
        }
        if (!option.Enabled)
        {
            return string.Join("\n", options);
        }
        foreach (var op in option.Children)
        {
            var str = OptionsToString(op);
            if (str != "")
            {
                options.Add(str);
            }
        }

        return string.Join("\n", options);
    }
}

internal class CustomOption<T> : CustomOption
{
    internal int Selection;
    internal T[] Selections;

    public CustomOption(int id, CustomOptionType type, TrKey nameKey, T[] selections, T defaultValue, CustomOption parent, bool hideIfParentEnabled, string format, Color color, CustomOptionHeader header = null)
    : this(id, type, nameKey, selections, Array.IndexOf(selections, defaultValue), parent, hideIfParentEnabled, format, color, header)
    { }

    public CustomOption(int id, CustomOptionType type, TrKey nameKey, T[] selections, int defaultSelection, CustomOption parent, bool hideIfParentEnabled, string format, Color color, CustomOptionHeader header = null) : base(id, type, nameKey, parent, hideIfParentEnabled, format, color, header)
    {
        Selections = selections;
        DefaultSelection = Mathf.Clamp(defaultSelection, 0, selections.Length - 1);

        if (id != 0)
        {
            Entry = Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
        }
        else
        {
            Entry = Instance.Config.Bind("General", "Selected Preset", DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            Preset = Selection;
        }
    }

    public T Value
    {
        get => Selections[Selection];
    }

    protected override object GetValue()
    {
        return Selections[Selection];
    }

    protected override int GetSelectionIndex()
    {
        return Selection;
    }

    protected override void SetSelectionIndex(int index)
    {
        Selection = Mathf.Clamp(index, 0, Selections.Length - 1);
    }

    protected override object[] GetSelections()
    {
        return [.. Selections];
    }
}