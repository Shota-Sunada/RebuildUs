using System.Collections;
using Object = UnityEngine.Object;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class KillAnimationPatch
{
    public static bool HideNextAnimation;
    public static bool AvoidNextKillMovement;

    private static int? _colorId;

    public static IEnumerator CoPerformKill(KillAnimation __instance, PlayerControl source, PlayerControl target)
    {
        Logger.LogMessage("CoPerformKill is overridden!");

        if (HideNextAnimation)
        {
            source = target;
            HideNextAnimation = false;
        }

        var cam = Camera.main.GetComponent<FollowerCamera>();
        var isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
        var sourcePhys = source.MyPhysics;
        KillAnimation.SetMovement(source, false);
        KillAnimation.SetMovement(target, false);
        if (isParticipant)
        {
            PlayerControl.LocalPlayer.isKilling = true;
            source.isKilling = true;
        }

        var deadBody = Object.Instantiate(GameManager.Instance.GetDeadBody(source.Data.Role));
        deadBody.enabled = false;
        deadBody.ParentId = target.PlayerId;
        foreach (var b in deadBody.bodyRenderers) target.SetPlayerMaterialColors(b);
        target.SetPlayerMaterialColors(deadBody.bloodSplatter);
        var vector3 = target.transform.position + __instance.BodyOffset;
        vector3.z = vector3.y / 1000f;
        deadBody.transform.position = vector3;
        source.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
        target.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
        if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Detective && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected) (PlayerControl.LocalPlayer.Data.Role as DetectiveRole).KillAnimSpecialSetup(deadBody, source, target);
        if (isParticipant)
        {
            cam.Locked = true;
            ConsoleJoystick.SetMode_Task();
            if (PlayerControl.LocalPlayer.AmOwner) PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
        }

        target.Die(DeathReason.Kill, true);
        yield return source.MyPhysics.Animations.CoPlayCustomAnimation(__instance.BlurAnim);
        if (AvoidNextKillMovement)
            AvoidNextKillMovement = false;
        else
            source.NetTransform.SnapTo(target.transform.position);
        sourcePhys.Animations.PlayIdleAnimation();
        KillAnimation.SetMovement(source, true);
        KillAnimation.SetMovement(target, true);
        deadBody.enabled = true;
        if (isParticipant)
        {
            cam.Locked = false;
            PlayerControl.LocalPlayer.isKilling = false;
            source.isKilling = false;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    public static void SetMovementPrefix(PlayerControl source, bool canMove)
    {
        var color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        if (Morphing.Exists && source.IsRole(RoleType.Morphing))
        {
            var index = Palette.PlayerColors.IndexOf(color);
            if (index != -1) _colorId = index;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    public static void Postfix(PlayerControl source, bool canMove)
    {
        if (_colorId.HasValue) source.RawSetColor(_colorId.Value);
        _colorId = null;
    }
}
