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
        StackFrame stack1 = new(1);
        StackFrame stack2 = new(2);
        Logger.LogInfo($"{morphing?.GetNameWithRole()} => {outfit?.PlayerName} at {stack1.GetMethod().Name} at {stack2.GetMethod().Name}", "setOutfit");

        morphing.Data.Outfits[PlayerOutfitType.Shapeshifted] = outfit;
        morphing.CurrentOutfitType = PlayerOutfitType.Shapeshifted;

        morphing.RawSetColor(outfit.ColorId);
        morphing.RawSetVisor(outfit.VisorId, outfit.ColorId);
        morphing.RawSetHat(outfit.HatId, outfit.ColorId);
        morphing.RawSetName(outfit.PlayerName);

        SkinViewData nextSkin = null;
        try { nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(outfit.SkinId); } catch { return; }
        var playerPhysics = morphing.MyPhysics;
        AnimationClip clip = null;
        var spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();

        if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim) clip = nextSkin.RunAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim) clip = nextSkin.SpawnAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim) clip = nextSkin.EnterVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim) clip = nextSkin.ExitVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim) clip = nextSkin.IdleAnim;
        else clip = nextSkin.IdleAnim;

        float progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
        playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

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