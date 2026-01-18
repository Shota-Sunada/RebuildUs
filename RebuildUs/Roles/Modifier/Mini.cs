namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class Mini : ModifierBase<Mini>
{
    public static Color NameColor = Color.yellow;
    public override Color ModifierColor => NameColor;

    public static List<PlayerControl> candidates
    {
        get
        {
            List<PlayerControl> validPlayers = [];

            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!player.HasModifier(ModifierType.Mini))
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }
    }

    public const float defaultColliderRadius = 0.2233912f;
    public const float defaultColliderOffset = 0.3636057f;

    public static float growingUpDuration = 400f;
    public DateTime timeOfGrowthStart = DateTime.UtcNow;
    public static bool triggerMiniLose = false;

    public float growingProgress()
    {
        float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
        return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
    }

    public static bool isGrownUp(PlayerControl player)
    {
        Mini mini = Players.First(x => x.Player == player);
        if (mini == null) return true;
        return mini.growingProgress() == 1f;
    }
    public static string postfix
    {
        get
        {
            return Tr.Get("miniPostfix");
        }
    }
    public static string fullName
    {
        get
        {
            return Tr.Get("mini");
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
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        triggerMiniLose = false;
        growingUpDuration = CustomOptionHolder.miniGrowingUpDuration.GetFloat();
    }
}
