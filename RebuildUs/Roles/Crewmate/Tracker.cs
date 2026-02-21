using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Tracker : RoleBase<Tracker>
{
    internal static Color NameColor = new Color32(100, 58, 220, byte.MaxValue);

    private static CustomButton _trackerTrackPlayerButton;
    private static CustomButton _trackerTrackCorpsesButton;
    internal static List<Vector3> DeadBodyPositions = [];

    internal static float CorpsesTrackingTimer;
    internal Arrow Arrow;

    internal PlayerControl CurrentTarget;
    internal List<Arrow> LocalArrows = [];
    internal float TimeUntilUpdate;
    internal PlayerControl Tracked;
    internal bool UsedTracker;

    public Tracker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Tracker;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float UpdateInterval { get => CustomOptionHolder.TrackerUpdateInterval.GetFloat(); }
    internal static bool ResetTargetAfterMeeting { get => CustomOptionHolder.TrackerResetTargetAfterMeeting.GetBool(); }
    internal static bool CanTrackCorpses { get => CustomOptionHolder.TrackerCanTrackCorpses.GetBool(); }
    internal static float CorpsesTrackingCooldown { get => CustomOptionHolder.TrackerCorpsesTrackingCooldown.GetFloat(); }
    internal static float CorpsesTrackingDuration { get => CustomOptionHolder.TrackerCorpsesTrackingDuration.GetFloat(); }

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        DeadBodyPositions = [];
    }

    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
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
                    bool trackedOnMap = !Tracked.Data.IsDead;
                    Vector3 position = Tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        Il2CppArrayBase<DeadBody> bodies = Object.FindObjectsOfType<DeadBody>();
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
            bool arrowsCountChanged = LocalArrows.Count != DeadBodyPositions.Count;
            int index = 0;

            if (arrowsCountChanged)
            {
                for (int i = 0; i < LocalArrows.Count; i++)
                {
                    if (LocalArrows[i]?.ArrowObject != null)
                        Object.Destroy(LocalArrows[i].ArrowObject);
                }

                LocalArrows.Clear();
            }

            for (int i = 0; i < DeadBodyPositions.Count; i++)
            {
                Vector3 position = DeadBodyPositions[i];
                if (arrowsCountChanged)
                {
                    Arrow a = new(RoleColor);
                    a.ArrowObject.SetActive(true);
                    LocalArrows.Add(a);
                }

                if (index < LocalArrows.Count) LocalArrows[index]?.Update(position);

                index++;
            }
        }
        else if (LocalArrows.Count > 0)
        {
            for (int i = 0; i < LocalArrows.Count; i++)
            {
                if (LocalArrows[i]?.ArrowObject != null)
                    Object.Destroy(LocalArrows[i].ArrowObject);
            }

            LocalArrows.Clear();
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _trackerTrackPlayerButton = new(() =>
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.TrackerUsedTracker);
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
        }, AssetLoader.PathfindButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilitySecondary, true, CorpsesTrackingDuration, () =>
        {
            _trackerTrackCorpsesButton.Timer = _trackerTrackCorpsesButton.MaxTimer;
        }, false, Tr.Get(TrKey.PathfindText));
    }

    internal static void SetButtonCooldowns()
    {
        _trackerTrackPlayerButton.MaxTimer = 0f;
        _trackerTrackCorpsesButton.MaxTimer = CorpsesTrackingCooldown;
        _trackerTrackCorpsesButton.EffectDuration = CorpsesTrackingDuration;
    }

    // write functions here
    internal void ResetTracked()
    {
        CurrentTarget = Tracked = null;
        UsedTracker = false;
        if (Arrow?.ArrowObject != null) Object.Destroy(Arrow.ArrowObject);
        Arrow = null;
    }

    internal static void Clear()
    {
        // reset configs here
        DeadBodyPositions = [];
        foreach (Tracker p in Players)
        {
            p.ResetTracked();
            if (p.LocalArrows != null)
            {
                foreach (Arrow arrow in p.LocalArrows)
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