namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class AntiTeleport : ModifierBase<AntiTeleport>
{
    public static Color NameColor = Palette.Orange;
    public override Color ModifierColor => NameColor;
    public static Vector3 Position = new();
    public static List<PlayerControl> Candidates
    {
        get
        {
            List<PlayerControl> validPlayers = [];
            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var player = allPlayers[i];
                if (!player.HasModifier(ModifierType.AntiTeleport))
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }
    }
    public static string Postfix
    {
        get
        {
            return Tr.Get("Hud.AntiTeleportPostfix");
        }
    }
    public static string FullName
    {
        get
        {
            return Tr.Get("Modifier.AntiTeleport");
        }
    }

    public AntiTeleport()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.AntiTeleport;
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
        Position = new Vector3();
    }
}