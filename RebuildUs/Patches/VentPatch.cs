namespace RebuildUs.Patches;

[HarmonyPatch]
public static class VentPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static bool CanUsePrefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
    {
        float num = float.MaxValue;
        PlayerControl @object = pc.Object;

        bool roleCouldUse = @object.roleCanUseVents();

        if (__instance.name.StartsWith("SealedVent_"))
        {
            canUse = couldUse = false;
            __result = num;
            return false;
        }

        // Submerged Compatibility if needed:
        if (SubmergedCompatibility.IsSubmerged)
        {
            // as submerged does, only change stuff for vents 9 and 14 of submerged. Code partially provided by AlexejheroYTB
            if (SubmergedCompatibility.GetInTransition())
            {
                __result = float.MaxValue;
                return canUse = couldUse = false;
            }
            switch (__instance.Id)
            {
                case 9:  // Cannot enter vent 9 (Engine Room Exit Only Vent)!
                    if (CachedPlayer.LocalPlayer.PlayerControl.inVent) break;
                    __result = float.MaxValue;
                    return canUse = couldUse = false;
                case 14: // Lower Central
                    __result = float.MaxValue;
                    couldUse = roleCouldUse && !pc.IsDead && (@object.CanMove || @object.inVent);
                    canUse = couldUse;
                    if (canUse)
                    {
                        Vector3 center = @object.Collider.bounds.center;
                        Vector3 position = __instance.transform.position;
                        __result = Vector2.Distance(center, position);
                        canUse &= __result <= __instance.UsableDistance;
                    }
                    return false;
            }
        }

        var usableDistance = __instance.UsableDistance;
        if (__instance.name.StartsWith("JackInTheBoxVent_"))
        {
            if (Trickster.trickster != CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.PlayerControl.isGM())
            {
                // Only the Trickster can use the Jack-In-The-Boxes!
                canUse = false;
                couldUse = false;
                __result = num;
                return false;
            }
            else
            {
                // Reduce the usable distance to reduce the risk of gettings stuck while trying to jump into the box if it's placed near objects
                usableDistance = 0.4f;
            }
        }
        else if (__instance.name.StartsWith("SealedVent_"))
        {
            canUse = couldUse = false;
            __result = num;
            return false;
        }

        couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
        canUse = couldUse;
        if (canUse)
        {
            Vector2 truePosition = @object.GetTruePosition();
            Vector3 position = __instance.transform.position;
            num = Vector2.Distance(truePosition, position);

            canUse &= num <= usableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false);
        }
        __result = num;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    public static bool DoClickPrefix(VentButton __instance)
    {
        // Manually modifying the VentButton to use Vent.Use again in order to trigger the Vent.Use prefix patch
        __instance.currentTarget?.Use();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
    public static bool UsePrefix(Vent __instance)
    {
        __instance.CanUse(CachedPlayer.LocalPlayer.PlayerControl.Data, out bool canUse, out bool couldUse);
        bool canMoveInVents = CachedPlayer.LocalPlayer.PlayerControl != Spy.spy && !CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Madmate) && !CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.CreatedMadmate);
        if (!canUse) return false; // No need to execute the native method as using is disallowed anyways

        bool isEnter = !CachedPlayer.LocalPlayer.PlayerControl.inVent;

        if (__instance.name.StartsWith("JackInTheBoxVent_"))
        {
            __instance.SetButtons(isEnter && canMoveInVents);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UseUncheckedVent, Hazel.SendOption.Reliable);
            writer.WritePacked(__instance.Id);
            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
            writer.Write(isEnter ? byte.MaxValue : (byte)0);
            writer.EndMessage();
            RPCProcedure.useUncheckedVent(__instance.Id, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, isEnter ? byte.MaxValue : (byte)0);
            return false;
        }

        if (isEnter)
        {
            CachedPlayer.LocalPlayer.PlayerControl.MyPhysics.RpcEnterVent(__instance.Id);
        }
        else
        {
            CachedPlayer.LocalPlayer.PlayerControl.MyPhysics.RpcExitVent(__instance.Id);
        }
        __instance.SetButtons(isEnter && canMoveInVents);
        return false;
    }

    // disable vent animation
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    public static bool EnterVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    public static bool ExitVentPrefix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        return !CustomOptionHolder.DisableVentAnimation.GetBool() || pc.AmOwner;
    }
}