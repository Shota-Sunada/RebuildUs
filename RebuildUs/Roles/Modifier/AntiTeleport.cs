namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class AntiTeleport : ModifierBase<AntiTeleport>
{
    public static Color NameColor = Palette.Orange;
    public override Color ModifierColor => NameColor;
    public static Vector3 position = new();
    public static List<PlayerControl> candidates
    {
        get
        {
            List<PlayerControl> validPlayers = [];

            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!player.HasModifier(ModifierType.AntiTeleport))
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }
    }
    public static string postfix
    {
        get
        {
            return Tr.Get("antiTeleportPostfix");
        }
    }
    public static string fullName
    {
        get
        {
            return Tr.Get("antiTeleport");
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
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        position = new Vector3();
    }
}
