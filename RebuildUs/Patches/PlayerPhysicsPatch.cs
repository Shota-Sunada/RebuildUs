using RebuildUs.Roles.Impostor;

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

    private static Vector2 offset = Vector2.zero;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    public static void WalkPlayerToPrefix(PlayerPhysics __instance)
    {
        bool correctOffset = Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (__instance.myPlayer.HasModifier(ModifierType.Mini) || (Morphing.Exists && __instance.myPlayer.IsRole(RoleType.Morphing) && Morphing.morphTarget.HasModifier(ModifierType.Mini) && Morphing.morphTimer > 0f));
        correctOffset = correctOffset && !(Mini.mini == Morphling.morphling && Morphing.morphTimer > 0f);
        if (correctOffset)
        {
            float currentScaling = (Mini.growingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
        }
    }
}