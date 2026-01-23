namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerPhysicsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static void CoSpawnPlayerPostfix()
    {
        CustomOption.CoSpawnSyncSettings();
    }

    private static Vector2 Offset = Vector2.zero;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    public static void WalkPlayerToPrefix(PlayerPhysics __instance)
    {
        bool isMini = __instance.myPlayer.HasModifier(ModifierType.Mini);
        bool isMorphedToMini = Morphing.Exists && __instance.myPlayer.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTarget.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f;

        bool correctOffset = Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (isMini || isMorphedToMini);

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
            Mini mini = Mini.GetRole(__instance.myPlayer);
            if (mini == null) return;
            float currentScaling = (mini.GrowingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.DefaultColliderOffset * Vector2.down;
        }
    }
}