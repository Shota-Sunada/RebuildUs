namespace RebuildUs.Roles.Neutral;

public static class Guesser
{
    public static bool OnlyAvailableRoles { get { return CustomOptionHolder.GuesserOnlyAvailableRoles.GetBool(); } }
    public static bool HasMultipleShotsPerMeeting { get { return CustomOptionHolder.GuesserHasMultipleShotsPerMeeting.GetBool(); } }
    public static bool ShowInfoInGhostChat { get { return CustomOptionHolder.GuesserShowInfoInGhostChat.GetBool(); } }
    public static bool KillsThroughShield { get { return CustomOptionHolder.GuesserKillsThroughShield.GetBool(); } }
    public static bool EvilCanKillSpy { get { return CustomOptionHolder.GuesserEvilCanKillSpy.GetBool(); } }

    public static int RemainingShotsNiceGuesser = 0;
    public static int RemainingShotsEvilGuesser = 0;

    public static void ClearAndReload()
    {
        RemainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        RemainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        NiceGuesser.Clear();
        EvilGuesser.Clear();
    }

    public static bool IsGuesser(byte playerId)
    {
        if (!EvilGuesser.Exists && !NiceGuesser.Exists) return false;

        var player = Helpers.PlayerById(playerId);
        return player.IsRole(RoleType.EvilGuesser) || player.IsRole(RoleType.NiceGuesser);
    }

    public static int RemainingShots(PlayerControl player, bool shoot = false)
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
            if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
            {
                remainingShots += LastImpostor.RemainingShots;
            }
            if (shoot)
            {
                // ラストインポスターの弾数を優先的に消費させる
                if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
                {
                    LastImpostor.RemainingShots = Mathf.Max(0, LastImpostor.RemainingShots - 1);
                }
                else
                {
                    RemainingShotsEvilGuesser = Mathf.Max(0, RemainingShotsEvilGuesser - 1);
                }
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
    public class NiceGuesser : RoleBase<NiceGuesser>
    {
        public static Color NameColor = new Color32(255, 255, 0, byte.MaxValue);
        public override Color RoleColor => NameColor;

        // write configs here

        public NiceGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.NiceGuesser;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void OnIntroEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        public override void OnDeath(PlayerControl killer = null) { }
        public override void OnFinishShipStatusBegin() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class EvilGuesser : RoleBase<EvilGuesser>
    {
        public static Color NameColor = Palette.ImpostorRed;
        public override Color RoleColor => NameColor;

        // write configs here

        public EvilGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.EvilGuesser;
        }

        public override void OnMeetingStart() { }
        public override void OnMeetingEnd() { }
        public override void OnIntroEnd() { }
        public override void FixedUpdate() { }
        public override void OnKill(PlayerControl target) { }
        public override void OnDeath(PlayerControl killer = null) { }
        public override void OnFinishShipStatusBegin() { }
        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

        // write functions here

        public static void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}