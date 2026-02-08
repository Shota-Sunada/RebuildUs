namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class TimeMaster : RoleBase<TimeMaster>
{
    public static Color NameColor = new Color32(112, 142, 239, byte.MaxValue);

    private static CustomButton _timeMasterShieldButton;

    public static bool ShieldActive = false;
    public static bool IsRewinding;

    public TimeMaster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.TimeMaster;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.TimeMasterCooldown.GetFloat();
    }

    public static float RewindTime
    {
        get => CustomOptionHolder.TimeMasterRewindTime.GetFloat();
    }

    public static float ShieldDuration
    {
        get => CustomOptionHolder.TimeMasterShieldDuration.GetFloat();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        if (IsRewinding)
        {
            if (GameHistory.LOCAL_PLAYER_POSITIONS.Count > 0)
            {
                // Set position
                var next = GameHistory.LOCAL_PLAYER_POSITIONS[0];
                if (next.Item2)
                {
                    // Exit current vent if necessary
                    if (PlayerControl.LocalPlayer.inVent)
                    {
                        var vents = MapUtilities.CachedShipStatus.AllVents;
                        for (var i = 0; i < vents.Length; i++)
                        {
                            var vent = vents[i];
                            vent.CanUse(PlayerControl.LocalPlayer?.Data, out var canUse, out var couldUse);
                            if (canUse)
                            {
                                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(vent.Id);
                                vent.SetButtons(false);
                            }
                        }
                    }

                    // Set position
                    PlayerControl.LocalPlayer.transform.position = next.Item1;
                }
                else if (GameHistory.LOCAL_PLAYER_POSITIONS.Any(x => x.Item2)) PlayerControl.LocalPlayer.transform.position = next.Item1;

                if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);

                GameHistory.LOCAL_PLAYER_POSITIONS.RemoveAt(0);

                if (GameHistory.LOCAL_PLAYER_POSITIONS.Count > 1)
                {
                    // Skip every second position to rewind twice as fast, but never skip the last position
                    GameHistory.LOCAL_PLAYER_POSITIONS.RemoveAt(0);
                }
            }
            else
            {
                IsRewinding = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
        else
        {
            while (GameHistory.LOCAL_PLAYER_POSITIONS.Count >= Mathf.Round(RewindTime / Time.fixedDeltaTime)) GameHistory.LOCAL_PLAYER_POSITIONS.RemoveAt(GameHistory.LOCAL_PLAYER_POSITIONS.Count - 1);
            GameHistory.LOCAL_PLAYER_POSITIONS.Insert(0, new(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
        }
    }

    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        _timeMasterShieldButton = new(() =>
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.TimeMasterShield);
            RPCProcedure.TimeMasterShield();
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer;
            _timeMasterShieldButton.IsEffectActive = false;
            _timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.TimeShieldButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, ShieldDuration, () => { _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer; }, false, Tr.Get(TrKey.TimeShieldText));
    }

    public static void SetButtonCooldowns()
    {
        _timeMasterShieldButton.MaxTimer = Cooldown;
        _timeMasterShieldButton.EffectDuration = ShieldDuration;
    }

    public static void ResetTimeMasterButton()
    {
        _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer;
        _timeMasterShieldButton.IsEffectActive = false;
        _timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
