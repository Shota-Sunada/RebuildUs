namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Hacker : MultiRoleBase<Hacker>
{
    internal static Color NameColor = new Color32(117, 250, 76, byte.MaxValue);

    private static CustomButton _hackerButton;
    internal static CustomButton HackerVitalsButton;
    internal static CustomButton HackerAdminTableButton;
    private static TMP_Text _hackerAdminTableChargesText;
    private static TMP_Text _hackerVitalsChargesText;

    internal static float HackerTimer;
    private static SystemConsole _vitalsConsole;
    private static SystemConsole _doorLogConsole;
    private int _chargesAdminTable = 1;
    private int _chargesVitals = 1;
    private Minigame _doorLog;
    private int _rechargedTasks = 2;

    private Minigame _vitals;

    public Hacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Hacker;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.HackerCooldown.GetFloat();
    }

    private static float Duration
    {
        get => CustomOptionHolder.HackerHackingDuration.GetFloat();
    }

    internal static bool OnlyColorType
    {
        get => CustomOptionHolder.HackerOnlyColorType.GetBool();
    }

    private static float ToolsNumber
    {
        get => CustomOptionHolder.HackerToolsNumber.GetFloat();
    }

    private static int RechargeTasksNumber
    {
        get => Mathf.RoundToInt(CustomOptionHolder.HackerRechargeTasksNumber.GetFloat());
    }

    private static bool NoMove
    {
        get => CustomOptionHolder.HackerNoMove.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) || !PlayerControl.LocalPlayer.IsAlive())
        {
            return;
        }
        (int playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
        if (playerCompleted != _rechargedTasks)
        {
            return;
        }
        _rechargedTasks += RechargeTasksNumber;
        if (ToolsNumber > _chargesVitals)
        {
            _chargesVitals++;
        }
        if (ToolsNumber > _chargesAdminTable)
        {
            _chargesAdminTable++;
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _hackerButton = new(() => HackerTimer = Duration,
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && PlayerControl.LocalPlayer.IsAlive(),
            () => true,
            () =>
            {
                _hackerButton.Timer = _hackerButton.MaxTimer;
                _hackerButton.IsEffectActive = false;
                _hackerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.HackerButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            true,
            0f,
            () =>
            {
                _hackerButton.Timer = _hackerButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.HackerText));

        HackerAdminTableButton = new(() =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local._chargesAdminTable--;
                HudManager.Instance.ToggleMapVisible(new()
                {
                    Mode = MapOptions.Modes.CountOverlay,
                    AllowMovementWhileMapOpen = !NoMove,
                    ShowLivePlayerPosition = true,
                    IncludeDeadBodies = true,
                });
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && MapSettings.CouldUseAdmin && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (_hackerAdminTableChargesText == null && _hackerVitalsChargesText == null)
                {
                    return Local._chargesAdminTable > 0 && MapSettings.CanUseAdmin;
                }
                string format = Tr.Get(TrKey.HackerChargesText);
                string text = string.Format(format, Local._chargesAdminTable, ToolsNumber);
                _hackerAdminTableChargesText?.text = text;
                _hackerVitalsChargesText?.text = text;

                return Local._chargesAdminTable > 0 && MapSettings.CanUseAdmin;
                ;
            },
            () =>
            {
                HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
                HackerAdminTableButton.IsEffectActive = false;
                HackerAdminTableButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            GetAdminSprite(),
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            true,
            0f,
            () =>
            {
                HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
                if (!HackerVitalsButton.IsEffectActive)
                {
                    PlayerControl.LocalPlayer.moveable = true;
                }
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled)
                {
                    MapBehaviour.Instance.Close();
                }
            },
            Helpers.GetOption(ByteOptionNames.MapId) == 3,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin));

        // Hacker Admin Table Charges
        _hackerAdminTableChargesText = UnityObject.Instantiate(HackerAdminTableButton.ActionButton.cooldownTimerText,
            HackerAdminTableButton.ActionButton.cooldownTimerText.transform.parent);
        _hackerAdminTableChargesText.text = "";
        _hackerAdminTableChargesText.enableWordWrapping = false;
        _hackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
        _hackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        HackerVitalsButton = new(() =>
            {
                if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
                {
                    if (Local._vitals == null)
                    {
                        if (_vitalsConsole == null)
                        {
                            Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                            for (int i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("panel_vitals"))
                                {
                                    _vitalsConsole = consoles[i];
                                    break;
                                }
                            }
                        }

                        if (_vitalsConsole == null || Camera.main == null)
                        {
                            return;
                        }
                        Local._vitals = UnityObject.Instantiate(_vitalsConsole.MinigamePrefab, Camera.main.transform, false);
                    }

                    if (Camera.main != null)
                    {
                        Local._vitals.transform.SetParent(Camera.main.transform, false);
                    }
                    Local._vitals.transform.localPosition = new(0.0f, 0.0f, -50f);
                    Local._vitals.Begin(null);
                }
                else
                {
                    if (Local._doorLog == null)
                    {
                        if (_doorLogConsole == null)
                        {
                            Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                            for (int i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("SurvLogConsole"))
                                {
                                    _doorLogConsole = consoles[i];
                                    break;
                                }
                            }
                        }

                        if (_doorLogConsole == null || Camera.main == null)
                        {
                            return;
                        }
                        Local._doorLog = UnityObject.Instantiate(_doorLogConsole.MinigamePrefab, Camera.main.transform, false);
                    }

                    if (Camera.main != null)
                    {
                        Local._doorLog.transform.SetParent(Camera.main.transform, false);
                    }
                    Local._doorLog.transform.localPosition = new(0.0f, 0.0f, -50f);
                    Local._doorLog.Begin(null);
                }

                if (NoMove)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                }
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement

                Local._chargesVitals--;
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Hacker)
                  && MapSettings.CouldUseVitals
                  && PlayerControl.LocalPlayer.IsAlive()
                  && Helpers.GetOption(ByteOptionNames.MapId) != 0
                  && Helpers.GetOption(ByteOptionNames.MapId) != 3,
            () =>
            {
                _hackerVitalsChargesText?.text = string.Format(Tr.Get(TrKey.HackerChargesText), Local._chargesVitals, ToolsNumber);
                HackerVitalsButton.ActionButton.graphic.sprite = Helpers.IsMiraHq
                    ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image
                    : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
                HackerVitalsButton.ActionButton.OverrideText(Helpers.IsMiraHq
                    ? TranslationController.Instance.GetString(StringNames.DoorlogLabel)
                    : TranslationController.Instance.GetString(StringNames.VitalsLabel));
                return Local._chargesVitals > 0 && MapSettings.CanUseVitals;
            },
            () =>
            {
                HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
                HackerVitalsButton.IsEffectActive = false;
                HackerVitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CommonAbilitySecondary,
            true,
            0f,
            () =>
            {
                HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
                if (!HackerAdminTableButton.IsEffectActive)
                {
                    PlayerControl.LocalPlayer.moveable = true;
                }
                if (!Minigame.Instance)
                {
                    return;
                }
                if (Helpers.IsMiraHq)
                {
                    Local._doorLog.ForceClose();
                }
                else
                {
                    Local._vitals.ForceClose();
                }
            },
            false,
            Helpers.IsMiraHq
                ? TranslationController.Instance.GetString(StringNames.DoorlogLabel)
                : TranslationController.Instance.GetString(StringNames.VitalsLabel));

        // Hacker Vitals Charges
        _hackerVitalsChargesText = UnityObject.Instantiate(HackerVitalsButton.ActionButton.cooldownTimerText,
            HackerVitalsButton.ActionButton.cooldownTimerText.transform.parent);
        _hackerVitalsChargesText.text = "";
        _hackerVitalsChargesText.enableWordWrapping = false;
        _hackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
        _hackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    internal static void SetButtonCooldowns()
    {
        _hackerButton.MaxTimer = Cooldown;
        HackerVitalsButton.MaxTimer = Cooldown;
        HackerAdminTableButton.MaxTimer = Cooldown;
        _hackerButton.EffectDuration = Duration;
        HackerVitalsButton.EffectDuration = Duration;
        HackerAdminTableButton.EffectDuration = Duration;
    }

    internal static Sprite GetAdminSprite()
    {
        byte mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
        UseButtonSettings button = mapId switch
        {
            0 or 3 => FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton],
            1 => FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton],
            4 => FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton],
            _ => FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton],
        };

        return button.Image;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        HackerTimer = 0f;
        Players.Clear();
    }
}