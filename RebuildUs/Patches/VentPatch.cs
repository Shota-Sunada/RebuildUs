namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class VentPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    internal static bool CanUsePrefix(Vent __instance,
                                      ref float __result,
                                      [HarmonyArgument(0)] NetworkedPlayerInfo pc,
                                      [HarmonyArgument(1)] out bool canUse,
                                      [HarmonyArgument(2)] out bool couldUse)
    {
        var num = float.MaxValue;
        var @object = pc.Object;

        var roleCouldUse = @object.CanUseVents();
        var name = __instance.name;

        if (name.StartsWith("SealedVent_"))
        {
            canUse = couldUse = false;
            __result = num;
            return false;
        }

        // Submerged Compatibility if needed:
        if (SubmergedCompatibility.IsSubmerged)
        {
            if (SubmergedCompatibility.GetInTransition())
            {
                __result = float.MaxValue;
                canUse = couldUse = false;
                return false;
            }

            switch (__instance.Id)
            {
                case 9: // Cannot enter vent 9 (Engine Room Exit Only Vent)!
                    if (PlayerControl.LocalPlayer.inVent)
                    {
                        break;
                    }
                    __result = float.MaxValue;
                    canUse = couldUse = false;
                    return false;
                case 14: // Lower Central
                    __result = float.MaxValue;
                    couldUse = roleCouldUse && !pc.IsDead && (@object.CanMove || @object.inVent);
                    canUse = couldUse;
                    if (!canUse)
                    {
                        return false;
                    }
                    var center = @object.Collider.bounds.center;
                    var position = __instance.transform.position;
                    __result = Vector2.Distance(center, position);
                    canUse &= __result <= __instance.UsableDistance;

                    return false;
            }
        }

        var usableDistance = __instance.UsableDistance;
        if (name.StartsWith("JackInTheBoxVent_"))
        {
            var lp = PlayerControl.LocalPlayer;
            if (lp != null && !lp.IsRole(RoleType.Trickster) && !lp.IsGm())
            {
                // Only the Trickster can use the Jack-In-The-Boxes!
                canUse = false;
                couldUse = false;
                __result = num;
                return false;
            }

            // Reduce the usable distance to reduce the risk of gettings stuck while trying to jump into the box if it's placed near objects
            usableDistance = 0.4f;
        }

        couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
        canUse = couldUse;
        if (canUse)
        {
            var truePosition = @object.GetTruePosition();
            var position = __instance.transform.position;
            num = Vector2.Distance(truePosition, position);

            canUse &= num <= usableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false);
        }

        __result = num;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    internal static bool DoClickPrefix(VentButton __instance)
    {
        // Manually modifying the VentButton to use Vent.Use again in order to trigger the Vent.Use prefix patch
        __instance.currentTarget?.Use();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
    internal static bool UsePrefix(Vent __instance)
    {
        var lp = PlayerControl.LocalPlayer;
        if (lp == null)
        {
            return false;
        }

        __instance.CanUse(lp.Data, out var canUse, out var _);
        if (!canUse)
        {
            return false; // No need to execute the native method as using is disallowed anyways
        }

        var isEnter = !lp.inVent;

        if (__instance.name.StartsWith("JackInTheBoxVent_"))
        {
            __instance.SetButtons(isEnter && lp.CanMoveInVents());
            {
                using RPCSender sender = new(lp.NetId, CustomRPC.UseUncheckedVent);
                sender.WritePacked(__instance.Id);
                sender.Write(lp.PlayerId);
                sender.Write(isEnter ? byte.MaxValue : (byte)0);
                RPCProcedure.UseUncheckedVent(__instance.Id, lp.PlayerId, isEnter ? byte.MaxValue : (byte)0);
            }
            return false;
        }

        if (isEnter)
        {
            lp.MyPhysics.RpcEnterVent(__instance.Id);
        }
        else
        {
            lp.MyPhysics.RpcExitVent(__instance.Id);
        }

        __instance.SetButtons(isEnter && lp.CanMoveInVents());
        return false;
    }

    // disable vent animation
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    internal static bool EnterVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    internal static bool ExitVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }
}