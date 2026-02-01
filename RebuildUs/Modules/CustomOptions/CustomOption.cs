using static RebuildUs.RebuildUs;

namespace RebuildUs.Modules.CustomOptions;

public enum CustomGamemodes
{
    Classic,
}
public enum CustomOptionType
{
    General,
    Impostor,
    Neutral,
    Crewmate,
    Modifier,
}
public partial class CustomOption
{
    public const int CUSTOM_OPTION_PRE_ID = 6000;

    public static List<CustomOption> AllOptions = [];
    public static Dictionary<int, CustomOption> AllOptionsById = [];
    public static Dictionary<CustomOptionType, List<CustomOption>> OptionsByType = [];
    public static Dictionary<OptionBehaviour, CustomOption> OptionsByBehaviour = [];
    public static int Preset = 0;

    public int Id;
    public TranslateKey NameKey;
    public string Format;
    public Color Color;
    public object[] Selections;

    public int DefaultSelection;
    public ConfigEntry<int> Entry;
    public int Selection;
    public OptionBehaviour OptionBehavior;
    public CustomOption Parent;
    public List<CustomOption> Children;
    public bool IsHeader;
    public TranslateKey HeaderKey;
    public CustomOptionType Type;
    public bool HideIfParentEnabled;

    public virtual bool Enabled
    {
        get
        {
            return Helpers.RolesEnabled && GetBool();
        }
    }

    public CustomOption() { }

    // Option creation
    public CustomOption(int id, CustomOptionType type, TranslateKey nameKey, object[] selections, object defaultValue, CustomOption parent, bool hideIfParentEnabled, string format, Color color)
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
            if (AllOptionsById.ContainsKey(id))
            {
                Logger.LogDebug($"CustomOption id {id} is used in multiple places.");
            }
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

    public static CustomOption Normal(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
        string[] selections,
        CustomOption parent = null,
        byte r = byte.MaxValue,
        byte g = byte.MaxValue,
        byte b = byte.MaxValue,
        byte a = byte.MaxValue,
        bool hideIfParentEnabled = false,
        string format = ""
        )
    {
        return new CustomOption(id, type, nameKey, selections, "", parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
        string[] selections,
        TranslateKey headerKey,
        byte r = byte.MaxValue,
        byte g = byte.MaxValue,
        byte b = byte.MaxValue,
        byte a = byte.MaxValue,
        string format = ""
        )
    {
        var opt = new CustomOption(id, type, nameKey, selections, "", null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    public static CustomOption Normal(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
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
        string format = ""
        )
    {
        List<object> selections = [];
        for (float s = min; s <= max; s += step)
        {
            selections.Add(s);
        }
        return new CustomOption(id, type, nameKey, [.. selections], defaultValue, parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
        float defaultValue,
        float min,
        float max,
        float step,
        TranslateKey headerKey,
        byte r = byte.MaxValue,
        byte g = byte.MaxValue,
        byte b = byte.MaxValue,
        byte a = byte.MaxValue,
        string format = ""
        )
    {
        List<object> selections = [];
        for (float s = min; s <= max; s += step)
        {
            selections.Add(s);
        }

        var opt = new CustomOption(id, type, nameKey, [.. selections], defaultValue, null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    public static CustomOption Normal(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
        bool defaultValue,
        CustomOption parent = null,
        byte r = byte.MaxValue,
        byte g = byte.MaxValue,
        byte b = byte.MaxValue,
        byte a = byte.MaxValue,
        bool hideIfParentEnabled = false,
        string format = ""
    )
    {
        return new CustomOption(id, type, nameKey, [Tr.Get(TranslateKey.Off), Tr.Get(TranslateKey.On)], defaultValue ? Tr.Get(TranslateKey.On) : Tr.Get(TranslateKey.Off), parent, hideIfParentEnabled, format, new Color32(r, g, b, a));
    }

    public static CustomOption Header(
        int id,
        CustomOptionType type,
        TranslateKey nameKey,
        bool defaultValue,
        TranslateKey headerKey,
        byte r = byte.MaxValue,
        byte g = byte.MaxValue,
        byte b = byte.MaxValue,
        byte a = byte.MaxValue,
        string format = ""
    )
    {
        var opt = new CustomOption(id, type, nameKey, [Tr.Get(TranslateKey.Off), Tr.Get(TranslateKey.On)], defaultValue ? Tr.Get(TranslateKey.On) : Tr.Get(TranslateKey.Off), null, false, format, new Color32(r, g, b, a));
        opt.SetHeader(headerKey);
        return opt;
    }

    private void SetHeader(TranslateKey key)
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
            UpdateGameOptionsMenu(CustomOptionType.General, GeneralTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Impostor, ImpostorTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Crewmate, CrewmateTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Neutral, NeutralTab.GetComponent<GameOptionsMenu>());
            UpdateGameOptionsMenu(CustomOptionType.Modifier, ModifierTab.GetComponent<GameOptionsMenu>());
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
        if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance!.AmHost == false && PlayerControl.LocalPlayer == null) return;

        int totalOptions = AllOptions.Count;
        int currentIndex = 0;

        while (currentIndex < totalOptions)
        {
            byte amount = (byte)Math.Min(totalOptions - currentIndex, 200);
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
            sender.Write(amount);
            for (int i = 0; i < amount; i++)
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

        if (sel == "On")
        {
            return "<color=#FFFF00FF>" + Tr.Get(TranslateKey.On) + "</color>";
        }
        else if (sel == "Off")
        {
            return "<color=#CCCCCCFF>" + Tr.Get(TranslateKey.Off) + "</color>";
        }

        return sel;
    }

    public string GetName()
    {
        return Helpers.Cs(Color, Tr.GetDynamic(NameKey));
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
                if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                {
                    SettingsPaneChangeTab(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, (PanePage)GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                }
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
            {
                ShareOptionChange((uint)Id);
            }
        }

        if (AmongUsClient.Instance?.AmHost == true)
        {
            if (GeneralTab.active)
            {
                var tab = GeneralTab.GetComponent<GameOptionsMenu>();
                if (tab != null)
                {
                    UpdateGameOptionsMenu(CustomOptionType.General, tab);
                }
            }
            else if (ImpostorTab.active)
            {
                var tab = ImpostorTab.GetComponent<GameOptionsMenu>();
                if (tab != null)
                {
                    UpdateGameOptionsMenu(CustomOptionType.Impostor, tab);
                }
            }
            else if (CrewmateTab.active)
            {
                var tab = CrewmateTab.GetComponent<GameOptionsMenu>();
                if (tab != null)
                {
                    UpdateGameOptionsMenu(CustomOptionType.Crewmate, tab);
                }
            }
            else if (NeutralTab.active)
            {
                var tab = NeutralTab.GetComponent<GameOptionsMenu>();
                if (tab != null)
                {
                    UpdateGameOptionsMenu(CustomOptionType.Neutral, tab);
                }
            }
            else if (ModifierTab.active)
            {
                var tab = ModifierTab.GetComponent<GameOptionsMenu>();
                if (tab != null)
                {
                    UpdateGameOptionsMenu(CustomOptionType.Modifier, tab);
                }
            }
        }
    }
}


