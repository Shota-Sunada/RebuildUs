namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class UseButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    internal static bool SetTargetPrefix(UseButton __instance, [HarmonyArgument(0)] IUsable target)
    {
        var pc = PlayerControl.LocalPlayer;
        __instance.enabled = true;

        if (Helpers.IsBlocked(target, pc))
        {
            __instance.currentTarget = null;
            __instance.buttonLabelText.text = Tr.Get(TrKey.ButtonBlocked);
            __instance.enabled = false;
            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 0f);
            return false;
        }

        __instance.currentTarget = target;
        return true;
    }
}