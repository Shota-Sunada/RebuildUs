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
        bool correctOffset = Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (__instance.myPlayer.HasModifier(ModifierType.Mini) || (Morphing.Exists && __instance.myPlayer.IsRole(RoleType.Morphing) && Morphing.MorphTarget.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f));
        foreach (var morph in Morphing.Players)
        {
            correctOffset = correctOffset && !(morph.Player.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f);
        }
        if (correctOffset)
        {
            Mini mini = Mini.Players.First(x => x.Player == __instance.myPlayer);
            if (mini == null) return;
            float currentScaling = (mini.GrowingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.DefaultColliderOffset * Vector2.down;
        }
    }
}