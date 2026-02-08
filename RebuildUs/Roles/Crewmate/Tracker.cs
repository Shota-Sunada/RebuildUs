using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Tracker : RoleBase<Tracker>
{
    public static Color NameColor = new Color32(100, 58, 220, byte.MaxValue);

    private static CustomButton _trackerTrackPlayerButton;
    private static CustomButton _trackerTrackCorpsesButton;
    public static List<Vector3> DeadBodyPositions = [];
    public static float CorpsesTrackingTimer;
    public Arrow Arrow;

    public PlayerControl CurrentTarget;
    public List<Arrow> LocalArrows = [];
    public float TimeUntilUpdate;
    public PlayerControl Tracked;
    public bool UsedTracker;

    public Tracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float UpdateInterval
    {
        get => CustomOptionHolder.TrackerUpdateInterval.GetFloat();
    }

    public static bool ResetTargetAfterMeeting
    {
        get => CustomOptionHolder.TrackerResetTargetAfterMeeting.GetBool();
    }

    public static bool CanTrackCorpses
    {
        get => CustomOptionHolder.TrackerCanTrackCorpses.GetBool();
    }

    public static float CorpsesTrackingCooldown
    {
        get => CustomOptionHolder.TrackerCorpsesTrackingCooldown.GetFloat();
    }

    public static float CorpsesTrackingDuration
    {
        get => CustomOptionHolder.TrackerCorpsesTrackingDuration.GetFloat();
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
            Arrow ??= new(RoleColor);
            if (Arrow.ArrowObject != null)
            {
                TimeUntilUpdate -= Time.fixedDeltaTime;

                if (TimeUntilUpdate <= 0f)
                {
                    var trackedOnMap = !Tracked.Data.IsDead;
                    var position = Tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        var bodies = Object.FindObjectsOfType<DeadBody>();
                        for (var i = 0; i < bodies.Length; i++)
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
                    Arrow.Update();
            }
        }
        else
        {
            if (Arrow?.ArrowObject != null)
            {
                Object.Destroy(Arrow.ArrowObject);
                Arrow = null;
            }
        }

        // Handle corpses tracking
        if (CorpsesTrackingTimer >= 0f && !Player.Data.IsDead)
        {
            var arrowsCountChanged = LocalArrows.Count != DeadBodyPositions.Count;
            var index = 0;

            if (arrowsCountChanged)
            {
                for (var i = 0; i < LocalArrows.Count; i++)
                {
                    if (LocalArrows[i]?.ArrowObject != null)
                        Object.Destroy(LocalArrows[i].ArrowObject);
                }

                LocalArrows.Clear();
            }

            for (var i = 0; i < DeadBodyPositions.Count; i++)
            {
                var position = DeadBodyPositions[i];
                if (arrowsCountChanged)
                {
                    var a = new Arrow(RoleColor);
                    a.ArrowObject.SetActive(true);
                    LocalArrows.Add(a);
                }

                if (index < LocalArrows.Count) LocalArrows[index]?.Update(position);
                index++;
            }
        }
        else if (LocalArrows.Count > 0)
        {
            for (var i = 0; i < LocalArrows.Count; i++)
            {
                if (LocalArrows[i]?.ArrowObject != null)
                    Object.Destroy(LocalArrows[i].ArrowObject);
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
        _trackerTrackPlayerButton = new(() =>
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.TrackerUsedTracker);
            sender.Write(Local.CurrentTarget.PlayerId);
            RPCProcedure.TrackerUsedTracker(Local.CurrentTarget.PlayerId, Local.Player.PlayerId);
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Tracker) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null && !Local.UsedTracker; }, () =>
        {
            if (ResetTargetAfterMeeting) Local.ResetTracked();
        }, AssetLoader.TrackerButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.TrackerText));

        _trackerTrackCorpsesButton = new(() => { CorpsesTrackingTimer = CorpsesTrackingDuration; }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Tracker) && PlayerControl.LocalPlayer.IsAlive() && CanTrackCorpses; }, () => { return PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            _trackerTrackCorpsesButton.Timer = _trackerTrackCorpsesButton.MaxTimer;
            _trackerTrackCorpsesButton.IsEffectActive = false;
            _trackerTrackCorpsesButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.PathfindButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilitySecondary, true, CorpsesTrackingDuration, () => { _trackerTrackCorpsesButton.Timer = _trackerTrackCorpsesButton.MaxTimer; }, false, Tr.Get(TrKey.PathfindText));
    }

    public static void SetButtonCooldowns()
    {
        _trackerTrackPlayerButton.MaxTimer = 0f;
        _trackerTrackCorpsesButton.MaxTimer = CorpsesTrackingCooldown;
        _trackerTrackCorpsesButton.EffectDuration = CorpsesTrackingDuration;
    }

    // write functions here
    public void ResetTracked()
    {
        CurrentTarget = Tracked = null;
        UsedTracker = false;
        if (Arrow?.ArrowObject != null) Object.Destroy(Arrow.ArrowObject);
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
                        Object.Destroy(arrow.ArrowObject);
                }
            }
        }

        CorpsesTrackingTimer = 0f;
        Players.Clear();
    }
}
