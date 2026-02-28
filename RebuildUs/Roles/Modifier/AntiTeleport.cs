namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
[RegisterModifier(ModifierType.AntiTeleport, typeof(AntiTeleport), nameof(NameColor), nameof(CustomOptionHolder.AntiTeleportSpawnRate))]
internal class AntiTeleport : ModifierBase<AntiTeleport>
{
    internal static Color NameColor = Palette.Orange;

    internal static Vector3 Position;

    public AntiTeleport()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.AntiTeleport;
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
            {
                if (!player.HasModifier(ModifierType.AntiTeleport))
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }
    }

    internal static string Postfix
    {
        get => Tr.Get(TrKey.AntiTeleportPostfix);
    }

    internal static string FullName
    {
        get => Tr.Get(TrKey.AntiTeleport);
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
        Position = new();
    }
}