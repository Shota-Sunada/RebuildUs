namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilTracker : RoleBase<EvilTracker>
{
    public static Color RoleColor = Palette.ImpostorRed;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.evilTrackerCooldown.GetFloat(); } }
    public static bool resetTargetAfterMeeting { get { return CustomOptionHolder.evilTrackerResetTargetAfterMeeting.GetBool(); } }
    public static bool canSeeDeathFlash { get { return CustomOptionHolder.evilTrackerCanSeeDeathFlash.GetBool(); } }
    public static bool canSeeTargetTask { get { return CustomOptionHolder.evilTrackerCanSeeTargetTask.GetBool(); } }
    public static bool canSeeTargetPosition { get { return CustomOptionHolder.evilTrackerCanSeeTargetPosition.GetBool(); } }
    public static bool canSetTargetOnMeeting { get { return CustomOptionHolder.evilTrackerCanSetTargetOnMeeting.GetBool(); } }
    public static PlayerControl target;
    public static PlayerControl currentTarget;
    public static CustomButton trackerButton;
    public static Dictionary<string, TMP_Text> impostorPositionText;
    public static TMP_Text targetPositionText;

    public EvilTracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override void OnMeetingStart()
    {
        if (resetTargetAfterMeeting)
        {
            target = null;
        }
    }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilTracker))
        {
            arrowUpdate();
        }
        if (Player.IsAlive())
        {
            currentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(currentTarget, Palette.ImpostorRed);
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        trackerButton = new CustomButton(
            () =>
            {
                target = currentTarget;
            },
            () => { return target == null && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilTracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return currentTarget != null && target == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { trackerButton.Timer = trackerButton.MaxTimer; },
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
        trackerButton.MaxTimer = cooldown;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        target = null;
        currentTarget = null;
        arrows = [];
        impostorPositionText = [];
    }

    public static List<Arrow> arrows = [];
    public static float updateTimer = 0f;
    public static float arrowUpdateInterval = 0.5f;
    static void arrowUpdate()
    {
        // 前フレームからの経過時間をマイナスする
        updateTimer -= Time.fixedDeltaTime;

        // 1秒経過したらArrowを更新
        if (updateTimer <= 0.0f)
        {

            // 前回のArrowをすべて破棄する
            foreach (Arrow arrow in arrows)
            {
                if (arrow != null && arrow.ArrowObject != null)
                {
                    arrow.ArrowObject.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
            }

            // Arrows一覧
            arrows = [];

            // インポスターの位置を示すArrowsを描画
            int count = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p.Data.IsDead)
                {
                    if (p.IsTeamImpostor() && impostorPositionText.ContainsKey(p.name))
                    {
                        impostorPositionText[p.name].text = "";
                    }
                    continue;
                }
                Arrow arrow;
                if (p.IsTeamImpostor() && p != CachedPlayer.LocalPlayer.PlayerControl)
                {
                    arrow = new Arrow(Palette.ImpostorRed);
                    arrow.ArrowObject.SetActive(true);
                    arrow.Update(p.transform.position);
                    arrows.Add(arrow);
                    count += 1;
                    if (!impostorPositionText.ContainsKey(p.name))
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
                        impostorPositionText.Add(p.name, positionText);
                    }
                    PlainShipRoom room = Helpers.GetPlainShipRoom(p);
                    impostorPositionText[p.name].gameObject.SetActive(true);
                    if (room != null)
                    {
                        impostorPositionText[p.name].text = "<color=#FF1919FF>" + $"{p.name}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        impostorPositionText[p.name].text = "";
                    }
                }
            }

            // ターゲットの位置を示すArrowを描画
            if (target != null && !target.IsDead())
            {
                var arrow = new Arrow(Palette.CrewmateBlue);
                arrow.ArrowObject.SetActive(true);
                arrow.Update(target.transform.position);
                arrows.Add(arrow);
                if (targetPositionText == null)
                {
                    RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                    if (roomTracker == null) return;
                    GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                    UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                    gameObject.transform.localScale = Vector3.one * 1.0f;
                    targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                    targetPositionText.alpha = 1.0f;
                }
                PlainShipRoom room = Helpers.GetPlainShipRoom(target);
                targetPositionText.gameObject.SetActive(true);
                if (room != null)
                {
                    targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.name}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                }
                else
                {
                    targetPositionText.text = "";
                }
            }
            else
            {
                targetPositionText?.text = "";
            }

            // タイマーに時間をセット
            updateTimer = arrowUpdateInterval;
        }
    }
}