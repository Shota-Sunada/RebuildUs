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
    protected static int Preset;

    internal readonly int Id;
    internal readonly CustomOptionType Type;
    internal readonly TrKey NameKey;
    internal Color Color;
    internal readonly CustomOption Parent;
    internal bool IsHeader;
    internal TrKey HeaderKey;
    internal string Format;
    internal readonly bool HideIfParentEnabled;
    internal readonly List<CustomOption> Children = [];

    private OptionBehaviour _optionBehavior;
    internal ConfigEntry<int> Entry;
    internal int DefaultSelection;

    protected CustomOption(int id,
                           CustomOptionType type,
                           TrKey nameKey,
                           CustomOption parent,
                           bool hideIfParentEnabled,
                           string format,
                           Color color)
    {
        Id = id;
        Type = type;
        NameKey = nameKey;
        Parent = parent;
        HideIfParentEnabled = hideIfParentEnabled;
        Format = format;
        Color = color;

        parent?.Children.Add(this);

        if (id != 0)
        {
            AllOptionsById[id] = this;
        }

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

    // Factory methods
    internal static CustomOption<bool> Normal(int id,
                                              CustomOptionType type,
                                              TrKey nameKey,
                                              bool defaultValue,
                                              CustomOption parent = null,
                                              Color? color = null,
                                              bool hideIfParentEnabled = false,
                                              string format = "")
    {
        return new(id, type, nameKey, [false, true], defaultValue, parent, hideIfParentEnabled, format, color ?? Color.white);
    }

    internal static CustomOption<bool> Header(int id,
                                              CustomOptionType type,
                                              TrKey nameKey,
                                              bool defaultValue,
                                              TrKey headerKey,
                                              Color? color = null,
                                              string format = "")
    {
        CustomOption<bool> opt = new(id, type, nameKey, [false, true], defaultValue, null, false, format, color ?? Color.white);
        opt.SetHeader(headerKey);
        return opt;
    }

    internal static CustomOption<float> Normal(int id,
                                               CustomOptionType type,
                                               TrKey nameKey,
                                               float defaultValue,
                                               float min,
                                               float max,
                                               float step,
                                               CustomOption parent = null,
                                               Color? color = null,
                                               bool hideIfParentEnabled = false,
                                               string format = "")
    {
        List<float> selections = [];
        for (float s = min; s <= max; s += step) selections.Add(s);

        return new(id, type, nameKey, [.. selections], defaultValue, parent, hideIfParentEnabled, format, color ?? Color.white);
    }

    internal static CustomOption<float> Header(int id,
                                               CustomOptionType type,
                                               TrKey nameKey,
                                               float defaultValue,
                                               float min,
                                               float max,
                                               float step,
                                               TrKey headerKey,
                                               Color? color = null,
                                               string format = "")
    {
        List<float> selections = [];
        for (float s = min; s <= max; s += step) selections.Add(s);

        CustomOption<float> opt = new(id, type, nameKey, [.. selections], defaultValue, null, false, format, color ?? Color.white);
        opt.SetHeader(headerKey);
        return opt;
    }

    internal static CustomOption<T> Normal<T>(int id,
                                              CustomOptionType type,
                                              TrKey nameKey,
                                              T[] selections,
                                              T defaultValue,
                                              CustomOption parent = null,
                                              Color? color = null,
                                              bool hideIfParentEnabled = false,
                                              string format = "")
    {
        return new(id, type, nameKey, selections, defaultValue, parent, hideIfParentEnabled, format, color ?? Color.white);
    }

    internal static CustomOption<string> Normal(int id,
                                                   CustomOptionType type,
                                                   TrKey nameKey,
                                                   string[] selections,
                                                   int defaultSelection,
                                                   CustomOption parent = null,
                                                   Color? color = null,
                                                   bool hideIfParentEnabled = false,
                                                   string format = "")
    {
        return new(id, type, nameKey, selections, defaultSelection, parent, hideIfParentEnabled, format, color ?? Color.white);
    }

    internal static CustomOption<T> Header<T>(int id,
                                              CustomOptionType type,
                                              TrKey nameKey,
                                              T[] selections,
                                              T defaultValue,
                                              TrKey headerKey,
                                              Color? color = null,
                                              string format = "")
    {
        CustomOption<T> opt = new(id, type, nameKey, selections, defaultValue, null, false, format, color ?? Color.white);
        opt.SetHeader(headerKey);
        return opt;
    }

    internal static CustomOption<string> Header(int id,
                                                CustomOptionType type,
                                                TrKey nameKey,
                                                string[] selections,
                                                int defaultSelection,
                                                TrKey headerKey,
                                                Color? color = null,
                                                string format = "")
    {
        CustomOption<string> opt = new(id, type, nameKey, selections, defaultSelection, null, false, format, color ?? Color.white);
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
        Preset = newPreset;
        foreach (CustomOption option in AllOptions)
        {
            if (option.Id == 0) continue;

            option.Entry = Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
            option.SetSelectionIndex(Mathf.Clamp(option.Entry.Value, 0, option.GetSelections().Length - 1));
            if (option._optionBehavior == null || option._optionBehavior is not StringOption stringOption) continue;
            stringOption.oldValue = stringOption.Value = option.GetSelectionIndex();
            stringOption.ValueText.text = option.GetValue().ToString();
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
        sender.WritePacked(Convert.ToUInt32(option.GetSelectionIndex()));
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
        object val = GetValue();
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
        string sel = GetValue()?.ToString() ?? "";

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
        object[] selections = GetSelections();
        int currentIndex = GetSelectionIndex();
        newSelection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
        if (AmongUsClient.Instance?.AmClient == true && notifyUsers && currentIndex != newSelection)
        {
            DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(Id + CUSTOM_OPTION_PRE_ID), selections[newSelection].ToString(), false, icon);
            try
            {
                SetSelectionIndex(newSelection);
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

        SetSelectionIndex(newSelection);

        if (_optionBehavior != null && _optionBehavior is StringOption stringOption)
        {
            stringOption.oldValue = stringOption.Value = GetSelectionIndex();
            stringOption.ValueText.text = GetValue().ToString();
        }

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

internal class CustomOption<T> : CustomOption
{
    internal T[] Selections;
    internal int Selection;

    public T Value
    {
        get => Selections[Selection];
    }

    public CustomOption(int id,
                        CustomOptionType type,
                        TrKey nameKey,
                        T[] selections,
                        T defaultValue,
                        CustomOption parent,
                        bool hideIfParentEnabled,
                        string format,
                        Color color)
        : this(id, type, nameKey, selections, Array.IndexOf(selections, defaultValue), parent, hideIfParentEnabled, format, color) { }

    public CustomOption(int id,
                        CustomOptionType type,
                        TrKey nameKey,
                        T[] selections,
                        int defaultSelection,
                        CustomOption parent,
                        bool hideIfParentEnabled,
                        string format,
                        Color color)
        : base(id, type, nameKey, parent, hideIfParentEnabled, format, color)
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
        return [.. Selections.Cast<object>()];
    }
}