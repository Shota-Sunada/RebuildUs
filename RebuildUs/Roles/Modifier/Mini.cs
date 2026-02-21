namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
internal class Mini : ModifierBase<Mini>
{
    internal const float DEFAULT_COLLIDER_RADIUS = 0.2233912f;
    internal const float DEFAULT_COLLIDER_OFFSET = 0.3636057f;
    internal static Color NameColor = Color.yellow;

    internal static float GrowingUpDuration = 400f;
    internal static bool TriggerMiniLose;
    internal DateTime TimeOfGrowthStart = DateTime.UtcNow;

    public Mini()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.Mini;
    }

    internal override Color ModifierColor
    {
        get => NameColor;
    }

    internal static List<PlayerControl> Candidates
    {
        get
        {
            List<PlayerControl> validPlayers = [];
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (!player.HasModifier(ModifierType.Mini))
                    validPlayers.Add(player);

            return validPlayers;
        }
    }

    internal static string Postfix
    {
        get => Tr.Get(TrKey.MiniPostfix);
    }

    internal static string FullName
    {
        get => Tr.Get(TrKey.Mini);
    }

    internal float GrowingProgress()
    {
        float timeSinceStart = (float)(DateTime.UtcNow - TimeOfGrowthStart).TotalMilliseconds;
        return Mathf.Clamp(timeSinceStart / (GrowingUpDuration * 1000), 0f, 1f);
    }

    internal static bool IsGrownUp(PlayerControl player)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Mini mini = Players[i];
            if (mini.Player == player) return mini.GrowingProgress() == 1f;
        }

        return true;
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
        TriggerMiniLose = false;
        GrowingUpDuration = CustomOptionHolder.MiniGrowingUpDuration.GetFloat();
    }
}