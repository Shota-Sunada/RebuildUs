using AmongUs.GameOptions;
using RebuildUs.Options;
using RebuildUs.Extensions;
using System.Text;

namespace RebuildUs.Modules.CustomOptions;

public partial class CustomOption
{
    public static byte[] SerializeOptions()
    {
        using MemoryStream memoryStream = new();
        using BinaryWriter binaryWriter = new(memoryStream);
        int lastId = -1;
        foreach (var option in AllOptions.OrderBy(x => x.Id))
        {
            if (option.Id == 0) continue;
            bool consecutive = lastId + 1 == option.Id;
            lastId = option.Id;

            binaryWriter.Write((byte)(option.Selection + (consecutive ? 128 : 0)));
            if (!consecutive) binaryWriter.Write((ushort)option.Id);
        }
        binaryWriter.Flush();
        memoryStream.Position = 0L;
        return memoryStream.ToArray();
    }

    public static int DeserializeOptions(byte[] inputValues)
    {
        BinaryReader reader = new(new MemoryStream(inputValues));
        int lastId = -1;
        bool somethingApplied = false;
        int errors = 0;
        while (reader.BaseStream.Position < inputValues.Length)
        {
            try
            {
                int selection = reader.ReadByte();
                int id = -1;
                bool consecutive = selection >= 128;
                if (consecutive)
                {
                    selection -= 128;
                    id = lastId + 1;
                }
                else
                {
                    id = reader.ReadUInt16();
                }
                if (id == 0) continue;
                lastId = id;
                CustomOption option = AllOptions.First(option => option.Id == id);
                option.Entry = RebuildUs.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
                option.Selection = selection;
                if (option.OptionBehavior != null && option.OptionBehavior is StringOption stringOption)
                {
                    stringOption.oldValue = stringOption.Value = option.Selection;
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
                somethingApplied = true;
            }
            catch (Exception e)
            {
                Logger.LogWarn($"id:{lastId}:{e}: while deserializing - tried to paste invalid settings!");
                errors++;
            }
        }
        return Convert.ToInt32(somethingApplied) + (errors > 0 ? 0 : 1);
    }

    public static void SettingMenuChangeTab(GameSettingMenu __instance, int tabNum, bool previewOnly)
    {
        if (previewOnly) return;
        foreach (var tab in SettingMenuCurrentTabs)
        {
            tab?.SetActive(false);
        }
        foreach (var button in SettingMenuCurrentButtons)
        {
            button.SelectButton(false);
        }
        if (tabNum > 2)
        {
            tabNum -= 3;
            SettingMenuCurrentTabs[tabNum].SetActive(true);
            SettingMenuCurrentButtons[tabNum].SelectButton(true);
        }
    }

    public static bool SettingsPaneSetTab(LobbyViewSettingsPane __instance)
    {
        if ((int)__instance.currentTab < 15)
        {
            SettingsPaneChangeTab(__instance, __instance.currentTab);
            return false;
        }
        return true;
    }

    public static void SettingsPaneChangeTab(LobbyViewSettingsPane __instance, StringNames category)
    {
        int tabNum = (int)category;

        foreach (var button in SettingsPaneCurrentButtons)
        {
            button.SelectButton(false);
        }
        if (tabNum > 20) // StringNames are in the range of 3000+
            return;
        __instance.taskTabButton.SelectButton(false);

        if (tabNum > 2)
        {
            tabNum -= 3;
            //GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
            SettingsPaneCurrentButtons[tabNum].SelectButton(true);
            DrawTab(__instance, SettingsPaneCurrentButtonTypes[tabNum]);
        }
    }

    public static void SettingsPaneUpdate(LobbyViewSettingsPane __instance)
    {
        if (SettingsPaneCurrentButtons.Count == 0)
        {
            GameModeChangedFlag = true;
            SettingsPaneAwake(__instance);
        }
    }

    public static List<PassiveButton> SettingsPaneCurrentButtons = [];
    public static List<CustomOptionType> SettingsPaneCurrentButtonTypes = [];
    public static bool GameModeChangedFlag = false;

    public static void CreateCustomButton(LobbyViewSettingsPane __instance, int targetMenu, string buttonName, string buttonText, CustomOptionType optionType)
    {
        buttonName = "View" + buttonName;
        var buttonTemplate = GameObject.Find("OverviewTab");
        var modSettingsButton = GameObject.Find(buttonName);
        if (modSettingsButton == null)
        {
            modSettingsButton = UnityEngine.Object.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            modSettingsButton.transform.localPosition += Vector3.right * 1.75f * (targetMenu - 2);
            modSettingsButton.name = buttonName;
            __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { modSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
            var modSettingsPassiveButton = modSettingsButton.GetComponent<PassiveButton>();
            modSettingsPassiveButton.OnClick.RemoveAllListeners();
            modSettingsPassiveButton.OnClick.AddListener((System.Action)(() =>
            {
                __instance.ChangeTab((StringNames)targetMenu);
            }));
            modSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
            modSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
            modSettingsPassiveButton.SelectButton(false);
            SettingsPaneCurrentButtons.Add(modSettingsPassiveButton);
            SettingsPaneCurrentButtonTypes.Add(optionType);
        }
    }

    public static void SettingsPaneAwake(LobbyViewSettingsPane __instance)
    {
        SettingsPaneCurrentButtons.ForEach(x => x?.Destroy());
        SettingsPaneCurrentButtons.Clear();
        SettingsPaneCurrentButtonTypes.Clear();

        RemoveVanillaTabs(__instance);

        CreateSettingTabs(__instance);

    }

    public static void RemoveVanillaTabs(LobbyViewSettingsPane __instance)
    {
        GameObject.Find("RolesTabs")?.Destroy();
        var overview = GameObject.Find("OverviewTab");
        if (!GameModeChangedFlag)
        {
            overview.transform.localScale = new Vector3(0.5f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
            overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);

        }
        overview.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.35f, 1f, 1f);
        overview.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-0.6f, -0.1f, 0f);
        GameModeChangedFlag = false;
    }

    public static void DrawTab(LobbyViewSettingsPane __instance, CustomOptionType optionType)
    {
        var relevantOptions = AllOptions.Where(x => x.Type == optionType || x.Type == CustomOptionType.Guesser && optionType == CustomOptionType.General).ToList();

        if ((int)optionType == 99)
        {
            // Create 4 Groups with Role settings only
            relevantOptions.Clear();
            relevantOptions.AddRange(AllOptions.Where(x => x.Type == CustomOptionType.Impostor && x.IsHeader));
            relevantOptions.AddRange(AllOptions.Where(x => x.Type == CustomOptionType.Neutral && x.IsHeader));
            relevantOptions.AddRange(AllOptions.Where(x => x.Type == CustomOptionType.Crewmate && x.IsHeader));
            relevantOptions.AddRange(AllOptions.Where(x => x.Type == CustomOptionType.Modifier && x.IsHeader));
            foreach (var option in AllOptions)
            {
                if (option.Parent != null && option.Parent.GetSelection() > 0)
                {
                    // if (option.id == 103) //Deputy
                    //     relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.sheriffSpawnRate) + 1, option);
                    // else if (option.id == 224) //Sidekick
                    //     relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.jackalSpawnRate) + 1, option);
                    // else if (option.id == 358) //Prosecutor
                    //     relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.lawyerSpawnRate) + 1, option);
                }
            }
        }

        // if (TORMapOptions.gameMode == CustomGamemodes.Guesser) // Exclude guesser options in neutral mode
        //     relevantOptions = relevantOptions.Where(x => !(new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318 }).Contains(x.id)).ToList();

        for (int j = 0; j < __instance.settingsInfo.Count; j++)
        {
            __instance.settingsInfo[j].gameObject.Destroy();
        }
        __instance.settingsInfo.Clear();

        float num = 1.44f;
        int i = 0;
        int singles = 1;
        int headers = 0;
        int lines = 0;
        var curType = CustomOptionType.Modifier;
        int numBonus = 0;

        foreach (var option in relevantOptions)
        {
            if (option.IsHeader && (int)optionType != 99 || (int)optionType == 99 && curType != option.Type)
            {
                curType = option.Type;
                if (i != 0)
                {
                    num -= 0.85f;
                    numBonus++;
                }
                if (i % 2 != 0) singles++;
                headers++; // for header
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(__instance.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                categoryHeaderMasked.Title.text = option.HeaderText != "" ? option.HeaderText : option.NameKey;
                if ((int)optionType == 99)
                {
                    categoryHeaderMasked.Title.text = new Dictionary<CustomOptionType, string>() { { CustomOptionType.Impostor, "Impostor Roles" }, { CustomOptionType.Neutral, "Neutral Roles" },
                            { CustomOptionType.Crewmate, "Crewmate Roles" }, { CustomOptionType.Modifier, "Modifiers" } }[curType];
                }

                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.2f;
                categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                i = 0;
            }
            else if (option.Parent != null && (option.Parent.Selection == 0 || option.Parent.Parent != null && option.Parent.Parent.Selection == 0))
            {
                continue;  // Hides options, for which the parent is disabled!
            }

            if (option == CustomOptionHolder.CrewmateRolesCountMax || option == CustomOptionHolder.NeutralRolesCountMax || option == CustomOptionHolder.ImpostorRolesCountMax || option == CustomOptionHolder.ModifiersCountMax)
                continue;

            ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
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
            var settingTuple = HandleSpecialOptionsView(option, option.NameKey, option.Selections[value].ToString());
            viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, settingTuple.Item2, 61);
            viewSettingsInfoPanel.titleText.text = settingTuple.Item1;
            if (option.IsHeader && (int)optionType != 99 && option.HeaderText == "" && (option.Type == CustomOptionType.Neutral || option.Type == CustomOptionType.Crewmate || option.Type == CustomOptionType.Impostor || option.Type == CustomOptionType.Modifier))
            {
                viewSettingsInfoPanel.titleText.text = "Spawn Chance";
            }
            if ((int)optionType == 99)
            {
                viewSettingsInfoPanel.titleText.outlineColor = Color.white;
                viewSettingsInfoPanel.titleText.outlineWidth = 0.2f;
                if (option.Type == CustomOptionType.Modifier)
                    viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text + BuildModifierExtras(option);
            }
            __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

            i++;
        }
        float actual_spacing = (headers * 1.05f + lines * 0.85f) / (headers + lines) * 1.01f;
        __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + singles * 2 + headers, 2f, 5f, actual_spacing);
    }

    private static Tuple<string, string> HandleSpecialOptionsView(CustomOption option, string defaultString, string defaultVal)
    {
        string name = defaultString;
        string val = defaultVal;
        if (option == CustomOptionHolder.CrewmateRolesCountMin)
        {
            val = "";
            name = "Crewmate Roles";
            var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
            var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
            if (min > max) min = max;
            val += (min == max) ? $"{max}" : $"{min} - {max}";
        }
        if (option == CustomOptionHolder.NeutralRolesCountMin)
        {
            name = "Neutral Roles";
            var min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
            var max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
            if (min > max) min = max;
            val = (min == max) ? $"{max}" : $"{min} - {max}";
        }
        if (option == CustomOptionHolder.ImpostorRolesCountMin)
        {
            name = "Impostor Roles";
            var min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
            var max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
            if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors) max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
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

    public static void CreateSettingTabs(LobbyViewSettingsPane __instance)
    {
        // Handle different gamemodes and tabs needed therein.
        int next = 3;
        if (MapOptions.GameMode == CustomGamemodes.Guesser || MapOptions.GameMode == CustomGamemodes.Classic)
        {

            // create RU settings
            CreateCustomButton(__instance, next++, "RUSettings", "RU Settings", CustomOptionType.General);
            // create role overview
            CreateCustomButton(__instance, next++, "RoleOverview", "Role Overview", (CustomOptionType)99);
            // IMp
            CreateCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles", CustomOptionType.Impostor);

            // Neutral
            CreateCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles", CustomOptionType.Neutral);
            // Crew
            CreateCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles", CustomOptionType.Crewmate);
            // Modifier
            CreateCustomButton(__instance, next++, "ModifierSettings", "Modifiers", CustomOptionType.Modifier);

        }
        else if (MapOptions.GameMode == CustomGamemodes.HideNSeek)
        {
            // create Main HNS settings
            CreateCustomButton(__instance, next++, "HideNSeekMain", "Hide 'N' Seek", CustomOptionType.HideNSeekMain);
            // create HNS Role settings
            CreateCustomButton(__instance, next++, "HideNSeekRoles", "Hide 'N' Seek Roles", CustomOptionType.HideNSeekRoles);
        }
        else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
        {
            CreateCustomButton(__instance, next++, "PropHunt", "Prop Hunt", CustomOptionType.PropHunt);
        }
    }

    public static void AdaptTaskCount(GameOptionsMenu __instance)
    {
        if (__instance.gameObject.name == "GAME SETTINGS TAB")
        {
            // Adapt task count for main options
            var commonTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
            commonTasksOption?.ValidRange = new FloatRange(0f, 4f);
            var shortTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).TryCast<NumberOption>();
            shortTasksOption?.ValidRange = new FloatRange(0f, 23f);
            var longTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).TryCast<NumberOption>();
            longTasksOption?.ValidRange = new FloatRange(0f, 15f);
        }
    }

    public static List<GameObject> SettingMenuCurrentTabs = [];
    public static List<PassiveButton> SettingMenuCurrentButtons = [];
    public static Dictionary<byte, GameOptionsMenu> CurrentGOMs = [];
    public static void SettingMenuStart(GameSettingMenu __instance)
    {
        SettingMenuCurrentTabs.ForEach(x => x?.Destroy());
        SettingMenuCurrentButtons.ForEach(x => x?.Destroy());
        SettingMenuCurrentTabs = [];
        SettingMenuCurrentButtons = [];
        CurrentGOMs.Clear();

        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        RemoveVanillaTabs(__instance);

        CreateSettingTabs(__instance);
    }

    private static void CreateSettings(GameOptionsMenu menu, List<CustomOption> options)
    {
        float num = 1.5f;
        foreach (CustomOption option in options)
        {
            if (option.IsHeader)
            {
                var categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                categoryHeaderMasked.Title.text = option.HeaderText != "" ? option.HeaderText : option.NameKey;
                categoryHeaderMasked.Title.outlineColor = Color.white;
                categoryHeaderMasked.Title.outlineWidth = 0.2f;
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                num -= 0.63f;
            }
            else if (option.Parent != null && (option.Parent.Selection == 0 && !option.HideIfParentEnabled || option.Parent.Parent != null && option.Parent.Parent.Selection == 0 && !option.Parent.HideIfParentEnabled))
            {
                continue;  // Hides options, for which the parent is disabled!
            }
            else if (option.Parent != null && option.Parent.Selection != 0 && option.HideIfParentEnabled)
            {
                continue;
            }

            OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
            optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
            optionBehaviour.SetClickMask(menu.ButtonClickMask);

            // "SetUpFromData"
            SpriteRenderer[] componentsInChildren = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
            }
            foreach (TextMeshPro textMeshPro in optionBehaviour.GetComponentsInChildren<TextMeshPro>(true))
            {
                textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                textMeshPro.fontMaterial.SetFloat("_Stencil", 20);
            }

            var stringOption = optionBehaviour as StringOption;
            stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
            stringOption.TitleText.text = option.NameKey;
            if (option.IsHeader && option.HeaderText == "" && (option.Type == CustomOptionType.Neutral || option.Type == CustomOptionType.Crewmate || option.Type == CustomOptionType.Impostor || option.Type == CustomOptionType.Modifier))
            {
                stringOption.TitleText.text = "Spawn Chance";
            }
            if (stringOption.TitleText.text.Length > 25)
            {
                stringOption.TitleText.fontSize = 2.2f;
            }
            if (stringOption.TitleText.text.Length > 40)
            {
                stringOption.TitleText.fontSize = 2f;
            }
            stringOption.Value = stringOption.oldValue = option.Selection;
            stringOption.ValueText.text = option.Selections[option.Selection].ToString();
            option.OptionBehavior = stringOption;

            menu.Children.Add(optionBehaviour);
            num -= 0.45f;
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }

        for (int i = 0; i < menu.Children.Count; i++)
        {
            OptionBehaviour optionBehaviour = menu.Children[i];
            if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
            {
                optionBehaviour.SetAsPlayer();
            }
        }
    }

    private static void RemoveVanillaTabs(GameSettingMenu __instance)
    {
        GameObject.Find("What Is This?")?.Destroy();
        GameObject.Find("GamePresetButton")?.Destroy();
        GameObject.Find("RoleSettingsButton")?.Destroy();
        __instance.ChangeTab(1, false);
    }

    public static void CreateCustomButton(GameSettingMenu __instance, int targetMenu, string buttonName, string buttonText)
    {
        var leftPanel = GameObject.Find("LeftPanel");
        var buttonTemplate = GameObject.Find("GameSettingsButton");
        if (targetMenu == 3)
        {
            buttonTemplate.transform.localPosition -= Vector3.up * 0.85f;
            buttonTemplate.transform.localScale *= Vector2.one * 0.75f;
        }
        var modSettingsButton = GameObject.Find(buttonName);
        if (modSettingsButton == null)
        {
            modSettingsButton = UnityEngine.Object.Instantiate(buttonTemplate, leftPanel.transform);
            modSettingsButton.transform.localPosition += Vector3.up * 0.5f * (targetMenu - 2);
            modSettingsButton.name = buttonName;
            __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { modSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
            var modSettingsPassiveButton = modSettingsButton.GetComponent<PassiveButton>();
            modSettingsPassiveButton.OnClick.RemoveAllListeners();
            modSettingsPassiveButton.OnClick.AddListener((System.Action)(() =>
            {
                __instance.ChangeTab(targetMenu, false);
            }));
            modSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
            modSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
            modSettingsPassiveButton.SelectButton(false);
            SettingMenuCurrentButtons.Add(modSettingsPassiveButton);
        }
    }

    public static void CreateGameOptionsMenu(GameSettingMenu __instance, CustomOptionType optionType, string settingName)
    {
        var tabTemplate = GameObject.Find("GAME SETTINGS TAB");
        SettingMenuCurrentTabs.RemoveAll(x => x == null);

        var modSettingsTab = UnityEngine.Object.Instantiate(tabTemplate, tabTemplate.transform.parent);
        modSettingsTab.name = settingName;

        var modSettingsGOM = modSettingsTab.GetComponent<GameOptionsMenu>();

        UpdateGameOptionsMenu(optionType, modSettingsGOM);

        SettingMenuCurrentTabs.Add(modSettingsTab);
        modSettingsTab.SetActive(false);
        CurrentGOMs.Add((byte)optionType, modSettingsGOM);
    }

    public static void UpdateGameOptionsMenu(CustomOptionType optionType, GameOptionsMenu torSettingsGOM)
    {
        foreach (var child in torSettingsGOM.Children)
        {
            child.Destroy();
        }
        torSettingsGOM.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        torSettingsGOM.Children.Clear();
        var relevantOptions = AllOptions.Where(x => x.Type == optionType).ToList();
        if (MapOptions.GameMode == CustomGamemodes.Guesser) // Exclude guesser options in neutral mode
            relevantOptions = [.. relevantOptions.Where(x => !new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318 }.Contains(x.Id))];
        CreateSettings(torSettingsGOM, relevantOptions);
    }

    private static void CreateSettingTabs(GameSettingMenu __instance)
    {
        // Handle different gamemodes and tabs needed therein.
        int next = 3;
        if (MapOptions.GameMode == CustomGamemodes.Guesser || MapOptions.GameMode == CustomGamemodes.Classic)
        {

            // create RU settings
            CreateCustomButton(__instance, next++, "RUSettings", "RU Settings");
            CreateGameOptionsMenu(__instance, CustomOptionType.General, "RUSettings");
            // Guesser if applicable
            if (MapOptions.GameMode == CustomGamemodes.Guesser)
            {
                CreateCustomButton(__instance, next++, "GuesserSettings", "Guesser Settings");
                CreateGameOptionsMenu(__instance, CustomOptionType.Guesser, "GuesserSettings");
            }
            // IMp
            CreateCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Impostor, "ImpostorSettings");

            // Neutral
            CreateCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Neutral, "NeutralSettings");
            // Crew
            CreateCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Crewmate, "CrewmateSettings");
            // Modifier
            CreateCustomButton(__instance, next++, "ModifierSettings", "Modifiers");
            CreateGameOptionsMenu(__instance, CustomOptionType.Modifier, "ModifierSettings");

        }
        else if (MapOptions.GameMode == CustomGamemodes.HideNSeek)
        {
            // create Main HNS settings
            CreateCustomButton(__instance, next++, "HideNSeekMain", "Hide 'N' Seek");
            CreateGameOptionsMenu(__instance, CustomOptionType.HideNSeekMain, "HideNSeekMain");
            // create HNS Role settings
            CreateCustomButton(__instance, next++, "HideNSeekRoles", "Hide 'N' Seek Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.HideNSeekRoles, "HideNSeekRoles");
        }
        else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
        {
            CreateCustomButton(__instance, next++, "PropHunt", "Prop Hunt");
            CreateGameOptionsMenu(__instance, CustomOptionType.PropHunt, "PropHunt");
        }
    }

    public static bool StringOptionInitialize(StringOption __instance)
    {
        var option = AllOptions.FirstOrDefault(option => option.OptionBehavior == __instance);
        if (option == null) return true;

        __instance.OnValueChanged = new Action<OptionBehaviour>((o) => { });
        //__instance.TitleText.text = option.name;
        __instance.Value = __instance.oldValue = option.Selection;
        __instance.ValueText.text = option.Selections[option.Selection].ToString();

        return false;
    }

    public static bool StringOptionIncrease(StringOption __instance)
    {
        var option = AllOptions.FirstOrDefault(option => option.OptionBehavior == __instance);
        if (option == null) return true;
        option.UpdateSelection(option.Selection + 1);
        if (CustomOptionHolderHelpers.IsMapSelectionOption(option))
        {
            IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            currentGameOptions.SetByte(ByteOptionNames.MapId, (byte)option.Selection);
            GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
            GameManager.Instance.LogicOptions.SyncOptions();
        }
        return false;
    }

    public static bool StringOptionDecrease(StringOption __instance)
    {
        var option = AllOptions.FirstOrDefault(option => option.OptionBehavior == __instance);
        if (option == null) return true;
        option.UpdateSelection(option.Selection - 1);
        if (CustomOptionHolderHelpers.IsMapSelectionOption(option))
        {
            IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            currentGameOptions.SetByte(ByteOptionNames.MapId, (byte)option.Selection);
            GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
            GameManager.Instance.LogicOptions.SyncOptions();
        }
        return false;
    }

    public static void StringOptionFixedUpdate(StringOption __instance)
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out PluginInfo _)) return;
        CustomOption option = AllOptions.FirstOrDefault(option => option.OptionBehavior == __instance);
        if (option == null || !CustomOptionHolderHelpers.IsMapSelectionOption(option)) return;
        if (GameOptionsManager.Instance.CurrentGameOptions.MapId == 6)
        {
            if (option.OptionBehavior != null && option.OptionBehavior is StringOption stringOption)
            {
                stringOption.ValueText.text = option.Selections[option.Selection].ToString();
            }
            else if (option.OptionBehavior != null && option.OptionBehavior is StringOption stringOptionToo)
            {
                stringOptionToo.oldValue = stringOptionToo.Value = option.Selection;
                stringOptionToo.ValueText.text = option.Selections[option.Selection].ToString();
            }
        }
    }

    public static void CoSpawnSyncSettings()
    {
        if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost)
        {
            GameManager.Instance.LogicOptions.SyncOptions();
            ShareOptionSelections();
        }
    }

    private static string BuildRoleOptions()
    {
        var impRoles = BuildOptionsOfType(CustomOptionType.Impostor, true) + "\n";
        var neutralRoles = BuildOptionsOfType(CustomOptionType.Neutral, true) + "\n";
        var crewRoles = BuildOptionsOfType(CustomOptionType.Crewmate, true) + "\n";
        var modifiers = BuildOptionsOfType(CustomOptionType.Modifier, true);
        return impRoles + neutralRoles + crewRoles + modifiers;
    }
    public static string BuildModifierExtras(CustomOption customOption)
    {
        // find options children with quantity
        var children = AllOptions.Where(o => o.Parent == customOption);
        var quantity = children.Where(o => o.NameKey.Contains("Quantity")).ToList();
        if (customOption.GetSelection() == 0) return "";
        if (quantity.Count == 1) return $" ({quantity[0].GetQuantity()})";
        // if (customOption == CustomOptionHolder.modifierLover)
        // {
        //     return $" (1 Evil: {CustomOptionHolder.modifierLoverImpLoverRate.getSelection() * 10}%)";
        // }
        return "";
    }

    private static string BuildOptionsOfType(CustomOptionType type, bool headerOnly)
    {
        StringBuilder sb = new("\n");
        var options = AllOptions.Where(o => o.Type == type);
        if (MapOptions.GameMode == CustomGamemodes.Guesser)
        {
            if (type == CustomOptionType.General)
            {
                options = AllOptions.Where(o => o.Type == type || o.Type == CustomOptionType.Guesser);
            }
            List<int> remove = [308, 310, 311, 312, 313, 314, 315, 316, 317, 318];
            options = options.Where(x => !remove.Contains(x.Id));
        }
        else if (MapOptions.GameMode == CustomGamemodes.Classic)
        {
            options = options.Where(x => !(x.Type == CustomOptionType.Guesser));
        }
        else if (MapOptions.GameMode == CustomGamemodes.HideNSeek)
        {
            options = options.Where(x => x.Type == CustomOptionType.HideNSeekMain || x.Type == CustomOptionType.HideNSeekRoles);
        }
        else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
        {
            options = options.Where(x => x.Type == CustomOptionType.PropHunt);
        }

        foreach (var option in options)
        {
            if (option.Parent == null)
            {
                string line = $"{option.NameKey}: {option.Selections[option.Selection]}";
                if (type == CustomOptionType.Modifier) line += BuildModifierExtras(option);
                sb.AppendLine(line);
            }
            else if (option.Parent.GetSelection() > 0 || option.HideIfParentEnabled && option.Parent.GetSelection() == 0)
            {
                // if (option.id == 103) //Deputy
                //     sb.AppendLine($"- {Helpers.cs(Deputy.color, "Deputy")}: {option.selections[option.selection].ToString()}");
                // else if (option.id == 224) //Sidekick
                //     sb.AppendLine($"- {Helpers.cs(Sidekick.color, "Sidekick")}: {option.selections[option.selection].ToString()}");
                // else if (option.id == 358) //Prosecutor
                //     sb.AppendLine($"- {Helpers.cs(Lawyer.color, "Prosecutor")}: {option.selections[option.selection].ToString()}");
            }
        }
        if (headerOnly) return sb.ToString();
        else sb = new StringBuilder();

        foreach (CustomOption option in options)
        {
            if (MapOptions.GameMode == CustomGamemodes.HideNSeek && option.Type != CustomOptionType.HideNSeekMain && option.Type != CustomOptionType.HideNSeekRoles) continue;
            if (MapOptions.GameMode == CustomGamemodes.PropHunt && option.Type != CustomOptionType.PropHunt) continue;
            if (option.Parent != null)
            {
                bool isIrrelevant = (option.Parent.GetSelection() == 0 && !option.HideIfParentEnabled) || (option.Parent.Parent != null && option.Parent.Parent.GetSelection() == 0 && !option.Parent.HideIfParentEnabled);

                Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                if (isIrrelevant) continue;
                sb.AppendLine(Helpers.Cs(c, $"{option.NameKey}: {option.Selections[option.Selection]}"));
            }
            else
            {
                if (option == CustomOptionHolder.CrewmateRolesCountMin)
                {
                    var optionName = CustomOptionHolderHelpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Crewmate Roles");
                    var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
                    var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
                    string optionValue = "";
                    if (min > max) min = max;
                    optionValue += (min == max) ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.NeutralRolesCountMin)
                {
                    var optionName = CustomOptionHolderHelpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Neutral Roles");
                    var min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                    var max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                    if (min > max) min = max;
                    var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.ImpostorRolesCountMin)
                {
                    var optionName = CustomOptionHolderHelpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Impostor Roles");
                    var min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
                    var max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
                    if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors) max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                    if (min > max) min = max;
                    var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.ModifiersCountMin)
                {
                    var optionName = CustomOptionHolderHelpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Modifiers");
                    var min = CustomOptionHolder.ModifiersCountMin.GetSelection();
                    var max = CustomOptionHolder.ModifiersCountMax.GetSelection();
                    if (min > max) min = max;
                    var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if ((option == CustomOptionHolder.CrewmateRolesCountMax) || (option == CustomOptionHolder.NeutralRolesCountMax) || (option == CustomOptionHolder.ImpostorRolesCountMax) || option == CustomOptionHolder.ModifiersCountMax)
                {
                    continue;
                }
                else
                {
                    sb.AppendLine($"\n{option.NameKey}: {option.Selections[option.Selection].ToString()}");
                }
            }
        }
        return sb.ToString();
    }

    public static int MaxPage = 7;
    public static string BuildAllOptions(string vanillaSettings = "", bool hideExtras = false)
    {
        if (vanillaSettings == "")
            vanillaSettings = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
        int counter = RebuildUs.OptionsPage;
        string hudString = counter != 0 && !hideExtras ? Helpers.Cs(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "(Use scroll wheel if necessary)\n\n") : "";

        if (MapOptions.GameMode == CustomGamemodes.HideNSeek)
        {
            if (RebuildUs.OptionsPage > 1) RebuildUs.OptionsPage = 0;
            MaxPage = 2;
            switch (counter)
            {
                case 0:
                    hudString += "Page 1: Hide N Seek Settings \n\n" + BuildOptionsOfType(CustomOptionType.HideNSeekMain, false);
                    break;
                case 1:
                    hudString += "Page 2: Hide N Seek Role Settings \n\n" + BuildOptionsOfType(CustomOptionType.HideNSeekRoles, false);
                    break;
            }
        }
        else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
        {
            MaxPage = 1;
            switch (counter)
            {
                case 0:
                    hudString += "Page 1: Prop Hunt Settings \n\n" + BuildOptionsOfType(CustomOptionType.PropHunt, false);
                    break;
            }
        }
        else
        {
            MaxPage = 7;
            switch (counter)
            {
                case 0:
                    hudString += (!hideExtras ? "" : "Page 1: Vanilla Settings \n\n") + vanillaSettings;
                    break;
                case 1:
                    hudString += "Page 2: The Other Roles Settings \n" + BuildOptionsOfType(CustomOptionType.General, false);
                    break;
                case 2:
                    hudString += "Page 3: Role and Modifier Rates \n" + BuildRoleOptions();
                    break;
                case 3:
                    hudString += "Page 4: Impostor Role Settings \n" + BuildOptionsOfType(CustomOptionType.Impostor, false);
                    break;
                case 4:
                    hudString += "Page 5: Neutral Role Settings \n" + BuildOptionsOfType(CustomOptionType.Neutral, false);
                    break;
                case 5:
                    hudString += "Page 6: Crewmate Role Settings \n" + BuildOptionsOfType(CustomOptionType.Crewmate, false);
                    break;
                case 6:
                    hudString += "Page 7: Modifier Settings \n" + BuildOptionsOfType(CustomOptionType.Modifier, false);
                    break;
            }
        }

        if (!hideExtras || counter != 0) hudString += $"\n Press TAB or Page Number for more... ({counter + 1}/{MaxPage})";
        return hudString;
    }

    public static void ToHudString(ref string __result)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek
        __result = BuildAllOptions(__result);
    }
    public static bool LGOAreInvalid(LegacyGameOptions __instance, ref int maxExpectedPlayers)
    {
        //making the killdistances bound check higher since extra short is added
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count
                || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f;
    }

    public static bool NGO10AreInvalid(NormalGameOptionsV10 __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                || __instance.KillDistance >= LegacyGameOptions.KillDistances.Count
                || __instance.PlayerSpeedMod <= 0f || __instance.PlayerSpeedMod > 3f;
    }

    public static void StringOptionInitializePrefix(StringOption __instance)
    {
    }

    public static void StringOptionInitializePostfix(StringOption __instance)
    {
    }

    public static void AppendItem(ref StringNames stringName, ref string value)
    {
    }

    public static bool AdjustStringForViewPanel(StringGameSetting __instance, float value, ref string __result)
    {
        return true;
    }

    // public static string optionToString(CustomOption option)
    // {
    //     if (option == null) return "";
    //     return $"{option.GetName()}: {option.GetString()}";
    // }

    // public static string optionsToString(CustomOption option, bool skipFirst = false)
    // {
    //     if (option == null) return "";

    //     List<string> options = new();
    //     if (!skipFirst) options.Add(optionToString(option));
    //     if (option.enabled)
    //     {
    //         foreach (CustomOption op in option.children)
    //         {
    //             string str = optionsToString(op);
    //             if (str != "") options.Add(str);
    //         }
    //     }
    //     return string.Join("\n", options);
    // }

    public static void KeyboardUpdate(KeyboardJoystick __instance)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            RebuildUs.OptionsPage = (RebuildUs.OptionsPage + 1) % 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            RebuildUs.OptionsPage = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            RebuildUs.OptionsPage = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            RebuildUs.OptionsPage = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            RebuildUs.OptionsPage = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            RebuildUs.OptionsPage = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            RebuildUs.OptionsPage = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            RebuildUs.OptionsPage = 6;
        }
        if (Input.GetKeyDown(KeyCode.F1))
            HudSettingsManager.ToggleSettings(HudManager.Instance);
        if (Input.GetKeyDown(KeyCode.F2) && LobbyBehaviour.Instance)
            HudSettingsManager.ToggleSummary(HudManager.Instance);
        if (RebuildUs.OptionsPage >= MaxPage) RebuildUs.OptionsPage = 0;
    }
}