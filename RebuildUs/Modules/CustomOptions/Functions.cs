// TODO: 設定画面開いたときに説明文章が更新されない問題があります

using AmongUs.GameOptions;
using RebuildUs.Options;
using RebuildUs.Extensions;
using System.Text;
using Epic.OnlineServices.RTC;
using UnityEngine.ProBuilder;
using System.Collections;

namespace RebuildUs.Modules.CustomOptions;

public enum OptionPage : int
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

public partial class CustomOption
{
    private static GameObject GeneralButton;
    private static GameObject ImpostorButton;
    private static GameObject CrewmateButton;
    private static GameObject NeutralButton;
    private static GameObject ModifierButton;
    private static GameObject GeneralTab;
    private static GameObject ImpostorTab;
    private static GameObject CrewmateTab;
    private static GameObject NeutralTab;
    private static GameObject ModifierTab;

    public static bool ChangeTabPrefix(GameSettingMenu __instance, OptionPage tabNum, bool previewOnly)
    {
        if (GeneralTab == null) return true;

        if (previewOnly && Controller.currentTouchType == Controller.TouchType.Joystick || !previewOnly)
        {
            __instance.PresetsTab.gameObject.SetActive(false);
            __instance.GameSettingsTab.gameObject.SetActive(false);
            __instance.RoleSettingsTab.gameObject.SetActive(false);
            GeneralTab.SetActive(false);
            ImpostorTab.SetActive(false);
            CrewmateTab.SetActive(false);
            NeutralTab.SetActive(false);
            ModifierTab.SetActive(false);
            __instance.GamePresetsButton.SelectButton(false);
            __instance.GameSettingsButton.SelectButton(false);
            __instance.RoleSettingsButton.SelectButton(false);
            GeneralButton.GetComponent<PassiveButton>().SelectButton(false);
            ImpostorButton.GetComponent<PassiveButton>().SelectButton(false);
            CrewmateButton.GetComponent<PassiveButton>().SelectButton(false);
            NeutralButton.GetComponent<PassiveButton>().SelectButton(false);
            ModifierButton.GetComponent<PassiveButton>().SelectButton(false);
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
                    GeneralTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = "This is general settings of RebuildUs.";
                    break;
                case OptionPage.ImpostorSettings:
                    ImpostorTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = "This is impostor roles settings of RebuildUs.";
                    break;
                case OptionPage.CrewmateSettings:
                    CrewmateTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = "This is crewmate roles settings of RebuildUs.";
                    break;
                case OptionPage.NeutralSettings:
                    NeutralTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = "This is neutral roles settings of RebuildUs.";
                    break;
                case OptionPage.ModifierSettings:
                    ModifierTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = "This is modifier settings of RebuildUs.";
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
                    GeneralTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    GeneralButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.ImpostorSettings:
                    ImpostorTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    ImpostorButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.CrewmateSettings:
                    CrewmateTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    CrewmateButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.NeutralSettings:
                    NeutralTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    NeutralButton.GetComponent<PassiveButton>().SelectButton(true);
                    break;
                case OptionPage.ModifierSettings:
                    ModifierTab.GetComponent<GameOptionsMenu>().OpenMenu();
                    ModifierButton.GetComponent<PassiveButton>().SelectButton(true);
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

    public static void OnEnablePrefix(GameSettingMenu __instance)
    {
        ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.GameSettingsButton, __instance.ControllerSelectable);
        if (Controller.currentTouchType != Controller.TouchType.Joystick)
        {
            __instance.ChangeTab((int)OptionPage.GameSettings, Controller.currentTouchType == Controller.TouchType.Joystick);
        }
        __instance.StartCoroutine(__instance.CoSelectDefault());
    }

    public static void SettingMenuStart(GameSettingMenu __instance)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        // Initialize vanilla tabs
        __instance.ChangeTab((int)OptionPage.GameSettings, false);

        // disable vanilla buttons
        __instance.GamePresetsButton.gameObject.SetActive(false);
        __instance.RoleSettingsButton.gameObject.SetActive(false);

        var gameSettingsButton = __instance.GameSettingsButton;
        var leftPanel = gameSettingsButton.gameObject.transform.parent.parent.FindEx("LeftPanel");
        var gameSettingsLabel = leftPanel.parent.gameObject.transform.FindEx("GameSettingsLabel");
        var whatIsThis = leftPanel.parent.gameObject.transform.FindEx("What Is This?");

        gameSettingsLabel.gameObject.SetActive(false);
        whatIsThis.transform.localPosition = new(whatIsThis.transform.localPosition.x - 0.4f, whatIsThis.transform.localPosition.y + 0.9f, whatIsThis.transform.localPosition.z);
        whatIsThis.transform.localScale *= Vector2.one * 0.9f;

        // Game Settings
        gameSettingsButton.transform.localPosition = new(gameSettingsButton.transform.localPosition.x - 0.2f, gameSettingsButton.transform.localPosition.y + 1.65f, gameSettingsButton.transform.localPosition.z);
        gameSettingsButton.transform.localScale *= Vector2.one * 0.75f;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { gameSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "AmongUs Settings"; })));
        gameSettingsButton.OnMouseOut.RemoveAllListeners();
        gameSettingsButton.OnMouseOver.RemoveAllListeners();
        gameSettingsButton.SelectButton(false);

        // Mod General Settings
        GeneralButton = CreateSettingButton(__instance, "RUGeneralSettingsButton", "RebuildUs Settings", OptionPage.GeneralSettings);
        GeneralTab = CreateSettingTab(__instance, "RUGeneralSettingsTab", CustomOptionType.General);
        ImpostorButton = CreateSettingButton(__instance, "RUImpostorSettingsButton", "Impostor Roles", OptionPage.ImpostorSettings);
        ImpostorTab = CreateSettingTab(__instance, "RUGeneralImpostorTab", CustomOptionType.Impostor);
        CrewmateButton = CreateSettingButton(__instance, "RUCrewmateSettingsButton", "Crewmate Roles", OptionPage.CrewmateSettings);
        CrewmateTab = CreateSettingTab(__instance, "RUCrewmateSettingsTab", CustomOptionType.Crewmate);
        NeutralButton = CreateSettingButton(__instance, "RUNeutralSettingsButton", "Neutral Roles", OptionPage.NeutralSettings);
        NeutralTab = CreateSettingTab(__instance, "RUNeutralSettingsTab", CustomOptionType.Neutral);
        ModifierButton = CreateSettingButton(__instance, "RUModifierSettingsButton", "Modifier Settings", OptionPage.ModifierSettings);
        ModifierTab = CreateSettingTab(__instance, "RUModifierSettingsTab", CustomOptionType.Modifier);

        __instance.GameSettingsButton.SelectButton(true);
        __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameSettingsDescription);
    }

    private static GameObject CreateSettingButton(GameSettingMenu __instance, string name, string buttontext, OptionPage id)
    {
        var template = __instance.GameSettingsButton.gameObject;
        var buttonObj = UnityEngine.Object.Instantiate(template, template.transform.parent);
        buttonObj.transform.localPosition += Vector3.down * 0.5f * ((int)id - 2); // 場所調整
        buttonObj.name = name;
        __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { buttonObj.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttontext; })));
        var buttonPB = buttonObj.GetComponent<PassiveButton>();
        buttonPB.OnClick.RemoveAllListeners();
        buttonPB.OnClick.AddListener((Action)(() =>
        {
            __instance.ChangeTab((int)id, false);
        }));
        buttonPB.OnMouseOut.RemoveAllListeners();
        buttonPB.OnMouseOver.RemoveAllListeners();
        buttonPB.SelectButton(false);

        return buttonObj;
    }

    private static GameObject CreateSettingTab(GameSettingMenu __instance, string name, CustomOptionType optionType)
    {
        var template = __instance.GameSettingsTab.gameObject;
        var tabObj = UnityEngine.Object.Instantiate(template, template.transform.parent);
        tabObj.name = name;

        var gameOptionsMenu = tabObj.GetComponent<GameOptionsMenu>();
        gameOptionsMenu.Children ??= new();
        foreach (var child in gameOptionsMenu.Children)
        {
            child.Destroy();
        }
        gameOptionsMenu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        gameOptionsMenu.Children.Clear();
        var relevantOptions = AllOptions.Where(x => x.Type == optionType).ToList();
        CreateSettingsNew(gameOptionsMenu, relevantOptions);

        tabObj.SetActive(false);
        return tabObj;
    }

    private static void CreateSettingsNew(GameOptionsMenu menu, List<CustomOption> options)
    {
        var num = 1.5f;
        foreach (var option in options)
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
            else if (option.Parent != null
                    && (option.Parent.Selection == 0
                    && !option.HideIfParentEnabled || option.Parent.Parent != null
                    && option.Parent.Parent.Selection == 0 && !option.Parent.HideIfParentEnabled)
                )
            {
                continue;  // Hides options, for which the parent is disabled!
            }
            else if (option.Parent != null
                    && option.Parent.Selection != 0
                    && option.HideIfParentEnabled
                )
            {
                continue;
            }

            var ob = UnityEngine.Object.Instantiate(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
            ob.transform.localPosition = new Vector3(0.952f, num, -2f);
            ob.SetClickMask(menu.ButtonClickMask);

            // "SetUpFromData"
            var renderer = ob.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderer.Length; i++)
            {
                renderer[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
            }
            foreach (var tmp in ob.GetComponentsInChildren<TextMeshPro>(true))
            {
                tmp.fontMaterial.SetFloat("_StencilComp", 3f);
                tmp.fontMaterial.SetFloat("_Stencil", 20);
            }

            var so = ob;
            so.OnValueChanged = new Action<OptionBehaviour>((o) => { });
            so.TitleText.text = option.NameKey;
            if (option.IsHeader
                && option.HeaderText == ""
                && (option.Type is CustomOptionType.Neutral or CustomOptionType.Crewmate or CustomOptionType.Impostor or CustomOptionType.Modifier)
            )
            {
                so.TitleText.text = "Spawn Chance";
            }

            if (so.TitleText.text.Length > 25)
            {
                so.TitleText.fontSize = 2.2f;
            }

            if (so.TitleText.text.Length > 40)
            {
                so.TitleText.fontSize = 2f;
            }

            so.Value = so.oldValue = option.Selection;
            so.ValueText.text = option.Selections[option.Selection].ToString();
            option.OptionBehavior = so;

            menu.Children.Add(ob);
            num -= 0.45f;
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }

        for (int i = 0; i < menu.Children.Count; i++)
        {
            var ob = menu.Children[i];
            if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
            {
                ob.SetAsPlayer();
            }
        }
    }

    public static void UpdateGameOptionsMenu(CustomOptionType optionType, GameOptionsMenu menu)
    {
        foreach (var child in menu.Children)
        {
            child.Destroy();
        }
        menu.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
        menu.Children.Clear();
        var options = new List<CustomOption>();
        foreach (var option in AllOptions)
        {
            if (option.Type == optionType)
            {
                options.Add(option);
            }
        }
        CreateSettingsNew(menu, options);
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
                    hudString += "Page 2: RebuildUs Settings \n" + BuildOptionsOfType(CustomOptionType.General, false);
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
        if (RebuildUs.OptionsPage >= MaxPage) RebuildUs.OptionsPage = 0;
    }
}