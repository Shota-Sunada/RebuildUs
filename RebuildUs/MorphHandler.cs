using System.Diagnostics;

namespace RebuildUs;

public static class MorphHandler
{
    public static void MorphToPlayer(this PlayerControl pc, PlayerControl target)
    {
        SetOutfit(pc, target.Data.DefaultOutfit, target.Visible);
    }

    public static void SetOutfit(this PlayerControl morphing, NetworkedPlayerInfo.PlayerOutfit outfit, bool visible = true)
    {
#if DEBUG
        StackFrame stack1 = new(1);
        StackFrame stack2 = new(2);
        Logger.LogInfo($"{morphing?.GetNameWithRole()} => {outfit?.PlayerName} at {stack1.GetMethod().Name} at {stack2.GetMethod().Name}", "setOutfit");
#endif

        morphing.Data.Outfits[PlayerOutfitType.Shapeshifted] = outfit;
        morphing.CurrentOutfitType = PlayerOutfitType.Shapeshifted;

        morphing.RawSetColor(outfit.ColorId);
        morphing.RawSetVisor(outfit.VisorId, outfit.ColorId);
        morphing.RawSetHat(outfit.HatId, outfit.ColorId);
        morphing.RawSetName(outfit.PlayerName);

        SkinViewData nextSkin = null;
        try { nextSkin = MapUtilities.CachedShipStatus.CosmeticsCache.GetSkin(outfit.SkinId); } catch { return; }
        var phys = morphing.MyPhysics;
        var group = phys.Animations.group;
        var currentAnim = phys.Animations.Animator.GetCurrentAnimation();

        AnimationClip clip = currentAnim switch
        {
            var a when a == group.RunAnim => nextSkin.RunAnim,
            var a when a == group.SpawnAnim => nextSkin.SpawnAnim,
            var a when a == group.EnterVentAnim => nextSkin.EnterVentAnim,
            var a when a == group.ExitVentAnim => nextSkin.ExitVentAnim,
            _ => nextSkin.IdleAnim
        };

        var spriteAnim = phys.myPlayer.cosmetics.skin.animator;
        float progress = phys.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        phys.myPlayer.cosmetics.skin.skin = nextSkin;
        phys.myPlayer.cosmetics.skin.UpdateMaterial();

        spriteAnim.Play(clip, 1f);
        spriteAnim.m_animator.Play("a", 0, progress % 1);
        spriteAnim.m_animator.Update(0f);

        morphing.RawSetPet(outfit.PetId, outfit.ColorId);
    }

    public static void ResetMorph(this PlayerControl pc)
    {
        MorphToPlayer(pc, pc);
        // Munou.reMorph(pc.PlayerId);
        pc.CurrentOutfitType = PlayerOutfitType.Default;
    }
}