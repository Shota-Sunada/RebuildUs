namespace RebuildUs;

public class CustomOverlays
{
    private static SpriteRenderer MeetingUnderlay;
    private static SpriteRenderer InfoUnderlay;
    private static TextMeshPro InfoOverlayTitle;
    private static TextMeshPro InfoOverlayRules;
    private static TextMeshPro InfoOverlayRulesRight;
    public static bool OverlayShown = false;
    private static List<string> OptionsData;
    public static int MaxOptionsPage = 0;

    private const int MaxLines = 26;
    private const float LeftColumnX = -2.4f;
    private const float RightColumnX = 1.2f;
    private const float TextY = 0.6f;
    private const float UnderlayZ = -900f;
    private const float TextZ = -910f;

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
            if (line == "\f" || currentLineCount >= maxLines)
            {
                pages.Add(currentPage.ToString().TrimEnd());
                currentPage.Clear();
                currentLineCount = 0;
                if (line == "\f") continue;
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

        int currentPageNumber = (RebuildUs.OptionsPage / 2) + 1;
        int totalPagesNumber = (MaxOptionsPage + 1) / 2;
        InfoOverlayTitle?.text = new StringBuilder(Tr.Get("Option.GameOptions")).Append(" <size=80%>").Append("Option.CurrentPage").Append(" (").Append(currentPageNumber).Append('/').Append(totalPagesNumber).Append(")\n").Append(Tr.Get("Option.ChangePage")).Append("</size>").ToString();

        var sb = new StringBuilder();
        sb.Append(OptionsData[RebuildUs.OptionsPage]);
        InfoOverlayRules.text = sb.ToString();

        if (RebuildUs.OptionsPage + 1 < OptionsData.Count)
        {
            InfoOverlayRulesRight.text = OptionsData[RebuildUs.OptionsPage + 1];
        }
        else
        {
            InfoOverlayRulesRight.text = string.Empty;
        }
    }

    private static void AppendRoleCount(ref StringBuilder sb, string key, CustomOption minOpt, CustomOption maxOpt)
    {
        int min = minOpt.GetSelection();
        int max = maxOpt.GetSelection();
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
        if (InfoOverlayTitle != null) UnityEngine.Object.Destroy(InfoOverlayTitle);
        if (InfoOverlayRules != null) UnityEngine.Object.Destroy(InfoOverlayRules);
        if (InfoOverlayRulesRight != null) UnityEngine.Object.Destroy(InfoOverlayRulesRight);
        MeetingUnderlay = InfoUnderlay = null;
        InfoOverlayTitle = null;
        InfoOverlayRules = null;
        InfoOverlayRulesRight = null;
        OverlayShown = false;
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
            MeetingUnderlay.name = "MeetingUnderlay";
            MeetingUnderlay.transform.localPosition = new Vector3(0f, 0f, 20f);
            MeetingUnderlay.gameObject.SetActive(true);
            MeetingUnderlay.enabled = false;
        }

        if (InfoUnderlay == null)
        {
            InfoUnderlay = UnityEngine.Object.Instantiate(MeetingUnderlay, hudManager.transform);
            InfoUnderlay.name = "InfoUnderlay";
            InfoUnderlay.transform.localPosition = new Vector3(0f, 0f, UnderlayZ);
            InfoUnderlay.gameObject.SetActive(true);
            InfoUnderlay.enabled = false;
        }

        if (InfoOverlayTitle == null)
        {
            InfoOverlayTitle = UnityEngine.Object.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            InfoOverlayTitle.name = "InfoOverlayTitle";
            InfoOverlayTitle.maxVisibleLines = MaxLines;
            InfoOverlayTitle.fontSize = InfoOverlayTitle.fontSizeMin = InfoOverlayTitle.fontSizeMax = 1.75f;
            InfoOverlayTitle.autoSizeTextContainer = false;
            InfoOverlayTitle.enableWordWrapping = false;
            InfoOverlayTitle.alignment = TextAlignmentOptions.Center;
            InfoOverlayTitle.transform.localPosition = new Vector3(0, 2.2f, TextZ);
            InfoOverlayTitle.color = Palette.White;
            InfoOverlayTitle.enabled = false;
        }

        if (InfoOverlayRules == null)
        {
            InfoOverlayRules = UnityEngine.Object.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            InfoOverlayRules.name = "InfoOverlayRules";
            InfoOverlayRules.maxVisibleLines = MaxLines;
            InfoOverlayRules.fontSize = InfoOverlayRules.fontSizeMin = InfoOverlayRules.fontSizeMax = 1.20f;
            InfoOverlayRules.autoSizeTextContainer = false;
            InfoOverlayRules.enableWordWrapping = false;
            InfoOverlayRules.alignment = TextAlignmentOptions.TopLeft;
            InfoOverlayRules.transform.localPosition = new Vector3(LeftColumnX, TextY, TextZ);
            InfoOverlayRules.color = Palette.White;
            InfoOverlayRules.enabled = false;
        }

        if (InfoOverlayRulesRight == null)
        {
            InfoOverlayRulesRight = UnityEngine.Object.Instantiate(InfoOverlayRules, hudManager.transform);
            InfoOverlayRulesRight.name = "InfoOverlayRulesRight";
            InfoOverlayRulesRight.transform.localPosition = new Vector3(RightColumnX, TextY, TextZ);
            InfoOverlayRulesRight.enabled = false;
        }

        var player = PlayerControl.LocalPlayer;
        if (player == null) return true;

        if (OptionsData == null)
        {
            OptionsData = [];
            var tr = DestroyableSingleton<TranslationController>.Instance;
            var sb = new StringBuilder();

            // Part 1: Among Us Settings
            int votingTime = Helpers.GetOption(Int32OptionNames.VotingTime);
            sb.Append("<size=120%>").Append(Tr.Get("Option.AmongUsSettings")).Append("</size>\n\n")
              .Append(tr.GetString(StringNames.GameNumImpostors)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumImpostors)).Append('\n')
              .Append(tr.GetString(StringNames.GameConfirmImpostor)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.ConfirmImpostor) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
              .Append(tr.GetString(StringNames.GameNumMeetings)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumEmergencyMeetings)).Append('\n')
              .Append(tr.GetString(StringNames.GameAnonymousVotes)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.AnonymousVotes) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
              .Append(tr.GetString(StringNames.GameEmergencyCooldown)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown))).Append('\n')
              .Append(tr.GetString(StringNames.GameDiscussTime)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown))).Append('\n')
              .Append(tr.GetString(StringNames.GameVotingTime)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, votingTime > 0 ? votingTime : "∞")).Append('\n')
              .Append(tr.GetString(StringNames.GamePlayerSpeed)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.PlayerSpeedMod)).Append('\n')
              .Append(tr.GetString(StringNames.GameTaskBarMode)).Append(": ").Append(tr.GetString((StringNames)(277 + Helpers.GetOption(Int32OptionNames.TaskBarMode)))).Append('\n')
              .Append(tr.GetString(StringNames.GameVisualTasks)).Append(": ").Append(Helpers.GetOption(BoolOptionNames.VisualTasks) ? Tr.Get("Option.On") : Tr.Get("Option.Off")).Append('\n')
              .Append(tr.GetString(StringNames.GameCrewLight)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.CrewLightMod)).Append('x').Append('\n')
              .Append(tr.GetString(StringNames.GameImpostorLight)).Append(": ").Append(Helpers.GetOption(FloatOptionNames.ImpostorLightMod)).Append('x').Append('\n')
              .Append(tr.GetString(StringNames.GameKillCooldown)).Append(": ").Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(FloatOptionNames.KillCooldown))).Append('\n')
              .Append(tr.GetString(StringNames.GameKillDistance)).Append(": ").Append(tr.GetString((StringNames)(204 + Helpers.GetOption(Int32OptionNames.KillDistance)))).Append('\n')
              .Append(tr.GetString(StringNames.GameCommonTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumCommonTasks)).Append('\n')
              .Append(tr.GetString(StringNames.GameLongTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumLongTasks)).Append('\n')
              .Append(tr.GetString(StringNames.GameShortTasks)).Append(": ").Append(Helpers.GetOption(Int32OptionNames.NumShortTasks)).Append("\n\n")
              .Append('\f');
            OptionsData.AddRange(SplitToPages(sb.ToString(), MaxLines - 1));

            // Part 2: Role Info for Player
            sb.Clear();
            foreach (var r in RoleInfo.GetRoleInfoForPlayer(player))
            {
                sb.Append("<size=150%>").Append(r.NameColored).Append("</size>");
                if (!string.IsNullOrEmpty(r.FullDescription)) sb.Append('\n').Append(r.FullDescription);
                sb.Append("\n\n");
                if (!string.IsNullOrEmpty(r.RoleOptions)) sb.Append(r.RoleOptions).Append("\n\n");
            }
            sb.Append('\f');
            OptionsData.AddRange(SplitToPages(sb.ToString(), MaxLines - 1));

            // Part 3: Custom Options Groups
            sb.Clear();
            sb.Append(CustomOption.OptionsToString(CustomOptionHolder.GameOptions)).Append("\n\n")
              .Append(CustomOption.OptionsToString(CustomOptionHolder.AirshipOptimize)).Append("\n\n")
              .Append(CustomOption.OptionsToString(CustomOptionHolder.RandomMap)).Append('\f');
            OptionsData.AddRange(SplitToPages(sb.ToString(), MaxLines - 1));

            // Part 4: Detailed Custom Options
            var entries = new List<string> { CustomOption.OptionToString(CustomOptionHolder.PresetSelection) };
            sb.Clear();
            AppendRoleCount(ref sb, "OptionPage.CrewmateRoles", CustomOptionHolder.CrewmateRolesCountMin, CustomOptionHolder.CrewmateRolesCountMax);
            AppendRoleCount(ref sb, "OptionPage.NeutralRoles", CustomOptionHolder.NeutralRolesCountMin, CustomOptionHolder.NeutralRolesCountMax);
            AppendRoleCount(ref sb, "OptionPage.ImpostorRoles", CustomOptionHolder.ImpostorRolesCountMin, CustomOptionHolder.ImpostorRolesCountMax);
            entries.Add(sb.ToString().TrimEnd());

            foreach (var option in CustomOption.AllOptions)
            {
                if (IsCommonOption(option)) continue;

                if (option.Parent == null && option.Enabled)
                {
                    sb.Clear();
                    sb.AppendLine(CustomOption.OptionToString(option));
                    AddChildren(option, sb);

                    string entryText = sb.ToString().TrimEnd();
                    int lines = CountLines(entryText);
                    if (lines > MaxLines) OptionsData.AddRange(SplitToPages(entryText, MaxLines));
                    else entries.Add(entryText);
                }
            }

            sb.Clear();
            int currentLineCount = 0;
            foreach (var e in entries)
            {
                int lines = CountLines(e);
                if (e == "\f" || currentLineCount + lines > MaxLines)
                {
                    if (sb.Length > 0)
                    {
                        OptionsData.Add(sb.ToString().TrimEnd());
                        sb.Clear();
                    }
                    currentLineCount = 0;
                    if (e == "\f") continue;
                }
                sb.Append(e).Append("\n\n");
                currentLineCount += lines + 1;
            }
            if (sb.Length > 0) OptionsData.Add(sb.ToString().TrimEnd());
            MaxOptionsPage = OptionsData.Count;
        }

        return true;
    }

    private static bool IsCommonOption(CustomOption option)
    {
        return option == CustomOptionHolder.PresetSelection ||
               option == CustomOptionHolder.CrewmateRolesCountMin ||
               option == CustomOptionHolder.CrewmateRolesCountMax ||
               option == CustomOptionHolder.NeutralRolesCountMin ||
               option == CustomOptionHolder.NeutralRolesCountMax ||
               option == CustomOptionHolder.ImpostorRolesCountMin ||
               option == CustomOptionHolder.ImpostorRolesCountMax;
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
        MeetingUnderlay.transform.localScale = new Vector3(13f, 5f, 1f);
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
        if (MapUtilities.CachedShipStatus == null || player == null || hudManager == null ||
            hudManager.IsIntroDisplayed || (!player.CanMove && MeetingHud.Instance == null))
        {
            return;
        }

        if (!InitializeOverlays()) return;

        MapBehaviour.Instance?.Close();
        hudManager.SetHudActive(false);

        OverlayShown = true;
        var parent = MeetingHud.Instance != null ? MeetingHud.Instance.transform : hudManager.transform;

        InfoUnderlay.transform.SetParent(parent);
        InfoUnderlay.sprite = AssetLoader.White;
        InfoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        InfoUnderlay.transform.localScale = new Vector3(13f, 5f, 1f);
        InfoUnderlay.enabled = true;

        InfoOverlayTitle.transform.SetParent(parent);
        InfoOverlayRules.transform.SetParent(parent);
        InfoOverlayRulesRight.transform.SetParent(parent);

        RebuildUs.OptionsPage = 0;
        SetInfoOverlayText();
        InfoOverlayTitle.enabled = InfoOverlayRules.enabled = InfoOverlayRulesRight.enabled = true;

        var transparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var opaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            InfoUnderlay.color = Color.Lerp(transparent, opaque, t);
            InfoOverlayTitle.color = InfoOverlayRules.color = InfoOverlayRulesRight.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
        })));
    }

    public static void HideInfoOverlay()
    {
        if (!OverlayShown) return;

        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (MeetingHud.Instance == null) hudManager.SetHudActive(true);

        OverlayShown = false;
        var transparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var opaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            if (InfoUnderlay != null)
            {
                InfoUnderlay.color = Color.Lerp(opaque, transparent, t);
                if (t >= 1.0f) InfoUnderlay.enabled = false;
            }

            if (InfoOverlayTitle != null)
            {
                InfoOverlayTitle.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) InfoOverlayTitle.enabled = false;
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
        if (OverlayShown) HideInfoOverlay();
        else ShowInfoOverlay();
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class CustomOverlayKeybinds
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            var cc = DestroyableSingleton<ChatController>.Instance;
            bool isChatOpen = cc != null && cc.IsOpenOrOpening;

            if (Input.GetKeyDown(KeyCode.H) &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !isChatOpen)
            {
                ToggleInfoOverlay();
            }

            if (OverlayShown && !isChatOpen)
            {
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    RebuildUs.OptionsPage -= 2;
                    if (RebuildUs.OptionsPage < 0) RebuildUs.OptionsPage = Math.Max(0, (MaxOptionsPage - 1) / 2 * 2);
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