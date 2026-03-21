namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
[RegisterModifier(ModifierType.AntiTeleport, typeof(AntiTeleport), nameof(CustomOptionHolder.AntiTeleportSpawnRate))]
internal class AntiTeleport : ModifierBase<AntiTeleport>
{
    public static Color Color = Palette.Orange;

    internal static Vector3 Position;

    public AntiTeleport()
    {
        StaticModifierType = CurrentModifierType = ModifierType.AntiTeleport;
    }

    internal static List<PlayerControl> Candidates
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

    internal static string Postfix { get => Tr.Get(TrKey.AntiTeleportPostfix); }
    internal static string FullName { get => Tr.Get(TrKey.AntiTeleport); }

    internal static void Clear()
    {
        Players.Clear();
        Position = new();
    }
}