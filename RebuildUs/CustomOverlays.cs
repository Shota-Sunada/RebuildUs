using System.Text;

namespace RebuildUs;

public class CustomOverlays
{
    private static SpriteRenderer MeetingUnderlay;
    private static SpriteRenderer InfoUnderlay;
    private static TextMeshPro InfoOverlayRules;
    private static TextMeshPro InfoOverlayRulesRight;
    public static bool OverlayShown = false;
    public static int RolePage = 0;
    public static int MaxRolePage = 0;
    private static List<string> RoleData;
    private static List<string> OptionsData;
    public static int MaxOptionsPage = 0;

    private const int MaxLines = 28;

    private static int CountLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int count = 1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n') count++;
        }
        return count;
    }

    private static List<string> SplitToPages(string text, int maxLines)
    {
        var pages = new List<string>();
        var lines = text.Replace("\r\n", "\n").Split('\n');
        var currentPage = new StringBuilder();
        var currentLineCount = 0;

        foreach (var line in lines)
        {
            if (currentLineCount >= maxLines)
            {
                pages.Add(currentPage.ToString().TrimEnd());
                currentPage.Clear();
                currentLineCount = 0;
            }
            currentPage.Append(line).Append('\n');
            currentLineCount++;
        }

        if (currentPage.Length > 0)
        {
            pages.Add(currentPage.ToString().TrimEnd());
        }

        return pages;
    }

    public static void SetInfoOverlayText()
    {
        if (OptionsData == null || RebuildUs.OptionsPage < 0 || RebuildUs.OptionsPage >= OptionsData.Count) return;

        var sb = new StringBuilder();
        // sb.Append("<size=150%>ゲーム設定</size> <size=100%>現在のページ (").Append((RebuildUs.OptionsPage / 2) + 1).Append('/').Append((MaxOptionsPage + 1) / 2).Append(")</size>\n");
        sb.Append(OptionsData[RebuildUs.OptionsPage]);
        InfoOverlayRules.text = sb.ToString();

        if (RebuildUs.OptionsPage + 1 < OptionsData.Count)
        {
            InfoOverlayRulesRight.text = OptionsData[RebuildUs.OptionsPage + 1];
        }
        else
        {
            InfoOverlayRulesRight.text = "";
        }
    }

    private static void AppendRoleCount(StringBuilder sb, string key, CustomOption minOpt, CustomOption maxOpt)
    {
        var min = minOpt.GetSelection();
        var max = maxOpt.GetSelection();
        if (min > max) min = max;

        sb.Append(Helpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), Tr.Get(key))).Append(": ");
        if (min == max) sb.Append(max);
        else sb.Append(min).Append(" - ").Append(max);
        sb.AppendLine();
    }

    public static void ResetOverlays()
    {
        HideBlackBG();
        HideInfoOverlay();
        if (MeetingUnderlay != null) UnityEngine.Object.Destroy(MeetingUnderlay);
        if (InfoUnderlay != null) UnityEngine.Object.Destroy(InfoUnderlay);
        if (InfoOverlayRules != null) UnityEngine.Object.Destroy(InfoOverlayRules);
        if (InfoOverlayRulesRight != null) UnityEngine.Object.Destroy(InfoOverlayRulesRight);
        MeetingUnderlay = InfoUnderlay = null;
        InfoOverlayRules = null;
        InfoOverlayRulesRight = null;
        OverlayShown = false;
        RolePage = 0;
        MaxRolePage = 0;
        RoleData = null;
        OptionsData = null;
        MaxOptionsPage = 0;
        RebuildUs.OptionsPage = 0;
    }

    public static bool InitializeOverlays()
    {
        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return false;

        if (MeetingUnderlay == null)
        {
            MeetingUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
            MeetingUnderlay.transform.localPosition = new Vector3(0f, 0f, 20f);
            MeetingUnderlay.gameObject.SetActive(true);
            MeetingUnderlay.enabled = false;
        }

        if (InfoUnderlay == null)
        {
            InfoUnderlay = UnityEngine.Object.Instantiate(MeetingUnderlay, hudManager.transform);
            InfoUnderlay.transform.localPosition = new Vector3(0f, 0f, -900f);
            InfoUnderlay.gameObject.SetActive(true);
            InfoUnderlay.enabled = false;
        }

        if (InfoOverlayRules == null)
        {
            InfoOverlayRules = UnityEngine.Object.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            InfoOverlayRules.maxVisibleLines = MaxLines;
            InfoOverlayRules.fontSize = InfoOverlayRules.fontSizeMin = InfoOverlayRules.fontSizeMax = 1.15f;
            InfoOverlayRules.autoSizeTextContainer = false;
            InfoOverlayRules.enableWordWrapping = false;
            InfoOverlayRules.alignment = TextAlignmentOptions.TopLeft;
            var transform = InfoOverlayRules.transform;
            transform.position = Vector3.zero;
            transform.localPosition = new Vector3(-2.5f, 1.15f, -910f);
            transform.localScale = Vector3.one;
            InfoOverlayRules.color = Palette.White;
            InfoOverlayRules.enabled = false;
        }

        if (InfoOverlayRulesRight == null)
        {
            InfoOverlayRulesRight = UnityEngine.Object.Instantiate(InfoOverlayRules, hudManager.transform);
            InfoOverlayRulesRight.transform.localPosition = new Vector3(1.2f, 1.15f, -910f);
            InfoOverlayRulesRight.enabled = false;
        }

        if (RoleData == null)
        {
            RoleData = [];

            var entries = new List<string> { CustomOption.OptionToString(CustomOptionHolder.PresetSelection) };

            var sb = new StringBuilder();
            AppendRoleCount(sb, "OptionPage.CrewmateRoles", CustomOptionHolder.CrewmateRolesCountMin, CustomOptionHolder.CrewmateRolesCountMax);
            AppendRoleCount(sb, "OptionPage.NeutralRoles", CustomOptionHolder.NeutralRolesCountMin, CustomOptionHolder.NeutralRolesCountMax);
            AppendRoleCount(sb, "OptionPage.ImpostorRoles", CustomOptionHolder.ImpostorRolesCountMin, CustomOptionHolder.ImpostorRolesCountMax);

            entries.Add(sb.ToString().TrimEnd());

            foreach (var option in CustomOption.AllOptions)
            {
                if (option == CustomOptionHolder.PresetSelection ||
                    option == CustomOptionHolder.CrewmateRolesCountMin ||
                    option == CustomOptionHolder.CrewmateRolesCountMax ||
                    option == CustomOptionHolder.NeutralRolesCountMin ||
                    option == CustomOptionHolder.NeutralRolesCountMax ||
                    option == CustomOptionHolder.ImpostorRolesCountMin ||
                    option == CustomOptionHolder.ImpostorRolesCountMax)
                {
                    continue;
                }

                if (option.Parent == null && option.Enabled)
                {
                    sb.Clear();
                    sb.AppendLine(CustomOption.OptionToString(option));
                    AddChildren(option, sb);

                    var entryText = sb.ToString().TrimEnd();
                    var linesArray = entryText.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

                    if (linesArray.Length > MaxLines)
                    {
                        for (int i = 0; i < linesArray.Length; i += MaxLines)
                        {
                            var count = Math.Min(MaxLines, linesArray.Length - i);
                            entries.Add(string.Join("\n", linesArray, i, count));
                        }
                    }
                    else
                    {
                        entries.Add(entryText);
                    }
                }
            }

            var pageSb = new StringBuilder();
            var currentLineCount = 0;
            foreach (var e in entries)
            {
                var lines = CountLines(e);

                if (currentLineCount + lines > MaxLines)
                {
                    RoleData.Add(pageSb.ToString().TrimEnd());
                    pageSb.Clear();
                    currentLineCount = 0;
                }

                pageSb.Append(e).Append("\n\n");
                currentLineCount += lines + 1;
            }

            var finalPageText = pageSb.ToString().TrimEnd();
            if (!string.IsNullOrEmpty(finalPageText))
            {
                RoleData.Add(finalPageText);
            }

            MaxRolePage = ((RoleData.Count - 1) / 3) + 1;
        }

        return true;
    }

    public static void AddChildren(CustomOption option, StringBuilder sb, bool indent = true)
    {
        if (!option.Enabled) return;

        foreach (var child in option.Children)
        {
            if (indent) sb.Append("    ");
            sb.Append(CustomOption.OptionToString(child)).AppendLine();
            AddChildren(child, sb, indent);
        }
    }

    public static void ShowBlackBG()
    {
        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (!InitializeOverlays()) return;

        MeetingUnderlay.sprite = AssetLoader.White;
        MeetingUnderlay.enabled = true;
        MeetingUnderlay.transform.localScale = new Vector3(20f, 20f, 1f);
        var clearBlack = new Color32(0, 0, 0, 0);

        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            MeetingUnderlay.color = Color.Lerp(clearBlack, Palette.Black, t);
        })));
    }

    public static void HideBlackBG()
    {
        if (MeetingUnderlay == null) return;
        MeetingUnderlay.enabled = false;
    }

    public static void ShowInfoOverlay()
    {
        if (OverlayShown) return;

        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        var player = PlayerControl.LocalPlayer;
        if (MapUtilities.CachedShipStatus == null || player == null || hudManager == null || hudManager.IsIntroDisplayed || (!player.CanMove && MeetingHud.Instance == null))
            return;

        if (!InitializeOverlays()) return;

        MapBehaviour.Instance?.Close();

        hudManager.SetHudActive(false);

        OverlayShown = true;

        var meetingHud = MeetingHud.Instance;
        var parent = meetingHud != null ? meetingHud.transform : hudManager.transform;
        InfoUnderlay.transform.SetParent(parent);
        InfoOverlayRules.transform.SetParent(parent);
        InfoOverlayRulesRight.transform.SetParent(parent);

        InfoUnderlay.sprite = AssetLoader.White;
        InfoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        InfoUnderlay.transform.localScale = new Vector3(7.5f, 5f, 1f);
        InfoUnderlay.enabled = true;

        var playerCount = GameData.Instance ? GameData.Instance.PlayerCount : 10;
        var hudOptions = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(playerCount);
        var tr = DestroyableSingleton<TranslationController>.Instance;
        var options = GameOptionsManager.Instance.CurrentGameOptions;

        int num2 = Helpers.GetOption(Int32OptionNames.VotingTime);
        var optionsSb = new StringBuilder(Tr.Get("Option.AmongUsSettings")).Append("\n\n")
            .Append(tr.GetString(StringNames.GameNumImpostors)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumImpostors)).Append('\n')
            .Append(tr.GetString(StringNames.GameConfirmImpostor)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.ConfirmImpostor) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
            .Append(tr.GetString(StringNames.GameNumMeetings)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumEmergencyMeetings)).Append('\n')
            .Append(tr.GetString(StringNames.GameAnonymousVotes)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.AnonymousVotes) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
            .Append(tr.GetString(StringNames.GameEmergencyCooldown)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown))).Append('\n')
            .Append(tr.GetString(StringNames.GameDiscussTime)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown))).Append('\n')
            .Append(tr.GetString(StringNames.GameVotingTime)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, num2 > 0 ? num2 : "∞")).Append('\n')
            .Append(tr.GetString(StringNames.GamePlayerSpeed)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.PlayerSpeedMod)).Append('\n')
            .Append(tr.GetString(StringNames.GameTaskBarMode)).Append(": ").Append(tr.GetString((StringNames)(277 + Helpers.GetOption(Int32OptionNames.TaskBarMode)))).Append('\n')
            .Append(tr.GetString(StringNames.GameVisualTasks)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.VisualTasks) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
            .Append(tr.GetString(StringNames.GameCrewLight)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.CrewLightMod)).Append('x').Append('\n')
            .Append(tr.GetString(StringNames.GameImpostorLight)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.ImpostorLightMod)).Append('x').Append('\n')
            .Append(tr.GetString(StringNames.GameKillCooldown)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(FloatOptionNames.KillCooldown))).Append('\n')
            .Append(tr.GetString(StringNames.GameKillDistance)).Append(": ").Append(tr.GetString((StringNames)(204 + Helpers.GetOption(Int32OptionNames.KillDistance)))).Append('\n')
            .Append(tr.GetString(StringNames.GameCommonTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumCommonTasks)).Append('\n')
            .Append(tr.GetString(StringNames.GameLongTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumLongTasks)).Append('\n')
            .Append(tr.GetString(StringNames.GameShortTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumShortTasks)).Append('\n')
            .Append("\n\n\n\n\n\n\n\n")
            .Append(CustomOption.OptionsToString(CustomOptionHolder.GameOptions)).Append("\n\n")
            .Append(CustomOption.OptionsToString(CustomOptionHolder.PolusOptions)).Append("\n\n")
            .Append(CustomOption.OptionsToString(CustomOptionHolder.AirshipOptions)).Append("\n\n")
            .Append(CustomOption.OptionsToString(CustomOptionHolder.RandomMap)).Append("\n\n");

        foreach (var r in RoleInfo.GetRoleInfoForPlayer(player))
        {
            optionsSb.Append("<size=150%>").Append(r.NameColored).Append("</size>");
            var roleDesc = r.FullDescription;
            if (roleDesc != "") optionsSb.Append('\n').Append(roleDesc);
            optionsSb.Append("\n\n");
            var roleOptions = r.RoleOptions;
            if (roleOptions != "") optionsSb.Append(roleOptions).Append("\n\n");
        }

        OptionsData = SplitToPages(optionsSb.ToString(), MaxLines - 1);
        MaxOptionsPage = OptionsData.Count;
        RebuildUs.OptionsPage = 0;
        SetInfoOverlayText();
        InfoOverlayRules.enabled = true;
        InfoOverlayRulesRight.enabled = true;

        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            InfoUnderlay.color = Color.Lerp(underlayTransparent, underlayOpaque, t);
            InfoOverlayRules.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
            InfoOverlayRulesRight.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
        })));
    }

    public static void HideInfoOverlay()
    {
        if (!OverlayShown) return;

        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (MeetingHud.Instance == null) hudManager.SetHudActive(true);

        OverlayShown = false;
        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            if (InfoUnderlay != null)
            {
                InfoUnderlay.color = Color.Lerp(underlayOpaque, underlayTransparent, t);
                if (t >= 1.0f) InfoUnderlay.enabled = false;
            }

            if (InfoOverlayRules != null)
            {
                InfoOverlayRules.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) InfoOverlayRules.enabled = false;
            }

            if (InfoOverlayRulesRight != null)
            {
                InfoOverlayRulesRight.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) InfoOverlayRulesRight.enabled = false;
            }
        })));
    }

    public static void ToggleInfoOverlay()
    {
        if (OverlayShown)
        {
            HideInfoOverlay();
        }
        else
        {
            ShowInfoOverlay();
        }
    }

    public static void ShowRoleOverlay()
    {
        if (RolePage != 0) return;

        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        var player = PlayerControl.LocalPlayer;
        if (MapUtilities.CachedShipStatus == null || player == null || hudManager == null || hudManager.IsIntroDisplayed || (!player.CanMove && MeetingHud.Instance == null))
        {
            return;
        }

        if (!InitializeOverlays()) return;

        HideInfoOverlay();

        MapBehaviour.Instance?.Close();

        hudManager.SetHudActive(false);

        RolePage = 1;
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class CustomOverlayKeybinds
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            ChatController cc = DestroyableSingleton<ChatController>.Instance;
            bool isOpen = cc != null && cc.IsOpenOrOpening;
            if (Input.GetKeyDown(KeyCode.H) && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !isOpen)
            {
                ToggleInfoOverlay();
            }

            if (OverlayShown && !isOpen)
            {
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    RebuildUs.OptionsPage -= 2;
                    if (RebuildUs.OptionsPage < 0) RebuildUs.OptionsPage = (MaxOptionsPage - 1) / 2 * 2;
                    SetInfoOverlayText();
                }
                else if (Input.GetKeyDown(KeyCode.Period))
                {
                    RebuildUs.OptionsPage += 2;
                    if (RebuildUs.OptionsPage >= MaxOptionsPage) RebuildUs.OptionsPage = 0;
                    SetInfoOverlayText();
                }
            }
        }
    }
}