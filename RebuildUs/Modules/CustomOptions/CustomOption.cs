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

internal partial class CustomOption
{
    internal const int CUSTOM_OPTION_PRE_ID = 60000;

    internal static readonly List<CustomOption> AllOptions = [];
    internal static readonly Dictionary<int, CustomOption> AllOptionsById = [];
    private static readonly Dictionary<CustomOptionType, List<CustomOption>> OptionsByType = [];
    private static readonly Dictionary<OptionBehaviour, CustomOption> OptionsByBehaviour = [];
    private static int _preset;
    private readonly bool _hideIfParentEnabled;
    internal readonly List<CustomOption> Children;
    private OptionBehaviour _optionBehavior;
    internal Color Color;

    internal int DefaultSelection;
    internal ConfigEntry<int> Entry;
    internal string Format;
    internal TrKey HeaderKey;

    internal int Id;
    internal bool IsHeader;
    internal TrKey NameKey;
    internal CustomOption Parent;
    internal int Selection;
    internal object[] Selections;
    internal CustomOptionType Type;

    // Option creation
    internal CustomOption(int id,
                          CustomOptionType type,
                          TrKey nameKey,
                          object[] selections,
                          object defaultValue,
                          CustomOption parent,
                          bool hideIfParentEnabled,
                          string format,
                          Color color)
    {
        Id = id;
        NameKey = nameKey;
        Color = color;
        Selections = selections;
        int index = Array.IndexOf(selections, defaultValue);
        DefaultSelection = index >= 0 ? index : 0;
        Parent = parent;
        Type = type;
        Children = [];
        parent?.Children.Add(this);
        _hideIfParentEnabled = hideIfParentEnabled;
        Format = format;
        Selection = 0;
        if (id != 0)
        {
            Entry = Instance.Config.Bind($"Preset{_preset}", id.ToString(), DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);

#if DEBUG
            if (AllOptionsById.ContainsKey(id)) Logger.LogDebug($"CustomOption id {id} is used in multiple places.");
#endif
        }
        else
        {
            Entry = Instance.Config.Bind("General", "Selected Preset", DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            _preset = Selection;
        }

        AllOptionsById[id] = this;

        if (!OptionsByType.TryGetValue(type, out List<CustomOption> list))
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

    internal static CustomOption Normal(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        object[] selections,
                                        CustomOption parent = null,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        bool hideIfParentEnabled = false,
                                        string format = "")
    {
        return new(id, type, nameKey, selections, "", parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    internal static CustomOption Header(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        object[] selections,
                                        TrKey headerKey,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        string format = "")
    {
        CustomOption opt = new(id, type, nameKey, selections, "", null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    internal static CustomOption Normal(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        float defaultValue,
                                        float min,
                                        float max,
                                        float step,
                                        CustomOption parent = null,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        bool hideIfParentEnabled = false,
                                        string format = "")
    {
        List<object> selections = [];
        for (float s = min; s <= max; s += step) selections.Add(s);

        return new(id, type, nameKey, [.. selections], defaultValue, parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    internal static CustomOption Header(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        float defaultValue,
                                        float min,
                                        float max,
                                        float step,
                                        TrKey headerKey,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        string format = "")
    {
        List<object> selections = [];
        for (float s = min; s <= max; s += step) selections.Add(s);

        CustomOption opt = new(id, type, nameKey, [.. selections], defaultValue, null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    internal static CustomOption Normal(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        bool defaultValue,
                                        CustomOption parent = null,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        bool hideIfParentEnabled = false,
                                        string format = "")
    {
        return new(id, type, nameKey, [Tr.Get(TrKey.Off), Tr.Get(TrKey.On)], defaultValue ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off), parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    internal static CustomOption Header(int id,
                                        CustomOptionType type,
                                        TrKey nameKey,
                                        bool defaultValue,
                                        TrKey headerKey,
                                        byte r = byte.MaxValue,
                                        byte g = byte.MaxValue,
                                        byte b = byte.MaxValue,
                                        byte a = byte.MaxValue,
                                        string format = "")
    {
        CustomOption opt = new(id, type, nameKey, [Tr.Get(TrKey.Off), Tr.Get(TrKey.On)], defaultValue ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off), null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    private void SetHeader(TrKey key)
    {
        IsHeader = true;
        HeaderKey = key;
    }

    // Static behavior
    private static void SwitchPreset(int newPreset)
    {
        _preset = newPreset;
        foreach (CustomOption option in AllOptions)
        {
            if (option.Id == 0) continue;

            option.Entry = Instance.Config.Bind($"Preset{_preset}", option.Id.ToString(), option.DefaultSelection);
            option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
            if (option._optionBehavior == null || option._optionBehavior is not StringOption stringOption) continue;
            stringOption.oldValue = stringOption.Value = option.Selection;
            stringOption.ValueText.text = option.Selections[option.Selection].ToString();
        }

        // make sure to reload all tabs, even the ones in the background, because they might have changed when the preset was switched!
        if (AmongUsClient.Instance?.AmHost != true) return;
        UpdateGameOptionsMenu(CustomOptionType.General, _generalTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Impostor, _impostorTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Crewmate, _crewmateTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Neutral, _neutralTab.GetComponent<GameOptionsMenu>());
        UpdateGameOptionsMenu(CustomOptionType.Modifier, _modifierTab.GetComponent<GameOptionsMenu>());
    }

    private static void ShareOptionChange(uint optionId)
    {
        if (!AllOptionsById.TryGetValue((int)optionId, out CustomOption option)) return;
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
        sender.Write((byte)1);
        sender.WritePacked((uint)option.Id);
        sender.WritePacked(Convert.ToUInt32(option.Selection));
    }

    private static void ShareOptionSelections()
    {
        if (PlayerControl.AllPlayerControls.Count <= 1 || (!AmongUsClient.Instance!.AmHost && PlayerControl.LocalPlayer == null)) return;

        int totalOptions = AllOptions.Count;
        int currentIndex = 0;

        while (currentIndex < totalOptions)
        {
            byte amount = (byte)Math.Min(totalOptions - currentIndex, 200);
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
            sender.Write(amount);
            for (int i = 0; i < amount; i++)
            {
                CustomOption option = AllOptions[currentIndex++];
                sender.WritePacked((uint)option.Id);
                sender.WritePacked(Convert.ToUInt32(option.Selection));
            }
        }
    }

    // Getter
    internal int GetSelection()
    {
        return Selection;
    }

    internal bool GetBool()
    {
        return Selection > 0;
    }

    internal float GetFloat()
    {
        return (float)Selections[Selection];
    }

    internal int GetQuantity()
    {
        return Selection + 1;
    }

    internal string GetString()
    {
        string sel = Selections[Selection].ToString();

        return sel switch
        {
            "On" => "<color=#FFFF00FF>" + Tr.Get(TrKey.On) + "</color>",
            "Off" => "<color=#CCCCCCFF>" + Tr.Get(TrKey.Off) + "</color>",
            _ => sel,
        };
    }

    public string GetName()
    {
        return Helpers.Cs(Color, Tr.Get(NameKey));
    }

    internal virtual void UpdateSelection(int newSelection, RoleTypes icon, bool notifyUsers = true)
    {
        newSelection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
        if (AmongUsClient.Instance?.AmClient == true && notifyUsers && Selection != newSelection)
        {
            DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(Id + CUSTOM_OPTION_PRE_ID), Selections[newSelection].ToString(), false, icon);
            try
            {
                Selection = newSelection;
                if (GameStartManager.Instance != null
                    && GameStartManager.Instance.LobbyInfoPane != null
                    && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null
                    && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                    SettingsPaneChangeTab(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, (PanePage)GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
            }
            catch
            {
                // ignored
            }
        }

        Selection = newSelection;

        if (_optionBehavior != null && _optionBehavior is StringOption stringOption)
        {
            stringOption.oldValue = stringOption.Value = Selection;
            stringOption.ValueText.text = Selections[Selection].ToString();
        }

        if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
        {
            Entry?.Value = Selection;

            if (Id == 0)
            {
                if (Selection != _preset)
                {
                    SwitchPreset(Selection);
                    ShareOptionSelections();
                }
            }
            else
                ShareOptionChange((uint)Id);
        }

        if (AmongUsClient.Instance?.AmHost != true) return;
        if (_generalTab.active)
        {
            GameOptionsMenu tab = _generalTab.GetComponent<GameOptionsMenu>();
            if (tab != null) UpdateGameOptionsMenu(CustomOptionType.General, tab);
        }
        else if (_impostorTab.active)
        {
            GameOptionsMenu tab = _impostorTab.GetComponent<GameOptionsMenu>();
            if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Impostor, tab);
        }
        else if (_crewmateTab.active)
        {
            GameOptionsMenu tab = _crewmateTab.GetComponent<GameOptionsMenu>();
            if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Crewmate, tab);
        }
        else if (_neutralTab.active)
        {
            GameOptionsMenu tab = _neutralTab.GetComponent<GameOptionsMenu>();
            if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Neutral, tab);
        }
        else if (_modifierTab.active)
        {
            GameOptionsMenu tab = _modifierTab.GetComponent<GameOptionsMenu>();
            if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Modifier, tab);
        }
    }
}