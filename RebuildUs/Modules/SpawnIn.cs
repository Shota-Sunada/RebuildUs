using PowerTools;
using UnityEngine.Events;

namespace RebuildUs.Modules;

internal static class SpawnIn
{
    private static PassiveButton _selected;
    private static List<SpawnCandidate> _spawnCandidates;
    internal static readonly SynchronizeData SynchronizeData = new();
    private static bool _isFirstSpawn = true;
    private static float InitialDoorCooldown { get => CustomOptionHolder.AirshipInitialDoorCooldown.GetFloat(); }
    private static float InitialSabotageCooldown { get => CustomOptionHolder.AirshipInitialSabotageCooldown.GetFloat(); }

    internal static void Reset()
    {
        _isFirstSpawn = true;
        ResetSpawnCandidates();
    }

    private static void ResetSpawnCandidates()
    {
        _spawnCandidates = [];
        if (!CustomOptionHolder.AirshipAdditionalSpawn.GetBool()) return;
        _spawnCandidates.Add(new(StringNames.VaultRoom, new(-8.8f, 8.6f), AssetLoader.VaultButton));
        _spawnCandidates.Add(new(StringNames.MeetingRoom, new(11.0f, 14.7f), AssetLoader.MeetingButton));
        _spawnCandidates.Add(new(StringNames.Cockpit, new(-22.0f, -1.2f), AssetLoader.CockpitButton));
        _spawnCandidates.Add(new(StringNames.Electrical, new(16.4f, -8.5f), AssetLoader.ElectricalButton));
        _spawnCandidates.Add(new(StringNames.Lounge, new(30.9f, 7.5f), AssetLoader.LoungeButton));
        _spawnCandidates.Add(new(StringNames.Medical, new(25.5f, -5.0f), AssetLoader.MedicalButton));
        _spawnCandidates.Add(new(StringNames.Security, new(10.3f, -16.2f), AssetLoader.SecurityButton));
        _spawnCandidates.Add(new(StringNames.ViewingDeck, new(-14.1f, -16.2f), AssetLoader.ViewingButton));
        _spawnCandidates.Add(new(StringNames.Armory, new(-10.7f, -6.3f), AssetLoader.ArmoryButton));
        _spawnCandidates.Add(new(StringNames.Comms, new(-11.8f, 3.2f), AssetLoader.CommunicationsButton));
        _spawnCandidates.Add(new(StringNames.Showers, new(20.8f, 2.8f), AssetLoader.ShowersButton));
        _spawnCandidates.Add(new(StringNames.GapRoom, new(13.8f, 6.4f), AssetLoader.GapButton));
    }

    private static void ResetButtons()
    {
        // MapUtilities.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().ForceSabTime(10f);
        _isFirstSpawn = false;
        if (CustomOptionHolder.AirshipSetOriginalCooldown.GetBool())
        {
            PlayerControl.LocalPlayer.SetKillTimerUnchecked(Helpers.GetOption(FloatOptionNames.KillCooldown));
            foreach (CustomButton t in CustomButton.Buttons) t.Timer = t.MaxTimer;
        }
        else
        {
            PlayerControl.LocalPlayer.SetKillTimerUnchecked(10f);
            foreach (CustomButton t in CustomButton.Buttons) t.Timer = 10f;
        }
    }

    internal static bool BeginPrefix(SpawnInMinigame __instance, PlayerTask task)
    {
        CustomButton.StopCountdown = true;
        // base.Begin(task);
        __instance.MyTask = task;
        __instance.MyNormTask = task as NormalPlayerTask;
        if (PlayerControl.LocalPlayer)
        {
            if (MapBehaviour.Instance) MapBehaviour.Instance.Close();

            PlayerControl.LocalPlayer.NetTransform.Halt();
        }

        __instance.StartCoroutine(__instance.CoAnimateOpen());

        List<SpawnInMinigame.SpawnLocation> list = [];
        foreach (SpawnInMinigame.SpawnLocation loc in __instance.Locations) list.Add(loc);

        foreach (SpawnCandidate spawnCandidate in _spawnCandidates)
        {
            SpawnInMinigame.SpawnLocation spawnLocation = new()
            {
                Location = spawnCandidate.SpawnLocation,
                Image = spawnCandidate.Sprite,
                Name = spawnCandidate.LocationKey,
                Rollover = new(),
                RolloverSfx = __instance.DefaultRolloverSound,
            };
            list.Add(spawnLocation);
        }

        // 手動シャッフル
        System.Random rnd = RebuildUs.Rnd;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        // Take と手動ソート
        int takeCount = Math.Min(list.Count, __instance.LocationButtons.Length);
        List<SpawnInMinigame.SpawnLocation> sortedList = [];
        for (int i = 0; i < takeCount; i++) sortedList.Add(list[i]);

        sortedList.Sort((a, b) =>
        {
            int res = a.Location.x.CompareTo(b.Location.x);
            return res != 0 ? res : b.Location.y.CompareTo(a.Location.y);
        });

        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new(-25f, 40f));

        for (int i = 0; i < sortedList.Count; i++)
        {
            PassiveButton passiveButton = __instance.LocationButtons[i];
            SpawnInMinigame.SpawnLocation pt = sortedList[i];
            passiveButton.OnClick.AddListener((UnityAction)(() => SpawnAt(__instance, pt.Location)));
            passiveButton.GetComponent<SpriteAnim>().Stop();
            passiveButton.GetComponent<SpriteRenderer>().sprite = pt.Image;
            passiveButton.GetComponentInChildren<TextMeshPro>().text = FastDestroyableSingleton<TranslationController>.Instance.GetString(pt.Name, new Il2CppReferenceArray<CppObject>(0));
            ButtonAnimRolloverHandler component = passiveButton.GetComponent<ButtonAnimRolloverHandler>();
            component.StaticOutImage = pt.Image;
            component.RolloverAnim = pt.Rollover;
            component.HoverSound = pt.RolloverSfx ? pt.RolloverSfx : __instance.DefaultRolloverSound;
        }

        PlayerControl.LocalPlayer.gameObject.SetActive(false);
        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new(-25f, 40f));
        if (CustomOptionHolder.AirshipRandomSpawn.GetBool())
            __instance.LocationButtons.Random().ReceiveClickUp();
        else
            __instance.StartCoroutine(__instance.RunTimer());

        ControllerManager.Instance.OpenOverlayMenu(__instance.name, null, __instance.DefaultButtonSelected, __instance.ControllerSelectable);
        PlayerControl.HideCursorTemporarily();
        ConsoleJoystick.SetMode_Menu();
        return false;
    }

    internal static void BeginPostfix(SpawnInMinigame __instance)
    {
        _selected = null;

        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool()) return;

        foreach (PassiveButton button in __instance.LocationButtons)
        {
            button.OnClick.AddListener((UnityAction)(() =>
            {
                if (_selected == null)
                    _selected = button;
            }));
        }
    }

    private static void Synchronize(SynchronizeTag tag, byte playerId)
    {
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.Synchronize);
        sender.Write(playerId);
        sender.Write((int)tag);
        RPCProcedure.Synchronize(playerId, (int)tag);
    }

    private static void SpawnAt(SpawnInMinigame __instance, Vector3 spawnAt)
    {
        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool())
        {
            if (_isFirstSpawn) ResetButtons();
            CustomButton.StopCountdown = false;
            if (__instance.amClosing != Minigame.CloseState.None) return;

            __instance.gotButton = true;
            PlayerControl.LocalPlayer.gameObject.SetActive(true);
            __instance.StopAllCoroutines();
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(spawnAt);
            FastDestroyableSingleton<HudManager>.Instance.PlayerCam.SnapToTarget();
            __instance.Close();
        }
        else
        {
            Synchronize(SynchronizeTag.PreSpawnMinigame, PlayerControl.LocalPlayer.PlayerId);
            if (__instance.amClosing != Minigame.CloseState.None) return;

            if (__instance.gotButton) return;

            __instance.gotButton = true;

            foreach (PassiveButton button in __instance.LocationButtons) button.enabled = false;

            __instance.StartCoroutine(Effects.Lerp(10f, new Action<float>(p =>
            {
                float time = p * 10f;
                bool aligned = SynchronizeData.Align(SynchronizeTag.PreSpawnMinigame, false) || p == 1f;

                foreach (PassiveButton button in __instance.LocationButtons)
                {
                    if (_selected == button)
                    {
                        if (!(time > 0.3f)) continue;
                        Vector3 pos = button.transform.localPosition;
                        float x = pos.x;
                        switch (x)
                        {
                            case < 0f:
                                x += 10f * Time.deltaTime;
                                break;
                            case > 0f:
                                x -= 10f * Time.deltaTime;
                                break;
                        }

                        if (Mathf.Abs(x) < 10f * Time.deltaTime) x = 0f;
                        button.transform.localPosition = new(x, pos.y, pos.z);
                    }
                    else
                    {
                        SpriteRenderer sr = button.GetComponent<SpriteRenderer>();
                        Color color = sr.color;
                        float a = color.a;
                        if (a > 0f) a -= 2f * Time.deltaTime;
                        if (a < 0f) a = 0f;
                        sr.color = new(color.r, color.g, color.b, a);
                        button.GetComponentInChildren<TextMeshPro>().color = new(1f, 1f, 1f, a);
                    }
                }

                if (__instance.amClosing != Minigame.CloseState.None) return;

                if (!aligned) return;
                PlayerControl.LocalPlayer.gameObject.SetActive(true);
                __instance.StopAllCoroutines();
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(spawnAt);
                FastDestroyableSingleton<HudManager>.Instance.PlayerCam.SnapToTarget();
                SynchronizeData.Reset(SynchronizeTag.PreSpawnMinigame);
                __instance.Close();
                CustomButton.StopCountdown = false;
                // サボタージュのクールダウンをリセット
                SabotageSystemType sabotageSystem = MapUtilities.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                sabotageSystem.IsDirty = true;
                sabotageSystem.Timer = InitialSabotageCooldown;
                DoorsSystemType doorSystem = MapUtilities.Systems[SystemTypes.Doors].Cast<DoorsSystemType>();
                doorSystem.IsDirty = true;
                doorSystem.timers[SystemTypes.MainHall] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Brig] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Comms] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Medical] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Engine] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Records] = InitialDoorCooldown;
                doorSystem.timers[SystemTypes.Kitchen] = InitialDoorCooldown;

                if (_isFirstSpawn) ResetButtons();
            })));
        }
    }

    internal static void MoveNextPostfix(SpawnInMinigame._RunTimer_d__10 __instance)
    {
        if (!CustomOptionHolder.AirshipSynchronizedSpawning.GetBool() || CustomOptionHolder.AirshipRandomSpawn.GetBool()) return;
        if (_selected != null) __instance.__4__this.Text.text = Tr.Get(TrKey.AirshipWait);
    }
}