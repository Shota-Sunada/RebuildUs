using PowerTools;

namespace RebuildUs.Modules;

public static class SpawnIn
{
    private static PassiveButton Selected = null;
    public static List<SpawnCandidate> SpawnCandidates;
    public static SynchronizeData SynchronizeData = new();
    public static bool IsFirstSpawn = true;
    public static float InitialDoorCooldown { get { return CustomOptionHolder.AirshipInitialDoorCooldown.GetFloat(); } }
    public static float InitialSabotageCooldown { get { return CustomOptionHolder.AirshipInitialSabotageCooldown.GetFloat(); } }

    public static void Reset()
    {
        IsFirstSpawn = true;
        ResetSpawnCandidates();
    }

    public static void ResetSpawnCandidates()
    {
        SpawnCandidates = [];
        if (CustomOptionHolder.AirshipAdditionalSpawn.GetBool())
        {
            SpawnCandidates.Add(new(StringNames.VaultRoom, new Vector2(-8.8f, 8.6f), AssetLoader.VaultButton));
            SpawnCandidates.Add(new(StringNames.MeetingRoom, new Vector2(11.0f, 14.7f), AssetLoader.MeetingButton));
            SpawnCandidates.Add(new(StringNames.Cockpit, new Vector2(-22.0f, -1.2f), AssetLoader.CockpitButton));
            SpawnCandidates.Add(new(StringNames.Electrical, new Vector2(16.4f, -8.5f), AssetLoader.ElectricalButton));
            SpawnCandidates.Add(new(StringNames.Lounge, new Vector2(30.9f, 7.5f), AssetLoader.LoungeButton));
            SpawnCandidates.Add(new(StringNames.Medical, new Vector2(25.5f, -5.0f), AssetLoader.MedicalButton));
            SpawnCandidates.Add(new(StringNames.Security, new Vector2(10.3f, -16.2f), AssetLoader.SecurityButton));
            SpawnCandidates.Add(new(StringNames.ViewingDeck, new Vector2(-14.1f, -16.2f), AssetLoader.ViewingButton));
            SpawnCandidates.Add(new(StringNames.Armory, new Vector2(-10.7f, -6.3f), AssetLoader.ArmoryButton));
            SpawnCandidates.Add(new(StringNames.Comms, new Vector2(-11.8f, 3.2f), AssetLoader.CommunicationsButton));
            SpawnCandidates.Add(new(StringNames.Showers, new Vector2(20.8f, 2.8f), AssetLoader.ShowersButton));
            SpawnCandidates.Add(new(StringNames.GapRoom, new Vector2(13.8f, 6.4f), AssetLoader.GapButton));
        }
    }

    private static void ResetButtons()
    {
        // MapUtilities.CachedShipStatus.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().ForceSabTime(10f);
        IsFirstSpawn = false;
        if (CustomOptionHolder.AirshipSetOriginalCooldown.GetBool())
        {
            CachedPlayer.LocalPlayer.PlayerControl.SetKillTimerUnchecked(Helpers.GetOption(FloatOptionNames.KillCooldown));
            foreach (var b in CustomButton.Buttons)
            {
                b.Timer = b.MaxTimer;
            }
        }
        else
        {
            CachedPlayer.LocalPlayer.PlayerControl.SetKillTimerUnchecked(10f);
            CustomButton.Buttons.ForEach(x => x.Timer = 10f);
        }
    }

    public static bool BeginPrefix(SpawnInMinigame __instance, PlayerTask task)
    {
        CustomButton.StopCountdown = true;
        // base.Begin(task);
        __instance.MyTask = task;
        __instance.MyNormTask = task as NormalPlayerTask;
        if (CachedPlayer.LocalPlayer.PlayerControl)
        {
            if (MapBehaviour.Instance)
            {
                MapBehaviour.Instance.Close();
            }
            CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt();
        }
        __instance.StartCoroutine(__instance.CoAnimateOpen());

        List<SpawnInMinigame.SpawnLocation> list = [.. __instance.Locations];
        foreach (var spawnCandidate in SpawnCandidates)
        {
            SpawnInMinigame.SpawnLocation spawnLocation = new()
            {
                Location = spawnCandidate.SpawnLocation,
                Image = spawnCandidate.Sprite,
                Name = spawnCandidate.LocationKey,
                Rollover = new AnimationClip(),
                RolloverSfx = __instance.DefaultRolloverSound
            };
            list.Add(spawnLocation);
        }

        SpawnInMinigame.SpawnLocation[] array = [.. list];
        array.Shuffle(0);
        array = [.. from s in array.Take(__instance.LocationButtons.Length)
                 orderby s.Location.x, s.Location.y descending
                 select s];
        CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(new Vector2(-25f, 40f));

        for (int i = 0; i < __instance.LocationButtons.Length; i++)
        {
            PassiveButton passiveButton = __instance.LocationButtons[i];
            SpawnInMinigame.SpawnLocation pt = array[i];
            passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => SpawnAt(__instance, pt.Location)));
            passiveButton.GetComponent<SpriteAnim>().Stop();
            passiveButton.GetComponent<SpriteRenderer>().sprite = pt.Image;
            // passiveButton.GetComponentInChildren<TextMeshPro>().text = FastDestroyableSingleton<TranslationController>.Instance.GetString(pt.Name, Array.Empty<object>());
            passiveButton.GetComponentInChildren<TextMeshPro>().text = FastDestroyableSingleton<TranslationController>.Instance.GetString(pt.Name, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
            ButtonAnimRolloverHandler component = passiveButton.GetComponent<ButtonAnimRolloverHandler>();
            component.StaticOutImage = pt.Image;
            component.RolloverAnim = pt.Rollover;
            component.HoverSound = pt.RolloverSfx ? pt.RolloverSfx : __instance.DefaultRolloverSound;
        }

        CachedPlayer.LocalPlayer.PlayerControl.gameObject.SetActive(false);
        CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(new Vector2(-25f, 40f));
        if (CustomOptionHolder.AirshipRandomSpawn.GetBool())
        {
            __instance.LocationButtons.Random<PassiveButton>().ReceiveClickUp();
        }
        else
        {
            __instance.StartCoroutine(__instance.RunTimer());
        }
        ControllerManager.Instance.OpenOverlayMenu(__instance.name, null, __instance.DefaultButtonSelected, __instance.ControllerSelectable, false);
        PlayerControl.HideCursorTemporarily();
        ConsoleJoystick.SetMode_Menu();
        return false;
    }

    public static void BeginPostfix(SpawnInMinigame __instance)
    {
        Selected = null;

        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool()) return;

        foreach (var button in __instance.LocationButtons)
        {
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                if (Selected == null)
                    Selected = button;
            }
            ));
        }
    }

    public static void Synchronize(SynchronizeTag tag, byte playerId)
    {
        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.Synchronize);
        sender.Write(playerId);
        sender.Write((int)tag);
        RPCProcedure.Synchronize(playerId, (int)tag);
    }

    public static void SpawnAt(SpawnInMinigame __instance, Vector3 spawnAt)
    {
        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool())
        {
            if (IsFirstSpawn) ResetButtons();
            CustomButton.StopCountdown = false;
            if (__instance.amClosing != Minigame.CloseState.None)
            {
                return;
            }
            __instance.gotButton = true;
            CachedPlayer.LocalPlayer.PlayerControl.gameObject.SetActive(true);
            __instance.StopAllCoroutines();
            CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(spawnAt);
            FastDestroyableSingleton<HudManager>.Instance.PlayerCam.SnapToTarget();
            __instance.Close();
        }
        else
        {
            Synchronize(SynchronizeTag.PreSpawnMinigame, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
            if (__instance.amClosing != Minigame.CloseState.None)
            {
                return;
            }
            if (__instance.gotButton) return;

            __instance.gotButton = true;

            foreach (var button in __instance.LocationButtons)
            {
                button.enabled = false;
            }

            __instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
            {
                float time = p * 10f;

                foreach (var button in __instance.LocationButtons)
                {
                    if (Selected == button)
                    {
                        if (time > 0.3f)
                        {
                            float x = button.transform.localPosition.x;
                            if (x < 0f) x += 10f * Time.deltaTime;
                            if (x > 0f) x -= 10f * Time.deltaTime;
                            if (Mathf.Abs(x) < 10f * Time.deltaTime) x = 0f;
                            button.transform.localPosition = new Vector3(x, button.transform.localPosition.y, button.transform.localPosition.z);
                        }
                    }
                    else
                    {
                        var color = button.GetComponent<SpriteRenderer>().color;
                        float a = color.a;
                        if (a > 0f) a -= 2f * Time.deltaTime;
                        if (a < 0f) a = 0f;
                        button.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, a);
                        button.GetComponentInChildren<TextMeshPro>().color = new Color(1f, 1f, 1f, a);
                    }

                    if (__instance.amClosing != Minigame.CloseState.None) return;

                    if (SynchronizeData.Align(SynchronizeTag.PreSpawnMinigame, false) || p == 1f)
                    {
                        CachedPlayer.LocalPlayer.PlayerControl.gameObject.SetActive(true);
                        __instance.StopAllCoroutines();
                        CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(spawnAt);
                        FastDestroyableSingleton<HudManager>.Instance.PlayerCam.SnapToTarget();
                        SynchronizeData.Reset(SynchronizeTag.PreSpawnMinigame);
                        __instance.Close();
                        CustomButton.StopCountdown = false;
                        // サボタージュのクールダウンをリセット
                        var sabotageSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                        sabotageSystem.IsDirty = true;
                        sabotageSystem.Timer = InitialSabotageCooldown;
                        var doorSystem = MapUtilities.CachedShipStatus.Systems[SystemTypes.Doors].Cast<DoorsSystemType>();
                        doorSystem.IsDirty = true;
                        doorSystem.timers[SystemTypes.MainHall] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Brig] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Comms] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Medical] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Engine] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Records] = InitialDoorCooldown;
                        doorSystem.timers[SystemTypes.Kitchen] = InitialDoorCooldown;

                        if (IsFirstSpawn) ResetButtons();
                    }
                }
            })));
            return;
        }
    }

    public static void MoveNextPostfix(SpawnInMinigame._RunTimer_d__10 __instance)
    {
        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool()) return;
        if (Selected != null)
        {
            __instance.__4__this.Text.text = Tr.Get("airshipWait");
        }
    }
}