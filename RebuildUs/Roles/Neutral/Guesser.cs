namespace RebuildUs.Roles.Neutral;

internal static class Guesser
{
    internal static int RemainingShotsNiceGuesser;
    internal static int RemainingShotsEvilGuesser;
    internal static bool OnlyAvailableRoles { get => CustomOptionHolder.GuesserOnlyAvailableRoles.GetBool(); }
    internal static bool HasMultipleShotsPerMeeting { get => CustomOptionHolder.GuesserHasMultipleShotsPerMeeting.GetBool(); }
    internal static bool ShowInfoInGhostChat { get => CustomOptionHolder.GuesserShowInfoInGhostChat.GetBool(); }
    internal static bool KillsThroughShield { get => CustomOptionHolder.GuesserKillsThroughShield.GetBool(); }
    internal static bool EvilCanKillSpy { get => CustomOptionHolder.GuesserEvilCanKillSpy.GetBool(); }

    internal static void ClearAndReload()
    {
        RemainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        RemainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        NiceGuesser.Clear();
        EvilGuesser.Clear();
    }

    internal static bool IsGuesser(byte playerId)
    {
        if (!EvilGuesser.Exists && !NiceGuesser.Exists) return false;

        PlayerControl player = Helpers.PlayerById(playerId);
        return player.IsRole(RoleType.EvilGuesser) || player.IsRole(RoleType.NiceGuesser);
    }

    internal static int RemainingShots(PlayerControl player, bool shoot = false)
    {
        int remainingShots = 0;
        if (player.IsRole(RoleType.NiceGuesser))
        {
            remainingShots = RemainingShotsNiceGuesser;
            if (shoot) RemainingShotsNiceGuesser = Mathf.Max(0, RemainingShotsNiceGuesser - 1);
        }
        else if (player.IsRole(RoleType.EvilGuesser))
        {
            remainingShots = RemainingShotsEvilGuesser;
            if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess()) remainingShots += LastImpostor.RemainingShots;

            if (shoot)
            {
                // ラストインポスターの弾数を優先的に消費させる
                if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
                    LastImpostor.RemainingShots = Mathf.Max(0, LastImpostor.RemainingShots - 1);
                else
                    RemainingShotsEvilGuesser = Mathf.Max(0, RemainingShotsEvilGuesser - 1);
            }
        }
        else if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
        {
            remainingShots = LastImpostor.RemainingShots;
            if (shoot) LastImpostor.RemainingShots = Mathf.Max(0, LastImpostor.RemainingShots - 1);
        }

        return remainingShots;
    }

    [HarmonyPatch]
    internal class NiceGuesser : RoleBase<NiceGuesser>
    {
        internal static Color NameColor = new Color32(255, 255, 0, byte.MaxValue);

        // write configs here

        public NiceGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.NiceGuesser;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        internal override void OnMeetingStart() { }
        internal override void OnMeetingEnd() { }
        internal override void OnIntroEnd() { }
        internal override void FixedUpdate() { }
        internal override void OnKill(PlayerControl target) { }
        internal override void OnDeath(PlayerControl killer = null) { }
        internal override void OnFinishShipStatusBegin() { }
        internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        internal static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    internal class EvilGuesser : RoleBase<EvilGuesser>
    {
        internal static Color NameColor = Palette.ImpostorRed;

        // write configs here

        public EvilGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.EvilGuesser;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        internal override void OnMeetingStart() { }
        internal override void OnMeetingEnd() { }
        internal override void OnIntroEnd() { }
        internal override void FixedUpdate() { }
        internal override void OnKill(PlayerControl target) { }
        internal override void OnDeath(PlayerControl killer = null) { }
        internal override void OnFinishShipStatusBegin() { }
        internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        internal static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}