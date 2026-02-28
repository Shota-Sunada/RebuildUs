using System.Diagnostics;
using PowerTools;

namespace RebuildUs;

internal static class MorphHandler
{
    internal static void MorphToPlayer(this PlayerControl pc, PlayerControl target)
    {
        pc.SetOutfit(target.Data.DefaultOutfit, target.Visible);
    }

    internal static void SetOutfit(this PlayerControl morphing, NetworkedPlayerInfo.PlayerOutfit outfit, bool visible = true)
    {
#if DEBUG
        StackFrame stack1 = new(1);
        StackFrame stack2 = new(2);
        Logger.LogInfo($"{morphing?.GetNameWithRole()} => {outfit?.PlayerName} at {stack1.GetMethod()?.Name} at {stack2.GetMethod()?.Name}",
            "setOutfit");
#endif

        morphing.Data.Outfits[PlayerOutfitType.Shapeshifted] = outfit;
        morphing.CurrentOutfitType = PlayerOutfitType.Shapeshifted;

        morphing.RawSetColor(outfit.ColorId);
        morphing.RawSetVisor(outfit.VisorId, outfit.ColorId);
        morphing.RawSetHat(outfit.HatId, outfit.ColorId);
        morphing.RawSetName(outfit.PlayerName);

        SkinViewData nextSkin = null;
        try
        {
            nextSkin = MapUtilities.CachedShipStatus.CosmeticsCache.GetSkin(outfit.SkinId);
        }
        catch
        {
            return;
        }

        PlayerPhysics phys = morphing.MyPhysics;
        PlayerAnimationGroup group = phys.Animations.group;
        AnimationClip currentAnim = phys.Animations.Animator.GetCurrentAnimation();

        AnimationClip clip = currentAnim switch
        {
            var a when a == group.RunAnim => nextSkin.RunAnim,
            var a when a == group.SpawnAnim => nextSkin.SpawnAnim,
            var a when a == group.EnterVentAnim => nextSkin.EnterVentAnim,
            var a when a == group.ExitVentAnim => nextSkin.ExitVentAnim,
            _ => nextSkin.IdleAnim,
        };

        SpriteAnim spriteAnim = phys.myPlayer.cosmetics.skin.animator;
        float progress = phys.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        phys.myPlayer.cosmetics.skin.skin = nextSkin;
        phys.myPlayer.cosmetics.skin.UpdateMaterial();

        spriteAnim.Play(clip);
        spriteAnim.m_animator.Play("a", 0, progress % 1);
        spriteAnim.m_animator.Update(0f);

        morphing.RawSetPet(outfit.PetId, outfit.ColorId);
    }

    internal static void ResetMorph(this PlayerControl pc)
    {
        pc.MorphToPlayer(pc);
        // Munou.reMorph(pc.PlayerId);
        pc.CurrentOutfitType = PlayerOutfitType.Default;
    }
}