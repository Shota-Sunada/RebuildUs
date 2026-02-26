namespace RebuildUs;

internal abstract class CustomOverlays
{
    private const int MAX_LINES = 26;
    private const int MAX_CHARS_PER_LINE = 45;
    private const float LEFT_COLUMN_X = -2.4f;
    private const float RIGHT_COLUMN_X = 1.2f;
    private const float TEXT_Y = 0.6f;
    private const float UNDERLAY_Z = -900f;

    private const float TEXT_Z = -910f;

    // private static SpriteRenderer MeetingUnderlay;
    private static SpriteRenderer _infoUnderlay;
    private static TextMeshPro _infoOverlayTitle;
    private static TextMeshPro _infoOverlayRules;
    private static TextMeshPro _infoOverlayRulesRight;
    private static bool _overlayShown;
    private static List<string> _optionsData;
    private static int _maxOptionsPage;

    private static int CountLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int count = 1;
        foreach (var t in text)
        {
            if (t == '\n')
                count++;
        }

        return count;
    }

    private static List<string> SplitToPages(string text, int maxLines)
    {
        List<string> pages = [];
        string[] lines = text.Replace("\r\n", "\n").Split('\n');
        StringBuilder currentPage = new();
        int currentLineCount = 0;

        foreach (string rawLine in lines)
        {
            if (rawLine == "\f")
            {
                if (currentPage.Length > 0) pages.Add(currentPage.ToString().TrimEnd());
                currentPage.Clear();
                currentLineCount = 0;
                continue;
            }

            List<string> wrappedLines = WrapLine(rawLine, MAX_CHARS_PER_LINE);
            foreach (string line in wrappedLines)
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
        }

        if (currentPage.Length > 0) pages.Add(currentPage.ToString().TrimEnd());

        return pages;
    }

    private static List<string> WrapLine(string line, int maxChars)
    {
        List<string> result = [];
        if (string.IsNullOrEmpty(line))
        {
            result.Add(string.Empty);
            return result;
        }

        StringBuilder sb = new();
        int visibleWidth = 0;
        bool inTag = false;

        foreach (var c in line)
        {
            if (c == '<') inTag = true;

            if (!inTag)
            {
                // Determine width: 2 for CJK/Full-width, 1 for ASCII/Half-width
                int charWidth = c <= '\u007f' ? 1 : 2;

                if (visibleWidth + charWidth > maxChars && visibleWidth > 0)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                    visibleWidth = 0;
                }

                visibleWidth += charWidth;
            }

            sb.Append(c);

            if (c == '>') inTag = false;
        }

        if (sb.Length > 0) result.Add(sb.ToString());
        return result;
    }

    private static void SetInfoOverlayText()
    {
        if (_optionsData == null || RebuildUs.OptionsPage < 0 || RebuildUs.OptionsPage >= _optionsData.Count) return;

        int currentPageNumber = (RebuildUs.OptionsPage / 2) + 1;
        int totalPagesNumber = (_maxOptionsPage + 1) / 2;
        _infoOverlayTitle?.text = new StringBuilder(Tr.Get(TrKey.GameOptions))
                                  .Append(" <size=80%>")
                                  .Append(Tr.Get(TrKey.CurrentPage))
                                  .Append(" (")
                                  .Append(currentPageNumber)
                                  .Append('/')
                                  .Append(totalPagesNumber)
                                  .Append(")\n")
                                  .Append(Tr.Get(TrKey.ChangePage))
                                  .Append("</size>")
                                  .ToString();

        StringBuilder sb = new();
        sb.Append(_optionsData[RebuildUs.OptionsPage]);
        _infoOverlayRules.text = sb.ToString();

        _infoOverlayRulesRight.text = RebuildUs.OptionsPage + 1 < _optionsData.Count ? _optionsData[RebuildUs.OptionsPage + 1] : string.Empty;
    }

    private static void AppendRoleCount(ref StringBuilder sb, string key, CustomOption minOpt, CustomOption maxOpt)
    {
        int min = minOpt.GetSelection();
        int max = maxOpt.GetSelection();
        if (min > max) min = max;

        sb.Append(Helpers.Cs(new(204f / 255f, 204f / 255f, 0, 1f), Tr.GetDynamic(key))).Append(": ");
        if (min == max) sb.Append(max);
        else sb.Append(min).Append(" - ").Append(max);
        sb.AppendLine();
    }

    internal static void ResetOverlays()
    {
        HideBlackBg();
        HideInfoOverlay();
        // if (MeetingUnderlay != null) UnityEngine.Object.Destroy(MeetingUnderlay);
        if (_infoUnderlay != null) UnityObject.Destroy(_infoUnderlay);
        if (_infoOverlayTitle != null) UnityObject.Destroy(_infoOverlayTitle);
        if (_infoOverlayRules != null) UnityObject.Destroy(_infoOverlayRules);
        if (_infoOverlayRulesRight != null) UnityObject.Destroy(_infoOverlayRulesRight);
        // MeetingUnderlay = InfoUnderlay = null;
        _infoUnderlay = null;
        _infoOverlayTitle = null;
        _infoOverlayRules = null;
        _infoOverlayRulesRight = null;
        _overlayShown = false;
        _optionsData = null;
        _maxOptionsPage = 0;
        RebuildUs.OptionsPage = 0;
    }

    private static bool InitializeOverlays()
    {
        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return false;

        // if (MeetingUnderlay == null)
        // {
        //     MeetingUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
        //     MeetingUnderlay.name = "MeetingUnderlay";
        //     MeetingUnderlay.transform.localPosition = new Vector3(0f, 0f, 20f);
        //     MeetingUnderlay.gameObject.SetActive(true);
        //     MeetingUnderlay.enabled = false;
        // }

        if (_infoUnderlay == null)
        {
            _infoUnderlay = UnityObject.Instantiate(hudManager.FullScreen, hudManager.transform);
            _infoUnderlay.name = "InfoUnderlay";
            _infoUnderlay.transform.localPosition = new(0f, 0f, UNDERLAY_Z);
            _infoUnderlay.gameObject.SetActive(true);
            _infoUnderlay.enabled = false;
        }

        if (_infoOverlayTitle == null)
        {
            _infoOverlayTitle = UnityObject.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            _infoOverlayTitle.name = "InfoOverlayTitle";
            _infoOverlayTitle.maxVisibleLines = MAX_LINES;
            _infoOverlayTitle.fontSize = _infoOverlayTitle.fontSizeMin = _infoOverlayTitle.fontSizeMax = 1.75f;
            _infoOverlayTitle.autoSizeTextContainer = false;
            _infoOverlayTitle.enableWordWrapping = false;
            _infoOverlayTitle.alignment = TextAlignmentOptions.Center;
            _infoOverlayTitle.transform.localPosition = new(0, 2.2f, TEXT_Z);
            _infoOverlayTitle.color = Palette.White;
            _infoOverlayTitle.enabled = false;
        }

        if (_infoOverlayRules == null)
        {
            _infoOverlayRules = UnityObject.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            _infoOverlayRules.name = "InfoOverlayRules";
            _infoOverlayRules.maxVisibleLines = MAX_LINES;
            _infoOverlayRules.fontSize = _infoOverlayRules.fontSizeMin = _infoOverlayRules.fontSizeMax = 1.20f;
            _infoOverlayRules.autoSizeTextContainer = false;
            _infoOverlayRules.enableWordWrapping = false;
            _infoOverlayRules.alignment = TextAlignmentOptions.TopLeft;
            _infoOverlayRules.transform.localPosition = new(LEFT_COLUMN_X, TEXT_Y, TEXT_Z);
            _infoOverlayRules.color = Palette.White;
            _infoOverlayRules.enabled = false;
        }

        if (_infoOverlayRulesRight == null)
        {
            _infoOverlayRulesRight = UnityObject.Instantiate(_infoOverlayRules, hudManager.transform);
            _infoOverlayRulesRight.name = "InfoOverlayRulesRight";
            _infoOverlayRulesRight.transform.localPosition = new(RIGHT_COLUMN_X, TEXT_Y, TEXT_Z);
            _infoOverlayRulesRight.enabled = false;
        }

        PlayerControl player = PlayerControl.LocalPlayer;
        if (player == null) return true;

        if (_optionsData != null) return true;
        _optionsData = [];
        TranslationController tr = DestroyableSingleton<TranslationController>.Instance;
        StringBuilder sb = new();

        // Part 1: Among Us Settings
        int votingTime = Helpers.GetOption(Int32OptionNames.VotingTime);
        sb.Append("<size=120%>")
          .Append(Tr.Get(TrKey.AmongUsSettings))
          .Append("</size>\n\n")
          .Append(tr.GetString(StringNames.GameNumImpostors))
          .Append(": ")
          .Append(Helpers.GetOption(Int32OptionNames.NumImpostors))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameConfirmImpostor))
          .Append(": ")
          .Append(Helpers.GetOption(BoolOptionNames.ConfirmImpostor) ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameNumMeetings))
          .Append(": ")
          .Append(Helpers.GetOption(Int32OptionNames.NumEmergencyMeetings))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameAnonymousVotes))
          .Append(": ")
          .Append(Helpers.GetOption(BoolOptionNames.AnonymousVotes) ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameEmergencyCooldown))
          .Append(": ")
          .Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown)))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameDiscussTime))
          .Append(": ")
          .Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(Int32OptionNames.EmergencyCooldown)))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameVotingTime))
          .Append(": ")
          .Append(tr.GetString(StringNames.GameSecondsAbbrev, votingTime > 0 ? votingTime : "∞"))
          .Append('\n')
          .Append(tr.GetString(StringNames.GamePlayerSpeed))
          .Append(": ")
          .Append(Helpers.GetOption(FloatOptionNames.PlayerSpeedMod))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameTaskBarMode))
          .Append(": ")
          .Append(tr.GetString((StringNames)(277 + Helpers.GetOption(Int32OptionNames.TaskBarMode))))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameVisualTasks))
          .Append(": ")
          .Append(Helpers.GetOption(BoolOptionNames.VisualTasks) ? Tr.Get(TrKey.On) : Tr.Get(TrKey.Off))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameCrewLight))
          .Append(": ")
          .Append(Helpers.GetOption(FloatOptionNames.CrewLightMod))
          .Append('x')
          .Append('\n')
          .Append(tr.GetString(StringNames.GameImpostorLight))
          .Append(": ")
          .Append(Helpers.GetOption(FloatOptionNames.ImpostorLightMod))
          .Append('x')
          .Append('\n')
          .Append(tr.GetString(StringNames.GameKillCooldown))
          .Append(": ")
          .Append(tr.GetString(StringNames.GameSecondsAbbrev, Helpers.GetOption(FloatOptionNames.KillCooldown)))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameKillDistance))
          .Append(": ")
          .Append(tr.GetString((StringNames)(204 + Helpers.GetOption(Int32OptionNames.KillDistance))))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameCommonTasks))
          .Append(": ")
          .Append(Helpers.GetOption(Int32OptionNames.NumCommonTasks))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameLongTasks))
          .Append(": ")
          .Append(Helpers.GetOption(Int32OptionNames.NumLongTasks))
          .Append('\n')
          .Append(tr.GetString(StringNames.GameShortTasks))
          .Append(": ")
          .Append(Helpers.GetOption(Int32OptionNames.NumShortTasks))
          .Append("\n\n")
          .Append('\f');
        _optionsData.AddRange(SplitToPages(sb.ToString(), MAX_LINES - 1));

        // Part 2: Role Info for Player
        sb.Clear();
        foreach (RoleInfo r in RoleInfo.GetRoleInfoForPlayer(player))
        {
            sb.Append("<size=150%>").Append(r.NameColored).Append("</size>");
            if (!string.IsNullOrEmpty(r.FullDescription)) sb.Append('\n').Append(r.FullDescription);
            sb.Append("\n\n");
            if (!string.IsNullOrEmpty(r.RoleOptions)) sb.Append(r.RoleOptions).Append("\n\n");
        }

        sb.Append('\f');
        _optionsData.AddRange(SplitToPages(sb.ToString(), MAX_LINES - 1));

        // Part 3: Custom Options Groups
        sb.Clear();
        sb.Append(CustomOption.OptionsToString(CustomOptionHolder.GameOptions)).Append("\n\n").Append(CustomOption.OptionsToString(CustomOptionHolder.AirshipOptimize)).Append("\n\n").Append(CustomOption.OptionsToString(CustomOptionHolder.RandomMap)).Append('\f');
        _optionsData.AddRange(SplitToPages(sb.ToString(), MAX_LINES - 1));

        // Part 4: Detailed Custom Options
        List<string> entries = [CustomOption.OptionToString(CustomOptionHolder.PresetSelection)];
        sb.Clear();
        AppendRoleCount(ref sb, "CrewmateRoles", CustomOptionHolder.CrewmateRolesCountMin, CustomOptionHolder.CrewmateRolesCountMax);
        AppendRoleCount(ref sb, "NeutralRoles", CustomOptionHolder.NeutralRolesCountMin, CustomOptionHolder.NeutralRolesCountMax);
        AppendRoleCount(ref sb, "ImpostorRoles", CustomOptionHolder.ImpostorRolesCountMin, CustomOptionHolder.ImpostorRolesCountMax);
        entries.Add(sb.ToString().TrimEnd());

        foreach (CustomOption option in CustomOption.AllOptions)
        {
            if (IsCommonOption(option)) continue;

            if (option.Parent != null || !option.Enabled) continue;
            sb.Clear();
            sb.AppendLine(CustomOption.OptionToString(option));
            AddChildren(option, sb);

            string entryText = sb.ToString().TrimEnd();
            int lines = CountLines(entryText);
            if (lines > MAX_LINES) _optionsData.AddRange(SplitToPages(entryText, MAX_LINES));
            else entries.Add(entryText);
        }

        sb.Clear();
        int currentLineCount = 0;
        foreach (string e in entries)
        {
            int lines = CountLines(e);
            if (e == "\f" || currentLineCount + lines > MAX_LINES)
            {
                if (sb.Length > 0)
                {
                    _optionsData.Add(sb.ToString().TrimEnd());
                    sb.Clear();
                }

                currentLineCount = 0;
                if (e == "\f") continue;
            }

            sb.Append(e).Append("\n\n");
            currentLineCount += lines + 1;
        }

        if (sb.Length > 0) _optionsData.Add(sb.ToString().TrimEnd());
        _maxOptionsPage = _optionsData.Count;

        return true;
    }

    private static bool IsCommonOption(CustomOption option)
    {
        return option == CustomOptionHolder.PresetSelection || option == CustomOptionHolder.CrewmateRolesCountMin || option == CustomOptionHolder.CrewmateRolesCountMax || option == CustomOptionHolder.NeutralRolesCountMin || option == CustomOptionHolder.NeutralRolesCountMax || option == CustomOptionHolder.ImpostorRolesCountMin || option == CustomOptionHolder.ImpostorRolesCountMax;
    }

    private static void AddChildren(CustomOption option, StringBuilder sb, bool indent = true)
    {
        if (!option.Enabled) return;
        foreach (CustomOption child in option.Children)
        {
            if (indent) sb.Append("    ");
            sb.Append(CustomOption.OptionToString(child)).AppendLine();
            AddChildren(child, sb, indent);
        }
    }

    internal static void ShowBlackBg()
    {
        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (!InitializeOverlays()) return;

        // MeetingUnderlay.sprite = AssetLoader.White;
        // MeetingUnderlay.enabled = true;
        // MeetingUnderlay.transform.localScale = new Vector3(13f, 5f, 1f);

        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            // MeetingUnderlay.color = Color.Lerp(clearBlack, Palette.Black, t);
        })));
    }

    internal static void HideBlackBg()
    {
        // if (MeetingUnderlay == null) return;
        // MeetingUnderlay.enabled = false;
    }

    private static void ShowInfoOverlay()
    {
        if (_overlayShown) return;

        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        PlayerControl player = PlayerControl.LocalPlayer;
        if (MapUtilities.CachedShipStatus == null || player == null || hudManager == null || hudManager.IsIntroDisplayed || (!player.CanMove && MeetingHud.Instance == null)) return;

        if (!InitializeOverlays()) return;

        if (MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) MapBehaviour.Instance.Close();

        hudManager.SetHudActive(false);

        _overlayShown = true;
        Transform parent = MeetingHud.Instance != null ? MeetingHud.Instance.transform : hudManager.transform;

        _infoUnderlay.transform.SetParent(parent);
        _infoUnderlay.sprite = AssetLoader.White;
        _infoUnderlay.color = new(0.1f, 0.1f, 0.1f, 0.88f);
        _infoUnderlay.transform.localScale = new(13f, 5f, 1f);
        _infoUnderlay.enabled = true;

        _infoOverlayTitle.transform.SetParent(parent);
        _infoOverlayRules.transform.SetParent(parent);
        _infoOverlayRulesRight.transform.SetParent(parent);

        RebuildUs.OptionsPage = 0;
        SetInfoOverlayText();
        _infoOverlayTitle.enabled = _infoOverlayRules.enabled = _infoOverlayRulesRight.enabled = true;

        Color transparent = new(0.1f, 0.1f, 0.1f, 0.0f);
        Color opaque = new(0.1f, 0.1f, 0.1f, 0.88f);
        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            _infoUnderlay.color = Color.Lerp(transparent, opaque, t);
            _infoOverlayTitle.color = _infoOverlayRules.color = _infoOverlayRulesRight.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
        })));
    }

    internal static void HideInfoOverlay()
    {
        if (!_overlayShown) return;

        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (MeetingHud.Instance == null) hudManager.SetHudActive(true);

        _overlayShown = false;
        Color transparent = new(0.1f, 0.1f, 0.1f, 0.0f);
        Color opaque = new(0.1f, 0.1f, 0.1f, 0.88f);

        hudManager.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            if (_infoUnderlay != null)
            {
                _infoUnderlay.color = Color.Lerp(opaque, transparent, t);
                if (t >= 1.0f) _infoUnderlay.enabled = false;
            }

            if (_infoOverlayTitle != null)
            {
                _infoOverlayTitle.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) _infoOverlayTitle.enabled = false;
            }

            if (_infoOverlayRules != null)
            {
                _infoOverlayRules.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) _infoOverlayRules.enabled = false;
            }

            if (_infoOverlayRulesRight != null)
            {
                _infoOverlayRulesRight.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) _infoOverlayRulesRight.enabled = false;
            }
        })));
    }

    private static void ToggleInfoOverlay()
    {
        if (_overlayShown) HideInfoOverlay();
        else ShowInfoOverlay();
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    internal static class CustomOverlayKeybinds
    {
        internal static void Postfix(KeyboardJoystick __instance)
        {
            ChatController cc = DestroyableSingleton<ChatController>.Instance;
            bool isChatOpen = cc != null && cc.IsOpenOrOpening;

            if (Input.GetKeyDown(KeyCode.H) && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && !isChatOpen) ToggleInfoOverlay();

            if (_overlayShown && !isChatOpen)
            {
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    RebuildUs.OptionsPage -= 2;
                    if (RebuildUs.OptionsPage < 0) RebuildUs.OptionsPage = Math.Max(0, ((_maxOptionsPage - 1) / 2) * 2);
                    SetInfoOverlayText();
                }
                else if (Input.GetKeyDown(KeyCode.Period))
                {
                    RebuildUs.OptionsPage += 2;
                    if (RebuildUs.OptionsPage >= _maxOptionsPage) RebuildUs.OptionsPage = 0;
                    SetInfoOverlayText();
                }
            }
        }
    }
}