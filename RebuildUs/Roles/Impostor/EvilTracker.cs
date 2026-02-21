using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class EvilTracker : RoleBase<EvilTracker>
{
    internal static Color NameColor = Palette.ImpostorRed;
    internal static PlayerControl Target;
    internal static PlayerControl CurrentTarget;
    internal static CustomButton TrackerButton;
    internal static Dictionary<string, TMP_Text> ImpostorPositionText;
    internal static TMP_Text TargetPositionText;

    internal static List<Arrow> Arrows = [];
    internal static float UpdateTimer;
    internal static float ArrowUpdateInterval = 0.5f;

    public EvilTracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.EvilTrackerCooldown.GetFloat(); }
    internal static bool ResetTargetAfterMeeting { get => CustomOptionHolder.EvilTrackerResetTargetAfterMeeting.GetBool(); }
    internal static bool CanSeeDeathFlash { get => CustomOptionHolder.EvilTrackerCanSeeDeathFlash.GetBool(); }
    internal static bool CanSeeTargetTask { get => CustomOptionHolder.EvilTrackerCanSeeTargetTask.GetBool(); }
    internal static bool CanSeeTargetPosition { get => CustomOptionHolder.EvilTrackerCanSeeTargetPosition.GetBool(); }
    internal static bool CanSetTargetOnMeeting { get => CustomOptionHolder.EvilTrackerCanSetTargetOnMeeting.GetBool(); }

    internal override void OnMeetingStart()
    {
        if (ResetTargetAfterMeeting) Target = null;
    }

    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        EvilTracker local = Local;
        if (local != null)
        {
            ArrowUpdate();

            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        TrackerButton = new(() =>
        {
            Target = CurrentTarget;
        }, () => { return Target == null && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return CurrentTarget != null && Target == null && PlayerControl.LocalPlayer.CanMove; }, () => { TrackerButton.Timer = TrackerButton.MaxTimer; }, AssetLoader.TrackerButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.TrackerText));
    }

    internal static void SetButtonCooldowns()
    {
        TrackerButton.MaxTimer = Cooldown;
    }

    // write functions here

    internal static void Clear()
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
            for (int i = 0; i < Arrows.Count; i++)
            {
                Arrow arrow = Arrows[i];
                if (arrow != null && arrow.ArrowObject != null)
                {
                    arrow.ArrowObject.SetActive(false);
                    Object.Destroy(arrow.ArrowObject);
                }
            }

            // Arrows一覧
            Arrows.Clear();

            // インポスターの位置を示すArrowsを描画
            int count = 0;
            StringBuilder sb = new();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data.IsDead)
                {
                    if (p.IsTeamImpostor() && ImpostorPositionText.TryGetValue(p.name, out TMP_Text txt)) txt.text = "";

                    continue;
                }

                if (p.IsTeamImpostor() && p.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
                    Arrow arrow = new(Palette.ImpostorRed);
                    arrow.ArrowObject.SetActive(true);
                    arrow.Update(p.transform.position);
                    Arrows.Add(arrow);
                    count += 1;
                    if (!ImpostorPositionText.TryGetValue(p.name, out TMP_Text positionText))
                    {
                        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = Object.Instantiate(roomTracker.gameObject);
                        Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new(0, -2.0f + (0.25f * count), gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        positionText = gameObject.GetComponent<TMP_Text>();
                        positionText.alpha = 1.0f;
                        ImpostorPositionText.Add(p.name, positionText);
                    }

                    PlainShipRoom room = Helpers.GetPlainShipRoom(p);
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
                Arrow arrow = new(Palette.CrewmateBlue);
                arrow.ArrowObject.SetActive(true);
                arrow.Update(Target.transform.position);
                Arrows.Add(arrow);
                if (TargetPositionText == null)
                {
                    RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                    if (roomTracker == null) return;
                    GameObject gameObject = Object.Instantiate(roomTracker.gameObject);
                    Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.transform.localPosition = new(0, -2.0f, gameObject.transform.localPosition.z);
                    gameObject.transform.localScale = Vector3.one * 1.0f;
                    TargetPositionText = gameObject.GetComponent<TMP_Text>();
                    TargetPositionText.alpha = 1.0f;
                }

                PlainShipRoom room = Helpers.GetPlainShipRoom(Target);
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