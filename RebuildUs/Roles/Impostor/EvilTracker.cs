using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class EvilTracker : RoleBase<EvilTracker>
{
    public static Color NameColor = Palette.ImpostorRed;
    public static PlayerControl Target;
    public static PlayerControl CurrentTarget;
    public static CustomButton TrackerButton;
    public static Dictionary<string, TMP_Text> ImpostorPositionText;
    public static TMP_Text TargetPositionText;

    public static List<Arrow> Arrows = [];
    public static float UpdateTimer;
    public static float ArrowUpdateInterval = 0.5f;

    public EvilTracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.EvilTrackerCooldown.GetFloat();
    }

    public static bool ResetTargetAfterMeeting
    {
        get => CustomOptionHolder.EvilTrackerResetTargetAfterMeeting.GetBool();
    }

    public static bool CanSeeDeathFlash
    {
        get => CustomOptionHolder.EvilTrackerCanSeeDeathFlash.GetBool();
    }

    public static bool CanSeeTargetTask
    {
        get => CustomOptionHolder.EvilTrackerCanSeeTargetTask.GetBool();
    }

    public static bool CanSeeTargetPosition
    {
        get => CustomOptionHolder.EvilTrackerCanSeeTargetPosition.GetBool();
    }

    public static bool CanSetTargetOnMeeting
    {
        get => CustomOptionHolder.EvilTrackerCanSetTargetOnMeeting.GetBool();
    }

    public override void OnMeetingStart()
    {
        if (ResetTargetAfterMeeting) Target = null;
    }

    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            ArrowUpdate();

            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }

    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        TrackerButton = new(() => { Target = CurrentTarget; }, () => { return Target == null && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return CurrentTarget != null && Target == null && PlayerControl.LocalPlayer.CanMove; }, () => { TrackerButton.Timer = TrackerButton.MaxTimer; }, AssetLoader.TrackerButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.TrackerText));
    }

    public static void SetButtonCooldowns()
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

    private static void ArrowUpdate()
    {
        // 前フレームからの経過時間をマイナスする
        UpdateTimer -= Time.fixedDeltaTime;

        // 1秒経過したらArrowを更新
        if (UpdateTimer <= 0.0f)
        {
            // 前回のArrowをすべて破棄する
            for (var i = 0; i < Arrows.Count; i++)
            {
                var arrow = Arrows[i];
                if (arrow != null && arrow.ArrowObject != null)
                {
                    arrow.ArrowObject.SetActive(false);
                    Object.Destroy(arrow.ArrowObject);
                }
            }

            // Arrows一覧
            Arrows.Clear();

            // インポスターの位置を示すArrowsを描画
            var count = 0;
            var sb = new StringBuilder();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data.IsDead)
                {
                    if (p.IsTeamImpostor() && ImpostorPositionText.TryGetValue(p.name, out var txt)) txt.text = "";
                    continue;
                }

                if (p.IsTeamImpostor() && p.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
                    var arrow = new Arrow(Palette.ImpostorRed);
                    arrow.ArrowObject.SetActive(true);
                    arrow.Update(p.transform.position);
                    Arrows.Add(arrow);
                    count += 1;
                    if (!ImpostorPositionText.TryGetValue(p.name, out var positionText))
                    {
                        var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        var gameObject = Object.Instantiate(roomTracker.gameObject);
                        Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new(0, -2.0f + (0.25f * count), gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        positionText = gameObject.GetComponent<TMP_Text>();
                        positionText.alpha = 1.0f;
                        ImpostorPositionText.Add(p.name, positionText);
                    }

                    var room = Helpers.GetPlainShipRoom(p);
                    positionText.gameObject.SetActive(true);
                    if (room != null)
                    {
                        sb.Clear();
                        sb.Append("<color=#FF1919FF>");
                        sb.Append(p.name);
                        sb.Append('(');
                        sb.Append(FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId));
                        sb.Append(")</color>");
                        positionText.text = sb.ToString();
                    }
                    else
                        positionText.text = "";
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
                    var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                    if (roomTracker == null) return;
                    var gameObject = Object.Instantiate(roomTracker.gameObject);
                    Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.transform.localPosition = new(0, -2.0f, gameObject.transform.localPosition.z);
                    gameObject.transform.localScale = Vector3.one * 1.0f;
                    TargetPositionText = gameObject.GetComponent<TMP_Text>();
                    TargetPositionText.alpha = 1.0f;
                }

                var room = Helpers.GetPlainShipRoom(Target);
                TargetPositionText.gameObject.SetActive(true);
                if (room != null)
                {
                    sb.Clear();
                    sb.Append("<color=#8CFFFFFF>");
                    sb.Append(Target.name);
                    sb.Append('(');
                    sb.Append(FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId));
                    sb.Append(")</color>");
                    TargetPositionText.text = sb.ToString();
                }
                else
                    TargetPositionText.text = "";
            }
            else
                TargetPositionText?.text = "";

            // タイマーに時間をセット
            UpdateTimer = ArrowUpdateInterval;
        }
    }
}
