namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlayerPhysicsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    internal static void CoSpawnPlayerPostfix()
    {
        CustomOption.CoSpawnSyncSettings();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    internal static void WalkPlayerToPrefix(PlayerPhysics __instance)
    {
        bool isMini = __instance.myPlayer.HasModifier(ModifierType.Mini);
        bool isMorphedToMini = Morphing.Exists
                               && __instance.myPlayer.IsRole(RoleType.Morphing)
                               && Morphing.MorphTarget != null
                               && Morphing.MorphTarget.HasModifier(ModifierType.Mini)
                               && Morphing.MorphTimer > 0f;

        bool correctOffset = Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (isMini || isMorphedToMini);

        if (correctOffset)
        {
            foreach (Morphing morph in Morphing.Players)
            {
                if (!morph.Player.HasModifier(ModifierType.Mini) || !(Morphing.MorphTimer > 0f))
                {
                    continue;
                }
                correctOffset = false;
                break;
            }
        }

        if (!correctOffset)
        {
            return;
        }
        Mini mini = Mini.GetModifier(__instance.myPlayer);
        if (mini == null)
        {
            return;
        }
        float currentScaling = (mini.GrowingProgress() + 1) * 0.5f;
        __instance.myPlayer.Collider.offset = currentScaling * Mini.DEFAULT_COLLIDER_OFFSET * Vector2.down;
    }
}