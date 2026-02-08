namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerPhysicsPatch
{
    private static Vector2 _offset = Vector2.zero;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static void CoSpawnPlayerPostfix()
    {
        CustomOption.CoSpawnSyncSettings();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    public static void WalkPlayerToPrefix(PlayerPhysics __instance)
    {
        var isMini = __instance.myPlayer.HasModifier(ModifierType.Mini);
        var isMorphedToMini = Morphing.Exists && __instance.myPlayer.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTarget.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f;

        var correctOffset = Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (isMini || isMorphedToMini);

        if (correctOffset)
        {
            foreach (var morph in Morphing.Players)
            {
                if (morph.Player.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f)
                {
                    correctOffset = false;
                    break;
                }
            }
        }

        if (correctOffset)
        {
            var mini = Mini.GetModifier(__instance.myPlayer);
            if (mini == null) return;
            var currentScaling = (mini.GrowingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.DEFAULT_COLLIDER_OFFSET * Vector2.down;
        }
    }
}
