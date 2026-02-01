namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class Mini : ModifierBase<Mini>
{
    public static Color NameColor = Color.yellow;
    public override Color ModifierColor => NameColor;

    public static List<PlayerControl> Candidates
    {
        get
        {
            List<PlayerControl> validPlayers = [];
            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var player = allPlayers[i];
                if (!player.HasModifier(ModifierType.Mini))
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }
    }

    public const float DefaultColliderRadius = 0.2233912f;
    public const float DefaultColliderOffset = 0.3636057f;

    public static float GrowingUpDuration = 400f;
    public DateTime TimeOfGrowthStart = DateTime.UtcNow;
    public static bool TriggerMiniLose = false;

    public float GrowingProgress()
    {
        float timeSinceStart = (float)(DateTime.UtcNow - TimeOfGrowthStart).TotalMilliseconds;
        return Mathf.Clamp(timeSinceStart / (GrowingUpDuration * 1000), 0f, 1f);
    }

    public static bool IsGrownUp(PlayerControl player)
    {
        for (var i = 0; i < Players.Count; i++)
        {
            var mini = Players[i];
            if (mini.Player == player)
            {
                return mini.GrowingProgress() == 1f;
            }
        }
        return true;
    }
    public static string Postfix
    {
        get
        {
            return Tr.Get("MiniPostfix");
        }
    }
    public static string FullName
    {
        get
        {
            return Tr.Get("Mini");
        }
    }

    public Mini()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.Mini;
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
        TriggerMiniLose = false;
        GrowingUpDuration = CustomOptionHolder.MiniGrowingUpDuration.GetFloat();
    }
}
