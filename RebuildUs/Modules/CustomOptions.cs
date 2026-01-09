using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using System.Text;
using RebuildUs.Utilities;
using static RebuildUs.RebuildUs;
using static RebuildUs.CustomOption;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using static ShipStatus;
using TMPro;
using Rewired.Utils.Platforms.Windows;
using static Il2CppSystem.Xml.Schema.FacetsChecker.FacetsCompiler;
using RebuildUs;
using RebuildUs.Modules.RPC;
using RebuildUs.Options;
using RebuildUs.Extensions;
using AmongUs.Data;

namespace RebuildUs
{
    public enum CustomGamemodes
    {
        Classic,
        Guesser,
        HideNSeek,
        PropHunt
    }
    public class CustomOption
    {
        public enum CustomOptionType
        {
            General,
            Impostor,
            Neutral,
            Crewmate,
            Modifier,
            Guesser,
            HideNSeekMain,
            HideNSeekRoles,
            PropHunt,
        }

        public static List<CustomOption> AllOptions = new();
        public static int Preset = 0;
        public static ConfigEntry<string> VanillaSettings;

        public int Id;
        public string Name;
        public System.Object[] Selections;

        public int DefaultSelection;
        public ConfigEntry<int> Entry;
        public int Selection;
        public OptionBehaviour OptionBehaviour;
        public CustomOption Parent;
        public bool IsHeader;
        public CustomOptionType Type;
        public Action OnChange = null;
        public string Heading = "";
        public bool InvertedParent;

        // Option creation

        public CustomOption(int id, CustomOptionType type, string name, System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, Action onChange = null, string heading = "", bool invertedParent = false)
        {
            this.Id = id;
            this.Name = parent == null ? name : "- " + name;
            this.Selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.DefaultSelection = index >= 0 ? index : 0;
            this.Parent = parent;
            this.IsHeader = isHeader;
            this.Type = type;
            this.OnChange = onChange;
            this.Heading = heading;
            this.InvertedParent = invertedParent;
            Selection = 0;
            if (id != 0)
            {
                Entry = RebuildUs.Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
                Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
            }
            AllOptions.Add(this);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", bool invertedParent = false)
        {
            return new CustomOption(id, type, name, selections, "", parent, isHeader, onChange, heading, invertedParent);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", bool invertedParent = false)
        {
            List<object> selections = new();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, type, name, [.. selections], defaultValue, parent, isHeader, onChange, heading, invertedParent);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", bool invertedParent = false)
        {
            return new CustomOption(id, type, name, new string[] { "Off", "On" }, defaultValue ? "On" : "Off", parent, isHeader, onChange, heading, invertedParent);
        }

        // Static behaviour

        public static void SwitchPreset(int newPreset)
        {
            SaveVanillaOptions();
            CustomOption.Preset = newPreset;
            VanillaSettings = RebuildUs.Instance.Config.Bind($"Preset{Preset}", "GameOptions", "");
            LoadVanillaOptions();
            foreach (CustomOption option in CustomOption.AllOptions)
            {
                if (option.Id == 0) continue;

                option.Entry = RebuildUs.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
                option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
                if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
                {
                    stringOption.oldValue = stringOption.Value = option.Selection;
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
            }

            // make sure to reload all tabs, even the ones in the background, because they might have changed when the preset was switched!
            if (AmongUsClient.Instance?.AmHost == true)
            {
                foreach (var entry in CurrentGOMs)
                {
                    CustomOptionType optionType = (CustomOptionType)entry.Key;
                    GameOptionsMenu gom = entry.Value;
                    if (gom != null)
                    {
                        UpdateGameOptionsMenu(optionType, gom);
                    }
                }
            }
        }

        public static void SaveVanillaOptions()
        {
            // TODO: エラー発生 修正余地絵
            // VanillaSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
        }

        public static bool LoadVanillaOptions()
        {
            string optionsString = VanillaSettings.Value;
            if (optionsString == "") return false;
            IGameOptions gameOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
            if (gameOptions.Version < 8)
            {
                Logger.LogMessage("tried to paste old settings, not doing this!");
                return false;
            }
            GameOptionsManager.Instance.GameHostOptions = gameOptions;
            GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
            GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
            GameManager.Instance.LogicOptions.SyncOptions();
            return true;
        }

        public static void ShareOptionChange(uint optionId)
        {
            var option = AllOptions.FirstOrDefault(x => x.Id == optionId);
            if (option == null) return;
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
            sender.Write((byte)1);
            sender.WritePacked((uint)option.Id);
            sender.WritePacked(Convert.ToUInt32(option.Selection));
        }

        public static void ShareOptionSelections()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance!.AmHost == false && PlayerControl.LocalPlayer == null) return;
            var optionsList = new List<CustomOption>(AllOptions);
            while (optionsList.Any())
            {
                byte amount = (byte)Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareOptions);
                sender.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = optionsList[0];
                    optionsList.RemoveAt(0);
                    sender.WritePacked((uint)option.Id);
                    sender.WritePacked(Convert.ToUInt32(option.Selection));
                }
            }
        }

        // Getter

        public int GetSelection()
        {
            return Selection;
        }

        public bool GetBool()
        {
            return Selection > 0;
        }

        public float GetFloat()
        {
            return (float)Selections[Selection];
        }

        public int GetQuantity()
        {
            return Selection + 1;
        }

        public void UpdateSelection(int newSelection, bool notifyUsers = true)
        {
            newSelection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
            if (AmongUsClient.Instance?.AmClient == true && notifyUsers && Selection != newSelection)
            {
                DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(this.Id + 6000), Selections[newSelection].ToString(), false);
                try
                {
                    Selection = newSelection;
                    if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                    {
                        CustomOption.SettingsPaneChangeTab(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                    }
                }
                catch { }
            }
            Selection = newSelection;
            try
            {
                if (OnChange != null) OnChange();
            }
            catch { }

            if (OptionBehaviour != null && OptionBehaviour is StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = Selection;
                stringOption.ValueText.text = Selections[Selection].ToString();
                if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
                {
                    if (Id == 0 && Selection != Preset)
                    {
                        SwitchPreset(Selection); // Switch presets
                        ShareOptionSelections();
                    }
                    else if (Entry != null)
                    {
                        Entry.Value = Selection; // Save selection to config
                        ShareOptionChange((uint)Id);// Share single selection
                    }
                }
            }
            else if (Id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
            {  // Share the preset switch for random maps, even if the menu isnt open!
                SwitchPreset(Selection);
                ShareOptionSelections();// Share all selections
            }

            if (AmongUsClient.Instance?.AmHost == true)
            {
                var currentTab = SettingMenuCurrentTabs.FirstOrDefault(x => x.active).GetComponent<GameOptionsMenu>();
                if (currentTab != null)
                {
                    var optionType = AllOptions.First(x => x.OptionBehaviour == currentTab.Children[0]).Type;
                    UpdateGameOptionsMenu(optionType, currentTab);
                }
            }
        }

        public static byte[] SerializeOptions()
        {
            using (MemoryStream memoryStream = new())
            {
                using (BinaryWriter binaryWriter = new(memoryStream))
                {
                    int lastId = -1;
                    foreach (var option in CustomOption.AllOptions.OrderBy(x => x.Id))
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
            }
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
                    if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
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

        // Copy to or paste from clipboard (as string)
        public static void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"{RebuildUs.MOD_VERSION}!{Convert.ToBase64String(SerializeOptions())}!{VanillaSettings.Value}";
        }

        public static int PasteFromClipboard()
        {
            string allSettings = GUIUtility.systemCopyBuffer;
            int torOptionsFine = 0;
            bool vanillaOptionsFine = false;
            try
            {
                var settingsSplit = allSettings.Split("!");
                Version versionInfo = Version.Parse(settingsSplit[0]);
                string modSettings = settingsSplit[1];
                string vanillaSettingsSub = settingsSplit[2];
                torOptionsFine = DeserializeOptions(Convert.FromBase64String(modSettings));
                ShareOptionSelections();
                if (RebuildUs.Instance.Version > versionInfo && versionInfo < Version.Parse("4.6.0"))
                {
                    vanillaOptionsFine = false;
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Host Info: Pasting vanilla settings failed, TOR Options applied!");
                }
                else
                {
                    VanillaSettings.Value = vanillaSettingsSub;
                    vanillaOptionsFine = LoadVanillaOptions();
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn($"{e}: tried to paste invalid settings!\n{allSettings}");
                string errorStr = allSettings.Length > 2 ? allSettings.Substring(0, 3) : "(empty clipboard) ";
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"Host Info: You tried to paste invalid settings: \"{errorStr}...\"");
                // SoundEffectsManager.Load();
                // SoundEffectsManager.play("fail");
            }
            return Convert.ToInt32(vanillaOptionsFine) + torOptionsFine;
        }

        public static void SettingMenuChangeTab(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (previewOnly) return;
            foreach (var tab in SettingMenuCurrentTabs)
            {
                tab?.SetActive(false);
            }
            foreach (var pbutton in SettingMenuCurrentButtons)
            {
                pbutton.SelectButton(false);
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

            foreach (var pbutton in SettingsPaneCurrentButtons)
            {
                pbutton.SelectButton(false);
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

        public static List<PassiveButton> SettingsPaneCurrentButtons = new();
        public static List<CustomOptionType> SettingsPaneCurrentButtonTypes = new();
        public static bool GameModeChangedFlag = false;

        public static void CreateCustomButton(LobbyViewSettingsPane __instance, int targetMenu, string buttonName, string buttonText, CustomOptionType optionType)
        {
            buttonName = "View" + buttonName;
            var buttonTemplate = GameObject.Find("OverviewTab");
            var modSettingsButton = GameObject.Find(buttonName);
            if (modSettingsButton == null)
            {
                modSettingsButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
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
            var relevantOptions = AllOptions.Where(x => x.Type == optionType || x.Type == CustomOption.CustomOptionType.Guesser && optionType == CustomOptionType.General).ToList();

            if ((int)optionType == 99)
            {
                // Create 4 Groups with Role settings only
                relevantOptions.Clear();
                relevantOptions.AddRange(CustomOption.AllOptions.Where(x => x.Type == CustomOptionType.Impostor && x.IsHeader));
                relevantOptions.AddRange(CustomOption.AllOptions.Where(x => x.Type == CustomOptionType.Neutral && x.IsHeader));
                relevantOptions.AddRange(CustomOption.AllOptions.Where(x => x.Type == CustomOptionType.Crewmate && x.IsHeader));
                relevantOptions.AddRange(CustomOption.AllOptions.Where(x => x.Type == CustomOptionType.Modifier && x.IsHeader));
                foreach (var option in CustomOption.AllOptions)
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
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                    categoryHeaderMasked.Title.text = option.Heading != "" ? option.Heading : option.Name;
                    if ((int)optionType == 99)
                        categoryHeaderMasked.Title.text = new Dictionary<CustomOptionType, string>() { { CustomOptionType.Impostor, "Impostor Roles" }, { CustomOptionType.Neutral, "Neutral Roles" },
                            { CustomOptionType.Crewmate, "Crewmate Roles" }, { CustomOptionType.Modifier, "Modifiers" } }[curType];
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.2f;
                    categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    i = 0;
                }
                else if (option.Parent != null && (option.Parent.Selection == 0 || option.Parent.Parent != null && option.Parent.Parent.Selection == 0)) continue;  // Hides options, for which the parent is disabled!
                if (option == CustomOptionHolder.CrewmateRolesCountMax || option == CustomOptionHolder.NeutralRolesCountMax || option == CustomOptionHolder.ImpostorRolesCountMax || option == CustomOptionHolder.ModifiersCountMax || option == CustomOptionHolder.CrewmateRolesFill)
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
                var settingTuple = HandleSpecialOptionsView(option, option.Name, option.Selections[value].ToString());
                viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, settingTuple.Item2, 61);
                viewSettingsInfoPanel.titleText.text = settingTuple.Item1;
                if (option.IsHeader && (int)optionType != 99 && option.Heading == "" && (option.Type == CustomOptionType.Neutral || option.Type == CustomOptionType.Crewmate || option.Type == CustomOptionType.Impostor || option.Type == CustomOptionType.Modifier))
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
            __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + singles * 2 + headers), 2f, 5f, actual_spacing);
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
                if (CustomOptionHolder.CrewmateRolesFill.GetBool())
                {
                    var crewCount = PlayerControl.AllPlayerControls.Count - GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                    int minNeutral = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                    int maxNeutral = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                    if (minNeutral > maxNeutral) minNeutral = maxNeutral;
                    min = crewCount - maxNeutral;
                    max = crewCount - minNeutral;
                    if (min < 0) min = 0;
                    if (max < 0) max = 0;
                    val = "Fill: ";
                }
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

                // create TOR settings
                CreateCustomButton(__instance, next++, "TORSettings", "TOR Settings", CustomOptionType.General);
                // create TOR settings
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

        public static List<GameObject> SettingMenuCurrentTabs = new();
        public static List<PassiveButton> SettingMenuCurrentButtons = new();
        public static Dictionary<byte, GameOptionsMenu> CurrentGOMs = new();
        public static void SettingMenuStart(GameSettingMenu __instance)
        {
            SettingMenuCurrentTabs.ForEach(x => x?.Destroy());
            SettingMenuCurrentButtons.ForEach(x => x?.Destroy());
            SettingMenuCurrentTabs = new();
            SettingMenuCurrentButtons = new();
            CurrentGOMs.Clear();

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            RemoveVanillaTabs(__instance);

            CreateSettingTabs(__instance);

            var gOMGameObject = GameObject.Find("GAME SETTINGS TAB");

            // create copy to clipboard and paste from clipboard buttons.
            var template = GameObject.Find("PlayerOptionsMenu(Clone)").transform.Find("CloseButton").gameObject;
            var holderGO = new GameObject("copyPasteButtonParent");
            var bgrenderer = holderGO.AddComponent<SpriteRenderer>();
            bgrenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.CopyPasteBG.png", 175f);
            holderGO.transform.SetParent(template.transform.parent, false);
            holderGO.transform.localPosition = template.transform.localPosition + new Vector3(-8.3f, 0.73f, -2f);
            holderGO.layer = template.layer;
            holderGO.SetActive(true);
            var copyButton = GameObject.Instantiate(template, holderGO.transform);
            copyButton.transform.localPosition = new Vector3(-0.3f, 0.02f, -2f);
            var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
            var copyButtonRenderer = copyButton.GetComponentInChildren<SpriteRenderer>();
            var copyButtonActiveRenderer = copyButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            copyButtonRenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Copy.png", 100f);
            copyButton.transform.GetChild(1).transform.localPosition = Vector3.zero;
            copyButtonActiveRenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.CopyActive.png", 100);
            copyButtonPassive.OnClick.RemoveAllListeners();
            copyButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            copyButtonPassive.OnClick.AddListener((System.Action)(() =>
            {
                CopyToClipboard();
                copyButtonRenderer.color = Color.green;
                copyButtonActiveRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) =>
                {
                    if (p > 0.95)
                    {
                        copyButtonRenderer.color = Color.white;
                        copyButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
            var pasteButton = GameObject.Instantiate(template, holderGO.transform);
            pasteButton.transform.localPosition = new Vector3(0.3f, 0.02f, -2f);
            var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
            var pasteButtonRenderer = pasteButton.GetComponentInChildren<SpriteRenderer>();
            var pasteButtonActiveRenderer = pasteButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            pasteButtonRenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Paste.png", 100f);
            pasteButtonActiveRenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.PasteActive.png", 100f);
            pasteButtonPassive.OnClick.RemoveAllListeners();
            pasteButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteButtonPassive.OnClick.AddListener((System.Action)(() =>
            {
                pasteButtonRenderer.color = Color.yellow;
                int success = PasteFromClipboard();
                pasteButtonRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                pasteButtonActiveRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) =>
                {
                    if (p > 0.95)
                    {
                        pasteButtonRenderer.color = Color.white;
                        pasteButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
        }

        private static void CreateSettings(GameOptionsMenu menu, List<CustomOption> options)
        {
            float num = 1.5f;
            foreach (CustomOption option in options)
            {
                if (option.IsHeader)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                    categoryHeaderMasked.Title.text = option.Heading != "" ? option.Heading : option.Name;
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.2f;
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                }
                else if (option.Parent != null && (option.Parent.Selection == 0 && !option.InvertedParent || option.Parent.Parent != null && option.Parent.Parent.Selection == 0 && !option.Parent.InvertedParent)) continue;  // Hides options, for which the parent is disabled!
                else if (option.Parent != null && option.Parent.Selection != 0 && option.InvertedParent) continue;
                OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
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
                    textMeshPro.fontMaterial.SetFloat("_Stencil", (float)20);
                }

                var stringOption = optionBehaviour as StringOption;
                stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                stringOption.TitleText.text = option.Name;
                if (option.IsHeader && option.Heading == "" && (option.Type == CustomOptionType.Neutral || option.Type == CustomOptionType.Crewmate || option.Type == CustomOptionType.Impostor || option.Type == CustomOptionType.Modifier))
                {
                    stringOption.TitleText.text = "Spawn Chance";
                }
                if (stringOption.TitleText.text.Length > 25)
                    stringOption.TitleText.fontSize = 2.2f;
                if (stringOption.TitleText.text.Length > 40)
                    stringOption.TitleText.fontSize = 2f;
                stringOption.Value = stringOption.oldValue = option.Selection;
                stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                option.OptionBehaviour = stringOption;

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
            var torSettingsButton = GameObject.Find(buttonName);
            if (torSettingsButton == null)
            {
                torSettingsButton = GameObject.Instantiate(buttonTemplate, leftPanel.transform);
                torSettingsButton.transform.localPosition += Vector3.up * 0.5f * (targetMenu - 2);
                torSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { torSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var torSettingsPassiveButton = torSettingsButton.GetComponent<PassiveButton>();
                torSettingsPassiveButton.OnClick.RemoveAllListeners();
                torSettingsPassiveButton.OnClick.AddListener((System.Action)(() =>
                {
                    __instance.ChangeTab(targetMenu, false);
                }));
                torSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                torSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                torSettingsPassiveButton.SelectButton(false);
                SettingMenuCurrentButtons.Add(torSettingsPassiveButton);
            }
        }

        public static void CreateGameOptionsMenu(GameSettingMenu __instance, CustomOptionType optionType, string settingName)
        {
            var tabTemplate = GameObject.Find("GAME SETTINGS TAB");
            SettingMenuCurrentTabs.RemoveAll(x => x == null);

            var torSettingsTab = GameObject.Instantiate(tabTemplate, tabTemplate.transform.parent);
            torSettingsTab.name = settingName;

            var torSettingsGOM = torSettingsTab.GetComponent<GameOptionsMenu>();

            UpdateGameOptionsMenu(optionType, torSettingsGOM);

            SettingMenuCurrentTabs.Add(torSettingsTab);
            torSettingsTab.SetActive(false);
            CurrentGOMs.Add((byte)optionType, torSettingsGOM);
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
                relevantOptions = [.. relevantOptions.Where(x => !(new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318 }).Contains(x.Id))];
            CreateSettings(torSettingsGOM, relevantOptions);
        }

        private static void CreateSettingTabs(GameSettingMenu __instance)
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;
            if (MapOptions.GameMode == CustomGamemodes.Guesser || MapOptions.GameMode == CustomGamemodes.Classic)
            {

                // create TOR settings
                CreateCustomButton(__instance, next++, "TORSettings", "TOR Settings");
                CreateGameOptionsMenu(__instance, CustomOptionType.General, "TORSettings");
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
            CustomOption option = CustomOption.AllOptions.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => { });
            //__instance.TitleText.text = option.name;
            __instance.Value = __instance.oldValue = option.Selection;
            __instance.ValueText.text = option.Selections[option.Selection].ToString();

            return false;
        }

        public static bool StringOptionIncrease(StringOption __instance)
        {
            CustomOption option = CustomOption.AllOptions.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.Selection + 1);
            if (CustomOptionHolder.IsMapSelectionOption(option))
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
            CustomOption option = CustomOption.AllOptions.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.Selection - 1);
            if (CustomOptionHolder.IsMapSelectionOption(option))
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
            CustomOption option = CustomOption.AllOptions.FirstOrDefault(option => option.OptionBehaviour == __instance);
            if (option == null || !CustomOptionHolder.IsMapSelectionOption(option)) return;
            if (GameOptionsManager.Instance.CurrentGameOptions.MapId == 6)
                if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOption)
                {
                    stringOption.ValueText.text = option.Selections[option.Selection].ToString();
                }
                else if (option.OptionBehaviour != null && option.OptionBehaviour is StringOption stringOptionToo)
                {
                    stringOptionToo.oldValue = stringOptionToo.Value = option.Selection;
                    stringOptionToo.ValueText.text = option.Selections[option.Selection].ToString();
                }
        }

        public static void SyncVanillaSettings()
        {
            //CustomOption.ShareOptionSelections();
            CustomOption.SaveVanillaOptions();
        }

        public static void CoSpawnSyncSettings()
        {
            if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost)
            {
                GameManager.Instance.LogicOptions.SyncOptions();
                CustomOption.ShareOptionSelections();
            }
        }

        private static string BuildRoleOptions()
        {
            var impRoles = BuildOptionsOfType(CustomOption.CustomOptionType.Impostor, true) + "\n";
            var neutralRoles = BuildOptionsOfType(CustomOption.CustomOptionType.Neutral, true) + "\n";
            var crewRoles = BuildOptionsOfType(CustomOption.CustomOptionType.Crewmate, true) + "\n";
            var modifiers = BuildOptionsOfType(CustomOption.CustomOptionType.Modifier, true);
            return impRoles + neutralRoles + crewRoles + modifiers;
        }
        public static string BuildModifierExtras(CustomOption customOption)
        {
            // find options children with quantity
            var children = CustomOption.AllOptions.Where(o => o.Parent == customOption);
            var quantity = children.Where(o => o.Name.Contains("Quantity")).ToList();
            if (customOption.GetSelection() == 0) return "";
            if (quantity.Count == 1) return $" ({quantity[0].GetQuantity()})";
            // if (customOption == CustomOptionHolder.modifierLover)
            // {
            //     return $" (1 Evil: {CustomOptionHolder.modifierLoverImpLoverRate.getSelection() * 10}%)";
            // }
            return "";
        }

        private static string BuildOptionsOfType(CustomOption.CustomOptionType type, bool headerOnly)
        {
            StringBuilder sb = new("\n");
            var options = CustomOption.AllOptions.Where(o => o.Type == type);
            if (MapOptions.GameMode == CustomGamemodes.Guesser)
            {
                if (type == CustomOption.CustomOptionType.General)
                    options = CustomOption.AllOptions.Where(o => o.Type == type || o.Type == CustomOption.CustomOptionType.Guesser);
                List<int> remove = new() { 308, 310, 311, 312, 313, 314, 315, 316, 317, 318 };
                options = options.Where(x => !remove.Contains(x.Id));
            }
            else if (MapOptions.GameMode == CustomGamemodes.Classic)
                options = options.Where(x => !(x.Type == CustomOption.CustomOptionType.Guesser || x == CustomOptionHolder.CrewmateRolesFill));
            else if (MapOptions.GameMode == CustomGamemodes.HideNSeek)
                options = options.Where(x => (x.Type == CustomOption.CustomOptionType.HideNSeekMain || x.Type == CustomOption.CustomOptionType.HideNSeekRoles));
            else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
                options = options.Where(x => (x.Type == CustomOption.CustomOptionType.PropHunt));

            foreach (var option in options)
            {
                if (option.Parent == null)
                {
                    string line = $"{option.Name}: {option.Selections[option.Selection]}";
                    if (type == CustomOption.CustomOptionType.Modifier) line += BuildModifierExtras(option);
                    sb.AppendLine(line);
                }
                else if (option.Parent.GetSelection() > 0 || option.InvertedParent && option.Parent.GetSelection() == 0)
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
                    bool isIrrelevant = (option.Parent.GetSelection() == 0 && !option.InvertedParent) || (option.Parent.Parent != null && option.Parent.Parent.GetSelection() == 0 && !option.Parent.InvertedParent);

                    Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                    if (isIrrelevant) continue;
                    sb.AppendLine(Helpers.Cs(c, $"{option.Name}: {option.Selections[option.Selection]}"));
                }
                else
                {
                    if (option == CustomOptionHolder.CrewmateRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Crewmate Roles");
                        var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
                        string optionValue = "";
                        if (CustomOptionHolder.CrewmateRolesFill.GetBool())
                        {
                            var crewCount = PlayerControl.AllPlayerControls.Count - GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                            int minNeutral = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                            int maxNeutral = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                            if (minNeutral > maxNeutral) minNeutral = maxNeutral;
                            min = crewCount - maxNeutral;
                            max = crewCount - minNeutral;
                            if (min < 0) min = 0;
                            if (max < 0) max = 0;
                            optionValue = "Fill: ";
                        }
                        if (min > max) min = max;
                        optionValue += (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.NeutralRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Neutral Roles");
                        var min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.ImpostorRolesCountMin)
                    {
                        var optionName = CustomOptionHolder.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Impostor Roles");
                        var min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
                        var max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
                        if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors) max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.ModifiersCountMin)
                    {
                        var optionName = CustomOptionHolder.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Modifiers");
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
                        sb.AppendLine($"\n{option.Name}: {option.Selections[option.Selection].ToString()}");
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
                        hudString += "Page 1: Hide N Seek Settings \n\n" + BuildOptionsOfType(CustomOption.CustomOptionType.HideNSeekMain, false);
                        break;
                    case 1:
                        hudString += "Page 2: Hide N Seek Role Settings \n\n" + BuildOptionsOfType(CustomOption.CustomOptionType.HideNSeekRoles, false);
                        break;
                }
            }
            else if (MapOptions.GameMode == CustomGamemodes.PropHunt)
            {
                MaxPage = 1;
                switch (counter)
                {
                    case 0:
                        hudString += "Page 1: Prop Hunt Settings \n\n" + BuildOptionsOfType(CustomOption.CustomOptionType.PropHunt, false);
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
                        hudString += "Page 2: The Other Roles Settings \n" + BuildOptionsOfType(CustomOption.CustomOptionType.General, false);
                        break;
                    case 2:
                        hudString += "Page 3: Role and Modifier Rates \n" + BuildRoleOptions();
                        break;
                    case 3:
                        hudString += "Page 4: Impostor Role Settings \n" + BuildOptionsOfType(CustomOption.CustomOptionType.Impostor, false);
                        break;
                    case 4:
                        hudString += "Page 5: Neutral Role Settings \n" + BuildOptionsOfType(CustomOption.CustomOptionType.Neutral, false);
                        break;
                    case 5:
                        hudString += "Page 6: Crewmate Role Settings \n" + BuildOptionsOfType(CustomOption.CustomOptionType.Crewmate, false);
                        break;
                    case 6:
                        hudString += "Page 7: Modifier Settings \n" + BuildOptionsOfType(CustomOption.CustomOptionType.Modifier, false);
                        break;
                }
            }

            if (!hideExtras || counter != 0) hudString += $"\n Press TAB or Page Number for more... ({counter + 1}/{MaxPage})";
            return hudString;
        }

        public static void ToHudString(ref string __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek
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
            //prevents indexoutofrange exception breaking the setting if long happens to be selected
            //when host opens the laptop
            if (__instance.Title == StringNames.GameKillDistance && __instance.Value == 3)
            {
                __instance.Value = 1;
                GameOptionsManager.Instance.currentNormalGameOptions.KillDistance = 1;
                GameManager.Instance.LogicOptions.SyncOptions();
            }
        }

        public static void StringOptionInitializePostfix(StringOption __instance)
        {
            if (__instance.Title == StringNames.GameKillDistance && __instance.Values.Count == 3)
            {
                __instance.Values = new(
                        [(StringNames)49999, StringNames.SettingShort, StringNames.SettingMedium, StringNames.SettingLong]);
            }
        }

        public static void AppendItem(ref StringNames stringName, ref string value)
        {
            if (stringName == StringNames.GameKillDistance)
            {
                int index = GameOptionsManager.Instance.currentGameMode == GameModes.Normal
                    ? GameOptionsManager.Instance.currentNormalGameOptions.KillDistance
                    : GameOptionsManager.Instance.currentHideNSeekGameOptions.KillDistance;
                value = LegacyGameOptions.KillDistanceStrings[index];
            }
        }

        public static bool VeryShortPatch(ref string __result, ref StringNames id)
        {
            if ((int)id == 49999)
            {
                __result = "Very Short";
                return false;
            }
            return true;
        }

        public static void AddKillDistance()
        {
            LegacyGameOptions.KillDistances = new([0.5f, 1f, 1.8f, 2.5f]);
            LegacyGameOptions.KillDistanceStrings = new(["Very Short", "Short", "Medium", "Long"]);
        }

        public static bool AdjustStringForViewPanel(StringGameSetting __instance, float value, ref string __result)
        {
            if (__instance.OptionName != Int32OptionNames.KillDistance) return true;
            __result = LegacyGameOptions.KillDistanceStrings[(int)value];
            return false;
        }

        public static void KeyboardUpdate(KeyboardJoystick __instance)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OptionsPage = (OptionsPage + 1) % 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                OptionsPage = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                OptionsPage = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                OptionsPage = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                OptionsPage = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                OptionsPage = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                OptionsPage = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                OptionsPage = 6;
            }
            if (Input.GetKeyDown(KeyCode.F1))
                HudSettingsManager.ToggleSettings(HudManager.Instance);
            if (Input.GetKeyDown(KeyCode.F2) && LobbyBehaviour.Instance)
                HudSettingsManager.ToggleSummary(HudManager.Instance);
            if (OptionsPage >= MaxPage) OptionsPage = 0;
        }

        public class HudSettingsManager
        {
            private static readonly GameObject GameSettingsObject;
            private static readonly TextMeshPro GameSettings;

            public static float
                MinX,/*-5.3F*/
                OriginalY = 2.9F,
                MinY = 2.9F;

            public static Scroller Scroller;
            private static Vector3 LastPosition;
            private static float LastAspect;
            private static bool SetLastPosition = false;

            public static void UpdateScrollerPosition(HudManager __instance)
            {
                if (GameSettings?.transform == null) return;

                // Sets the MinX position to the left edge of the screen + 0.1 units
                Rect safeArea = Screen.safeArea;
                float aspect = Mathf.Min((Camera.main).aspect, safeArea.width / safeArea.height);
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                MinX = 0.1f - safeOrthographicSize * aspect;

                if (!SetLastPosition || aspect != LastAspect)
                {
                    LastPosition = new Vector3(MinX, MinY);
                    LastAspect = aspect;
                    SetLastPosition = true;
                    Scroller?.ContentXBounds = new FloatRange(MinX, MinX);
                }

                CreateScroller(__instance);

                Scroller.gameObject.SetActive(GameSettings.gameObject.activeSelf);

                if (!Scroller.gameObject.active) return;

                var rows = GameSettings.text.Count(c => c == '\n');
                float lobbyTextRowHeight = 0.06F;
                var maxY = Mathf.Max(MinY, rows * lobbyTextRowHeight + (rows - 38) * lobbyTextRowHeight);

                Scroller.ContentYBounds = new FloatRange(MinY, maxY);

                // Prevent scrolling when the player is interacting with a menu
                if (PlayerControl.LocalPlayer.CanMove != true)
                {
                    GameSettings.transform.localPosition = LastPosition;

                    return;
                }

                if (GameSettings.transform.localPosition.x != MinX ||
                    GameSettings.transform.localPosition.y < MinY) return;

                LastPosition = GameSettings.transform.localPosition;
            }

            private static void CreateScroller(HudManager __instance)
            {
                if (Scroller != null) return;

                Transform target = GameSettings.transform;

                Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
                Scroller.transform.SetParent(GameSettings.transform.parent);
                Scroller.gameObject.layer = 5;

                Scroller.transform.localScale = Vector3.one;
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.active = true;
                Scroller.velocity = new Vector2(0, 0);
                Scroller.ScrollbarYBounds = new FloatRange(0, 0);
                Scroller.ContentXBounds = new FloatRange(MinX, MinX);
                Scroller.enabled = true;

                Scroller.Inner = target;
                target.SetParent(Scroller.transform);
            }

            public static void UpdateHudSettings(HudManager __instance)
            {
                if (!SettingsTMPs[0]) return;
                foreach (var tmp in SettingsTMPs) tmp.text = "";
                var settingsString = BuildAllOptions(hideExtras: true);
                var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries); ;
                string curString = "";
                string curBlock;
                int j = 0;
                for (int i = 0; i < blocks.Length; i++)
                {
                    curBlock = blocks[i];
                    if (Helpers.LineCount(curBlock) + Helpers.LineCount(curString) < 43)
                    {
                        curString += curBlock + "\n\n";
                    }
                    else
                    {
                        SettingsTMPs[j].text = curString;
                        j++;

                        curString = "\n" + curBlock + "\n\n";
                        if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
                    }
                }
                if (j < SettingsTMPs.Length) SettingsTMPs[j].text = curString;
                int blockCount = 0;
                foreach (var tmp in SettingsTMPs)
                {
                    if (tmp.text != "")
                        blockCount++;
                }
                for (int i = 0; i < blockCount; i++)
                {
                    SettingsTMPs[i].transform.localPosition = new Vector3(-blockCount * 1.2f + 2.7f * i, 2.2f, -500f);
                }
            }

            private static readonly TMPro.TextMeshPro[] SettingsTMPs = new TMPro.TextMeshPro[4];
            private static GameObject SettingsBackground;
            public static void OpenSettings(HudManager __instance)
            {
                if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) return;
                if (SummaryTMP)
                {
                    CloseSummary();
                }
                SettingsBackground = GameObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
                SettingsBackground.SetActive(true);
                var renderer = SettingsBackground.GetComponent<SpriteRenderer>();
                renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                renderer.enabled = true;

                for (int i = 0; i < SettingsTMPs.Length; i++)
                {
                    SettingsTMPs[i] = GameObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                    SettingsTMPs[i].alignment = TMPro.TextAlignmentOptions.TopLeft;
                    SettingsTMPs[i].enableWordWrapping = false;
                    SettingsTMPs[i].transform.localScale = Vector3.one * 0.25f;
                    SettingsTMPs[i].gameObject.SetActive(true);
                }
            }

            public static void CloseSettings()
            {
                foreach (var tmp in SettingsTMPs)
                    if (tmp) tmp.gameObject.Destroy();

                if (SettingsBackground) SettingsBackground.Destroy();
            }

            public static void ToggleSettings(HudManager __instance)
            {
                if (SettingsTMPs[0]) CloseSettings();
                else OpenSettings(__instance);
            }

            public static void UpdateEndGameSummary(HudManager __instance)
            {
                if (!SummaryTMP) return;
                SummaryTMP.text = Helpers.PreviousEndGameSummary;
                SummaryTMP.transform.localPosition = new Vector3(-3 * 1.2f, 2.2f, -500f);
            }

            private static TMPro.TextMeshPro SummaryTMP = null;
            private static GameObject SummaryBackground;
            public static void OpenSummary(HudManager __instance)
            {
                if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen || Helpers.PreviousEndGameSummary.IsNullOrWhiteSpace()) return;
                if (SettingsTMPs[0])
                {
                    CloseSettings();
                }
                SummaryBackground = GameObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
                SummaryBackground.SetActive(true);
                var renderer = SummaryBackground.GetComponent<SpriteRenderer>();
                renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                renderer.enabled = true;

                SummaryTMP = GameObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                SummaryTMP.alignment = TMPro.TextAlignmentOptions.TopLeft;
                SummaryTMP.enableWordWrapping = false;
                SummaryTMP.transform.localScale = Vector3.one * 0.3f;
                SummaryTMP.gameObject.SetActive(true);
            }

            public static void CloseSummary()
            {
                SummaryTMP?.gameObject.Destroy();
                SummaryTMP = null;
                if (SummaryBackground) SummaryBackground.Destroy();
            }

            public static void ToggleSummary(HudManager __instance)
            {
                if (SummaryTMP) CloseSummary();
                else OpenSummary(__instance);
            }

            static PassiveButton ToggleSettingsButton;
            static GameObject ToggleSettingsButtonObject;

            static PassiveButton ToggleSummaryButton;
            static GameObject ToggleSummaryButtonObject;

            static GameObject ToggleZoomButtonObject;
            static PassiveButton ToggleZoomButton;

            public static void UpdateHudButtons(HudManager __instance)
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
                if (!ToggleSettingsButton || !ToggleSettingsButtonObject)
                {
                    // add a special button for settings viewing:
                    ToggleSettingsButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                    ToggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                    ToggleSettingsButtonObject.name = "TOGGLESETTINGSBUTTON";
                    SpriteRenderer renderer = ToggleSettingsButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                    SpriteRenderer rendererActive = ToggleSettingsButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                    ToggleSettingsButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                    renderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Settings_Button.png", 100f);
                    rendererActive.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Settings_ButtonActive.png", 100);
                    ToggleSettingsButton = ToggleSettingsButtonObject.GetComponent<PassiveButton>();
                    ToggleSettingsButton.OnClick.RemoveAllListeners();
                    ToggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
                }
                ToggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek);
                ToggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.8f, -500f);

                if (!ToggleZoomButton || !ToggleZoomButtonObject)
                {
                    // add a special button for settings viewing:
                    ToggleZoomButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                    ToggleZoomButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                    ToggleZoomButtonObject.name = "TOGGLEZOOMBUTTON";
                    SpriteRenderer tZrenderer = ToggleZoomButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                    SpriteRenderer tZArenderer = ToggleZoomButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                    ToggleZoomButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                    tZrenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Minus_Button.png", 100f);
                    tZArenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Minus_ButtonActive.png", 100);
                    ToggleZoomButton = ToggleZoomButtonObject.GetComponent<PassiveButton>();
                    ToggleZoomButton.OnClick.RemoveAllListeners();
                    ToggleZoomButton.OnClick.AddListener((Action)(() => Helpers.ToggleZoom()));
                }
                var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
                int numberOfLeftTasks = playerTotal - playerCompleted;
                bool zoomButtonActive = !(PlayerControl.LocalPlayer == null || !PlayerControl.LocalPlayer.Data.IsDead || (PlayerControl.LocalPlayer.Data.Role.IsImpostor && !CustomOptionHolder.DeadImpsBlockSabotage.GetBool()) || MeetingHud.Instance);
                zoomButtonActive &= numberOfLeftTasks <= 0 || !CustomOptionHolder.FinishTasksBeforeHauntingOrZoomingOut.GetBool();
                ToggleZoomButtonObject.SetActive(zoomButtonActive);
                var posOffset = Helpers.ZoomOutStatus ? new Vector3(-1.27f, -7.92f, -52f) : new Vector3(0, -1.6f, -52f);
                ToggleZoomButtonObject.transform.localPosition = HudManager.Instance.MapButton.transform.localPosition + posOffset;
            }

            public static void ToggleSummaryButtonHandler(HudManager __instance)
            {
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    if (ToggleSummaryButtonObject != null)
                    {
                        ToggleSummaryButtonObject.SetActive(false);
                        ToggleSummaryButtonObject.Destroy();
                        ToggleSummaryButton.Destroy();
                    }
                    return;
                }
                if (!ToggleSummaryButton || !ToggleSummaryButtonObject)
                {
                    // add a special button for settings viewing:
                    ToggleSummaryButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                    ToggleSummaryButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                    ToggleSummaryButtonObject.name = "TOGGLESUMMARYSBUTTON";
                    SpriteRenderer renderer = ToggleSummaryButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                    SpriteRenderer rendererActive = ToggleSummaryButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                    ToggleSummaryButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                    renderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Endscreen.png", 100f);
                    rendererActive.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.EndscreenActive.png", 100f);
                    ToggleSummaryButton = ToggleSummaryButtonObject.GetComponent<PassiveButton>();
                    ToggleSummaryButton.OnClick.RemoveAllListeners();
                    ToggleSummaryButton.OnClick.AddListener((Action)(() => ToggleSummary(__instance)));
                }
                ToggleSummaryButtonObject.SetActive(__instance.SettingsButton.gameObject.active && LobbyBehaviour.Instance && !Helpers.PreviousEndGameSummary.IsNullOrWhiteSpace() && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek
                    && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started);
                ToggleSummaryButtonObject.transform.localPosition = __instance.SettingsButton.transform.localPosition + new Vector3(-1.45f, 0.03f, -500f);
            }
        }
    }
}