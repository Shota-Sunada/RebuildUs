using RebuildUs.Roles.Modifier;

namespace RebuildUs.Roles.Neutral;

public static class Guesser
{
    public static bool onlyAvailableRoles { get { return CustomOptionHolder.guesserOnlyAvailableRoles.GetBool(); } }
    public static bool hasMultipleShotsPerMeeting { get { return CustomOptionHolder.guesserHasMultipleShotsPerMeeting.GetBool(); } }
    public static bool showInfoInGhostChat { get { return CustomOptionHolder.guesserShowInfoInGhostChat.GetBool(); } }
    public static bool killsThroughShield { get { return CustomOptionHolder.guesserKillsThroughShield.GetBool(); } }
    public static bool evilCanKillSpy { get { return CustomOptionHolder.guesserEvilCanKillSpy.GetBool(); } }

    public static int remainingShotsNiceGuesser = 0;
    public static int remainingShotsEvilGuesser = 0;

    public static void ClearAndReload()
    {
        remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.GetFloat());
        remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.GetFloat());
    }

    public static bool IsGuesser(byte playerId)
    {
        if (!EvilGuesser.Exists && !NiceGuesser.Exists) return false;

        var player = Helpers.PlayerById(playerId);
        return player.IsRole(RoleType.EvilGuesser) || player.IsRole(RoleType.NiceGuesser);
    }

    public static int remainingShots(PlayerControl player, bool shoot = false)
    {
        int remainingShots = 0;
        if (player.IsRole(RoleType.NiceGuesser))
        {
            remainingShots = remainingShotsNiceGuesser;
            if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
        }
        else if (player.IsRole(RoleType.EvilGuesser))
        {
            remainingShots = remainingShotsEvilGuesser;
            if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.canGuess())
            {
                remainingShots += LastImpostor.remainingShots;
            }
            if (shoot)
            {
                // ラストインポスターの弾数を優先的に消費させる
                if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.canGuess())
                {
                    LastImpostor.remainingShots = Mathf.Max(0, LastImpostor.remainingShots - 1);
                }
                else
                {
                    remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
                }
            }
        }
        else if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.canGuess())
        {
            remainingShots = LastImpostor.remainingShots;
            if (shoot) LastImpostor.remainingShots = Mathf.Max(0, LastImpostor.remainingShots - 1);
        }

        return remainingShots;
    }

    [HarmonyPatch]
    public class NiceGuesser : RoleBase<NiceGuesser>
    {
        public static Color RoleColor = new Color32(255, 255, 0, byte.MaxValue);

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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

        // write functions here

        public override void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }

    [HarmonyPatch]
    public class EvilGuesser : RoleBase<EvilGuesser>
    {
        public static Color RoleColor = Palette.ImpostorRed;

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
        public override void MakeButtons(HudManager hm) { }
        public override void SetButtonCooldowns() { }

        // write functions here

        public override void Clear()
        {
            // reset configs here
            Players.Clear();
        }
    }
}