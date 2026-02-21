using RebuildUs.Modules.Cosmetics.Extensions;

namespace RebuildUs.Modules.Cosmetics.Patches;

[HarmonyPatch(typeof(PlayerPhysics))]
internal static class PlayerPhysicsPatches
{
    [HarmonyPatch(nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPostfix]
    private static void HandleAnimationPostfix(PlayerPhysics __instance)
    {
        AnimationClip currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
        if (currentAnimation == __instance.Animations.group.ClimbUpAnim) return;
        if (currentAnimation == __instance.Animations.group.ClimbDownAnim) return;
        HatParent hatParent = __instance.myPlayer.cosmetics.hat;
        if (hatParent == null) return;
        if (!hatParent.TryGetCached(out HatViewData viewData)) return;
        HatExtension extend = hatParent.Hat.GetHatExtension();
        if (extend == null) return;
        if (extend.FlipImage != null) hatParent.FrontLayer.sprite = __instance.FlipX ? extend.FlipImage : viewData.MainImage;

        if (extend.BackFlipImage == null) return;
        hatParent.BackLayer.sprite = __instance.FlipX ? extend.BackFlipImage : viewData.BackImage;
    }
}