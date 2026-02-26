namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class KillAnimationPatch
{
    internal static bool HideNextAnimation;
    internal static bool AvoidNextKillMovement;

    private static int? _colorId;
    private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");

    internal static IEnumerator CoPerformKill(KillAnimation __instance, PlayerControl source, PlayerControl target)
    {
        Logger.LogMessage("CoPerformKill is overridden!");

        if (HideNextAnimation)
        {
            source = target;
            HideNextAnimation = false;
        }

        if (Camera.main == null) yield break;
        FollowerCamera cam = Camera.main.GetComponent<FollowerCamera>();
        bool isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
        PlayerPhysics sourcePhys = source.MyPhysics;
        KillAnimation.SetMovement(source, false);
        KillAnimation.SetMovement(target, false);
        if (isParticipant)
        {
            PlayerControl.LocalPlayer.isKilling = true;
            source.isKilling = true;
        }

        DeadBody deadBody = UnityObject.Instantiate(GameManager.Instance.GetDeadBody(source.Data.Role));
        deadBody.enabled = false;
        deadBody.ParentId = target.PlayerId;
        foreach (SpriteRenderer b in deadBody.bodyRenderers) target.SetPlayerMaterialColors(b);

        target.SetPlayerMaterialColors(deadBody.bloodSplatter);
        Vector3 vector3 = target.transform.position + __instance.BodyOffset;
        vector3.z = vector3.y / 1000f;
        deadBody.transform.position = vector3;
        source.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
        target.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
        if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Detective && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected) (PlayerControl.LocalPlayer.Data.Role as DetectiveRole)?.KillAnimSpecialSetup(deadBody, source, target);

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
        if (!isParticipant) yield break;
        cam.Locked = false;
        PlayerControl.LocalPlayer.isKilling = false;
        source.isKilling = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    internal static void SetMovementPrefix(PlayerControl source, bool canMove)
    {
        Color color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor(BodyColor);
        if (!Morphing.Exists || !source.IsRole(RoleType.Morphing)) return;
        int index = Palette.PlayerColors.IndexOf(color);
        if (index != -1) _colorId = index;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement), typeof(PlayerControl), typeof(bool))]
    internal static void Postfix(PlayerControl source, bool canMove)
    {
        if (_colorId.HasValue) source.RawSetColor(_colorId.Value);
        _colorId = null;
    }
}