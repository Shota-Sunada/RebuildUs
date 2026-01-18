namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilTracker : RoleBase<EvilTracker>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.EvilTrackerCooldown.GetFloat(); } }
    public static bool ResetTargetAfterMeeting { get { return CustomOptionHolder.EvilTrackerResetTargetAfterMeeting.GetBool(); } }
    public static bool CanSeeDeathFlash { get { return CustomOptionHolder.EvilTrackerCanSeeDeathFlash.GetBool(); } }
    public static bool CanSeeTargetTask { get { return CustomOptionHolder.EvilTrackerCanSeeTargetTask.GetBool(); } }
    public static bool CanSeeTargetPosition { get { return CustomOptionHolder.EvilTrackerCanSeeTargetPosition.GetBool(); } }
    public static bool CanSetTargetOnMeeting { get { return CustomOptionHolder.EvilTrackerCanSetTargetOnMeeting.GetBool(); } }
    public static PlayerControl Target;
    public static PlayerControl CurrentTarget;
    public static CustomButton TrackerButton;
    public static Dictionary<string, TMP_Text> ImpostorPositionText;
    public static TMP_Text TargetPositionText;

    public EvilTracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override void OnMeetingStart()
    {
        if (ResetTargetAfterMeeting)
        {
            Target = null;
        }
    }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilTracker))
        {
            ArrowUpdate();
        }
        if (Player.IsAlive())
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        TrackerButton = new CustomButton(
            () =>
            {
                Target = CurrentTarget;
            },
            () => { return Target == null && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilTracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return CurrentTarget != null && Target == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { TrackerButton.Timer = TrackerButton.MaxTimer; },
            AssetLoader.TrackerButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("TrackerText")
        };
    }
    public override void SetButtonCooldowns()
    {
        TrackerButton.MaxTimer = Cooldown;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        Target = null;
        CurrentTarget = null;
        Arrows = [];
        ImpostorPositionText = [];
    }

    public static List<Arrow> Arrows = [];
    public static float UpdateTimer = 0f;
    public static float ArrowUpdateInterval = 0.5f;
    static void ArrowUpdate()
    {
        // 前フレームからの経過時間をマイナスする
        UpdateTimer -= Time.fixedDeltaTime;

        // 1秒経過したらArrowを更新
        if (UpdateTimer <= 0.0f)
        {

            // 前回のArrowをすべて破棄する
            foreach (Arrow arrow in Arrows)
            {
                if (arrow != null && arrow.ArrowObject != null)
                {
                    arrow.ArrowObject.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
            }

            // Arrows一覧
            Arrows = [];

            // インポスターの位置を示すArrowsを描画
            int count = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead)
                {
                    if (p.IsTeamImpostor() && ImpostorPositionText.ContainsKey(p.name))
                    {
                        ImpostorPositionText[p.name].text = "";
                    }
                    continue;
                }
                Arrow arrow;
                if (p.IsTeamImpostor() && p != CachedPlayer.LocalPlayer.PlayerControl)
                {
                    arrow = new Arrow(Palette.ImpostorRed);
                    arrow.ArrowObject.SetActive(true);
                    arrow.Update(p.transform.position);
                    Arrows.Add(arrow);
                    count += 1;
                    if (!ImpostorPositionText.ContainsKey(p.name))
                    {
                        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f + 0.25f * count, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        TMPro.TMP_Text positionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        positionText.alpha = 1.0f;
                        ImpostorPositionText.Add(p.name, positionText);
                    }
                    PlainShipRoom room = Helpers.GetPlainShipRoom(p);
                    ImpostorPositionText[p.name].gameObject.SetActive(true);
                    if (room != null)
                    {
                        ImpostorPositionText[p.name].text = "<color=#FF1919FF>" + $"{p.name}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        ImpostorPositionText[p.name].text = "";
                    }
                }
            }

            // ターゲットの位置を示すArrowを描画
            if (Target != null && !Target.IsDead())
            {
                var arrow = new Arrow(Palette.CrewmateBlue);
                arrow.ArrowObject.SetActive(true);
                arrow.Update(Target.transform.position);
                Arrows.Add(arrow);
                if (TargetPositionText == null)
                {
                    RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                    if (roomTracker == null) return;
                    GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                    UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                    gameObject.transform.localScale = Vector3.one * 1.0f;
                    TargetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                    TargetPositionText.alpha = 1.0f;
                }
                PlainShipRoom room = Helpers.GetPlainShipRoom(Target);
                TargetPositionText.gameObject.SetActive(true);
                if (room != null)
                {
                    TargetPositionText.text = "<color=#8CFFFFFF>" + $"{Target.name}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                }
                else
                {
                    TargetPositionText.text = "";
                }
            }
            else
            {
                TargetPositionText?.text = "";
            }

            // タイマーに時間をセット
            UpdateTimer = ArrowUpdateInterval;
        }
    }
}