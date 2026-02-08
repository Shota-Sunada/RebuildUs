using static RebuildUs.RebuildUs;

namespace RebuildUs.Modules.CustomOptions;

public enum CustomOptionType
{
    General,
    Impostor,
    Neutral,
    Crewmate,
    Modifier
}

public partial class CustomOption
{
    public const int CUSTOM_OPTION_PRE_ID = 60000;

    public static List<CustomOption> AllOptions = [];
    public static Dictionary<int, CustomOption> AllOptionsById = [];
    public static Dictionary<CustomOptionType, List<CustomOption>> OptionsByType = [];
    public static Dictionary<OptionBehaviour, CustomOption> OptionsByBehaviour = [];
    public static int Preset;
    public List<CustomOption> Children;
    public Color Color;

    public int DefaultSelection;
    public ConfigEntry<int> Entry;
    public string Format;
    public TrKey HeaderKey;
    public bool HideIfParentEnabled;

    public int Id;
    public bool IsHeader;
    public TrKey NameKey;
    public OptionBehaviour OptionBehavior;
    public CustomOption Parent;
    public int Selection;
    public object[] Selections;
    public CustomOptionType Type;

    public CustomOption() { }

    // Option creation
    public CustomOption(int id, CustomOptionType type, TrKey nameKey, object[] selections, object defaultValue, CustomOption parent, bool hideIfParentEnabled, string format, Color color)
    {
        Id = id;
        NameKey = nameKey;
        Color = color;
        Selections = selections;
        var index = Array.IndexOf(selections, defaultValue);
        DefaultSelection = index >= 0 ? index : 0;
        Parent = parent;
        Type = type;
        Children = [];
        parent?.Children.Add(this);
        HideIfParentEnabled = hideIfParentEnabled;
        Selection = 0;
        if (id != 0)
        {
            Entry = Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);

#if DEBUG
            if (AllOptionsById.ContainsKey(id)) Logger.LogDebug($"CustomOption id {id} is used in multiple places.");
#endif
        }
        else
        {
            Entry = Instance.Config.Bind("General", "Selected Preset", DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            Preset = Selection;
        }

        AllOptionsById[id] = this;

        if (!OptionsByType.TryGetValue(type, out var list))
        {
            list = [];
            OptionsByType[type] = list;
        }

        list.Add(this);

        AllOptions.Add(this);
    }

    public virtual bool Enabled
    {
        get => Helpers.RolesEnabled && GetBool();
    }

    public static CustomOption Normal(int id, CustomOptionType type, TrKey nameKey, string[] selections, CustomOption parent = null, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, bool hideIfParentEnabled = false, string format = "")
    {
        return new(id, type, nameKey, selections, "", parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(int id, CustomOptionType type, TrKey nameKey, string[] selections, TrKey headerKey, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, string format = "")
    {
        var opt = new CustomOption(id, type, nameKey, selections, "", null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    public static CustomOption Normal(int id, CustomOptionType type, TrKey nameKey, float defaultValue, float min, float max, float step, CustomOption parent = null, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, bool hideIfParentEnabled = false, string format = "")
    {
        List<object> selections = [];
        for (var s = min; s <= max; s += step) selections.Add(s);
        return new(id, type, nameKey, [.. selections], defaultValue, parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(int id, CustomOptionType type, TrKey nameKey, float defaultValue, float min, float max, float step, TrKey headerKey, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, string format = "")
    {
        List<object> selections = [];
        for (var s = min; s <= max; s += step) selections.Add(s);

        var opt = new CustomOption(id, type, nameKey, [.. selections], defaultValue, null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    public static CustomOption Normal(int id, CustomOptionType type, TrKey nameKey, bool defaultValue, CustomOption parent = null, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, bool hideIfParentEnabled = false, string format = "")
    {
        return new(id, type, nameKey, [Tr.Get(TrKey.Off), Tr.Get(TrKey.On)], defaultValue ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off), parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(int id, CustomOptionType type, TrKey nameKey, bool defaultValue, TrKey headerKey, byte r = byte.MaxValue, byte g = byte.MaxValue, byte b = byte.MaxValue, byte a = byte.MaxValue, string format = "")
    {
        var opt = new CustomOption(id, type, nameKey, [Tr.Get(TrKey.Off), Tr.Get(TrKey.On)], defaultValue ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off), null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    private void SetHeader(TrKey key)
    {
        IsHeader = true;
        HeaderKey = key;
    }

    // Static behaviour
    public static void SwitchPreset(int newPreset)
    {
        Preset = newPreset;
        foreach (var option in AllOptions)
        {
            if (option.Id == 0) continue;

            option.Entry = Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
            option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
            if (option.OptionBehavior != null && option.OptionBehavior is StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = option.Selection;
                stringOption.ValueText.text = option.Selections[option.Selection].ToString();
            }
        }

        // make sure to reload all tabs, even the ones in the background, because they might have changed when the preset was switched!
        if (AmongUsClient.Instance?.AmHost == true)
        {
            UpdateGameOptionsMenu(CustomOptionType.General, _generalTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Impostor, _impostorTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Crewmate, _crewmateTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Neutral, _neutralTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Modifier, _modifierTab.GetComponent<GameOptionsMenu>());
        }
    }

    public static void ShareOptionChange(uint optionId)
    {
        if (!AllOptionsById.TryGetValue((int)optionId, out var option)) return;
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
        sender.Write((byte)1);
        sender.WritePacked((uint)option.Id);
        sender.WritePacked(Convert.ToUInt32(option.Selection));
    }

    public static void ShareOptionSelections()
    {
        if (PlayerControl.AllPlayerControls.Count <= 1 || (!AmongUsClient.Instance!.AmHost && PlayerControl.LocalPlayer == null)) return;

        var totalOptions = AllOptions.Count;
        var currentIndex = 0;

        while (currentIndex < totalOptions)
        {
            var amount = (byte)Math.Min(totalOptions - currentIndex, 200);
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
            sender.Write(amount);
            for (var i = 0; i < amount; i++)
            {
                var option = AllOptions[currentIndex++];
                sender.WritePacked((uint)option.Id);
                sender.WritePacked(Convert.ToUInt32(option.Selection));
            }
        }
    }

    // Getter
    public virtual int GetSelection()
    {
        return Selection;
    }

    public virtual bool GetBool()
    {
        return Selection > 0;
    }

    public virtual float GetFloat()
    {
        return (float)Selections[Selection];
    }

    public int GetQuantity()
    {
        return Selection + 1;
    }

    public virtual string GetString()
    {
        var sel = Selections[Selection].ToString();

        if (sel == "On") return "<color=#FFFF00FF>" + Tr.Get(TrKey.On) + "</color>";

        if (sel == "Off") return "<color=#CCCCCCFF>" + Tr.Get(TrKey.Off) + "</color>";

        return sel;
    }

    public string GetName()
    {
        return Helpers.Cs(Color, Tr.Get(NameKey));
    }

    public virtual void UpdateSelection(int newSelection, RoleTypes icon, bool notifyUsers = true)
    {
        newSelection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
        if (AmongUsClient.Instance?.AmClient == true && notifyUsers && Selection != newSelection)
        {
            DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(Id + CUSTOM_OPTION_PRE_ID), Selections[newSelection].ToString(), false, icon);
            try
            {
                Selection = newSelection;
                if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf) SettingsPaneChangeTab(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, (PanePage)GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
            }
            catch { }
        }

        Selection = newSelection;

        if (OptionBehavior != null && OptionBehavior is StringOption stringOption)
        {
            stringOption.oldValue = stringOption.Value = Selection;
            stringOption.ValueText.text = Selections[Selection].ToString();
        }

        if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
        {
            Entry?.Value = Selection;

            if (Id == 0)
            {
                if (Selection != Preset)
                {
                    SwitchPreset(Selection);
                    ShareOptionSelections();
                }
            }
            else
                ShareOptionChange((uint)Id);
        }

        if (AmongUsClient.Instance?.AmHost == true)
        {
            if (_generalTab.active)
            {
                var tab = _generalTab.GetComponent<GameOptionsMenu>();
                if (tab != null) UpdateGameOptionsMenu(CustomOptionType.General, tab);
            }
            else if (_impostorTab.active)
            {
                var tab = _impostorTab.GetComponent<GameOptionsMenu>();
                if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Impostor, tab);
            }
            else if (_crewmateTab.active)
            {
                var tab = _crewmateTab.GetComponent<GameOptionsMenu>();
                if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Crewmate, tab);
            }
            else if (_neutralTab.active)
            {
                var tab = _neutralTab.GetComponent<GameOptionsMenu>();
                if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Neutral, tab);
            }
            else if (_modifierTab.active)
            {
                var tab = _modifierTab.GetComponent<GameOptionsMenu>();
                if (tab != null) UpdateGameOptionsMenu(CustomOptionType.Modifier, tab);
            }
        }
    }
}
