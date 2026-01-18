namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Tracker : RoleBase<Tracker>
{
    public static Color NameColor = new Color32(100, 58, 220, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton TrackerTrackPlayerButton;
    private static CustomButton TrackerTrackCorpsesButton;
    public static List<Vector3> DeadBodyPositions = [];
    public List<Arrow> LocalArrows = [];
    public float CorpsesTrackingTimer = 0f;
    // write configs here
    public static float UpdateInterval { get { return CustomOptionHolder.TrackerUpdateInterval.GetFloat(); } }
    public static bool ResetTargetAfterMeeting { get { return CustomOptionHolder.TrackerResetTargetAfterMeeting.GetBool(); } }
    public static bool CanTrackCorpses { get { return CustomOptionHolder.TrackerCanTrackCorpses.GetBool(); } }
    public static float CorpsesTrackingCooldown { get { return CustomOptionHolder.TrackerCorpsesTrackingCooldown.GetFloat(); } }
    public static float CorpsesTrackingDuration { get { return CustomOptionHolder.TrackerCorpsesTrackingDuration.GetFloat(); } }

    public PlayerControl CurrentTarget;
    public PlayerControl Tracked;
    public bool UsedTracker = false;
    public float TimeUntilUpdate = 0f;
    public Arrow Arrow = new(Color.blue);

    public Tracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        DeadBodyPositions = [];
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker)) return;

        CurrentTarget = Helpers.SetTarget();
        if (!UsedTracker) Helpers.SetPlayerOutline(CurrentTarget, RoleColor);

        if (Arrow?.ArrowObject != null)
        {
            if (Tracked != null && !Player.Data.IsDead)
            {
                TimeUntilUpdate -= Time.fixedDeltaTime;

                if (TimeUntilUpdate <= 0f)
                {
                    bool trackedOnMap = !Tracked.Data.IsDead;
                    Vector3 position = Tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Tracked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }

                    Arrow.Update(position);
                    Arrow.ArrowObject.SetActive(trackedOnMap);
                    TimeUntilUpdate = UpdateInterval;
                }
                else
                {
                    Arrow.Update();
                }
            }
        }

        // Handle corpses tracking
        if (CorpsesTrackingTimer >= 0f && !Player.Data.IsDead)
        {
            bool arrowsCountChanged = LocalArrows.Count != DeadBodyPositions.Count();
            int index = 0;

            if (arrowsCountChanged)
            {
                foreach (var arrow in LocalArrows)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
                LocalArrows = [];
            }
            foreach (Vector3 position in DeadBodyPositions)
            {
                if (arrowsCountChanged)
                {
                    LocalArrows.Add(new Arrow(RoleColor));
                    LocalArrows[index].ArrowObject.SetActive(true);
                }
                LocalArrows[index]?.Update(position);
                index++;
            }
        }
        else if (LocalArrows.Count > 0)
        {
            foreach (var arrow in LocalArrows)
            {
                UnityEngine.Object.Destroy(arrow.ArrowObject);
            }
            LocalArrows = [];
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        TrackerTrackPlayerButton = new CustomButton(
               () =>
               {
                   using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.TrackerUsedTracker);
                   sender.Write(CurrentTarget.PlayerId);
                   RPCProcedure.TrackerUsedTracker(CurrentTarget.PlayerId, Player.PlayerId);
               },
               () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
               () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && CurrentTarget != null && !UsedTracker; },
               () => { if (ResetTargetAfterMeeting) ResetTracked(); },
               AssetLoader.TrackerButton,
               new Vector3(-1.8f, -0.06f, 0),
               hm,
               hm.UseButton,
               KeyCode.F
           )
        {
            ButtonText = Tr.Get("Hud.TrackerText")
        };

        TrackerTrackCorpsesButton = new CustomButton(
            () => { CorpsesTrackingTimer = CorpsesTrackingDuration; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && CanTrackCorpses; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                TrackerTrackCorpsesButton.Timer = TrackerTrackCorpsesButton.MaxTimer;
                TrackerTrackCorpsesButton.IsEffectActive = false;
                TrackerTrackCorpsesButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PathfindButton,
            new Vector3(-2.7f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.Q,
            true,
            CorpsesTrackingDuration,
            () =>
            {
                TrackerTrackCorpsesButton.Timer = TrackerTrackCorpsesButton.MaxTimer;
            }
        )
        {
            ButtonText = Tr.Get("Hud.PathfindText")
        };
    }
    public override void SetButtonCooldowns()
    {
        TrackerTrackPlayerButton.MaxTimer = 0f;
        TrackerTrackCorpsesButton.MaxTimer = CorpsesTrackingCooldown;
        TrackerTrackCorpsesButton.EffectDuration = CorpsesTrackingDuration;
    }

    // write functions here
    public void ResetTracked()
    {
        CurrentTarget = Tracked = null;
        UsedTracker = false;
        if (Arrow?.ArrowObject != null) UnityEngine.Object.Destroy(Arrow.ArrowObject);
        Arrow = new Arrow(Color.blue);
        Arrow.ArrowObject?.SetActive(false);
    }

    public static void Clear()
    {
        // reset configs here
        DeadBodyPositions = [];
        foreach (var p in Players)
        {
            p.ResetTracked();
            if (p.LocalArrows != null)
            {
                foreach (var arrow in p.LocalArrows)
                {
                    if (arrow?.ArrowObject != null)
                    {
                        UnityEngine.Object.Destroy(arrow.ArrowObject);
                    }
                }
            }
        }
        Players.Clear();
    }
}