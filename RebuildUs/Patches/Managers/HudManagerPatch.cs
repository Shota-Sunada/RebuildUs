namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class HudManagerPatch
{
    private static bool _isUpdating;
    private static readonly Dictionary<byte, Color> ColorCache = [];

    private static readonly StringBuilder TagStringBuilder = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal static void UpdatePostfix(HudManager __instance)
    {
        if (AmongUsClient.Instance?.GameState != InnerNetClient.GameStates.Started)
        {
            return;
        }

        try
        {
            CustomButton.HudUpdate();
            UpdatePlayerNamesAndColors();
            CamouflageAndMorphActions();
            UpdateImpostorKillButton(__instance);
            UpdateSabotageButton(__instance);
            UpdateUseButton(__instance);
            UpdateReportButton(__instance);
            UpdateVentButton(__instance);

            Hacker.HackerTimer -= Time.deltaTime;
            Trickster.LightsOutTimer -= Time.deltaTime;
            Tracker.CorpsesTrackingTimer -= Time.deltaTime;
        }
        catch (Exception ex)
        {
            Logger.LogError($"[HudManagerPatch] UpdatePostfix error: {ex}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
    internal static void SetHudActivePostfix(HudManager __instance)
    {
        __instance.TaskPanel.gameObject.SetActive(true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
    internal static void Prefix(HudManager __instance)
    {
        Meeting.StartMeetingClear();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    internal static void StartPostfix(HudManager __instance)
    {
        RebuildUs.MakeButtons(__instance);
        RebuildUs.SetButtonCooldowns();
    }

    private static void UpdatePlayerNamesAndColors()
    {
        _isUpdating = true;
        ColorCache.Clear();

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            _isUpdating = false;
            return;
        }

        var isLocalImpostor = localPlayer.IsTeamImpostor();
        var isLocalDead = localPlayer.IsDead();
        var meetingShow = Helpers.ShowMeetingText;

        // 1. Initialize Base Colors
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null)
            {
                continue;
            }
            var baseColor = isLocalImpostor && p.IsTeamImpostor() ? Palette.ImpostorRed : Color.white;
            SetPlayerNameColor(p, baseColor);
        }

        // 2. Calculate Role/Modifier Colors (populates ColorCache via SetPlayerNameColor)
        var lp = PlayerControl.LocalPlayer;
        if (lp == null)
        {
            return;
        }

        // 1. Set Local Player Color
        var roleInstance = ModRoleManager.GetRole(lp);
        if (roleInstance != null)
        {
            SetPlayerNameColor(lp, roleInstance.RoleColor);
        }

        foreach (var mod in PlayerModifier.GetModifiers(lp))
        {
            SetPlayerNameColor(lp, mod.ModifierColor);
        }

        // 2. Process logic-heavy vision (Jackal seeing Sidekick, Spy seeing Impostors, etc.)
        foreach (var r in ModRoleManager.AllRoles)
        {
            r.OnUpdateNameColors();
        }
        foreach (var m in PlayerModifier.AllModifiers)
        {
            m.OnUpdateNameColors();
        }

        // 3. Update Player Instances
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player == null || player.cosmetics == null || player.cosmetics.nameText == null)
            {
                continue;
            }

            // --- Name Calculation ---
            var finalName = player.CurrentOutfit.PlayerName;
            var hideName = Helpers.HidePlayerName(localPlayer, player);

            if (hideName)
            {
                finalName = "";
            }
            else if (isLocalDead)
            {
                if (string.IsNullOrEmpty(finalName) && Camouflager.CamouflageTimer > 0f)
                {
                    finalName = player.Data.DefaultOutfit.PlayerName;
                }
                else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                {
                    finalName = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
                }
            }

            TagStringBuilder.Clear();
            TagStringBuilder.Append(finalName);

            if (!string.IsNullOrEmpty(finalName))
            {
                // Role Tags
                var r = ModRoleManager.GetRole(player);
                if (r != null)
                {
                    if (!string.IsNullOrEmpty(r.NameTag))
                    {
                        TagStringBuilder.Append(r.NameTag);
                    }
                    r.OnUpdateNameTags();
                }

                // Modifier Tags
                foreach (var m in PlayerModifier.GetModifiers(player))
                {
                    if (!string.IsNullOrEmpty(m.NameTag))
                    {
                        TagStringBuilder.Append(m.NameTag);
                    }
                    m.OnUpdateNameTags();
                }

                // Lovers
                if (Lovers.IsLovers(player) && (localPlayer.IsLovers() || MapSettings.GhostsSeeRoles && isLocalDead))
                {
                    TagStringBuilder.Append(Lovers.GetIcon(player));
                }
            }

            var resultText = TagStringBuilder.ToString();
            if (player.cosmetics.nameText.text != resultText)
            {
                player.cosmetics.nameText.text = resultText;
            }

            if (!ColorCache.TryGetValue(player.PlayerId, out var c))
            {
                continue;
            }
            if (player.cosmetics.nameText.color != c)
            {
                player.cosmetics.nameText.color = c;
            }
        }

        // 4. Update Meeting HUD
        if (MeetingHud.Instance != null)
        {
            var playersById = Helpers.AllPlayersById();
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                if (pva == null || pva.NameText == null)
                {
                    continue;
                }
                if (!playersById.TryGetValue(pva.TargetPlayerId, out var target) || target == null || target.Data == null)
                {
                    continue;
                }

                var baseName = target.Data.PlayerName;
                if (isLocalDead)
                {
                    if (string.IsNullOrEmpty(baseName) && Camouflager.CamouflageTimer > 0f)
                    {
                        baseName = target.Data.DefaultOutfit?.PlayerName ?? "";
                    }
                    else if (target.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    {
                        baseName = $"{target.CurrentOutfit?.PlayerName} ({target.Data.DefaultOutfit?.PlayerName})";
                    }
                }

                TagStringBuilder.Clear();
                TagStringBuilder.Append(baseName);

                // Role Tags (Meeting Only)
                var r = ModRoleManager.GetRole(target);
                if (r != null && !string.IsNullOrEmpty(r.NameTag))
                {
                    TagStringBuilder.Append(r.NameTag);
                }

                // Detective / Hacker
                if (MapSettings.ShowLighterDarker && meetingShow)
                {
                    TagStringBuilder
                        .Append(" (")
                        .Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId)
                            ? Tr.Get(TrKey.DetectiveLightLabel)
                            : Tr.Get(TrKey.DetectiveDarkLabel))
                        .Append(')');
                }

                // Lovers
                if (Lovers.IsLovers(target) && (localPlayer.IsLovers() || MapSettings.GhostsSeeRoles && isLocalDead))
                {
                    TagStringBuilder.Append(Lovers.GetIcon(target));
                }

                var resultText = TagStringBuilder.ToString();
                if (pva.NameText.text != resultText)
                {
                    pva.NameText.text = resultText;
                }

                if (!ColorCache.TryGetValue(target.PlayerId, out var c))
                {
                    continue;
                }
                if (pva.NameText.color != c)
                {
                    pva.NameText.color = c;
                }
            }
        }

        _isUpdating = false;
    }

    private static void UpdateImpostorKillButton(HudManager __instance)
    {
        if (__instance == null || __instance.KillButton == null)
        {
            return;
        }
        if (PlayerControl.LocalPlayer?.Data?.Role?.IsImpostor != true)
        {
            return;
        }
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        var enabled = Helpers.ShowButtons;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Vampire)
            || PlayerControl.LocalPlayer.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanKill
            || PlayerControl.LocalPlayer.IsRole(RoleType.Janitor))
        {
            enabled = false;
        }

        if (enabled)
        {
            __instance.KillButton.Show();
        }
        else
        {
            __instance.KillButton.Hide();
        }
    }

    private static void UpdateUseButton(HudManager __instance)
    {
        if (__instance?.UseButton == null)
        {
            return;
        }
        if (MeetingHud.Instance)
        {
            __instance.UseButton.Hide();
        }
    }

    private static void UpdateSabotageButton(HudManager __instance)
    {
        if (__instance?.SabotageButton == null)
        {
            return;
        }
        if (MeetingHud.Instance)
        {
            __instance.SabotageButton.Hide();
        }
    }

    private static void UpdateVentButton(HudManager __instance)
    {
        if (__instance?.ImpostorVentButton == null)
        {
            return;
        }
        if (MeetingHud.Instance)
        {
            __instance.ImpostorVentButton.Hide();
        }
    }

    private static void UpdateReportButton(HudManager __instance)
    {
        if (__instance?.ReportButton == null)
        {
            return;
        }
        if (MeetingHud.Instance)
        {
            __instance.ReportButton.Hide();
        }
    }

    private static void CamouflageAndMorphActions()
    {
        var oldCamouflageTimer = Camouflager.CamouflageTimer;
        var oldMorphTimer = Morphing.MorphTimer;

        Camouflager.CamouflageTimer -= Time.deltaTime;
        Morphing.MorphTimer -= Time.deltaTime;

        // Everyone but morphing reset
        if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f)
        {
            Camouflager.ResetCamouflage();
        }

        // Morphing reset
        if (oldMorphTimer > 0f && Morphing.MorphTimer <= 0f)
        {
            Morphing.ResetMorph();
        }
    }

    internal static void SetPlayerNameColor(PlayerControl p, Color color)
    {
        if (_isUpdating)
        {
            ColorCache[p.PlayerId] = color;
            return;
        }

        p.cosmetics.nameText.color = color;
        if (MeetingHud.Instance == null)
        {
            return;
        }
        foreach (var player in MeetingHud.Instance.playerStates)
        {
            if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
            {
                player.NameText.color = color;
            }
        }
    }
}