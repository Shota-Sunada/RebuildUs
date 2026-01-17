namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class TimeMaster : RoleBase<TimeMaster>
{
    public static Color RoleColor = new Color32(112, 142, 239, byte.MaxValue);
    private static CustomButton timeMasterShieldButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.timeMasterCooldown.GetFloat(); } }
    public static float rewindTime { get { return CustomOptionHolder.timeMasterRewindTime.GetFloat(); } }
    public static float shieldDuration { get { return CustomOptionHolder.timeMasterShieldDuration.GetFloat(); } }

    public static bool shieldActive = false;
    public static bool isRewinding = false;

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
        if (isRewinding)
        {
            if (GameHistory.LocalPlayerPositions.Count > 0)
            {
                // Set position
                var next = GameHistory.LocalPlayerPositions[0];
                if (next.Item2 == true)
                {
                    // Exit current vent if necessary
                    if (CachedPlayer.LocalPlayer.PlayerControl.inVent)
                    {
                        foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                        {
                            vent.CanUse(CachedPlayer.LocalPlayer.PlayerControl.Data, out bool canUse, out bool couldUse);
                            if (canUse)
                            {
                                CachedPlayer.LocalPlayer.PlayerControl.MyPhysics.RpcExitVent(vent.Id);
                                vent.SetButtons(false);
                            }
                        }
                    }
                    // Set position
                    CachedPlayer.LocalPlayer.PlayerControl.transform.position = next.Item1;
                }
                else if (GameHistory.LocalPlayerPositions.Any(x => x.Item2 == true))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.transform.position = next.Item1;
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
                isRewinding = false;
                CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
            }
        }
        else
        {
            while (GameHistory.LocalPlayerPositions.Count >= Mathf.Round(rewindTime / Time.fixedDeltaTime))
            {
                GameHistory.LocalPlayerPositions.RemoveAt(GameHistory.LocalPlayerPositions.Count - 1);
            }
            GameHistory.LocalPlayerPositions.Insert(0, new Tuple<Vector3, bool>(CachedPlayer.LocalPlayer.PlayerControl.transform.position, CachedPlayer.LocalPlayer.PlayerControl.CanMove)); // CanMove = CanMove
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        timeMasterShieldButton = new CustomButton
        (
            () =>
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TimeMasterShield, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.timeMasterShield();
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.TimeMaster) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                timeMasterShieldButton.IsEffectActive = false;
                timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            TimeMaster.getButtonSprite(),
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            shieldDuration,
            () => { timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer; }
        )
        {
            ButtonText = Tr.Get("TimeShieldText")
        };
    }
    public static void SetButtonCooldowns()
    {
        timeMasterShieldButton.MaxTimer = cooldown;
        timeMasterShieldButton.EffectDuration = shieldDuration;
    }

    public static void resetTimeMasterButton()
    {
        timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
        timeMasterShieldButton.IsEffectActive = false;
        timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}