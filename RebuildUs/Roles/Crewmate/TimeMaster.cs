namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class TimeMaster : RoleBase<TimeMaster>
{
    public static Color NameColor = new Color32(112, 142, 239, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton TimeMasterShieldButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.TimeMasterCooldown.GetFloat(); } }
    public static float RewindTime { get { return CustomOptionHolder.TimeMasterRewindTime.GetFloat(); } }
    public static float ShieldDuration { get { return CustomOptionHolder.TimeMasterShieldDuration.GetFloat(); } }

    public static bool ShieldActive = false;
    public static bool IsRewinding = false;

    public TimeMaster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.TimeMaster;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (IsRewinding)
        {
            if (GameHistory.LocalPlayerPositions.Count > 0)
            {
                // Set position
                var next = GameHistory.LocalPlayerPositions[0];
                if (next.Item2 == true)
                {
                    // Exit current vent if necessary
                    if (PlayerControl.LocalPlayer.inVent)
                    {
                        foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                        {
                            vent.CanUse(PlayerControl.LocalPlayer.Data, out bool canUse, out bool couldUse);
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
                else if (GameHistory.LocalPlayerPositions.Any(x => x.Item2 == true))
                {
                    PlayerControl.LocalPlayer.transform.position = next.Item1;
                }
                if (SubmergedCompatibility.IsSubmerged)
                {
                    SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);
                }

                GameHistory.LocalPlayerPositions.RemoveAt(0);

                if (GameHistory.LocalPlayerPositions.Count > 1)
                {
                    // Skip every second position to rewind twice as fast, but never skip the last position
                    GameHistory.LocalPlayerPositions.RemoveAt(0);
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
            while (GameHistory.LocalPlayerPositions.Count >= Mathf.Round(RewindTime / Time.fixedDeltaTime))
            {
                GameHistory.LocalPlayerPositions.RemoveAt(GameHistory.LocalPlayerPositions.Count - 1);
            }
            GameHistory.LocalPlayerPositions.Insert(0, new Tuple<Vector3, bool>(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        TimeMasterShieldButton = new CustomButton
        (
            () =>
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.TimeMasterShield);
                RPCProcedure.TimeMasterShield();
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                TimeMasterShieldButton.Timer = TimeMasterShieldButton.MaxTimer;
                TimeMasterShieldButton.IsEffectActive = false;
                TimeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.TimeShieldButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            ShieldDuration,
            () => { TimeMasterShieldButton.Timer = TimeMasterShieldButton.MaxTimer; }
        )
        {
            ButtonText = Tr.Get("Hud.TimeShieldText")
        };
    }
    public override void SetButtonCooldowns()
    {
        TimeMasterShieldButton.MaxTimer = Cooldown;
        TimeMasterShieldButton.EffectDuration = ShieldDuration;
    }

    public static void ResetTimeMasterButton()
    {
        TimeMasterShieldButton.Timer = TimeMasterShieldButton.MaxTimer;
        TimeMasterShieldButton.IsEffectActive = false;
        TimeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}