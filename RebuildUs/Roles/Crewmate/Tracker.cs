namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Tracker : RoleBase<Tracker>
{
    public static Color RoleColor = new Color32(100, 58, 220, byte.MaxValue);
    private static CustomButton trackerTrackPlayerButton;
    private static CustomButton trackerTrackCorpsesButton;
    public static List<Vector3> deadBodyPositions = [];
    public List<Arrow> localArrows = [];
    public float corpsesTrackingTimer = 0f;
    // write configs here
    public static float updateInterval { get { return CustomOptionHolder.trackerUpdateInterval.GetFloat(); } }
    public static bool resetTargetAfterMeeting { get { return CustomOptionHolder.trackerResetTargetAfterMeeting.GetBool(); } }
    public static bool canTrackCorpses { get { return CustomOptionHolder.trackerCanTrackCorpses.GetBool(); } }
    public static float corpsesTrackingCooldown { get { return CustomOptionHolder.trackerCorpsesTrackingCooldown.GetFloat(); } }
    public static float corpsesTrackingDuration { get { return CustomOptionHolder.trackerCorpsesTrackingDuration.GetFloat(); } }

    public PlayerControl currentTarget;
    public PlayerControl tracked;
    public bool usedTracker = false;
    public float timeUntilUpdate = 0f;
    public Arrow arrow = new(Color.blue);

    public Tracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        deadBodyPositions = [];
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker)) return;

        currentTarget = Helpers.SetTarget();
        if (!usedTracker) Helpers.SetPlayerOutline(currentTarget, RoleColor);

        if (arrow?.ArrowObject != null)
        {
            if (tracked != null && !Player.Data.IsDead)
            {
                timeUntilUpdate -= Time.fixedDeltaTime;

                if (timeUntilUpdate <= 0f)
                {
                    bool trackedOnMap = !tracked.Data.IsDead;
                    Vector3 position = tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == tracked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }

                    arrow.Update(position);
                    arrow.ArrowObject.SetActive(trackedOnMap);
                    timeUntilUpdate = updateInterval;
                }
                else
                {
                    arrow.Update();
                }
            }
        }

        // Handle corpses tracking
        if (corpsesTrackingTimer >= 0f && !Player.Data.IsDead)
        {
            bool arrowsCountChanged = localArrows.Count != deadBodyPositions.Count();
            int index = 0;

            if (arrowsCountChanged)
            {
                foreach (var arrow in localArrows)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
                localArrows = [];
            }
            foreach (Vector3 position in deadBodyPositions)
            {
                if (arrowsCountChanged)
                {
                    localArrows.Add(new Arrow(RoleColor));
                    localArrows[index].ArrowObject.SetActive(true);
                }
                localArrows[index]?.Update(position);
                index++;
            }
        }
        else if (localArrows.Count > 0)
        {
            foreach (var arrow in localArrows)
            {
                UnityEngine.Object.Destroy(arrow.ArrowObject);
            }
            localArrows = [];
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        trackerTrackPlayerButton = new CustomButton(
               () =>
               {
                   MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrackerUsedTracker, SendOption.Reliable, -1);
                   writer.Write(currentTarget.PlayerId);
                   AmongUsClient.Instance.FinishRpcImmediately(writer);
                   RPCProcedure.trackerUsedTracker(currentTarget.PlayerId, Player.PlayerId);
               },
               () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
               () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && currentTarget != null && !usedTracker; },
               () => { if (resetTargetAfterMeeting) resetTracked(); },
               AssetLoader.TrackerButton,
               new Vector3(-1.8f, -0.06f, 0),
               hm,
               hm.UseButton,
               KeyCode.F
           )
        {
            ButtonText = Tr.Get("TrackerText")
        };

        trackerTrackCorpsesButton = new CustomButton(
            () => { corpsesTrackingTimer = corpsesTrackingDuration; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && canTrackCorpses; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                trackerTrackCorpsesButton.IsEffectActive = false;
                trackerTrackCorpsesButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PathfindButton,
            new Vector3(-2.7f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.Q,
            true,
            corpsesTrackingDuration,
            () =>
            {
                trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
            }
        )
        {
            ButtonText = Tr.Get("PathfindText")
        };
    }
    public override void SetButtonCooldowns()
    {
        trackerTrackPlayerButton.MaxTimer = 0f;
        trackerTrackCorpsesButton.MaxTimer = corpsesTrackingCooldown;
        trackerTrackCorpsesButton.EffectDuration = corpsesTrackingDuration;
    }

    // write functions here
    public void resetTracked()
    {
        currentTarget = tracked = null;
        usedTracker = false;
        if (arrow?.ArrowObject != null) UnityEngine.Object.Destroy(arrow.ArrowObject);
        arrow = new Arrow(Color.blue);
        arrow.ArrowObject?.SetActive(false);
    }

    public override void Clear()
    {
        // reset configs here
        deadBodyPositions = [];
        resetTracked();
        if (localArrows != null)
        {
            foreach (var arrow in localArrows)
            {
                if (arrow?.ArrowObject != null)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
            }
        }
        Players.Clear();
    }
}