using System.Text;

namespace RebuildUs;

public class CustomOverlays
{
    private static SpriteRenderer MeetingUnderlay;
    private static SpriteRenderer InfoUnderlay;
    private static TextMeshPro InfoOverlayRules;
    private static TextMeshPro InfoOverlayRoles;
    public static bool OverlayShown = false;
    private static SpriteRenderer RoleUnderlay;
    private static TextMeshPro[] RoleOverlayList;
    public static int RolePage = 0;
    public static int MaxRolePage = 0;
    private static List<string> RoleData;

    public static void ResetOverlays()
    {
        HideBlackBG();
        HideInfoOverlay();
        HideRoleOverlay();
        UnityEngine.Object.Destroy(MeetingUnderlay);
        UnityEngine.Object.Destroy(InfoUnderlay);
        UnityEngine.Object.Destroy(InfoOverlayRules);
        UnityEngine.Object.Destroy(InfoOverlayRoles);
        UnityEngine.Object.Destroy(RoleUnderlay);
        if (RoleOverlayList != null)
        {
            foreach (var roleOverlay in RoleOverlayList)
            {
                UnityEngine.Object.Destroy(roleOverlay);
            }
        }
        MeetingUnderlay = InfoUnderlay = null;
        InfoOverlayRules = InfoOverlayRoles = null;
        OverlayShown = false;
        RoleUnderlay = null;
        RoleOverlayList = null;
        RolePage = 0;
        MaxRolePage = 0;
        RoleData = null;
    }

    public static bool InitializeOverlays()
    {
        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
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
            InfoOverlayRules.fontSize = InfoOverlayRules.fontSizeMin = InfoOverlayRules.fontSizeMax = 1.15f;
            InfoOverlayRules.autoSizeTextContainer = false;
            InfoOverlayRules.enableWordWrapping = false;
            InfoOverlayRules.alignment = TMPro.TextAlignmentOptions.TopLeft;
            InfoOverlayRules.transform.position = Vector3.zero;
            InfoOverlayRules.transform.localPosition = new Vector3(-2.5f, 1.15f, -910f);
            InfoOverlayRules.transform.localScale = Vector3.one;
            InfoOverlayRules.color = Palette.White;
            InfoOverlayRules.enabled = false;
        }

        if (InfoOverlayRoles == null)
        {
            InfoOverlayRoles = UnityEngine.Object.Instantiate(InfoOverlayRules, hudManager.transform);
            InfoOverlayRoles.maxVisibleLines = 28;
            InfoOverlayRoles.fontSize = InfoOverlayRoles.fontSizeMin = InfoOverlayRoles.fontSizeMax = 1.15f;
            InfoOverlayRoles.outlineWidth += 0.02f;
            InfoOverlayRoles.autoSizeTextContainer = false;
            InfoOverlayRoles.enableWordWrapping = false;
            InfoOverlayRoles.alignment = TMPro.TextAlignmentOptions.TopLeft;
            InfoOverlayRoles.transform.position = Vector3.zero;
            InfoOverlayRoles.transform.localPosition = InfoOverlayRules.transform.localPosition + new Vector3(2.5f, 0.0f, 0.0f);
            InfoOverlayRoles.transform.localScale = Vector3.one;
            InfoOverlayRoles.color = Palette.White;
            InfoOverlayRoles.enabled = false;
        }

        if (RoleUnderlay == null)
        {
            RoleUnderlay = UnityEngine.Object.Instantiate(MeetingUnderlay, hudManager.transform);
            RoleUnderlay.transform.localPosition = new Vector3(0f, 0f, -900f);
            RoleUnderlay.gameObject.SetActive(true);
            RoleUnderlay.enabled = false;
        }

        if (RoleOverlayList == null)
        {
            RoleOverlayList = new TextMeshPro[3];
        }

        for (var i = 0; i < RoleOverlayList.Length; i++)
        {
            if (RoleOverlayList[i] == null)
            {
                if (i == 0)
                {
                    RoleOverlayList[i] = UnityEngine.Object.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);

                    InitializeRoleOverlay(RoleOverlayList[i]);

                    RoleOverlayList[i].transform.localPosition = new Vector3(-3.5f, 1.2f, -910f);
                }
                else
                {
                    RoleOverlayList[i] = UnityEngine.Object.Instantiate(RoleOverlayList[i - 1], hudManager.transform);

                    InitializeRoleOverlay(RoleOverlayList[i]);

                    RoleOverlayList[i].transform.localPosition = RoleOverlayList[i - 1].transform.localPosition + new Vector3(3.1f, 0.0f, 0.0f);
                }
            }
        }

        if (RoleData == null)
        {
            RoleData = [];

            StringBuilder entry = new();
            List<string> entries =
            [
                // First add the presets and the role counts
                CustomOption.OptionToString(CustomOptionHolder.PresetSelection),
            ];

            var optionName = Helpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), Tr.Get("OptionPage.CrewmateRoles"));
            var min = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
            var max = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
            if (min > max) min = max;
            var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = Helpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), Tr.Get("OptionPage.NeutralRoles"));
            min = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
            max = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            optionName = Helpers.Cs(new Color(204f / 255f, 204f / 255f, 0, 1f), Tr.Get("OptionPage.ImpostorRoles"));
            min = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
            max = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
            if (min > max) min = max;
            optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
            entry.AppendLine($"{optionName}: {optionValue}");

            entries.Add(entry.ToString().Trim('\r', '\n'));

            int maxLines = 28;

            foreach (CustomOption option in CustomOption.AllOptions)
            {
                if ((option == CustomOptionHolder.PresetSelection) ||
                    (option == CustomOptionHolder.CrewmateRolesCountMin) ||
                    (option == CustomOptionHolder.CrewmateRolesCountMax) ||
                    (option == CustomOptionHolder.NeutralRolesCountMin) ||
                    (option == CustomOptionHolder.NeutralRolesCountMax) ||
                    (option == CustomOptionHolder.ImpostorRolesCountMin) ||
                    (option == CustomOptionHolder.ImpostorRolesCountMax))
                {
                    continue;
                }

                if (option.Parent == null)
                {
                    if (!option.Enabled)
                    {
                        continue;
                    }

                    entry = new StringBuilder();
                    entry.AppendLine(CustomOption.OptionToString(option));

                    AddChildren(option, ref entry);

                    // 1つのオプションが最大行を越えていた場合、最大行までで分割する
                    int lines = entry.ToString().Trim('\r', '\n').Count(c => c == '\n') + 1;
                    while (lines > maxLines)
                    {
                        var line = 0;
                        var newEntry = new StringBuilder();
                        var entryLines = entry.ToString().Trim('\r', '\n').Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
                        foreach (var entryLine in entryLines)
                        {
                            newEntry.AppendLine(entryLine);
                            entry.Remove(0, entryLine.Length + Environment.NewLine.Length);
                            line++;
                            if (maxLines <= line)
                            {
                                break;
                            }
                        }
                        entries.Add(newEntry.ToString().Trim('\r', '\n'));
                        lines -= maxLines;
                    }

                    entries.Add(entry.ToString().Trim('\r', '\n'));
                }
            }

            int lineCount = 0;
            string page = "";
            foreach (var e in entries)
            {
                int lines = e.Count(c => c == '\n') + 1;

                if (lineCount + lines > maxLines)
                {
                    RoleData.Add(page);
                    page = "";
                    lineCount = 0;
                }

                page = page + e + "\n\n";
                lineCount += lines + 1;
            }

            page = page.Trim('\r', '\n');
            if (page != "")
            {
                RoleData.Add(page);
            }

            MaxRolePage = ((RoleData.Count - 1) / 3) + 1;
        }

        return true;
    }

    private static void InitializeRoleOverlay(TextMeshPro roleOverlay)
    {
        roleOverlay.maxVisibleLines = 29;
        roleOverlay.fontSize = roleOverlay.fontSizeMin = roleOverlay.fontSizeMax = 1.15f;
        roleOverlay.autoSizeTextContainer = false;
        roleOverlay.enableWordWrapping = false;
        roleOverlay.alignment = TMPro.TextAlignmentOptions.TopLeft;
        roleOverlay.transform.position = Vector3.zero;
        roleOverlay.transform.localScale = Vector3.one;
        roleOverlay.color = Palette.White;
        roleOverlay.enabled = false;
    }

    public static void AddChildren(CustomOption option, ref StringBuilder entry, bool indent = true)
    {
        if (!option.Enabled) return;

        foreach (var child in option.Children)
        {
            entry.AppendLine((indent ? "    " : "") + CustomOption.OptionToString(child));
            AddChildren(child, ref entry, indent);
        }
    }

    public static void ShowBlackBG()
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null) return;
        if (!InitializeOverlays()) return;

        MeetingUnderlay.sprite = AssetLoader.White;
        MeetingUnderlay.enabled = true;
        MeetingUnderlay.transform.localScale = new Vector3(20f, 20f, 1f);
        var clearBlack = new Color32(0, 0, 0, 0);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
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

        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (MapUtilities.CachedShipStatus == null || PlayerControl.LocalPlayer == null || hudManager == null || FastDestroyableSingleton<HudManager>.Instance.IsIntroDisplayed || (!PlayerControl.LocalPlayer.CanMove && MeetingHud.Instance == null))
            return;

        if (!InitializeOverlays()) return;

        HideRoleOverlay();

        MapBehaviour.Instance?.Close();

        hudManager.SetHudActive(false);

        OverlayShown = true;

        Transform parent = MeetingHud.Instance != null ? MeetingHud.Instance.transform : hudManager.transform;
        InfoUnderlay.transform.SetParent(parent);
        InfoOverlayRules.transform.SetParent(parent);
        InfoOverlayRoles.transform.SetParent(parent);

        InfoUnderlay.sprite = AssetLoader.White;
        InfoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        InfoUnderlay.transform.localScale = new Vector3(7.5f, 5f, 1f);
        InfoUnderlay.enabled = true;

        RebuildUs.OptionsPage = 0;
        var option = GameOptionsManager.Instance.CurrentGameOptions;
        // var gameOptions = option.ToString().Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList().GetRange(2, 17);
        InfoOverlayRules.text = string.Join("\n", option) + "\n\n" + CustomOption.OptionsToString(CustomOptionHolder.PolusOptions) + "\n\n" + CustomOption.OptionsToString(CustomOptionHolder.AirshipOptions) + "\n\n" + CustomOption.OptionsToString(CustomOptionHolder.RandomMap);
        InfoOverlayRules.enabled = true;

        string rolesText = "";
        foreach (var r in RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer))
        {
            string roleOptions = r.RoleOptions;
            string roleDesc = r.FullDescription;
            rolesText += $"<size=150%>{r.NameColored}</size>" +
                (roleDesc != "" ? $"\n{r.FullDescription}" : "") + "\n\n" +
                (roleOptions != "" ? $"{roleOptions}\n\n" : "");
        }

        InfoOverlayRoles.text = rolesText;
        InfoOverlayRoles.enabled = true;

        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            InfoUnderlay.color = Color.Lerp(underlayTransparent, underlayOpaque, t);
            InfoOverlayRules.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
            InfoOverlayRoles.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
        })));
    }

    public static void HideInfoOverlay()
    {
        if (!OverlayShown) return;

        if (MeetingHud.Instance == null) FastDestroyableSingleton<HudManager>.Instance.SetHudActive(true);

        OverlayShown = false;
        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
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

            if (InfoOverlayRoles != null)
            {
                InfoOverlayRoles.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) InfoOverlayRoles.enabled = false;
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

        HudManager hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (MapUtilities.CachedShipStatus == null || PlayerControl.LocalPlayer == null || hudManager == null || FastDestroyableSingleton<HudManager>.Instance.IsIntroDisplayed || (!PlayerControl.LocalPlayer.CanMove && MeetingHud.Instance == null))
        {
            return;
        }

        if (!InitializeOverlays()) return;

        HideInfoOverlay();

        MapBehaviour.Instance?.Close();

        hudManager.SetHudActive(false);

        RolePage = 1;

        Transform parent = MeetingHud.Instance != null ? MeetingHud.Instance.transform : hudManager.transform;
        RoleUnderlay.transform.parent = parent;
        foreach (var roleOverlay in RoleOverlayList)
        {
            roleOverlay.transform.parent = parent;
        }

        RoleUnderlay.sprite = AssetLoader.White;
        RoleUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        RoleUnderlay.transform.localScale = new Vector3(9.3f, 5.1f, 1f);
        RoleUnderlay.enabled = true;

        SetRoleOverlayText();

        foreach (var roleOverlay in RoleOverlayList)
        {
            roleOverlay.enabled = true;
        }

        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            RoleUnderlay.color = Color.Lerp(underlayTransparent, underlayOpaque, t);
            foreach (var roleOverlay in RoleOverlayList)
            {
                roleOverlay.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
            }
        })));
    }

    public static void SetRoleOverlayText()
    {
        var i = (RolePage - 1) * 3;
        var pageText = $" ({RolePage}/{MaxRolePage})" + "\n";
        foreach (var roleOverlay in RoleOverlayList)
        {
            roleOverlay.text = i < RoleData.Count ? pageText + RoleData[i].Trim('\r', '\n') : string.Empty;
            i++;
            pageText = "\n";
        }
    }

    public static void HideRoleOverlay()
    {
        if (RolePage == 0) return;

        if (MeetingHud.Instance == null) FastDestroyableSingleton<HudManager>.Instance.SetHudActive(true);

        RolePage = 0;
        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            if (RoleUnderlay != null)
            {
                RoleUnderlay.color = Color.Lerp(underlayOpaque, underlayTransparent, t);
                if (t >= 1.0f) RoleUnderlay.enabled = false;
            }

            if (RoleOverlayList != null)
            {
                foreach (var roleOverlay in RoleOverlayList)
                {
                    if (roleOverlay != null)
                    {
                        roleOverlay.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                        if (t >= 1.0f) roleOverlay.enabled = false;
                    }
                }
            }
        })));
    }

    public static void ToggleRoleOverlay()
    {
        if (RolePage == 0)
        {
            ShowRoleOverlay();
        }
        else if (MaxRolePage <= RolePage)
        {
            HideRoleOverlay();
        }
        else
        {
            RolePage++;
            SetRoleOverlayText();
        }
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
            else if (Input.GetKeyDown(KeyCode.I) && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !isOpen)
            {
                ToggleRoleOverlay();
            }
        }
    }
}