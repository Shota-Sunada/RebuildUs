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
    public static float CorpsesTrackingTimer = 0f;
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
    public Arrow Arrow;

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
        if (Player != PlayerControl.LocalPlayer) return;

        CurrentTarget = Helpers.SetTarget();
        if (!UsedTracker) Helpers.SetPlayerOutline(CurrentTarget, RoleColor);

        if (UsedTracker && Tracked != null && !Player.Data.IsDead)
        {
            Arrow ??= new Arrow(RoleColor);
            if (Arrow.ArrowObject != null)
            {
                TimeUntilUpdate -= Time.fixedDeltaTime;

                if (TimeUntilUpdate <= 0f)
                {
                    bool trackedOnMap = !Tracked.Data.IsDead;
                    Vector3 position = Tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        var bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                        for (int i = 0; i < bodies.Length; i++)
                        {
                            if (bodies[i].ParentId == Tracked.PlayerId)
                            {
                                trackedOnMap = true;
                                position = bodies[i].transform.position;
                                break;
                            }
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
        else
        {
            if (Arrow?.ArrowObject != null)
            {
                UnityEngine.Object.Destroy(Arrow.ArrowObject);
                Arrow = null;
            }
        }

        // Handle corpses tracking
        if (CorpsesTrackingTimer >= 0f && !Player.Data.IsDead)
        {
            bool arrowsCountChanged = LocalArrows.Count != DeadBodyPositions.Count;
            int index = 0;

            if (arrowsCountChanged)
            {
                for (int i = 0; i < LocalArrows.Count; i++)
                {
                    if (LocalArrows[i]?.ArrowObject != null)
                    {
                        UnityEngine.Object.Destroy(LocalArrows[i].ArrowObject);
                    }
                }
                LocalArrows.Clear();
            }
            for (int i = 0; i < DeadBodyPositions.Count; i++)
            {
                Vector3 position = DeadBodyPositions[i];
                if (arrowsCountChanged)
                {
                    var a = new Arrow(RoleColor);
                    a.ArrowObject.SetActive(true);
                    LocalArrows.Add(a);
                }
                if (index < LocalArrows.Count)
                {
                    LocalArrows[index]?.Update(position);
                }
                index++;
            }
        }
        else if (LocalArrows.Count > 0)
        {
            for (int i = 0; i < LocalArrows.Count; i++)
            {
                if (LocalArrows[i]?.ArrowObject != null)
                {
                    UnityEngine.Object.Destroy(LocalArrows[i].ArrowObject);
                }
            }
            LocalArrows.Clear();
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        TrackerTrackPlayerButton = new CustomButton(
            () =>
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.TrackerUsedTracker);
                sender.Write(Local.CurrentTarget.PlayerId);
                RPCProcedure.TrackerUsedTracker(Local.CurrentTarget.PlayerId, Local.Player.PlayerId);
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Tracker) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null && !Local.UsedTracker; },
            () => { if (ResetTargetAfterMeeting) Local.ResetTracked(); },
            AssetLoader.TrackerButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.TrackerText)
        );

        TrackerTrackCorpsesButton = new CustomButton(
            () => { CorpsesTrackingTimer = CorpsesTrackingDuration; },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Tracker) && PlayerControl.LocalPlayer.IsAlive() && CanTrackCorpses; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                TrackerTrackCorpsesButton.Timer = TrackerTrackCorpsesButton.MaxTimer;
                TrackerTrackCorpsesButton.IsEffectActive = false;
                TrackerTrackCorpsesButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PathfindButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            true,
            CorpsesTrackingDuration,
            () =>
            {
                TrackerTrackCorpsesButton.Timer = TrackerTrackCorpsesButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.PathfindText)
        );
    }
    public static void SetButtonCooldowns()
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
        Arrow = null;
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
        CorpsesTrackingTimer = 0f;
        Players.Clear();
    }
}