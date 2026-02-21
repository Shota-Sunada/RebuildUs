namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class TimeMaster : RoleBase<TimeMaster>
{
    internal static Color NameColor = new Color32(112, 142, 239, byte.MaxValue);

    private static CustomButton _timeMasterShieldButton;

    internal static bool ShieldActive = false;
    internal static bool IsRewinding;

    public TimeMaster()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.TimeMaster;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.TimeMasterCooldown.GetFloat(); }
    internal static float RewindTime { get => CustomOptionHolder.TimeMasterRewindTime.GetFloat(); }
    internal static float ShieldDuration { get => CustomOptionHolder.TimeMasterShieldDuration.GetFloat(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (IsRewinding)
        {
            if (GameHistory.LocalPlayerPositions.Count > 0)
            {
                // Set position
                Tuple<Vector3, bool> next = GameHistory.LocalPlayerPositions[0];
                if (next.Item2)
                {
                    // Exit current vent if necessary
                    if (PlayerControl.LocalPlayer.inVent)
                    {
                        Il2CppReferenceArray<Vent> vents = MapUtilities.CachedShipStatus.AllVents;
                        for (int i = 0; i < vents.Length; i++)
                        {
                            Vent vent = vents[i];
                            vent.CanUse(PlayerControl.LocalPlayer?.Data, out bool canUse, out bool couldUse);
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
                else if (GameHistory.LocalPlayerPositions.Any(x => x.Item2)) PlayerControl.LocalPlayer.transform.position = next.Item1;

                if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);

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
            while (GameHistory.LocalPlayerPositions.Count >= Mathf.Round(RewindTime / Time.fixedDeltaTime)) GameHistory.LocalPlayerPositions.RemoveAt(GameHistory.LocalPlayerPositions.Count - 1);

            GameHistory.LocalPlayerPositions.Insert(0, new(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _timeMasterShieldButton = new(() =>
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.TimeMasterShield);
            RPCProcedure.TimeMasterShield();
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.TimeMaster) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer;
            _timeMasterShieldButton.IsEffectActive = false;
            _timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.TimeShieldButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, ShieldDuration, () => { _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer; }, false, Tr.Get(TrKey.TimeShieldText));
    }

    internal static void SetButtonCooldowns()
    {
        _timeMasterShieldButton.MaxTimer = Cooldown;
        _timeMasterShieldButton.EffectDuration = ShieldDuration;
    }

    internal static void ResetTimeMasterButton()
    {
        _timeMasterShieldButton.Timer = _timeMasterShieldButton.MaxTimer;
        _timeMasterShieldButton.IsEffectActive = false;
        _timeMasterShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}